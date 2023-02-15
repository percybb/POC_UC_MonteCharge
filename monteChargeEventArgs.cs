using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UcMonteChange
{
    public class monteChargeEventArgs: EventArgs
    {
        public readonly string message;
        public readonly int etage;
        public monteChargeEventArgs(string message, int etage)
        {
            this.message = message;
            this.etage = etage;
        }
        
    }
}
