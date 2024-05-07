using System.Text.RegularExpressions;

namespace StringScanner;
public class Scanner
{
    RegexOptions _regexOptions = RegexOptions.Multiline;
    
    public int Pos { get; set; }
    public string Str { get; set; }

    public Match? Matched { get; set; }

    /// <summary>
    /// Creates a new Scanner object. The scan pointer is set to the beginning of the string, and the match register is cleared.
    /// </summary>
    /// <param name="str"></param>
    public Scanner(string str)
    {
        this.Str = str;
        this.Pos = 0;
    }
    /// <summary>
    /// Returns true if the scan pointer is at the beginning of the line.
    /// </summary>
    public bool IsBeginningOfLine()
    {
        return this.Pos == 0 || this.Str[this.Pos - 1] == '\n';
    }

    /// <summary>
    /// Returns true if the scan pointer is at the end of the line.
    /// </summary>
    public bool IsEndOfLine()
    {
        return this.Pos == this.Str.Length || this.Str[this.Pos] == '\n';
    }

    /// <summary>
    /// Returns true if the scan pointer is at the beginning of the string.
    /// </summary>
    public bool IsBeginningOfString()
    {
        return this.Pos == 0;
    }

    /// <summary>
    /// Returns true if the scan pointer is at the end of the string.
    /// </summary>
    public bool IsEndOfString()
    {
        return this.Pos == this.Str.Length;
    }

    /// <summary>
    /// This returns the value that Scan would return, without advancing the scan pointer. The match register is affected, though.
    /// </summary>
    /// <param name="regex"></param>
    public string? Check(Regex regex)
    {
        Match(regex);
        if (Matched.Success && Matched.Index == this.Pos)
        {
            return Matched.Value;
        }
        return null;
    }

    /// <summary>
    /// This returns the value that ScanUntil would return, without advancing the scan pointer. The match register is affected, though.
    /// </summary>
    /// <param name="regex"></param>
    public string? CheckUntil(Regex regex)
    {
        Match(regex);
        if (Matched.Success)
        {
            return this.Str.Substring(this.Pos, Matched.Index - this.Pos + Matched.Length);
        }
        return null;
    }

    /// <summary>
    /// Looks ahead to see if the regex exists anywhere in the string, without advancing the scan pointer. This predicates whether a ScanUntil will return a value.
    /// </summary>
    /// <param name="regex"></param>
    public bool Exist(Regex regex)
    {
        return Match(regex).Success;
    }

    /// <summary>
    /// Scans one character and returns it.
    /// </summary>
    /// <param name="len"></param>
    public string? Read(int len = 1)
    {
        Matched = null;
        if (this.Pos + len > this.Str.Length)
        {
            return null;
        }
        var result = this.Str.Substring(this.Pos, len);
        this.Pos += len;
        return result;
    }

    /// <summary>
    /// Returns a string that represents the Scanner object, showing:
    ///     the current position
    ///     the size of the string
    ///     the characters surrounding the scan pointer
    /// </summary>
    public override string ToString()
    {
        return $"#<Scanner {this.Pos}/{this.Str.Length} @ \"{this.Str}\">";
    }

    private Match? Match(Regex regex)
    {
        if (IsEndOfString())
        {
            return Matched = null;
        }
        return Matched = regex.Match(this.Str, this.Pos);
    }

    /// <summary>
    /// Tests whether the given regex is matched from the current scan pointer. Returns the length of the match, or null. The scan pointer is not advanced.
    /// </summary>
    /// <param name="regex"></param>
    public bool IsMatch(Regex regex)
    {
        Match(regex);
        return Matched.Success;
    }

    /// <summary>
    /// Extracts a string corresponding to string[pos,len], without advancing the scan pointer.
    /// </summary>
    /// <param name="len"></param>
    public string? Peek(int len)
    {
        return this.Str.Substring(this.Pos, len);
    }
    
    /// <summary>
    /// Resets the scanner. The scan pointer is set to the beginning of the string, and the match register is cleared.
    /// </summary>
    public void Reset()
    {
        this.Pos = 0;
        this.Matched = null;
    }

    /// <summary>
    /// Terminates the scanner. The scan pointer is set to the end of the string, and the match register is cleared.
    /// </summary>
    public void Terminate()
    {
        this.Pos = this.Str.Length;
        this.Matched = null;
    }

    /// <summary>
    /// Returns the “rest” of the string (i.e. everything after the scan pointer). If there is no more data (eos? = true), it returns "".
    /// </summary>
    public string Rest()
    {
        return this.Str.Substring(this.Pos);
    }

    /// <summary>
    /// Tries to match with regex at the current position. If there’s a match, the scanner advances the “scan pointer” and returns the matched string. Otherwise, the scanner returns null.
    /// </summary>
    /// <param name="regex"></param>
    public string? Scan(Regex regex)
    {
        Match(regex);
        
        if (Matched != null && Matched.Success && Matched.Index == this.Pos)
        {
            this.Pos += Matched.Length;
            return Matched.Value;
        }
        else
        {
            Matched = null;
            return null;
        }
    }

    /// <summary>
    /// Scans the string until the regex is matched. Returns the substring up to and including the end of the match, advancing the scan pointer to that location. If there is no match, null is returned.
    /// </summary>
    /// <param name="regex"></param>
    public string? ScanUntil(Regex regex)
    {
        Match(regex);
        if (Matched != null && Matched.Success)
        {
            var result = this.Str.Substring(this.Pos, Matched.Index - this.Pos + Matched.Length);
            this.Pos = Matched.Index + Matched.Length;
            return result;
        }
        else
        {
            Matched = null;
            return null;
        }
    }

    /// <summary>
    /// Attempts to skip over the given regex beginning with the scan pointer. If it matches, the scan pointer is advanced to the end of the match, and the length of the match is returned. Otherwise, null is returned.
    /// 
    /// It’s similar to scan, but without returning the matched string.
    /// </summary>
    /// <param name="regex"></param>
    public int? Skip(Regex regex)
    {
        return Scan(regex) == null ? null : Matched?.Length;
    }

    /// <summary>
    /// Advances the scan pointer until regex is matched and consumed. Returns the number of bytes advanced, or null if no match was found.
    /// 
    /// Look ahead to match regex, and advance the scan pointer to the end of the match. Return the number of characters advanced, or nil if the match was unsuccessful.
    /// 
    /// It’s similar to scan_until, but without returning the intervening string.
    /// </summary>
    /// <param name="regex"></param>
    public int? SkipUntil(Regex regex)
    {
        return ScanUntil(regex) == null ? null : Matched?.Index + Matched?.Length - this.Pos;
    }
    
    /// <summary>
    /// Returns the subgroups in the most recent match at the given indices. If nothing was priorly matched, it returns null.
    /// </summary>
    /// <param name="indices"></param>
    public string[]? ValuesAt(params int[] indices)
    {
        if (Matched == null)
        {
            return null;
        }
        var result = new string[indices.Length];
        for (int i = 0; i < indices.Length; i++)
        {
            if (indices[i] < 0)
            {
                result[i] = Matched.Groups[Matched.Groups.Count + indices[i]].Value;
            }
            else
            {
                result[i] = Matched.Groups[indices[i]].Value;
            }
        }
        return result;
    }

    /// <summary>
    /// Returns the subgroups in the most recent match at the given names. If nothing was priorly matched, it returns null.
    /// </summary>
    /// <param name="names"></param>
    public string[] ValuesAt(params string[] names)
    {
        if (Matched == null)
        {
            return null;
        }
        var result = new string[names.Length];
        for (int i = 0; i < names.Length; i++)
        {
            result[i] = Matched.Groups[names[i]].Value;
        }
        return result;
    }
    
    /// <summary>
    /// Returns the subgroups in the most recent match. If nothing was priorly matched, it returns null.
    /// </summary>
    public string[]? Captures()
    {
        if (Matched == null)
        {
            return null;
        }
        var result = new string[Matched.Groups.Count - 1];
        for (int i = 1; i < Matched.Groups.Count; i++)
        {
            result[i - 1] = Matched.Groups[i].Value;
        }
        return result;
    }

    /// <summary>
    /// Returns a dictionary of named captures in the most recent match. If nothing was priorly matched, it returns null.
    /// </summary>
    public Dictionary<string, string>? NamedCaptures()
    {
        if (Matched == null)
        {
            return null;
        }
        var result = new Dictionary<string, string>();
        foreach (var groupName in Matched.Groups.Keys.Cast<string>().Where(x => x != "0"))
        {
            result[groupName] = Matched.Groups[groupName].Value.ToString();
        }
        return result;
    }

    /// <summary>
    /// Returns the part of the string before the most recent match. If nothing was priorly matched, it returns the entire string.
    /// </summary>
    public string? PreMatch()
    {
        return Matched == null ? null : this.Str.Substring(0, Matched.Index);
    }

    /// <summary>
    /// Returns the part of the string after the most recent match. If nothing was priorly matched, it returns the entire string.
    /// </summary>
    public string? PostMatch()
    {
        return Matched == null ? null : this.Str.Substring(Matched.Index + Matched.Length);
    }
}