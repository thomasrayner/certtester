using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace CertTester.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            string emptyUri = "https://" + HttpContext.Request.Host.ToUriComponent() + "/Empty";
            X509Certificate2 serverCert = null;
            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, cert, __, ___) =>
                {
                    serverCert = new X509Certificate2(cert.GetRawCertData());
                    return true;
                }
            };
            var httpClient = new HttpClient(httpClientHandler);
            httpClient.Send(new HttpRequestMessage(HttpMethod.Get, emptyUri));

            ViewData["Protocol"] = HttpContext.Request.IsHttps ? "HTTPS" : "HTTP";
            ViewData["Issuer"] = serverCert.Issuer;
            ViewData["SubjectName"] = serverCert.SubjectName.Name;
            ViewData["Serial"] = serverCert.GetSerialNumberString();
            ViewData["Thumbprint"] = serverCert.Thumbprint;
            ViewData["Expiration"] = serverCert.NotAfter.ToString();
        }
    }
}
