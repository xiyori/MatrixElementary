using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_Elementary.Scripts
{
    public static class LaTeXTools
    {
        public static MyMatrix FromWolfram(string input)
        {
            string[] lines = input.Replace("\n", "").Replace("\r", "").Replace(" ", "").Split(new string[] { "},{" }, StringSplitOptions.None);
            lines[0] = lines[0].Replace("{", "");
            lines[lines.Length - 1] = lines.Last().Replace("}", "");
            MyMatrix tmp = new MyMatrix(lines.Length, lines[0].Split(',').Length);
            for (int i = 0; i < lines.Length; i++)
            {
                string[] args = lines[i].Split(',');
                for (int j = 0; j < args.Length; j++)
                    tmp[i, j] = new Rational(args[j]);
            }
            return tmp;
        }

        public static MyMatrix FromLaTeX(string input)
        {
            string[] lines = input.Replace("\n", "").Replace("\r", "").Replace(" ", "").Split(new string[] { "\\\\" }, StringSplitOptions.None);
            if (lines[0].Contains("begin"))
            {
                lines[0] = lines[0].Substring(lines[0].IndexOf('}') + 1);
                if (lines[0][0] == '{')
                    lines[0] = lines[0].Substring(lines[0].IndexOf('}') + 1);
            }
            if (lines.Last().Contains("end"))
            {
                lines[lines.Length - 1] = lines.Last().Substring(0, lines.Last().IndexOf("\\end"));
            }
            int end_empty = 0;
            if (lines.Last().Length == 0)
                end_empty = 1;
            MyMatrix tmp = new MyMatrix(lines.Length - end_empty, lines[0].Split('&').Length);
            for (int i = 0; i < tmp.N; i++)
            {
                string[] args = lines[i].Split('&');
                for (int j = 0; j < args.Length; j++)
                {
                    if (args[j].Contains("frac"))
                        tmp[i, j] = ParseFrac(args[j]);
                    else
                        tmp[i, j] = new Rational(args[j]);
                }
            }
            return tmp;
        }

        public static Rational ParseFrac(string s)
        {
            Rational sign = new Rational(s[0] == '-' ? -1 : 1);
            s = s.Remove(0, s.IndexOf("frac") + 4);
            if (s.Last() == '}') s = s.Remove(s.Length - 1);
            int ind = s.IndexOf('}');
            if (s.Contains("}{"))
            {
                return sign * new Rational(Convert.ToInt64(s.Substring(1, ind - 1)), Convert.ToInt64(s.Substring(ind + 2)));
            }
            else if (s.Contains('{'))
            {
                if (s[0] == '{')
                    return sign * new Rational(Convert.ToInt64(s.Substring(1, ind - 1)),
                        Convert.ToInt64(s[ind + 1].ToString()));
                else
                    return sign * new Rational(Convert.ToInt64(s[0].ToString()), Convert.ToInt64(s.Substring(2)));
            }
            return sign * new Rational(Convert.ToInt64(s[0].ToString()),
                Convert.ToInt64(s[1].ToString()));
        }
    }
}
