# HomeLibrary

Домашняя библиотека на ASP.NET Core 8 + PostgreSQL.

## Стек

- **.NET 8**, ASP.NET Core MVC
- **PostgreSQL 16**
- **EF Core 8** + **Dapper**
- **Quill** — HTML-редактор оглавления
- **Docker**

## Архитектура

- `HomeLibrary.Domain` — сущности, интерфейсы
- `HomeLibrary.Application` — use cases, DTO
- `HomeLibrary.Infrastructure` — EF Core, репозитории, миграции
- `HomeLibrary.Web` — контроллеры, представления

## Запуск через Docker

```bash
docker compose up --build

После запуска приложение по умолчанию доступно по http://localhost:8080/
