using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace HttpSignatures
{

	public interface IHttpSigner {
		VerifiedSignature Signature(HttpRequest r, ISignatureSpecification spec, IKeyStore keyStore);
	}

	public class HttpSigner : IHttpSigner
	{
		IAuthorizationParser authorizationParser;

		IHttpSignatureStringExtractor signatureStringExtractor;

		IKeyStore keyStore;

		public HttpSigner (IAuthorizationParser authorizationParser, IHttpSignatureStringExtractor signatureStringExtractor, IKeyStore keyStore)
		{
			this.keyStore = keyStore;
			this.signatureStringExtractor = signatureStringExtractor;
			this.authorizationParser = authorizationParser;
		}


        public HttpSigner(IAuthorizationParser authorizationParser, IHttpSignatureStringExtractor signatureStringExtractor) : this(authorizationParser, signatureStringExtractor, null)
        {}

		public VerifiedSignature Signature (HttpRequest r, ISignatureSpecification spec, IKeyStore keyStore)
		{
			var authorization = r.Headers.Get("Authorization");
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

			var signatureString = signatureStringExtractor.ExtractSignatureString (r, spec);
			var hmac = HMAC.Create (signatureAuth.Algorithm.Replace("-", "").ToUpper());
			hmac.Initialize ();
			hmac.Key = Convert.FromBase64String(keyStore.Get (signatureAuth.KeyId));
			var bytes = hmac.ComputeHash (new MemoryStream(Encoding.UTF8.GetBytes(signatureString)));
			var signature = Convert.ToBase64String (bytes);
            return new VerifiedSignature(signatureAuth, signature == signatureAuth.Signature);
		}
	}
	
}
