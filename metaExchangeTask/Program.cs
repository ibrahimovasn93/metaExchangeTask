using Newtonsoft.Json;
namespace metaExchangeTask
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter the file path:");
            string filePath = Console.ReadLine();

            Console.WriteLine("Please enter Order Type");
            String orderType = Console.ReadLine();

            Console.WriteLine("Please enter Order Amount");
            decimal amount = decimal.Parse(Console.ReadLine());

            List<Balance> balances = new List<Balance>();
            Console.WriteLine("Please enter BTC balance");
            decimal btcbalance = decimal.Parse(Console.ReadLine());

            Console.WriteLine("Please enter Euro balance");
            decimal eurobalance = decimal.Parse(Console.ReadLine());
            balances.Add(new Balance { BTCbalance = btcbalance, EURObalance = eurobalance });

            List<OrderBook> Orderbooks = DeserializedJsonData(filePath);

        }

        //reading order book from file deserialize json
        public static List<OrderBook> DeserializedJsonData(string filePath)
        {
            List<OrderBook> orderbooks = new List<OrderBook>();

            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("File not found.");
                    return orderbooks;
                }

                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        int index = line.IndexOf('{');

                        // Ensure '{' character is found and index is valid
                        if (index >= 0 && index < line.Length)
                        {
                            string substring = line.Substring(0, index).Trim(); // Trim to remove extra spaces
                            string json = line.Substring(index);

                            OrderBook DeserializedOrderBook = JsonConvert.DeserializeObject<OrderBook>(json);
                            DeserializedOrderBook.ExchangeName = substring;
                            orderbooks.Add(DeserializedOrderBook);
                        }
                        else
                        {
                            // Handle the case where '{' character is not found
                            Console.WriteLine("Invalid line format: " + line);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            return orderbooks;
        }      

        public class Balance
        {
            public decimal BTCbalance { get; set; }
            public decimal EURObalance { get; set; }

        }

        public class Ask
        {
            public Order Order { get; set; }
        }

        public class Bid
        {
            public Order Order { get; set; }
        }
        public class Order
        {
            public object Id { get; set; }
            public DateTime Time { get; set; }
            public string Type { get; set; }
            public string Kind { get; set; }
            public decimal Amount { get; set; }
            public decimal Price { get; set; }
        }
        public class OrderBook
        {
            public String ExchangeName { get; set; }
            public String AcqTime { get; set; }
            public List<Bid> Bids { get; set; }
            public List<Ask> Asks { get; set; }

        }

        public enum OrderType
        {
            Buy,
            Sell
        }
    }
}
