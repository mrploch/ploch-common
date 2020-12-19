using AutoFixture;

namespace Ploch.TestingSupport.AutoFixture
{
    public interface IFixtureConfigurator
    {
        void Configure(IFixture fixture);
    }
}