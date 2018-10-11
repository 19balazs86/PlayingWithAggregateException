using System;
using System.Threading;
using System.Threading.Tasks;

namespace PlayingWithAggregateException.Handlers
{
  public class ThrowExceptionNumberHandler : NumberHandlerBase
  {
    public ThrowExceptionNumberHandler(int num) : base(num)
    {
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
      await Task.Delay(100);

      throw new NotImplementedException();
    }
  }
}
