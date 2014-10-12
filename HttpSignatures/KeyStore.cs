using System.Collections.Generic;

namespace HttpSignatures
{

	public interface IKeyStore {
		string Get (string keyId);
	}

	public class KeyStore : IKeyStore {
	    readonly Dictionary<string, string> keys;

		public KeyStore(Dictionary<string,string> keys){
			this.keys = keys;
		}

		public string Get (string KeyId) {
			return this.keys [KeyId];
		}
	}
	
}
