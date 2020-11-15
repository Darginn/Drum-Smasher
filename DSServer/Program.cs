using System;
using System.Threading.Tasks;

namespace DSServer
{
    class Program
    {
        static void Main(string[] args)
            => MainTask(args).ConfigureAwait(false).GetAwaiter().GetResult();

        static async Task MainTask(string[] args)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            await Task.Delay(-1).ConfigureAwait(false);
        }
    }
}
