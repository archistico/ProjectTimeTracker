# ProjectTimeTracker

Applicazione per il tracciamento dei tempi di lavoro sui progetti (API + Web UI).

## Panoramica

Repository composto da tre progetti principali:
- `src/ProjectTimeTracker.Api` - API REST in ASP.NET Core
- `src/ProjectTimeTracker.Web` - Web application (Razor Pages)
- `tests/ProjectTimeTracker.Api.Tests` - Test automatici (xUnit)

Tutto il codice è targettizzato su .NET 8.

## Prerequisiti

- .NET 8 SDK
- (Per esecuzione locale dell'API) Un provider di database configurato — di default il progetto usa SQLite tramite la stringa di connessione `DefaultConnection`.

## Eseguire il progetto

1. Clona la repository e posizionati nella cartella del codice:

```bash
git clone <repo-url>
cd <repository-root>
```

2. Buildare la soluzione:

```bash
dotnet build
```

3. Avviare l'API (cartella `src/ProjectTimeTracker.Api`):

```bash
cd src/ProjectTimeTracker.Api
dotnet run
```

L'API legge la stringa di connessione `DefaultConnection` da `appsettings.json`. Per sviluppo locale puoi usare SQLite (es. `Data Source=projecttimetracker.db`).

4. Avviare la Web UI (cartella `src/ProjectTimeTracker.Web`):

```bash
cd src/ProjectTimeTracker.Web
dotnet run
```

La Web UI è una Razor Pages app che interagisce con l'API.

## Test

Eseguire tutti i test con:

```bash
dotnet test
```

Il progetto di test `tests/ProjectTimeTracker.Api.Tests` contiene test unitari e di integrazione (usano TestServer / in-memory database o configurazione di test dedicata).

## Seed dei dati

Lo step di inizializzazione del database e del seed è implementato nell'API (vedi `src/ProjectTimeTracker.Api/Data/SeedData.cs`). In ambiente di sviluppo i test possono usare un database in memoria o un database SQLite temporaneo per ripristinare lo stato iniziale.

## Struttura del codice

- `Controllers/` - Endpoint REST
- `Services/` - Logica applicativa e accesso ai dati
- `Data/` - DbContext ed entità
- `Dtos/`, `Models/` - DTO e model classes
- `Middleware/` - gestione errori globali

## Contribuire

1. Fork del repository
2. Crea un branch per la feature/fix: `git checkout -b feature/nome-feature`
3. Implementa e aggiungi test
4. Apri una Pull Request verso `main`

## Licenza

Controlla il file `LICENSE` nella root del repository per i dettagli sulla licenza.

---
Per qualsiasi dettaglio tecnico o se vuoi che aggiunga istruzioni più specifiche per l'ambiente di sviluppo (es. Docker, variabili d'ambiente, CI), dimmelo e aggiorno il README.
