using System;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace HttpSignatures
{

	public interface IHttpSigner {
        VerifiedSignature Signature(IRequest r, ISignatureSpecification spec, IKeyStore keyStore);
        VerifiedSignature Signature(HttpRequest r, ISignatureSpecification spec, IKeyStore keyStore);
	}

	public class HttpSigner : IHttpSigner
	{
		IAuthorizationParser authorizationParser;

		IHttpSignatureStringExtractor signatureStringExtractor;

		public HttpSigner (IAuthorizationParser authorizationParser, IHttpSignatureStringExtractor signatureStringExtractor)
		{
			this.signatureStringExtractor = signatureStringExtractor;
			this.authorizationParser = authorizationParser;
		}

		public VerifiedSignature Signature (IRequest r, ISignatureSpecification spec, IKeyStore keyStore)
		{
			var authorization = r.GetHeader("authorization");
		    if (string.IsNullOrEmpty(authorization)) throw new SignatureMissingException("No authorization header present");

			var signatureAuth = authorizationParser.Parse(authorization);
			if (spec == null) {
				spec = signatureAuth;
			} else {
				if (spec.Algorithm != signatureAuth.Algorithm) {
					throw new InvalidSignatureException(string.Format("Algorith mismatch. Wanted: {0}, found: {1}", spec.Algorithm, signatureAuth.Algorithm));
				}
				var missingHeaders = spec.Headers.Where(h=> !signatureAuth.Headers.Contains(h)).ToList();
				if (missingHeaders.Any()) {
					throw new InvalidSignatureException(string.Format("Missing headers in signature: {0}", string.Join(",", missingHeaders)));
				}
			}

		    var signature = CalculateSignature(r, spec, keyStore.Get(signatureAuth.KeyId));
            return new VerifiedSignature(signatureAuth, signature == signatureAuth.Signature);
		}

	    public string CalculateSignature(IRequest r, ISignatureSpecification spec, string key)
	    {
	        var algorithm = spec.Algorithm;
            var signatureString = signatureStringExtractor.ExtractSignatureString(r, spec);
            var hmac = HMAC.Create(algorithm.Replace("-", "").ToUpper());
            hmac.Initialize();
            hmac.Key = Convert.FromBase64String(key);
            var bytes = hmac.ComputeHash(new MemoryStream(Encoding.UTF8.GetBytes(signatureString)));
            var signature = Convert.ToBase64String(bytes);
	        return signature;
	    }

	    public void Sign(IRequest r, ISignatureSpecification spec, string keyId, string base64Key)
	    {
            var signature = CalculateSignature(r, spec, base64Key);
	        var auth = FormatAuthorization(spec, signature);
            r.SetHeader("Authorization", auth);
	    }

        public void Sign(HttpRequestMessage r, ISignatureSpecification spec, string keyId, string base64Key)
        {
            var req = new HttpRequestMessageWrapper(r);
            Sign(req, spec, keyId, base64Key);
        }

	    private string FormatAuthorization(ISignatureSpecification spec, string signature)
	    {
	        return string.Format("Signature keyId=\"{0}\",algorithm=\"{1}\",headers=\"{2}\",signature=\"{3}\"",
                spec.KeyId,
                spec.Algorithm,
                string.Join(" ", spec.Headers),
                signature);
	    }

	    public VerifiedSignature Signature(HttpRequest r, ISignatureSpecification spec, IKeyStore keyStore)
	    {
	        return Signature(Request.FromHttpRequest(r), spec, keyStore);
	    }
	}
	
}
