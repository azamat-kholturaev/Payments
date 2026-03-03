# Payments API

Бэкенд-сервис для обработки платежей по заказам с упором на **надежность, идемпотентность и чистую архитектуру**.

---

## 1. Что делает сервис

Сервис покрывает полный базовый платежный флоу:
- регистрация и логин пользователя (JWT),
- создание заказа,
- создание платежа для заказа,
- подтверждение платежа через абстракцию провайдера,
- контроль владельца сущностей (order/payment ownership),
- идемпотентность команд через `Idempotency-Key`.

Типовой сценарий:
1. Пользователь регистрируется/логинится и получает JWT.
2. Создает заказ (статус `Created`).
3. Создает платеж по заказу (статус `Pending`) с `Idempotency-Key`.
4. Подтверждает платеж с `Idempotency-Key`.
5. При успехе провайдера: платеж `Successful`, заказ `Paid`.

---

## 2. Архитектура и границы слоев

Проект организован по принципам **Clean Architecture + CQRS**:

- `Payments.API`
  - REST-контроллеры, middleware, auth, rate limit, OpenAPI/Swagger.
- `Payments.Application`
  - use-cases (команды/запросы), MediatR handlers, валидация, транзакционный pipeline.
- `Payments.Domain`
  - доменные сущности, value objects, инварианты, state transition rules.
- `Payments.Infrastructure`
  - EF Core/PostgreSQL, репозитории, JWT/BCrypt, idempotency-store, провайдер платежей.
- `Payments.Contracts`
  - API-контракты запросов/ответов.

---

## 3. Технологии

- **.NET 10** (`net10.0`),
- **ASP.NET Core Web API**,
- **MediatR** (CQRS-диспетчеризация),
- **FluentValidation**,
- **EF Core 10 + Npgsql**,
- **JWT Bearer Authentication**,
- **BCrypt** для хеширования паролей,
- **MemoryCache** для справочника валют,
- **OpenAPI + Swagger UI + Scalar**,
- **Docker Compose** для локального запуска.

---

## 4. Функционал API

## Auth
- `POST /api/auth/register`
  - валидация email/пароля,
  - проверка уникальности email,
  - хеширование пароля,
  - возврат JWT.
- `POST /api/auth/login`
  - проверка учетных данных,
  - возврат JWT.

## Orders
- `POST /api/orders`
  - требуется JWT,
  - валидация суммы/валюты,
  - проверка поддерживаемой валюты,
  - создание заказа в статусе `Created`.
- `GET /api/orders/{orderId}`
  - доступ только владельцу,
  - получение данных и статуса заказа.

## Payments
- `POST /api/orders/{orderId}/payments`
  - обязателен заголовок `Idempotency-Key`,
  - проверка владельца и возможности оплаты,
  - создание платежа `Pending`,
  - сохранение snapshot-ответа для идемпотентного повтора.
- `GET /api/orders/{orderId}/payments`
  - доступ только владельцу,
  - список платежей заказа.
- `POST /api/payments/{paymentId}/confirm`
  - обязателен `Idempotency-Key`,
  - подтверждение у платежного провайдера,
  - корректная обработка кейса «заказ уже оплачен»,
  - повтор запроса с тем же ключом возвращает стабильно тот же результат.

---

## 5. Доменные состояния и инварианты

## Order
Состояния:
- `Created`
- `Paid`
- `Cancelled`

Правила:
- только `Created` может перейти в `Paid`,
- оплаченный заказ нельзя отменить.

## Payment
Состояния:
- `Pending`
- `Successful`
- `Failed`

Правила:
- только `Pending` может перейти в `Successful`/`Failed`,
- повторное подтверждение уже успешного платежа не ломает консистентность.

---

## 6. Идемпотентность и консистентность БД

Реализованы несколько уровней защиты:

1. **Application-level replay**
   - ответ команды сохраняется в idempotency-store по `(UserId, Key, Scope)`.
2. **Уникальные индексы в БД**
   - уникальность платежа по `(OrderId, IdempotencyKey)`,
   - уникальность записи replay по `(UserId, Key, Scope)`.
3. **Защита от двойной оплаты заказа**
   - partial unique index на «один успешный платеж на заказ».
4. **Транзакционное выполнение команд**
   - команды оборачиваются в `TransactionBehavior`,
   - изменения order/payment фиксируются атомарно.

---

## 7. Безопасность и устойчивость

- JWT с валидацией issuer/audience/signature/lifetime,
- извлечение текущего пользователя из claims,
- owner-check в use-case логике,
- глобальная нормализация ошибок в `ProblemDetails`,
- rate limiting:
  - публичные endpoint'ы — лимит по IP,
  - приватные команды/запросы — лимит по пользователю.

---

## 8. Pipeline обработки запроса (CQRS)

1. Контроллер принимает HTTP-запрос.
2. Отправляет command/query в MediatR.
3. `ValidationBehavior` запускает FluentValidation.
4. `TransactionBehavior` открывает транзакцию для команд.
5. Handler выполняет бизнес-логику.
6. Репозитории/UnitOfWork сохраняют изменения.
7. Результат возвращается как HTTP-ответ.

---

## 9. Запуск локально

## Вариант A (рекомендуется): Docker

```bash
docker compose up --build
```

Swagger:
- `http://localhost:8080/swagger`

## Вариант B: локально через dotnet

1. Поднять PostgreSQL с параметрами из `appsettings.json`.
2. Проверить `ConnectionStrings:ConnectionString`.
3. Выполнить:

```bash
dotnet restore
dotnet build
dotnet run --project Payments.API
```

---

## 10. Быстрый E2E сценарий (curl)

## 1) Register
```bash
curl -X POST http://localhost:8080/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"senior@example.com","password":"Strong#Pass1"}'
```

## 2) Create order
```bash
curl -X POST http://localhost:8080/api/orders \
  -H "Authorization: Bearer <JWT>" \
  -H "Content-Type: application/json" \
  -d '{"amount":100.50,"currency":"USD"}'
```

## 3) Create payment
```bash
curl -X POST http://localhost:8080/api/orders/<ORDER_ID>/payments \
  -H "Authorization: Bearer <JWT>" \
  -H "Idempotency-Key: create-pay-001"
```

## 4) Confirm payment
```bash
curl -X POST http://localhost:8080/api/payments/<PAYMENT_ID>/confirm \
  -H "Authorization: Bearer <JWT>" \
  -H "Idempotency-Key: confirm-pay-001"
```

---

## 11. Реализованные best practices

- тонкие контроллеры, толстая use-case/domain логика,
- доменные инварианты в сущностях,
- CQRS + pipeline behaviors,
- транзакционность команд,
- идемпотентность write-операций,
- единый error-contract (`ProblemDetails`),
- защита от double-charge на уровне БД + домена.

---


## 12. Операционные заметки

- Для create/confirm платежа `Idempotency-Key` обязателен.
- Поддерживаемые валюты сидируются в БД (`USD`, `EUR`, `TJS`).
- В текущей реализации миграции применяются автоматически при старте.
- Клиент провайдера сейчас заглушка (симуляция подтверждения).
