using Smartstore.ComponentModel;
using Smartstore.Core.Configuration;
using Smartstore.Core.Stores;

namespace Smartstore.Web.Models.Pdf
{
    internal class PdfReceiptMapper : Mapper<Store, PdfReceiptSectionModel>
    {
        private readonly ISettingFactory _settingFactory;

        public PdfReceiptMapper(ISettingFactory settingFactory)
        {
            _settingFactory = settingFactory;
        }

        protected override void Map(Store from, PdfReceiptSectionModel to, dynamic parameters = null)
            => throw new NotImplementedException();

        public override async Task MapAsync(Store from, PdfReceiptSectionModel to, dynamic parameters = null)
        {
            Guard.NotNull(from, nameof(from));
            Guard.NotNull(to, nameof(to));

            to.StoreName = from.Name;
            to.StoreUrl = from.Url;
            to.LogoId = from.LogoMediaFileId;
        }
    }
}
