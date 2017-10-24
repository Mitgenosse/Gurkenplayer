using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CSLCoop
{
    [Serializable]
    public class MPException : Exception
    {
        public MPException() { }
        public MPException(string message) : base(message) { }
        public MPException(string message, Exception inner) : base(message, inner) { }
        protected MPException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}