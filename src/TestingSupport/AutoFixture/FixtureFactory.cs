using AutoFixture;

namespace Ploch.TestingSupport.AutoFixture
{
    public static class FixtureFactory
    {
        private static IFixtureConfigurator _configurator = FixtureConfiguratorFactory.AutoMoqConfiguredDefault();

        public static void SetConfigurator(IFixtureConfigurator configurator)
        {
            _configurator = configurator;
        }

        public static void ResetConfigurator()
        {
            _configurator = FixtureConfiguratorFactory.AutoMoqConfiguredDefault();
        }

        public static IFixture CreateFixture()
        {
            var fixture = new Fixture();

            _configurator.Configure(fixture);
            return fixture;
        }
    }
}