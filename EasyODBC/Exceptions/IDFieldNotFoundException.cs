using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyODBC.Exceptions
{
    public class IDFieldNotFoundException:Exception
    {
        public IDFieldNotFoundException(string message) : base(message)
        {

        }
    }
}
