// See https://aka.ms/new-console-template for more information
namespace SQLQuerySearchOptimization
{
    class Program
    {
        static  void Main(string[] args)
        {
            try
            {

                SqlSerachOptimization sqlSerach = new SqlSerachOptimization();
                sqlSerach.TestRun();

                Console.ReadKey();

            }
            catch (Exception e)
            {
                Console.WriteLine("{e.ToString()}" + e.ToString());

            }

        }

    }
}
