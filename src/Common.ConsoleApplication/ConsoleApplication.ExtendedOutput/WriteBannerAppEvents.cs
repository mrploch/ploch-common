using System;
using Ploch.Common.ConsoleApplication.Core;

namespace Ploch.Common.ConsoleApplication.ExtendedOutput
{
    public class WriteBannerAppEvents : IAppEvents
    {
        private readonly IBannerCreator _bannerCreator;

        public WriteBannerAppEvents(IBannerCreator bannerCreator)
        {
            _bannerCreator = bannerCreator;
        }

        public int Order { get; } = -1;

        /// <inheritdoc />
        public void OnStartup(IServiceProvider serviceProvider)
        {
            _bannerCreator.ShowStartupBanner();
        }
        

        /// <inheritdoc />
        public void OnShutdown(IServiceProvider serviceProvider)
        {
            // Method intentionally left empty.
        }
    }
}