using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

class Program
{
    private static readonly HttpClient httpClient = new HttpClient();
    private static bool isHttpClientInitialized = false;

    static async Task Main(string[] args)
    {
        try
        {
            do
            {
                Console.WriteLine("Enter 'github-activity <username>' to fetch the user's activity or type 'exit' to close the application.");
                string? rawInput = Console.ReadLine()?.Trim();
                string command;
                string username;

                if (string.IsNullOrWhiteSpace(rawInput))
                {
                    continue;
                }

                if (rawInput == "exit")
                {
                    break;
                }

                string[] commandParts = rawInput.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

                if (commandParts.Length != 2)
                {
                    Console.WriteLine("Invalid input format.");
                    continue;
                }

                command = commandParts[0];
                username = commandParts[1].ToLower();


                if (command == "github-activity")
                {
                    // Check if httpClient is already initizalized
                    if (!isHttpClientInitialized)
                    {
                        // Set base address and headers
                        httpClient.BaseAddress = new Uri("https://api.github.com/");
                        httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("CSharpApp", "1.0"));
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                        isHttpClientInitialized = true;
                    }

                    var response = await httpClient.GetAsync($"users/{username}/events");

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();

                        // Deserialize events json
                        var events = JsonSerializer.Deserialize<GitHubUserEvent[]>(json);

                        // Handle for different event types
                        foreach (var userEvent in events)
                        {
                            Console.WriteLine($"\nEvent ID: {userEvent.Id}");
                            Console.WriteLine($"Event Type: {userEvent.Type}");

                            switch (userEvent.Type)
                            {
                                case "PushEvent":
                                    Console.WriteLine($"A push was made to {userEvent.Repo.Name}");
                                    break;

                                case "PullRequestEvent":
                                    Console.WriteLine("A pull request was opened, closed, or merged.");
                                    break;

                                case "IssuesEvent":
                                    Console.WriteLine("An issue was opened, closed, or updated.");
                                    break;

                                case "IssueCommentEvent":
                                    Console.WriteLine("A comment was added to an issue or pull request.");
                                    break;

                                case "ForkEvent":
                                    Console.WriteLine("A repository was forked.");
                                    break;

                                case "WatchEvent":
                                    Console.WriteLine("A user starred a repository.");
                                    break;

                                case "CreateEvent":
                                    Console.WriteLine("A branch, tag, or repository was created.");
                                    break;

                                case "DeleteEvent":
                                    Console.WriteLine("A branch or tag was deleted.");
                                    break;

                                case "ReleaseEvent":
                                    Console.WriteLine("A release was published.");
                                    break;

                                case "MemberEvent":
                                    Console.WriteLine("A user was added as a collaborator.");
                                    break;

                                case "PublicEvent":
                                    Console.WriteLine("A private repository was made public.");
                                    break;

                                case "CommitCommentEvent":
                                    Console.WriteLine("A comment was added to a commit.");
                                    break;

                                case "GollumEvent":
                                    Console.WriteLine("A wiki page was created or updated.");
                                    break;

                                case "PullRequestReviewEvent":
                                    Console.WriteLine("A pull request review was submitted.");
                                    break;

                                case "PullRequestReviewCommentEvent":
                                    Console.WriteLine("A comment was added to a pull request review.");
                                    break;

                                case "PullRequestReviewThreadEvent":
                                    Console.WriteLine("A pull request review thread was updated.");
                                    break;

                                case "SponsorshipEvent":
                                    Console.WriteLine("A sponsorship event occurred.");
                                    break;

                                default:
                                    Console.WriteLine("Unrecognized event type.");
                                    break;
                            }
                        }

                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid command. Try again.");
                }
            } while (true);
        }
        // Handle network-related errors
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Network error: {ex.Message}");
        }
        // Handle issues regarding JSON (malformed, wrong data type, etc.)
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON parsing error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }

    // Class for Event JSON
    public class GitHubUserEvent
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("type")]
        public required string Type { get; set; }
        [JsonPropertyName("repo")]
        public required Repo Repo { get; set; }
    }

    // Class for Repo JSON
    public class Repo
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; }
        [JsonPropertyName("name")]
        public required string Name { get; set; }
    }
}

