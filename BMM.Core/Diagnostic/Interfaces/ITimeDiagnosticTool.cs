using System;
using System.Threading.Tasks;

namespace BMM.Core.Diagnostic.Interfaces
{
    public interface ITimeDiagnosticTool
    {
        Task LogIfConditionIsTrueAfterSpecifiedTime(
            Func<bool> condition,
            int timeInMillis,
            string errorTag);
    }
}