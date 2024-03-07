using Microsoft.AspNetCore.Mvc;

namespace SecilStoreProject.Ui.ViewComponents.LayoutViewComponents
{
    public class _AdminSidebarLayoutComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
