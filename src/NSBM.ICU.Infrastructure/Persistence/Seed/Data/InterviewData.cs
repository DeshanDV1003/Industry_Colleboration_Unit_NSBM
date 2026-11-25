namespace NSBM.ICU.Infrastructure.Persistence.Seed.Data;

/// <summary>Practice interview questions. Pure data — the seeding logic lives in <see cref="InterviewSeeder"/>.</summary>
internal static class InterviewData
{
    public sealed record QuestionSeed(string Topic, string Prompt, string[] Keywords, string Tip, QuestionSeed[]? FollowUps = null);

    public static readonly QuestionSeed[] SoftSkills =
    [
        new("Introduction", "Tell me about yourself and why you want an internship in IT.",
            ["degree", "project", "skills", "interest", "goal", "team"],
            "Keep it to a minute: what you study, one project you are proud of, and what you want to learn next.",
            [new("Introduction", "Which part of that project would you do differently now, and why?",
                ["learned", "improve", "mistake", "feedback", "next time"], "Showing that you reflect on your work is a strength, not a weakness.")]),

        new("Teamwork", "Describe a time you worked in a team to deliver something under a deadline.",
            ["team", "deadline", "role", "communicate", "result", "plan"],
            "Use STAR: Situation, Task, Action, Result. Be clear about what you personally did.",
            [new("Teamwork", "What would you do if a teammate stopped contributing close to the deadline?",
                ["talk", "listen", "support", "lecturer", "redistribute", "early"], "Show that you address problems early and respectfully before escalating.")]),

        new("Conflict", "Tell me about a disagreement you had with someone and how you handled it.",
            ["listen", "understand", "compromise", "calm", "respect", "outcome"],
            "Focus on how you listened and found common ground, not on who was right.",
            [new("Conflict", "Imagine your team lead rejects your technical idea in a meeting. How do you respond?",
                ["ask", "reason", "data", "accept", "respect", "follow up"], "Ask questions to understand the reasoning; offer evidence calmly; commit to the team decision.")]),

        new("Problem solving", "Describe a problem you could not solve at first. What did you do?",
            ["research", "debug", "ask", "documentation", "break down", "solved"],
            "Show your process: how you narrowed the problem, what you tried and when you asked for help."),

        new("Communication", "How would you explain a technical problem to a non-technical client?",
            ["simple", "example", "impact", "avoid jargon", "check understanding", "next steps"],
            "Lead with the impact on them, use an everyday example and confirm they understood."),

        new("Workplace", "You realise you will miss a deadline your manager is relying on. What do you do?",
            ["early", "inform", "manager", "reason", "plan", "options"],
            "Raise it early, explain why, and come with a revised plan or options — never stay silent."),
    ];

    public static readonly Dictionary<string, QuestionSeed[]> Technical = new()
    {
        ["software-engineering"] =
        [
            new("OOP", "Explain the difference between an interface and an abstract class, and when you would use each.",
                ["contract", "implementation", "inheritance", "multiple", "shared code", "abstract"],
                "Give a short example from a project, e.g. an IPaymentProvider interface."),
            new("Debugging", "A feature works on your machine but fails in the test environment. How do you investigate?",
                ["logs", "configuration", "environment", "reproduce", "version", "data"],
                "Talk through comparing environments step by step instead of guessing."),
            new("APIs", "What makes a REST API well designed?",
                ["resources", "status codes", "versioning", "validation", "consistent", "documentation"],
                "Mention correct HTTP methods and status codes, consistent naming and clear errors."),
        ],
        ["quality-assurance"] =
        [
            new("Test design", "How do you decide which test cases to write for a new feature?",
                ["requirements", "risk", "boundary", "negative", "priority", "acceptance criteria"],
                "Show you start from requirements and risk, then cover positive, negative and boundary cases."),
            new("Bug reporting", "Walk me through a good bug report.",
                ["steps to reproduce", "expected", "actual", "environment", "severity", "evidence"],
                "Structure matters: title, steps, expected vs actual, environment and screenshots or logs."),
            new("Automation", "Which tests would you automate first, and why?",
                ["regression", "repetitive", "stable", "smoke", "pipeline", "maintenance"],
                "Explain the trade-off between the value of automation and the cost of maintaining it."),
        ],
        ["devops"] =
        [
            new("CI/CD", "Describe the stages of a CI/CD pipeline you would build for a web app.",
                ["build", "test", "artifact", "deploy", "rollback", "environment"],
                "Walk from commit to production and mention how failures stop the pipeline."),
            new("Containers", "What problems do containers solve, and what are their limits?",
                ["consistent", "isolation", "dependencies", "portable", "orchestration", "state"],
                "Balance the benefits with challenges such as persistent data and security updates."),
            new("Monitoring", "A service is slow in production. How do you find the cause?",
                ["metrics", "logs", "latency", "database", "recent change", "alert"],
                "Start from metrics and recent changes, then narrow down with logs and traces."),
        ],
        ["cloud-engineering"] =
        [
            new("Security", "How would you secure access to a cloud storage bucket that holds invoices?",
                ["least privilege", "role", "encryption", "public access", "https", "audit"],
                "Mention least privilege, blocking public access, encryption and logging."),
            new("Networking", "Explain how you would design a network for a web app with a database.",
                ["vpc", "public subnet", "private subnet", "load balancer", "security group", "nat"],
                "Keep the database private and expose only the load balancer to the internet."),
            new("Cost", "Your cloud bill doubled this month. What do you check?",
                ["usage", "idle resources", "budget", "alerts", "rightsizing", "tags"],
                "Show a methodical review: cost explorer, idle or oversized resources, and budget alerts."),
        ],
    };
}
