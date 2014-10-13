using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

namespace HttpSignatures
{
    public static class HttpSignature
    {
        static HttpSignature()
        {
            AuthParser = new AuthorizationParser();
            Signer = new HttpSigner(new AuthorizationParser(), new HttpSignatureStringExtractor());
        }

        private static HttpSigner Signer { get; set; }

        private static IHttpSignatureStringExtractor SignatureStringExtractor { get; set; }

        private static IAuthorizationParser AuthParser { get; set; }

        public static VerifiedSignature VerifiedSignature(HttpRequest request, ISignatureSpecification spec, IKeyStore keyStore)
        {
            return Signer.Signature(request, spec, keyStore);         
        }

        public static void Sign(HttpRequestMessage request, ISignatureSpecification spec, string keyId, string key)
        {
            Signer.Sign(request, spec, keyId, key);
        }

        public static void Sign(IRequest request, ISignatureSpecification spec, string keyId, string key)
        {
            Signer.Sign(request, spec, keyId, key);
        }
    }
}
