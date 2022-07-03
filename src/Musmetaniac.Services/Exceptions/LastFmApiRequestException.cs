using System;

namespace Musmetaniac.Services.Exceptions
{
    public class LastFmApiRequestException : Exception
    {
        public LastFmApiRequestException(string message = null) : base(message)
        {
        }
    }
}
