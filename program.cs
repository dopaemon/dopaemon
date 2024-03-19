using System;
using System.Linq;
using System.Collections.Generic;

namespace DeQui
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> Origin = new List<int>();
            int max;
            Console.Write("Nhap so phan tu muon nhap: ");
            max = int.Parse(Console.ReadLine());
            for (int i = 0; i < max; i++)
            {
                // Console.Write("Nhap phan tu thu {0}: ", i + 1);
                // Origin.Add(int.Parse(Console.ReadLine()));
                Origin.Add(i + 1);
            }

            Console.Write("A = { ");
            for (int i = 0; i < max; i++)
            {
                Console.Write("{0} ", Origin[i]);
            }
            Console.WriteLine("};");

            Console.Write("Nhap so phan tu muon lay: ");
            int k = int.Parse(Console.ReadLine());
            List<int> kq = new List<int> { };
            kq.AddRange(Origin.OrderBy(x => Guid.NewGuid()).Take(k));
            kq.Sort();
           

            Console.Write("B = { ");
            for (int i = 0; i < k; i++) {
                Console.Write("{0} ", kq[i]);
            }
            Console.WriteLine("};");

            Console.WriteLine("{0}", Combination(max, k));
        }

        private static int Factorial(int n)
        {
            if (n == 0)
            {
                return 1;
            }
            return n * Factorial(n - 1);
        }

        private static int Combination(int n, int k)
        {
            return Factorial(n) / (Factorial(k) * Factorial(n - k));
        }

        ////private List<List<int>> RandomFunc(List<List<int>> a, int k, int max)
        ////{
        ////    int chap = Combination(max, k);
        ////    List<List<int>> kq;
        ////    return RandomFunc(kq, k, max);
        ////}
    }
}
