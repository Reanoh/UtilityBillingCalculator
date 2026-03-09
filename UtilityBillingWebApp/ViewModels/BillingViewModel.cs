using System.ComponentModel.DataAnnotations;

namespace UtilityBillingWebApp.ViewModels
{
    public class BillingViewModel
    {
        [Required(ErrorMessage = "Please enter water usage")]
        [Range(0, double.MaxValue, ErrorMessage = "Water usage cannot be negative")]
        [Display(Name = "Current Water Usage (cubic meters)")]
        public double WaterUsage { get; set; }

        // Historical usage for forecasting
        [Display(Name = "Usage 6 months ago")]
        [Range(0, double.MaxValue, ErrorMessage = "Usage cannot be negative")]
        public double? Usage6MonthsAgo { get; set; }

        [Display(Name = "Usage 5 months ago")]
        [Range(0, double.MaxValue, ErrorMessage = "Usage cannot be negative")]
        public double? Usage5MonthsAgo { get; set; }

        [Display(Name = "Usage 4 months ago")]
        [Range(0, double.MaxValue, ErrorMessage = "Usage cannot be negative")]
        public double? Usage4MonthsAgo { get; set; }

        [Display(Name = "Usage 3 months ago")]
        [Range(0, double.MaxValue, ErrorMessage = "Usage cannot be negative")]
        public double? Usage3MonthsAgo { get; set; }

        [Display(Name = "Usage 2 months ago")]
        [Range(0, double.MaxValue, ErrorMessage = "Usage cannot be negative")]
        public double? Usage2MonthsAgo { get; set; }

        [Display(Name = "Last month's usage")]
        [Range(0, double.MaxValue, ErrorMessage = "Usage cannot be negative")]
        public double? UsageLastMonth { get; set; }

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

        // Forecasting results
        public List<double> HistoricalUsages { get; set; } = new List<double>();
        public double? PredictedNextUsage { get; set; }
        public double? PredictedNextBill { get; set; }
        public double? GrowthRate { get; set; }
        public string TrendDirection { get; set; } = "Stable";

        // PDF Export properties
        public string BillId { get; set; } = string.Empty;
        public DateTime GenerationDate { get; set; }
        public bool ShowDownloadButton { get; set; }

        // Flag to indicate if we have results to display
        public bool HasResults => Total.HasValue;
        public bool HasForecast => PredictedNextUsage.HasValue && PredictedNextBill.HasValue;
    }
}