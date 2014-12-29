using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Cryptor
{



    public static class EncryptDecrypt
    {

        public static string EncodeQuery(string NumberToEncode, byte[] Key)
        {
            
            string EncodedValue = Encode(Convert.ToInt32(NumberToEncode), Key);
            return EncodedValue;
        }

        public static int DecodeQuery(string NumberToDecode, byte[] Key)
        {
            

            long DecodedValue = Decode(NumberToDecode, Key);

            return Convert.ToInt32(DecodedValue);
        }


        static string ToHex(byte[] value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in value)
                sb.AppendFormat("{0:x2}", b);
            return sb.ToString();
        }
        static string Encode(long value, byte[] key)
        {
            byte[] InputBuffer = new byte[8];
            byte[] OutputBuffer;
            unsafe
            {
                fixed (byte* pInputBuffer = InputBuffer)
                {
                    ((long*)pInputBuffer)[0] = value;
                }
            }
            TripleDESCryptoServiceProvider TDes = new TripleDESCryptoServiceProvider();
            TDes.Mode = CipherMode.ECB;
            TDes.Padding = PaddingMode.None;
            TDes.Key = key;

            using (ICryptoTransform Encryptor = TDes.CreateEncryptor())
            {
                OutputBuffer = Encryptor.TransformFinalBlock(InputBuffer, 0, 8);
            }
            TDes.Clear();

            return ToHex(OutputBuffer);
        }
        static long Decode(string value, byte[] key)
        {
            byte[] InputBuffer = new byte[8];
            byte[] OutputBuffer;

            for (int i = 0; i < 8; i++)
            {
                InputBuffer[i] = Convert.ToByte(value.Substring(i * 2, 2), 16);
            }

            TripleDESCryptoServiceProvider TDes = new TripleDESCryptoServiceProvider();
            TDes.Mode = CipherMode.ECB;
            TDes.Padding = PaddingMode.None;
            TDes.Key = key;

            using (ICryptoTransform Decryptor = TDes.CreateDecryptor())
            {
                OutputBuffer = Decryptor.TransformFinalBlock(InputBuffer, 0, 8);
            }
            TDes.Clear();

            unsafe
            {
                fixed (byte* pOutputBuffer = OutputBuffer)
                {
                    return ((long*)pOutputBuffer)[0];
                }
            }
        }
        static void Main(string[] args)
        {
            //long NumberToEncode = (new Random()).Next();
            //Console.WriteLine("Number to encode = {0}.", NumberToEncode);
            //byte[] Key = new byte[24];
            //(new RNGCryptoServiceProvider()).GetBytes(Key);
            //Console.WriteLine("Key to encode with is {0}.", ToHex(Key));
            //string EncodedValue = Encode(NumberToEncode, Key);
            //Console.WriteLine("The encoded value is {0}.", EncodedValue);
            //long DecodedValue = Decode(EncodedValue, Key);
            //Console.WriteLine("The decoded result is {0}.", DecodedValue);
        }

       
    }
}
