using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection.Metadata.Ecma335;

namespace RazorPagesHandlers.Pages
{
    public class SearchModel : PageModel
    {
        public void OnGet()
        {
        }

        public Task OnPostAsync()
        {
            return Task.CompletedTask;
        }

        public void OnPostCustomSearch()
        {

        }
    }
}
