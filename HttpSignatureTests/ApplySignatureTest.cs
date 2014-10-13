using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HttpSignatures;
using NUnit.Framework;

namespace HttpSignatureTests
{
    [TestFixture()]
    public class ApplySignatureTest
    {
        [Test]
        public void ApplySignatureToIRequest()
        {
            var signer = new HttpSigner(new AuthorizationParser(), new HttpSignatureStringExtractor());

            var request = new Request();
            request.Path = "/hello-world";
            request.Method = HttpMethod.Post;
            request.SetHeader("host", "example.com");
            request.SetHeader("date", DateTime.UtcNow.ToString("r"));

            var spec = new SignatureSpecification()
            {
                Algorithm = "hmac-sha256",
                Headers = new string[] {"host", "(request-target)"},
                KeyId = Fixture.KeyId
            };
            signer.Sign(request, spec, Fixture.KeyId, Fixture.KeyBase64);
            var keyStore = new KeyStore(new Dictionary<string, string>()
            {
                {Fixture.KeyId, Fixture.KeyBase64}
            });
            var signature = signer.Signature(request, spec, keyStore);
            Assert.True(signature.Valid);
        }
    }
}
