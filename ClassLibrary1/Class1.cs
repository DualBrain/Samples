using System;

namespace ClassLibrary1
{
    public class Class1
    {

    }

    public record A
    {
        public int A1 { get; init; }
        public int B1 { get; init; }
        public int C1 { get; init; }
    }

    public record B(int A1);
    public record C(int A1, int B2);

}
