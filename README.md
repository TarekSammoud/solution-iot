# Plateforme de Gestion d'Objets Connectés IoT

Système de gestion d'objets connectés développé avec .NET 9.0 et suivant les principes de **Clean Architecture**.

## Architecture du Projet

Le projet est organisé en plusieurs couches pour assurer une séparation claire des responsabilités :

```
SolutionIoT/
├── src/
│   ├── Domain/                      # Couche Domain (Entités, Enums, Interfaces)
│   ├── Application/                 # Couche Application (DTOs, Services)
│   ├── Infrastructure/              # Couche Infrastructure (DbContext, Repositories)
│   ├── Presentation.API/            # API REST
│   └── Presentation.Web/            # Interface Blazor
│       ├── Presentation.Web.Server/ # Blazor Server
│       └── Presentation.Web.Client/ # Blazor WebAssembly
```

### Couche Domain
- **Entités** : Sonde, Releve, Utilisateur
- **Enums** : TypeSonde, UniteReleve
- **Interfaces** : Repositories (ISondeRepository, IReleveRepository, IUtilisateurRepository)

### Couche Application
- **DTOs** : SondeDto, ReleveDto, UtilisateurDto
- **Services** : SondeService, ReleveService avec validation et mapping

### Couche Infrastructure
- **DbContext** : Configuration Entity Framework Core avec SQLite
- **Repositories** : Implémentation des interfaces avec EF Core
- **Initialisation** : DbInitializer avec données de test

### Couche Presentation
- **API REST** : Contrôleurs avec gestion d'erreurs et documentation Swagger
- **Blazor Server/WebAssembly** : Interface utilisateur moderne et responsive

## Technologies Utilisées

- **.NET 9.0**
- **Blazor Server & WebAssembly**
- **Entity Framework Core 9.0**
- **SQLite** (base de données)
- **ASP.NET Core Web API**
- **Swagger/OpenAPI** (documentation API)

## Prérequis

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Un éditeur de code (Visual Studio 2022, VS Code, Rider)

## Installation et Démarrage

### 1. Restaurer les packages NuGet

```bash
dotnet restore
```

### 2. Créer la base de données et appliquer les migrations

```bash
cd src/Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../Presentation.API
dotnet ef database update --startup-project ../Presentation.API
```

### 3. Lancer l'API

```bash
cd src/Presentation.API
dotnet run
```

L'API sera disponible à : `https://localhost:7001`
La documentation Swagger sera accessible à : `https://localhost:7001/swagger`

### 4. Lancer l'interface Blazor (dans un nouveau terminal)

```bash
cd src/Presentation.Web/Presentation.Web.Server
dotnet run
```

L'interface web sera disponible à : `https://localhost:7002`

## Fonctionnalités

### Gestion des Sondes
- ✓ Créer une nouvelle sonde
- ✓ Lister toutes les sondes
- ✓ Modifier une sonde existante
- ✓ Supprimer une sonde
- ✓ Visualiser les détails d'une sonde

### Types de Sondes Supportés
- Température
- Hydrométrie
- Qualité de l'air
- Pression
- Luminosité

### Tableau de Bord
- Statistiques en temps réel
- Nombre total de sondes
- Sondes actives/inactives

## Structure de la Base de Données

### Table Sondes
- Id (int, clé primaire)
- Nom (string)
- Type (enum TypeSonde)
- Localisation (string)
- EstActive (bool)
- DateInstallation (DateTime)

### Table Releves
- Id (int, clé primaire)
- SondeId (int, clé étrangère)
- Valeur (decimal)
- Unite (enum UniteReleve)
- DateReleve (DateTime)

### Table Utilisateurs
- Id (int, clé primaire)
- Nom (string)
- Email (string, unique)
- MotDePasseHash (string)
- Role (string)

## API Endpoints

### Sondes

- `GET /api/sondes` - Récupérer toutes les sondes
- `GET /api/sondes/{id}` - Récupérer une sonde par ID
- `POST /api/sondes` - Créer une nouvelle sonde
- `PUT /api/sondes/{id}` - Mettre à jour une sonde
- `DELETE /api/sondes/{id}` - Supprimer une sonde

## Configuration

### API (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=iot.db"
  }
}
```

### Blazor Server (Program.cs)
Le HttpClient est configuré pour communiquer avec l'API :
```csharp
BaseAddress = new Uri("https://localhost:7001/")
```

## Déploiement

### Build pour production

```bash
dotnet publish -c Release
```

### Docker (optionnel)

Un Dockerfile pourra être ajouté ultérieurement pour faciliter le déploiement en conteneur.

## Développement Futur

- [ ] Authentification et autorisation
- [ ] Gestion des relevés avec graphiques
- [ ] Notifications en temps réel (SignalR)
- [ ] Export de données (CSV, Excel)
- [ ] Tests unitaires et d'intégration
- [ ] CI/CD avec GitHub Actions

## Contribution

Les contributions sont les bienvenues. Veuillez créer une issue ou une pull request pour toute amélioration.

## Licence

Ce projet est développé à des fins éducatives.

## Support

Pour toute question ou assistance, veuillez contacter l'équipe de développement.
