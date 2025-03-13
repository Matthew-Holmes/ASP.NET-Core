
using System.ComponentModel.Design;

namespace FirstRazorWebApp.Pages
{
    public class ToDoService
    {
        internal List<ToDoListModel> GetItemsForCategory(string category)
        {
            if (category == "widgets")
            {
                return new List<ToDoListModel>
                {
                    new ToDoListModel("widget1", 10),
                    new ToDoListModel("widget2", 3),
                };
           } else
            {
                return new List<ToDoListModel>();
            }
        }
    }
}