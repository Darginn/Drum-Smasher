using System;
using System.Threading.Tasks;
using System.IO;

namespace DSLauncher
{
    class Program
    {
        static void Main(string[] args)
            => MainTask(args).ConfigureAwait(false).GetAwaiter().GetResult();

        private static async Task MainTask(string[] args)
        {
            try
            {
                using(UI.MainWindow mw = UI.MainWindow.Default)
                {
                    mw.Run();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            await Task.Delay(-1);
        }
    }
}
