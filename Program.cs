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
                            userEvent.Payload.TryGetProperty("action", out var actionProp);
                            switch (userEvent.Type)
                            {
                                case "PushEvent":
                                    var pushPayLoad = userEvent.Payload.Deserialize<PushEventPayload>();
                                    string message = pushPayLoad?.Size > 1
                                        ? $"Pushed {pushPayLoad?.Size} commits to" : "Pushed a commit to";
                                    Console.WriteLine($"{message} {userEvent.Repo.Name}");
                                    break;

                                case "PullRequestEvent":
                                    Console.WriteLine($"A pull request was {actionProp.GetString()} at {userEvent.Repo.Name}");
                                    break;

                                case "IssuesEvent":
                                    Console.WriteLine($"An issue was {actionProp.GetString()} at {userEvent.Repo.Name}");
                                    break;

                                case "WatchEvent":
                                    Console.WriteLine($"Starred {userEvent.Repo.Name}");
                                    break;

                                case "CreateEvent":
                                    userEvent.Payload.TryGetProperty("ref_type", out var refTypeProp);
                                    Console.WriteLine($"Created a {refTypeProp}");
                                    break;

                                default:
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
        [JsonPropertyName("payload")]
        public JsonElement Payload { get; set; }
    }

    // Class for Repo JSON
    public class Repo
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; }
        [JsonPropertyName("name")]
        public required string Name { get; set; }
    }

    // Class for PushEvent Payload
    public class PushEventPayload
    {
        [JsonPropertyName("size")]
        public required int Size { get; set; }
    }
}

