using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Infrastructure.Identity;

namespace NSBM.ICU.Infrastructure.Persistence.Seed.Data;

/// <summary>Fictional company-style practice tasks. Pure data — the seeding logic lives in <see cref="TaskSeeder"/>.</summary>
internal static class IndustryTaskData
{
    public sealed record CheckSeed(string Description, string Pattern, bool ShouldMatch, string Hint);

    public sealed record TaskSeed(
        string Company,
        string AuthorEmail,
        string CareerPathSlug,
        string Title,
        string Summary,
        string Instructions,
        LearningStage Level,
        WorkSituation Situation,
        int EstimatedMinutes,
        string StarterLanguage,
        string StarterCode,
        string[] Skills,
        CheckSeed[] Checks,
        string[] KnowledgeQuestions,
        bool Approved);

    public static readonly TaskSeed[] Tasks =
    [
        new(
            Company: PeopleSeeder.SerendibSoftwareLabs,
            AuthorEmail: "partner2@nsbm.demo",
            CareerPathSlug: "software-engineering",
            Title: "Fix the order total discount bug",
            Summary: "Customers with a discount code are charged the wrong amount. Find and fix the bug in the order total calculation.",
            Instructions: """
                Support has escalated a ticket: customers using a 10% discount code are charged far too little, and a 100% code produces a negative total.

                Your job:
                1. Read calculateOrderTotal and work out why discounted totals are wrong.
                2. Fix it so the discount is applied as a percentage of the subtotal.
                3. Round the final total to two decimal places.
                4. Keep the function signature unchanged — other modules call it.

                Example: items [{ price: 1200, qty: 2 }, { price: 450, qty: 1 }] with discountPercent 10 should return 2565.00.
                """,
            Level: LearningStage.Beginner,
            Situation: WorkSituation.BugFix,
            EstimatedMinutes: 30,
            StarterLanguage: "javascript",
            StarterCode: """
                // order-service/pricing.js
                function calculateOrderTotal(items, discountPercent) {
                  let total = 0;
                  for (const item of items) {
                    total += item.price * item.qty;
                  }
                  // BUG: customers report wrong totals when a discount is used
                  total = total - discountPercent;
                  return total;
                }

                module.exports = { calculateOrderTotal };
                """,
            Skills: ["JavaScript", "Debugging", "Unit testing"],
            Checks:
            [
                new("Treats the discount as a percentage", @"/\s*100", true, "A 10% discount means multiplying by 10 / 100, not subtracting 10."),
                new("No longer subtracts the raw discount value", @"total\s*-\s*discountPercent\b", false, "Subtracting discountPercent directly is the original bug."),
                new("Rounds the total to two decimal places", @"toFixed\(\s*2\s*\)|Math\.round", true, "Money values should be rounded, e.g. Math.round(total * 100) / 100."),
            ],
            KnowledgeQuestions:
            [
                "In your own words, what caused the wrong totals?",
                "Give one test case (inputs and expected output) that proves your fix works.",
                "What would you look for in a code review to catch similar bugs early?",
            ],
            Approved: true),

        new(
            Company: PeopleSeeder.SerendibSoftwareLabs,
            AuthorEmail: "partner2@nsbm.demo",
            CareerPathSlug: "software-engineering",
            Title: "Validate input on the registration endpoint",
            Summary: "The sign-up API accepts empty names and invalid emails. Add validation and return proper error responses.",
            Instructions: """
                The mobile team reports that users can register with an empty name or an email like "abc". Bad data is now in production.

                Your job:
                1. Reject an empty or whitespace-only FullName.
                2. Reject an Email that is not a valid address.
                3. Return 400 Bad Request with a helpful message when validation fails.
                4. Keep returning 201 Created for valid requests.
                """,
            Level: LearningStage.Intermediate,
            Situation: WorkSituation.Feature,
            EstimatedMinutes: 45,
            StarterLanguage: "csharp",
            StarterCode: """
                [ApiController]
                [Route("api/users")]
                public class UsersController(IUserService users) : ControllerBase
                {
                    public record RegisterRequest(string FullName, string Email);

                    [HttpPost]
                    public async Task<IActionResult> Register(RegisterRequest request)
                    {
                        var id = await users.CreateAsync(request.FullName, request.Email);
                        return Created($"/api/users/{id}", new { id });
                    }
                }
                """,
            Skills: ["C#", "REST APIs", "Object-oriented design"],
            Checks:
            [
                new("Checks the name for empty or whitespace input", @"IsNullOrWhiteSpace", true, "string.IsNullOrWhiteSpace handles null, empty and spaces in one call."),
                new("Validates the email address", @"MailAddress|EmailAddress|@", true, "Use System.Net.Mail.MailAddress, an [EmailAddress] attribute or a validator."),
                new("Returns 400 Bad Request for invalid input", @"BadRequest|ValidationProblem", true, "Clients rely on a 400 status to show validation messages."),
            ],
            KnowledgeQuestions:
            [
                "Why is it better to validate on the server even if the mobile app also validates?",
                "Which status code did you return for invalid input, and why?",
            ],
            Approved: true),

        new(
            Company: PeopleSeeder.CeylonQualityFirst,
            AuthorEmail: "partner3@nsbm.demo",
            CareerPathSlug: "quality-assurance",
            Title: "Design test cases for the login form",
            Summary: "Write a focused set of test cases for a login form that locks accounts after five failed attempts.",
            Instructions: """
                Our client's login form has these rules:
                - Email and password are required.
                - Passwords are 8–64 characters.
                - After 5 failed attempts the account is locked for 15 minutes.

                Write at least six test cases in the template. Each needs an ID, steps, test data and an expected result.
                Cover positive, negative and boundary cases, and the lockout rule.
                """,
            Level: LearningStage.Beginner,
            Situation: WorkSituation.Testing,
            EstimatedMinutes: 40,
            StarterLanguage: "text",
            StarterCode: """
                TC-01
                Title:
                Steps:
                Test data:
                Expected result:

                TC-02
                Title:
                Steps:
                Test data:
                Expected result:
                """,
            Skills: ["Test case design", "Bug reporting"],
            Checks:
            [
                new("Includes boundary value cases for password length", @"(?i)\b(7|8|64|65)\b.*(char|length)|boundary", true, "Test just below, at and just above each limit (7, 8, 64, 65)."),
                new("Covers the account lockout rule", @"(?i)lock", true, "What happens on the 5th failed attempt? And after 15 minutes?"),
                new("Covers empty required fields", @"(?i)empty|blank|required", true, "Leave each required field empty in turn."),
                new("Every case states an expected result", @"(?im)^[ \t]*expected result:[ \t]*$", false, "A test case without an expected result cannot pass or fail."),
            ],
            KnowledgeQuestions:
            [
                "Which test case do you think is most likely to find a defect, and why?",
                "Explain how you chose the boundary values for the password length.",
            ],
            Approved: true),

        new(
            Company: PeopleSeeder.CeylonQualityFirst,
            AuthorEmail: "partner3@nsbm.demo",
            CareerPathSlug: "quality-assurance",
            Title: "Automate the checkout smoke test",
            Summary: "Turn a manual checkout smoke test into a reliable Playwright test.",
            Instructions: """
                Every release, a tester manually checks that a customer can add an item to the cart and reach the payment page. Automate it.

                Your job:
                1. Open the shop home page.
                2. Add the first product to the cart.
                3. Go to checkout and assert that the "Payment details" heading is visible.
                4. Avoid fixed sleeps — rely on Playwright's auto-waiting and assertions.
                """,
            Level: LearningStage.Intermediate,
            Situation: WorkSituation.Testing,
            EstimatedMinutes: 45,
            StarterLanguage: "javascript",
            StarterCode: """
                // tests/checkout.spec.js
                const { test, expect } = require('@playwright/test');

                test('customer can reach the payment page', async ({ page }) => {
                  // TODO: implement the smoke test
                });
                """,
            Skills: ["Test automation", "JavaScript"],
            Checks:
            [
                new("Navigates to the shop", @"page\.goto\(", true, "Start with await page.goto('https://shop.example.test')."),
                new("Asserts the expected outcome", @"expect\(", true, "A test without an assertion only proves the page did not crash."),
                new("Avoids fixed sleeps", @"waitForTimeout", false, "Fixed waits make tests slow and flaky — use expect(...).toBeVisible()."),
            ],
            KnowledgeQuestions:
            [
                "Why are fixed sleeps a problem in UI automation?",
                "Which selector strategy did you use, and why is it robust to UI changes?",
            ],
            Approved: true),

        new(
            Company: PeopleSeeder.LankaCloudWorks,
            AuthorEmail: DemoAccounts.Industry.Email,
            CareerPathSlug: "devops",
            Title: "Containerise the Node.js API",
            Summary: "Write a production-ready Dockerfile for a small Express API.",
            Instructions: """
                A client wants their Express API (listening on port 3000) to run in containers.

                Your job — write a Dockerfile that:
                1. Uses an official Node.js LTS base image.
                2. Installs dependencies reproducibly with npm ci.
                3. Exposes port 3000.
                4. Does not run the app as root.
                5. Starts the app with node server.js.
                """,
            Level: LearningStage.Beginner,
            Situation: WorkSituation.Deployment,
            EstimatedMinutes: 30,
            StarterLanguage: "dockerfile",
            StarterCode: """
                # Dockerfile
                # TODO: build a production image for the API
                """,
            Skills: ["Docker", "Linux & shell"],
            Checks:
            [
                new("Uses an official Node.js base image", @"(?im)^\s*FROM\s+node:", true, "For example FROM node:20-alpine."),
                new("Installs dependencies with npm ci", @"npm\s+ci", true, "npm ci installs exactly what package-lock.json specifies."),
                new("Exposes port 3000", @"(?im)^\s*EXPOSE\s+3000", true, "EXPOSE documents the port the container listens on."),
                new("Runs as a non-root user", @"(?im)^\s*USER\s+(?!root\b)\w+", true, "The official image ships a 'node' user — switch to it with USER node."),
            ],
            KnowledgeQuestions:
            [
                "Why is npm ci preferred over npm install in a Dockerfile?",
                "What risk does running a container as root create?",
            ],
            Approved: true),

        new(
            Company: PeopleSeeder.LankaCloudWorks,
            AuthorEmail: DemoAccounts.Industry.Email,
            CareerPathSlug: "cloud-engineering",
            Title: "Lock down a public storage bucket",
            Summary: "A storage bucket policy exposes customer files to the internet. Fix the policy.",
            Instructions: """
                A security scan flagged that the "invoices" bucket is readable by anyone. Only the billing application's role should read it, and only over HTTPS.

                Your job — rewrite the policy so that:
                1. It no longer grants access to every principal ("*").
                2. Only the role arn:aws:iam::111122223333:role/billing-app can read objects.
                3. Requests that do not use HTTPS are denied (aws:SecureTransport).
                """,
            Level: LearningStage.Intermediate,
            Situation: WorkSituation.Troubleshooting,
            EstimatedMinutes: 35,
            StarterLanguage: "json",
            StarterCode: """
                {
                  "Version": "2012-10-17",
                  "Statement": [
                    {
                      "Effect": "Allow",
                      "Principal": "*",
                      "Action": "s3:GetObject",
                      "Resource": "arn:aws:s3:::invoices/*"
                    }
                  ]
                }
                """,
            Skills: ["Cloud security & IAM", "Cloud fundamentals"],
            Checks:
            [
                new("No longer allows every principal", "\"Principal\"\\s*:\\s*\"\\*\"", false, "\"Principal\": \"*\" means anyone on the internet."),
                new("Grants access to the billing role", @"role/billing-app", true, "Reference the role ARN in the Principal block."),
                new("Denies requests without HTTPS", @"aws:SecureTransport", true, "Add a Deny statement with a Bool condition on aws:SecureTransport = false."),
            ],
            KnowledgeQuestions:
            [
                "Explain the principle of least privilege using this policy as the example.",
                "Why deny insecure transport explicitly instead of relying on clients to use HTTPS?",
            ],
            Approved: true),

        new(
            Company: PeopleSeeder.LankaCloudWorks,
            AuthorEmail: DemoAccounts.Industry.Email,
            CareerPathSlug: "devops",
            Title: "Add a test stage to the CI pipeline",
            Summary: "The pipeline deploys without running tests. Add a test job that must pass before deployment.",
            Instructions: """
                The current GitHub Actions workflow builds and deploys on every push to main — but never runs the tests.

                Your job:
                1. Add a "test" job that installs dependencies and runs npm test.
                2. Make the deploy job depend on the test job so a failing test blocks deployment.
                """,
            Level: LearningStage.Intermediate,
            Situation: WorkSituation.Deployment,
            EstimatedMinutes: 30,
            StarterLanguage: "yaml",
            StarterCode: """
                name: ci
                on:
                  push:
                    branches: [main]
                jobs:
                  deploy:
                    runs-on: ubuntu-latest
                    steps:
                      - uses: actions/checkout@v4
                      - run: npm ci
                      - run: ./scripts/deploy.sh
                """,
            Skills: ["CI/CD pipelines", "Git & version control"],
            Checks:
            [
                new("Runs the test suite", @"npm\s+(run\s+)?test", true, "Add a step that runs npm test."),
                new("Deployment depends on the tests", @"needs:\s*\[?\s*test", true, "Use needs: test on the deploy job."),
            ],
            KnowledgeQuestions:
            [
                "Why should deployment depend on the test job instead of running in parallel?",
            ],
            Approved: false),

        new(
            Company: PeopleSeeder.SerendibSoftwareLabs,
            AuthorEmail: "partner2@nsbm.demo",
            CareerPathSlug: "software-engineering",
            Title: "Remove SQL injection from the product search",
            Summary: "A code review found string-built SQL in the product search. Rewrite it with parameters.",
            Instructions: """
                The product search builds its SQL by concatenating user input, which allows SQL injection.

                Your job:
                1. Rewrite the query to use a parameter for the search term.
                2. Keep the LIKE search behaviour (matches anywhere in the name).
                """,
            Level: LearningStage.Intermediate,
            Situation: WorkSituation.CodeReview,
            EstimatedMinutes: 25,
            StarterLanguage: "csharp",
            StarterCode: """
                public async Task<List<Product>> SearchAsync(string term)
                {
                    var sql = "SELECT Id, Name, Price FROM Products WHERE Name LIKE '%" + term + "%'";
                    using var command = new SqlCommand(sql, _connection);
                    return await ReadProductsAsync(command);
                }
                """,
            Skills: ["SQL", "C#"],
            Checks:
            [
                new("Uses a query parameter", @"@\w+", true, "Write WHERE Name LIKE @term and add the parameter to the command."),
                new("No longer concatenates the search term into SQL", "\"\\s*\\+\\s*term\\s*\\+\\s*\"", false, "Concatenating user input into SQL is what enables injection."),
            ],
            KnowledgeQuestions:
            [
                "Describe an input an attacker could use against the original query and what it would do.",
            ],
            Approved: false),
    ];
}
