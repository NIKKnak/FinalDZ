using System.Security.Cryptography;

namespace WebApiLibrary.rsa
{
    public static class RSAService
    {
        public static RSA GetPublicKey()
        {
            var key = File.ReadAllText(@"..\WebApiLibrary\rsa\public_key.pem");
            var rsa = RSA.Create();
            rsa.ImportFromPem(key);
            return rsa;
        }

        public static RSA GetPrivateKey()
        {
            var key = File.ReadAllText(@"..\WebApiLibrary\rsa\private_key.pem");
            var rsa = RSA.Create();
            rsa.ImportFromPem(key);
            return rsa;
        }
    }
}
