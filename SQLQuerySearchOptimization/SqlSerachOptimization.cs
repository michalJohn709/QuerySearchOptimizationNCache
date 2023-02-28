using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Exceptions;
using SampleData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLQuerySearchOptimization
{
    public class SqlSerachOptimization
    {
        private static ICache _cache;

        public static void Run()
        {
            //Initialize Cache.
            InitializeCache();


            //Start StopWatch with Only Keys 1st attempt
            //1st Run
            var timer = new Stopwatch();
            timer.Start();

            //Perform Operation
            GetKeysOnly();

            timer.Stop();
            TimeSpan timeTaken = timer.Elapsed;
            string timeFinal = "Time taken With Only Keys Fetch 1st attempt: " + timeTaken.ToString();
            Console.WriteLine(timeFinal + "\n");




            //Start StopWatch with Only Keys 2nd attempt
            //2nd Run
            var timer2 = new Stopwatch();
            timer.Start();

            //Perform Operation
            GetKeysOnly();

            timer.Stop();
            TimeSpan timeTaken2 = timer2.Elapsed;
            string timeFinal2 = "Time taken With Only Keys Fetch 2nd attempt:" + timeTaken2.ToString();
            Console.WriteLine(timeFinal2 + "\n");





            //Start StopWatch with Only Keys 3rd attempt
            //3rd Run
            var timer3 = new Stopwatch();
            timer.Start();

            //Perform Operation
            GetKeysOnly();

            timer.Stop();
            TimeSpan timeTaken3 = timer3.Elapsed;
            string timeFinal3 = "Time taken With Only Keys Fetch 3rd attempt:" + timeTaken3.ToString();
            Console.WriteLine(timeFinal3 + "\n");




            //Start StopWatch with Keys and Data 1st attempt
            //1st Run
            var timer1 = new Stopwatch();
            timer1.Start();

            //Perform Operation
            GetKeysAndData();

            timer1.Stop();
            TimeSpan timeTaken1 = timer1.Elapsed;
            string timeFinal1 = "Time taken With Data & Keys from Cluster Cache 1st attempt: " + timeTaken1.ToString();
            Console.WriteLine(timeFinal1 + "\n");




            //Start StopWatch with Keys and Data 2nd attempt
            //2nd Run
            var timer2nd = new Stopwatch();
            timer2nd.Start();

            //Perform Operation
            GetKeysAndData();

            timer1.Stop();
            TimeSpan timeTaken2nd = timer2nd.Elapsed;
            string timeFinal2nd = "Time taken With Data & Keys from Cluster Cache 2nd attempt: " + timeTaken2nd.ToString();
            Console.WriteLine(timeFinal2nd);


            // Dispose the cache once done
            _cache.Dispose();


        }

        private static void InitializeCache()
        {
            string cache = "getKeysCache"; //ConfigurationManager.AppSettings["CacheId"];

            if (String.IsNullOrEmpty(cache))
            {
                Console.WriteLine("The Cache Name cannot be null or empty.");
                return;
            }

            // Initialize an instance of the cache to begin performing operations:
            _cache = CacheManager.GetCache(cache);
            Console.WriteLine("Cache initialized successfully\n");
        }

        private static void GetKeysOnly()
        {
            try
            {

                // Pre-condition: Cache is already connected
                // Items are already present in the cache
                // Create a query which will be executed on the data set
                // Use the Fully Qualified Name (FQN) of your own custom class
                string query = "SELECT $Value$ FROM SampleData.Product WHERE UnitPrice > ?";

                // Use QueryCommand for query execution
                QueryCommand queryCommand = new QueryCommand(query);

                // Providing parameters for query
                queryCommand.Parameters.Add("UnitPrice", Convert.ToDecimal(0));

                //Initialize List for All Keys List
                List<string> keys = new List<string>();

                // Executing QueryCommand through ICacheReader
                ICacheReader reader = _cache.SearchService.ExecuteReader(queryCommand, false);

                // Check if the result set is not empty
                if (reader.FieldCount > 0)
                {
                    while (reader.Read())
                    {
                        //Populate Keys List
                        keys.Add(reader.GetValue<string>(0));
                    }
                }

                //Initialize Dictionary for Key-Value pairs of Query.
                IDictionary<string, Product> productsList = new Dictionary<string, Product>();

                //Loop to get Data in chunk of 5000 from keys List.
                for (int i = 0; i < keys.Count; i += 5000)
                {
                    //Get Data from Cahche using GetBulk API of cache.
                    IDictionary<string, Product> products = _cache.GetBulk<Product>(keys.GetRange(i, Math.Min(5000, keys.Count - i)));

                    foreach (var product in products)
                    {
                        //Populate key-value Dictionary with Data
                        productsList.Add(product);
                    }

                }
                Console.WriteLine("\nFetch Item Count: " + productsList.Count);
            }
            catch (OperationFailedException ex)
            {
                if (ex.ErrorCode == NCacheErrorCodes.INCORRECT_FORMAT)
                {
                    // Make sure that the query format is correct
                }
                else
                {
                    // Exception can occur due to:
                    // Connection Failures
                    // Operation performed during state transfer
                    // Operation Timeout
                }
            }
            catch (Exception ex)
            {
                // Any generic exception like ArgumentException, ArgumentNullException
            }

        }

        private static void GetKeysAndData()
        {
            try
            {
                // Pre-condition: Cache is already connected
                // Items are already present in the cache
                // Create a query which will be executed on the data set
                // Use the Fully Qualified Name (FQN) of your own custom class
                string query = "SELECT $Value$ FROM SampleData.Product WHERE UnitPrice > ?";

                // Use QueryCommand for query execution
                QueryCommand queryCommand = new QueryCommand(query);

                // Providing parameters for query
                queryCommand.Parameters.Add("UnitPrice", Convert.ToDecimal(0));

                // Executing QueryCommand through ICacheReader
                ICacheReader reader = _cache.SearchService.ExecuteReader(queryCommand, true);

                //Initialize Dictionary for Key-Value pairs of Query.
                Dictionary<string, object> result = new Dictionary<string, object>();

                // Check if the result set is not empty
                if (reader.FieldCount > 0)
                {
                    while (reader.Read())
                    {
                        string key = reader.GetValue<string>(0);
                        var product = reader.GetValue<Product>(1);

                        result.Add(key, product);

                    }
                }
                Console.WriteLine("\nFetch Count: " + result.Count);

            }
            catch (OperationFailedException ex)
            {
                if (ex.ErrorCode == NCacheErrorCodes.INCORRECT_FORMAT)
                {
                    // Make sure that the query format is correct
                }
                else
                {
                    // Exception can occur due to:
                    // Connection Failures
                    // Operation performed during state transfer
                    // Operation Timeout
                }
            }
            catch (Exception ex)
            {
                // Any generic exception like ArgumentException, ArgumentNullException
            }
        }
    }
}
