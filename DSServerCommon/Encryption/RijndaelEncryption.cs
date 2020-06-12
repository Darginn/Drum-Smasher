using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DSServerCommon.Encryption
{
    public class RijndaelEncryption
    {
        public byte[] Key { get; private set; }
        public byte[] IV { get; private set; }

        public RijndaelEncryption()
        {
            using (Rijndael rjn = GetRijndael(true))
            {
                Key = rjn.Key;
                IV = rjn.IV;
            }
        }
        
        public RijndaelEncryption(byte[] key, byte[] iv)
        {
            Key = key;
            IV = iv;
        }

        private Rijndael GetRijndael(bool createNew = false)
        {
            Rijndael r = Rijndael.Create();
            r.Padding = PaddingMode.PKCS7;
            r.Mode = CipherMode.CBC;

            if (!createNew)
            {
                r.IV = IV;
                r.Key = Key;
            }

            return r;
        }

        private byte[] DoCrypticAction(byte[] data, ICryptoTransform transform)
        {
            using (MemoryStream mstream = new MemoryStream())
            {
                using (CryptoStream crypto = new CryptoStream(mstream, transform, CryptoStreamMode.Write))
                {
                    crypto.Write(data, 0, data.Length);
                    crypto.FlushFinalBlock();
                }

                return mstream.ToArray();
            }
        }

        public byte[] Encrypt(byte[] data)
        {
            using (Rijndael rjn = GetRijndael())
            {
                ICryptoTransform transform = rjn.CreateEncryptor();

                return DoCrypticAction(data, transform);
            }
        }

        public byte[] Decrypt(byte[] data)
        {
            using (Rijndael rjn = GetRijndael())
            {
                ICryptoTransform transform = rjn.CreateDecryptor();

                return DoCrypticAction(data, transform);
            }
        }
    }
}
