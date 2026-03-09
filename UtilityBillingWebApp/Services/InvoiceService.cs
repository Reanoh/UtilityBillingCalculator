using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO; // ADD THIS for Stream

namespace UtilityBillingWebApp.Services
{
    /// <summary>
    /// Service responsible for generating PDF invoices
    /// </summary>
    public class InvoiceService
    {
        private readonly BillingService _billingService;

        public InvoiceService(BillingService billingService)
        {
            _billingService = billingService;
            // Set QuestPDF license (free for development and small-scale production)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        /// <summary>
        /// Generates a unique bill ID (shortened GUID)
        /// </summary>
        public string GenerateBillId()
        {
            // Take first 8 characters of a GUID for a readable unique ID
            return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        }

        /// <summary>
        /// Generates a PDF invoice as a byte array
        /// </summary>
        public byte[] GenerateInvoicePdf(
            string billId,
            DateTime generationDate,
            double waterUsage,
            BillDetails billDetails)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Page settings
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    // Header
                    page.Header().Element(ComposeHeader);

                    // Content - FIXED: Removed the extra 'container' parameter
                    page.Content().Element(content => ComposeContent(content, billId, generationDate, waterUsage, billDetails));

                    // Footer
                    page.Footer().Element(ComposeFooter);
                });
            });

            return document.GeneratePdf();
        }

        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("UTILITY BILLING")
                        .FontSize(20)
                        .Bold()
                        .FontColor(Colors.Blue.Medium);

                    column.Item().Text("Water Usage Invoice")
                        .FontSize(14)
                        .FontColor(Colors.Grey.Medium);
                });

                // FIXED: Removed the image reference that was causing the error
                row.ConstantItem(150).Text("") // Empty text instead of image
                    .FontSize(0);
            });
        }

        private void ComposeContent(IContainer container, string billId, DateTime generationDate, double waterUsage, BillDetails billDetails)
        {
            container.Column(column =>
            {
                // Bill Information
                column.Item().PaddingVertical(10).Element(c => ComposeBillInfo(c, billId, generationDate));

                // Customer/Usage Information
                column.Item().PaddingVertical(10).Element(c => ComposeUsageInfo(c, waterUsage));

                // Bill Details Table
                column.Item().PaddingVertical(10).Element(c => ComposeBillDetailsTable(c, billDetails));

                // Summary
                column.Item().PaddingVertical(10).AlignRight().Element(c => ComposeSummary(c, billDetails));

                // Footer note
                column.Item().PaddingTop(20).Text("This is a computer-generated invoice. No signature is required.")
                    .FontSize(9)
                    .FontColor(Colors.Grey.Medium);
            });
        }

        private void ComposeBillInfo(IContainer container, string billId, DateTime generationDate)
        {
            container.DefaultTextStyle(x => x.FontSize(11));

            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Cell().Text("Bill ID:").Bold();
                table.Cell().Text(billId);

                table.Cell().Text("Date Generated:").Bold();
                table.Cell().Text(generationDate.ToString("dd MMMM yyyy HH:mm"));

                table.Cell().Text("Payment Due:").Bold();
                table.Cell().Text(generationDate.AddDays(30).ToString("dd MMMM yyyy"));
            });
        }

        private void ComposeUsageInfo(IContainer container, double waterUsage)
        {
            container.Background(Colors.Grey.Lighten3).Padding(10).Column(column =>
            {
                column.Item().Text("Water Usage Summary").Bold().FontSize(12);
                column.Item().Text($"Total Water Consumption: {waterUsage:F2} cubic meters (m³)");
            });
        }

        private void ComposeBillDetailsTable(IContainer container, BillDetails bill)
        {
            container.Table(table =>
            {
                // Table columns definition
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                });

                // Table headers
                table.Cell().Element(CellHeaderStyle).Text("Description").Bold();
                table.Cell().Element(CellHeaderStyle).Text("Units").Bold();
                table.Cell().Element(CellHeaderStyle).Text("Rate").Bold();
                table.Cell().Element(CellHeaderStyle).Text("Amount").Bold();

                // Tier 1
                if (bill.Tier1Units > 0)
                {
                    table.Cell().Element(CellStyle).Text("Tier 1 (0-10 units)");
                    table.Cell().Element(CellStyle).Text($"{bill.Tier1Units:F2}");
                    table.Cell().Element(CellStyle).Text("R 5.00");
                    table.Cell().Element(CellStyle).Text($"R {bill.Tier1Cost:F2}");
                }

                // Tier 2
                if (bill.Tier2Units > 0)
                {
                    table.Cell().Element(CellStyle).Text("Tier 2 (11-30 units)");
                    table.Cell().Element(CellStyle).Text($"{bill.Tier2Units:F2}");
                    table.Cell().Element(CellStyle).Text("R 8.00");
                    table.Cell().Element(CellStyle).Text($"R {bill.Tier2Cost:F2}");
                }

                // Tier 3
                if (bill.Tier3Units > 0)
                {
                    table.Cell().Element(CellStyle).Text("Tier 3 (31+ units)");
                    table.Cell().Element(CellStyle).Text($"{bill.Tier3Units:F2}");
                    table.Cell().Element(CellStyle).Text("R 12.00");
                    table.Cell().Element(CellStyle).Text($"R {bill.Tier3Cost:F2}");
                }

                // Local helper methods for cell styling
                static IContainer CellHeaderStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.SemiBold())
                        .PaddingVertical(5)
                        .BorderBottom(1)
                        .BorderColor(Colors.Black);
                }

                static IContainer CellStyle(IContainer container)
                {
                    return container.BorderBottom(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .PaddingVertical(5);
                }
            });
        }

        private void ComposeSummary(IContainer container, BillDetails bill)
        {
            container.Width(250).Column(column =>
            {
                column.Item().PaddingVertical(2).Row(row =>
                {
                    row.RelativeItem().Text("Subtotal:").Bold();
                    row.RelativeItem().Text($"R {bill.Subtotal:F2}").AlignRight();
                });

                if (bill.SurchargeApplied)
                {
                    column.Item().PaddingVertical(2).Row(row =>
                    {
                        row.RelativeItem().Text("Surcharge (10%):").Bold();
                        row.RelativeItem().Text($"R {bill.Surcharge:F2}").AlignRight().FontColor(Colors.Red.Medium);
                    });
                }
                else
                {
                    column.Item().PaddingVertical(2).Row(row =>
                    {
                        row.RelativeItem().Text("Surcharge:").Bold();
                        row.RelativeItem().Text("Not applicable").AlignRight().FontColor(Colors.Grey.Medium);
                    });
                }

                column.Item().PaddingVertical(5).BorderTop(1).BorderColor(Colors.Black).Row(row =>
                {
                    row.RelativeItem().Text("TOTAL AMOUNT DUE:").Bold().FontSize(14);
                    row.RelativeItem().Text($"R {bill.Total:F2}").AlignRight().Bold().FontSize(14).FontColor(Colors.Blue.Medium);
                });
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().AlignCenter().Text("Thank you for your business!")
                        .FontSize(10)
                        .FontColor(Colors.Grey.Medium);

                    column.Item().AlignCenter().Text("For questions about this bill, please contact our customer service.")
                        .FontSize(8)
                        .FontColor(Colors.Grey.Lighten1);
                });
            });
        }
    }
}