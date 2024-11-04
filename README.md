# Room Reservation System

## Popis
Room Reservation System je webová aplikace vyvinutá pomocí **ASP.NET Core Web API**, která umožňuje uživatelům rezervovat místnosti, spravovat rezervace, komunikovat prostřednictvím zpráv a spravovat soubory. Aplikace podporuje rolovou autorizaci s rolemi **Administrátor** a **Registrovaný uživatel**.

## Technologie
- **.NET 8.0**
- **ASP.NET Core Web API**
- **Oracle Database**
- **JWT (JSON Web Tokens) pro autentizaci**
- **Postman pro testování API**

## Použití

1. **Registrace uživatele**
   - Použijte endpoint `POST /api/auth/register` k vytvoření nového uživatele.

2. **Přihlášení**
   - Použijte endpoint `POST /api/auth/login` pro získání JWT tokenu.

3. **Rezervace místnosti**
   - Autorizovaný uživatel může vytvářet, upravovat a mazat rezervace prostřednictvím endpointů `POST /api/bookings`, `PUT /api/bookings/{id}` a `DELETE /api/bookings/{id}`.

4. **Správa uživatelů (pro Administrátory)**
   - Administrátoři mohou spravovat uživatele pomocí endpointů `GET /api/users`, `PUT /api/users/{id}` a `DELETE /api/users/{id}`.
  
...
