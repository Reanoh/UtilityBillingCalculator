using Microsoft.AspNetCore.Mvc;
using UtilityBillingWebApp.Services;
using UtilityBillingWebApp.ViewModels;

namespace UtilityBillingWebApp.Controllers
{
    public class BillingController : Controller
    {
        private readonly BillingService _billingService;
        private readonly InvoiceService _invoiceService;

        public BillingController(BillingService billingService, InvoiceService invoiceService)
        {
            _billingService = billingService;
            _invoiceService = invoiceService;
        }

        // GET: Billing
        public IActionResult Index()
        {
            return View(new BillingViewModel());
        }

        // POST: Billing
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(BillingViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Calculate the bill for current usage
                    var billDetails = _billingService.CalculateBill(model.WaterUsage);

                    // Map the results to the view model
                    MapBillDetailsToViewModel(model, billDetails);

                    // Generate bill ID and date for PDF
                    model.BillId = _invoiceService.GenerateBillId();
                    model.GenerationDate = DateTime.Now;
                    model.ShowDownloadButton = true;

                    // Perform forecasting
                    PerformForecasting(model);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred while calculating the bill: {ex.Message}");
                }
            }

            return View(model);
        }

        // GET: Download PDF
        [HttpGet]
        public IActionResult DownloadPdf(string billId, double waterUsage, double totalUsage, 
            double tier1Units, double tier1Cost, double tier2Units, double tier2Cost, 
            double tier3Units, double tier3Cost, double subtotal, double surcharge, 
            bool surchargeApplied, double total, DateTime generationDate)
        {
            try
            {
                // Reconstruct BillDetails from parameters
                var billDetails = new BillDetails
                {
                    TotalUsage = totalUsage,
                    Tier1Units = tier1Units,
                    Tier1Cost = tier1Cost,
                    Tier2Units = tier2Units,
                    Tier2Cost = tier2Cost,
                    Tier3Units = tier3Units,
                    Tier3Cost = tier3Cost,
                    Subtotal = subtotal,
                    Surcharge = surcharge,
                    SurchargeApplied = surchargeApplied,
                    Total = total
                };

                // Generate PDF
                byte[] pdfBytes = _invoiceService.GenerateInvoicePdf(
                    billId,
                    generationDate,
                    waterUsage,
                    billDetails);

                // Return PDF file
                return File(pdfBytes, "application/pdf", $"WaterBill_{billId}.pdf");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to generate PDF: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        private void MapBillDetailsToViewModel(BillingViewModel model, BillDetails billDetails)
        {
            model.TotalUsage = billDetails.TotalUsage;
            model.Tier1Units = billDetails.Tier1Units;
            model.Tier1Cost = billDetails.Tier1Cost;
            model.Tier2Units = billDetails.Tier2Units;
            model.Tier2Cost = billDetails.Tier2Cost;
            model.Tier3Units = billDetails.Tier3Units;
            model.Tier3Cost = billDetails.Tier3Cost;
            model.Subtotal = billDetails.Subtotal;
            model.Surcharge = billDetails.Surcharge;
            model.SurchargeApplied = billDetails.SurchargeApplied;
            model.Total = billDetails.Total;
        }

        private void PerformForecasting(BillingViewModel model)
        {
            // Collect historical usage for forecasting
            var historicalUsages = new List<double>();
            
            if (model.Usage6MonthsAgo.HasValue)
                historicalUsages.Add(model.Usage6MonthsAgo.Value);
            if (model.Usage5MonthsAgo.HasValue)
                historicalUsages.Add(model.Usage5MonthsAgo.Value);
            if (model.Usage4MonthsAgo.HasValue)
                historicalUsages.Add(model.Usage4MonthsAgo.Value);
            if (model.Usage3MonthsAgo.HasValue)
                historicalUsages.Add(model.Usage3MonthsAgo.Value);
            if (model.Usage2MonthsAgo.HasValue)
                historicalUsages.Add(model.Usage2MonthsAgo.Value);
            if (model.UsageLastMonth.HasValue)
                historicalUsages.Add(model.UsageLastMonth.Value);

            model.HistoricalUsages = historicalUsages;

            // Perform forecasting if we have enough historical data
            if (historicalUsages.Count >= 2)
            {
                model.PredictedNextUsage = _billingService.PredictNextUsage(historicalUsages);
                
                var trend = _billingService.CalculateTrend(historicalUsages);
                model.GrowthRate = trend.GrowthRate;
                model.TrendDirection = trend.Direction;

                if (model.PredictedNextUsage.HasValue)
                {
                    var predictedBill = _billingService.CalculateBill(model.PredictedNextUsage.Value);
                    model.PredictedNextBill = predictedBill.Total;
                }
            }
        }
    }
}