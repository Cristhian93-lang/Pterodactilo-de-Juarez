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

    private enum Estado { Patrulla, Aviso, Picada, Subida }
    private Estado estado = Estado.Patrulla;
    private float tiempo;
    private float angulo;
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        tiempo = Random.Range(esperaMin, esperaMax);
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
                    tiempo = duracionAviso;
                    chillido.Play();
                    gestor.Avisar("¡Chillido! ¡Escóndete!");
                }
                break;

            case Estado.Aviso:
                VolarEnCirculo();
                tiempo -= Time.deltaTime;
                if (tiempo <= 0f)
                {
                    estado = Estado.Picada;
                    animator.SetBool("picada", true);   // cambia de aletear a planear
                }
                break;

            case Estado.Picada:
                // Persigue la posición actual del jugador
                Vector3 destino = jugador.position + Vector3.up;
                Mover(destino, velocidadPicada);
                if (Vector3.Distance(transform.position, destino) < 1f)
                {
                    polvo.transform.position = jugador.position;
                    polvo.Play();
                    if (gestor.Escondido) gestor.MostrarEstado();   // falló: el jugador estaba en la sombra
                    else gestor.Perder();
                    estado = Estado.Subida;
                    animator.SetBool("picada", false);
                }
                break;

            case Estado.Subida:
                Mover(PuntoDelCirculo(), velocidadPicada * 0.6f);
                if (Vector3.Distance(transform.position, PuntoDelCirculo()) < 1f)
                {
                    estado = Estado.Patrulla;
                    tiempo = Random.Range(esperaMin, esperaMax);
                }
                break;
        }
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
