namespace ChatGPT.Services;

public class CommandService
{
    private string apiKey;
    private string gptVersion = ChatGPTApi.GPT3Model;

    public async Task StartAsync()
    {
        GetApiKey();
        SelectGptVersion();
        Console.Clear();
        Console.WriteLine("Please enter a command :)");

        while (true)
        {
            await ProcessCommandAsync();
        }
    }

    private void GetApiKey()
    {
        Console.WriteLine("Please enter your OpenAI API key: ");
        apiKey = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Console.WriteLine("Error, API key cannot be empty.");
            Environment.Exit(0);
        }
    }

    private void SelectGptVersion()
    {
        Console.WriteLine("Would you like to use GPT3.5 or GPT4? GPT3.5 is the default and is cheaper, but GPT4 is more powerful and requires special access to use.");
        Console.WriteLine("Type '3.5' or '4' to select a version.");
        gptVersion = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(gptVersion))
        {
            Console.WriteLine("Error, GPT version cannot be empty.");
            Environment.Exit(0);
        }

        switch (gptVersion)
        {
            case "3.5":
                gptVersion = ChatGPTApi.GPT3Model;
                break;
            case "4":
                gptVersion = ChatGPTApi.GPT4Model;
                break;
            default:
                Console.WriteLine("Error, GPT version must be either '3.5' or '4'.");
                Environment.Exit(0);
                break;
        }
    }

    private async Task ProcessCommandAsync()
    {
        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
            input = "";

        switch (input.ToLower())
        {
            case "ask-question":
                await AskQuestionAsync();
                break;
            case "quit":
                Console.WriteLine("Goodbye!");
                Environment.Exit(0);
                break;
            case "help":
                Console.WriteLine("\nask-question\nhelp\nquit\n");
                break;
            default:
                Console.WriteLine("Error, command doesn't exist, please try again. Type 'Help' to view the available commands.");
                break;
        }
    }

    private async Task AskQuestionAsync()
    {
        Console.WriteLine("What is your question?");
        var question = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(question))
        {
            Console.WriteLine("Error, question cannot be empty.");
            return;
        }

        var answer = await ChatGPTApi.AskQuestionAsync(question, apiKey, gptVersion);

        if (string.IsNullOrWhiteSpace(answer))
        {
            Console.WriteLine("Error, answer cannot be empty.");
            return;
        }

        Console.WriteLine(answer);
        await File.WriteAllLinesAsync(Environment.CurrentDirectory + $@"\question-{DateTime.Now.ToFileTime()}.txt", new[] { question, answer });
    }
}
