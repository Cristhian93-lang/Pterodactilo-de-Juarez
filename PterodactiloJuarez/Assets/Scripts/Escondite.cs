using UnityEngine;

/// <summary>
/// Zona de sombra (trigger) bajo un árbol o sombrilla.
/// Mientras el jugador está adentro, el pterodáctilo no lo puede atrapar.
/// </summary>
public class Escondite : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            Object.FindAnyObjectByType<GestorPartida>().EntrarEscondite();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            Object.FindAnyObjectByType<GestorPartida>().SalirEscondite();
    }
}
