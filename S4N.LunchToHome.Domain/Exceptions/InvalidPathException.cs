using System;

namespace S4N.LunchToHome.Domain.Exceptions
{
    public class InvalidPathException : Exception
    {
        public InvalidPathException(string direction) : base($"Invalid character in path {direction ?? "null"}")
        {
        }
    }
}