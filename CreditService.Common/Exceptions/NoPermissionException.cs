using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallClientBusiness.Common.Exceptions
{
    [Serializable]
    public class NoPermissionException : Exception
    {
        public NoPermissionException() { }

        public NoPermissionException(string message) : base(message) { }

        public NoPermissionException(string message, Exception innerException) : base(message, innerException) { }
    }
}
