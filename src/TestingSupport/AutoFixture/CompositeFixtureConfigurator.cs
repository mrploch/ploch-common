using AutoFixture;

namespace Ploch.TestingSupport.AutoFixture
{
    public class CompositeFixtureConfigurator : IFixtureConfigurator
    {
        private readonly IFixtureConfigurator[] _fixtureConfigurators;

        public CompositeFixtureConfigurator(params IFixtureConfigurator[] fixtureConfigurators)
        {
            _fixtureConfigurators = fixtureConfigurators;
        }

        public void Configure(IFixture fixture)
        {
            foreach (var fixtureConfigurator in _fixtureConfigurators)
            {
                fixtureConfigurator.Configure(fixture);
            }
        }
    }
}