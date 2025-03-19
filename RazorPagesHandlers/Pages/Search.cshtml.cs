using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection.Metadata.Ecma335;

namespace RazorPagesHandlers.Pages
{
    public class SearchModel : PageModel
    {
        private readonly SearchService _searchService;

        public SearchModel(SearchService searchService)
        {
            _searchService = searchService;
        }

        [BindProperty] // declares a property to be model bound
        public BindingModel Input { get; set; }

        public List<Product> Results { get; set; }



        public void OnGet()
        { 
            // doesn't require model binding, so just do the default and render the view
        }

        public IActionResult OnPost(int max)
        {
            if (ModelState.IsValid)
            {
                Results = _searchService.Search(Input.SearchTerms, max);
                return Page();
            }
            return RedirectToPage("./Index");
        }



        //public Task OnPostAsync()
        //{
        //    return Task.CompletedTask;
        //}

        //public void OnPostCustomSearch()
        //{

        //}
    }

    public class BindingModel
    {
        public List<String> SearchTerms { get; set; }
    }
}
