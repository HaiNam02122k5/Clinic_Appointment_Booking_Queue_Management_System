# Backend Common Structure

Khung backend nay tham khao `AgriTrace-Backend-feature-CleanArchitechture`, chi chot cau truc chung. Chua chot `.NET`, chua tao `.csproj`, chua them package, chua cau hinh database.

## Directory Structure

```text
backend/                                      # Thu muc backend chinh
├── Clinic.API/                              # Tang giao tiep HTTP/API
│   ├── Controllers/                         # Xu ly endpoint va route
│   ├── Models/                              # Request/Response model cho client
│   ├── Common/                              # Middleware, filter, response chung
│   └── Mappings/                            # Mapping giua API model va Application
├── Clinic.Domain/                           # Tang loi nghiep vu
│   ├── Entities/                            # Thuc the nghiep vu
│   ├── Interfaces/                          # Hop dong repository/service
│   ├── Services/                            # Domain service va rule phuc tap
│   └── Common/                              # Base entity, value object chung
├── Clinic.Application/                      # Tang use case
│   ├── Contracts/                           # DTO/contract tra ve tu use case
│   ├── Features/                            # CQRS command/query theo tinh nang
│   ├── Mappings/                            # Mapping noi bo Application
│   └── Common/                              # Thanh phan dung chung Application
│       └── Exceptions/                      # Exception nghiep vu/use case
└── Clinic.Infrastructure.Sqlserver/         # Tang ha tang SQL Server
    ├── Persistence/                         # DbContext va cau hinh ket noi DB
    ├── Configurations/                      # Fluent API mapping table
    ├── Models/                              # Data model gan voi database
    └── Repositories/                        # Implement repository tu Domain
```

## Layer Rules

- `Clinic.Domain`: chua entity, interface, domain service, rule nghiep vu loi. Khong phu thuoc layer khac.
- `Clinic.Application`: chua use case, DTO/contract, CQRS command/query, mapping noi bo. Chi phu thuoc `Domain`.
- `Clinic.Infrastructure.Sqlserver`: chua EF/database model, DbContext, repository implementation. Phu thuoc `Domain`.
- `Clinic.API`: chua HTTP controller, request/response model, API mapping, middleware/common response. Phu thuoc `Application` va goi DI cua `Infrastructure` khi team tao project.

## Dependency Direction

```text
Clinic.API
  ├── Clinic.Application ──► Clinic.Domain
  └── Clinic.Infrastructure.Sqlserver ──► Clinic.Domain
```

Domain la loi trung tam. Domain khong reference API, Application, Infrastructure.

## CQRS Flow

```text
Client
  -> API Request Model
  -> Application Command/Query
  -> Domain Entity/Service
  -> Repository Interface
  -> Infrastructure Repository/Data Model
  -> Application DTO
  -> API Response Model
  -> Client
```

## Team Notes

- Moi thanh vien tu cau hinh `.NET`, package va project file theo nhanh rieng cho den khi nhom chot version.
- Khi chot framework, tao solution/project theo prefix tam thoi `Clinic`.
- Khong dua logic nghiep vu AgriTrace vao backend nay.
