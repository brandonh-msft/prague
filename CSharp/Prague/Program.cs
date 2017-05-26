using System;
using System.Text.RegularExpressions;

namespace Prague
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine(@"Type 'I am <your name>' and hit <Enter>:");
                var name = Console.ReadLine();

                PragueFirst<string>.Create(name,
                    PragueRule<string, RegexMatch>.Create(
                        s => Console.WriteLine($@"Hi {s.Match.Groups[1].Value}! Welcome!"),
                        s => RegexMatch.Create(@"I am (.*)", s)))



                        .Action?.Invoke();
            }

        }

        class RegexMatch
        {
            public static RegexMatch Create(string regex, string text)
            {
                var tryCreate = new RegexMatch(regex, text);
                return tryCreate.Match.Success ? tryCreate : null;
            }

            private RegexMatch(string regex, string text)
            {
                this.Text = text;
                this.Regex = new Regex(regex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                this.Match = this.Regex.Match(text);
            }
            public string Text { get; private set; }
            public Match Match { get; private set; }
            public Regex Regex { get; private set; }
        }
    }
}