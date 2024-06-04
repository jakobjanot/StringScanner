using System.Text.RegularExpressions;

namespace StringScanner;

public class Scanner
{
    public int Pos { get; set; }
    public string Str { get; set; }

    private Match? _lastMatch;
    private Match? LastMatch { get => _lastMatch; set { LastMatchLength = value?.Length ?? null; _lastMatch = value; } }

    public int? LastMatchLength { get; set; }

    /// <summary>
    /// Creates a new Scanner object. The scan pointer is set to the beginning of the string, and the match register is cleared.
    /// </summary>
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
    /// Scans one character and returns it.
    /// </summary>
    public string? Read(int len = 1)
    {
        LastMatch = null;
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

    private Match? Match(Regex pattern)
    {
        if (IsEndOfString()) return LastMatch = null;
        return LastMatch = pattern.Match(this.Str, this.Pos);
    }

    /// <summary>
    /// Tests whether the given pattern is matched from the current scan pointer. Returns the length of the match, or null. The scan pointer is not advanced.
    /// </summary>
    public bool IsMatch(Regex pattern)
    {
        Match(pattern);
        return LastMatch != null && LastMatch.Success;
    }

    /// <summary>
    /// Extracts a string corresponding to string[pos,len], without advancing the scan pointer.
    /// </summary>
    public string? Peek(int len) => this.Str.Substring(this.Pos, len);

    /// <summary>
    /// Resets the scanner. The scan pointer is set to the beginning of the string, and the match register is cleared.
    /// </summary>
    public void Reset()
    {
        this.Pos = 0;
        this.LastMatch = null;
    }

    /// <summary>
    /// Terminates the scanner. The scan pointer is set to the end of the string, and the match register is cleared.
    /// </summary>
    public void Terminate()
    {
        this.Pos = this.Str.Length;
        this.LastMatch = null;
    }

    /// <summary>
    /// Returns the “rest” of the string (i.e. everything after the scan pointer). If there is no more data (eos? = true), it returns "".
    /// </summary>
    public string Rest() => this.Str.Substring(this.Pos);

    /// <summary>
    /// This returns the value that Scan would return, without advancing the scan pointer. The match register is affected, though.
    /// </summary>
    public bool Check(Regex pattern)
    {
        Match(pattern);
        return LastMatch != null && LastMatch.Success && LastMatch.Index == this.Pos;
    }

    /// <summary>
    /// Like Check, but a function is used to transform the named captures in the match before it is returned.
    /// </summary>
    public string? Check(Regex pattern, Func<Func<string, string?>, string> map) => Check(pattern) ? map(key => LastMatch!.Groups[key].Value) : null;

    /// <summary>
    /// This returns the value that ScanUntil would return, without advancing the scan pointer. The match register is affected, though.
    /// </summary>
    public bool CheckUntil(Regex pattern)
    {
        Match(pattern);
        return LastMatch != null && LastMatch.Success;
    }

    /// <summary>
    /// Like CheckUntil, but a function is used to transform the named captures in the match before it is returned.
    /// </summary>
    public string? CheckUntil(Regex pattern, Func<Func<string, string?>, string> map) => CheckUntil(pattern) ? map(key => LastMatch!.Groups[key].Value) : null;

    /// <summary>
    /// Tries to match with pattern at the current position. If there’s a match, the scanner advances the “scan pointer” and returns true. Otherwise, the scanner returns false.
    /// </summary>
    public bool Scan(Regex pattern)
    {
        Match(pattern);

        if (LastMatch != null && LastMatch.Success && LastMatch.Index == this.Pos)
        {
            this.Pos += LastMatch.Length;
            return true;
        }
        else
        {
            LastMatch = null;
            return false;
        }
    }

    /// <summary>
    /// Like scan, but a function is used to transform the named captures in the match before it is returned.
    /// </summary>
    public string? Scan(Regex pattern, Func<Func<string, string?>, string> map) => Scan(pattern) ? map(key => LastMatch!.Groups[key].Value) : null;

    /// <summary>
    /// Scans the string until the pattern is matched. Returns the substring up to and including the end of the match, advancing the scan pointer to that location. If there is no match, null is returned.
    /// </summary>
    public bool ScanUntil(Regex pattern)
    {
        Match(pattern);
        
        if (LastMatch != null && LastMatch.Success)
        {
            this.Pos = LastMatch.Index + LastMatch.Length;
            return true;
        }
        else
        {
            LastMatch = null;
            return false;
        }
    }

    /// <summary>
    /// Like scan, but a function is used to transform the named captures in the match before it is returned.
    /// </summary>
    public string? ScanUntil(Regex pattern, Func<Func<string, string?>, string> map) => ScanUntil(pattern) ? map(key => LastMatch!.Groups[key].Value) : null;

    /// <summary>
    /// Attempts to skip over the given pattern beginning with the scan pointer. If it matches, the scan pointer is advanced to the end of the match.
    /// 
    /// It’s similar to scan, but without returning the matched string.
    /// </summary>
    public bool Skip(Regex pattern) => Scan(pattern);

    /// <summary>
    /// Advances the scan pointer until pattern is matched and consumed. Returns the number of bytes advanced, or null if no match was found.
    /// 
    /// Look ahead to match pattern, and advance the scan pointer to the end of the match. Return the number of characters advanced, or nil if the match was unsuccessful.
    /// 
    /// It’s similar to scan_until, but without returning the intervening string.
    /// </summary>
    public bool SkipUntil(Regex pattern) => ScanUntil(pattern);

    /// <summary>
    /// Returns the subgroups in the most recent match at the given indices. If nothing was priorly matched, it returns null.
    /// </summary>
    public string[]? ValuesAt(params int[] indices)
    {
        if (LastMatch == null) return null;
        var result = new string[indices.Length];
        for (int i = 0; i < indices.Length; i++)
        {
            if (indices[i] < 0)
            {
                result[i] = LastMatch.Groups[LastMatch.Groups.Count + indices[i]].Value;
            }
            else
            {
                result[i] = LastMatch.Groups[indices[i]].Value;
            }
        }
        return result;
    }

    /// <summary>
    /// Returns the first subgroup in the most recent match. If nothing was priorly matched, it returns null.
    /// </summary>
    public string? ValueAt(int index) => ValuesAt(index)?.FirstOrDefault();

    /// <summary>
    /// Returns the subgroups in the most recent match at the given names. If nothing was priorly matched, it returns null.
    /// </summary>
    public string[] ValuesAt(params string[] names)
    {
        if (LastMatch == null) return new string[0];
        var result = new string[names.Length];
        for (int i = 0; i < names.Length; i++)
        {
            result[i] = LastMatch.Groups[names[i]].Value;
        }
        return result;
    }

    /// <summary>
    /// Returns the first subgroup in the most recent match at the given name. If nothing was priorly matched, it returns null.
    /// </summary>
    public string? ValueAt(string name) => ValuesAt(name)[0];
    
    /// <summary>
    /// Returns the subgroups in the most recent match. If nothing was priorly matched, it returns null.
    /// </summary>
    public string[]? Captures()
    {
        if (LastMatch == null) return null;
        var result = new string[LastMatch.Groups.Count - 1];
        for (int i = 1; i < LastMatch.Groups.Count; i++)
        {
            result[i - 1] = LastMatch.Groups[i].Value;
        }
        return result;
    }

    /// <summary>
    /// Returns a dictionary of named captures in the most recent match. If nothing was priorly matched, it returns null.
    /// </summary>
    public Dictionary<string, string>? NamedCaptures()
    {
        if (LastMatch == null) return null;
        var result = new Dictionary<string, string>();
        foreach (var groupName in LastMatch.Groups.Keys.Cast<string>().Where(x => x != "0"))
        {
            result[groupName] = LastMatch.Groups[groupName].Value.ToString();
        }
        return result;
    }

    /// <summary>
    /// Returns the part of the string before the most recent match. If nothing was priorly matched, it returns the entire string.
    /// </summary>
    public string? PreMatch() => LastMatch == null ? null : this.Str.Substring(0, LastMatch.Index);

    /// <summary>
    /// Returns the part of the string after the most recent match. If nothing was priorly matched, it returns the entire string.
    /// </summary>
    public string? PostMatch() => LastMatch == null ? null : this.Str.Substring(LastMatch.Index + LastMatch.Length);
}