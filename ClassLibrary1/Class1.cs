﻿using System;

namespace ClassLibrary1
{
    public class Class1
    {

        private bool _blah;

        public bool Blah(bool value) {
            _blah = value;

      //string a = "2.5";
      //int b = (int)a;

      return _blah;


    }

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
    }

    public record B(int A1);
    public record C(int A1, int B1);

}
