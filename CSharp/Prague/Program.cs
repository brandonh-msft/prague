using System;
using System.Text.RegularExpressions;
using Prague.Interfaces;

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

                PragueFirst.Create(name,
                    s => Console.WriteLine($@"Hi {s.Match.Groups[1].Value}! Welcome!"),
                    s => RegexMatch.Create(@"I am (.*)", s))



                    ?.Action?.Invoke();
            }

        }

        class RegexMatch : IPragueConditionResult<string>
        {
            public static RegexMatch Create(string regex, string text)
            {
                var tryCreate = new RegexMatch(regex, text);
                return tryCreate.Match.Success ? tryCreate : null;
            }

            private RegexMatch(string regex, string text)
            {
                this.Source = text;
                this.Regex = new Regex(regex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                this.Match = this.Regex.Match(text);
            }
            public string Text => this.Source;
            public Match Match { get; private set; }
            public Regex Regex { get; private set; }

            public string Source { get; private set; }
        }
    }
}