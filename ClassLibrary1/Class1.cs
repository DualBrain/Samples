using System;

namespace ClassLibrary1
{
    public class Class1
    {

        public static void InlineIncrementTest()
        {

            int value = 1;
            Console.WriteLine($"({value}) value=5 = {value=5} ({value})");
            Console.WriteLine($"({value}) value++ = {value++} ({value})");
            Console.WriteLine($"({value}) value-- = {value--} ({value})");
            Console.WriteLine($"({value}) ++value = {++value} ({value})");
            Console.WriteLine($"({value}) --value = {--value} ({value})");

        }

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
