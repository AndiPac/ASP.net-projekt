# VetAmb Development SKILL

---

## Metadata

- **Project**: VetAmb - Veterinary Clinic Management System
- **Framework**: ASP.NET Core (MVC/Razor Pages)
- **UI Design**: Modern Glassmorphism Style
- **Language**: C# 11+
- **Target**: .NET 8.0
- **Repository**: C:\git\VetAmb

---

## Mandatory Development Rules

### 1. Namespace Requirements

**All new code MUST use the following namespace structure:**

```csharp
// Models and Data Access Layer
namespace VetAmb.Models { }
namespace VetAmb.Data { }

// Controllers and Views
namespace VetAmb.Controllers { }

// Repositories
namespace VetAmb.Repositories { }

// Business Logic / Services
namespace VetAmb.Services { }

// Utilities
namespace VetAmb.Utilities { }
```

**Enforcement Rule**: 
- Every new class must explicitly declare `namespace VetAmb.*`
- No classes allowed outside these namespaces
- Exception: Auto-generated code and configuration files

**Example**:
```csharp
using System;
using System.Collections.Generic;

#nullable enable

namespace VetAmb.Models
{
    public class MyNewEntity
    {
        public int Id { get; set; }
        // Implementation
    }
}

namespace VetAmb.Repositories
{
    public class MyRepository : IMyRepository
    {
        public MyRepository(VetAmbContext context)
        {
            // Implementation
        }
    }
}
```

### 2. Data Layer Requirements

**All data access must use the VetAmb.Data namespace:**

- Database context class: `VetAmbContext` (in `VetAmb.Data` namespace)
- All DbSet properties must represent model entities
- Use Entity Framework Core for data operations
- Foreign key naming convention: `{EntityName}Id`

**Example**:
```csharp
namespace VetAmb.Data
{
    public class VetAmbContext : DbContext
    {
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Vet> Vets { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<AppointmentService> AppointmentServices { get; set; }

        public VetAmbContext(DbContextOptions<VetAmbContext> options)
            : base(options) { }
    }
}
```

### 3. UI Style Requirements - Modern Glassmorphism

**MANDATORY**: All UI must maintain the Modern Glassmorphism design language.

#### Core Design Principles

1. **Color Palette**:
   ```css
   /* Primary Colors */
   --primary-blue: #0066ff;
   --primary-purple: #7c3aed;
   --primary-cyan: #06b6d4;
   
   /* Glass Effect */
   --glass-bg: rgba(255, 255, 255, 0.1);
   --glass-border: rgba(255, 255, 255, 0.2);
   
   /* Background Gradient */
   --gradient-start: #0f0c29;
   --gradient-middle: #302b63;
   --gradient-end: #24243e;
   ```

2. **Glass Effect CSS Classes**:
   ```css
   .glass-panel {
       background: rgba(255, 255, 255, 0.1);
       backdrop-filter: blur(10px);
       -webkit-backdrop-filter: blur(10px);
       border: 1px solid rgba(255, 255, 255, 0.2);
       border-radius: 10px;
       padding: 20px;
       box-shadow: 0 8px 32px 0 rgba(31, 38, 135, 0.37);
   }

   .glass-card {
       background: rgba(255, 255, 255, 0.05);
       backdrop-filter: blur(15px);
       border: 1px solid rgba(255, 255, 255, 0.18);
       border-radius: 12px;
       padding: 24px;
       transition: all 0.3s ease;
   }

   .glass-card:hover {
       background: rgba(255, 255, 255, 0.08);
       border-color: rgba(255, 255, 255, 0.3);
       transform: translateY(-2px);
       box-shadow: 0 12px 40px 0 rgba(31, 38, 135, 0.45);
   }

   .glass-input {
       background: rgba(255, 255, 255, 0.05);
       border: 1px solid rgba(255, 255, 255, 0.2);
       border-radius: 8px;
       padding: 10px 15px;
       color: #ffffff;
       backdrop-filter: blur(10px);
   }

   .glass-input::placeholder {
       color: rgba(255, 255, 255, 0.6);
   }

   .glass-input:focus {
       background: rgba(255, 255, 255, 0.1);
       border-color: #0066ff;
       outline: none;
       box-shadow: 0 0 20px rgba(0, 102, 255, 0.3);
   }

   .glass-button {
       background: linear-gradient(135deg, #0066ff, #7c3aed);
       border: 1px solid rgba(255, 255, 255, 0.2);
       color: white;
       padding: 12px 28px;
       border-radius: 8px;
       cursor: pointer;
       transition: all 0.3s ease;
       font-weight: 600;
   }

   .glass-button:hover {
       background: linear-gradient(135deg, #0052cc, #6d28d9);
       box-shadow: 0 8px 25px rgba(0, 102, 255, 0.4);
       transform: translateY(-2px);
   }

   .glass-button:active {
       transform: translateY(0);
   }
   ```

3. **Responsive Grid System**:
   ```css
   .glass-container {
       display: grid;
       grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
       gap: 20px;
       padding: 20px;
   }

   @media (max-width: 768px) {
       .glass-container {
           grid-template-columns: 1fr;
           gap: 15px;
       }
   }
   ```

4. **Typography**:
   - Primary Font: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif
   - Headers: Bold, 28-36px
   - Body: Regular, 14-16px
   - Color: #ffffff with opacity variations

5. **Animation Standards**:
   ```css
   /* Smooth transitions */
   * {
       transition: background-color 0.3s, border-color 0.3s, color 0.3s;
   }

   /* Fade in animation */
   @keyframes fadeIn {
       from {
           opacity: 0;
           transform: translateY(10px);
       }
       to {
           opacity: 1;
           transform: translateY(0);
       }
   }

   .glass-element {
       animation: fadeIn 0.5s ease-out;
   }
   ```

#### Implementation Requirements

- **Navigation Bar**: Glassmorphic header with semi-transparent background
- **Forms**: All input fields must use glass-input class with blur effect
- **Buttons**: Gradient buttons with hover/active states
- **Cards**: Glass cards for content containers (clinics, patients, vets, etc.)
- **Modals**: Semi-transparent backdrop with glass panel
- **Error Messages**: Glass alerts with appropriate color coding
- **Success Messages**: Green-tinted glass alerts

#### Example View Structure (Razor)

```html
<!-- _Layout.cshtml -->
<nav class="glass-nav">
    <div class="glass-logo">VetAmb</div>
    <ul class="nav-menu">
        <li><a href="/" class="nav-link">Početna</a></li>
        <li><a href="/klinike" class="nav-link">Klinike</a></li>
        <li><a href="/vlasnici" class="nav-link">Vlasnici</a></li>
    </ul>
</nav>

<!-- Content Area -->
<main class="glass-container">
    @RenderBody()
</main>

<!-- Index.cshtml (Example) -->
<div class="glass-card">
    <h1>Sve klinike</h1>
    <div class="glass-container">
        @foreach (var clinic in Model)
        {
            <article class="glass-panel">
                <h3>@clinic.Name</h3>
                <p>@clinic.Address</p>
                <p>📞 @clinic.Phone</p>
                <button class="glass-button">Više info</button>
            </article>
        }
    </div>
</div>

<!-- Form Example -->
<form method="post" class="glass-form">
    <div class="form-group">
        <label for="name">Naziv klinike</label>
        <input type="text" id="name" name="Name" class="glass-input" placeholder="Unesite naziv..." required>
    </div>
    <button type="submit" class="glass-button">Spremi</button>
    <button type="reset" class="glass-button-secondary">Otkaži</button>
</form>
```

### 4. Model Development Standards

**All models must:**

```csharp
// Use nullable reference types
#nullable enable

namespace VetAmb.Models
{
    /// <summary>
    /// Entity summary documentation
    /// </summary>
    public class Entity
    {
        // Primary Key
        public int Id { get; set; }

        // Properties
        public string? Property { get; set; }
        public DateTime CreatedDate { get; set; }

        // Foreign Keys
        public int RelatedEntityId { get; set; }
        public RelatedEntity? RelatedEntity { get; set; }

        // Navigation Collections
        public ICollection<ChildEntity> ChildEntities { get; set; } = new List<ChildEntity>();

        // Validation & Constraints
        [Required]
        [StringLength(100)]
        public string RequiredProperty { get; set; } = string.Empty;
    }
}
```

### 5. Repository Pattern Requirements

**All data access must use repositories:**

```csharp
namespace VetAmb.Repositories
{
    public interface IEntityRepository
    {
        Task<IEnumerable<Entity>> GetAllAsync();
        Task<Entity?> GetByIdAsync(int id);
        Task AddAsync(Entity entity);
        Task UpdateAsync(Entity entity);
        Task DeleteAsync(int id);
    }

    public class EntityRepository : IEntityRepository
    {
        private readonly VetAmbContext _context;

        public EntityRepository(VetAmbContext context)
        {
            _context = context;
        }

        // Implementation
    }
}
```

### 6. Controller Standards

```csharp
namespace VetAmb.Controllers
{
    [Route("[controller]")]
    [ApiController] // or just [Controller] for MVC
    public class EntityController : Controller
    {
        private readonly IEntityRepository _repository;

        public EntityController(IEntityRepository repository)
        {
            _repository = repository;
        }

        // Actions
    }
}
```

### 7. Validation Rules

- Use Data Annotations for model validation
- Implement server-side validation in repositories/services
- Return meaningful error messages to users
- Log validation errors

```csharp
public class Entity
{
    [Required(ErrorMessage = "Polje je obavezno")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Age { get; set; }

    [EmailAddress]
    public string? Email { get; set; }
}
```

### 8. Dependency Injection Configuration

**In Program.cs:**

```csharp
builder.Services.AddScoped<IClinicRepository, MockClinicRepository>();
builder.Services.AddScoped<IOwnerRepository, MockOwnerRepository>();
builder.Services.AddScoped<IPatientRepository, MockPatientRepository>();
builder.Services.AddScoped<IVetRepository, MockVetRepository>();
builder.Services.AddScoped<IAppointmentRepository, MockAppointmentRepository>();
builder.Services.AddScoped<IServiceRepository, MockServiceRepository>();
builder.Services.AddScoped<IMedicalRecordRepository, MockMedicalRecordRepository>();
```

### 9. Localization Support

- All UI strings should support multiple languages (en, hr, es, fr, de, etc.)
- Use resource files (.resx) for translations
- Implement language selector in navigation
- Route patterns should support culture prefix: `/{culture}/{controller}/{action}`

### 10. Code Quality Standards

- Use XML documentation comments for public APIs
- Maintain 80+ code coverage for critical business logic
- Follow Microsoft C# Coding Conventions
- Use const/readonly for immutable values
- Avoid magic strings - use constants or enums

---

## Project File Structure

```
VetAmb/
├── Models/           (All domain entities - VetAmb.Models)
├── Controllers/      (MVC Controllers - VetAmb.Controllers)
├── Views/            (Razor views with Glassmorphism styling)
├── Repositories/     (Data access - VetAmb.Repositories)
├── Services/         (Business logic - VetAmb.Services)
├── Data/             (DbContext - VetAmb.Data)
├── wwwroot/
│   ├── css/
│   │   └── site.css  (Glass effect styles)
│   └── js/
├── Pages/            (Razor Pages if used)
├── Program.cs        (Configuration & DI)
└── VetAmb.csproj
```

---

## Checklist for New Features

- [ ] Model created in `VetAmb.Models` with documentation
- [ ] Repository interface in `VetAmb.Repositories`
- [ ] Repository implementation in `VetAmb.Repositories`
- [ ] Dependency injected in `Program.cs`
- [ ] Controller created in `VetAmb.Controllers`
- [ ] Views follow Glassmorphism design
- [ ] Validation rules added to model
- [ ] Error handling implemented
- [ ] Unit tests written (if applicable)
- [ ] Code documented with XML comments
- [ ] Routing configured correctly

---

## Violation Penalties

❌ **WILL NOT BE ACCEPTED**:
- Code outside `VetAmb.*` namespaces
- UI that doesn't follow Glassmorphism design
- Hard-coded strings without localization
- Missing XML documentation on public APIs
- Incomplete error handling
- Data access outside Repository pattern

---

## Reference Files

- Semantic Model: `semantic-model.md`
- Routes & Navigation: `sitemap.md`
- Existing Models: `Models/`
- Existing Repositories: `Repositories/`
- UI Styles: `wwwroot/css/site.css`

---

**Last Updated**: May 8, 2026  
**Version**: 1.0  
**Maintainer**: VetAmb Development Team
