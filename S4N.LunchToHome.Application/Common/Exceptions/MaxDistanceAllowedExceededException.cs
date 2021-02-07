using System;

namespace S4N.LunchToHome.Application.Common.Exceptions
{
    public class MaxDistanceAllowedExceededException : Exception
    {
        public MaxDistanceAllowedExceededException(string message) : base(message)
        {
        }
    }
}