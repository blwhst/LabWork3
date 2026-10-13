using NLog;

namespace ShoeStoreException;

public static class ExceptionLogger
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static string HandleException(Exception exception)
    {
        if (exception is BaseException known)
        {
            Logger.Warn(known, known.Message);
            return known.Message;
        }

        BaseException unknown = Exceptions.Unknown(exception);
        Logger.Error(unknown, unknown.Message);
        return unknown.Message;
    }

    public static void LogWarning(string message) => Logger.Warn(message);

    public static void LogInformation(string message) => Logger.Info(message);
}