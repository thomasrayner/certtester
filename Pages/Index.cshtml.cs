﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

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
            string targetUri = string.Empty;

            if (HttpContext.Request.Query.ContainsKey("endpoint"))
            {
                HttpContext.Request.Query.TryGetValue("endpoint", out var userProvidedEndpoint);
                string userProvidedString = userProvidedEndpoint.ToString();
                
                if (!Regex.Match(userProvidedString, @"^https\:\/\/").Success)
                {
                    targetUri = "https://";
                }

                targetUri += userProvidedString;
            }
            else
            {
                string host = Regex.Match(HttpContext.Request.Host.Value, "localhost").Success ?
                HttpContext.Connection.LocalIpAddress.MapToIPv4() + ":" + HttpContext.Connection.LocalPort :
                HttpContext.Request.Host.Value;

                targetUri = "https://" + host + "/Empty";
            }
            
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
            httpClient.Send(new HttpRequestMessage(HttpMethod.Get, targetUri));

            ViewData["TestURI"] = targetUri;
            ViewData["Issuer"] = serverCert.Issuer;
            ViewData["SubjectName"] = serverCert.SubjectName.Name;
            ViewData["Thumbprint"] = serverCert.Thumbprint;
            ViewData["Expiration"] = serverCert.NotAfter.ToString();
            ViewData["SerialNumber"] = serverCert.SerialNumber;
            ViewData["SignatureAlgo"] = serverCert.SignatureAlgorithm.FriendlyName;
            ViewData["ValidFrom"] = serverCert.NotBefore.ToString();

            string allDataFormatted = "<ul>";
            foreach (var line in serverCert.ToString(true).Split('['))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                allDataFormatted += "<li>" + line.Replace("]", ":") + "</li>";
            }
            allDataFormatted += "</ul>";

            ViewData["AllData"] = allDataFormatted;
         }
    }
}
