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

        //"SELECT $Value$ FROM SampleData.Product WHERE UnitPrice > ?"
        private static IEnumerable<Product>  SearchProducts(string sql)
        {
            // Use QueryCommand for query execution
            QueryCommand queryCommand = new QueryCommand(sql);

            // Providing parameters for query
            queryCommand.Parameters.Add("UnitPrice", Convert.ToDecimal(13));

            // Executing QueryCommand through ICacheReader
            ICacheReader reader = _cache.SearchService.ExecuteReader(queryCommand, true);

            //Initialize Dictionary for Key-Value pairs of Query.
            List<Product> products = new List<Product>();

            // Check if the result set is not empty
            if (reader.FieldCount > 0)
            {
                while (reader.Read())
                {

                    var product = reader.GetValue<Product>(1);

                    //Populate key-value Dictionary with object Data
                    products.Add(product);

                }
            }

            return products;
        }


        //this method performs better only if you are using client cache and have most of the data available in client cache.
        private static IEnumerable<Product> SearchProductsOptimized(string sql)
        {
            // Use QueryCommand for query execution
            QueryCommand queryCommand = new QueryCommand(sql);

            // Providing parameters for query
            queryCommand.Parameters.Add("UnitPrice", Convert.ToDecimal(13));

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

            //Get Data using Bulk API
            IDictionary<string, Product> productsList = _cache.GetBulk<Product>(keys);

            return productsList.Values;
        }


        //this method performs better only if you are using client cache and have most of the data available in client cache.
        private static  IEnumerable<Product> SearchProductsOptimizedMethod2(string sql)
        {
            // Use QueryCommand for query execution
            QueryCommand queryCommand = new QueryCommand(sql);

            // Providing parameters for query
            queryCommand.Parameters.Add("UnitPrice", Convert.ToDecimal(13));

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
            List<Product> productList = new List<Product>();

            //Get Data in chunk of 5000 if number of keys are too high.
            List<List<string>> keysChunk = GetChunkedList(keys,5000);

            //Perform Operation on each List
            for (int i = 0; i < keysChunk.Count; i ++)
            {
                //Get Data from Cahche using GetBulk API of cache.
                IDictionary<string, Product> products = _cache.GetBulk<Product>(keysChunk[i]);

                productList.AddRange(products.Values);

            }

            return productList;
        }


        //GetChunked List
        private static List<List<string>> GetChunkedList(List<string> keys, int size)
        {
            return keys
            .Select((x, i) => new { index = i, value = x })
            .GroupBy(x => x.index / size)
            .Select(x => x.Select(v => v.value).ToList())
            .ToList();
        }


        private static void InitializeCache()
        {
            string cache = "getKeysCache"; //"OutProcCluser";//////  //ConfigurationManager.AppSettings["CacheId"];

            if (String.IsNullOrEmpty(cache))
            {
                Console.WriteLine("The Cache Name cannot be null or empty.");
                return;
            }

            // Initialize an instance of the cache to begin performing operations:
            _cache = CacheManager.GetCache(cache);
            Console.WriteLine("Cache initialized successfully\n");
        }


        public static void TestRun()
        {
            //Initialize Cache.
            InitializeCache();

            //Start StopWatch with Only Keys 1st attempt
            //1st Run
            var timer = new Stopwatch();
            timer.Start();

            //Perform Operation
  
            SearchProducts("SELECT $Value$ FROM SampleData.Product WHERE UnitPrice > ?");

            timer.Stop();
            TimeSpan timeTaken = timer.Elapsed;
            string timeFinal = "Time taken Fetch : (SearchProduct) " + timeTaken.ToString();
            Console.WriteLine(timeFinal + "\n");







            //Start StopWatch with Keys and Data 1st attempt
            //1st Run
            var timer1 = new Stopwatch();
            timer1.Start();

            //Perform Operation
            SearchProductsOptimizedMethod2("SELECT $Value$ FROM SampleData.Product WHERE UnitPrice > ?");

            timer1.Stop();
            TimeSpan timeTaken1 = timer1.Elapsed;
            string timeFinal1 = "Time taken SearchProductsOptimizedMethod2  " + timeTaken1.ToString();
            Console.WriteLine(timeFinal1 + "\n");







            //Start StopWatch with Only Keys 2nd attempt
            //2nd Run
            var timer2 = new Stopwatch();
            timer2.Start();

            //Perform Operation
            SearchProductsOptimized("SELECT $Value$ FROM SampleData.Product WHERE UnitPrice > ?");


            timer2.Stop();
            TimeSpan timeTaken2 = timer2.Elapsed;
            string timeFinal2 = "Time taken Fetch : (SearchProductOptimized) " + timeTaken2.ToString();
            Console.WriteLine(timeFinal2 + "\n");





            //Start StopWatch with Only Keys 3rd attempt
            //3rd Run
            var timer3 = new Stopwatch();
            timer3.Start();

            //Perform Operation
            SearchProductsOptimized("SELECT $Value$ FROM SampleData.Product WHERE UnitPrice > ?");

            timer3.Stop();
            TimeSpan timeTaken3 = timer3.Elapsed;
            string timeFinal3 = "Time taken fetch  : (SearchProductOptimized) " + timeTaken3.ToString();
            Console.WriteLine(timeFinal3 + "\n");





            // Dispose the cache once done
            _cache.Dispose();


        }


    }
}
