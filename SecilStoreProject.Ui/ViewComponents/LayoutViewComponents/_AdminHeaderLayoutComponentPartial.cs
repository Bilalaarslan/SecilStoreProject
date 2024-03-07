using Microsoft.AspNetCore.Mvc;

namespace SecilStoreProject.Ui.ViewComponents.LayoutViewComponents
{
    public class _AdminHeaderLayoutComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
