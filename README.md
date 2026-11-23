# Pterodáctilo de Juárez

Vertical slice hecho en Unity para la Actividad de Medio Término (Gráficas) de TC2008B.

Cruza el barrio de Juárez hasta la tienda y regresa a casa sin que el pterodáctilo te atrape. Cuando escuches su chillido o veas su sombra, escóndete bajo un árbol, una sombrilla o un techito. Si te agarra a la vista, se te lleva volando.

**Autor:** Cristhian Viery Maida Suarez (A01668790)

**Video de una partida:** https://youtu.be/O6K3CmdSm_s

## Cómo jugar

| Tecla | Acción |
|---|---|
| WASD / Flechas | Moverse |
| Espacio | Empezar la partida / usar el machete (después de la tienda) |
| R | Reiniciar al ganar o perder |

1. **Objetivo 1/2:** llega a la tienda (al final de la calle, donde brillan las partículas).
2. En la tienda consigues un **machete** de un solo uso.
3. **Objetivo 2/2:** regresa a casa. De regreso el pterodáctilo ataca más seguido y avisa con menos tiempo.
4. Cuando chille tienes unos segundos para meterte a una sombra. Si estás en la sombra, la picada falla; si estás a la vista, pierdes. Con el machete puedes espantar un ataque presionando Espacio.

## Abrir el proyecto

- Unity **6000.6.8f1** (Unity 6), plantilla Universal 3D (URP).
- Abrir la carpeta `PterodactiloJuarez/` desde Unity Hub.
- Escena: `Assets/Scenes/Juego.unity` y dar Play.

## Qué cubre del vertical slice

| Requisito | Dónde |
|---|---|
| Loop cerrado (objetivo, ganar, perder, reiniciar) | `GestorPartida.cs`, `Objetivo.cs` (tienda → casa), `SceneManager.LoadScene` con R |
| Animación ligada al estado | `AC_Jugador`: idle/walk con `SetFloat("velocidad")`, die con `SetTrigger("atrapado")`, machetazo. `AC_Pterodactilo`: aletear/planear con `SetBool("picada")` |
| Partículas con propósito | Polvo donde aterriza la picada; brillo dorado sobre el objetivo actual |
| Iluminación manual | Luz direccional de atardecer con sombras suaves (la sombra del pterodáctilo avisa dónde está) |
| Cámara | `FollowPlayer.cs` con offset en `LateUpdate` |
| UI | Objetivo, estado (Escondido / A la vista / ¡Chillido!), pantalla de inicio, victoria y derrota |
| Extras | Música, chillido, pantalla de inicio, se lo lleva volando al perder, machete, dificultad mayor de regreso |

## Scripts

| Script | Qué hace |
|---|---|
| `MovimientoJugador.cs` | Movimiento con Rigidbody en `FixedUpdate` y parámetro del Animator (script de clase) + `Detener`, `Atrapado`, `Machetazo` |
| `FollowPlayer.cs` | Cámara que sigue al jugador (Lab 1) |
| `GestorPartida.cs` | Estado del juego: inicio, objetivo, escondites, machete, ganar/perder, UI |
| `Pterodactilo.cs` | Máquina de estados: Patrulla → Aviso → Picada → Subida / Llevar, con temporizadores |
| `Escondite.cs` | Trigger de sombra (`OnTriggerEnter` / `OnTriggerExit`) |
| `Objetivo.cs` | Trigger de la tienda y la casa |
| `OcultarSinTexto.cs` | Muestra el fondo del mensaje central solo cuando hay texto |

## Créditos

| Asset | Autor | Licencia |
|---|---|---|
| [Blocky Characters](https://kenney.nl/assets/blocky-characters) (jugador y animaciones) | Kenney | CC0 |
| [City Kit Suburban](https://kenney.nl/assets/city-kit-suburban) (casas, bardas, árboles, banqueta) | Kenney | CC0 |
| [City Kit Commercial](https://kenney.nl/assets/city-kit-commercial) (tienda, toldos, sombrillas) | Kenney | CC0 |
| [Particle Pack](https://kenney.nl/assets/particle-pack) (texturas de polvo y brillo) | Kenney | CC0 |
| [Low Spec Pterosaur](https://opengameart.org/node/131134) (pterodáctilo, convertido de .blend a FBX) | pistachio (OpenGameArt) | CC0 |
| Música [Desert Theme](https://opengameart.org/content/desert-theme) (`musica_desierto.ogg`, original `caravan.ogg`) | yd (OpenGameArt) | CC0 |
| Chillido del pterodáctilo (`chirrido_pterodactilo.mp3`) | freesound_community (Pixabay) | Pixabay Content License |

## Uso de IA

Se usó Claude (Anthropic) como apoyo:

- Conversión del pterodáctilo de `.blend` a FBX con Blender.
- Guía paso a paso para el escenario, los prefabs, los colliders, el movimiento y la cámara.
- Escritura de `Pterodactilo.cs`, `Escondite.cs`, `Objetivo.cs` y `OcultarSinTexto.cs`, y de los métodos `Detener`, `Atrapado` y `Machetazo` de `MovimientoJugador.cs`.
- `GestorPartida.cs` parte de la estructura del de la clase (`Ganar`, `Perder`, `Reiniciar` con `SceneManager`); la IA le agregó la pantalla de inicio, el objetivo de dos fases, el contador de escondites, el machete y los textos de la UI.

El detalle está en el reporte.
