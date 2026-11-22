using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Muestra el fondo oscuro del mensaje central solo cuando hay texto (inicio, victoria o derrota).
/// </summary>
public class OcultarSinTexto : MonoBehaviour
{
    public Text texto;   // el texto del mensaje central
    private Image fondo;

    void Start()
    {
        fondo = GetComponent<Image>();
    }

    void Update()
    {
        fondo.enabled = texto.text != "";
    }
}
