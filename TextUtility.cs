using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OitAntenna
{
    public static class TextUtility
    {
        private const string halfWidthKana = "ｱｧｲｨｳｩｴｪｵｫｶｷｸｹｺｻｼｽｾｿﾀﾁﾂｯﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔｬﾕｭﾖｮﾗﾘﾙﾚﾛﾜｦﾝﾞﾟｰ､｡･｢｣";
        private const string fullWidthKana = "アァイィウゥエェオォカキクケコサシスセソタチツッテトナニヌネノハヒフヘホマミムメモヤャユュヨョラリルレロワヲン゛゜ー、。・「」";

        public static IList<string> ReadLines(string fileName)
        {
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(fileName, Encoding.GetEncoding(Settings.TextEncoding)))
            {
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    lines.Add(line);
                }
            }
            return lines;
        }

        public static string Normalize(string s)
        {
            string t = ToHalfWidthAlphabet(ToFullWidthKana(s)).ToLower();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < t.Length; i++)
            {
                if ('0' <= t[i] && t[i] <= '9' ||
                    'a' <= t[i] && t[i] <= 'z' ||
                    'ぁ' <= t[i] && t[i] <= 'ん' ||
                    'ァ' <= t[i] && t[i] <= 'ヶ' ||
                    t[i] == 'ー' ||
                    '\u4E00' <= t[i] && t[i] <= '\u9FFF')
                {
                    sb.Append(t[i]);
                }
            }
            return sb.ToString();
        }

        public static int GetLevenshteinDistance(string x, string y)
        {
            int m = x.Length;
            int n = y.Length;
            int[,] d = new int[m + 1, n + 1];

            if (m == 0)
            {
                return n;
            }

            if (n == 0)
            {
                return m;
            }

            for (int i = 0; i <= m; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= n; d[0, j] = j++)
            {
            }

            for (int i = 1; i <= m; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    int cost = (y[j - 1] == x[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[m, n];
        }

        public static string ToHalfWidthAlphabet(string s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                if ('！' <= s[i] && s[i] <= '～')
                {
                    sb.Append((char)(s[i] - '！' + '!'));
                }
                else
                {
                    sb.Append(s[i]);
                }
            }
            return sb.ToString();
        }

        public static string ToFullWidthKana(string s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                if ('｡' <= s[i] && s[i] <= 'ﾟ')
                {
                    if (i < s.Length - 1 && (s[i] == 'ｳ' || 'ｶ' <= s[i] && s[i] <= 'ﾄ' || 'ﾊ' <= s[i] && s[i] <= 'ﾎ'))
                    {
                        if (s[i + 1] == 'ﾞ')
                        {
                            if (s[i] != 'ｳ')
                            {
                                sb.Append((char)(ToFullWidthKanaSub(s[i]) + 1));
                            }
                            else
                            {
                                sb.Append('ヴ');
                            }
                            i++;
                        }
                        else if ('ﾊ' <= s[i] && s[i] <= 'ﾎ' && s[i + 1] == 'ﾟ')
                        {
                            sb.Append((char)(ToFullWidthKanaSub(s[i]) + 2));
                            i++;
                        }
                        else
                        {
                            sb.Append(ToFullWidthKanaSub(s[i]));
                        }
                    }
                    else
                    {
                        sb.Append(ToFullWidthKanaSub(s[i]));
                    }
                }
                else
                {
                    sb.Append(s[i]);
                }
            }
            return sb.ToString();
        }

        private static char ToFullWidthKanaSub(char c)
        {
            int i = halfWidthKana.IndexOf(c);
            if (0 <= i && i < fullWidthKana.Length)
            {
                return fullWidthKana[i];
            }
            else
            {
                return '　';
            }
        }
    }
}
