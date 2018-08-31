using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kuznyechik
{
    class Program
    {

        static void Main(string[] args)
        {
            Kuznyechik a = new Kuznyechik();
            //Сообщение в виде массива байт
            byte[] M =
            {
                0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x00, 
                0xff, 0xee, 0xdd, 0xcc, 0xbb, 0xaa, 0x99, 0x88
            };
            //ключ 256 бит
            byte[] K =
            {
                0x88, 0x99, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff,
                0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77,
                0xfe, 0xdc, 0xba, 0x98, 0x76, 0x54, 0x32, 0x10,
                0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef
            };
            for (int i = 0; i < M.Length; i++)
            {
                Console.Write(Convert.ToChar(M[i]) + " ");
            }
            Console.WriteLine();
            byte[] b = a.encrypt(M, K);
            for (int i = 0; i < b.Length; i++)
            {
                Console.Write(b[i].ToString("x2") + " ");
            }
            Console.WriteLine();
            byte[] c = a.decrypt(b, K);
            for (int i = 0; i < c.Length; i++)
            {
                Console.Write(c[i].ToString("x2") + " ");
            }
            Console.WriteLine();

            Console.WriteLine("=============================");
            //Сообщение в виде строки
            string str = "I'll be back!   ";
            Console.WriteLine(str);
            b = a.encrypt(str.Select(x => Convert.ToByte(x)).ToArray(), K);
            for (int i = 0; i < b.Length; i++)
            {
                Console.Write(b[i].ToString("x2") + " ");
            }
            Console.WriteLine();
            c = a.decrypt(b, K);
            for (int i = 0; i < c.Length; i++)
            {
                Console.Write(Convert.ToChar(c[i]));
            }
            Console.ReadKey();
        }
    }
}
