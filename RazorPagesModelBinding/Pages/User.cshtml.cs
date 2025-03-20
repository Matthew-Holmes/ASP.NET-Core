using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesModelBinding.Pages
{
    public class UserModel : PageModel
    {

        [BindProperty(SupportsGet = true)]
        public UserBindingModel Input { get; set; }

        public void OnGet()
        {
            Splash = "hello " + Input.FirstName + " " + Input.LastName + "!";
            Contact = Input.Email;
        }

        public string Splash { get; set; }
        public string Contact { get; set; }
    }


    public class UserBindingModel 
    {
        public UserBindingModel() { }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

}
