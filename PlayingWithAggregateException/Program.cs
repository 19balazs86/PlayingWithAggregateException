using PlayingWithAggregateException.Handlers;

namespace PlayingWithAggregateException
{
    public class Program
  {
    private static readonly Random _random = new Random();

    public static async Task Main(string[] args)
    {
      doProcess1();

      Console.WriteLine("-------- doProcess2 --------");

      try
      {
        doProcess2();
      }
      catch (AggregateException exp)
      {
        foreach (var innerEx in exp.Flatten().InnerExceptions)
          Console.WriteLine($"Here we have something for you: '{innerEx.Message}'");
      }

      Console.WriteLine("-------- doProcess3 --------");

      try
      {
        await doProcess3Async();
      }
      catch (AggregateException exp)
      {
        foreach (var innerEx in exp.Flatten().InnerExceptions)
          Console.WriteLine($"Here we have something for you: '{innerEx.Message}'");
      }

      Console.WriteLine("-------- Done --------");
    }

    // Example: Using Task.WhenAll and manage all types of exceptions inside this method.
    private static void doProcess1()
    {
      List<INumberHandler> handlers = Enumerable.Range(0, 10).Select(n => getNumberHandler(n)).ToList();

      using (CancellationTokenSource cancelTokenSource = new CancellationTokenSource())
      {
        for (int batchPos = 0; batchPos < handlers.Count; batchPos += 3)
        {
          Task[] tasks = handlers
            .Skip(batchPos)
            .Take(3)
            .Select(h => h.HandleAsync(cancelTokenSource.Token)) // Need the async keyword in the implementation!
            .ToArray();

          try
          {
            Task.WaitAll(tasks);
          }
          catch (AggregateException exp)
          {
            //var notImpExceptions = exp.Flatten().InnerExceptions.OfType<NotImplementedException>();

            // Each iteration has an own AggregateException.

            foreach (Exception innerEx in exp.Flatten().InnerExceptions)
              Console.WriteLine(innerEx.Message);
          }
        }
      }
    }

    // Example: Handle the NotImplementedException and throw the rest.
    private static void doProcess2()
    {
      IEnumerable<INumberHandler> handlers = Enumerable.Range(0, 5).Select(n => getNumberHandler(n));

      using (CancellationTokenSource cancelTokenSource = new CancellationTokenSource())
      {
        Task[] tasks = handlers.Select(h => h.HandleAsync(cancelTokenSource.Token)).ToArray();

        try
        {
          Task.WaitAll(tasks);
        }
        catch (AggregateException exp)
        {
          // If you do not know what to do.
          // throw exp.Flatten();

          exp.Handle(ex => {
            if (ex is NotImplementedException)
            {
              // I want to handle this type of exception.
              Console.WriteLine("You forgot to implement this method!");
              return true;
            }

            // The other types of exceptions will wrap in to AggregateException.
            return false;
          });
        }
      }
    }

    // Example: Using await Task.WhenAll and manage all types of exceptions outside this method.
    private static async Task doProcess3Async()
    {
      List<INumberHandler> handlers = Enumerable.Range(0, 10).Select(n => getNumberHandler(n)).ToList();

      List<Exception> exceptions = new List<Exception>();

      using (CancellationTokenSource cancelTokenSource = new CancellationTokenSource())
      {
        for (int batchPos = 0; batchPos < handlers.Count; batchPos += 3)
        {
          Task[] tasks = handlers
            .Skip(batchPos)
            .Take(3)
            .Select(h => h.HandleAsync(cancelTokenSource.Token))
            .ToArray();

          try
          {
            await Task.WhenAll(tasks); // If 1 task is failed, can happen, the others are not able to start.

            Console.WriteLine("No Exception, otherwise this is not happening.");
          }
          catch (Exception ex)
          {
            // Collect or handle the exceptions 1 by 1.
            exceptions.Add(ex);
          }
        }
      }

      // Throw the collection.
      if (exceptions.Count > 0)
        throw new AggregateException("doProcess3Async has some exceptions.", exceptions);
    }

    private static INumberHandler getNumberHandler(int num)
    {
      int r = _random.Next(1, 4);

      if (r % 3 == 0)
        return new EvenNumberHandler(num);
      else if (r % 3 == 1)
        return new OddNumberHandler(num);
      else return new
          ThrowExceptionNumberHandler(num);
    }
  }
}
