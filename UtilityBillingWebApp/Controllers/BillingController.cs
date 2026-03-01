using Microsoft.AspNetCore.Mvc;
using UtilityBillingCalculator.Services;
using UtilityBillingCalculator.ViewModels;

namespace UtilityBillingCalculator.Controllers
{
    public class BillingController : Controller
    {
        private readonly BillingService _billingService;

        public BillingController(BillingService billingService)
        {
            _billingService = billingService;
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
                    // Calculate the bill using the service
                    var billDetails = _billingService.CalculateBill(model.WaterUsage);

                    // Map the results back to the view model
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
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred while calculating the bill: {ex.Message}");
                }
            }

            return View(model);
        }
    }
}