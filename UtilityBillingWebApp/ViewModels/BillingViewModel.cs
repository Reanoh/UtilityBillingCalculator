using System.ComponentModel.DataAnnotations;

namespace UtilityBillingCalculator.ViewModels
{
    public class BillingViewModel
    {
        [Required(ErrorMessage = "Please enter water usage")]
        [Range(0, double.MaxValue, ErrorMessage = "Water usage cannot be negative")]
        [Display(Name = "Water Usage (cubic meters)")]
        public double WaterUsage { get; set; }

        // Calculation results
        public double? TotalUsage { get; set; }
        public double? Tier1Units { get; set; }
        public double? Tier1Cost { get; set; }
        public double? Tier2Units { get; set; }
        public double? Tier2Cost { get; set; }
        public double? Tier3Units { get; set; }
        public double? Tier3Cost { get; set; }
        public double? Subtotal { get; set; }
        public double? Surcharge { get; set; }
        public bool SurchargeApplied { get; set; }
        public double? Total { get; set; }

        // Flag to indicate if we have results to display
        public bool HasResults => Total.HasValue;
    }
}