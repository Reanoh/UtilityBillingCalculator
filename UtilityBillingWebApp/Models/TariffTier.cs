namespace UtilityBillingWebApp.Models
{
    /// <summary>
    /// Represents a single tariff tier for water billing
    /// </summary>
    public class TariffTier
    {
        /// <summary>
        /// Minimum units for this tier (inclusive)
        /// </summary>
        public int Min { get; set; }

        /// <summary>
        /// Maximum units for this tier (inclusive)
        /// </summary>
        public int Max { get; set; }

        /// <summary>
        /// Rate per unit for this tier in Rands
        /// </summary>
        public double Rate { get; set; }  // Changed from decimal to double

        /// <summary>
        /// Validates that the tier configuration is correct
        /// </summary>
        public bool IsValid()
        {
            return Min >= 0 && Max > Min && Rate > 0;
        }

        /// <summary>
        /// Gets the range size of this tier
        /// </summary>
        public int RangeSize => Max - Min + 1;
    }
}