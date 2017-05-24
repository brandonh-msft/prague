using System;
using System.Text.RegularExpressions;
using Prague.Interfaces;
using Prague.Matchers;

namespace Prague
{
    class Program
    {
        class ConsoleMatch : IPragueMatch
        {
            public ConsoleMatch(string text, Action<string> reply)
            {
                this.Text = text;
                this.Reply = reply;
            }

            public Action<string> Reply { get; private set; } = null;

            public string Text { get; private set; }
        }

        class MatchResult<TMatch> : IPragueMatchResult<TMatch>, IPragueMatchWithName, IPragueMatch where TMatch : IPragueMatch
        {
            public MatchResult(TMatch match)
            {
                this.Match = match;
            }

            public TMatch Match { get; private set; }

            public string Name { get; set; }

            public Action<string> Reply => this.Match?.Reply;
        }

        class ConsoleNameRule : IPragueRule<IPragueMatchResult<ConsoleMatch>, ConsoleMatch>
        {
            private Func<ConsoleMatch, MatchResult<ConsoleMatch>> matchName;
            private Action<IPragueMatchWithName> _handler;

            public ConsoleNameRule(Action<IPragueMatchWithName> handler, Func<ConsoleMatch, MatchResult<ConsoleMatch>> matchName = null)
            {
                this.matchName = matchName;
                this._handler = handler;
            }

            public void CallHandlerIfMatch(PragueMatcher<IPragueMatchResult<ConsoleMatch>, ConsoleMatch> matcher, ConsoleMatch match)
            {
                return CallHandlerIfMatch(match, matcher.Evaluate);
            }

            public void CallHandlerIfMatch(Func<ConsoleMatch, IPragueMatchResult<ConsoleMatch>> matcher, ConsoleMatch match)
            {
                return CallHandlerIfMatch(match, matcher);
            }

            private bool CallHandlerIfMatch(ConsoleMatch match, Func<ConsoleMatch, IPragueMatchResult<ConsoleMatch>> matcher = null)
            {
                var r = matcher?.Invoke(match) ?? matchName?.Invoke(match);
                var m = r?.Match;
                if (m != null)
                {
                    _handler(r);
                    return true;
                }

                return false;
            }
        }

        static void Main(string[] args)
        {
            Action<IPragueMatchWithName> greetGuest = match =>
            {
                match.Reply($@"Hi there, {match.Name}! Welcome to CafeBot.");
            };

            Func<ConsoleMatch, IPragueMatchResult<ConsoleMatch>> matchName = match =>
            {
                var re = new Regex(@"I am (.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase).Match(match.Text);
                return !re.Success ? null : new MatchResult<ConsoleMatch>(match)
                {
                    Name = re.Groups[1].Value,
                };
            };

            var nameRule = new ConsoleNameRule(greetGuest, matchName);

            var text = Console.ReadLine();
            if (nameRule.CallHandlerIfMatch(matchName, new ConsoleMatch(text, Console.WriteLine)))
            {
                // Stop processing rules if we're in a 'Best' scenario
            }

            Console.WriteLine(@"all done. press enter");
            Console.ReadLine();
        }
    }
}