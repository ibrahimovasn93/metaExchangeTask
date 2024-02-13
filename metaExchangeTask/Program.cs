using Newtonsoft.Json;
namespace metaExchangeTask
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<OrderBook> orders = DeserializedJsonData();           

        }
        public static List<OrderBook> DeserializedJsonData()
        {
            List<OrderBook> orderbooks = new List<OrderBook>();

            string String_OrderBook = @"1548759600.25189	{
                ""AcqTime"":""2019-01-29T11:00:00.2518854Z"",
                ""Bids"":[{""Order"":{""Id"":null,""Time"":""0001-01-01T00:00:00"",""Type"":""Buy"",""Kind"":""Limit"",""Amount"":0.01,""Price"":2960.64}},
            		{""Order"":{""Id"":null,""Time"":""0001-01-01T00:00:00"",""Type"":""Buy"",""Kind"":""Limit"",""Amount"":0.01,""Price"":2960.63}},
            		{""Order"":{""Id"":null,""Time"":""0001-01-01T00:00:00"",""Type"":""Buy"",""Kind"":""Limit"",""Amount"":0.5,""Price"":2960.48}},
            		{""Order"":{""Id"":null,""Time"":""0001-01-01T00:00:00"",""Type"":""Buy"",""Kind"":""Limit"",""Amount"":0.062,""Price"":2960.45}},
            		{""Order"":{""Id"":null,""Time"":""0001-01-01T00:00:00"",""Type"":""Buy"",""Kind"":""Limit"",""Amount"":0.5,""Price"":2958.87}}
            		],
            	""Asks"":[{""Order"":{""Id"":null,""Time"":""0001-01-01T00:00:00"",""Type"":""Sell"",""Kind"":""Limit"",""Amount"":0.405,""Price"":2964.29}},
            			{""Order"":{""Id"":null,""Time"":""0001-01-01T00:00:00"",""Type"":""Sell"",""Kind"":""Limit"",""Amount"":0.405,""Price"":2964.3}},
            			{""Order"":{""Id"":null,""Time"":""0001-01-01T00:00:00"",""Type"":""Sell"",""Kind"":""Limit"",""Amount"":0.49,""Price"":2965.0}}]}
            ";
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
    }
}
