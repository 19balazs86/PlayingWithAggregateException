using System.Threading;
using System.Threading.Tasks;

namespace PlayingWithAggregateException.Handlers
{
  public abstract class NumberHandlerBase : INumberHandler
  {
    public int MyNumber { get; private set; }

    public NumberHandlerBase(int num)
    {
      MyNumber = num;
    }

    public abstract Task HandleAsync(CancellationToken cancellationToken);
  }
}
