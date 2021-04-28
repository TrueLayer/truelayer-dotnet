using System.Text;

namespace TrueLayerSdk.Common
{
    public static class StringExtensions
    {
        private enum SnakeCaseState
        {
            Start,
            Lower,
            Upper,
            NewWord
        }
        
        public static string ToSnakeCase(this string s)
        {
            const char separator = '_';
            if (string.IsNullOrWhiteSpace(s))
            {
                return s;
            }

            var sb = new StringBuilder();
            var state = SnakeCaseState.Start;

            for (var i = 0; i < s.Length; i++)
            {
                if (s[i] == ' ')
                {
                    if (state != SnakeCaseState.Start)
                    {
                        state = SnakeCaseState.NewWord;
                    }
                }
                else if (char.IsUpper(s[i]))
                {
                    switch (state)
                    {
                        case SnakeCaseState.Upper:
                            var hasNext = (i + 1 < s.Length);
                            if (i > 0 && hasNext)
                            {
                                var nextChar = s[i + 1];
                                if (!char.IsUpper(nextChar) && nextChar != separator)
                                {
                                    sb.Append(separator);
                                }
                            }
                            break;
                        case SnakeCaseState.Lower:
                        case SnakeCaseState.NewWord:
                            sb.Append(separator);
                            break;
                    }

                    sb.Append(char.ToLowerInvariant(s[i]));
                    state = SnakeCaseState.Upper;
                }
                else if (s[i] == separator)
                {
                    sb.Append(separator);
                    state = SnakeCaseState.Start;
                }
                else
                {
                    if (state == SnakeCaseState.NewWord)
                    {
                        sb.Append(separator);
                    }

                    sb.Append(s[i]);
                    state = SnakeCaseState.Lower;
                }
            }

            return sb.ToString();
        }
    }
}
