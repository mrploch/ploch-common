using AutoFixture;
using AutoFixture.AutoMoq;


namespace Ploch.TestingSupport.AutoFixture
{
    /// <summary>
    /// Fixture configuration with AutoMock customization.
    /// Fixture factory using <see cref="DefaultFixtureConfigurator">Default Fixture Configurator</see>
    /// </summary>
    public static class FixtureConfiguratorFactory
    {
        /// <summary>
        ///     Automatics the moq configured default.
        /// </summary>
        /// <returns></returns>
        public static IFixtureConfigurator AutoMoqConfiguredDefault()
        {
            return new DefaultFixtureConfigurator();
        }
    }

    /// <summary>
    ///     Default Fixture Configurator
    /// </summary>
    /// <seealso cref="Ploch.TestingSupport.AutoFixture.IFixtureConfigurator" />
    public class DefaultFixtureConfigurator : IFixtureConfigurator
    {
        private readonly ICustomization[] _customizations;

        public DefaultFixtureConfigurator() : this(new AutoMoqCustomization { ConfigureMembers = true })
        { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultFixtureConfigurator" /> class.
        /// </summary>
        /// <param name="customizations">The customizations.</param>
        public DefaultFixtureConfigurator(params ICustomization[] customizations)
        {
            _customizations = customizations;
        }

        public virtual void Configure(IFixture fixture)
        {
            foreach (var customization in _customizations) fixture.Customize(customization);
            //fixture.Customize(new AutoMoqCustomization());            
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }
    }

    public class DefaultFixtureConfigurator2 : IFixtureConfigurator
    {
        public void Configure(IFixture fixture)
        {
            fixture.Customize(new AutoMoqCustomization());
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }
    }
}