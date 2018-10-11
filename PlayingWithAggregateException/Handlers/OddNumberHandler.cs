using System;
using System.Threading;
using System.Threading.Tasks;

namespace PlayingWithAggregateException.Handlers
{
  public class OddNumberHandler : NumberHandlerBase
  {
    public OddNumberHandler(int num) : base(num)
    {
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
      await Task.Delay(100);

      if (MyNumber % 2 == 0) throw new NumberHandlerException("My number is NOT odd!") { Number = MyNumber };

      Console.WriteLine("Ok: my number is odd.");
    }
  }
}
