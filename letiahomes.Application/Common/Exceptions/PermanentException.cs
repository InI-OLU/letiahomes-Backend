using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Common.Exceptions
{
    public sealed class PermanentException:Exception
    {
        public PermanentException(string message) : base(message)
        {

        }
        
        public PermanentException(string message,Exception innerException):base(message , innerException)
        {

        }
    }
}
