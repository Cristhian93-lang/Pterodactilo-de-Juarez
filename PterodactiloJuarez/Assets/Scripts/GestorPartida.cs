using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla el loop del juego: pantalla de inicio, objetivo (tienda y luego casa),
/// si el jugador está escondido, y ganar / perder / reiniciar.
/// Basado en el GestorPartida de la clase (Ganar, Perder, Reiniciar con SceneManager).
/// </summary>
public class GestorPartida : MonoBehaviour
{
    [Header("UI")]
    public Text textoObjetivo;   // arriba a la izquierda: qué hay que hacer
    public Text textoEstado;     // arriba al centro: escondido / expuesto / aviso
    public Text textoMensaje;    // centro: inicio, victoria, derrota

    [Header("Marcas del objetivo (partículas)")]
    public ParticleSystem marcaTienda;
    public ParticleSystem marcaCasa;

    [Header("Jugador")]
    public MovimientoJugador jugador;
    public GameObject macheteEnMano;   // modelo del machete en la mano (oculto hasta la tienda)

    // static: sobrevive al recargar la escena, así al reiniciar no sale otra vez la pantalla de inicio
    private static bool yaEmpezo = false;

    private bool fueALaTienda = false;
    private bool terminada = false;
    private bool tieneMachete = false;
    private int escondites = 0;   // en cuántos escondites está parado ahora (pueden traslaparse)

    public bool Escondido { get { return escondites > 0; } }
    public bool Terminada { get { return terminada; } }
    public bool Jugando { get { return yaEmpezo && !terminada; } }
    public bool TieneMachete { get { return tieneMachete; } }
    public bool EnRegreso { get { return fueALaTienda; } }

    void Start()
    {
        if (yaEmpezo) Empezar();
        else
        {
            Time.timeScale = 0f;   // congela el juego hasta presionar Espacio
            textoMensaje.text = "PTERODÁCTILO DE JUÁREZ\n\n<size=36>Ve a la tienda y regresa a casa.\nCuando escuches el chillido, ¡escóndete bajo un árbol o una sombrilla!\n\nWASD / Flechas para moverte\n\nPresiona ESPACIO para empezar</size>";
        }
        macheteEnMano.SetActive(false);
        ActualizarObjetivo();
    }

    void Update()
    {
        if (!yaEmpezo && Input.GetKeyDown(KeyCode.Space)) { yaEmpezo = true; Empezar(); }
        if (terminada && Input.GetKeyDown(KeyCode.R)) Reiniciar();
    }

    void Empezar()
    {
        Time.timeScale = 1f;
        textoMensaje.text = "";
        MostrarEstado();
    }

    // ---- Escondites (los llama Escondite.cs) ----
    public void EntrarEscondite() { escondites++; MostrarEstado(); }
    public void SalirEscondite()  { escondites--; MostrarEstado(); }

    public void MostrarEstado()
    {
        if (terminada) return;
        textoEstado.text = Escondido ? "<color=#7CFC7C>Escondido</color>" : "<color=#FFD27F>A la vista</color>";
    }

    public void Avisar(string texto)
    {
        if (!terminada) textoEstado.text = "<color=#FF6B5B>" + texto + "</color>";
    }

    // ---- Objetivo (los llama Objetivo.cs) ----
    public void LlegarATienda()
    {
        if (fueALaTienda || terminada) return;
        fueALaTienda = true;
        tieneMachete = true;              // en la tienda compra un machete para el regreso
        macheteEnMano.SetActive(true);
        ActualizarObjetivo();
    }

    public void LlegarACasa()
    {
        if (fueALaTienda) Ganar();
    }

    void ActualizarObjetivo()
    {
        textoObjetivo.text = fueALaTienda ? "Objetivo 2/2: regresa a casa" : "Objetivo 1/2: ve a la tienda";
        if (tieneMachete) textoObjetivo.text += "\n<size=32><color=#FFD27F>Machete: presiona ESPACIO cuando chille</color></size>";
        // La marca de partículas solo brilla sobre el objetivo actual
        if (fueALaTienda) { marcaTienda.Stop(); marcaCasa.Play(); }
        else              { marcaTienda.Play(); marcaCasa.Stop(); }
    }

    // ---- Machete (lo llama Pterodactilo.cs): un solo uso ----
    public void UsarMachete()
    {
        tieneMachete = false;
        jugador.Machetazo();
        Invoke("GuardarMachete", 1f);   // deja ver el golpe antes de guardarlo
        ActualizarObjetivo();
    }

    void GuardarMachete()
    {
        macheteEnMano.SetActive(false);
    }

    // ---- Final ----
    public void Ganar()
    {
        if (terminada) return;
        terminada = true;
        jugador.Detener();
        marcaCasa.Stop();
        textoEstado.text = "";
        textoMensaje.text = "¡LLEGASTE A CASA!\n<size=36>Sobreviviste al Pterodáctilo de Juárez\n\nPresiona R para jugar otra vez</size>";
    }

    public void Perder()
    {
        if (terminada) return;
        terminada = true;
        jugador.Atrapado();
        textoEstado.text = "";
        Invoke("MostrarDerrota", 1.5f);   // primero se ve cómo se lo lleva volando
    }

    void MostrarDerrota()
    {
        textoMensaje.text = "<color=#FF6B5B>¡TE ATRAPÓ!</color>\n<size=36>Te quedaste a la vista\n\nPresiona R para reintentar</size>";
    }

    void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
