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
        public monteChargeEventArgs(string message)
        {
            this.message = message;
        }
        
    }
}
