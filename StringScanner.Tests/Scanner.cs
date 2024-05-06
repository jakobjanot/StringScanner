namespace StringScanner.Tests;

public class ScannerTests
{
    [Fact]
    public void TestScan()
    {
        var scanner = new Scanner("Hello, World!");

        Assert.Equal("Hello", scanner.Scan("Hello"));
        Assert.Equal(", ", scanner.Scan(", "));
        Assert.Null(scanner.Scan("Hello"));
        Assert.Equal("World", scanner.Scan("World"));
        Assert.Equal("!", scanner.Scan("!"));
        Assert.Null(scanner.Scan("Hello"));
    }
}