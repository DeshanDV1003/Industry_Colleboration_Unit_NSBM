using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Infrastructure.Persistence.Seed.Data;

/// <summary>
/// Reference content for the four career paths: skills, roadmaps and baseline questions.
/// Pure data — the seeding logic lives in <see cref="CatalogueSeeder"/>.
/// </summary>
internal static class CareerCatalogueData
{
    public sealed record SkillSeed(string Name, SkillCategory Category, string Description);

    public sealed record StepSeed(LearningStage Stage, string Title, string Description, string Outcome, string? Skill);

    public sealed record QuestionSeed(string Skill, string Prompt, string[] Options, int CorrectIndex, string Explanation);

    public sealed record PathSeed(
        string Slug,
        string Name,
        string Tagline,
        string Description,
        string EntryRoles,
        string[] CoreSkills,
        string[] SupportingSkills,
        StepSeed[] Roadmap,
        QuestionSeed[] Baseline);

    // ── Skills ──────────────────────────────────────────────────────────────

    public static readonly SkillSeed[] Skills =
    [
        new("Object-oriented design", SkillCategory.Programming, "Classes, interfaces, encapsulation and SOLID principles."),
        new("Data structures & algorithms", SkillCategory.Programming, "Lists, maps, trees, searching, sorting and complexity."),
        new("Git & version control", SkillCategory.Programming, "Branching, commits, pull requests and resolving conflicts."),
        new("SQL", SkillCategory.Programming, "Querying, joining and modelling relational data."),
        new("REST APIs", SkillCategory.Programming, "Designing and consuming HTTP APIs and JSON."),
        new("Debugging", SkillCategory.Programming, "Reproducing, isolating and fixing defects with tools and logs."),
        new("JavaScript", SkillCategory.Programming, "Modern JavaScript and TypeScript for web applications."),
        new("Java", SkillCategory.Programming, "Core Java and the Spring ecosystem."),
        new("C#", SkillCategory.Programming, "C# and .NET application development."),
        new("Python", SkillCategory.Programming, "Python for scripting, automation and services."),
        new("Unit testing", SkillCategory.Testing, "Writing fast, isolated tests with frameworks like JUnit, xUnit or Jest."),
        new("Test case design", SkillCategory.Testing, "Equivalence partitioning, boundary values and test scenarios."),
        new("Bug reporting", SkillCategory.Testing, "Clear, reproducible defect reports with expected and actual results."),
        new("Test automation", SkillCategory.Testing, "UI and end-to-end automation with Selenium or Playwright."),
        new("API testing", SkillCategory.Testing, "Testing endpoints with Postman or code, including negative cases."),
        new("Linux & shell", SkillCategory.Infrastructure, "Command line, permissions, processes and shell scripting."),
        new("Docker", SkillCategory.Infrastructure, "Building images and running containers."),
        new("CI/CD pipelines", SkillCategory.Infrastructure, "Automated build, test and deployment pipelines."),
        new("Infrastructure as code", SkillCategory.Infrastructure, "Provisioning with Terraform, Bicep or CloudFormation."),
        new("Monitoring & logging", SkillCategory.Infrastructure, "Metrics, logs, alerts and dashboards."),
        new("Networking basics", SkillCategory.Infrastructure, "IP, DNS, ports, subnets and load balancing."),
        new("Cloud fundamentals", SkillCategory.Infrastructure, "Core compute, storage and database services on AWS or Azure."),
        new("Cloud security & IAM", SkillCategory.Infrastructure, "Identities, roles, least privilege and secrets."),
        new("Kubernetes", SkillCategory.Infrastructure, "Deploying and operating containers on Kubernetes."),
        new("Agile & Scrum", SkillCategory.Professional, "Sprints, stand-ups, user stories and retrospectives."),
        new("Communication", SkillCategory.Professional, "Explaining technical work clearly to different audiences."),
        new("Teamwork", SkillCategory.Professional, "Collaborating, sharing knowledge and resolving conflict."),
    ];

    // ── Career paths ────────────────────────────────────────────────────────

    public static readonly PathSeed[] Paths =
    [
        new(
            Slug: "software-engineering",
            Name: "Software Engineering",
            Tagline: "Design, build and maintain reliable software products.",
            Description: "Software engineers turn requirements into working, maintainable code. Entry-level engineers fix bugs, build small features, write tests, review code with teammates and learn the team's architecture and delivery practices.",
            EntryRoles: "Associate Software Engineer;Trainee Software Engineer;Junior Full-Stack Developer",
            CoreSkills: ["Object-oriented design", "Data structures & algorithms", "Git & version control", "SQL", "REST APIs", "Debugging", "Unit testing"],
            SupportingSkills: ["JavaScript", "Java", "C#", "Agile & Scrum", "Communication"],
            Roadmap:
            [
                new(LearningStage.Beginner, "Programming foundations", "Write clean code in one main language using variables, control flow, functions and collections.", "Solve small problems confidently in code.", "Data structures & algorithms"),
                new(LearningStage.Beginner, "Version control with Git", "Use branches, meaningful commits and pull requests in a shared repository.", "Collaborate on a codebase without losing work.", "Git & version control"),
                new(LearningStage.Intermediate, "Object-oriented design", "Model a domain with classes and interfaces and apply SOLID principles.", "Structure code so it is easy to change.", "Object-oriented design"),
                new(LearningStage.Intermediate, "Data and APIs", "Design a relational schema, write SQL and expose it through a REST API.", "Build a small end-to-end backend feature.", "REST APIs"),
                new(LearningStage.Intermediate, "Testing and debugging", "Write unit tests and use a debugger and logs to find the cause of defects.", "Fix bugs with evidence instead of guesswork.", "Debugging"),
                new(LearningStage.Advanced, "Ship a feature in a team", "Take a user story from refinement to a reviewed, tested, deployed change.", "Work like an entry-level engineer in a sprint.", "Agile & Scrum"),
            ],
            Baseline:
            [
                new("Object-oriented design", "Which principle says a class should have only one reason to change?",
                    ["Open/closed principle", "Single responsibility principle", "Liskov substitution principle", "Dependency inversion principle"], 1,
                    "Single responsibility keeps each class focused on one job, so changes stay local."),
                new("Data structures & algorithms", "What is the average time complexity of looking up a key in a hash map?",
                    ["O(1)", "O(log n)", "O(n)", "O(n log n)"], 0,
                    "Hashing jumps straight to a bucket, so average lookups take constant time."),
                new("Git & version control", "You want to work on a new feature without affecting the main branch. What do you do first?",
                    ["Commit directly to main", "Create a new branch", "Delete the remote repository", "Run git reset --hard"], 1,
                    "A feature branch isolates your changes until they are reviewed and merged."),
                new("SQL", "Which clause filters groups after a GROUP BY?",
                    ["WHERE", "ORDER BY", "HAVING", "LIMIT"], 2,
                    "WHERE filters rows before grouping; HAVING filters the grouped results."),
                new("REST APIs", "Which HTTP status code best describes a successful request that created a resource?",
                    ["200 OK", "201 Created", "204 No Content", "400 Bad Request"], 1,
                    "201 Created tells the client a new resource now exists."),
                new("Debugging", "An order total is wrong only for orders with discounts. What is the best first step?",
                    ["Rewrite the whole checkout module", "Reproduce it with a failing test case", "Ask users to stop using discounts", "Add more servers"], 1,
                    "A reliable reproduction narrows the cause and proves the fix later."),
            ]),

        new(
            Slug: "quality-assurance",
            Name: "Quality Assurance",
            Tagline: "Make sure software works the way users expect.",
            Description: "QA engineers design tests, find and report defects, and automate checks so teams can release with confidence. Entry-level QA engineers write test cases, run regression tests, log clear bug reports and grow into automation.",
            EntryRoles: "Associate QA Engineer;Trainee Software Tester;Junior Automation Engineer",
            CoreSkills: ["Test case design", "Bug reporting", "Test automation", "API testing", "SQL"],
            SupportingSkills: ["Agile & Scrum", "Communication", "Git & version control", "JavaScript", "Unit testing"],
            Roadmap:
            [
                new(LearningStage.Beginner, "Testing fundamentals", "Learn testing levels and types: functional, regression, smoke and exploratory testing.", "Explain what to test and why.", "Test case design"),
                new(LearningStage.Beginner, "Writing test cases", "Apply equivalence partitioning and boundary value analysis to real requirements.", "Write test cases that find defects efficiently.", "Test case design"),
                new(LearningStage.Intermediate, "Bug reporting and tracking", "Log reproducible defects with steps, expected and actual results, and evidence.", "Report bugs developers can act on quickly.", "Bug reporting"),
                new(LearningStage.Intermediate, "API and data testing", "Test REST endpoints with Postman and verify data with SQL queries.", "Test below the user interface.", "API testing"),
                new(LearningStage.Intermediate, "UI test automation", "Automate key user journeys with Playwright or Selenium.", "Turn repetitive checks into automated suites.", "Test automation"),
                new(LearningStage.Advanced, "Quality in the delivery pipeline", "Run automated tests in CI and contribute to the team's definition of done.", "Help a team release with confidence.", "Agile & Scrum"),
            ],
            Baseline:
            [
                new("Test case design", "A field accepts ages 18 to 60. Which set best applies boundary value analysis?",
                    ["20, 30, 40", "17, 18, 60, 61", "0, 100", "25 only"], 1,
                    "Boundary values test just inside and just outside each limit."),
                new("Bug reporting", "Which item is essential in a good bug report?",
                    ["The tester's opinion of the developer", "Clear steps to reproduce", "A guess at the fix", "Only a screenshot"], 1,
                    "Steps to reproduce let anyone see the defect for themselves."),
                new("Test automation", "Which tests are usually the best first candidates for automation?",
                    ["One-off exploratory sessions", "Stable, repetitive regression checks", "Tests for features still being designed", "Usability opinions"], 1,
                    "Stable, frequently repeated checks give automation the best return."),
                new("API testing", "An API returns 200 OK for a request with a missing required field. What is this?",
                    ["Expected behaviour", "A likely defect — it should reject invalid input", "A network problem", "A UI issue"], 1,
                    "Invalid input should be rejected, typically with 400 Bad Request."),
                new("SQL", "Which query counts the orders in an Orders table?",
                    ["SELECT COUNT(*) FROM Orders", "COUNT Orders", "SELECT Orders.COUNT", "GET COUNT FROM Orders"], 0,
                    "COUNT(*) returns the number of rows."),
                new("Test case design", "What is regression testing?",
                    ["Testing only new features", "Re-testing existing features after a change", "Testing performance under load", "Testing on old browsers only"], 1,
                    "Regression testing checks that changes did not break what already worked."),
            ]),

        new(
            Slug: "devops",
            Name: "DevOps",
            Tagline: "Automate how software is built, shipped and run.",
            Description: "DevOps engineers connect development and operations with automation. Entry-level DevOps engineers maintain CI/CD pipelines, containerise applications, script routine tasks and help monitor systems in production.",
            EntryRoles: "Associate DevOps Engineer;Junior Site Reliability Engineer;Build & Release Engineer",
            CoreSkills: ["Linux & shell", "Docker", "CI/CD pipelines", "Infrastructure as code", "Monitoring & logging", "Git & version control"],
            SupportingSkills: ["Python", "Kubernetes", "Networking basics", "Cloud fundamentals", "Teamwork"],
            Roadmap:
            [
                new(LearningStage.Beginner, "Linux and the command line", "Navigate the file system, manage processes and permissions, and write simple shell scripts.", "Work comfortably on a Linux server.", "Linux & shell"),
                new(LearningStage.Beginner, "Git workflows", "Use branching strategies and pull requests that suit automated delivery.", "Support a team's release workflow.", "Git & version control"),
                new(LearningStage.Intermediate, "Containers with Docker", "Write Dockerfiles, build images and run multi-container apps with Compose.", "Package any app so it runs the same everywhere.", "Docker"),
                new(LearningStage.Intermediate, "CI/CD pipelines", "Build pipelines that compile, test and publish on every change.", "Automate the path from commit to deployment.", "CI/CD pipelines"),
                new(LearningStage.Intermediate, "Infrastructure as code", "Provision environments repeatably with Terraform or Bicep.", "Create environments from code, not clicks.", "Infrastructure as code"),
                new(LearningStage.Advanced, "Observe and operate", "Set up logs, metrics and alerts and respond to a simulated incident.", "Keep services healthy in production.", "Monitoring & logging"),
            ],
            Baseline:
            [
                new("Linux & shell", "Which command shows the processes currently running on a Linux machine?",
                    ["ls", "ps", "cd", "mkdir"], 1,
                    "ps lists running processes; top and htop show them live."),
                new("Docker", "What is the relationship between a Docker image and a container?",
                    ["They are the same thing", "A container is a running instance of an image", "An image is a running container", "Images run only on Windows"], 1,
                    "An image is the template; a container is a running instance of it."),
                new("CI/CD pipelines", "What should usually happen when automated tests fail in a CI pipeline?",
                    ["Deploy anyway", "Stop the pipeline and report the failure", "Delete the failing tests", "Restart the server"], 1,
                    "Failing fast stops broken changes from reaching users."),
                new("Infrastructure as code", "What is the main benefit of infrastructure as code?",
                    ["It removes the need for servers", "Environments become repeatable and reviewable", "It makes applications run faster", "It replaces testing"], 1,
                    "Code-defined infrastructure can be versioned, reviewed and recreated reliably."),
                new("Monitoring & logging", "A service is slow but not failing. Which signal helps most to investigate?",
                    ["Response-time metrics and logs", "The office Wi-Fi password", "The number of developers", "The git commit count"], 0,
                    "Latency metrics and correlated logs show where time is spent."),
                new("Git & version control", "What does a merge conflict mean?",
                    ["The repository is corrupted", "Two changes touched the same lines and Git needs a human decision", "The remote server is down", "A commit message is missing"], 1,
                    "Git cannot choose between overlapping edits, so you resolve them."),
            ]),

        new(
            Slug: "cloud-engineering",
            Name: "Cloud Engineering",
            Tagline: "Build secure, scalable systems on cloud platforms.",
            Description: "Cloud engineers design, deploy and secure workloads on platforms such as AWS and Azure. Entry-level cloud engineers provision resources, configure networks and access, automate deployments and help keep costs and security under control.",
            EntryRoles: "Associate Cloud Engineer;Cloud Support Associate;Junior Cloud Administrator",
            CoreSkills: ["Cloud fundamentals", "Networking basics", "Cloud security & IAM", "Infrastructure as code", "Linux & shell"],
            SupportingSkills: ["Docker", "Kubernetes", "Monitoring & logging", "Python", "Communication"],
            Roadmap:
            [
                new(LearningStage.Beginner, "Cloud concepts", "Understand regions, availability zones, IaaS, PaaS, SaaS and the shared responsibility model.", "Explain how the cloud differs from on-premise.", "Cloud fundamentals"),
                new(LearningStage.Beginner, "Networking essentials", "Work with IP addressing, subnets, DNS, routing and load balancers.", "Design a simple, secure network layout.", "Networking basics"),
                new(LearningStage.Intermediate, "Identity and security", "Apply least privilege with roles and policies, and manage secrets safely.", "Grant only the access each service needs.", "Cloud security & IAM"),
                new(LearningStage.Intermediate, "Core services hands-on", "Deploy a web app using compute, storage and a managed database.", "Run a real workload in the cloud.", "Cloud fundamentals"),
                new(LearningStage.Intermediate, "Automate provisioning", "Describe the same environment in Terraform or Bicep and deploy it repeatably.", "Replace manual console work with code.", "Infrastructure as code"),
                new(LearningStage.Advanced, "Operate and optimise", "Add monitoring, backups and cost alerts, and review a workload against a well-architected checklist.", "Keep a cloud workload reliable and affordable.", "Monitoring & logging"),
            ],
            Baseline:
            [
                new("Cloud fundamentals", "In the shared responsibility model, who is responsible for patching the operating system on a virtual machine (IaaS)?",
                    ["The cloud provider", "The customer", "Nobody", "The internet service provider"], 1,
                    "With IaaS the provider secures the hardware; the customer manages the OS."),
                new("Networking basics", "What does DNS do?",
                    ["Encrypts traffic", "Translates domain names into IP addresses", "Stores files", "Balances CPU load"], 1,
                    "DNS resolves human-friendly names to the IP addresses machines use."),
                new("Cloud security & IAM", "What does the principle of least privilege mean?",
                    ["Give everyone admin access to save time", "Grant only the permissions a user or service needs", "Disable all logins", "Share one account across the team"], 1,
                    "Minimal permissions limit the damage of mistakes or compromised credentials."),
                new("Infrastructure as code", "Which tool is commonly used for infrastructure as code across multiple clouds?",
                    ["Terraform", "Photoshop", "Excel", "Jira"], 0,
                    "Terraform describes resources declaratively for many providers."),
                new("Linux & shell", "Which command changes the permissions of a file in Linux?",
                    ["chmod", "chown", "grep", "cat"], 0,
                    "chmod changes permissions; chown changes ownership."),
                new("Networking basics", "Why place a database in a private subnet?",
                    ["So it is faster", "So it is not reachable directly from the internet", "So it costs nothing", "So backups are automatic"], 1,
                    "Private subnets keep sensitive services away from public traffic."),
            ]),
    ];
}
