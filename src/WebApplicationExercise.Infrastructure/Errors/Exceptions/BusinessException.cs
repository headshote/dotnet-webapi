using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WebApplicationExercise.Infrastructure.Errors.Exceptions
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
