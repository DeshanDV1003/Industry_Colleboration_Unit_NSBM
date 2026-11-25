"""
End-to-end API regression suite for every delivered feature (milestones 1-13).

Resets the demo data first, walks each portal's journeys, and asserts results and role permissions.
Run with the API on http://localhost:5300 and Demo:Enabled = true:

    python tests/api/regression_test.py

Exit code is 0 only when every check passes. Nothing outside the demo data is touched.
"""
import datetime
import json
import sys
import urllib.error
import urllib.request

API = "http://localhost:5300/api/v1"
TODAY = datetime.date.today()
results: list[tuple[str, bool, str]] = []


# ---------------------------------------------------------------- helpers
def call(method, path, token=None, body=None):
    req = urllib.request.Request(API + path, method=method, data=None if body is None else json.dumps(body).encode())
    req.add_header("Content-Type", "application/json")
    if token:
        req.add_header("Authorization", "Bearer " + token)
    try:
        with urllib.request.urlopen(req, timeout=30) as r:
            raw = r.read()
            return r.status, (json.loads(raw) if raw else None)
    except urllib.error.HTTPError as e:
        raw = e.read()
        try:
            return e.code, (json.loads(raw) if raw else None)
        except ValueError:
            return e.code, raw.decode(errors="replace")


def check(name, condition, detail=""):
    results.append((name, bool(condition), "" if condition else str(detail)[:300]))


def expect(name, response, status):
    check(f"{name} -> {status}", response[0] == status, f"got {response[0]}: {response[1]}")
    return response[1]


def login(email, password):
    status, body = call("POST", "/auth/login", body={"email": email, "password": password})
    if status != 200:
        sys.exit(f"Cannot sign in as {email}: {status} {body}")
    return body["accessToken"]


def section(title):
    results.append((f"== {title}", True, ""))


# ---------------------------------------------------------------- setup
import time
for _ in range(60):  # wait for a freshly started API
    try:
        if call("GET", "/skills")[0] == 401:
            break
    except OSError:
        pass
    time.sleep(2)

adm = login("icu.head@nsbm.demo", "Admin@123")
expect("demo reset", call("POST", "/settings/demo-reset", adm), 204)

stu = login("student@nsbm.demo", "Student@123")          # Sahan, DEMO-001
kav = login("demo-002@nsbm.demo", "Student@123")         # Kavindi, cohort student
lec = login("lecturer@nsbm.demo", "Lecturer@123")
lanka = login("partner@nsbm.demo", "Industry@123")       # Lanka CloudWorks
ser = login("partner2@nsbm.demo", "Industry@123")        # Serendib Software Labs
qa = login("partner3@nsbm.demo", "Industry@123")         # Ceylon QualityFirst
reg = login("registrar@nsbm.demo", "Registrar@123")      # Assistant Registrar
ROLE_TOKENS = {"Student": stu, "Lecturer": lec, "Industry": lanka, "Admin": adm, "Registrar": reg}

# ---------------------------------------------------------------- M1 auth and role gates
section("M1 Auth and permissions")
expect("wrong password", call("POST", "/auth/login", body={"email": "student@nsbm.demo", "password": "nope"}), 401)
expect("no token", call("GET", "/auth/me"), 401)
for role, token in ROLE_TOKENS.items():
    me = expect(f"{role} /auth/me", call("GET", "/auth/me", token), 200)
    check(f"{role} role claim", me and me.get("role") == role, me)
for role in ROLE_TOKENS:
    s, b = call("POST", "/auth/demo-login", body={"role": role})
    check(f"demo-login {role}", s == 200 and b.get("accessToken"), (s, b))

# Every read endpoint: allowed roles get 200, everyone else 403.
year = TODAY.year
ALL_ROLES = set(ROLE_TOKENS)
READS = {
    "/career-paths": ALL_ROLES,
    "/skills": ALL_ROLES,
    f"/calendar/{year}": ALL_ROLES,
    "/me/notifications": ALL_ROLES,
    "/me/digest": ALL_ROLES,
    "/calendar/options": {"Admin"},
    "/dashboard/student": {"Student"},
    "/me/baseline": {"Student"},
    "/me/roadmap": {"Student"},
    "/me/readiness": {"Student"},
    "/me/profile/skills": {"Student"},
    "/practice/tasks": {"Student"},
    "/career-center/vacancies": {"Student"},
    "/career-center/applications": {"Student"},
    "/me/interviews/SoftSkills": {"Student"},
    "/me/interviews/Technical": {"Student"},
    "/me/interviews/report": {"Student"},
    "/me/cv": {"Student"},
    "/me/planner": {"Student"},
    "/mentoring/mine": {"Student"},
    "/me/sprint": {"Student"},
    "/me/portfolio": {"Student"},
    "/industry-tasks/mine": {"Industry"},
    "/vacancies/mine": {"Industry"},
    "/candidates/applications": {"Industry"},
    "/candidates/shared-profiles": {"Industry"},
    "/mentoring/queue": {"Industry"},
    "/task-reviews/attempts": {"Industry"},
    "/industry-tasks/review-queue": {"Admin"},
    "/vacancies/review-queue": {"Admin"},
    "/lecturer/students": {"Lecturer"},
    "/lecturer/assignable-tasks": {"Lecturer"},
    "/research/overview": {"Lecturer", "Admin"},
    "/events": {"Student"},
    "/events/certificates": {"Student"},
    "/industry-events/mine": {"Industry"},
    "/industry-events/review-queue": {"Admin"},
    "/me/ai-interviews": {"Student"},
    "/me/ai-interviews/availability": {"Student"},
    "/me/achievements": {"Student"},
    "/badge-reviews": {"Lecturer"},
    "/showcase": ALL_ROLES,
    "/partner-enquiries": {"Admin"},
    "/venues": {"Registrar", "Admin"},
    f"/venues/bookings?from={TODAY.isoformat()}&to={(TODAY + datetime.timedelta(days=30)).isoformat()}": {"Registrar", "Admin"},
    "/venues/booking-options": {"Registrar"},
    "/announcements": {"Admin"},
}
for path, allowed in READS.items():
    for role, token in ROLE_TOKENS.items():
        s, b = call("GET", path, token)
        want = 200 if role in allowed else 403
        # A student's readiness needs a career path first; 400 there is a correct business answer before M2 runs.
        ok = s == want or (path == "/me/readiness" and role == "Student" and s == 400)
        check(f"GET {path} as {role} -> {want}", ok, f"got {s}: {b}")

# ---------------------------------------------------------------- M2 catalogue, baseline, roadmap
section("M2 Career paths, baseline, roadmap")
paths = expect("career paths", call("GET", "/career-paths", stu), 200)
check("career paths seeded", paths and len(paths) >= 4, paths)
se = next(p for p in paths if p["slug"] == "software-engineering")
qa_path = next(p for p in paths if p["slug"] == "quality-assurance")
empty = expect("baseline before path", call("GET", "/me/baseline", stu), 200)
check("no baseline questions before a path", empty and empty["careerPathId"] is None and not empty["questions"], empty)
expect("select unknown path", call("POST", "/career-paths/999/selection", stu), 404)
expect("lecturer cannot select", call("POST", f"/career-paths/{qa_path['id']}/selection", lec), 403)
expect("select QA path", call("POST", f"/career-paths/{qa_path['id']}/selection", stu), 204)
detail = expect("path detail", call("GET", "/career-paths/quality-assurance", stu), 200)
check("path marked selected", detail and detail["isSelected"], detail)
expect("unknown slug", call("GET", "/career-paths/nope", stu), 404)
baseline = expect("baseline", call("GET", "/me/baseline", stu), 200)
check("baseline has questions", baseline and len(baseline["questions"]) > 0, baseline)
expect("partial baseline rejected", call("POST", "/me/baseline/attempts", stu,
       {"answers": [{"questionId": baseline["questions"][0]["id"], "optionId": baseline["questions"][0]["options"][0]["id"]}]}), 400)
result = expect("full baseline", call("POST", "/me/baseline/attempts", stu,
                {"answers": [{"questionId": q["id"], "optionId": q["options"][0]["id"]} for q in baseline["questions"]]}), 200)
check("baseline scored", result and result["questionCount"] == len(baseline["questions"]), result)
roadmap = expect("roadmap", call("GET", "/me/roadmap", stu), 200)
step = roadmap["steps"][0]["id"]
expect("complete step", call("PUT", f"/me/roadmap/steps/{step}/completion", stu, {"completed": True}), 204)
expect("complete step again (idempotent)", call("PUT", f"/me/roadmap/steps/{step}/completion", stu, {"completed": True}), 204)
check("roadmap progress", call("GET", "/me/roadmap", stu)[1]["completedCount"] == 1)
overview = expect("student overview", call("GET", "/dashboard/student", stu), 200)
check("overview shows path and progress", overview["careerPath"]["name"] == qa_path["name"] and overview["roadmap"]["completed"] == 1, overview)

# ---------------------------------------------------------------- M3 industry tasks
section("M3 Industry tasks")
queue = [t for t in expect("task review queue", call("GET", "/industry-tasks/review-queue", adm), 200) if t["status"] == "Pending"]
check("seeded pending tasks in queue", queue, queue)
library = expect("task library", call("GET", "/practice/tasks", stu), 200)
check("library has approved tasks", library and len(library) > 0, library)
if queue:
    expect("student cannot open pending task", call("GET", f"/practice/tasks/{queue[0]['id']}", stu), 404)
foreign = next(t for t in library if t["companyName"] != "Lanka CloudWorks (Pvt) Ltd")
expect("partner cannot read other company task", call("GET", f"/industry-tasks/{foreign['id']}", lanka), 403)
bad_task = {"title": "", "summary": "s", "instructions": "i", "careerPathId": qa_path["id"], "level": "Beginner", "situation": "Testing",
            "estimatedMinutes": 20, "starterLanguage": None, "starterCode": None, "skillIds": [],
            "checks": [{"description": "x", "pattern": "([unclosed", "shouldMatch": True, "hint": None}], "knowledgeQuestions": []}
s, b = call("POST", "/industry-tasks", qa, bad_task)
check("invalid task rejected with field errors", s == 400 and "errors" in b and any("Pattern" in k for k in b["errors"]), (s, b))
skills = {x["name"]: x["id"] for x in call("GET", "/skills", stu)[1]}
good_task = dict(bad_task, title="Regression: write a bug report", skillIds=[skills["SQL"]],
                 checks=[{"description": "Includes steps", "pattern": "(?i)steps", "shouldMatch": True, "hint": "List steps."}],
                 knowledgeQuestions=["What makes a bug report actionable?"])
new_task = expect("partner creates task", call("POST", "/industry-tasks", qa, good_task), 201)["id"]
check("admin notified of new task", any("Regression: write a bug report" in n["title"] for n in call("GET", "/me/notifications", adm)[1]["items"]))
expect("reject needs note", call("POST", f"/industry-tasks/{new_task}/review", adm, {"decision": "Reject", "note": ""}), 400)
expect("reject", call("POST", f"/industry-tasks/{new_task}/review", adm, {"decision": "Reject", "note": "Add expected results."}), 204)
check("partner notified of rejection", any(n["title"].startswith("Changes needed") for n in call("GET", "/me/notifications", qa)[1]["items"]))
expect("resubmit", call("PUT", f"/industry-tasks/{new_task}", qa, dict(good_task, summary="Updated summary")), 204)
check("resubmitted is pending", call("GET", f"/industry-tasks/{new_task}", adm)[1]["status"] == "Pending")
expect("approve", call("POST", f"/industry-tasks/{new_task}/review", adm, {"decision": "Approve", "note": None}), 204)
expect("approve twice", call("POST", f"/industry-tasks/{new_task}/review", adm, {"decision": "Approve", "note": None}), 400)
expect("edit approved", call("PUT", f"/industry-tasks/{new_task}", qa, good_task), 400)
check("approved task in library", any(t["id"] == new_task for t in call("GET", "/practice/tasks", stu)[1]))

fix_task = next(t for t in library if t["title"].startswith("Fix the order"))
wb = expect("workbench", call("GET", f"/practice/tasks/{fix_task['id']}", stu), 200)
check("check patterns hidden from students", "pattern" not in json.dumps(wb["task"]["checks"]))
expect("knowledge check before attempt", call("PUT", f"/practice/tasks/{fix_task['id']}/knowledge-check", stu, {"answers": []}), 400)
a1 = expect("starter attempt", call("POST", f"/practice/tasks/{fix_task['id']}/attempts", stu, {"code": wb["task"]["starterCode"]}), 200)
check("starter fails some checks", a1["passedChecks"] < a1["totalChecks"], a1)
fixed = wb["task"]["starterCode"].replace("total = total - discountPercent;",
                                          "total = total * (1 - discountPercent / 100);\n  total = Math.round(total * 100) / 100;")
a2 = expect("fixed attempt", call("POST", f"/practice/tasks/{fix_task['id']}/attempts", stu, {"code": fixed}), 200)
check("fixed attempt passes all", a2["passedChecks"] == a2["totalChecks"], a2)
expect("empty attempt", call("POST", f"/practice/tasks/{fix_task['id']}/attempts", stu, {"code": ""}), 400)
kc = expect("knowledge check", call("GET", f"/practice/tasks/{fix_task['id']}/knowledge-check", stu), 200)
expect("short answers", call("PUT", f"/practice/tasks/{fix_task['id']}/knowledge-check", stu,
       {"answers": [{"questionId": q["id"], "text": "short"} for q in kc["questions"]]}), 400)
expect("full answers", call("PUT", f"/practice/tasks/{fix_task['id']}/knowledge-check", stu,
       {"answers": [{"questionId": q["id"], "text": f"My detailed explanation for question {q['id']}."} for q in kc["questions"]]}), 204)
row = next(t for t in call("GET", "/practice/tasks", stu)[1] if t["id"] == fix_task["id"])
check("library shows progress", row["myAttemptCount"] == 2 and row["knowledgeCheckDone"], row)

# ---------------------------------------------------------------- M4 vacancies, readiness, candidates
section("M4 Vacancies, readiness, candidates")
expect("set listed skills", call("PUT", "/me/profile/skills", stu, {"skillIds": [skills["SQL"], skills["Communication"]]}), 204)
expect("unknown skill", call("PUT", "/me/profile/skills", stu, {"skillIds": [99999]}), 400)
readiness = expect("readiness", call("GET", "/me/readiness", stu), 200)
check("readiness has match percent", 0 <= readiness["matchPercent"] <= 100, readiness)
qa_vacs = expect("QA vacancies", call("GET", f"/career-center/vacancies?careerPathId={qa_path['id']}", stu), 200)
qa_vac = qa_vacs[0]
expect("short cover note", call("POST", f"/career-center/vacancies/{qa_vac['id']}/applications", stu, {"coverNote": "pls"}), 400)
expect("apply", call("POST", f"/career-center/vacancies/{qa_vac['id']}/applications", stu,
       {"coverNote": "I completed the QA practice tasks and would like to grow as a QA engineer."}), 201)
expect("apply twice", call("POST", f"/career-center/vacancies/{qa_vac['id']}/applications", stu,
       {"coverNote": "Applying a second time to the same vacancy here."}), 400)
apps = call("GET", "/candidates/applications", qa)[1]
sahan_app = next(a for a in apps if a["studentNo"] == "DEMO-001" and a["vacancyId"] == qa_vac["id"])
expect("evidence for applicant", call("GET", f"/candidates/evidence?studentId={sahan_app['studentId']}&vacancyId={qa_vac['id']}", qa), 200)
expect("other company cannot change status", call("PUT", f"/candidates/applications/{sahan_app['id']}/status", lanka, {"status": "Shortlisted"}), 403)
expect("cannot move back to Submitted", call("PUT", f"/candidates/applications/{sahan_app['id']}/status", qa, {"status": "Submitted"}), 400)
expect("invite to interview", call("PUT", f"/candidates/applications/{sahan_app['id']}/status", qa, {"status": "InterviewInvited"}), 204)
mine_apps = call("GET", "/career-center/applications", stu)[1]
check("student sees invitation", any(a["status"] == "InterviewInvited" for a in mine_apps), mine_apps)
check("student notified of status change", any(n["kind"] == "ApplicationUpdate" for n in call("GET", "/me/notifications", stu)[1]["items"]))
future = (TODAY + datetime.timedelta(days=30)).isoformat()
past = (TODAY - datetime.timedelta(days=1)).isoformat()
vac = {"title": "Regression Data Intern", "description": "Work with data.", "careerPathId": se["id"], "workMode": "Remote",
       "location": "Remote", "durationMonths": 6, "closingDate": past, "requiredSkillIds": [skills["SQL"]], "preferredSkillIds": []}
s, b = call("POST", "/vacancies", ser, vac)
check("past closing date rejected", s == 400 and "ClosingDate" in json.dumps(b), (s, b))
vid = expect("create vacancy", call("POST", "/vacancies", ser, dict(vac, closingDate=future)), 201)["id"]
check("pending vacancy hidden from students", not any(v["id"] == vid for v in call("GET", "/career-center/vacancies", stu)[1]))
expect("approve vacancy", call("POST", f"/vacancies/{vid}/review", adm, {"decision": "Approve", "note": None}), 204)
check("approved vacancy visible", any(v["id"] == vid for v in call("GET", "/career-center/vacancies", stu)[1]))
expect("other company cannot read vacancy", call("GET", f"/vacancies/{qa_vac['id']}", ser), 403)

# ---------------------------------------------------------------- M5 interviews
section("M5 Interview practice")
soft = expect("soft-skills questions", call("GET", "/me/interviews/SoftSkills", stu), 200)
tech = expect("technical questions", call("GET", "/me/interviews/Technical", stu), 200)
q = soft["questions"][0]
long_answer = ("In my second year our team of four had a deadline for a library system. My role was the backend. "
               "I made a plan with the team, we agreed tasks and I helped communicate progress every day. "
               "When one module was late I paired with a teammate. As a result we delivered on time.")
fb = expect("answer with feedback", call("POST", "/me/interviews/answers", stu, {"questionId": q["id"], "text": long_answer, "inputMode": "Voice"}), 200)
check("feedback has coverage", fb and fb["coveragePercent"] > 0, fb)
expect("technical answer", call("POST", "/me/interviews/answers", stu,
       {"questionId": tech["questions"][0]["id"], "text": "I start from requirements and risk, then add boundary cases.", "inputMode": "Typed"}), 200)
expect("empty answer", call("POST", "/me/interviews/answers", stu, {"questionId": q["id"], "text": "  ", "inputMode": "Typed"}), 400)
expect("unknown question", call("POST", "/me/interviews/answers", stu, {"questionId": 999999, "text": "x", "inputMode": "Typed"}), 404)
report = expect("interview report", call("GET", "/me/interviews/report", stu), 200)
check("report lists answers", len(report["items"]) >= 2, report)

# ---------------------------------------------------------------- M6 growth tools
section("M6 CV, planner, mentoring, reviews, sprint, portfolio")
expect("save CV", call("PUT", "/me/cv", stu, {"headline": "Aspiring QA engineer", "summary": "Third-year SE student.", "education": "BSc SE, NSBM",
       "projects": "Library system", "gitHubUrl": "https://github.com/sahan-demo", "linkedInUrl": "https://www.linkedin.com/in/sahan-demo"}), 204)
check("CV saved", call("GET", "/me/cv", stu)[1]["headline"] == "Aspiring QA engineer")
kw = expect("keyword finder", call("POST", "/me/cv/keywords", stu, {"text": "Used Git, Docker and SQL. Wrote unit tests."}), 200)
check("keywords found", any(k["name"] == "SQL" for k in kw), kw)
expect("deadline needs date", call("POST", "/me/planner", stu, {"kind": "Deadline", "title": "Submit CV", "notes": None, "dueDate": None}), 400)
overdue_day = (TODAY - datetime.timedelta(days=2)).isoformat()
pid = expect("add planner item", call("POST", "/me/planner", stu, {"kind": "Deadline", "title": "Regression deadline", "notes": None, "dueDate": overdue_day}), 200)["id"]
expect("mark done", call("PUT", f"/me/planner/{pid}/done", stu, {"done": True}), 204)
expect("mark undone", call("PUT", f"/me/planner/{pid}/done", stu, {"done": False}), 204)
expect("other student cannot touch item", call("PUT", f"/me/planner/{pid}/done", kav, {"done": True}), 404)
expect("short mentor question", call("POST", "/mentoring/questions", stu, {"title": "Q", "body": "short"}), 400)
qid = expect("ask mentor", call("POST", "/mentoring/questions", stu,
             {"title": "Which test tools should I learn first?", "body": "I know manual testing. Should I start with Playwright or Postman?"}), 200)["id"]
expect("mentor replies", call("POST", f"/mentoring/questions/{qid}/reply", qa, {"reply": "Start with Postman for API basics, then add Playwright."}), 204)
expect("second reply rejected", call("POST", f"/mentoring/questions/{qid}/reply", lanka, {"reply": "Another mentor reply that is long enough."}), 400)
check("student notified of mentor reply", any(n["kind"] == "MentorReply" for n in call("GET", "/me/notifications", stu)[1]["items"]))
# The fixed task belongs to one partner company; find whose review list holds the attempts.
owner = next(t for t in (lanka, ser, qa) if call("GET", "/task-reviews/attempts", t)[1])
outsider = lanka if owner is not lanka else ser
attempts = expect("attempts to review", call("GET", "/task-reviews/attempts", owner), 200)
if attempts:
    aid = attempts[0]["attemptId"]
    expect("other company cannot review", call("POST", f"/task-reviews/attempts/{aid}", outsider, {"comment": "Looks good overall.", "improvements": None}), 403)
    expect("company reviews attempt", call("POST", f"/task-reviews/attempts/{aid}", owner, {"comment": "Clear, well-structured work.", "improvements": "Add an edge case."}), 204)
else:
    check("a partner has attempts to review", False, "no attempts")
board = expect("sprint board", call("GET", "/me/sprint", stu), 200)
mine = next(i for i in board["items"] if i["canPickUp"])
other = next(i for i in board["items"] if not i["canPickUp"] and not i["isMine"])
expect("pick up card", call("PUT", f"/me/sprint/items/{mine['id']}", stu, {"status": "InProgress"}), 204)
expect("cannot move other role's card", call("PUT", f"/me/sprint/items/{other['id']}", stu, {"status": "Done"}), 400)
expect("finish card", call("PUT", f"/me/sprint/items/{mine['id']}", stu, {"status": "Done"}), 204)
portfolio = expect("portfolio", call("GET", "/me/portfolio", stu), 200)
check("portfolio reflects activity", portfolio["sprintItemsDone"] >= 1 and portfolio["applicationsSent"] >= 1 and portfolio["tasks"], portfolio)

# ---------------------------------------------------------------- M7 lecturer and admin
section("M7 Lecturer support, research, settings")
directory = expect("student directory", call("GET", "/lecturer/students", lec), 200)
sid = next(x["id"] for x in directory if x["studentNo"] == "DEMO-001")
insights = expect("student insights", call("GET", f"/lecturer/students/{sid}", lec), 200)
check("insights include attempts", insights["attempts"], insights)
expect("student interview report (lecturer)", call("GET", f"/lecturer/students/{sid}/interview-report", lec), 200)
assignable = expect("assignable tasks", call("GET", "/lecturer/assignable-tasks", lec), 200)
tid = next(t["id"] for t in assignable if "Automate" in t["title"])
expect("assign task", call("POST", f"/lecturer/students/{sid}/assignments", lec, {"taskId": tid, "note": "Good next step."}), 200)
expect("assign twice", call("POST", f"/lecturer/students/{sid}/assignments", lec, {"taskId": tid, "note": None}), 400)
expect("short note", call("POST", f"/lecturer/students/{sid}/notes", lec, {"text": "ok"}), 400)
expect("add note", call("POST", f"/lecturer/students/{sid}/notes", lec, {"text": "Great progress on QA. Try automating a test case next."}), 200)
check("assigned task in library", any(t.get("assignedBy") for t in call("GET", "/practice/tasks", stu)[1]))
check("note in portfolio", call("GET", "/me/portfolio", stu)[1]["lecturerNotes"])
kinds = {n["kind"] for n in call("GET", "/me/notifications", stu)[1]["items"]}
check("student notified of note and assignment", {"LecturerNote", "TaskAssigned"} <= kinds, kinds)
research = expect("research overview", call("GET", "/research/overview", adm), 200)
check("research counts students", research["students"] > 0, research)
me = call("GET", "/auth/me", adm)[1]
expect("update designation", call("PUT", "/settings/profile", adm, {"designation": "Regression designation"}), 204)
check("designation changed", call("GET", "/auth/me", adm)[1]["headline"] == "Regression designation")
expect("restore designation", call("PUT", "/settings/profile", adm, {"designation": me["headline"]}), 204)
expect("lecturer cannot reset", call("POST", "/settings/demo-reset", lec), 403)

# ---------------------------------------------------------------- M8 calendar
section("M8 Year calendar")
cal = {r: call("GET", f"/calendar/{year}", t)[1] for r, t in ROLE_TOKENS.items()}
check("sample year plan seeded", len([e for e in cal["Admin"] if e["source"] == "UnitEvent"]) >= 10, len(cal["Admin"]))
stu_cal = call("GET", f"/calendar/{year}", stu)[1]
sources = {e["source"] for e in stu_cal}
check("student calendar merges personal dates", {"InterviewInvite", "AssignedTask", "PlannerDeadline"} <= sources, sources)
check("partner sees no student-only events", not any(e["audience"] == "Students" for e in cal["Industry"] if e["source"] == "UnitEvent"))
opts = call("GET", "/calendar/options", adm)[1]
event = {"title": "Regression career talk", "type": "IndustryTalk", "audience": "Partners", "startDate": future, "endDate": future,
         "description": None, "location": "Online", "careerPathId": None, "industryTaskId": None, "vacancyId": None}
expect("career path only for student events", call("POST", "/calendar/events", adm, dict(event, careerPathId=opts["careerPaths"][0]["id"])), 400)
expect("task release needs task", call("POST", "/calendar/events", adm, dict(event, type="TaskRelease", audience="Students")), 400)
expect("end before start", call("POST", "/calendar/events", adm, dict(event, endDate=TODAY.isoformat())), 400)
expect("student cannot create", call("POST", "/calendar/events", stu, event), 403)
eid = expect("create event", call("POST", "/calendar/events", adm, event), 200)["id"]
check("partner sees partner event", any(e.get("eventId") == eid for e in call("GET", f"/calendar/{int(future[:4])}", lanka)[1]))
check("student does not see partner event", not any(e.get("eventId") == eid for e in call("GET", f"/calendar/{int(future[:4])}", stu)[1]))
check("partner notified of event", any("Regression career talk" in n["title"] for n in call("GET", "/me/notifications", lanka)[1]["items"]))
check("student not notified of partner event", not any("Regression career talk" in n["title"] for n in call("GET", "/me/notifications", stu)[1]["items"]))
expect("update event", call("PUT", f"/calendar/events/{eid}", adm, dict(event, title="Regression career talk (moved)")), 204)
expect("delete event", call("DELETE", f"/calendar/events/{eid}", adm), 204)
expect("delete again", call("DELETE", f"/calendar/events/{eid}", adm), 404)

# ---------------------------------------------------------------- M9 notifications and digest
section("M9 Notifications and digest")
listing = call("GET", "/me/notifications", stu)[1]
check("student has unread notifications", listing["unreadCount"] > 0, listing)
first = listing["items"][0]
expect("mark one read", call("POST", f"/me/notifications/{first['id']}/read", stu), 204)
check("unread count drops", call("GET", "/me/notifications", stu)[1]["unreadCount"] == listing["unreadCount"] - 1)
before_lanka = call("GET", "/me/notifications", lanka)[1]["unreadCount"]
expect("other user's id is a no-op", call("POST", f"/me/notifications/{listing['items'][1]['id']}/read", lanka), 204)
check("other user's notification untouched", call("GET", "/me/notifications", stu)[1]["unreadCount"] == listing["unreadCount"] - 1)
digest = expect("student digest", call("GET", "/me/digest", stu), 200)
steps = {s["kind"] for s in digest["nextSteps"]}
check("digest suggests overdue planner item", "OverduePlannerItems" in steps, steps)
check("digest covers seven days", digest["from"] == TODAY.isoformat(), digest["from"])
admin_digest = call("GET", "/me/digest", adm)[1]
check("admin digest returns", admin_digest is not None and "nextSteps" in admin_digest)
expect("mark all read", call("POST", "/me/notifications/read-all", stu), 204)
check("all read", call("GET", "/me/notifications", stu)[1]["unreadCount"] == 0)

# ---------------------------------------------------------------- M10 events, teams, certificates
section("M10 Industry events, teams, certificates")
tha = login("demo-003@nsbm.demo", "Student@123")         # Tharindu, cohort student
hub = expect("event hub", call("GET", "/events", stu), 200)
titles = {e["title"]: e for e in hub}
bug_bash = next(e for t, e in titles.items() if t.startswith("Bug Bash"))
green = next(e for t, e in titles.items() if t.startswith("Green Hack"))
check("past event shows my certificate", bug_bash["hasEnded"] and bug_bash["hasCertificate"], bug_bash)
check("pending partner event hidden from students", not any(t.startswith("Hands-on workshop") for t in titles))
pending_event = next(e for e in call("GET", "/industry-events/review-queue", adm)[1] if e["status"] == "Pending")
expect("student cannot open pending event", call("GET", f"/events/{pending_event['id']}", stu), 404)
check("partner events on student calendar", any(e["source"] == "IndustryEvent" for e in call("GET", f"/calendar/{year}", stu)[1]))

start = (TODAY + datetime.timedelta(days=10)).isoformat()
close = (TODAY + datetime.timedelta(days=8)).isoformat()
ev_in = {"title": "Regression mini hackathon", "description": "A short regression hackathon for testing teams and places.",
         "format": "Hackathon", "startDate": start, "endDate": start, "location": "Lab 1", "capacity": 2,
         "registrationClosesOn": close, "minTeamSize": 2, "maxTeamSize": 2}
s_, b_ = call("POST", "/industry-events", qa, dict(ev_in, registrationClosesOn=(TODAY + datetime.timedelta(days=12)).isoformat(), minTeamSize=None))
check("invalid event rejected with field errors", s_ == 400 and {"Event.RegistrationClosesOn", "Event.MinTeamSize"} <= set(b_.get("errors", {})), (s_, b_))
expect("student cannot create event", call("POST", "/industry-events", stu, ev_in), 403)
mini = expect("partner creates event", call("POST", "/industry-events", qa, ev_in), 201)["id"]
check("admin notified of new event", any("Regression mini hackathon" in n["title"] for n in call("GET", "/me/notifications", adm)[1]["items"]))
expect("other company cannot read event", call("GET", f"/industry-events/{mini}", lanka), 403)
expect("reject event", call("POST", f"/industry-events/{mini}/review", adm, {"decision": "Reject", "note": "Add the judging criteria."}), 204)
expect("resubmit event", call("PUT", f"/industry-events/{mini}", qa, dict(ev_in, description=ev_in["description"] + " Judged on impact.")), 204)
expect("approve event", call("POST", f"/industry-events/{mini}/review", adm, {"decision": "Approve", "note": None}), 204)
expect("edit approved event", call("PUT", f"/industry-events/{mini}", qa, ev_in), 400)
check("students told about approved event", any(n["kind"] == "IndustryEvent" and "Regression mini" in n["title"] for n in call("GET", "/me/notifications", tha)[1]["items"]))

expect("register", call("POST", f"/events/{mini}/registration", stu), 204)
expect("register twice", call("POST", f"/events/{mini}/registration", stu), 400)
expect("team before registering", call("POST", f"/events/{mini}/teams", kav, {"name": "Second"}), 400)
expect("second student registers", call("POST", f"/events/{mini}/registration", kav), 204)
s_, b_ = call("POST", f"/events/{mini}/registration", tha)
check("full event refuses registration", s_ == 400 and "full" in json.dumps(b_).lower(), (s_, b_))
code = expect("create team", call("POST", f"/events/{mini}/teams", stu, {"name": "Regression Rockets"}), 200)["joinCode"]
check("join code has six characters", len(code) == 6, code)
expect("duplicate team name", call("POST", f"/events/{mini}/teams", kav, {"name": "regression rockets"}), 400)
expect("wrong join code", call("POST", f"/events/{mini}/teams/join", kav, {"joinCode": "ZZZZZZ"}), 400)
expect("join with lower-case code", call("POST", f"/events/{mini}/teams/join", kav, {"joinCode": code.lower()}), 204)
team = call("GET", f"/events/{mini}", kav)[1]["myTeam"]
check("team has both members, captain marked", len(team["members"]) == 2 and [m["isCaptain"] for m in team["members"]] == [True, False], team)
detail = call("GET", f"/events/{mini}", stu)[1]
check("full team shown as full", detail["teams"][0]["isFull"], detail["teams"])
expect("captain leaves team", call("DELETE", f"/events/{mini}/team", stu), 204)
team = call("GET", f"/events/{mini}", kav)[1]["myTeam"]
check("captaincy passes to remaining member", team["isCaptain"] and len(team["members"]) == 1, team)
expect("last member cancels registration", call("DELETE", f"/events/{mini}/registration", kav), 204)
check("empty team removed", call("GET", f"/events/{mini}", stu)[1]["teams"] == [])
expect("attendance before start", call("PUT", f"/industry-events/{mini}/attendance/1", qa, {"attended": True}), 400)
expect("certificates before end", call("POST", f"/industry-events/{mini}/certificates", qa), 400)
expect("other company attendance sheet", call("GET", f"/industry-events/{mini}/attendance", lanka), 403)

sheet = expect("attendance sheet (past event)", call("GET", f"/industry-events/{bug_bash['id']}/attendance", qa), 200)
check("past event attendees have certificates", sheet["attendees"] and all(a["certificateCode"] for a in sheet["attendees"]), sheet)
expect("cannot un-mark certified attendee", call("PUT", f"/industry-events/{bug_bash['id']}/attendance/{sheet['attendees'][0]['registrationId']}", qa, {"attended": False}), 400)
expect("re-issuing finds nobody new", call("POST", f"/industry-events/{bug_bash['id']}/certificates", qa), 400)
certs = expect("my certificates", call("GET", "/events/certificates", stu), 200)
check("certificate listed", certs and certs[0]["eventTitle"].startswith("Bug Bash"), certs)
verified = expect("public verification (no sign-in)", call("GET", f"/certificates/{certs[0]['code'].lower()}"), 200)
check("verification matches", verified["studentName"] == certs[0]["studentName"], verified)
expect("unknown certificate", call("GET", "/certificates/ICU-NOTREAL00"), 404)

# Captaincy on the seeded hackathon: Kavindi leads, Tharindu is a member.
expect("register for seeded hackathon", call("POST", f"/events/{green['id']}/registration", stu), 204)
seeded_code = call("GET", f"/events/{green['id']}", kav)[1]["myTeam"]["joinCode"]
expect("join seeded team", call("POST", f"/events/{green['id']}/teams/join", stu, {"joinCode": seeded_code}), 204)
expect("seeded captain leaves", call("DELETE", f"/events/{green['id']}/team", kav), 204)
members = call("GET", f"/events/{green['id']}", stu)[1]["myTeam"]["members"]
check("earliest member becomes captain", next(m for m in members if m["isCaptain"])["name"].startswith("Tharindu"), members)

# ---------------------------------------------------------------- M11 AI interviewer
section("M11 AI interviewer (no AI calls are made)")
avail = expect("AI availability", call("GET", "/me/ai-interviews/availability", stu), 200)
check("availability reports question count", avail["questionsPerSession"] == 6, avail)
s_, b_ = call("POST", "/me/ai-interviews", stu, {"stage": "SoftSkills", "consentToAiProcessing": False})
check("consent is required", s_ == 400 and "ConsentToAiProcessing" in json.dumps(b_), (s_, b_))
if not avail["isAvailable"]:
    s_, b_ = call("POST", "/me/ai-interviews", stu, {"stage": "SoftSkills", "consentToAiProcessing": True})
    check("without a key, start answers 503 with a clear message", s_ == 503 and "not set up" in json.dumps(b_), (s_, b_))
check("no sessions stored", call("GET", "/me/ai-interviews", stu)[1] == [])
expect("unknown session", call("GET", "/me/ai-interviews/999999", stu), 404)
expect("answer unknown session", call("POST", "/me/ai-interviews/999999/answers", stu, {"answer": "Hello"}), 404)
expect("empty answer rejected", call("POST", "/me/ai-interviews/999999/answers", stu, {"answer": ""}), 400)
expect("lecturer cannot read AI interviews", call("GET", "/me/ai-interviews/999999", lec), 403)

# ---------------------------------------------------------------- M12 badges, streaks, profile card
section("M12 Skill badges, streaks, profile card")
ach = expect("my achievements", call("GET", "/me/achievements", stu), 200)
check("practice today counts this week", ach["progress"]["activeThisWeek"] and ach["progress"]["currentStreak"] >= 1, ach["progress"])
check("twelve recent weeks", len(ach["progress"]["recentWeeks"]) == 12 and ach["progress"]["recentWeeks"][-1], ach["progress"])
check("level reflects roadmap", ach["progress"]["level"] == "Explorer" and ach["progress"]["roadmapCompleted"] == 1, ach["progress"])
check("card private by default", ach["profileCard"] == {"isShared": False, "slug": None}, ach["profileCard"])
eligible = ach["eligibleSkills"]
check("practised skills can be badged", eligible, ach)
eligible_ids = {e["skillId"] for e in eligible}
no_evidence = next(sid for name, sid in skills.items() if sid not in eligible_ids)
s_, b_ = call("POST", "/me/achievements/badges", stu, {"skillId": no_evidence, "note": None})
check("no badge without practice evidence", s_ == 400 and "Practise" in json.dumps(b_), (s_, b_))
target = eligible[0]
badge_id = expect("request badge", call("POST", "/me/achievements/badges", stu, {"skillId": target["skillId"], "note": "Used it in the order task."}), 200)["id"]
expect("request twice while pending", call("POST", "/me/achievements/badges", stu, {"skillId": target["skillId"], "note": None}), 400)
check("lecturers told about the request", any(n["kind"] == "BadgeRequested" for n in call("GET", "/me/notifications", lec)[1]["items"]))
queue = expect("badge review queue", call("GET", "/badge-reviews", lec), 200)
item = next(q for q in queue if q["id"] == badge_id)
check("reviewer sees captured evidence", item["evidence"] == target["sources"] and item["studentNo"] == "DEMO-001", item)
check("seeded request waiting", any(q["status"] == "Pending" and q["studentNo"] == "DEMO-004" for q in queue), queue)
expect("decline needs a note", call("POST", f"/badge-reviews/{badge_id}", lec, {"decision": "Reject", "note": ""}), 400)
expect("decline with advice", call("POST", f"/badge-reviews/{badge_id}", lec, {"decision": "Reject", "note": "Pass one more task that uses it."}), 204)
check("student told it is not yet", any(n["kind"] == "BadgeReviewed" and n["title"].startswith("Badge not yet") for n in call("GET", "/me/notifications", stu)[1]["items"]))
expect("ask again after decline", call("POST", "/me/achievements/badges", stu, {"skillId": target["skillId"], "note": "Practised more."}), 200)
expect("approve", call("POST", f"/badge-reviews/{badge_id}", lec, {"decision": "Approve", "note": None}), 204)
expect("approve twice", call("POST", f"/badge-reviews/{badge_id}", lec, {"decision": "Approve", "note": None}), 400)
mine = next(b for b in call("GET", "/me/achievements", stu)[1]["badges"] if b["id"] == badge_id)
check("approved badge has a verification code", mine["status"] == "Approved" and mine["verificationCode"].startswith("BDG-"), mine)
expect("partner cannot review badges", call("POST", f"/badge-reviews/{badge_id}", lanka, {"decision": "Approve", "note": None}), 403)

card_settings = expect("share profile card", call("PUT", "/me/achievements/profile-card", stu, {"share": True}), 200)
slug = card_settings["slug"]
check("shared card has a link", card_settings["isShared"] and slug and len(slug) == 12, card_settings)
card = expect("public card (no sign-in)", call("GET", f"/profile-cards/{slug}"), 200)
check("card shows the approved badge", any(b["verificationCode"] == mine["verificationCode"] for b in card["badges"]), card)
check("card shows certificate", any(c["eventTitle"].startswith("Bug Bash") for c in card["certificates"]), card)
check("card holds no private data", set(card) == {"fullName", "programme", "careerPathName", "level", "currentStreak", "bestStreak", "badges", "certificates"}, set(card))
expect("make card private", call("PUT", "/me/achievements/profile-card", stu, {"share": False}), 200)
expect("private card is not found", call("GET", f"/profile-cards/{slug}"), 404)
check("re-sharing keeps the same link", call("PUT", "/me/achievements/profile-card", stu, {"share": True})[1]["slug"] == slug)
expect("unknown card", call("GET", "/profile-cards/doesnotexist"), 404)
kav_card = call("GET", "/me/achievements", kav)[1]["profileCard"]
check("seeded card shared", kav_card["isShared"], kav_card)
kav_public = expect("seeded public card", call("GET", f"/profile-cards/{kav_card['slug']}"), 200)
check("seeded card shows SQL badge", any(b["skillName"] == "SQL" for b in kav_public["badges"]), kav_public)
check("seeded badge is backed by real evidence", any(e["skillName"] == "SQL" and e["badgeStatus"] == "Approved" for e in call("GET", "/me/achievements", kav)[1]["eligibleSkills"]))

# ---------------------------------------------------------------- M13 public showcase
section("M13 Public showcase and partner enquiries")
show = expect("showcase (no sign-in)", call("GET", "/showcase"), 200)
check("live stats", show["stats"]["openInternships"] >= 1 and show["stats"]["partnerCompanies"] >= 1 and show["stats"]["badgesAwarded"] >= 1, show["stats"])
check("career paths listed", len(show["careerPaths"]) >= 4)
check("only active partners", show["partners"] and all(p["tasks"] + p["internships"] + p["events"] > 0 for p in show["partners"]), show["partners"])
check("internships are open ones", all(i["closingDate"] >= TODAY.isoformat() for i in show["internships"]) and show["internships"])
student_only = {e["title"] for e in cal["Admin"] if e["source"] == "UnitEvent" and e["audience"] != "Everyone"}
check("no student-only or lecturer events on the public page", not any(e["title"] in student_only for e in show["upcomingEvents"]), show["upcomingEvents"])
check("pending partner events stay hidden", not any(e["title"].startswith("Hands-on workshop") for e in show["upcomingEvents"]))
profiles = show["featuredProfiles"]
check("featured profiles are only shared cards", profiles and all(call("GET", f"/profile-cards/{p['cardSlug']}")[0] == 200 for p in profiles), profiles)
check("featured profiles hold no private data", all(set(p) == {"fullName", "programme", "careerPathName", "badges", "cardSlug"} for p in profiles))
check("private students never featured", not any(p["fullName"].startswith("Tharindu") for p in profiles))

enquiry = {"organisationName": "Regression Tech (Pvt) Ltd", "contactName": "Test Person", "email": "partner.test@example.com",
           "phone": "+94 11 234 5678", "interest": "Internships", "message": "We would like to offer two QA internships next year.", "website": ""}
s_, b_ = call("POST", "/showcase/partner-enquiries", body=dict(enquiry, email="not-an-email", message="short"))
check("invalid enquiry rejected with field errors", s_ == 400 and {"Email", "Message"} <= set(b_.get("errors", {})), (s_, b_))
expect("honeypot submission looks accepted", call("POST", "/showcase/partner-enquiries", body=dict(enquiry, organisationName="Spam Bot", website="http://spam.example")), 202)
expect("valid enquiry", call("POST", "/showcase/partner-enquiries", body=enquiry), 202)
inbox = expect("admin inbox", call("GET", "/partner-enquiries", adm), 200)
check("real enquiry stored, honeypot one dropped", any(e["organisationName"] == "Regression Tech (Pvt) Ltd" for e in inbox) and not any(e["organisationName"] == "Spam Bot" for e in inbox), inbox)
check("admins told about the enquiry", any(n["kind"] == "PartnerEnquiry" for n in call("GET", "/me/notifications", adm)[1]["items"]))
eid_ = next(e["id"] for e in inbox if e["organisationName"] == "Regression Tech (Pvt) Ltd")
expect("partner cannot mark enquiries", call("POST", f"/partner-enquiries/{eid_}/handled", lanka), 403)
expect("mark handled", call("POST", f"/partner-enquiries/{eid_}/handled", adm), 204)
check("handled by admin", next(e for e in call("GET", "/partner-enquiries", adm)[1] if e["id"] == eid_)["isHandled"])
codes = [call("POST", "/showcase/partner-enquiries", body=dict(enquiry, organisationName=f"Flood {i}"))[0] for i in range(4)]
check("public form is rate limited", 429 in codes, codes)

# ---------------------------------------------------------------- M15 assistant registrar, halls, announcements
section("M15 Assistant Registrar: venues, hall allocation, announcements")
me = call("GET", "/auth/me", reg)[1]
check("registrar profile", me["role"] == "Registrar" and me.get("headline") == "Assistant Registrar", me)
venues = expect("venue list", call("GET", "/venues", reg), 200)
by_code = {v["code"]: v for v in venues}
check("campus venues seeded", {"LH-101", "LH-102", "LAB-01", "LAB-02", "AUD-01", "EH-01"} <= set(by_code), list(by_code))

monday = TODAY + datetime.timedelta(days=(7 - TODAY.weekday()) % 7 or 7)
window = f"from={TODAY.isoformat()}&to={(monday + datetime.timedelta(days=30)).isoformat()}"
seeded = call("GET", f"/venues/bookings?{window}", reg)[1]
check("weekly lectures seeded", sum(1 for b in seeded if b["title"] == "Software Architecture") == 4, seeded[:3])
check("a partner event has a hall", any(b["purpose"] == "IndustryEvent" for b in seeded))
expect("range over 62 days rejected", call("GET", f"/venues/bookings?from={TODAY.isoformat()}&to={(TODAY + datetime.timedelta(days=90)).isoformat()}", reg), 400)

sahan_cal = call("GET", f"/calendar/{monday.year}", stu)[1]
check("student sees their cohort's lecture hall", any(e["source"] == "HallBooking" and e["title"] == "Software Architecture" and "LH-101" in (e["location"] or "") for e in sahan_cal))
kav_cal_before = call("GET", f"/calendar/{monday.year}", kav)[1]
check("other cohort does not see it", not any(e["source"] == "HallBooking" and e["title"] == "Software Architecture" for e in kav_cal_before))

opts = expect("booking options", call("GET", "/venues/booking-options", reg), 200)
check("cohorts with sizes", any(c["programme"] == "BSc (Hons) Software Engineering" and c["academicYear"] == 3 and c["studentCount"] == 2 for c in opts["cohorts"]), opts["cohorts"])

def lecture(**over):
    body = {"venueId": by_code["LH-101"]["id"], "purpose": "Lecture", "title": "Regression lecture", "date": monday.isoformat(),
            "startTime": "11:00:00", "endTime": "13:00:00", "programme": "BSc (Hons) Computer Science", "academicYear": 3,
            "industryEventId": None, "calendarEventId": None, "expectedAttendees": None, "note": None, "repeatWeeks": 1}
    body.update(over)
    return body

s_, b_ = call("POST", "/venues/bookings", reg, lecture(programme=None, academicYear=None))
check("lecture needs a cohort", s_ == 400 and "Booking.Programme" in b_.get("errors", {}), (s_, b_))
expect("end before start", call("POST", "/venues/bookings", reg, lecture(startTime="13:00:00", endTime="11:00:00")), 400)
expect("past date", call("POST", "/venues/bookings", reg, lecture(date=(TODAY - datetime.timedelta(days=1)).isoformat())), 400)
expect("outside teaching hours", call("POST", "/venues/bookings", reg, lecture(startTime="20:00:00", endTime="22:00:00")), 400)
expect("unknown cohort", call("POST", "/venues/bookings", reg, lecture(programme="BSc (Hons) Nothing")), 400)
s_, b_ = call("POST", "/venues/bookings", reg, lecture(venueId=by_code["LAB-02"]["id"], expectedAttendees=100))
check("over capacity rejected", s_ == 400 and "seats 40" in str(b_), (s_, b_))
s_, b_ = call("POST", "/venues/bookings", reg, lecture(startTime="10:00:00", endTime="12:00:00"))
check("hall clash rejected", s_ == 400 and "already booked" in str(b_), (s_, b_))
s_, b_ = call("POST", "/venues/bookings", reg, lecture(venueId=by_code["LAB-02"]["id"], programme="BSc (Hons) Software Engineering", startTime="10:00:00", endTime="11:00:00"))
check("cohort double-booking rejected", s_ == 400 and "cohort already has" in str(b_), (s_, b_))
expect("student cannot allocate", call("POST", "/venues/bookings", stu, lecture()), 403)
expect("admin follows but cannot allocate", call("POST", "/venues/bookings", adm, lecture()), 403)
expect("only lectures repeat", call("POST", "/venues/bookings", reg, lecture(purpose="UnitEvent", programme=None, academicYear=None, calendarEventId=1, expectedAttendees=10, repeatWeeks=2)), 400)

kav_before = call("GET", "/me/notifications", kav)[1]["unreadCount"]
ids = expect("back-to-back slot books, repeating weekly", call("POST", "/venues/bookings", reg, lecture(repeatWeeks=3)), 200)["ids"]
check("one booking per week", len(ids) == 3, ids)
kav_notes = call("GET", "/me/notifications", kav)[1]
check("cohort student notified once", kav_notes["unreadCount"] == kav_before + 1 and kav_notes["items"][0]["kind"] == "HallAssigned"
      and "LH-101" in kav_notes["items"][0]["body"] and "3 weeks" in kav_notes["items"][0]["body"], kav_notes["items"][:1])
check("other cohorts not notified", not any(n["title"] == "Hall assigned: Regression lecture" for n in call("GET", "/me/notifications", stu)[1]["items"]))
check("admin notified of the allocation", any(n["title"] == "Hall assigned: Regression lecture" and n["link"] == "/hall-bookings" for n in call("GET", "/me/notifications", adm)[1]["items"]))
check("registrar not notified of own allocation", not any(n["title"] == "Hall assigned: Regression lecture" for n in call("GET", "/me/notifications", reg)[1]["items"]))
check("lecture on the student's calendar", any(e["source"] == "HallBooking" and e["title"] == "Regression lecture" for e in call("GET", f"/calendar/{monday.year}", kav)[1]))
check("admin sees allocations", any(b["id"] == ids[0] for b in call("GET", f"/venues/bookings?{window}", adm)[1]))

expect("student cannot cancel", call("DELETE", f"/venues/bookings/{ids[0]}", stu), 403)
expect("cancel a booking", call("DELETE", f"/venues/bookings/{ids[0]}", reg), 204)
check("cohort told about the cancellation", call("GET", "/me/notifications", kav)[1]["items"][0]["kind"] == "HallBookingCancelled")
expect("cancel again", call("DELETE", f"/venues/bookings/{ids[0]}", reg), 404)

hall = {"code": "lh-901", "name": "Regression Hall", "building": "Test Block", "type": "LectureHall", "capacity": 30, "facilities": None, "isActive": True}
vid = expect("add venue", call("POST", "/venues", reg, hall), 200)["id"]
check("code normalised", any(v["id"] == vid and v["code"] == "LH-901" for v in call("GET", "/venues", reg)[1]))
expect("duplicate code", call("POST", "/venues", reg, hall), 400)
expect("admin cannot add venues", call("POST", "/venues", adm, dict(hall, code="LH-902")), 403)
expect("book the new hall", call("POST", "/venues/bookings", reg, lecture(venueId=vid, startTime="14:00:00", endTime="15:00:00", programme="BSc (Hons) Computer Networks", academicYear=4)), 200)
s_, b_ = call("PUT", f"/venues/{vid}", reg, dict(hall, capacity=0))
check("capacity must be positive", s_ == 400, (s_, b_))
expect("close the hall", call("PUT", f"/venues/{vid}", reg, dict(hall, isActive=False)), 204)
s_, b_ = call("POST", "/venues/bookings", reg, lecture(venueId=vid, startTime="15:00:00", endTime="16:00:00", programme="BSc (Hons) Computer Networks", academicYear=4))
check("closed hall takes no bookings", s_ == 400 and "not taking bookings" in str(b_), (s_, b_))

pending_hall = [e for e in opts["industryEvents"] if not e["hasHall"]]
reg_digest = call("GET", "/me/digest", reg)[1]
check("registrar digest lists events without a hall", (not pending_hall) or any(s["kind"] == "AllocateEventHalls" and s["count"] == len(pending_hall) for s in reg_digest["nextSteps"]), reg_digest["nextSteps"])
if pending_hall:
    ev = pending_hall[0]
    body = lecture(purpose="IndustryEvent", title=None, programme=None, academicYear=None, industryEventId=ev["id"], venueId=by_code["AUD-01"]["id"],
                   date=ev["startDate"], startTime="09:00:00", endTime="12:00:00")
    expect("event hall outside the event's dates", call("POST", "/venues/bookings", reg, dict(body, date=(datetime.date.fromisoformat(ev["endDate"]) + datetime.timedelta(days=1)).isoformat())), 400)
    expect("allocate an event hall", call("POST", "/venues/bookings", reg, body), 200)
    check("event takes the event title", any(b["industryEventId"] == ev["id"] and b["title"] == ev["title"] for b in call("GET", f"/venues/bookings?from={ev['startDate']}&to={ev['startDate']}", reg)[1]))
    check("hall flag updated", next(e for e in call("GET", "/venues/booking-options", reg)[1]["industryEvents"] if e["id"] == ev["id"])["hasHall"])

# Admin announcements
ann = {"audience": "Everyone", "title": "Campus closed on Friday", "message": "The campus is closed this Friday for maintenance. Online sessions continue."}
expect("announcement needs a title", call("POST", "/announcements", adm, dict(ann, title="")), 400)
expect("only admins announce", call("POST", "/announcements", reg, ann), 403)
expect("announce to everyone", call("POST", "/announcements", adm, ann), 200)
for who, token in {"student": stu, "lecturer": lec, "partner": lanka, "registrar": reg}.items():
    check(f"{who} received the announcement", any(n["kind"] == "Announcement" and n["title"] == ann["title"] for n in call("GET", "/me/notifications", token)[1]["items"]))
check("sender not notified", not any(n["kind"] == "Announcement" for n in call("GET", "/me/notifications", adm)[1]["items"]))
expect("announce to students only", call("POST", "/announcements", adm, dict(ann, audience="Students", title="Exam timetable published")), 200)
check("partners left out of a student announcement", not any(n["title"] == "Exam timetable published" for n in call("GET", "/me/notifications", lanka)[1]["items"]))
history = expect("announcement history", call("GET", "/announcements", adm), 200)
check("history newest first with recipient counts", history[0]["title"] == "Exam timetable published" and history[1]["recipientCount"] > history[0]["recipientCount"] > 0, history)

# ---------------------------------------------------------------- restore
section("Restore")
expect("final demo reset", call("POST", "/settings/demo-reset", adm), 204)

# ---------------------------------------------------------------- report
failed = [r for r in results if not r[1]]
checks = [r for r in results if not r[0].startswith("==")]
for name, ok, detail in results:
    if name.startswith("=="):
        print(f"\n{name}")
    elif not ok:
        print(f"  FAIL  {name}\n        {detail}")
print(f"\n{len(checks) - len(failed)}/{len(checks)} checks passed.")
sys.exit(1 if failed else 0)
