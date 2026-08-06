# Gestión de Estudiantes API

Proyecto individual desarrollado para la asignatura Programación III del Instituto Tecnológico de las Américas.

La aplicación consiste en una API REST para registrar y administrar estudiantes. El proyecto se utiliza para aplicar los conceptos de Git, manejo de ramas, Git Flow y Pull Requests.

## Tecnologías utilizadas

- C#
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Swagger
- Git y GitHub

## Funciones principales

- Registrar estudiantes.
- Consultar estudiantes.
- Actualizar estudiantes.
- Eliminar estudiantes.
- Validar la información recibida.

## Autor

Deivy Jose Ureña Namias  
Matrícula: 20250938
<!-- PRUEBAS-SELENIUM -->

## Pruebas automatizadas

El proyecto incluye 15 pruebas automatizadas con NUnit, Selenium WebDriver y Microsoft Edge.

Se validan cinco funciones: inicio de sesión, registro, consulta, actualización y eliminación de estudiantes. Cada función incluye un camino feliz, un camino negativo y un caso límite.

### Resultado

- Ejecutadas: 15
- Aprobadas: 15
- Fallidas: 0
- Omitidas: 0

### Ejecución

La aplicación debe estar activa en `http://localhost:5176`.

Configura `TEST_LOGIN_USER` y `TEST_LOGIN_PASSWORD`, y ejecuta:

`SELENIUM_HEADLESS=1 dotnet test GestionEstudiantes.SeleniumTests/GestionEstudiantes.SeleniumTests.csproj`

Las capturas y el reporte HTML se encuentran en la carpeta `Evidencias`.

La guía completa está en `docs/PRUEBAS_AUTOMATIZADAS.md`.
