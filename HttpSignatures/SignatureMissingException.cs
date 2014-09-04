using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpSignatures
{
    public class SignatureMissingException : InvalidSignatureException
    {
        public SignatureMissingException(string message)
            : base(message)
        {

        }
    }
}
