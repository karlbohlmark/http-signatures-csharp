using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace HttpSignatures
{

	public interface IHttpSignatureStringExtractor {
		string ExtractSignatureString(HttpRequest request, ISignatureSpecification spec);
	}

	public class HttpSignatureStringExtractor: IHttpSignatureStringExtractor {
		public string ExtractSignatureString (HttpRequest request, ISignatureSpecification signatureAuth)
		{
			var headerStrings = (from h in signatureAuth.Headers
				select string.Format("{0}: {1}", h, GetHeaderValue (h, request))).ToList();
			return string.Join("\n", headerStrings);
		}

		private string GetHeaderValue(string header, HttpRequest request) {
			switch (header) {
				case "(request-target)":
					return request.HttpMethod.ToLower () + " " + request.RawUrl;
				default:
					return request.Headers.Get (header);
			}
		}
	}
	
}
