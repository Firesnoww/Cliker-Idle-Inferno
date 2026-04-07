# Idle Clicker Foundation

## Objetivo

Construir una base de idle/clicker en Unity inspirada en la progresion de Cookie Clicker:

- Un recurso principal generado por clic manual y por produccion pasiva.
- Edificios con costo escalado y produccion creciente.
- Progresion clara, facil de balancear y facil de ampliar.
- Arquitectura separada entre datos, logica y presentacion.

## Principios

1. La economia manda. La UI depende de la economia, no al reves.
2. Los datos de balance deben vivir fuera de la logica dura.
3. Todo sistema nuevo debe entrar como capa, no como parche.
4. Primero un vertical slice jugable. Luego capas de profundidad.

## Bucle base

1. El jugador hace clic y gana moneda principal.
2. Compra edificios.
3. Los edificios generan moneda por segundo.
4. La produccion permite comprar mas edificios.
5. El costo de edificios escala.
6. Mas adelante entran upgrades, multiplicadores, guardado y progreso offline.

## Arquitectura propuesta

### 1. Data

Contiene configuraciones editables desde Unity.

- `GameBalanceConfig`
- `BuildingDefinition`

Responsabilidad:

- Valores de clic base.
- Lista de edificios.
- Costo inicial.
- Produccion base por edificio.
- Factor de crecimiento del costo.

### 2. Runtime State

Contiene el estado vivo de la partida.

- moneda actual
- cantidad comprada de cada edificio
- produccion total por segundo
- temporizadores de tick

Responsabilidad:

- representar la partida actual
- no contener referencias a UI

### 3. Domain Services

Logica pura del juego.

- calcular costo del siguiente edificio
- calcular produccion por segundo
- validar compras

Responsabilidad:

- reglas matematicas y de progresion
- facil de testear

### 4. Game Flow

Orquesta el runtime.

- aplicar clic manual
- procesar ticks de produccion
- comprar edificios

Responsabilidad:

- conectar config + estado + servicios
- exponer eventos para UI mas adelante

### 5. UI

Solo presenta y dispara acciones.

- boton de clic
- lista de edificios
- textos de moneda y CPS

Responsabilidad:

- nunca calcular economia
- reflejar estado del juego

### 6. Persistence

Entrara despues del slice base.

- guardado local
- carga de partida
- progreso offline

## Orden fijo de trabajo

Esta es la lista de tareas que no debemos mover hasta terminarlas:

1. Economia base: clic, moneda, tick pasivo, compra de edificios.
2. Datos de balance: definiciones editables para edificios y valores base.
3. UI minima funcional: moneda, CPS, boton de clic, lista de compra.
4. Guardado local: persistir moneda y edificios comprados.
5. Progreso offline: calcular produccion al reabrir el juego.
6. Upgrades base: multiplicadores simples de clic o produccion.
7. Pulido de balance: costos, curva de desbloqueo y ritmo de progresion.

## Reglas de implementacion

- No mezclar UI con formulas economicas.
- No hardcodear edificios dentro del controlador principal.
- Todo edificio debe poder agregarse desde datos, no desde codigo extra.
- Cada feature nueva debe encajar en esta estructura.

## Informacion que todavia necesitamos definir

Podemos avanzar con valores provisionales, pero luego hay que cerrar esto:

- nombre del recurso principal
- tema visual del juego
- lista inicial de edificios
- ritmo deseado de progresion temprana
- si habra prestigio o reinicios mas adelante

## Siguiente entrega tecnica

La base inmediata debe dejar:

- un controlador central del juego
- datos de balance serializables
- estado runtime de edificios
- servicio de calculo de produccion y costos

Con eso ya podemos montar la primera UI sin rehacer la base.
