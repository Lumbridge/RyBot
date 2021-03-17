using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RyBot.Helpers
{
    public static class StringHelper
    {
        public static List<Tuple<string, string>> ParseEmotesFromMessage(string input)
        {
            var results = new List<Tuple<string, string>>();

            var animatedEmoteNames = GetSubStrings(input, "<a:", ">").ToList();
            
            foreach (var emoteTag in animatedEmoteNames) {
                results.Add(new Tuple<string, string>(emoteTag.Split(":")[0], $"https://cdn.discordapp.com/emojis/{emoteTag.Split(":")[1]}.gif"));
                input = input.Replace($"<a:{emoteTag}>", string.Empty);
            }

            var staticEmoteNames = GetSubStrings(input, "<:", ">").ToList();

            foreach (var emoteTag in staticEmoteNames) {
                results.Add(new Tuple<string, string>(emoteTag.Split(":")[0], $"https://cdn.discordapp.com/emojis/{emoteTag.Split(":")[1]}.png"));
                input = input.Replace(emoteTag, string.Empty);
            }
            
            return results;
        }

        public static IEnumerable<string> GetSubStrings(string input, string start, string end)
        {
            var r = new Regex(Regex.Escape(start) + "(.*?)" + Regex.Escape(end));
            var matches = r.Matches(input);
            foreach (Match match in matches)
                yield return match.Groups[1].Value;
        }
    }
}
