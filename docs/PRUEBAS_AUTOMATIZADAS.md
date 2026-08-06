# Pruebas automatizadas con Selenium

## Descripción

Este proyecto incluye una suite de pruebas automatizadas para validar las funciones principales del sistema de Gestión de Estudiantes.

Las pruebas fueron desarrolladas con NUnit y Selenium WebDriver, utilizando Microsoft Edge como navegador.

## Historias de usuario automatizadas

Se automatizaron cinco historias de usuario:

1. Inicio de sesión.
2. Registro de estudiantes.
3. Consulta de estudiantes.
4. Actualización de estudiantes.
5. Eliminación de estudiantes.

Cada historia contiene tres escenarios:

- Camino feliz.
- Camino negativo.
- Caso límite.

En total se ejecutan 15 pruebas automatizadas.

## Estructura principal

```text
GestionEstudiantes.SeleniumTests/
├── Base/
├── Pages/
├── Reporting/
├── Tests/
└── Utilities/
```

La automatización utiliza el patrón Page Object Model para separar la lógica de las páginas de los casos de prueba.

## Requisitos

- .NET 8 SDK.
- Microsoft Edge.
- Aplicación disponible en `http://localhost:5176`.
- Base de datos configurada y activa.
- Credenciales válidas configuradas mediante variables de entorno.

## Configuración de credenciales

```bash
export TEST_LOGIN_USER="usuario"
export TEST_LOGIN_PASSWORD="contraseña"
```

Las credenciales reales no se almacenan en el repositorio.

## Ejecución

Desde la raíz del proyecto:

```bash
SELENIUM_HEADLESS=1 dotnet test GestionEstudiantes.SeleniumTests/GestionEstudiantes.SeleniumTests.csproj
```

Para ejecutar las pruebas mostrando el navegador:

```bash
dotnet test GestionEstudiantes.SeleniumTests/GestionEstudiantes.SeleniumTests.csproj
```

## Resultado final

- Pruebas ejecutadas: 15
- Pruebas aprobadas: 15
- Pruebas fallidas: 0
- Pruebas omitidas: 0

## Evidencias

Las evidencias se encuentran en:

```text
Evidencias/
├── Capturas/
└── Reportes/
```

La carpeta `Capturas` contiene una imagen PNG por cada prueba.

El reporte HTML está disponible en:

```text
Evidencias/Reportes/Reporte-Selenium.html
```

El reporte fue generado con ExtentReports e incluye el estado, la duración, la categoría y la captura final de cada prueba.
