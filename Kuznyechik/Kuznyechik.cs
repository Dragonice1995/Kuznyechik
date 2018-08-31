using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kuznyechik
{
    class Kuznyechik
    {
        private byte[] sblock = {252, 238, 221, 17, 207, 110, 49, 22, 251, 196, 250, 218, 35, 197, 4, 77, 233, 119, 240,
                                219, 147, 46, 153, 186, 23, 54, 241, 187, 20, 205, 95, 193, 249, 24, 101, 90, 226, 92, 239,
                                33, 129, 28, 60, 66, 139, 1, 142, 79, 5, 132, 2, 174, 227, 106, 143, 160, 6, 11, 237, 152, 127,
                                212, 211, 31, 235, 52, 44, 81, 234, 200, 72, 171, 242, 42, 104, 162, 253, 58, 206, 204, 181,
                                112, 14, 86, 8, 12, 118, 18, 191, 114, 19, 71, 156, 183, 93, 135, 21, 161, 150, 41, 16, 123,
                                154, 199, 243, 145, 120, 111, 157, 158, 178, 177, 50, 117, 25, 61, 255, 53, 138, 126, 109,
                                84, 198, 128, 195, 189, 13, 87, 223, 245, 36, 169, 62, 168, 67, 201, 215, 121, 214, 246, 124,
                                34, 185, 3, 224, 15, 236, 222, 122, 148, 176, 188, 220, 232, 40, 80, 78, 51, 10, 74, 167, 151,
                                96, 115, 30, 0, 98, 68, 26, 184, 56, 130, 100, 159, 38, 65, 173, 69, 70, 146, 39, 94, 85, 47,
                                140, 163, 165, 125, 105, 213, 149, 59, 7, 88, 179, 64, 134, 172, 29, 247, 48, 55, 107, 228,
                                136, 217, 231, 137, 225, 27, 131, 73, 76, 63, 248, 254, 141, 83, 170, 144, 202, 216, 133, 97,
                                32, 113, 103, 164, 45, 43, 9, 91, 203, 155, 37, 208, 190, 229, 108, 82, 89, 166, 116, 210,
                                230, 244, 180, 192, 209, 102, 175, 194, 57, 75, 99, 182};

        private byte[] constR = { 148, 32, 133, 16, 194, 192, 1, 251, 1, 192, 194, 16, 133, 32, 148, 1 };

        //xor двух векторов
        private byte[] XOR(byte[] a, byte[] b)
        {
            byte[] result = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                result[i] = (byte)(a[i] ^ b[i]);
            }
            return result;
        }
        //преобразование S
        private byte[] S(byte[] a)
        {
            byte[] result = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                result[i] = sblock[a[i]];
            }
            return result;
        }
        //преобразование обратное S
        public byte[] ReverseS(byte[] a)
        {
            //byte[] result = new byte[16];
            //for (int i = 0; i < 16; i++)
            //{
            //    result[i] = (byte)sblock.ToList().IndexOf(a[i]);
            //}
            return a.Select(x => (byte)sblock.ToList().IndexOf(x)).ToArray();
        }
        //Сложение в поле F(2) по модулю x^8 + x^7 + x^6 + x + 1
        private byte mult(byte a, byte b)
        {
            byte result = 0x00;
            while (b != 0x00)
            {
                if ((b & 1) != 0x00)
                {
                    result ^= a;
                }
                a = (byte)((a << 1) ^ ((a & 0x80) == 0x80 ? 0xC3 : 0x00));
                b >>= 1;
            }
            return result;
        }
        //преобразование R
        private byte[] R(byte[] a)
        {
            byte c = 0x00;
            for (int i = 0; i < 16; i++)
            {
                c ^= mult(constR[i], a[i]);
            }
            for (int i = 14; i >= 0; i--)
            {
                a[i + 1] = a[i];
            }
            a[0] = c;
            return a;
        }
        //преобразование обратное R
        private byte[] ReverseR(byte[] a)
        {
            byte c = a[0];
            for (int i = 0; i < 15; i++)
            {
                a[i] = a[i + 1];
            }
            a[15] = c;
            c = 0x00;
            for (int i = 0; i < 16; i++)
            {
                c ^= mult(constR[i], a[i]);
            }
            a[15] = c;
            return a;
        }
        //Преобразование L
        private byte[] L(byte[] a)
        {
            for (int i = 0; i < 16; i++)
            {
                a = R(a);
            }
            return a;
        }
        //Преобразование обратное L
        private byte[] ReverseL(byte[] a)
        {
            for (int i = 0; i < 16; i++)
            {
                a = ReverseR(a);
            }
            return a;
        }
        //Преобразование F
        private byte[] F(byte[] a, byte[] b, byte[] K)
        {
            byte[] result = new byte[32];
            Array.Copy(a, 0, result, 16, 16);
            byte[] vr = new byte[16];
            vr = XOR(a, K);
            vr = S(vr);
            vr = L(vr);
            vr = XOR(vr, b);
            Array.Copy(vr, 0, result, 0, 16);
            return result;
        }
        //Генерация раундовых ключей
        private byte[][] genK(byte[] K)
        {
            byte[][] result = new byte[10][];
            for (int i = 0; i < 10; i++)
            {
                result[i] = new byte[16];
            }
            Array.Copy(K, 0, result[0], 0, 16);
            Array.Copy(K, 16, result[1], 0, 16);
            for (int i = 0; i < 8; i += 2)
            {
                byte[] keys = new byte[32];
                Array.Copy(result[i], 0, keys, 0, 16);
                Array.Copy(result[i + 1], 0, keys, 16, 16);
                for (int j = 0; j < 8; j++)
                {
                    byte[] K1 = new byte[16];
                    byte[] K2 = new byte[16];
                    Array.Copy(keys, 0, K1, 0, 16);
                    Array.Copy(keys, 16, K2, 0, 16);
                    byte[] vecint = new byte[16];
                    for (int z = 0; z < 15; z++)
                    {
                        vecint[0] = 0x00;
                    }
                    vecint[15] = (byte)((i / 2) * 8 + j + 1);
                    keys = F(K1, K2, L(vecint));
                }
                Array.Copy(keys, 0, result[i + 2], 0, 16);
                Array.Copy(keys, 16, result[i + 3], 0, 16);
            }
            return result;
        }
        //Шифрование
        public byte[] encrypt(byte[] M, byte[] key)
        {
            byte[] K1 = new byte[16];
            byte[] K2 = new byte[16];
            byte[][] K = genK(key);
            for (int i = 0; i < 9; i++)
            {
                M = XOR(M, K[i]);
                M = S(M);
                M = L(M);
            }
            M = XOR(M, K[9]);
            return M;
        }
        //Дешифрование
        public byte[] decrypt(byte[] M, byte[] key)
        {
            byte[] K1 = new byte[16];
            byte[] K2 = new byte[16];
            byte[][] K = genK(key);
            for (int i = 9; i > 0; i--)
            {
                M = XOR(M, K[i]);
                M = ReverseL(M);
                M = ReverseS(M);
            }
            M = XOR(M, K[0]);
            return M;
        }
    }
}
