using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesModelBinding.Pages
{
    public class ConvertModel : PageModel
    {
        public void OnGet(
            string currencyIn,
            string currencyOut,
            int qty)
        {
            decimal converted = qty * 1.1m;
            ConvertedAmount = converted.ToString() + " " + currencyOut;
        }

        public string ConvertedAmount { get; set; }
    }
}
