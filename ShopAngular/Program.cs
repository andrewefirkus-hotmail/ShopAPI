using System.Diagnostics;

// Binary lands in ShopAngular\bin\launcher\; two levels up is the Angular root.
var angularRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", ".."));

Console.WriteLine("Starting Angular dev server...");
Console.WriteLine($"Root : {angularRoot}");
Console.WriteLine("URL  : http://localhost:4200");
Console.WriteLine();

try
{
    // npm is a .cmd file on Windows — it must be invoked via cmd.exe when
    // UseShellExecute is false, otherwise the process exits immediately.
    using var process = Process.Start(new ProcessStartInfo
    {
        FileName = "cmd.exe",
        Arguments = "/c npm start",
        WorkingDirectory = angularRoot,
        UseShellExecute = false,
    }) ?? throw new InvalidOperationException("Process failed to start.");

    await process.WaitForExitAsync();
}
catch (Exception ex)
{
    Console.Error.WriteLine($"\nFailed to start Angular dev server: {ex.Message}");
    Console.Error.WriteLine("Press any key to exit...");
    Console.ReadKey();
}
