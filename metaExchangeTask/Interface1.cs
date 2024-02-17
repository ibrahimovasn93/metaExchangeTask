using static metaExchangeTask.Program;

namespace metaExchangeTask
{
        public interface IMetaExchange
        {
            List<Order> GetMatchedOrders(List<OrderBook> orderBooks, OrderType orderType, decimal amount, Balance balances);
            List<OrderBook> DeserializeJsonData(string filePath);
        }
 }
