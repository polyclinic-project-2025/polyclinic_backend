
namespace PolyclinicApplication.Common.Interfaces
{
    public interface IExportStrategyFactory
    {
        IExportStrategy CreateExportStrategy(string format);
    }
}