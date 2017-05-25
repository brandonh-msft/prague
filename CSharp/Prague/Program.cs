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

            PragueClient.First(name,
                PragueRule<string, RegexMatch>.Create(
                    s =>
                    {
                        Console.WriteLine($@"Hi {s.Match.Groups[1].Value}! Welcome!");
                        return s;
                    },
                    PragueRule<string, RegexMatch>.Create()

        }

        class RegexMatch
        {
            public RegexMatch(Regex regex, string text)
            {
                this.Text = text;
                this.Regex = regex;
                this.Match = regex.Match(text);
            }
            public string Text { get; private set; }
            public Match Match { get; private set; }
            public Regex Regex { get; private set; }
        }
    }
}