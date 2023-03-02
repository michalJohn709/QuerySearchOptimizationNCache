// See https://aka.ms/new-console-template for more information
namespace json_api
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                BasicOperation.Run();
                Console.ReadKey();

            }
            catch (Exception e)
            {
                Console.WriteLine("{e.ToString()}" + e.ToString());

            }

        }

    }
}
