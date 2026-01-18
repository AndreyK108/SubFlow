# DB — Модель данных SubFlow (ER)

## 1. Цели модели данных
SubFlow хранит:
- подписки/сервисы (карточки, категории, владельцы, лицензии),
- параметры биллинга (период, сумма, валюта, даты списаний),
- напоминания (правила за N дней до события),
- привязку Telegram (токен/чат),
- B2C (личные данные пользователя) и упрощённый B2B (организация + роли).

Календарные события (списания/окончания/trial) НЕ обязаны храниться в БД: они вычисляются динамически из данных подписок.

## 2. Принцип разделения данных (B2C + B2B)
- Для B2C: данные принадлежат конкретному пользователю (UserId).
- Для B2B: данные принадлежат организации (OrganizationId) + пользователю (кто создал/владеет).
- В таблицах доменных сущностей используем:
  - OrganizationId (nullable) — если NULL, знать что это “личное” (B2C).
  - OwnerUserId — владелец записи (кто видит/управляет в личном режиме; в B2B используется для аудита и персональных прав).

Важно: доступ к данным определяется правилами авторизации (см. ADR по ролям).

## 3. Сущности и атрибуты (основной минимум)

### 3.1 Organization (Организация)
- Id (PK, GUID/INT)
- Name
- CreatedAt

### 3.2 OrganizationMember (Участник организации)
- Id (PK)
- OrganizationId (FK -> Organization)
- UserId (FK -> AspNetUsers)
- Role (enum/string: Admin/User)
- CreatedAt

Связи:
- Organization 1 — M OrganizationMember
- User 1 — M OrganizationMember

### 3.3 Category (Категория)
- Id (PK)
- OrganizationId (nullable FK -> Organization)
- OwnerUserId (FK -> AspNetUsers)
- Name
- Color (optional)
- SortOrder (optional)

Связи:
- Category M — 1 User (OwnerUserId)
- Category M — 0..1 Organization

### 3.4 Service (Сервис/Провайдер)
Это “что за сервис”: Netflix, Figma, GitHub, etc.
- Id (PK)
- Name
- WebsiteUrl (optional)
- LogoUrl (optional)

Связи:
- Service 1 — M Subscription

### 3.5 Subscription (Подписка)
Основная сущность карточки подписки.
- Id (PK)
- OrganizationId (nullable FK -> Organization)
- OwnerUserId (FK -> AspNetUsers)

- ServiceId (FK -> Service)
- CategoryId (nullable FK -> Category)

- Title (как показывать в карточке, например “Figma — Team”)
- Status (enum: Active/Paused/Canceled/Expired)

Биллинг:
- Amount (decimal)
- Currency (string, напр. “BYN”, “USD”)
- BillingPeriod (enum: Monthly/Yearly/Custom)
- BillingPeriodDays (nullable int) — если Custom
- NextChargeDate (date) — опорная дата списания
- AutoRenew (bool)

Даты:
- StartDate (date)
- EndDate (nullable date) — если подписка ограничена сроком
- TrialEndDate (nullable date)

Дополнительно:
- Notes (nullable text)
- CreatedAt
- UpdatedAt

Связи:
- User 1 — M Subscription (OwnerUserId)
- Organization 0..1 — M Subscription
- Service 1 — M Subscription
- Category 0..1 — M Subscription

### 3.6 License (Лицензия)
Если подписка “по лицензиям/местам”.
- Id (PK)
- SubscriptionId (FK -> Subscription)

- LicenseKey (nullable string)
- SeatsTotal (nullable int)
- SeatsUsed (nullable int)
- ExpiresAt (nullable date) — если лицензия отдельно истекает
- Notes (nullable text)

Связи:
- Subscription 1 — M License

### 3.7 ReminderRule (Правило напоминания)
Правила, по которым генерируются уведомления.
- Id (PK)
- SubscriptionId (FK -> Subscription)

- EventType (enum: Charge/TrialEnd/EndDate)
- DaysBefore (int) — за сколько дней напомнить
- Channel (enum: InApp/Email/Telegram)
- IsEnabled (bool)
- LastTriggeredAt (nullable datetime) — защита от дублей

Связи:
- Subscription 1 — M ReminderRule

### 3.8 TelegramBinding (Привязка Telegram)
Привязка пользователя к Telegram (для уведомлений).
- Id (PK)
- UserId (FK -> AspNetUsers, unique)
- ChatId (nullable string/long)
- BindToken (string, unique)
- IsEnabled (bool)
- BoundAt (nullable datetime)

Связи:
- User 1 — 0..1 TelegramBinding

## 4. Что рисовать на ER-диаграмме (для графической части)
На ER-диаграмме показываем:
- Organization, OrganizationMember
- Category
- Service
- Subscription
- License
- ReminderRule
- TelegramBinding
- AspNetUsers (можно как “User” прямоугольник без деталей)

Кардинальности (crow’s foot):
- Organization 1—M OrganizationMember
- User 1—M OrganizationMember
- User 1—M Subscription
- Organization 0..1—M Subscription
- Service 1—M Subscription
- Category 0..1—M Subscription
- Subscription 1—M License
- Subscription 1—M ReminderRule
- User 1—0..1 TelegramBinding

## 5. Ограничения и индексы (минимум)
- Subscription: индекс по (OwnerUserId, OrganizationId) для фильтрации
- Category: уникальность Name в рамках (OwnerUserId, OrganizationId)
- TelegramBinding: уникальный BindToken, уникальный UserId
- OrganizationMember: уникальная пара (OrganizationId, UserId)

## 6. Примечание про календарь
События календаря вычисляются из Subscription + ReminderRule:
- Charge: NextChargeDate + период
- TrialEnd: TrialEndDate
- EndDate: EndDate
Напоминания: DaysBefore от соответствующего события.
