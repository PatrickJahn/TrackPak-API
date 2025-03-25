using System.Runtime.CompilerServices;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Monitoring;

public static class LoggerExtensions
{
    public static ILogger AddContext(this ILogger logger,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        return logger
            .ForContext("MemberName", memberName)
            .ForContext("FilePath", sourceFilePath)
            .ForContext("LineNumber", lineNumber);
    }
}