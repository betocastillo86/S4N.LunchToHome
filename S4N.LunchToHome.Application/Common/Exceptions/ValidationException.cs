using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace S4N.LunchToHome.Application.Common.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }

        public IDictionary<string, string[]> Errors { get; }

        public string ErrorsString
        {
            get
            {
                return string.Join("\n", this.Errors.Select(c => string.Concat(c.Key, ": ", string.Join(",", c.Value))));
            }
        }
    }
}