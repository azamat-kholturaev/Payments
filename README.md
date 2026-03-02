# Payments API (мини-платёжка)

REST API, где пользователь создаёт заказ, делает попытки оплаты и подтверждает платёж; при первом успешном подтверждении заказ становится `paid`, а повторная успешная оплата исключается даже при гонках.

## Архитектура

- `Payments.API` — HTTP-контроллеры, JWT авторизация, rate limiting, глобальный exception pipeline (`IExceptionHandler` + `ProblemDetails`) (без `try/catch` в каждом endpoint).
- `Payments.Application` — use-cases и бизнес-правила (MediatR + handlers).
- `Payments.Domain` — сущности и инварианты (`Order`, `Payment`, `User`).
- `Payments.Infrastructure` — EF Core/PostgreSQL, репозитории, JWT генерация, idempotency store.

## Основные гарантии корректности

### Защита от двойной оплаты

`POST /api/payments/{paymentId}/confirm`:

1. Обрабатывается в транзакции (pipeline `TransactionBehavior`).
2. Забирает `Order` через `SELECT ... FOR UPDATE`.
3. Если заказ уже `paid` → `409 order.not_payable`.
4. При успехе провайдера атомарно:
   - `Payment.status = successful`
   - `Order.status = paid`
5. На БД есть частичный уникальный индекс: только один `successful` на `order_id`.

Итого: есть и блокировка строки, и unique-барьер в БД.

### Идемпотентность

Для POST-операций используется заголовок `Idempotency-Key`:
- `POST /api/orders/{orderId}/payments`
- `POST /api/payments/{paymentId}/confirm`

Схема хранения: `idempotency(user_id, key, scope, status_code, response_json, created_at, expires_at)`.

Повторный запрос с тем же ключом и scope возвращает сохранённый ответ.

## Data model

### User
- `id`
- `email` (unique)
- `password_hash`
- `created_at`

### Order
- `id`
- `user_id`
- `amount`
- `currency`
- `status`: `created | paid | cancelled`

### Payment
- `id`
- `order_id`
- `user_id`
- `amount`
- `currency`
- `status`: `pending | successful | failed`
- `created_at`
- `idempotency_key`
- `provider_payment_id`
- `failure_reason`

## API

### Auth
- `POST /api/auth/register`
- `POST /api/auth/login`

### Orders
- `POST /api/orders`
- `GET /api/orders/{orderId}`

### Payments
- `POST /api/orders/{orderId}/payments`
- `GET /api/orders/{orderId}/payments`
- `POST /api/payments/{paymentId}/confirm`

## Единый формат ошибок

Сервис возвращает `ProblemDetails` (RFC 7807), например:

```json
{
  "type": "order.not_payable",
  "title": "Business error",
  "status": 409,
  "detail": "Order already paid",
  "instance": "/api/payments/{paymentId}/confirm"
}
```

## Rate limiting

Для подтверждения платежа включён отдельный policy `payments-strict` (жёсткий лимит).

## Запуск

1. Поднять PostgreSQL.
2. Обновить connection string в `Payments.API/appsettings*.json` при необходимости.
3. Запустить:

```bash
dotnet restore
dotnet build Payments.slnx
dotnet run --project Payments.API/Payments.API.csproj
```
