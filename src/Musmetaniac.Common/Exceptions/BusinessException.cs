using System;

namespace Musmetaniac.Common.Exceptions
{
    public class BusinessException : Exception
    {
        public BusinessException(string message = null) : base(message)
        {
        }
    }
}
