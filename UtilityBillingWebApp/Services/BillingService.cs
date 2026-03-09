using UtilityBillingWebApp.Models;

namespace UtilityBillingWebApp.Services
{
    /// <summary>
    /// Service class that handles billing calculation logic with configurable tariffs and forecasting
    /// </summary>
    public class BillingService
    {
        private readonly List<TariffTier> _tariffs;
        private readonly int _surchargeThreshold;
        private readonly double _surchargeRate;

        public BillingService(IConfiguration configuration)
        {
            // Load tariffs from configuration
            var tariffs = configuration.GetSection("Tariffs").Get<List<TariffTier>>();
            
            if (tariffs == null || tariffs.Count == 0)
            {
                throw new InvalidOperationException("Tariffs configuration is missing or empty.");
            }

            // Validate tariffs
            ValidateTariffs(tariffs);
            
            _tariffs = tariffs.OrderBy(t => t.Min).ToList();

            // Load surcharge configuration
            _surchargeThreshold = configuration.GetValue<int>("Surcharge:Threshold", 50);
            _surchargeRate = configuration.GetValue<double>("Surcharge:Rate", 0.10);
        
}
        /// <summary>
        /// Validates that tariffs are properly configured
        /// </summary>
        private void ValidateTariffs(List<TariffTier> tariffs)
        {
            var ordered = tariffs.OrderBy(t => t.Min).ToList();
            
            for (int i = 0; i < ordered.Count; i++)
            {
                var current = ordered[i];
                
                if (!current.IsValid())
                {
                    throw new InvalidOperationException(
                        $"Invalid tariff tier at index {i}: Min={current.Min}, Max={current.Max}, Rate={current.Rate}");
                }

                if (i < ordered.Count - 1)
                {
                    var next = ordered[i + 1];
                    if (current.Max + 1 != next.Min)
                    {
                        throw new InvalidOperationException(
                            $"Gap or overlap between tiers: Tier {i} ends at {current.Max}, Tier {i + 1} starts at {next.Min}");
                    }
                }
            }

            if (ordered[0].Min != 0)
            {
                throw new InvalidOperationException("First tariff tier must start at 0");
            }
        }

        /// <summary>
        /// Calculates the bill based on configurable tiered pricing and surcharges
        /// </summary>
        /// <summary>
/// Calculates the bill based on configurable tiered pricing and surcharges
/// </summary>
public BillDetails CalculateBill(double usage)
{
    if (usage < 0)
        throw new ArgumentException("Usage cannot be negative", nameof(usage));

    var bill = new BillDetails { TotalUsage = usage };
    
    double totalCost = 0;
    double remainingUsage = usage;

    var tierUnits = new List<double>();
    var tierCosts = new List<double>();

    // Tier 1: 0 to 10 (but NOT including 10)
    double tier1Capacity = 10; // 0 to 10 exclusive of 10
    double tier1Units = Math.Min(remainingUsage, tier1Capacity);
    if (tier1Units > 0)
    {
        tierUnits.Add(tier1Units);
        tierCosts.Add(tier1Units * 5);
        totalCost += tier1Units * 5;
        remainingUsage -= tier1Units;
    }

    // Tier 2: 10 to 30 (but NOT including 30)
    if (remainingUsage > 0)
    {
        double tier2Capacity = 20; // 10 to 30 = 20 units
        double tier2Units = Math.Min(remainingUsage, tier2Capacity);
        if (tier2Units > 0)
        {
            tierUnits.Add(tier2Units);
            tierCosts.Add(tier2Units * 8);
            totalCost += tier2Units * 8;
            remainingUsage -= tier2Units;
        }
    }

    // Tier 3: 30 and above
    if (remainingUsage > 0)
    {
        tierUnits.Add(remainingUsage);
        tierCosts.Add(remainingUsage * 12);
        totalCost += remainingUsage * 12;
    }

    bill.Subtotal = totalCost;
    
    // Assign tier details
    bill.Tier1Units = tierUnits.Count > 0 ? tierUnits[0] : 0;
    bill.Tier1Cost = tierCosts.Count > 0 ? tierCosts[0] : 0;
    bill.Tier2Units = tierUnits.Count > 1 ? tierUnits[1] : 0;
    bill.Tier2Cost = tierCosts.Count > 1 ? tierCosts[1] : 0;
    bill.Tier3Units = tierUnits.Count > 2 ? tierUnits[2] : 0;
    bill.Tier3Cost = tierCosts.Count > 2 ? tierCosts[2] : 0;
    
    // Apply surcharge
    if (usage > 50)
    {
        bill.Surcharge = bill.Subtotal * 0.10;
        bill.SurchargeApplied = true;
    }
    
    bill.Total = bill.Subtotal + bill.Surcharge;
    
    return bill;
}

        /// <summary>
        /// Predicts future usage based on historical data using linear trend
        /// </summary>
        /// <param name="historicalUsages">List of historical usage values (oldest to newest)</param>
        /// <returns>Predicted next month's usage</returns>
        public double? PredictNextUsage(List<double> historicalUsages)
        {
            // Need at least 2 data points for trend analysis
            if (historicalUsages == null || historicalUsages.Count < 2)
            {
                return null;
            }

            // Filter out any null or negative values
            var validUsages = historicalUsages.Where(u => u >= 0).ToList();
            
            if (validUsages.Count < 2)
            {
                return null;
            }

            // Simple linear trend calculation
            double firstMonth = validUsages.First();
            double lastMonth = validUsages.Last();
            int numberOfMonths = validUsages.Count - 1; // Number of intervals between measurements

            // Calculate average monthly growth
            double growth = (lastMonth - firstMonth) / numberOfMonths;
            
            // Predict next month: last month + growth
            double predictedUsage = lastMonth + growth;

            // Ensure prediction is not negative
            return Math.Max(0, predictedUsage);
        }

        /// <summary>
        /// Calculates the trend direction and growth rate
        /// </summary>
        public (double? GrowthRate, string Direction) CalculateTrend(List<double> historicalUsages)
        {
            if (historicalUsages == null || historicalUsages.Count < 2)
            {
                return (null, "Insufficient Data");
            }

            var validUsages = historicalUsages.Where(u => u >= 0).ToList();
            
            if (validUsages.Count < 2)
            {
                return (null, "Insufficient Data");
            }

            double firstMonth = validUsages.First();
            double lastMonth = validUsages.Last();
            int numberOfMonths = validUsages.Count - 1;
            
            double growth = (lastMonth - firstMonth) / numberOfMonths;
            
            string direction;
            if (growth > 0.5)
                direction = "Increasing";
            else if (growth < -0.5)
                direction = "Decreasing";
            else
                direction = "Stable";

            return (growth, direction);
        }
    }

    /// <summary>
    /// Class to hold all bill calculation details
    /// </summary>
    public class BillDetails
    {
        public double TotalUsage { get; set; }
        public double Tier1Units { get; set; }
        public double Tier1Cost { get; set; }
        public double Tier2Units { get; set; }
        public double Tier2Cost { get; set; }
        public double Tier3Units { get; set; }
        public double Tier3Cost { get; set; }
        public double Surcharge { get; set; }
        public bool SurchargeApplied { get; set; }
        public double Subtotal { get; set; }
        public double Total { get; set; }
    }
}