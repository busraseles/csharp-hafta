var c1 = new NoktaClass { X = 1, Y = 2 };
var c2 = new NoktaClass { X = 1, Y = 2 };
var r1 = new NoktaRecord(1, 2);
var r2 = new NoktaRecord(1, 2);

Console.WriteLine(c1 == c2);
Console.WriteLine(r1 == r2);
Console.WriteLine(c1);
Console.WriteLine(r1);

public class NoktaClass
{
    public int X { get; init; }
    public int Y { get; init; }
}

public record NoktaRecord(int X, int Y);