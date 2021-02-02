using Figgle;
using Ploch.Common.ConsoleApplication.Core;

namespace Ploch.Common.ConsoleApplication.ExtendedOutput
{
    public class DefaultBannerCreator : IBannerCreator
    {
        private readonly IOutput _output;
        private readonly IAppInfoProvider _appInfoProvider;

        public DefaultBannerCreator(IOutput output, IAppInfoProvider appInfoProvider)
        {
            _output = output;
            _appInfoProvider = appInfoProvider;
        }

        /// <inheritdoc />
        public void ShowStartupBanner()
        {
            var appInfo = _appInfoProvider.GetAppInfo();
            _output.WriteLine(FiggleFonts.Standard.Render(appInfo.AppTitle))
                   .WriteLine($"Version: {appInfo.Version}")
                   .WriteLine();
            if (!appInfo.AppDescription.IsNullOrEmpty())
            {
                _output.WriteLine(appInfo.AppDescription);
            }
            if (!appInfo.Copyright.IsNullOrEmpty())
            {
                _output.Write(appInfo.Copyright).Write(" ");
            }
            if (!appInfo.Author.IsNullOrEmpty())
            {
                _output.Write(appInfo.Author);
            }
        }
    }
}