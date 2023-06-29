using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    internal interface ITradesRepository
    { 
        void AddTrade(Trade trade);
        void DeleteAll();
    }
    internal class TradesRepository: ITradesRepository
    {
        TradesContext tradesContext;

        public TradesRepository(TradesContext tradesContext)
        {
            this.tradesContext = tradesContext;
        }
       
        void ITradesRepository.AddTrade(Trade trade)
        {
            tradesContext.Trades.Add(trade);
            tradesContext.SaveChanges();
        }

        public void DeleteAll()
        {
            foreach(var trade in tradesContext.Trades)
            {
                tradesContext.Trades.Remove(trade);
            }
            tradesContext.SaveChanges();
        }        
    }
}
