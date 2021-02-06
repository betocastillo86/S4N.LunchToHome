﻿using System.Text.RegularExpressions;
using S4N.LuchToHome.Domain.Exceptions;

namespace S4N.LuchToHome.Domain.ValueObjects
{
    public class Route
    {
        protected Route(string path)
        {
            this.Path = path;
        }

        public string Path { get; private set; }

        public static Route CreateRoute(string path)
        {
            if (string.IsNullOrEmpty(path) || !Regex.IsMatch(path, "^[AID]+$"))
            {
                throw new InvalidPathException(path);
            }

            return new Route(path);
        }
    }
}