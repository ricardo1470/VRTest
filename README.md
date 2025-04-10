
# 🕶️ Experiencia de Realidad Virtual Multijugador

Este proyecto implementa una experiencia de realidad virtual multijugador desarrollada en Unity 6, que permite a los usuarios interactuar en un entorno compartido a través de portales y objetos físicos dinámicos, utilizando dispositivos **Meta Quest/Pico**.

<p align="center">
  <img src="https://github.com/ricardo1470/VRTest/blob/main/img/7.png" width="80%" />
</p>

---

## 📌 Tabla de Contenido

1. [Descripción General](#descripción-general)
2. [Arquitectura General](#arquitectura-general)
   - [Tecnologías Utilizadas](#tecnologías-utilizadas)
   - [Decisiones de Diseño](#decisiones-de-diseño)
3. [Mecánicas y Funcionalidades](#mecánicas-y-funcionalidades)
   - [Sistema de Interacción en VR](#sistema-de-interacción-en-vr)
   - [Sistema de Portales](#mecánica-creativa-sistema-de-portales)
   - [Física de Objetos](#física-de-objetos)
   - [Sistema de UI](#sistema-de-ui)
4. [Optimizaciones Implementadas](#optimizaciones-implementadas)
5. [Pruebas y Validación](#pruebas-y-validación)
6. [Desafíos y Soluciones](#desafíos-y-soluciones)
7. [Conclusiones y Trabajo Futuro](#conclusiones-y-trabajo-futuro)
8. [Instalación y Ejecución](#instalación-y-ejecución)
9. [Contacto](#contact-💬)

---

## 📖 Descripción General

El entorno multijugador de VR desarrollado permite:

- Conexión en red local entre dispositivos
- Sincronización de avatares personalizados
- Interacción física con objetos dinámicos
- Teleportación mediante portales sincronizados
- Carga dinámica de assets en tiempo real

---

## 🏗️ Arquitectura General

### Tecnologías Utilizadas

- **Motor**: Unity 6 + Universal Render Pipeline (URP)
- **Networking**: Unity Netcode for GameObjects
- **VR Framework**: XR Toolkit y Open XR
- **Avatares**: Ready Player Me (RPM)
- **Rigging**: Final IK
- **Assets**: Addressables

---

## 🧠 Decisiones de Diseño

### 🔌 Sistema de Red

#### Netcode
- Elección: **Unity Netcode for GameObjects**
- Justificación: integración nativa con Unity 6, soporte oficial, documentación activa.

#### Modelo Host/Cliente
- Conexión local (sin servidores externos)
- Interfaz intuitiva para elección de rol
- Validación IP estricta (`192.168.x.x`)
- Manejo visual de errores

#### Sincronización de Avatares RPM
- Problema: huesos no sincronizados correctamente
- Solución: uso de `NetworkTransform` en componentes clave
- Estado actual: funcional, con margen de mejora

---

## 🕹️ Mecánicas y Funcionalidades

### Sistema de Interacción en VR

- Uso de XR Origin con *Action-Based Move Provider*
- Ray interactor y agarre directo
- Teleportation System integrado

<p align="center">
  <img src="https://github.com/ricardo1470/VRTest/blob/main/img/2.png" width="80%" />
</p>

### Final IK

- Mapeo completo del usuario al avatar
- Calibración automática según altura
- Gestos y posturas sincronizadas

---

### Mecánica Creativa: Sistema de Portales

- Teletransporte entre zonas de la escena
- Efectos visuales (partículas azul/violeta)
- Cooldown para evitar abusos
- Sincronización total entre jugadores

<p align="center">
  <img src="https://github.com/ricardo1470/VRTest/blob/main/img/1.png" width="80%" />
</p>

---

### Física de Objetos

1. **Objeto con Rigidbody**
   - Colisiones reales (`Physics.Simulate`)
   - Sincronización con `NetworkRigidbody`

2. **Objeto Cinemático**
   - Activado por triggers (ej: plataformas móviles)
   - Movimiento determinista sincronizado

3. **Objeto con Constraints**
   - Ejemplo: puerta giratoria
   - Uso de `HingeJoint`
   - Sincronización híbrida (`NetworkTransform` + `NetworkRigidbody`)

<p align="center">
  <img src="https://github.com/ricardo1470/VRTest/blob/main/img/4.png" width="80%" />
</p>

---

### Sistema de UI

#### Menú de Conexión
- Opciones: Host / Cliente / Test IP
- Compatible con puntero láser
- Espacialmente colocado para VR

<p align="center">
  <img src="https://github.com/ricardo1470/VRTest/blob/main/img/3.png" width="80%" />
</p>

#### Addressables
- Carga de objetos a demanda
- Botones para 10 / 100 / 1000 instancias
- Monitoreo de FPS en vivo

---

## 🚀 Optimizaciones Implementadas

### Renderizado
- URP optimizado para VR
- Reducción de sombras
- Draw distance adaptativo
- Uso de GPU Instancing y Shader Graph

### Addressables
- Carga asíncrona
- Liberación de memoria ociosa
- Agrupación según frecuencia de uso

### Red
- Compresión de datos
- Interpolación y predicción de movimiento
- Prioridad según distancia y relevancia

---

## 🧪 Pruebas y Validación

### Pruebas Unitarias
- IP válida
- Sincronización de objetos
- Funcionamiento de portales

### Pruebas de Rendimiento

| Unidades | FPS (Meta Quest 2) |
|---------|---------------------|
| 10      | 90+ FPS             |
| 100     | 60–75 FPS           |
| 1000    | 30–45 FPS           |

---

## 🧩 Desafíos y Soluciones

### Integración RPM + Netcode
- **Problema**: incompatibilidad en animaciones de huesos
- **Solución**: sincronización de huesos principales y animación local

### Física de Objetos
- **Problema**: inconsistencias entre clientes
- **Solución**: autoridad local y reconciliación de estado

---

## ✅ Conclusiones y Trabajo Futuro

### Estado Actual
- Multijugador funcional (red local)
- Portales y objetos físicos sincronizados
- Addressables implementado

### Mejoras Potenciales
- Servidor dedicado
- Mejor sincronización RPM
- Persistencia entre sesiones
- Efectos de distorsión en portales

### Lecciones Aprendidas
- Optimizar desde el inicio
- Modularidad como clave de escalabilidad
- Importancia de pruebas bajo red real

---

## 🛠️ Instalación y Ejecución

### Requisitos
- Unity 6.x
- URP
- XR Toolkit 2.3+
- Netcode for GameObjects
- Final IK

### Configuración
1. Clonar el repositorio
2. Abrir en Unity 6.x
3. Instalar paquetes en el Package Manager
4. Configurar XR Plugin Management

### Build
- **PC**: Windows x64
- **Quest/Pico**: Android ARM64

---

## 📬 Contact 💬

<div align="center">

### ¡Conectemos!

| [<img src="https://github.com/ricardo1470/ricardo1470/blob/master/img/GitHub.png" alt="Github logo" width="34">](https://github.com/ricardo1470/README/blob/master/README.md) | [<img src="https://github.com/ricardo1470/ricardo1470/blob/master/img/email.png" alt="email logo" height="32">](mailto:ricardo.alfonso.camayo@gmail.com) | [<img src="https://github.com/ricardo1470/ricardo1470/blob/master/img/linkedin-icon.png" alt="Linkedin Logo" width="32">](https://www.linkedin.com/in/ricardo-alfonso-camayo/) | [<img src="https://github.com/ricardo1470/ricardo1470/blob/master/img/twitter.png" alt="Twitter Logo" width="30">](https://twitter.com/RICARDO1470) |
|:---:|:---:|:---:|:---:|

</div>
