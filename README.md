# Experiencia de Realidad Virtual Multijugador

## Descripción General
Este proyecto implementa una experiencia de realidad virtual multijugador desarrollada en Unity 6, que permite a los usuarios interactuar en un entorno compartido a través de portales y objetos físicos dinámicos, utilizando dispositivos Meta Quest/Pico.

![Vista general del proyecto](imagen_general.png)

## Arquitectura General

### Tecnologías Utilizadas
- **Motor**: Unity 6 con Universal Render Pipeline (URP)
- **Networking**: Unity Netcode for GameObjects
- **VR**: XR Toolkit y Open XR
- **Avatar**: Ready Player Me (RPM)
- **Rigging**: Final IK para control de avatar en VR
- **Optimización**: Addressables para carga dinámica de assets

## Decisiones de Diseño

### Sistema de Red

#### Elección de Netcode
Se seleccionó Unity Netcode for GameObjects como solución de red por su integración nativa con el ecosistema Unity, documentación actualizada y soporte oficial. Consideramos Mirror como alternativa, pero la integración más estrecha de Netcode con los sistemas modernos de Unity 6 fue el factor determinante.

![Sistema de red en funcionamiento](imagen_network.png)

#### Modelo Host/Cliente
- Implementación de un sistema de conexión local que no requiere servidores externos
- UI intuitiva para selección de rol (Host/Cliente)
- Validación de IP con formato estricto (192.168.x.x)
- Manejo de errores con feedback visual al usuario

#### Sincronización de Avatares
Se encontraron desafíos en la integración de RPM con Netcode:
- Problema: Los huesos del avatar no se sincronizaban correctamente en la red
- Solución parcial: Implementación de NetworkTransform para componentes clave
- Estado actual: Funcionalidad básica operativa con oportunidades de mejora

### Sistema de Interacción en VR

#### XR Toolkit
- Configuración del XR Origin con Action-Based Continuous Move Provider
- Sistema de interacción mediante rayos y agarre directo
- Implementación de teleportación como método alternativo de movimiento

![Interacción en VR](imagen_interaccion_vr.png)

#### Final IK
- Implementación para mapeo de movimientos del usuario al avatar
- Calibración automática basada en altura del usuario
- Sincronización de gestos y postura

### Mecánica Creativa: Sistema de Portales

Los portales funcionan como puntos de teletransporte entre diferentes áreas de la escena:
- Efecto visual de partículas azul/violeta para indicar entradas/salidas
- Sistema de "cooling down" para evitar teleportaciones rápidas consecutivas
- Sincronización en red del estado del portal y teletransporte de jugadores

![Portal en funcionamiento](imagen_portal.png)

### Física de Objetos

Se implementaron tres tipos de objetos con comportamientos físicos distintos:

1. **Objeto con Rigidbody y gravedad**
   - Colisiones precisas mediante Physics.Simulate
   - Comportamiento físico realista
   - Sincronización en red mediante NetworkRigidbody

2. **Objeto cinemático controlado por triggers**
   - Ejemplo: plataforma móvil que se activa al ser pisada
   - Movimiento determinista controlado por código
   - Sincronización mediante NetworkTransform

3. **Objeto dinámico con constraints**
   - Ejemplo: puerta giratoria con límites de rotación
   - Uso de HingeJoint para restricción de movimiento
   - Sincronización utilizando NetworkTransform y NetworkRigidbody

![Objetos interactivos](imagen_objetos.png)

### Sistema de UI

#### Menú de Conexión
- Interfaz minimalista con tres opciones: Host, Cliente, Test IP
- Diseño adaptado para interacción en VR mediante puntero láser
- Posicionamiento en el espacio para fácil acceso

![Menú de conexión](imagen_menu_conexion.png)

#### Sistema de Addressables
- Interfaz para carga dinámica de assets en tiempo de ejecución
- Monitoreo de FPS en tiempo real
- Opciones para cargar múltiples instancias (10, 100, 1000) para pruebas de rendimiento

![Interfaz de Addressables](imagen_addressables.png)

## Optimizaciones Implementadas

### Renderizado
- **Configuración URP optimizada**:
  - Reducción de resolución de sombras en dispositivos móviles
  - Ajuste dinámico de distancia de dibujado según capacidad del dispositivo
  - Desactivación de efectos post-proceso intensivos

- **GPU Instancing**:
  - Implementación para todos los objetos repetitivos
  - Reducción significativa de draw calls
  - Uso de Shader Graph para efectos visuales eficientes

### Carga de Assets
- **Sistema Addressables**:
  - Carga asíncrona y bajo demanda
  - Liberación de memoria cuando los assets no están en uso
  - Agrupación estratégica por frecuencia de uso

- **Pruebas de Rendimiento**:
  - Con 10 unidades: 90+ FPS en Meta Quest 2
  - Con 100 unidades: 60-75 FPS en Meta Quest 2
  - Con 1000 unidades: 30-45 FPS en Meta Quest 2

### Red
- **Optimización de Ancho de Banda**:
  - Uso de compresión de datos
  - Priorización de actualizaciones por distancia
  - Buffer adaptativo según condiciones de red

- **Manejo de la Latencia**:
  - Predicción de movimiento para avatares
  - Buffer de suavizado para movimientos bruscos
  - Interpolación de posiciones para transiciones naturales

## Pruebas y Validación

### Pruebas Unitarias
- Implementación de tests automatizados con Unity Test Framework
- Cobertura de componentes críticos:
  - Validación de formato IP
  - Sincronización de objetos
  - Funcionamiento de portales

### Pruebas de Rendimiento
- Monitoreo de FPS, uso de memoria y ancho de banda
- Pruebas en dispositivos Meta Quest 2 y Pico 4
- Identificación de cuellos de botella y optimizaciones resultantes

## Desafíos y Soluciones

### Integración RPM-Netcode
- **Problema**: Incompatibilidad entre el sistema de animación de RPM y la sincronización de red
- **Solución**: Desarrollo de un componente personalizado para sincronizar sólo los huesos principales y utilizar animación local para detalles

### Sincronización de Física
- **Problema**: Inconsistencias en la física de objetos entre clientes
- **Solución**: Implementación de autoridad sobre objetos físicos y sistema de reconciliación para diferencias críticas

## Conclusiones y Trabajo Futuro

### Estado Actual
El proyecto cumple con los requisitos funcionales establecidos, demostrando:
- Conexión multijugador funcional en red local
- Interacción con objetos físicos sincronizados
- Sistema de portales para navegación
- Carga dinámica de assets mediante Addressables

### Mejoras Potenciales
- Implementación de servidor dedicado para mejorar estabilidad
- Perfeccionamiento de la sincronización de avatares RPM
- Expansión de la mecánica de portales con efectos de distorsión espacial
- Sistema de persistencia de estado entre sesiones

### Lecciones Aprendidas
- La importancia de priorizar la optimización desde etapas tempranas para VR
- Los beneficios de un enfoque modular en el desarrollo de sistemas interconectados
- La necesidad de pruebas exhaustivas en condiciones de red variables

## Instalación y Ejecución

### Requisitos
- Unity 6.x
- URP Package
- XR Interaction Toolkit 2.3.0+
- XR Plugin Management
- Netcode for GameObjects
- Final IK (Asset Store)

### Configuración
1. Clonar el repositorio
2. Abrir con Unity 6.x
3. Instalar paquetes dependientes a través del Package Manager
4. Configurar el dispositivo XR en XR Plugin Management

### Construcción
- Para PC (Modo desarrollo): Build para Windows x64
- Para Meta Quest/Pico: Build para Android con arquitectura ARM64
