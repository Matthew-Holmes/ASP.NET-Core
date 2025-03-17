using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesURLRouting.Pages.Currency
{
    public class ViewModel : PageModel
    {
        private readonly LinkGenerator _link;

        public ViewModel(LinkGenerator link)
        {
            _link = link;
        }


        public string Code { get; set; }

        public String URL1 { get; set; }
        public String URL2 { get; set; }
        public String URL3 { get; set; }
        public String URL4 { get; set; }

        public decimal Rate { get; set; } = 1.28m;

        public void OnGet(string code)
        {
            Code = code;

            String? url1 = Url.Page("/Currency/View", new { Code = "USD" }); 
            String? url2 = _link.GetPathByPage(
                                "/Currency/View",
                                values: new { Code = "USD" });
            String? url3 = _link.GetPathByPage(
                                HttpContext,
                                "/Currency/View",
                                values: new { Code = "USD" });
            String? url4 = _link.GetUriByPage(
                                page: "/Currency/View",
                                handler: null,
                                values: new { Code = "USD" },
                                scheme: "https",
                                host: new HostString("example.com"));

            URL1 = url1; URL2 = url2; URL3 = url3; URL4 = url4;
        }
    }
}
