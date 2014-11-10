using System.Web;

namespace HttpSignatures
{
    public class SignatureVerificationModule: IHttpModule
    {
        private readonly ISignatureAuthenticator _authenticator;

        public SignatureVerificationModule(ISignatureAuthenticator authenticator)
        {
            _authenticator = authenticator;
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += context_BeginRequest;
        }

        void context_BeginRequest(object sender, System.EventArgs e)
        {
            var context = HttpContext.Current;
            if (!_authenticator.IsExempt(context.Request))
            {
                var verifiedSignature = _authenticator.Authenticate(context.Request, context.Response);
            }
        }

        public void Dispose()
        {
        }
    }
}
