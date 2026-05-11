using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Common.Exceptions
{
    public sealed class TemporaryException:Exception
    {
        public TemporaryException(string message) : base(message)
        {

        }
        public TemporaryException(string message,Exception innerException) : base(message,innerException)
        {

        }
    }
}
