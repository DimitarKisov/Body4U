namespace Body4U.Web.Areas.Administration.Controllers
{
    using Body4U.Web.ViewModels.Administration.Dashboard;

    using Microsoft.AspNetCore.Mvc;

    public class DashboardController : AdministrationController
    {
        public DashboardController()
        {
        }

        public IActionResult Index()
        {
            return this.View();
        }
    }
}
