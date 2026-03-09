using UtilityBillingWebApp.Models;

namespace UtilityBillingWebApp.Services
{
    /// <summary>
    /// Interface for tariff service to enable testing and future extensions
    /// </summary>
    public interface ITariffService
    {
        BillDetails CalculateBill(double usage);
        IReadOnlyList<TariffTier> GetCurrentTariffs();
    }
}