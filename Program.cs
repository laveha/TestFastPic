using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

class Program
{
    static User currentUser;
    static Stopwatch stopwatch;
    static List<User> leaderboard;

    static void Main()
    {
        LoadLeaderboard();
        Console.WriteLine("Введите ваше имя:");
        string userName = Console.ReadLine();
        currentUser = new User { Name = userName };

        do
        {
            Console.Clear();
            Console.WriteLine("Нажмите Enter, чтобы начать тест.");
            Console.ReadLine(); 
            StartTypingTest();
        } while (ShouldRetry());

        Leaderboard();
        SaveLeaderboard();
    }

    static void StartTypingTest()
    {
        TypingTest typingTest = new TypingTest();
        typingTest.Start();

        stopwatch = Stopwatch.StartNew();
        string userInput;

        do
        {
            userInput = typingTest.GetUserInput();
        } while (userInput == null);

        stopwatch.Stop();

        double charactersPerMinute = CalculateCharactersMinute(userInput);
        double charactersPerSecond = CalculateCharactersSecond(userInput);

        currentUser.CharactersMinute = charactersPerMinute;
        currentUser.CharactersSecond = charactersPerSecond;

        leaderboard.Add(currentUser);
    }
    static double CalculateCharactersMinute(string userInput)
    {
        int charactersTyped = userInput.Length;
        double minutes = stopwatch.Elapsed.TotalMinutes;
        return charactersTyped / minutes;
    }

    static double CalculateCharactersSecond(string userInput)
    {
        int charactersTyped = userInput.Length;
        double seconds = stopwatch.Elapsed.TotalSeconds;
        return charactersTyped / seconds;
    }

    static bool ShouldRetry()
    {
        Console.WriteLine("Хотите повторить тест? (да/нет)");
        string response = Console.ReadLine();
        return response.ToLower() == "да";
    }

    static void Leaderboard()
    {
        Console.WriteLine("Таблица рекордов:");
        foreach (var user in leaderboard.OrderByDescending(u => u.CharactersMinute))
        {
            Console.WriteLine($"{user.Name}: {Convert.ToInt32(user.CharactersMinute)} символов в минуту, {user.CharactersSecond:F2} символов в секунду");
        }
    }

    static void LoadLeaderboard()
    {
        if (File.Exists("leaderboard.json"))
        {
            string json = File.ReadAllText("leaderboard.json");
            leaderboard = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User>>(json);
        }
        else
        {
            leaderboard = new List<User>();
        }
    }

    static void SaveLeaderboard()
    {
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(leaderboard);
        File.WriteAllText("leaderboard.json", json);
    }
}
class User
{
    public string Name { get; set; }
    public double CharactersMinute { get; set; }
    public double CharactersSecond { get; set; }
}
class TypingTest
{
    private const string TestText = "Ваш текст для набора";
    private int currentPosition = 0;

    public void Start()
    {
        Console.WriteLine("Начнем тест. Ваша задача - печатать следующий текст:\n");
        Console.WriteLine(TestText);
        Console.WriteLine("\nКогда будите готовы, нажмите Enter.");

        Console.ReadLine();
    }

    public string GetUserInput()
    {
        Console.Clear();
        Console.WriteLine("Текст для набора:\n");

        for (int i = 0; i < currentPosition; i++)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(TestText[i]);
            Console.ResetColor();
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(TestText[currentPosition]);
        Console.ResetColor();

        for (int i = currentPosition + 1; i < TestText.Length; i++)
        {
            Console.Write(TestText[i]);
        }

        Console.WriteLine("\nВводите текст!");
        ConsoleKeyInfo keyInfo = Console.ReadKey();

        if (keyInfo.KeyChar == TestText[currentPosition])
        {
            Console.Clear();
            currentPosition++;
        }
        else
        {
            Console.Clear();

        }

        return currentPosition < TestText.Length ? null : TestText;
    }
}