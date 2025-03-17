using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesURLRouting.Pages.Currency
{
    public class ViewModel : PageModel
    {
        public string Code { get; set; }

        public decimal Rate { get; set; } = 1.28m;

        public void OnGet(string code)
        {
            Code = code;
        }
    }
}
