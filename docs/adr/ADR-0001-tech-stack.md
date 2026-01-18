# ADR-0001: Технологический стек SubFlow

Дата: 2026-01-18
Статус: Proposed

## Контекст
Нужна веб-система учёта подписок с календарём, импортом/экспортом Excel и опциональными Telegram-уведомлениями.
Проект ведётся в Visual Studio.

## Решение
Выбран стек:
- Backend/UI: ASP.NET Core (LTS), Razor Pages (допускается MVC при необходимости)
- ORM: Entity Framework Core
- БД: SQL Server (LocalDB для разработки)
- Auth: ASP.NET Core Identity (cookie)
- Excel: ClosedXML
- Telegram: Telegram Bot API через HttpClient

## Обоснование
- ASP.NET Core + EF Core нативно поддерживаются в Visual Studio, удобны для диплома.
- Razor Pages хорошо ложится на принцип “1 экран = 1 задача”.
- Identity закрывает типовые задачи аутентификации/ролей.

## Последствия
- UI преимущественно server-side, динамика календаря — JS библиотека на странице.
- Интеграции (Excel/Telegram) реализуются в Infrastructure как адаптеры.
