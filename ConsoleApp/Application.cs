using System;

namespace ConsoleApp
{
  internal class Application
  {
    private const int EXIT_SUCCESS = 0;
    private const int EXIT_FAILURE = 1;

    public int Run(string[] args)
    {
      Logger.Instance.Write(Logger.LogLevelEnumeration.Info, "ProcessLog", "Result");
      Logger.Instance.Write(Logger.LogLevelEnumeration.Info, "ProcessLog", new string[] { "Result.A", "Result.B", "Result.C" });

      try
      {
        throw new Exception("crash!");
      }
      catch (Exception e)
      {
        try
        {
          throw new Exception("_crash!", e);
        }
        catch (Exception ee)
        {
          Logger.Instance.Write(Logger.LogLevelEnumeration.Info, "ExceptionLog", ee);
        }
      }

      Logger.Instance.Write(Logger.LogLevelEnumeration.Info, "ProcessLog", "Result.Split", "Split");

      return EXIT_SUCCESS;
    }
  }
}
