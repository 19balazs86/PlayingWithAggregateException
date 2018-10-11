using System.Threading;
using System.Threading.Tasks;

namespace PlayingWithAggregateException.Handlers
{
  public interface INumberHandler
  {
    Task HandleAsync(CancellationToken cancellationToken);
  }
}
