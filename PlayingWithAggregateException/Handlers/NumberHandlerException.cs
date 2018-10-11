using System;

namespace PlayingWithAggregateException.Handlers
{
  public class NumberHandlerException : Exception
  {
    public int Number { get; set; }

    public NumberHandlerException()
    {
    }

    public NumberHandlerException(string message) : base(message)
    {
    }

    public NumberHandlerException(string message, Exception innerException) : base(message, innerException)
    {
    }
  }
}
