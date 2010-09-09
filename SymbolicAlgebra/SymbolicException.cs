using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SymbolicAlgebra
{
    [Serializable()]
    public class SymbolicException : Exception
    {
      public SymbolicException()
      {
         // Add any type-specific logic, and supply the default message.
      }

      public SymbolicException(string message): base(message) 
      {
         // Add any type-specific logic.
      }
      public SymbolicException(string message, Exception innerException): 
         base (message, innerException)
      {
         // Add any type-specific logic for inner exceptions.
      }
      protected SymbolicException(SerializationInfo info, 
         StreamingContext context) : base(info, context)
      {
         // Implement type-specific serialization constructor logic.
      }    

    }
}
