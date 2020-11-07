using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace DSServerCommon.Encryption
{
    public class RSAEncryption
    {
        public string PublicXml { get; private set; }
        public string PublicPrivateXml { get; private set; }
        
        private const int _PROVIDER_RSA_FULL = 1;
        private const string _CONTAINER_NAME = "KeyContainer";

        public RSAEncryption()
        {
            using (RSACryptoServiceProvider provider = GetProvider(GetParameters()))
            {
                PublicXml = provider.ToXmlString(false);
                PublicPrivateXml = provider.ToXmlString(true);
            }
        }

        public RSAEncryption(string xml, bool @private)
        {
            using (RSACryptoServiceProvider provider = GetProvider(GetParameters(), xml))
            {
                PublicXml = provider.ToXmlString(false);

                if (@private)
                    PublicPrivateXml = provider.ToXmlString(true);
            }
        }

        public byte[] Encrypt(byte[] data)
        {
            using (RSACryptoServiceProvider provider = GetProvider(GetParameters(), PublicXml))
            {
                return provider.Encrypt(data, true);
            }
        }

        public byte[] Decrypt(byte[] data)
        {
            using (RSACryptoServiceProvider provider = GetProvider(GetParameters(), PublicPrivateXml))
            {
                return provider.Decrypt(data, true);
            }
        }

        private CspParameters GetParameters()
        {
            CspParameters cspParams;
            cspParams = new CspParameters(_PROVIDER_RSA_FULL);
            cspParams.KeyContainerName = _CONTAINER_NAME;
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            cspParams.ProviderName = "Microsoft Strong Cryptographic Provider";

            return cspParams;
        }

        private RSACryptoServiceProvider GetProvider(CspParameters parameters, string xmlString = null)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(parameters);
            provider.PersistKeyInCsp = false;

            if (xmlString != null)
                provider.FromXmlString(xmlString);

            return provider;
        }
    }
}
