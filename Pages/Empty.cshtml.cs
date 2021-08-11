using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CertTester.Pages
{
    public class EmptyModel : PageModel
    {
        public void OnGet()
        {
            string emptyUri = "https://" + HttpContext.Request.Host.Value + "/Empty";
            ViewData["TestURI"] = emptyUri;
        }
    }
}
