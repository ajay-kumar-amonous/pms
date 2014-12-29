using System.Web.Mvc;

namespace pEasyPrint.Areas.ContentWritter
{
    public class ContentWritterAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ContentWritter";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ContentWritter_default",
                "ContentWritter/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
