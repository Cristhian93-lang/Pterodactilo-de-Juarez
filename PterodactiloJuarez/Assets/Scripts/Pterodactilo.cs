using UnityEngine;

/// <summary>
/// El pterodáctilo vuela en círculos sobre el jugador. Cada cierto tiempo (temporizador como en el Lab 3)
/// chilla para avisar y después baja en picada. Si al llegar el jugador no está escondido, pierde.
/// </summary>
public class Pterodactilo : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;
    public GestorPartida gestor;
    public AudioSource chillido;
    public ParticleSystem polvo;   // nube de polvo donde aterriza la picada

    [Header("Vuelo")]
    public float altura = 7f;
    public float radio = 8f;
    public Vector3 centro = new Vector3(0f, 0f, 6f);   // el círculo va un poco delante del jugador para que la cámara lo vea
    public float velocidadGiro = 0.4f;      // radianes por segundo
    public float velocidadPicada = 20f;

    [Header("Tiempos (segundos)")]
    public float esperaMin = 6f;
    public float esperaMax = 10f;
    public float duracionAviso = 2.5f;      // tiempo para esconderse después del chillido

    [Header("Regreso a casa (más difícil)")]
    public float esperaRegresoMin = 4f;
    public float esperaRegresoMax = 7f;
    public float avisoRegreso = 1.8f;

    private enum Estado { Patrulla, Aviso, Picada, Subida, Llevar }
    private Estado estado = Estado.Patrulla;
    private float tiempo;
    private float angulo;
    private Animator animator;
    private Renderer modelo;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        modelo = GetComponentInChildren<Renderer>();
        tiempo = SiguienteEspera();
        transform.position = PuntoDelCirculo();
    }

    void Update()
    {
        switch (estado)
        {
            case Estado.Patrulla:
                VolarEnCirculo();
                if (gestor.Jugando) tiempo -= Time.deltaTime;   // solo ataca durante la partida
                if (tiempo <= 0f)
                {
                    estado = Estado.Aviso;
                    tiempo = gestor.EnRegreso ? avisoRegreso : duracionAviso;
                    chillido.Play();
                    gestor.Avisar("¡Chillido! ¡Escóndete!");
                }
                break;

            case Estado.Aviso:
                VolarEnCirculo();
                if (gestor.TieneMachete && Input.GetKeyDown(KeyCode.Space)) { Espantar(); break; }
                tiempo -= Time.deltaTime;
                if (tiempo <= 0f)
                {
                    estado = Estado.Picada;
                    animator.SetBool("picada", true);   // cambia de aletear a planear
                }
                break;

            case Estado.Picada:
                if (gestor.TieneMachete && Input.GetKeyDown(KeyCode.Space)) { Espantar(); break; }
                // Persigue la posición actual del jugador
                Vector3 destino = jugador.position + Vector3.up;
                Mover(destino, velocidadPicada);
                // Si está escondido no lo ve: se regresa antes de chocar (cambio por el playtest)
                if (gestor.Escondido && Vector3.Distance(transform.position, destino) < 5f)
                {
                    animator.SetBool("picada", false);
                    gestor.MostrarEstado();
                    estado = Estado.Subida;
                    break;
                }
                if (Vector3.Distance(transform.position, destino) < 1f)
                {
                    polvo.transform.position = jugador.position;
                    polvo.Play();
                    animator.SetBool("picada", false);
                    if (gestor.Escondido)
                    {
                        gestor.MostrarEstado();   // falló: el jugador estaba en la sombra
                        estado = Estado.Subida;
                    }
                    else
                    {
                        gestor.Perder();
                        // Lo atrapa: el Rigidbody pasa a kinematic para que la física no lo jale hacia abajo
                        Rigidbody rb = jugador.GetComponent<Rigidbody>();
                        rb.isKinematic = true;
                        rb.interpolation = RigidbodyInterpolation.None;   // sin interpolación para que no se quede atrás
                        estado = Estado.Llevar;
                    }
                }
                break;

            case Estado.Subida:
                Mover(PuntoDelCirculo(), velocidadPicada * 0.6f);
                if (Vector3.Distance(transform.position, PuntoDelCirculo()) < 1f)
                {
                    estado = Estado.Patrulla;
                    tiempo = SiguienteEspera();
                    gestor.MostrarEstado();
                }
                break;

            case Estado.Llevar:
                // Se eleva y se aleja llevando al jugador colgado de las patas
                Vector3 adelante = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(adelante + Vector3.up * 0.5f), 3f * Time.deltaTime);
                transform.position += (Vector3.up * 6f + adelante * 5f) * Time.deltaTime;
                // Lo cuelga justo debajo del cuerpo del modelo (no del pivote, que está desplazado)
                Bounds cuerpo = modelo.bounds;
                jugador.position = new Vector3(cuerpo.center.x, cuerpo.center.y - 1.9f, cuerpo.center.z);
                break;
        }
    }

    // De regreso a casa ataca más seguido
    float SiguienteEspera()
    {
        if (gestor.EnRegreso) return Random.Range(esperaRegresoMin, esperaRegresoMax);
        return Random.Range(esperaMin, esperaMax);
    }

    // El jugador lo espanta con el machete: cancela el ataque y vuelve a subir
    void Espantar()
    {
        gestor.UsarMachete();
        gestor.Avisar("¡Lo espantaste con el machete!");
        animator.SetBool("picada", false);
        chillido.Play();
        estado = Estado.Subida;
    }

    // Punto del círculo alrededor del jugador, a la altura de vuelo
    Vector3 PuntoDelCirculo()
    {
        Vector3 offset = new Vector3(Mathf.Cos(angulo), 0f, Mathf.Sin(angulo)) * radio;
        return jugador.position + centro + offset + Vector3.up * altura;
    }

    void VolarEnCirculo()
    {
        angulo += velocidadGiro * Time.deltaTime;
        Vector3 objetivo = PuntoDelCirculo();
        Vector3 direccion = objetivo - transform.position;
        transform.position = Vector3.Lerp(transform.position, objetivo, 3f * Time.deltaTime);
        if (direccion.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direccion), 5f * Time.deltaTime);
    }

    void Mover(Vector3 destino, float velocidad)
    {
        transform.position = Vector3.MoveTowards(transform.position, destino, velocidad * Time.deltaTime);
        Vector3 direccion = destino - transform.position;
        if (direccion.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(direccion);
    }
}
