// A deliberately minimal console application used as a launch target by process-related
// integration tests. See docs/libraries/testing-support-mock-console-app.md.

Console.WriteLine("Hello, World! I'm a mock console app that can be used in testing.");

if (Console.IsInputRedirected)
{
    // Console.ReadKey throws InvalidOperationException when standard input is redirected, which is
    // exactly how a test harness launches a child process (issue #275). Wait for a line instead, so
    // the harness can end the process cleanly by writing to StandardInput rather than killing it.
    Console.WriteLine("Send a line on standard input to exit.");
    Console.ReadLine();
}
else
{
    Console.WriteLine("Press any key to exit.");
    Console.ReadKey();
}
