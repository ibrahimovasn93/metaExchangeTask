using Newtonsoft.Json;
namespace metaExchangeTask
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*** MetaExchange Task ***");

            // Get file path from the user
            string filePath = GetFilePathFromUser();
            List<OrderBook> orderBooks = DeserializeJsonData(filePath);

            if (orderBooks.Count == 0)
            {
                Console.WriteLine("No order books found or file not found.");
                return;
            }

            // Get user input for order type, amount, and balances
            OrderType orderType = GetOrderTypeFromUser();
            decimal amount = GetOrderAmountFromUser();
            Balance balance = GetBalanceFromUser();

            // Get matched orders
            List<Order> matchedOrders = GetMatchedOrders(orderBooks, orderType, amount, balance);
            string matchedOrdersJson = JsonConvert.SerializeObject(matchedOrders);
            Console.WriteLine(matchedOrdersJson);

        }

        public static string GetFilePathFromUser()
        {
            Console.WriteLine("Please enter the file path:");
            return Console.ReadLine();
        }

        // Get order type from the user
        public static OrderType GetOrderTypeFromUser()
        {
            Console.WriteLine("Please enter Order Type (Buy/Sell):");
            string input = Console.ReadLine().ToLower();
            return input == "buy" ? OrderType.Buy : OrderType.Sell;
        }

        // Get order amount from the user
        public static decimal GetOrderAmountFromUser()
        {
            Console.WriteLine("Please enter Order Amount:");
            return decimal.Parse(Console.ReadLine());
        }

        // Get balance from the user
        public static Balance GetBalanceFromUser()
        {
            Console.WriteLine("Please enter BTC balance:");
            decimal btcBalance = decimal.Parse(Console.ReadLine());

            Console.WriteLine("Please enter Euro balance:");
            decimal euroBalance = decimal.Parse(Console.ReadLine());

            return new Balance { BTCbalance = btcBalance, EURObalance = euroBalance };
        }

        //reading order book from file deserialize json
        public static List<OrderBook> DeserializeJsonData(string filePath)
        {
            List<OrderBook> orderBooks = new List<OrderBook>();

            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("File not found.");
                    return orderBooks;
                }

                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        // Find the index of '{' character
                        int index = line.IndexOf('{');

                        // Ensure '{' character is found and index is valid
                        if (index >= 0 && index < line.Length)
                        {
                            // Extract the exchange name substring
                            string exchangeName = line.Substring(0, index).Trim();

                            // Extract the JSON substring
                            string json = line.Substring(index);

                            // Deserialize JSON to OrderBook object
                            OrderBook orderBook = JsonConvert.DeserializeObject<OrderBook>(json);

                            // Set the ExchangeName property and add to the list
                            orderBook.ExchangeName = exchangeName;
                            orderBooks.Add(orderBook);
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
                Console.WriteLine("An error occurred while reading the file:");
                Console.WriteLine(e.Message);
            }

            return orderBooks;
        }

        public static List<Order> GetMatchedOrders(List<OrderBook> orderBooks, OrderType orderType, decimal amount, Balance balances)
        {
            List<Order> matchedOrders = new List<Order>();
            decimal btcbalance = balances.BTCbalance;
            decimal eurobalance = balances.EURObalance;

            if (orderType == OrderType.Buy)
            {
                var matchedAsks = orderBooks.SelectMany(o => o.Asks)
                    .OrderBy(a => a.Order.Price); // Ascending for buy orders

                foreach (var ask in matchedAsks)
                {
                    // Check if there are sufficient balances and amount to execute the trade
                    if (btcbalance > 0 && eurobalance > 0 && amount > 0)
                    {
                        // Calculate the total cost of buying 'amount' BTC at the ask price
                        decimal totalCost = amount * ask.Order.Price;

                        // Check if the total cost is within the available EUR balance
                        if (totalCost <= eurobalance)
                        {
                            // Check if the amount to buy is less than or equal to the ask amount
                            if (amount <= ask.Order.Amount)
                            {
                                // Update balances and amount
                                eurobalance -= totalCost;
                                btcbalance += amount;
                                amount = 0; // All requested BTC has been bought
                                matchedOrders.Add(new Order
                                {
                                    Id = ask.Order.Id,
                                    Time = ask.Order.Time,
                                    Type = ask.Order.Type,
                                    Kind = ask.Order.Kind,
                                    Amount = ask.Order.Amount,
                                    Price = ask.Order.Price
                                });

                            }
                            else
                            {
                                // Update balances and amount
                                eurobalance -= ask.Order.Amount * ask.Order.Price;
                                btcbalance += ask.Order.Amount;
                                amount -= ask.Order.Amount;
                                matchedOrders.Add(new Order
                                {
                                    Id = ask.Order.Id,
                                    Time = ask.Order.Time,
                                    Type = ask.Order.Type,
                                    Kind = ask.Order.Kind,
                                    Amount = ask.Order.Amount,
                                    Price = ask.Order.Price
                                });
                            }

                        }
                        else
                        {
                            // Insufficient EUR balance to execute the trade
                            break;
                        }
                    }
                    else
                    {
                        // Insufficient balance or no more amount to buy
                        break;
                    }
                }
            }
            else if (orderType == OrderType.Sell)
            {
                var matchedBids = orderBooks.SelectMany(o => o.Bids)
                                            .OrderByDescending(b => b.Order.Price); // Descending for sell orders

                foreach (var bid in matchedBids)
                {
                    // Check if there are sufficient balances and amount to execute the trade
                    if (btcbalance > 0 && eurobalance > 0 && amount > 0)
                    {
                        // Calculate the total proceeds of selling 'amount' BTC at the bid price
                        decimal totalProceeds = amount * bid.Order.Price;

                        // Check if the amount to sell is less than or equal to the bid amount
                        if (amount <= bid.Order.Amount)
                        {
                            // Update balances and amount
                            eurobalance += totalProceeds;
                            btcbalance -= amount;
                            amount = 0; // All requested BTC has been sold
                            matchedOrders.Add(new Order
                            {
                                Id = bid.Order.Id,
                                Time = bid.Order.Time,
                                Type = bid.Order.Type,
                                Kind = bid.Order.Kind,
                                Amount = bid.Order.Amount,
                                Price = bid.Order.Price
                            });
                        }
                        else
                        {
                            // Update balances and amount
                            eurobalance += bid.Order.Amount * bid.Order.Price;
                            btcbalance -= bid.Order.Amount;
                            amount -= bid.Order.Amount;
                            matchedOrders.Add(new Order
                            {
                                Id = bid.Order.Id,
                                Time = bid.Order.Time,
                                Type = bid.Order.Type,
                                Kind = bid.Order.Kind,
                                Amount = bid.Order.Amount,
                                Price = bid.Order.Price
                            });
                        }
                    }
                    else
                    {
                        // Insufficient balance or no more amount to sell
                        break;
                    }
                }
            }

            return matchedOrders;
        }

        public class OrderBook
        {
            public String ExchangeName { get; set; }
            public String AcqTime { get; set; }
            public List<Bid> Bids { get; set; }
            public List<Ask> Asks { get; set; }

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

        public class Ask
        {
            public Order Order { get; set; }
        }

        public class Bid
        {
            public Order Order { get; set; }
        }

        public class Balance
        {
            public decimal BTCbalance { get; set; }
            public decimal EURObalance { get; set; }

        }

        public enum OrderType
        {
            Buy,
            Sell
        }
    }
}
