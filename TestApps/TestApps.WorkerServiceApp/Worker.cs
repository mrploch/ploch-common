using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TracerAttributes;

namespace Ploch.TestApps.WorkerServiceApp
{
    public class Worker : IHostedService
    {
        private readonly ILogger<Worker> _logger;
        
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        // protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        // {
        //     while (!stoppingToken.IsCancellationRequested)
        //     {
        //         _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //         await Task.Delay(1000, stoppingToken);
        //     }
        // }


        /// <exception cref="T:System.Threading.Tasks.TaskCanceledException">The task has been canceled.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="delay" /> represents a negative time interval other than <see langword="TimeSpan.FromMilliseconds(-1)" />.  
        ///  -or-  
        ///  The <paramref name="delay" /> argument's <see cref="P:System.TimeSpan.TotalMilliseconds" /> property is greater than <see cref="F:System.Int32.MaxValue" />.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The provided <paramref name="cancellationToken" /> has already been disposed.</exception>
        /// <exception cref="T:System.OverflowException"><paramref name="value" /> is less than <see cref="F:System.TimeSpan.MinValue" /> or greater than <see cref="F:System.TimeSpan.MaxValue" />.  
        ///  -or-  
        ///  <paramref name="value" /> is <see cref="F:System.Double.PositiveInfinity" />.  
        ///  -or-  
        ///  <paramref name="value" /> is <see cref="F:System.Double.NegativeInfinity" />.</exception>
        ///
        [Interceptor]
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => Console.WriteLine("Cancellation requested..."));

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(20), cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException e)
                {
                    Console.WriteLine("Operation cancelled!");
                    break;
                }

            }
        }

        /// <exception cref="T:System.Threading.Tasks.TaskCanceledException">The task has been canceled.</exception>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"Shutting down... Cancellation requested? {cancellationToken.IsCancellationRequested}");
            return Task.Delay(5, cancellationToken);
        }
    }
}
