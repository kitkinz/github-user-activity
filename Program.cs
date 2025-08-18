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
                Console.WriteLine("Enter the GitHub username you want to fetch activity for, or type 'exit' to close the application.");
                string? rawInput = Console.ReadLine();
                string userInput;

                if (string.IsNullOrWhiteSpace(rawInput))
                {
                    continue;
                }

                userInput = rawInput.ToLower().Trim();

                if (userInput == "exit")
                {
                    break;
                }

                // Check if httpClient is already initizalized
                if (!isHttpClientInitialized)
                {
                    // Set base address and headers
                    httpClient.BaseAddress = new Uri("https://api.github.com/");
                    httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("CSharpApp", "1.0"));
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                    isHttpClientInitialized = true;
                }

                var response = await httpClient.GetAsync($"users/{userInput}/events");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    // Deserialize events json
                    var events = JsonSerializer.Deserialize<GitHubUserEvent[]>(json);

                    foreach (var userEvent in events)
                    {
                        Console.WriteLine($"ID: {userEvent.Id} - Type: {userEvent.Type}");
                    }
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
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

    public class GitHubUserEvent
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("type")]
        public required string Type { get; set; }
    }
}
