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

                //PragueFirst.Create
                //    .WithParam(name)
                //    .WithRule(
                //        a => Console.WriteLine($@"Hi {a.Match.Groups[1].Value}! Welcome!"),
                //        s => RegexMatch.Create(@"I am (.*)", s))
                //    .WithRule(
                //        a => Console.WriteLine($@"Yo {a.Source}! Welcome!"),
                //        s => ConditionResult.FromBoolean(s, s?.StartsWith("b", StringComparison.OrdinalIgnoreCase) == true))
                //?.Action?.Invoke();

                PragueFirst.Create
                    .Easy(name,
                        a => Console.WriteLine($@"Hi {a.Source}! Welcome!"),
                        s =>
                        {
                            var m = RegexMatch.Create(@"I am (.*)", s);
                            return ConditionResult.FromBoolean(m?.Match.Groups[1].Value, m != null);
                        },
                        s => ConditionResult.FromBoolean(s, s?.StartsWith("b", StringComparison.OrdinalIgnoreCase) == true))
                ?.Action?.Invoke();
            }
        }

        class RegexMatch : ConditionResult<string>
        {
            public static RegexMatch Create(string regex, string text)
            {
                try
                {
                    var tryCreate = new RegexMatch(regex, text);
                    return tryCreate.Match.Success ? tryCreate : null;
                }
                catch
                {
                    return null;
                }
            }

            private RegexMatch(string regex, string text) : base(text)
            {
                this.Regex = new Regex(regex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                this.Match = this.Regex.Match(text);
            }

            public string Text => this.Source;
            public Match Match { get; private set; }
            public Regex Regex { get; private set; }
        }
    }
}