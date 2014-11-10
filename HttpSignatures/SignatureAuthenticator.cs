using System;
using System.Web;
using Common.Logging;

namespace HttpSignatures
{
    public interface ISignatureAuthenticator
    {
        bool IsExempt(HttpRequest request);
        VerifiedSignature Authenticate(HttpRequest request, HttpResponse response);
    }

    public class SignatureAuthenticator : ISignatureAuthenticator
    {
        protected readonly ISignatureSpecification _signatureSpec;
        protected readonly IKeyStore _keyStore;
        protected readonly ILog _log;

        public SignatureAuthenticator(ISignatureSpecification signatureSpec, IKeyStore keyStore)
        {
            _signatureSpec = signatureSpec;
            _keyStore = keyStore;
            _log = LogManager.GetCurrentClassLogger();
        }

        public virtual bool IsExempt(HttpRequest request)
        {
            return false;
        }

        public virtual void OnAuthenticateSuccess(VerifiedSignature signature)
        {
            return;
        }

        public virtual void OnAuthenticateFailure(VerifiedSignature signature)
        {
            return;
        }

        public virtual VerifiedSignature Authenticate(HttpRequest request, HttpResponse response)
        {
            VerifiedSignature verifiedSignature = null;
            try
            {
                verifiedSignature = HttpSignature.VerifiedSignature(HttpContext.Current.Request, _signatureSpec,
                    _keyStore);
                if (verifiedSignature !=null && verifiedSignature.Valid)
                {
                    this.OnAuthenticateSuccess(verifiedSignature);
                }
            }
            catch (InvalidSignatureException ex)
            {
                _log.Error(string.Format("Invalid signature: {0}, {1}", ex.Message, ex.ToString()));
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }

            if (verifiedSignature == null || !verifiedSignature.Valid)
            {
                HttpContext.Current.Response.StatusCode = 401;
                HttpContext.Current.Response.AddHeader("WWW-Authenticate", _signatureSpec.WwwAuthenticateChallenge());
                HttpContext.Current.Response.Write("Signature authentication required.");
                HttpContext.Current.Response.End();
                this.OnAuthenticateFailure(verifiedSignature);
            }
            return verifiedSignature;
        }
    }

    public static class SignatureSpecificationExtensions
    {
	    public static string WwwAuthenticateChallenge(this ISignatureSpecification spec)
	    {
	        return string.Format("Signature realm=\"{0}\",headers=\"{1}\"", spec.Realm, string.Join(" ", spec.Headers));
	    }
    }
}