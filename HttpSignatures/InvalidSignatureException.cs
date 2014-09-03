using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace HttpSignatures
{

	public class InvalidSignatureException : Exception {
		public InvalidSignatureException(string message) : base(message) {

		}
	}
	
}
