using System;
using System.Reflection.Metadata;

namespace UtilityBillingCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("       UTILITY BILLING CALCULATOR       ");
            Console.WriteLine("========================================");

            try
            {
                double waterUsage = GetWaterUsage();
                BillDetails bill = CalculateBill(waterUsage);
                DisplayBill(bill);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"\nAn unexpected error occurred: {ex.Message}");
                Console.WriteLine("Please restart the application and try again.");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
        static void DisplayBill(BillDetails bill)
        {
            Console.WriteLine("\n\n========================================");
            Console.WriteLine("          UTILITY BILL SUMMARY");
            Console.WriteLine("========================================\n");
            
            Console.WriteLine($"Total Water Usage: {bill.TotalUsage:F2} cubic meters\n");
            
            Console.WriteLine("CHARGE BREAKDOWN:");
            Console.WriteLine("-----------------");
            
            // Display tier 1 charges
            if (bill.Tier1Units > 0)
            {
                Console.WriteLine($"Tier 1 (0-10 units @ R5/unit): {bill.Tier1Units:F2} units x R5 = R{bill.Tier1Cost:F2}");
            }
            
            // Display tier 2 charges
            if (bill.Tier2Units > 0)
            {
                Console.WriteLine($"Tier 2 (11-30 units @ R8/unit): {bill.Tier2Units:F2} units x R8 = R{bill.Tier2Cost:F2}");
            }
            
            // Display tier 3 charges
            if (bill.Tier3Units > 0)
            {
                Console.WriteLine($"Tier 3 (31+ units @ R12/unit): {bill.Tier3Units:F2} units x R12 = R{bill.Tier3Cost:F2}");
            }
            
            Console.WriteLine("\nBILL SUMMARY:");
            Console.WriteLine("-------------");
            Console.WriteLine($"Subtotal: R{bill.Subtotal:F2}");
            
            // Display surcharge if applied
            if (bill.SurchargeApplied)
            {
                Console.WriteLine($"Surcharge (10%): R{bill.Surcharge:F2}");
            }
            else
            {
                Console.WriteLine("Surcharge: Not applicable (usage ≤ 50 units)");
            }
            
            Console.WriteLine($"\nTOTAL AMOUNT DUE: R{bill.Total:F2}");
            Console.WriteLine("========================================");
        }

        static double GetWaterUsage()
        {
            double waterUsage = 0;
            bool isValidInput = false;

            while (!isValidInput)
            {
                Console.Write("Enter water usage in cubic meters: ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Error: Input cannot be empty. Please try again.\n");
                    continue;
                }
                if (!double.TryParse(input, out waterUsage))
                {
                    Console.WriteLine("Error: Invalid number format. Please enter a valid number.\n");
                    continue;
                }
                if (waterUsage < 0)
                {
                     Console.WriteLine("Error: Water usage cannot be negative. Please try again.\n");
                    continue;
                }
                isValidInput = true;
            }
            
            return waterUsage;
        }
        static BillDetails CalculateBill(double usage)
        {
            BillDetails bill = new BillDetails();
            bill.TotalUsage = usage;

            double remainingUsage = usage;
            double tier1Units = Math.Min(remainingUsage, 10);
            bill.Tier1Cost = tier1Units * 5;
            bill.Tier1Units = tier1Units;
            remainingUsage -= tier1Units;

            if (remainingUsage > 0)
            {
                double tier2Units = Math.Min(remainingUsage, 20); // 20 units in tier 2 (11-30)
                bill.Tier2Cost = tier2Units * 8;
                bill.Tier2Units = tier2Units;
                remainingUsage -= tier2Units;
            }
            if (remainingUsage > 0)
            {
                bill.Tier3Units = remainingUsage;
                bill.Tier3Cost = remainingUsage * 12;
            }
            bill.Subtotal = bill.Tier1Cost + bill.Tier2Cost + bill.Tier3Cost;
            if (usage > 50)
            {
                bill.Surcharge = bill.Subtotal * 0.10;
                bill.SurchargeApplied = true;
            }
            bill.Total = bill.Subtotal + bill.Surcharge;
            return bill;
        }
        
        
    }
    public class BillDetails
    {
        public double TotalUsage{get; set;}

        public double Tier1Units{get;set;}
        public double Tier1Cost { get; set; } 

        public double Tier2Units{get;set;}
        public double Tier2Cost { get; set; } 

        public double Tier3Units{get;set;}
        public double Tier3Cost { get; set; } 

        public double Surcharge { get; set; }
        public bool SurchargeApplied { get; set; }

        public double Subtotal { get; set; }
        public double Total { get; set; }

    }
}
