using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SymbolicAlgebra
{
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

    }
}
