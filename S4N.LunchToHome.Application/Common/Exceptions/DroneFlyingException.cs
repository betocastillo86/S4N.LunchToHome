using System;

namespace S4N.LunchToHome.Application.Common.Exceptions
{
    public class DroneFlyingException : Exception
    {
        public DroneFlyingException()
        {

        }

        public DroneFlyingException(string message)
            : base(message)
        {
        }
    }
}