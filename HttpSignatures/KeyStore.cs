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
		Dictionary<string, string> keys;

		public KeyStore(Dictionary<string,string> keys){
			this.keys = keys;
		}

		public string Get (string KeyId) {
			return this.keys [KeyId];
		}
	}
	
}
