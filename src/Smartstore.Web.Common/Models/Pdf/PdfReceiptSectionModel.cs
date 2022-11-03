using Smartstore.Web.Modelling;

namespace Smartstore.Web.Models.Pdf
{
    public partial class PdfReceiptSectionModel : ModelBase
    {
        public string StoreName { get; set; }
        public string StoreUrl { get; set; }
        public int LogoId { get; set; }
    }
}
