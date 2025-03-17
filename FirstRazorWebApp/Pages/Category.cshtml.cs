using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FirstRazorWebApp.Pages
{
    public class CategoryModel : PageModel
    {
        private readonly ToDoService _service;

        public CategoryModel(ToDoService service)
        {
            _service = service;
        }

        public ActionResult OnGet(string category)
        {
            Items = _service.GetItemsForCategory(category);
            // return a `PageResult` indicates the Razor view should be rendered
            return Page();
        }


        public List<ToDoListModel> Items { get; set; }

    }

    public record ToDoListModel(String Name, int quantity);

}
