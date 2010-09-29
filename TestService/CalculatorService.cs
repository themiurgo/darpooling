using System;

namespace OpenNETCF.WCF.Sample
{
    public class CalculatorService : ICalculator
    {
        public int Add(int a, int b)
        {
            Console.WriteLine(string.Format(
                "Received 'Add({0}, {1})' returning {2}", a, b, a + b));
            return a + b;
        }
    }
}
