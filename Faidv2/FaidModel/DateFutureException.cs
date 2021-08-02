using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faidv2.FaidModel
{
    public class DateFutureException : Exception
    {
        public DateFutureException(string message) : base(message) { }
        public DateFutureException(string message, Exception inner) : base(message, inner) { }
    }
}
