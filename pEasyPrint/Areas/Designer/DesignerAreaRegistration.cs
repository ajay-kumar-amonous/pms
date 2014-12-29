using System.Web.Mvc;

namespace pEasyPrint.Areas.Designer
{
    public class DesignerAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Designer";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Designer_default",
                "Designer/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
