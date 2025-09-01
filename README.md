![Docker Image Version](https://img.shields.io/docker/v/blueclikk/kwik-nesta.property.svc?sort=semver&label=version)
![Docker Pulls](https://img.shields.io/docker/pulls/blueclikk/kwik-nesta.property.svc.svc)


# 🏢 Kwik Nesta Property Management Service

The Property Management Service is a backend microservice designed to handle property-related operations for real estate and rental management systems. 
It enables easy management of properties, tenants, leases, and maintenance requests, providing APIs for CRUD operations and seamless integration into larger platforms.

---

## 🚀 Features

- 🏠 Property Management
  - Create, update, view, and archive properties
  - Support for categories (residential, commercial, etc.)

- 👥 Tenant Management
  - Manage tenant profiles and records
  - Assign tenants to properties or units

- 📜 Lease Management
  - Create and track lease agreements
  - Manage lease terms, rent schedules, and status

- 🛠️ Maintenance Management
  - Log and track maintenance requests
  - Update request status (pending, in-progress, resolved)

- 📊 Reports & Audit Logs
  - Activity logging for accountability
  - Exportable data for business insights

---

## ⚙️ Tech Stack
- .NET 8 (ASP.NET Core Web API)
- Entity Framework Core (Data Access)
- SQL Database (Persistence)
- Swagger / OpenAPI (API Documentation)

---

## ⚡ gRPC Integration

This service depends on `SystemSupportService` to fetch location details.

```csharp
// Example registration in Program.cs
builder.Services.AddGrpcClient<LocationService.LocationServiceClient>(o =>
{
    o.Address = new Uri("https://system-support-service:5001"); // Adjust per environment
});
```

## 📜 License

MIT License © Kwik Nesta