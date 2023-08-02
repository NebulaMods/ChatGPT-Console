namespace ChatGPT.Services;

public class CommandService
{
    public async Task StartAsync()
    {
        Console.WriteLine("Please enter your OpenAI API key: ");
        var apiKey = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Console.WriteLine("Error, API key cannot be empty.");
            Environment.Exit(0);
        }
        Console.Clear();
        Console.WriteLine("Please enter a command :)");

        while (true)
        {
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                input = "";
            switch (input.ToLower())
            {
                case "ask-question":
                    Console.WriteLine("What is your question?");
                    var question = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(question))
                    {
                        Console.WriteLine("Error, question cannot be empty.");
                        break;
                    }
                    var answer = await ChatGPTApi.AskQuestionAsync(question, apiKey);
                    if (string.IsNullOrWhiteSpace(answer))
                    {
                        Console.WriteLine("Error, answer cannot be empty.");
                        break;
                    }
                    Console.WriteLine(answer);
                    await File.WriteAllLinesAsync(Environment.CurrentDirectory + $@"\question-{DateTime.Now.ToFileTime()}.txt", new[] { question, answer });
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
    }
}
