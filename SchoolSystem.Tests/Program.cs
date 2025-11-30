using System.Diagnostics;
using System.Text;
using System.Text.Json;

Console.WriteLine("Waiting for services to start...");
await Task.Delay(5000);

var httpClient = new HttpClient();

Console.WriteLine("--- Starting Tests ---");

// Test 1: Classic Mode
Console.WriteLine("\nTest 1: Classic Mode (Synchronous)");
var stopwatch = Stopwatch.StartNew();
try
{
    var content = new StringContent(
        JsonSerializer.Serialize(new { StudentId = "student1", Subject = "Math", Score = 95 }),
        Encoding.UTF8,
        "application/json");

    var response = await httpClient.PostAsync("http://localhost:5001/grades?mode=classic", content);
    response.EnsureSuccessStatusCode();
    
    stopwatch.Stop();
    Console.WriteLine($"Classic Mode Duration: {stopwatch.ElapsedMilliseconds}ms");
}
catch (Exception ex)
{
    Console.WriteLine($"Classic Mode Failed: {ex.Message}");
}

// Test 2: Event Mode
Console.WriteLine("\nTest 2: Event Mode (Asynchronous)");
stopwatch.Restart();
try
{
    var content = new StringContent(
        JsonSerializer.Serialize(new { StudentId = "student2", Subject = "Physics", Score = 88 }),
        Encoding.UTF8,
        "application/json");

    var response = await httpClient.PostAsync("http://localhost:5001/grades?mode=event", content);
    response.EnsureSuccessStatusCode();
    
    stopwatch.Stop();
    Console.WriteLine($"Event Mode Duration: {stopwatch.ElapsedMilliseconds}ms");
}
catch (Exception ex)
{
    Console.WriteLine($"Event Mode Failed: {ex.Message}");
}

Console.WriteLine("\n--- Tests Completed ---");
