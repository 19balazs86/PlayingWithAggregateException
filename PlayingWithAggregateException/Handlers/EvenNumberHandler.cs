using System;
using System.Threading;
using System.Threading.Tasks;

namespace PlayingWithAggregateException.Handlers
{
  public class EvenNumberHandler : NumberHandlerBase
  {
    public EvenNumberHandler(int num) : base(num)
    {
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
      await Task.Delay(100);

      if (MyNumber % 2 != 0) throw new NumberHandlerException("My number is NOT even!") { Number = MyNumber };

      Console.WriteLine("Ok: my number is even.");
    }
  }
}
