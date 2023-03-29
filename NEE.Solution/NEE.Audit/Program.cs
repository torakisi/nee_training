using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NEE.Audit
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("NEE Clearance process");
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) =>
            {
                e.Cancel = true;
                cancellationTokenSource.Cancel();
            };

            try
            {
                //await ProcessClearance(cancellationToken);
                await RunTestAuditProcess();
                Console.WriteLine("FINISHED");
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Cancelled");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("Press [Enter] to exit");
            Console.ReadLine();

        }
        private static async Task RunTestAuditProcess()
        {
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            Console.WriteLine();
            Console.WriteLine($"Welcome {userName} to nee clearance process.");
            Console.WriteLine("This is a safe service and you may run it as many times as you want with any parameters.");
            Console.WriteLine("Running it more than once cannot result in the submit of wrong payments.");
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
