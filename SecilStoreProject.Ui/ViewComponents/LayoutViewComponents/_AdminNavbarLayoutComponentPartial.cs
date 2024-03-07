using Microsoft.AspNetCore.Mvc;

namespace SecilStoreProject.Ui.ViewComponents.LayoutViewComponents
{
    public class _AdminNavbarLayoutComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
