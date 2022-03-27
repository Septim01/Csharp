using System;
using System.Collections.Generic;

namespace Program
{
    class Program
    {
        static void multiply(List<int> large, int small)
        {
            int temp;
            int carry = 0;
            for (int i = 0; i < large.Count; ++i)
            {
                temp = large[i] * small;
                temp += carry;
                large[i] = temp % 10;
                carry = temp / 10;
            }
            while (carry > 0){
                large.Add(carry % 10);
                carry /= 10;
            }
        }
        static void Main()
        {
            int n = int.Parse(Console.ReadLine());
            List<int> a = new List<int>{1};
            for (int i = 2; i <= n; ++i)
            {
                multiply(a, i);
            }
            a.Reverse();
            foreach (var x in a)
            {
                Console.Write(x);
            }
            Console.WriteLine();
        }
    }
}
