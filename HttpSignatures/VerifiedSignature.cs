﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpSignatures
{
    public class VerifiedSignature : ISignatureAuthorization
    {
        internal VerifiedSignature(ISignatureAuthorization signatureAuth, bool valid)
        {
            Valid = valid;
            KeyId = signatureAuth.KeyId;
            Algorithm = signatureAuth.Algorithm;
            Signature = signatureAuth.Signature;
            Headers = new List<string>(signatureAuth.Headers);
        }

        public string KeyId { get; private set; }
        public IEnumerable<string> Headers { get; private set; }
        public string Algorithm { get; private set; }
        public string HashAlgorithm { get; private set; }
        public string Signature { get; private set; }
        public bool Valid { get; private set; }
    }
}