using AutoFixture;

namespace Ploch.TestingSupport.AutoFixture
{
    public static class FixtureFactory
    {
        private static readonly IFixtureConfigurator DefaultConfigurator =
            new CompositeFixtureConfigurator(new DefaultFixtureConfigurator(), new AutoMoqFixtureConfigurator());

        public static IFixtureConfigurator Configurator { get; set; } = DefaultConfigurator;

        public static void SetConfigurator(IFixtureConfigurator configurator)
        {
            Configurator = configurator;
        }

        public static void ResetConfigurator()
        {
            Configurator = DefaultConfigurator;
        }

        public static IFixture CreateFixture()
        {
            var fixture = new Fixture();

            Configurator.Configure(fixture);

            return fixture;
        }
    }
}