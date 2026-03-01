namespace UtilityBillingCalculator.Services
{
    /// <summary>
    /// Service class that handles all billing calculation logic
    /// </summary>
    public class BillingService
    {
        /// <summary>
        /// Calculates the bill based on tiered pricing and surcharges
        /// </summary>
        /// <param name="usage">Water usage in cubic meters</param>
        /// <returns>BillDetails object containing all bill calculations</returns>
        public BillDetails CalculateBill(double usage)
        {
            var bill = new BillDetails
            {
                TotalUsage = usage
            };
            
            // Calculate tiered pricing
            double remainingUsage = usage;
            
            // Tier 1: 0-10 units at R5 per unit
            double tier1Units = Math.Min(remainingUsage, 10);
            bill.Tier1Cost = tier1Units * 5;
            bill.Tier1Units = tier1Units;
            remainingUsage -= tier1Units;
            
            // Tier 2: 11-30 units at R8 per unit
            if (remainingUsage > 0)
            {
                double tier2Units = Math.Min(remainingUsage, 20); // 20 units in tier 2 (11-30)
                bill.Tier2Cost = tier2Units * 8;
                bill.Tier2Units = tier2Units;
                remainingUsage -= tier2Units;
            }
            
            // Tier 3: 31+ units at R12 per unit
            if (remainingUsage > 0)
            {
                bill.Tier3Units = remainingUsage;
                bill.Tier3Cost = remainingUsage * 12;
            }
            
            // Calculate subtotal
            bill.Subtotal = bill.Tier1Cost + bill.Tier2Cost + bill.Tier3Cost;
            
            // Apply 10% surcharge if usage exceeds 50 units
            if (usage > 50)
            {
                bill.Surcharge = bill.Subtotal * 0.10;
                bill.SurchargeApplied = true;
            }
            
            // Calculate total
            bill.Total = bill.Subtotal + bill.Surcharge;
            
            return bill;
        }
    }

    /// <summary>
    /// Class to hold all bill calculation details
    /// </summary>
    public class BillDetails
    {
        // Usage details
        public double TotalUsage { get; set; }
        
        // Tier 1 details (0-10 units)
        public double Tier1Units { get; set; }
        public double Tier1Cost { get; set; }
        
        // Tier 2 details (11-30 units)
        public double Tier2Units { get; set; }
        public double Tier2Cost { get; set; }
        
        // Tier 3 details (31+ units)
        public double Tier3Units { get; set; }
        public double Tier3Cost { get; set; }
        
        // Surcharge details
        public double Surcharge { get; set; }
        public bool SurchargeApplied { get; set; }
        
        // Totals
        public double Subtotal { get; set; }
        public double Total { get; set; }
    }
}