using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace HttpSignatures
{

	public interface IAuthorizationParser {
		ISignatureAuthorization Parse(string authorizationHeader);
	}

	public class AuthorizationParser: IAuthorizationParser {

		private List<string> Algorithms = new List<string>(){
			"hmac-sha1",
			"hmac-sha256",
			"hmac-sha512"
		};

		public ISignatureAuthorization Parse (string authorizationHeader)
		{
			var authz = authorizationHeader;
			var state = State.New;
			var substate = ParamsState.Name;
			var parsed = new ParsedAuthorization ();
			var tmpName = "";
			var tmpValue = "";
			for (var i = 0; i < authz.Length; i++) {
				var c = authz[i];

				switch (state) {

				case State.New:
					if (c != ' ') parsed.Scheme += c;
					else state = State.Params;
					break;

				case State.Params:
					switch (substate) {

					case ParamsState.Name:
						var code = (int) c;
						// restricted name of A-Z / a-z
						if ((code >= 0x41 && code <= 0x5a) || // A-Z
							(code >= 0x61 && code <= 0x7a)) { // a-z
							tmpName += c;
						} else if (c == '=') {
							if (tmpName.Length == 0)
								throw new InvalidHeaderException("bad param format");
							substate = ParamsState.Quote;
						} else {
							throw new InvalidHeaderException("bad param format");
						}
						break;

					case ParamsState.Quote:
						if (c == '"') {
							tmpValue = "";
							substate = ParamsState.Value;
						} else {
							throw new InvalidHeaderException("bad param format");
						}
						break;

					case ParamsState.Value:
						if (c == '"') {
							parsed.Params[tmpName] = tmpValue;
							substate = ParamsState.Comma;
						} else {
							tmpValue += c;
						}
						break;

					case ParamsState.Comma:
						if (c == ',') {
							tmpName = "";
							substate = ParamsState.Name;
						} else {
							throw new InvalidHeaderException("bad param format");
						}
						break;

					default:
						throw new Exception("Invalid substate");
					}
					break;

				default:
					throw new Exception("Invalid substate");
				}

			}

			if (string.IsNullOrEmpty(parsed.Params["headers"]) || parsed.Params["headers"] == "") {
//				if (request.headers['x-date']) {
//					parsed.params.headers = ['x-date'];
//				} else {
//					parsed.params.headers = ['date'];
//				}
				parsed.Headers = new string[]{"date"};
			} else {
				parsed.Headers = parsed.Params["headers"].Split(' ');
			}

			// Minimally validate the parsed object
			if (string.IsNullOrEmpty(parsed.Scheme) || parsed.Scheme != "Signature")
				throw new InvalidHeaderException("scheme was not \"Signature\"");

			if (string.IsNullOrEmpty(parsed.Params["keyId"]))
				throw new InvalidHeaderException("keyId was not specified");

			if (string.IsNullOrEmpty(parsed.Params["algorithm"]))
				throw new InvalidHeaderException("algorithm was not specified");

			if (string.IsNullOrEmpty(parsed.Params["signature"]))
				throw new InvalidHeaderException("signature was not specified");

			parsed.Params["algorithm"] = parsed.Params["algorithm"].ToLower();
			if (!this.Algorithms.Contains (parsed.Params ["algorithm"])) {
				throw new InvalidParamsException(parsed.Params["algorithm"] + " is not supported");
			}

			return new SignatureAuthorization (parsed);
		}

	}

	public class ParsedAuthorization {
		public ParsedAuthorization() {
			Scheme = "";
			Params = new Dictionary<string, string> ();
		}
		public string Scheme {get;set;}
		public Dictionary<string, string> Params {get; set;}
		public IEnumerable<string> Headers {get;set;}
	}

	public class InvalidHeaderException : InvalidSignatureException {
		public InvalidHeaderException(string message) : base(message) {}
	}

	public class InvalidParamsException : InvalidSignatureException {
		public InvalidParamsException(string message) : base(message) {}
	}


	public enum State {
		New = 0,
		Params = 1
	};

	public enum ParamsState {
		Name = 0,
		Quote = 1,
		Value = 2,
		Comma = 3
	}

	public interface ISignatureAuthorization: ISignatureSpecification {
		string Signature { get; }
	}

	public class SignatureAuthorization: ISignatureAuthorization {
		private ParsedAuthorization authorization;
		public SignatureAuthorization(ParsedAuthorization auth) {
			this.authorization = auth;
		}
		public string Signature {
			get {
				return authorization.Params ["signature"];
			}
		}

		public string KeyId {
			get {
				return authorization.Params ["keyId"];
			}
		}

		public IEnumerable<string> Headers {
			get {
				return authorization.Headers;
			}
		}

		public string Algorithm {
			get {
				return authorization.Params ["algorithm"];
			}
		}

		public string HashAlgorithm {
			get {
				return Algorithm.Split('-')[1];
			}
		}
	}
}
