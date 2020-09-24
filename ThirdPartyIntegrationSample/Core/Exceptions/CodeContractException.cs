using System;
using System.Runtime.Serialization;

namespace Core.Exceptions
{
    /// <summary>
    /// The exception raised from CodeContract 
    /// </summary>
    [Serializable]
    public class CodeContractException : Exception
    {
        public CodeContractException() { }
        public CodeContractException(string message) : base(message) { }
        public CodeContractException(string message, Exception inner) : base(message, inner) { }
        protected CodeContractException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }
    }
}