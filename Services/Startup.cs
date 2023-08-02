using System.Runtime.InteropServices;

using Microsoft.Extensions.DependencyInjection;

namespace ChatGPT.Services;

public class Startup
{
    #region Look & Feel

    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    private static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
    #endregion
    public Startup()
    {
        Console.Title = "Orbital, Inc. [ChatGPT] ~ Nebula";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            SetWindowPos(GetConsoleWindow(), 0, 550, 300, 0, 0, 0x0001);
            Console.SetWindowSize(85, 20);
            Console.SetBufferSize(85, 20);
        }
    }

    internal async Task RunAsync()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        var provider = services.BuildServiceProvider();
        await provider.GetRequiredService<CommandService>().StartAsync();
        await Task.Delay(Timeout.Infinite);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services
        .AddSingleton(new Random())
        .AddSingleton<CommandService>();
    }
}
