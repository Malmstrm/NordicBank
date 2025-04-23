NordicBank 🏦

Ett fullfjlädrat banksystem byggt i ASP.NET Core Razor Pages med rollstyrd åtkomst, transaktionshantering, AML-scanning och modern UI.

📂 Lösningsstruktur
NordicBankSolution
├── NordicBank/                # UI (Razor Pages med Bootstrap)
│   ├── Pages/                 # Kundsidor, rapporter, dashboards
│   ├── ViewModel/             # ViewModels för vyer (ingen entitet i vyer!)
│   └── wwwroot/               # CSS, JS, bilder
│
├── Service/                   # Affärslogik
│   ├── Interfaces/            # Ex. ICustomerService.cs
│   └── Services/              # Implementationer för tjänster (Customer, Account, AML, etc)
│
├── DAL/                       # Data access med Entity Framework Core
│   ├── Data/                  # DbContext och konfigurationer
│   ├── DTO/                   # Data Transfer Objects
│   ├── Models/                # Domänmodeller
│   └── Migration/             # EF Core migrationer
│
└── NordicBank.Console/        # Console app för AML-scanning (se nedan)
✨ Funktioner

🔐 ASP.NET Core Identity + rollbaserad inloggning (Admin / Cashier)

👤 CRUD för kunder, konton och systemanvändare

🏦 Fullständig kundbild med konton, transaktioner och saldosammanställning

🔎 Sökfunktioner med paginering och AJAX-laddning av transaktioner

📊 Startsida med statistik per land + toppkunder

📊 Interaktiv diagram via Chart.js

💸 Transaktioner med deposit/withdraw/transfer (decimal, ej direkt saldojustering)

🚨 Anti Money Laundering-modul (se nedan)

💾 Rapportgenerering (.txt) för misstänksamma transaktioner

🧠 Automapper, validering, clean arch

🔐 Inloggning

Roll

E-post

Lösenord

Admin

richard.chalk@admin.se

Abc123#

Cashier

richard.chalk@cashier.se

Abc123#

🧪 AML Console App – Anti Money Laundering

Verktyget körs separat från webben men återanvänder samma tjänstelager (services/DAL).

🔍 Regler:

Transaktion över 15 000 kr

Transaktionssumma > 23 000 kr inom 72h

📄 Output:

Skapar .txt-rapport per land (datumstämpel)

Körs per land och sparar endast nya misstänkta transaktioner

Lagrar körningshistorik för att undvika dubbletter

▶️ Exempel på körning:

dotnet run --project NordicBank.Console

📊 Startsida (publik)

Antal kunder, konton och saldo per land

Klicka på ett land → visas top 10 kunder i det landet

Den sidan är response-cachad i 60 sekunder

🛠️ Kom igång lokalt

✅ Krav

.NET 9

SQL Server

Visual Studio / VS Code

⚙️ Så här gör du:

# (Om databas ej är skapad ännu)
dotnet ef database update

# Starta webben
dotnet run --project NordicBank

🚀 Kör AML-verktyget manuellt

dotnet run --project NordicBank.Console

🧹 Teknologier

ASP.NET Core 8 (Razor Pages)

Entity Framework Core (Database First)

AutoMapper

Chart.js

Bootstrap 5

Identity + Role management

LINQ & LINQ-to-EF

Clean code-principer


🔮 Påbyggnad & Vidareutveckling

Detta projekt är välstrukturerat men har stor potential för vidare funktioner:

🧾 PDF-export av rapporter – Lägg till PDF-export via t.ex. DinkToPdf.

💬 Notis/meddelandesystem – Låt admins/cashiers lämna interna kommentarer per kund.

📈 Utökad analysdashboard – Mer avancerad statistik och filtreringsmöjligheter.

📱 Responsiv mobilvy – Gör mobilanvändning ännu smidigare.

📬 Mailnotifiering för flaggade transaktioner – Skicka e-post vid AML-träff.

