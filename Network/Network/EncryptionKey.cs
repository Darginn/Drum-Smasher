using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Network
{
    public class EncryptionKey
    {
        private RSACryptoServiceProvider _rsa;

        public byte[] PrivateKey
        {
            get
            {
                return _rsa.ExportRSAPrivateKey();
            }
            set
            {
                _rsa.ImportRSAPrivateKey(value, out int _);
            }
        }
        public byte[] PublicKey
        {
            get
            {
                return _rsa.ExportRSAPublicKey();
            }
            set
            {
                _rsa.ImportRSAPublicKey(value, out int _);
            }
        }

        public RSAParameters Parameters
        {
            get
            {
                return _rsa.ExportParameters(true);
            }
            set
            {
                _rsa.ImportParameters(value);
            }
        }
        public RSAParameters ParametersPublic => _rsa.ExportParameters(false);

        public EncryptionKey()
        {
            _rsa = new RSACryptoServiceProvider();
        }

        public byte[] Encrypt(byte[] data)
        {
            return _rsa.Encrypt(data, false);
        }

        public byte[] Decrypt(byte[] data)
        {
            return _rsa.Decrypt(data, false);
        }
    }
}
