using Dawn;

namespace Ploch.Common.ConsoleApplication.Core
{
    public abstract class AppCommand<TOptions> : ICommand<TOptions>
    {
        public abstract void Execute(TOptions options);

        void ICommand.Execute(object options)
        {
            Guard.Argument(options).Compatible<TOptions>();
            Execute((TOptions) options);
        }
    }
}