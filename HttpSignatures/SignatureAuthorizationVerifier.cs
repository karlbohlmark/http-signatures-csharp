using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace HttpSignatures
{
	public interface ISignatureAuthorizationVerifier {
		bool Verify(ISignatureAuthorization signatureAuthorization);
	}

	public class SignatureAuthorizationVerifier: ISignatureAuthorizationVerifier {
		public bool Verify (ISignatureAuthorization signatureAuthorization)
		{
			throw new NotImplementedException ();
		}
	}
}
