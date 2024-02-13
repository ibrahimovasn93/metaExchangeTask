using Newtonsoft.Json;
namespace metaExchangeTask
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter Order books");
            String orderbooks = Console.ReadLine();
           
            Console.WriteLine("Please enter Order Type");
            String orderType = Console.ReadLine();

            Console.WriteLine("Please enter Order Amount");
            double amount = Convert.ToDouble(Console.ReadLine());  


            List<OrderBook> orders = DeserializedJsonData(orderbooks);           

        }
        public static List<OrderBook> DeserializedJsonData(string String_OrderBook)
        {
            List<OrderBook> orderbooks = new List<OrderBook>();

            int index = String_OrderBook.IndexOf('{');          
            string json = String_OrderBook.Substring(index);

            OrderBook DeserializedOrderBook = JsonConvert.DeserializeObject<OrderBook>(json);
            orderbooks.Add(DeserializedOrderBook);

            
            return orderbooks;
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
            public double Amount { get; set; }
            public double Price { get; set; }
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
