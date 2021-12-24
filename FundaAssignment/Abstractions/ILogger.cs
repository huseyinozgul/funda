using System;
namespace fundaconsole.Abstractions
{
    public interface ILogger
    {
        void Error(Exception ex);
        void Info(string message);
    }
}
