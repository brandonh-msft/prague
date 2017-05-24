using System;
using System.Text.RegularExpressions;

namespace Prague
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(@"Type 'I am <your name>' and hit <Enter>:");
            var name = Console.ReadLine();

            var winningRule = PragueClient.First(name,
                new PragueRule<string>(
                    s =>
                    {
                        var r = new Regex(@"I am (.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        Console.WriteLine($@"Hi {r.Match(s).Groups[1].Value}!");
                    },
                    s =>
                    {
                        var r = new Regex(@"I am (.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        return r.IsMatch(s);
                    }
            ));

            winningRule
        }
    }
}