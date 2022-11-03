namespace Smartstore.Web.Models.System
{
    public class StartupModel : ApiModel
    {
        public string DefaultTitle { get; set; }
        public string RouteTitle { get; set; }
        public bool IsRegistered { get; set; }
        public bool IsAdmin { get; set; }
    }
}
