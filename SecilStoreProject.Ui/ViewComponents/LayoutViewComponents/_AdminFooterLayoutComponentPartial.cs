using Microsoft.AspNetCore.Mvc;

namespace SecilStoreProject.Ui.ViewComponents.LayoutViewComponents
{
    public class _AdminFooterLayoutComponentPartial :ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
