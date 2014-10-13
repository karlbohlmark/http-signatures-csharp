using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace HttpSignatures
{
    public interface IRequest
    {
        HttpMethod Method { get; }
        string Body { get; }
        string GetHeader(string header);
        void SetHeader(string header, string value);
        string Path { get; }
    }
}
