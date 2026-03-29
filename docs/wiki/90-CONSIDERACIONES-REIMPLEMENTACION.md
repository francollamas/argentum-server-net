# 90 - Consideraciones para la Reimplementacion

> Este documento reune las notas de diseno, decisiones arquitectonicas y puntos a tener en
> cuenta para construir un servidor nuevo. Esta separado de la documentacion funcional porque
> describe **como queremos que sea**, no como es el servidor actual.

---

## 1. Arquitectura

### 1.1 Single-threaded vs Multi-threaded

El servidor original es completamente single-threaded. Esto simplifica enormemente la logica
(no hay race conditions, no hay locks, no hay deadlocks) pero limita el rendimiento a un
solo nucleo de CPU. Para un servidor moderno, considerar:

- Mantener single-threaded para la logica del juego (es mas seguro y predecible)
- Usar threads separados para I/O de red y persistencia
- O usar un modelo de actores / event loop moderno (como en game servers actuales)

### 1.2 Estado en Memoria vs Base de Datos

El servidor original mantiene todo en memoria y persiste a archivos planos. Considerar:
- Mantener el estado del juego en memoria (es necesario para latencia baja)
- Usar una base de datos para persistencia de personajes (en lugar de archivos INI)
- Separar claramente el estado mutable (runtime) del estado persistido (disco)

### 1.3 Protocolo Binario

El protocolo original es un formato binario custom muy eficiente pero fragil (sin versionado,
sin checksums, sin compresion). Considerar:
- Mantener protocolo binario para eficiencia (el cliente lo requiere para compatibilidad)
- Agregar versionado de protocolo
- Considerar compresion para paquetes grandes (datos de mapa al entrar)

### 1.4 Separacion de Responsabilidades

El servidor original mezcla completamente la logica de juego con la serializacion de red,
la persistencia a disco, y el envio de mensajes. Una reimplementacion deberia separar:
- **Capa de red**: acepta conexiones, serializa/deserializa paquetes
- **Capa de logica de juego**: reglas, formulas, validaciones
- **Capa de estado**: gestion del estado del mundo
- **Capa de persistencia**: guardado/carga de datos
- **Capa de broadcast**: decision de a quien enviar cada actualizacion

### 1.5 Configuracion Externalizada

Todos los intervalos, limites, y constantes de balance deben ser configurables sin recompilar.
El servidor original ya hace esto parcialmente (via Server.ini y Balance.dat), pero muchas
constantes estan hardcodeadas. La reimplementacion deberia externalizar TODAS las constantes
de balance y gameplay.

---

## 2. Compatibilidad de Protocolo

### 2.1 Critico para el Nuevo Servidor

Para mantener compatibilidad con el cliente existente:

- El **orden de bytes** debe ser exactamente el mismo (Little-endian)
- La **codificacion de texto** debe ser Windows-1252 (no UTF-8)
- Los **IDs de paquetes** deben coincidir exactamente
- La **estructura de cada paquete** (orden y tipo de campos) debe ser identica
- Los **tamanos de campos** deben coincidir (Int16 vs Int32, etc.)
- Los paquetes no tienen header de longitud; el parser depende del conocimiento de la estructura

### 2.2 Fragilidad del Protocolo

El protocolo es fragil por disenio:
- Sin versionado: no hay forma de negociar la version del protocolo
- Sin checksums: no se detectan errores de transmision (TCP los maneja, pero no la logica)
- Sin framing: un byte corrupto desincroniza todo el stream
- Sin compresion: los datos se envian en crudo

Un nuevo servidor que quiera ser compatible con el cliente existente **debe implementar este
protocolo exactamente como esta**.

---

## 3. Recursos del Juego

En principio, la reimplementacion NO modifica los recursos del juego. Se deben cargar
los mismos archivos de datos (DATs, mapas, etc.) que usa el servidor actual.
