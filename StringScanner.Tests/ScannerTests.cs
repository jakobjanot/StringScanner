using System.Text;

namespace StringScanner.Tests;

public class ScannerTests
{
    [Fact]
    public void TestNew()
    {
        var s = new Scanner("tør bøf");
        Assert.IsType<Scanner>(s);
        Assert.Equal(0, s.Pos);
        Assert.Equal("tør bøf", s.Str);
        Assert.Null(s.Matched);
    }

    [Fact]
    public void TestReset()
    {
        var s = new Scanner("tør bøf");
        s.Pos = 3;
        s.Reset();
        Assert.Equal(0, s.Pos);
        Assert.Equal("tør bøf", s.Str);
        Assert.Null(s.Matched);
    }

    [Fact]
    public void TestTerminate()
    {
        var s = new Scanner("tør bøf");
        s.Terminate();
        Assert.Equal(7, s.Pos);
        Assert.Null(s.Matched);
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
    public void TestScan()
    {
        var s = new Scanner("tør bøf");
        Assert.Equal("tør", s.Scan(@"\w+"));
        Assert.Null(s.Scan(@"\w+"));
        Assert.Equal(" ", s.Scan(@"\s+"));
        Assert.Equal("bø", s.Scan("bø"));
        Assert.Equal("f", s.Scan(@"\w+"));
        Assert.Null(s.Scan(@"\w+"));
    }

    [Fact]
    public void TestCheck()
    {
        var s = new Scanner("tør bøf");
        Assert.Equal("tør", s.Check("tør"));
        Assert.Equal(0, s.Pos);
        Assert.Equal("tør", s.Matched?.ToString());
        Assert.Null(s.Check("bøf"));
    }

    [Fact]
    public void TestSkip()
    {
        var s = new Scanner("tør bøf");
        Assert.Null(s.Skip("r"));
        Assert.Equal(0, s.Pos);
        Assert.Equal(3, s.Skip("tør"));
        Assert.Equal(3, s.Pos);
    }

    [Fact]
    public void TestScanUntil()
    {
        var s = new Scanner("tør bøf.");
        Assert.Equal("tør bø", s.ScanUntil(@"bø"));
        Assert.Null(s.ScanUntil(@"XYZ"));
        Assert.Equal("f", s.ScanUntil(@"\w+"));
        Assert.Null(s.ScanUntil(@"\w+"));
    }

    [Fact]
    public void TestSkipUntil()
    {
        var s = new Scanner("tør bøf");
        s.SkipUntil("r");
        Assert.Equal(3, s.Pos);
        s.SkipUntil("b");
        Assert.Equal(5, s.Pos);
        Assert.Null(s.SkipUntil("x"));
    }
    
    [Fact]
    public void TestCheckUntil()
    {
        var s = new Scanner("tør bøf");
        Assert.Equal("tør", s.CheckUntil("r"));
        Assert.Equal(0, s.Pos);
        Assert.Equal("tør b", s.CheckUntil("b"));
        Assert.Equal(0, s.Pos);
        Assert.Null(s.CheckUntil("x"));
    }

    [Fact]
    public void TestValuesAtWithPositionalParams()
    {
        var s = new Scanner("Timestamp: Fri Dec 12 1975 14:39");
        s.Scan("Timestamp: ");
        s.Scan(@"(\w+) (\w+) (\d+) ");
        Assert.Equal(new string[] { "Fri Dec 12 ", "12", "", "Dec" }, s.ValuesAt(0, -1, 5, 2));
        s.Scan(@"(\w+) (\w+) (\d+) ");
        Assert.Null(s.ValuesAt(0, -1, 5, 2));
    }

    [Fact]
    public void TestValuesAtWithNamedParams()
    {
        var s = new Scanner("Timestamp: Fri Dec 12 1975 14:39");
        s.Scan("Timestamp: ");
        s.Scan(@"(?<date>(?<day>\w+) (?<month>\w+) (?<year>\d+)) ");
        Assert.Equal(new string[] { "Fri Dec 12", "Fri", "12", "Dec" }, s.ValuesAt("date", "day", "year", "month"));
        s.Scan(@"(\w+) (\w+) (\d+) ");
        Assert.Null(s.ValuesAt(0, -1, 5, 2));
    }

    [Fact]
    public void TestCaptures()
    {
        var s = new Scanner("Timestamp: Fri Dec 12 1975 14:39");
        s.Scan("Timestamp: ");
        s.Scan(@"((\w+) (\w+) (\d+)) ");
        Assert.Equal(new string[] { "Fri Dec 12", "Fri", "Dec", "12" }, s.Captures());
        s.Scan(@"(\w+) (\w+) (\d+) ");
        Assert.Null(s.Captures());
    }

    [Fact]
    public void TestNamedCaptures()
    {
        var s = new Scanner("Timestamp: Fri Dec 12 1975 14:39");
        s.Scan("Timestamp: ");
        s.Scan(@"(?<date>(?<day>\w+) (?<month>\w+) (?<year>\d+)) ");
        Assert.Equal(new Dictionary<string, string> { { "date", "Fri Dec 12" }, { "day", "Fri" }, { "month", "Dec" }, { "year", "12" } }, s.NamedCaptures());
        s.Scan(@"(\w+) (\w+) (\d+) ");
        Assert.Null(s.NamedCaptures());
    }

    [Fact]
    public void TestPreAndPostMatch()
    {
        var s = new Scanner("a b c d e");
        s.Scan(@"\w");
        Assert.Equal("", s.PreMatch());
        Assert.Equal(" b c d e", s.PostMatch());
        s.Skip(@"\s");
        Assert.Equal("a", s.PreMatch());
        Assert.Equal("b c d e", s.PostMatch());
        s.Scan("b");
        Assert.Equal("a ", s.PreMatch());
        Assert.Equal(" c d e", s.PostMatch());
        s.ScanUntil("c");
        Assert.Equal("a b ", s.PreMatch());
        Assert.Equal(" d e", s.PostMatch());
        Assert.Equal(" ", s.Read());
        Assert.Null(s.PreMatch());
        Assert.Null(s.PostMatch());
        s.Scan("never match");
        Assert.Null(s.PreMatch());
        Assert.Null(s.PostMatch());
    }
}