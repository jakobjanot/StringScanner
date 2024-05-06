using System.Text.RegularExpressions;
using static System.String;

namespace StringScanner;

public class Scanner
{
    private const RegexOptions _regexOptions = RegexOptions.Multiline;
    public Scanner(string text)
    {
        Text = text;
        Reset();
    }

    public string Text { get; private set; }

    public int Position { get; private set; }

    //= Advancing the Scan Pointer
    /// <summary>
    /// Tries to match with +pattern+ at the current position. If there's a match,
    /// the scanner advances the "scan pointer" and returns the matched string.
    /// Otherwise, the scanner returns +null+.
    /// 
    ///  var s = new Scanner("test string");
    ///  s.Scan("\w+"); // -> "test"
    ///  s.Scan("\w+"); // -> null
    ///  s.Scan("\s+"); // -> " "
    ///  s.Scan("str"); // -> "str"
    ///  s.Scan("\w+"); // -> "ing"
    ///  s.Scan(".");   // -> null 
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="options"></param>
    public string? Scan(string pattern, RegexOptions options = _regexOptions)
    {
        var match = Match(Text.Substring(Position), @"^{pattern}", options);
        if (match.Success)
        {
            Position += match.Length;
            return match.Value;
        }
        return null;
    }
        
    // scan_until
    /// <summary>
    /// Scans the string _until_ the +pattern+ is matched.  Returns the substring up
    /// to and including the end of the match, advancing the scan pointer to that
    /// location. If there is no match, +null+ is returned.
    /// 
    /// var s = new Scanner("Fri Dec 12 1975 14:39");
    /// s.ScanUntil("1"); // -> "Fri Dec 1"
    /// s.PreMatch;       // -> "Fri Dec "
    /// s.ScanUntil("XYZ"); // -> null
    /// </summary>    
    /// <param name="pattern"></param>
    /// <param name="options"></param>
    public string? ScanUntil(string pattern, RegexOptions options = _regexOptions)
    {
        var match = Match(Text, pattern, options);
        if (match.Success)
        {
            Position += (match.Index + match.Length);
            return Text.Substring(0, match.Length);
        }
        return null;
    }
    // skip
    /// <summary>
    /// Attempts to skip over the given +pattern+ beginning with the scan pointer.
    /// If it matches, the scan pointer is advanced to the end of the match, and the
    /// length of the match is returned. Otherwise, +null+ is returned.
    /// 
    /// It's similar to #scan, but without returning the matched string.
    /// Example:
    /// var s = new Scanner("test string");
    /// s.Skip("\w+"); // -> true
    /// s.Skip("\w+"); // -> false
    /// s.Skip("\s+"); // -> true
    /// s.Skip("st");  // -> true
    /// s.Skip("\w+"); // -> true
    /// s.Skip(".");   // -> false
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="options"></param>
    public int? Skip(string pattern, RegexOptions options = _regexOptions)
    {
        var match = Match(Text, pattern, options);
        if (match.Success)
        {
            Position += match.Length;
            return match.Length;
        }
        return null;
    }
    // skip_until
    /// <summary>
    /// Advances the scan pointer until +pattern+ is matched and consumed.  Returns
    /// the number of bytes advanced, or +null+ if no match was found.
    /// 
    /// Example:
    /// var s = new Scanner("Fri Dec 12 1975 14:39");
    /// s.SkipUntil("12"); // -> 10
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public int? SkipUntil(string pattern, RegexOptions options = _regexOptions)
    {
        var match = Match(Text, pattern, options);
        if (match.Success)
        {
            Position += (match.Index + match.Length);
            return match.Length;
        }
        return null;
    }

    //= Looking Ahead
    // check
    /// <summary>
    /// Tests whether the given +pattern+ is matched from the current scan pointer.
    /// Returns the length of the match, or +null+.  The scan pointer is not advanced.
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public int? Check(string pattern, RegexOptions options = _regexOptions)
    {
        var match = Match(Text, pattern, options);
        if (match.Success)
        {
            return match.Length;
        }
        return null;
    }
    // check_until
    /// <summary>
    /// This returns the value that #scan_until would return, without advancing the
    /// scan pointer.  The match register is affected, though.
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public string? CheckUntil(string pattern, RegexOptions options = _regexOptions)
    {
        var match = Match(Text, pattern, options);
        if (match.Success)
        {
            return Text.Substring(0, match.Length);
        }
        return null;
    }
    // exist?

    // match?
    // peek
    // peek_byte
    //= Finding Where we Are
    // beginning_of_line? (<tt>// bol?</tt>)
    // eos?
    // rest?
    // rest_size
    // pos
    //= Setting Where we Are
    // reset
    public void Reset()
    {
        Position = 0;
    }
    // terminate
    public void Terminate()
    {
        Position = Text.Length;
    }

    private Match Match(string text, string pattern, RegexOptions options)
    {
        return Regex.Match(text, pattern, options);
    }

    // pos=
    //= Match Data
    // matched
    // matched?
    // matched_size
    // pre_match
    // post_match
    //= Miscellaneous
    // concat
    // string
    // string=
    // unscan
}