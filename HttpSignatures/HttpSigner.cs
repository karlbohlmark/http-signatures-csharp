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
		string Signature(HttpRequestBase r, ISignatureSpecification spec);
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

		public string Signature (HttpRequestBase r, ISignatureSpecification spec)
		{
			var authorization = r.Headers.Get("Authorization");
			var signatureAuth = authorizationParser.Parse(authorization);
			if (spec == null) {
				spec = signatureAuth;
			} else {
				if (spec.Algorithm != signatureAuth.Algorithm) {
					throw new InvalidSignatureException(string.Format("Algorith mismatch. Wanted: {0}, found: {1}", spec.Algorithm, signatureAuth.Algorithm));
				}
				var missingHeaders = spec.Headers.Where(h=> !signatureAuth.Headers.Contains(h));
				if (missingHeaders.Any()) {
					throw new InvalidSignatureException(string.Format("Missing headers in signature: {0}", string.Join(",", missingHeaders)));
				}
			}
			var signatureString = signatureStringExtractor.ExtractSignatureString (r, spec);

			var hmac = HMACSHA256.Create (signatureAuth.Algorithm.Replace("-", "").ToUpper());
			hmac.Initialize ();
			hmac.Key = Convert.FromBase64String(keyStore.Get (signatureAuth.KeyId));
			var bytes = hmac.ComputeHash (new MemoryStream(Encoding.UTF8.GetBytes(signatureString)));
			var signature = Convert.ToBase64String (bytes);
			return signature;
		}
	}
	
}
