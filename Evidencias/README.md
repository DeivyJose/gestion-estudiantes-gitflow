# Evidencias de pruebas automatizadas

Esta carpeta contiene las evidencias del sistema de Gestión de Estudiantes.

## Resultado

- Pruebas ejecutadas: 15
- Aprobadas: 15
- Fallidas: 0
- Omitidas: 0

## Historias automatizadas

1. Inicio de sesión.
2. Registro de estudiantes.
3. Consulta de estudiantes.
4. Actualización de estudiantes.
5. Eliminación de estudiantes.

Cada historia incluye un camino feliz, un camino negativo y un caso límite.

## Archivos

- `Capturas/`: contiene una captura PNG por cada prueba.
- `Reportes/Reporte-Selenium.html`: reporte generado con ExtentReports.

## Tecnologías

.NET 8, NUnit, Selenium WebDriver, Microsoft Edge y ExtentReports.

## Ejecución

La aplicación debe estar disponible en `http://localhost:5176`.

```bash
export TEST_LOGIN_USER="usuario"
export TEST_LOGIN_PASSWORD="contraseña"

SELENIUM_HEADLESS=1 dotnet test GestionEstudiantes.SeleniumTests/GestionEstudiantes.SeleniumTests.csproj
```
