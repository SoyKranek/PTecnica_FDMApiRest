Prueba Técnica Backend
En Fundación delamujer estamos comprometidos con mejorar la calidad de vida de las mujeres colombianas y la de sus familias. Como parte de nuestro proceso de selección, se invita a los candidatos a realizar la siguiente prueba técnica para optar por el cargo de Backend, en la cual se evaluarán habilidades clave necesarias para el desarrollo del rol.
Instrucciones Generales
●	Esta prueba debe ser enviada a más tardar el JUEVES 30 A LAS 23:59:59
●	La entrega debe realizarse mediante el enlace a un repositorio de código (GitHub/GitLab/Bitbucket).
●	Se valorará el tiempo de entrega, la originalidad, la claridad y profundidad de las respuestas, la justificación técnica basada en experiencias reales, así como la aplicación de buenas prácticas. 
●	El proyecto debe ser completamente replicable para su correcta evaluación. El entregable debe incluir instrucciones claras de despliegue.
●	Se requiere incluir un despliegue funcional por medio de URL pública.
Prueba
El objetivo de esta prueba es evaluar la capacidad del candidato para diseñar, desarrollar y desplegar una API REST backend lista para producción, aplicando buenas prácticas de arquitectura, desarrollo, persistencia de datos y despliegue en la nube. Se espera que el candidato tome decisiones técnicas propias, las implemente correctamente y las documente de forma clara.

Contexto del caso de uso

Una empresa necesita una API que permita gestionar un catálogo de productos y su stock. Esta API será el núcleo transaccional consumido por múltiples plataformas concurrentes. Debe permitir administrar productos, controlar su disponibilidad de forma segura y exponer endpoints consistentes.

Alcance de la solución

●	Lenguaje y framework: .NET (C#) reciente.
●	Arquitectura: Clean Architecture, Hexagonal o Arquitectura Orientada a Dominio (DDD).
●	Base de datos: A elección del candidato (Relacional o NoSQL).
●	Los contratos de la API (request/response) quedan a criterio del candidato.
●	Recomendamos realizar commits descriptivos y progresivos en el repositorio para entender tu evolución en el desarrollo.
●	El foco está en la calidad técnica, no en la complejidad del dominio.

Funcionalidades y calidad técnica

La API debe permitir:
●	CRUD de Productos: Implementación robusta con respuestas HTTP semánticas (201, 204, 400, 404, etc.) y paginación en el listado.
●	Gestión de Stock: Endpoint específico para descontar/aumentar inventario.
●	Validaciones: Manejo de errores y validación de reglas de negocio (ej: el stock no puede ser negativo).
●	Documentación: Swagger configurado, funcional y legible.

