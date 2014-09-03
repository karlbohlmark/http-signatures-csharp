using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace HttpSignatures
{

	public interface ISignatureSpecification {
		string KeyId { get; }
		IEnumerable<string> Headers { get; }
		string Algorithm { get; }
		string HashAlgorithm { get; }
	}

	public class SignatureSpecification: ISignatureSpecification {
		public string KeyId {
			get;
			set;
		}

		public IEnumerable<string> Headers {
			get;
			set;
		}

		public string Algorithm {get;set;}

		public string HashAlgorithm {
			get {
				return Algorithm.Split('-')[1];
			}
		}
	}
	
}
