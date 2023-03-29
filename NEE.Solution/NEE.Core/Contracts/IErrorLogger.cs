using NEE.Core.BO;

namespace NEE.Core.Contracts
{
    public interface IErrorLogger
    {
        string LogErrorAsync(ErrorLog errorLog);
    }
}
