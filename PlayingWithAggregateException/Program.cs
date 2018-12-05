using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PlayingWithAggregateException.Handlers;

namespace PlayingWithAggregateException
{
  public class Program
  {
    private static readonly Random _random = new Random();

    public static void Main(string[] args)
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

      Console.WriteLine("-------- Done --------");
    }

    // This example: we manage all types of exceptions inside this method.
    private static void doProcess1()
    {
      List<INumberHandler> handlers = Enumerable.Range(0, 10).Select(n => getNumberHandler(n)).ToList();

      using (CancellationTokenSource tokenSource = new CancellationTokenSource())
      {
        for (int batchPos = 0; batchPos < handlers.Count; batchPos += 3)
        {
          try
          {
            Task[] tasks = handlers
              .Skip(batchPos)
              .Take(3)
              .Select(h => h.HandleAsync(tokenSource.Token)) // Need the async keyword in the implementation!
              .ToArray();

            Task.WaitAll(tasks);
            // Or: await Task.WhenAll(tasks);
            // Diff: https://stackoverflow.com/questions/6123406/waitall-vs-whenall
          }
          catch (AggregateException exp)
          {
            //var notImpExceptions = exp.Flatten().InnerExceptions.OfType<NotImplementedException>();

            foreach (Exception innerEx in exp.Flatten().InnerExceptions)
              Console.WriteLine(innerEx.Message);
          }
        }
      }
    }

    // This example: we handle the NotImplementedException and throw the rest.
    private static void doProcess2()
    {
      IEnumerable<INumberHandler> handlers = Enumerable.Range(0, 5).Select(n => getNumberHandler(n));

      using (CancellationTokenSource tokenSource = new CancellationTokenSource())
      {
        try
        {
          Task[] tasks = handlers.Select(h => h.HandleAsync(tokenSource.Token)).ToArray();

          Task.WaitAll(tasks);
        }
        catch (AggregateException exp)
        {
          // If I do not know what to do.
          // throw exp.Flatten();

          // I know what should I do with this exception.
          exp.Handle(ex => {
            if (ex is NotImplementedException)
              Console.WriteLine("You forgot to implement this method!");

            return ex is NotImplementedException;
          });
        }
      }
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
