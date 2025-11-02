# Tienda UCN - Backend (API)

El objetivo de este proyecto es implementar una API REST utilizando **ASP.NET Core 9** y **SQLite** para crear una plataforma de comercio electrónico llamada **Tienda UCN**. Incluye autenticación de usuarios con JWT, gestión de perfiles, productos y carrito de compras.

Se implementa el **Patrón Repositorio** para asegurar una arquitectura limpia, separación de responsabilidades y fácil mantenimiento. Se utiliza **Cloudinary** para el almacenamiento externo de medios, permitiendo un manejo eficiente de imágenes y otros activos. El sistema está diseñado para escalabilidad y seguridad.

## Contenido

* [Características Principales](#características-principales)
* [Requisitos](#requisitos)
* [Instalación y Ejecución](#instalación-y-ejecución)
* [Configuración Requerida](#configuración-requerida)
* [Endpoints Evaluados (Flujos Funcionales)](#endpoints-evaluados-flujos-funcionales)
* [Uso](#uso)
* [Autor](#autor)

---

## Características Principales

El proyecto cubre la totalidad de los flujos de negocio evaluados, asegurando:

* **Seguridad:** Autenticación JWT y protección de todas las rutas administrativas por rol `Admin`.
* **Integridad:** Validación de unicidad (`name`, `slug`, `email`, `rut`) y control estricto de referencias.
* **Defensa:** Sanitización de todas las entradas de usuario (`POST/PUT`) para prevenir inyección de HTML/Script (XSS).
* **Transacciones:** Uso de transacciones para flujos críticos como la creación de pedidos.

---

## Requisitos

Para la ejecución del proyecto, los siguientes componentes deben estar instalados:

* **.NET 9 SDK** (ASP.NET Core).
* **Visual Studio Code** (versión 1.89.1+).
* **Git** (versión 2.45.1+).
* **Postman**.

## Instalación y Ejecución

Sigue estos pasos para poner en marcha el backend:

1.  **Clonar el repositorio:**
    ```bash
    git clone https://github.com/maximilianoBezares/TiendaUCN.git
    ```

3. **Navegar a la carpeta del proyecto:**
    ```bash
    cd TiendaUCN
    ```
4. **Abrir el proyecto con Visual Studio Code:**
    ```bash
    code .
    ```

5.  **Configurar Variables de Entorno (Requerido):**
    Copia el contenido del archivo de ejemplo para crear el archivo de configuración activo:
    ```bash
    cp appsettings.example.json appsettings.json
    ```

6.  **Actualizar Credenciales** (Ver sección [Configuración Requerida](#configuración-requerida)).

7.  **Restaurar las dependencias:**
    ```bash
    dotnet restore
    ```

8.  **Aplicar Migraciones y Ejecutar:**
    *La base de datos (SQLite) se actualiza y el Seeder crea Roles, el usuario Admin y datos de prueba.*
    ```bash
    dotnet run
    ```

---

## Configuración Requerida

El archivo `appsettings.json` debe ser actualizado con las credenciales necesarias:

* Reemplace **`JWTSecret`** con una clave secreta fuerte de al menos **32 caracteres** de longitud.
* Reemplace **`ResendAPIKey`** con su API Key de Resend. Puede obtener su clave en el siguiente enlace: [Resend - Getting Started].
* Reemplace las credenciales de Cloudinary con sus valores reales.
* Reemplace admin **`Rut`** con el formato **XXXXXXXX-X**.
* Reemplace admin **`BirthDate`** con el formato **YYYY-MM-DD**.
* Reemplace admin **`PhoneNumber`** con el formato **+569 XXXXXXXX**.
* Reemplace admin **`Password`** y **`RandomUserPassword`** con una contraseña alfanumérica que contenga al menos una mayúscula, una minúscula y un carácter especial.
* Reemplace **`WelcomeSubject`**, **`From`** y **`VerificationSubject`** con sus propias variables de correo electrónico. Se recomienda usar el dominio de correo electrónico **`<onboarding@resend.dev>`** para usar el plan gratuito de la API de Resend.
* Reemplace **`TimeZone`** con su zona horaria local.
* Reemplace **`CronJobDeleteUnconfirmedUsers`** con su propio cronjob.
* Reemplace **`DaysOfDeleteUnconfirmedUsers`** con su propio intervalo en días para eliminar a los usuarios no confirmados.
* Reemplace **`DefaultImageUrl`** con su propia URL de imagen predeterminada.
* Reemplace **`CookieExpirationDays`** con el tiempo de expiración de las cookies de su preferencia.
* Mantenga la sección **`HangfireDashboard`** si desea una configuración predeterminada del dashboard.
* Mantenga la configuración de **`AllowedUserNameCharacters`**, **`ExpirationTimeInMinutes`**, **`TransformationWidth`**, **`TransformationCrop`**, **`TransformationQuality`**, **`TransformationFetchFormat`**, **`DefaultPageSize`** e **`ImageMaxSizeInBytes`** del archivo `appsettings.example.json`.

---

## Endpoints del sistema

### Flujo 1: Sistema de Autenticación Completo

| Sub Flujo | Método | Ruta | Requisito Técnico Clave |
| :---: | :---: | :--- | :--- |
| **Registro** | `POST` | `/api/auth/register` | Validación de RUT/Email únicos; creación con rol "Cliente"; generación de código. |
| **Verificación**| `POST` | `/api/auth/verify` | Valida código y activa la cuenta. |
| **Login** | `POST` | `/api/auth/login` | Validación de credenciales; generación de JWT con ID, rol, email. |
| **Recuperación**| `POST` | `/api/auth/recover-password` | Genera y envía código de recuperación por email. |
| **Restablecer**| `PATCH`| `/api/auth/reset-password` | Restablece contraseña validando código y reglas de seguridad. |

### Flujo 2: Gestión de Cuenta de Usuario

| Sub Flujo | Método | Ruta | Requisito Técnico Clave |
| :---: | :---: | :--- | :--- |
| **Perfil** | `GET` | `/api/user/profile` | Retorna el perfil del usuario autenticado. |
| **Actualizar** | `PUT` | `/api/user/profile` | Edición de datos personales; validaciones espejo de registro; cambio de email con verificación. |
| **Contraseña** | `PATCH`| `/api/user/change-password` | Cambio seguro de contraseña (verifica actual, invalida sesiones). |

### Flujo 3: Sistema de Carrito de Compras

| Recurso | Método | Ruta | Requisito Técnico Clave |
| :---: | :---: | :--- | :--- |
| **Obtener** | `GET` | `/api/cart` | Retorna carrito con cálculos de totales/ahorro, stock y precios vigentes. |
| **Creacion**| `POST` | `/api/cart/items` | Añade/consolida ítem, validando stock disponible. |
| **Actualizar** | `PUT` | `/api/cart/items/{itemId}` | Modifica cantidad con validación de stock. |
| **Eliminar**| `DELETE`| `/api/cart/items/{itemId}` | Elimina ítem del carrito. |

### Flujo 4: Proceso de Compra y Pedidos

| Recurso | Método | Ruta | Requisito Técnico Clave |
| :---: | :---: | :--- | :--- |
| **Creación** | `POST` | `/api/orders` | **Transacción**; revalida stock/precios; descuenta stock y vacía carrito. |
| **Listado** | `GET` | `/api/orders` | Listado paginado filtrado por `userld` del token. |
| **Detalle** | `GET` | `/api/orders/{id}` | Detalle del pedido con ítems y precios aplicados. |

### Flujo 5: Catálogo de Productos (Público)

| Recurso | Método | Ruta | Requisito Técnico Clave |
| :---: | :---: | :--- | :--- |
| **Listado** | `GET` | `/api/products` | **Público.** Paginación, filtros (categoría, marca, precio) y ordenamiento seguro. |
| **Detalle** | `GET` | `/api/products/{id}` | **Público.** Retorna solo activos; `finalPrice` calculado en servidor. |

### Flujo 6: Administración de Productos (Admin)

| Recurso | Método | Ruta | Requisito Técnico Clave |
| :---: | :---: | :--- | :--- |
| **CRUD** | `GET` | `/api/admin/products` | Listado interno (incluye inactivos/eliminados), paginación/filtros. |
| **CRUD** | `GET` | `/api/admin/products/{id}` | Detalle interno completo. |
| **CRUD** | `POST` | `/api/admin/products` | Creación; valida precio/stock; sanitiza. |
| **CRUD** | `PUT` | `/api/admin/products/{id}` | Edición; mantiene inmutables. |
| **CRUD** | `DELETE`| `/api/admin/products/{id}` | **Eliminación Lógica** (`soft delete`). |
| **Imágenes** | `POST` | `/api/admin/products/{id}/images` | Subida a Cloudinary; valida MIME/tamaño; guarda `publicId`. |
| **Imágenes** | `DELETE`| `/api/admin/products/{id}/images/{imageId}` | Eliminación sincronizada con *storage* remoto. |
| **Descuento** | `PATCH`| `/api/admin/products/{id}/discount` | Gestión de descuentos. |
| **Estado** | `PATCH`| `/api/admin/products/{id}/status` | Activar/desactivar (`active=true/false`). |

### Flujo 7: CRUD de Categorías y Marcas (Admin)

**(Flujo completado en esta conversación)**

| Recurso | Método | Ruta | Requisito Técnico Clave |
| :---: | :---: | :--- | :--- |
| **Categorías** | `GET` | `/api/admin/categories` | Listado con paginación/búsqueda. |
| **Categorías** | `GET` | `/api/admin/categories/{id}` | Detalle con metadatos (`productCount`). |
| **Categorías** | `POST` | `/api/admin/categories` | Creación; unicidad `name`/`slug`; sanitización. |
| **Categorías** | `PUT` | `/api/admin/categories/{id}` | Edición; unicidad con exclusión; sanitización. |
| **Categorías** | `DELETE`| `/api/admin/categories/{id}` | Eliminación con restricción de integridad (`409 Conflict`). |
| **Marcas** | `GET` | `/api/admin/brands` | Listado con paginación/búsqueda. |
| **Marcas** | `GET` | `/api/admin/brands/{id}` | Detalle con metadatos (`productCount`). |
| **Marcas** | `POST` | `/api/admin/brands` | Creación; unicidad `name`/`slug`; sanitización. |
| **Marcas** | `PUT` | `/api/admin/brands/{id}` | Edición; unicidad con exclusión; sanitización. |
| **Marcas** | `DELETE`| `/api/admin/brands/{id}` | Eliminación con restricción de integridad (`409 Conflict`). |

### Flujo 8: Administración de Pedidos (Admin)

| Recurso | Método | Ruta | Requisito Técnico Clave |
| :---: | :---: | :--- | :--- |
| **Listado** | `GET` | `/api/admin/orders` | Listado paginado con filtros (estado, fechas, email, número); ordenamiento seguro. |
| **Detalle** | `GET` | `/api/admin/orders/{id}` | Detalle completo del pedido. |
| **Estado** | `PATCH`| `/api/admin/orders/{id}/status` | Actualización de estado; respeta **máquina de estados**; registra auditoría. |

### Flujo 9: Administración de Usuarios (Admin)

| Recurso | Método | Ruta | Requisito Técnico Clave |
| :---: | :---: | :--- | :--- |
| **Listado** | `GET` | `/api/admin/users` | Listado paginado con filtros por rol, estado, email; ordenamiento seguro. |
| **Detalle** | `GET` | `/api/admin/users/{id}` | Detalle interno del usuario. |
| **Bloqueo** | `PATCH`| `/api/admin/users/{id}/status` | Bloqueo/Desbloqueo; prohíbe auto-bloqueo; invalida sesiones. |
| **Rol** | `PATCH`| `/api/admin/users/{id}/role` | Asignación/Cambio de rol; evita dejar el sistema sin Admin. |

## Autor
**@maximilianoBezares** (Maximiliano Bezares)
**@Gaboooou** (Gabriel Briones)

