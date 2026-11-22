using UnityEngine;

/// <summary>
/// Zona trigger frente a la tienda o la casa. Avisa al GestorPartida cuando llega el jugador.
/// Igual que Salida.cs de la clase (OnTriggerEnter + CompareTag).
/// </summary>
public class Objetivo : MonoBehaviour
{
    public bool esTienda = true;   // true = tienda, false = casa

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GestorPartida gestor = Object.FindAnyObjectByType<GestorPartida>();
        if (esTienda) gestor.LlegarATienda();
        else gestor.LlegarACasa();
    }
}
