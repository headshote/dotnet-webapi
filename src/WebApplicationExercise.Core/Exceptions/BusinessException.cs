using System;
using System.Runtime.Serialization;

namespace WebApplicationExercise.Core.Exceptions
{
    public class BusinessException : Exception
    {
        public string BusinessMessage { get; set; }

        public BusinessException()
        {
        }

        public BusinessException(string message) : base(message)
        {
            BusinessMessage = message;
        }

        public BusinessException(string message, Exception inner) : base(message, inner)
        {
        }

        protected BusinessException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
