using System.Text;
using System.Text.RegularExpressions;

namespace StringScanner.Tests;

public class ScannerTests
{
    private Regex R(string pattern) => new Regex(pattern);
    private Regex wordRegex = new Regex(@"(?<word>\w+)");

    [Fact]
    public void TestNew()
    {
        var s = new Scanner("tør bøf");
        Assert.IsType<Scanner>(s);
        Assert.Equal(0, s.Pos);
        Assert.Equal("tør bøf", s.Str);
        Assert.Null(s.LastMatchLength);
    }

    [Fact]
    public void TestReset()
    {
        var s = new Scanner("tør bøf");
        s.Pos = 3;
        s.Reset();
        Assert.Equal(0, s.Pos);
        Assert.Equal("tør bøf", s.Str);
        Assert.Null(s.LastMatchLength);
    }

    [Fact]
    public void TestTerminate()
    {
        var s = new Scanner("tør bøf");
        s.Terminate();
        Assert.Equal(7, s.Pos);
        Assert.Null(s.LastMatchLength);
        Assert.True(s.IsEndOfString());
    }

    [Fact]
    public void TestRest()
    {
        var s = new Scanner("tør bøf");
        Assert.Equal("tør bøf", s.Rest());
        s.Pos = 3;
        Assert.Equal(" bøf", s.Rest());
        s.Pos = 4;
        Assert.Equal("bøf", s.Rest());
        s.Terminate();
        Assert.Equal("", s.Rest());
    }

    [Fact]
    public void TestRead()
    {
        var s = new Scanner("tør bøf");
        Assert.Equal("t", s.Read());
        Assert.Equal(1, s.Pos);
        Assert.Equal("ø", s.Read());
        Assert.Equal(2, s.Pos);
        Assert.Equal("r", s.Read());
        Assert.Equal(3, s.Pos);
    }

    [Fact]
    public void TestReadWithLength()
    {
        var s = new Scanner("tør bøf");
        Assert.Equal("tør", s.Read(3));
        Assert.Equal(3, s.Pos);
        Assert.Equal(" b", s.Read(2));
        Assert.Equal(5, s.Pos);
        Assert.Equal("øf", s.Read(2));
        Assert.Equal(7, s.Pos);
    }

    [Fact]
    public void TestIsEndOfString()
    {
        var s = new Scanner("tør bøf");
        Assert.False(s.IsEndOfString());
        s.Pos = 0;
        Assert.False(s.IsEndOfString());
        s.Pos = 4;
        Assert.False(s.IsEndOfString());
        s.Pos = 7;
        Assert.True(s.IsEndOfString());
    }

    [Fact]
    public void TestIsBeginningOfString()
    {
        var s = new Scanner("tør bøf");
        Assert.True(s.IsBeginningOfString());
        s.Pos = 1;
        Assert.False(s.IsBeginningOfString());
        s.Pos = 4;
        Assert.False(s.IsBeginningOfString());
        s.Pos = 7;
        Assert.False(s.IsBeginningOfString());
    }

    [Fact]
    public void TestIsEndOfLine()
    {
        var s = new Scanner("tør\nbøf");
        Assert.False(s.IsEndOfLine());
        s.Pos = 3;
        Assert.True(s.IsEndOfLine());
        s.Pos = 4;
        Assert.False(s.IsEndOfLine());
        s.Pos = 5;
        Assert.False(s.IsEndOfLine());
        s.Pos = 7;
        Assert.True(s.IsEndOfLine());
    }

    [Fact]
    public void TestIsBeginningOfLine()
    {
        var s = new Scanner("tør\nbøf");
        Assert.True(s.IsBeginningOfLine());
        s.Pos = 3;
        Assert.False(s.IsBeginningOfLine());
        s.Pos = 4;
        Assert.True(s.IsBeginningOfLine());
        s.Pos = 5;
        Assert.False(s.IsBeginningOfLine());
        s.Pos = 7;
        Assert.False(s.IsBeginningOfLine());
    }

    [Fact]
    public void TestPos()
    {
        var s = new Scanner("tør bøf");
        Assert.Equal(0, s.Pos);
        s.Read();
        Assert.Equal(1, s.Pos);
        s.Read();
        Assert.Equal(2, s.Pos);
        s.Terminate();
        Assert.Equal(7, s.Pos);
    }

    [Fact]
    public void TestScanWithFunc()
    {
        var s = new Scanner("tør bøf)");
        Assert.Equal("ordet er: tør", s.Scan(wordRegex, m => $"ordet er: {m("word")}"));
        Assert.Null(s.Scan(wordRegex, m => m("word")!));
        Assert.True(s.Scan(R(@"\s+")));
        Assert.True(s.Scan(R("bø")));
        Assert.Equal("f", s.Scan(wordRegex, m => m("word")!));
        Assert.Null(s.Scan(wordRegex, m => m("word")!));
    }

    [Fact]
    public void TestScan()
    {
        var s = new Scanner("tør bøf");
        Assert.True(s.Scan(wordRegex));
        Assert.Equal(3, s.LastMatchLength);
        Assert.Equal(3, s.Pos);
        Assert.False(s.Scan(wordRegex));
        Assert.Equal(3, s.Pos);
        Assert.True(s.Scan(R(@"\s+")));
        Assert.Equal(1, s.LastMatchLength);
        Assert.Equal(4, s.Pos);
        Assert.True(s.Scan(R("bø")));
        Assert.Equal(2, s.LastMatchLength);
        Assert.Equal(6, s.Pos);
        Assert.True(s.Scan(wordRegex));
        Assert.Equal(1, s.LastMatchLength);
        Assert.Equal(7, s.Pos);
        Assert.False(s.Scan(wordRegex));
        Assert.Equal(7, s.Pos);
    }

    [Fact]
    public void TestCheckWithFunc()
    {
        var s = new Scanner("tør bøf");
        Assert.Equal("tø", s.Check(R("(?<letter>t)(?<letter2>ø)r"), m => m("letter")! + m("letter2")!));
        Assert.Equal(0, s.Pos);
        Assert.Equal(3, s.LastMatchLength);
        Assert.False(s.Check(R("bøf")));
    }

    [Fact]
    public void TestCheck()
    {
        var s = new Scanner("tør bøf");
        Assert.True(s.Check(R("tør")));
        Assert.Equal(0, s.Pos);
        Assert.Equal(3, s.LastMatchLength);
        Assert.False(s.Check(R("bøf")));
    }

    [Fact]
    public void TestCheckUntilWithFunc()
    {
        var s = new Scanner("tør bøf");
        Assert.Equal("r", s.CheckUntil(R("(?<letter>r)"), m => m("letter")!));
        Assert.Equal(0, s.Pos);
        Assert.Equal("b", s.CheckUntil(R("(?<letter>b)"), m => m("letter")!));
        Assert.Equal(0, s.Pos);
        Assert.Null(s.CheckUntil(R("(?<letter>x)"), m => m("letter")!));
    }

    [Fact]
    public void TestCheckUntil()
    {
        var s = new Scanner("tør bøf");
        Assert.True(s.CheckUntil(R("r")));
        Assert.Equal(0, s.Pos);
        Assert.True(s.CheckUntil(R("b")));
        Assert.Equal(0, s.Pos);
        Assert.False(s.CheckUntil(R("x")));
    }

    [Fact]
    public void TestSkipWithFunc()
    {
        var s = new Scanner("tør bøf");
        Assert.False(s.Skip(R("r")));
        Assert.Equal(0, s.Pos);
        Assert.True(s.Skip(R("(?<dry>tør) (?<beef>bøf)")));
        Assert.Equal(7, s.Pos);
    }

    [Fact]
    public void TestSkip()
    {
        var s = new Scanner("tør bøf");
        Assert.False(s.Skip(R("r")));
        Assert.Equal(0, s.Pos);
        Assert.True(s.Skip(R("tør")));
        Assert.Equal(3, s.Pos);
    }

    [Fact]
    public void TestScanUntil()
    {
        var s = new Scanner("tør bøf.");
        Assert.True(s.ScanUntil(R(@"bø")));
        Assert.Equal(6, s.Pos);
        Assert.False(s.ScanUntil(R(@"XYZ")));
        Assert.Equal(6, s.Pos);
        Assert.True(s.ScanUntil(wordRegex));
        Assert.Equal(7, s.Pos);
        Assert.False(s.ScanUntil(wordRegex));
        Assert.Equal(7, s.Pos);
    }

    [Fact]
    public void TestScanUntilWithCaptures()
    {
        var s = new Scanner("tør bøf.");
        s.ScanUntil(R(@"(?<beef>bøf)"));
        Assert.Equal(7, s.Pos);
        Assert.Null(s.ScanUntil(R(@"(?<beef>XYZ)"), m => m("beef")!));
        Assert.Equal(7, s.Pos);
        Assert.Null(s.ScanUntil(wordRegex, m => m("word")!));
        s.ScanUntil(wordRegex);
        Assert.Equal(7, s.Pos);
        s.ScanUntil(wordRegex);
        Assert.Equal(7, s.Pos);
    }

    [Fact]
    public void TestSkipUntil()
    {
        var s = new Scanner("tør bøf");
        s.SkipUntil(R("r"));
        Assert.Equal(3, s.Pos);
        s.SkipUntil(R("b"));
        Assert.Equal(5, s.Pos);
        Assert.False(s.SkipUntil(R("x")));
        Assert.Equal(5, s.Pos);
    }

    [Fact]
    public void TestSkipUntilWithCaptures()
    {
        var s = new Scanner("tør bøf");
        s.SkipUntil(R("r"));
        Assert.Equal(3, s.Pos);
        s.SkipUntil(R("b"));
        Assert.Equal(5, s.Pos);
        s.SkipUntil(R("x"));
        Assert.Equal(5, s.Pos);
    }

    [Fact]
    public void TestValuesAtWithPositionalParams()
    {
        var s = new Scanner("Timestamp: Fri Dec 12 1975 14:39");
        s.Scan(R("Timestamp: "));
        s.Scan(R(@"(\w+) (\w+) (\d+) "));
        Assert.Equal(new string[] { "Fri Dec 12 ", "12", "", "Dec" }, s.ValuesAt(0, -1, 5, 2));
        s.Scan(R(@"(\w+) (\w+) (\d+) "));
        Assert.Null(s.ValuesAt(0, -1, 5, 2));
    }

    [Fact]
    public void TestValuesAtWithNamedParams()
    {
        var s = new Scanner("Timestamp: Fri Dec 12 1975 14:39");
        s.Scan(R("Timestamp: "));
        s.Scan(R(@"(?<date>(?<day>\w+) (?<month>\w+) (?<year>\d+)) "));
        Assert.Equal(new string[] { "Fri Dec 12", "Fri", "12", "Dec" }, s.ValuesAt("date", "day", "year", "month"));
        s.Scan(R(@"(\w+) (\w+) (\d+) "));
        Assert.Null(s.ValuesAt(0, -1, 5, 2));
    }

    [Fact]
    public void TestCaptures()
    {
        var s = new Scanner("Timestamp: Fri Dec 12 1975 14:39");
        s.Scan(R("Timestamp: "));
        s.Scan(R(@"((\w+) (\w+) (\d+)) "));
        Assert.Equal(new string[] { "Fri Dec 12", "Fri", "Dec", "12" }, s.Captures());
        s.Scan(R(@"(\w+) (\w+) (\d+) "));
        Assert.Null(s.Captures());
    }

    [Fact]
    public void TestNamedCaptures()
    {
        var s = new Scanner("Timestamp: Fri Dec 12 1975 14:39");
        s.Scan(R("Timestamp: "));
        s.Scan(R(@"(?<date>(?<day>\w+) (?<month>\w+) (?<year>\d+)) "));
        Assert.Equal(new Dictionary<string, string> { { "date", "Fri Dec 12" }, { "day", "Fri" }, { "month", "Dec" }, { "year", "12" } }, s.NamedCaptures());
        s.Scan(R(@"(\w+) (\w+) (\d+) "));
        Assert.Null(s.NamedCaptures());
    }

    [Fact]
    public void TestPreAndPostMatch()
    {
        var s = new Scanner("a b c d e");
        s.Scan(R(@"\w"));
        Assert.Equal("", s.PreMatch());
        Assert.Equal(" b c d e", s.PostMatch());
        s.Skip(R(@"\s"));
        Assert.Equal("a", s.PreMatch());
        Assert.Equal("b c d e", s.PostMatch());
        s.Scan(R("b"));
        Assert.Equal("a ", s.PreMatch());
        Assert.Equal(" c d e", s.PostMatch());
        s.ScanUntil(R("c"));
        Assert.Equal("a b ", s.PreMatch());
        Assert.Equal(" d e", s.PostMatch());
        Assert.Equal(" ", s.Read());
        Assert.Null(s.PreMatch());
        Assert.Null(s.PostMatch());
        s.Scan(R("never match"));
        Assert.Null(s.PreMatch());
        Assert.Null(s.PostMatch());
    }
}