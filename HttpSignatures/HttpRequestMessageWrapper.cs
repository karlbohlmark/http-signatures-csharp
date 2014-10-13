using System;
using System.Linq;
using System.Net.Http;

namespace HttpSignatures
{
    public class HttpRequestMessageWrapper: IRequest
    {
        private readonly HttpRequestMessage _r;

        public HttpRequestMessageWrapper(HttpRequestMessage r)
        {
            _r = r;
        }

        public HttpMethod Method {
            get { return _r.Method; }
        }
        public string Body { get; private set; }
        public string GetHeader(string header)
        {
            var h = _r.Headers.GetValues(header).ToArray();
            if (h.Count() > 1)
            {
                throw new NotImplementedException("Signing request with duplicate headers is not implemented.");
            }
            return h.FirstOrDefault();
        }

        public void SetHeader(string header, string value)
        {
            _r.Headers.Add(header, value);
        }

        public string Path { get; private set; }
    }
}