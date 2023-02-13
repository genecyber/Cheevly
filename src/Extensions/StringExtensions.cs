using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Cheevly.Extensions {
    public static class StringExtensions {

        public static bool HasConsonant(this string input) {
            return !input.All(each => each.IsVowel());
        }

        public static bool IsAlphabetical(this char text) {
            return text.ToString().IsAlphabetical();
        }

        public static bool HasNumbers(this string text) {
            return text.Any(char.IsDigit);
        }

        public static bool IsAlphabetical(this string text) {
            return Regex.IsMatch(text, @"^[a-zA-Z]+$");
        }

        public static bool HasVowel(this string input) {
            return input.Any(each => each.IsVowel());
        }

        public static bool IsVowel(this char input) {
            const string vowels = "aeiou";
            return vowels.IndexOf(char.ToLower(input)) >= 0;
        }

        public static bool IsNumeric(this char input) {
            return input.ToString().IsNumeric();
        }

        public static bool IsNumeric(this string input) {
            double retNum;

            return double.TryParse(input, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out retNum);
        }

        public static IEnumerable<string> ReadLines(this string input) {
            using (var reader = new StringReader(input)) {
                var line = reader.ReadLine();

                while (!line.IsEmpty()) {
                    yield return line;
                    line = reader.ReadLine();
                }
            }
        }

        public static string Parse(this string input, string startPattern, string endPattern) {
            var start = input.IndexOf(startPattern);
            var offset = startPattern.Length + start;
            var end = input.IndexOf(endPattern, offset);
            return input.Substring(offset, end - offset);
        }

        public static string Tokenize(this string input, string leftPattern, string rightPattern, Func<string, string> tokenFunction) {
            leftPattern = leftPattern.Replace("$", @"\$");
            rightPattern = rightPattern.Replace("$", @"\$");

            var pattern = leftPattern + @"(.*?)" + rightPattern;
            var matches = Regex.Matches(input, pattern);

            foreach (Match match in matches) {
                var matchValue = match.Value;
                var group = match.Groups[1].Value;

                input = input.Replace(matchValue, tokenFunction(group));
            }

            return input;
        }

        public static string FormatWithObject(this string input, object instance) {
            return input.Tokenize("{", "}", token => {
                var context = instance;

                // Todo:  Reflection cache
                foreach (var member in token.Split('.')) {
                    var type = context.GetType();

                    if (member.EndsWith("()")) {
                        context = type.GetMethod(member.Replace("()", "")).Invoke(context, null);
                    }
                    else if (member.EndsWith("]")) {
                        var split = member.Split('[');
                        var name = split[0];
                        var index = int.Parse(split[1].Split(']')[0]);
                        var property = type.GetProperty(name);

                        context = property.GetValue(context, new object[] { index });
                    }
                    else {
                        context = type.GetProperty(member).GetValue(context);
                    }
                }

                return context.ToString();
            });
        }

        public static string ReplaceNonNumbers(this string value, string replaceWith = "") {
            var digitsOnly = new Regex(@"[^\d]");
            return digitsOnly.Replace(value, replaceWith);
        }

        public static T ToEnum<T>(this string value) {
            return (T)Enum.Parse(typeof(T), value);
        }

        public static byte[] ToBytes(this string input) {
            var encoding = new UTF8Encoding(false);
            return encoding.GetBytes(input);
        }

        public static string Right(this string input, int characters) {
            return input.Substring(input.Length - characters);
        }

        public static string Left(this string input, int characters) {
            return input.Substring(0, characters);
        }

        public static bool IsEmpty(this string input) {
            return string.IsNullOrEmpty(input);
        }

        public static string Capitalize(this string input) {
            var temp = input.Substring(0, 1);
            return temp.ToUpper() + input.Remove(0, 1);
        }

        public static IEnumerable<int> GetOccurancesOf(this string input, params string[] patterns) {

            foreach (var pattern in patterns) {
                var i = 0;

                while ((i = input.IndexOf(pattern, i)) != -1) {
                    yield return i;
                    i += pattern.Length;
                }
            }
        }

        public static int CountOccurancesOf(this string input, string match) {
            return input.GetOccurancesOf(match).Count();
        }

        public static string CamelCase(this string input) {
            if (string.IsNullOrEmpty(input)) {
                return string.Empty;
            }

            if (input.ToUpper() == input) {
                return input.ToLower();
            }

            return char.ToLowerInvariant(input[0]) + input.Substring(1);
        }

        public static string SplitCamelCase(this string input, char splitter = ' ') {
            return Regex.Replace(Regex.Replace(input, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1" + splitter + "$2"), @"(\p{Ll})(\P{Ll})", "$1" + splitter + "$2");
        }
    }
}