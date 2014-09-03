using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace HttpSignatures
{

	public interface IKeyStore {
		string Get (string keyId);
	}

	public class KeyStore : IKeyStore {
		public string Get (string KeyId) {
			return "MzJhMDYxN2FhYjRjOWZlNzI1ZjFiNWJjNDQxMjkxMTgwYWQyNWI3MyAgLQo=";
		}
	}
	
}
