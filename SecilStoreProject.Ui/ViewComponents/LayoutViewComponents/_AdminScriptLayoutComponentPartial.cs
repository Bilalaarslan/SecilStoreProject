using Microsoft.AspNetCore.Mvc;

namespace SecilStoreProject.Ui.ViewComponents.LayoutViewComponents
{
    public class _AdminScriptLayoutComponentPartial :ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
