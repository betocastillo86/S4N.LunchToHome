using System;

namespace S4N.LunchToHome.Application.Common.Exceptions
{
    public class ProcessPathException : Exception
    {
        public ProcessPathException()
        {

        }

        public ProcessPathException(string message)
            : base(message)
        {
        }
    }
}