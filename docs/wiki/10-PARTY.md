# 10 - Party (Grupo)

> Este documento describe el sistema de parties (grupos temporales): creacion, ingreso,
> distribucion de experiencia, comunicacion, liderazgo, y disolucion. Las parties son
> el mecanismo de cooperacion tactica a corto plazo del juego.

---

## 1. Vision General

Una party es un grupo temporal de jugadores que comparten experiencia y comunicacion.
A diferencia de los clanes (permanentes y con estructura compleja), las parties son
efimeras: se crean, se usan, y se disuelven en la misma sesion de juego.

El servidor soporta hasta **300 parties** simultaneas.
Cada party tiene un maximo de **5 miembros**.

---

## 2. Creacion de Party

### 2.1 Requisitos del Creador

| Requisito | Valor |
|-----------|-------|
| Carisma x Liderazgo | >= 100 |
| Skill de Liderazgo | >= 5 |
| No estar muerto | Debe estar vivo |
| No pertenecer a otra party | Debe estar libre |

**Ejemplo:** un personaje con Carisma 20 necesita Liderazgo 5 (20 x 5 = 100).
Un personaje con Carisma 10 necesita Liderazgo 10 (10 x 10 = 100).

### 2.2 Al Crearse

El creador se convierte automaticamente en el **lider (fundador)** de la party.
Es el unico miembro inicialmente.

---

## 3. Ingreso a una Party

### 3.1 Proceso de Solicitud

1. El jugador hace click sobre alguien que pertenezca a una party
2. Escribe el comando `/PARTY` para solicitar ingreso
3. El **lider** de la party recibe la solicitud
4. El lider decide si acepta con `/ACCEPTPARTY`

Nota: la solicitud se envia al **lider**, no al miembro sobre el que se hizo click.

### 3.2 Condiciones para Ser Aceptado

Todas las condiciones deben cumplirse en el momento de la aceptacion:

| Condicion | Detalle |
|-----------|---------|
| Distancia al lider | Maximo **2 tiles** |
| Estado vital | No estar muerto |
| Compatibilidad faccionaria | Ver seccion 3.3 |
| Party no llena | Maximo 5 miembros |

### 3.3 Compatibilidad Faccionaria

Las parties tienen restricciones de composicion basadas en facciones:

| Miembro existente | Nuevo miembro | Permitido |
|-------------------|---------------|-----------|
| Armada Real | Criminal | **No** |
| Criminal | Armada Real | **No** |
| Legion Oscura | Ciudadano | **No** |
| Ciudadano | Legion Oscura | **No** |
| Armada Real | Ciudadano | Si |
| Legion Oscura | Criminal | Si |
| Ciudadano | Ciudadano | Si |
| Criminal | Criminal | Si |

En resumen: la Armada Real y los criminales son incompatibles, y la Legion Oscura y los
ciudadanos son incompatibles. No se pueden mezclar bandos opuestos en una misma party.

---

## 4. Distribucion de Experiencia

Este es el sistema mas complejo de las parties.

### 4.1 Formula de Distribucion

La experiencia se distribuye con una formula ponderada por nivel:

```
ExpMiembro = ExpTotal x (NivelMiembro ^ Exponente) / SumaNivelesElevados
```

Donde:
- **ExpTotal**: la experiencia generada por el evento (kill de NPC, etc.)
- **NivelMiembro**: el nivel del miembro que recibira la experiencia
- **Exponente**: configurable en el archivo de balance (ExponenteNivelParty)
- **SumaNivelesElevados**: la suma de `Nivel ^ Exponente` de **todos los miembros elegibles**

### 4.2 Implicacion del Exponente

El exponente determina que tan "desigual" es la distribucion:
- **Exponente = 1**: distribucion proporcional lineal al nivel (un nivel 40 recibe el doble que un nivel 20)
- **Exponente > 1**: los de mayor nivel reciben proporcionalmente MAS (escala exponencial)
- **Exponente = 0**: todos reciben partes iguales (irrelevante del nivel)

### 4.3 Condiciones para Recibir Experiencia

Para que un miembro reciba experiencia de un evento, debe cumplir **todas** estas condiciones:

| Condicion | Detalle |
|-----------|---------|
| Mismo mapa | Estar en el mapa donde ocurrio el evento |
| Distancia maxima | Estar a **18 tiles o menos** del evento |
| Estado vital | **No estar muerto** |

Si un miembro no cumple alguna condicion, no recibe nada de ese evento especifico. Su porcion
se redistribuye entre los demas miembros elegibles.

### 4.4 Acumulacion de Experiencia

La experiencia de la party se **acumula internamente** durante la existencia de la party.
No se entrega inmediatamente al miembro en cada evento.

La experiencia acumulada se entrega al miembro cuando:
- El miembro **abandona** la party
- La party se **disuelve**
- Se ejecuta un **WorldSave**

### 4.5 Penalizacion por Muerte de Miembro

Cuando un miembro de la party muere, la party sufre una penalizacion de experiencia. Esto
desalienta estrategias de sacrificio donde un miembro se deja morir repetidamente.

---

## 5. Comunicacion

### 5.1 Chat de Party

Los miembros de la party comparten un canal de chat exclusivo. Los mensajes enviados al
chat de party llegan a todos los miembros con el formato:

```
[NombreDelEmisor] texto del mensaje
```

---

## 6. Gestion de Liderazgo

### 6.1 Poderes del Lider

El lider de la party puede:
- Aceptar solicitudes de ingreso
- Expulsar miembros individualmente
- Transferir el liderazgo a otro miembro
- Disolver la party (abandonandola)

### 6.2 Transferencia de Liderazgo

El lider puede transferir el rol de lider a otro miembro de la party:
- El nuevo lider no debe estar muerto
- La transferencia es inmediata
- El lider anterior se convierte en miembro normal

### 6.3 Expulsion de Miembros

El lider puede expulsar a cualquier miembro:
- El miembro expulsado recibe su experiencia acumulada
- Se le notifica de la expulsion
- Sale de la party inmediatamente

---

## 7. Disolucion

### 7.1 El Lider Abandona

Si el **lider abandona** la party, esta se **disuelve completamente**:
- Todos los miembros reciben su experiencia acumulada
- Se notifica a todos
- La party deja de existir

No hay herencia automatica de liderazgo: si el lider se va sin transferir, la party muere.

### 7.2 Un Miembro No-Lider Abandona

Si un miembro que no es lider abandona:
- Solo el se va
- Recibe su experiencia acumulada
- La party sigue existiendo con los demas miembros

### 7.3 Desconexion

Si un miembro se desconecta:
- Se ejecuta la logica de abandono (recibe experiencia acumulada, sale de la party)
- Si era el lider, la party se disuelve

---

## 8. Relacion con Otros Sistemas

### 8.1 Combate
- La experiencia de combate se comparte segun la formula de la party
- Las mascotas de miembros de la party pueden cooperar en combate
- La propiedad de NPC se respeta entre miembros de la misma party

### 8.2 Clanes
- Las parties y los clanes son sistemas independientes y complementarios
- Un jugador puede estar en ambos simultaneamente
- No hay restriccion de que los miembros de una party pertenezcan al mismo clan

### 8.3 Facciones
- La composicion de la party esta restringida por compatibilidad faccionaria
- Miembros de facciones opuestas no pueden estar en la misma party

### 8.4 Muerte
- Al morir, el miembro deja de ser elegible para experiencia
- La party sufre una penalizacion de experiencia
- La muerte no expulsa automaticamente de la party
