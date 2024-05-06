using System;
using System.Text.RegularExpressions;
namespace StringScanner
{
    public class RubyStringScanner
    {
        private string str;
        private int curr = 0;
        private int prev = -1;
        private bool matched;
        private bool fixedAnchor;

        public RubyStringScanner(string str)
        {
            this.str = str;
            this.fixedAnchor = false;
        }

        private void ClearMatched()
        {
            matched = false;
        }

        private void SetMatched()
        {
            matched = true;
        }

        private bool IsMatched()
        {
            return matched;
        }

        private void Check()
        {
            if (str == null) throw new ArgumentException("uninitialized StringScanner object");
        }

        public void Reset()
        {
            Check();
            curr = 0;
            ClearMatched();
        }

        public void Terminate()
        {
            Check();
            curr = str.Length;
            ClearMatched();
        }

        public string GetString()
        {
            return str;
        }

        public void SetString(string newStr)
        {
            str = newStr;
            curr = 0;
            ClearMatched();
        }

        public void Concat(string obj)
        {
            Check();
            str += obj;
        }

        public int GetPos()
        {
            Check();
            return curr;
        }

        public void SetPos(int pos)
        {
            Check();

            if (pos < 0) pos += str.Length;
            if (pos < 0 || pos > str.Length) throw new ArgumentOutOfRangeException("index out of range.");
            curr = pos;
        }

        public int GetCharPos()
        {
            return curr;
        }

        private string ExtractSubstring(int start, int length)
        {
            if (start > str.Length) return null;
            if (start + length > str.Length) length = str.Length - start;
            return str.Substring(start, length);
        }

        public string Scan(string regex, bool advancePosition, bool returnMatched, bool headOnly)
        {
            Check();

            if (fixedAnchor)
            {
                regex = @"\G" + regex;
            }

            var match = Match(str, regex, RegexOptions.None, new TimeSpan(0, 0, 0, 0, 1000));

            if (match.Success)
            {
                if (advancePosition)
                {
                    curr = match.Index + match.Length;
                }

                if (returnMatched)
                {
                    SetMatched();
                    return match.Value;
                }
                else
                {
                    return match.Value;
                }
            }
            else
            {
                return null;
            }
        }

        public bool Match(string regex)
        {
            return Scan(regex) != null;
        }

        public void Skip(string regex)
        {
            Scan(regex);
        }
    }
}