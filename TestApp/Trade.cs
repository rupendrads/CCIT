using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    internal class Trade
    {
        string tradeId;
        string isin;
        string notional;

        public Trade() { }

        public Trade(string tradeId, string isin, string notional)
        {
            this.tradeId = tradeId;
            this.isin = isin;   
            this.notional = notional;
        }

        public string TradeID { get => tradeId; set => tradeId = value; }
        public string ISIN { get => isin; set => isin = value; }
        public string Notional { get => notional; set => notional = value; }
    }
}
