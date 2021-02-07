using System;

namespace S4N.LunchToHome.Application.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException()
            : base()
        {
        }

        public NotFoundException(object key)
            : base($"Entity ({key}) was not found.")
        {
        }
    }
}