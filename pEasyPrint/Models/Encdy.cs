using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cryptor;

namespace pEasyPrint.Models
{
    public static class Encdy
    {
         //byte[] Key = new byte[24];
         //   (new RNGCryptoServiceProvider()).GetBytes(Key);

        public static string Encode(int id)
        {
            return EncryptDecrypt.EncodeQuery(id.ToString(), GetBytes("AmonousTechn"));
        }

        public static int Decode(string id)
        {
            return EncryptDecrypt.DecodeQuery(id, GetBytes("AmonousTechn"));
        }

         static byte[] GetBytes(string str)
         {
             byte[] bytes = new byte[str.Length * sizeof(char)];
             System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
             return bytes;
         }

    }
}