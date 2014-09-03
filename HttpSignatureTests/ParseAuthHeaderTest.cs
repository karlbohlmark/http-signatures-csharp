using System;
using HttpSignatures;
using NUnit.Framework;

namespace HttpSignatureTests
{
	[TestFixture ()]
	public class ParseAuthHeaderTest
	{
		public ParseAuthHeaderTest (){}

		[Test]
		public void TestParseAuthHeader ()
		{
			var authParser = new AuthorizationParser ();
			var authHeader = "Signature keyId=\"some-key\",algorithm=\"hmac-sha256\",headers=\"date,(request-target)\",signature=\"9MmDJ/7WkGLsBvq9g3/TNhvXgFm5n11j0XqjvF2z9Rc=\"";
			var auth = authParser.Parse (authHeader);
			Assert.IsTrue (auth.Algorithm == "hmac-sha256");
			Assert.AreEqual (auth.KeyId, "some-key");
		}
	}
}

