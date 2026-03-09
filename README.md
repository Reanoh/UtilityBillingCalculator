# Utility Billing Calculator (ASP.NET Core MVC Web App)

A modern **ASP.NET Core MVC Web Application** that calculates municipal water bills using configurable tariff tiers, automatic surcharge rules, and professional invoice generation.

This project demonstrates clean MVC architecture, configurable business logic, and production-style project organization.

---

## Primary Application

> The main implementation is the **Web Application (MVC)** version.

It provides:

* Clean browser-based interface
* Configurable tier-based billing calculation
* Automatic surcharge application
* Optional usage forecasting
* PDF invoice generation
* Input validation
* Structured MVC architecture

---

## Billing Logic

The application calculates water bills using **tier-based pricing loaded from configuration**.

Default configuration:

* **Tier 1:** 0–10 units @ R5 per unit
* **Tier 2:** 11–30 units @ R8 per unit
* **Tier 3:** 31+ units @ R12 per unit

Additional rule:

* **10% surcharge** applied when usage exceeds 50 units

Tariff tiers are **not hardcoded** and are instead loaded dynamically from `appsettings.json`, allowing tariff changes without modifying the application code.

---

## Forecasting Feature

The application can optionally estimate the **next month's water usage and bill** using previous usage values.

Users may enter past monthly usage values and the system calculates a **simple growth trend** to estimate future consumption.

The predicted usage is then processed by the billing engine to estimate the **next expected bill**.

---

## Invoice Export

After a bill is generated, the system can produce a **downloadable PDF invoice** containing:

* Unique Bill ID
* Date generated
* Water usage
* Tier cost breakdown
* Subtotal
* Surcharge
* Total amount

PDF generation is handled by a dedicated service to maintain clean separation from billing logic.

---

## Project Structure

```
UtilityBillingWebApp/        → Main ASP.NET Core MVC Application
UtilityBillingCalculator/    → Original Console Application
UtilityBillingCalculator.sln → Solution File
```

---

## Tech Stack

* ASP.NET Core MVC (.NET 8)
* C#
* Bootstrap
* QuestPDF
* Git & GitHub

---

## Download & Run

You can download the executable from the **Releases section**:

1. Go to the **Releases** tab
2. Download `UtilityBillingWebApp.zip` or `UtilityBillingCalculator.exe`
3. Extract (if zipped)
4. Run the executable

Note: If you are downloading the console application executable, it will run in the terminal environment.

---

## Author

Reanoh
