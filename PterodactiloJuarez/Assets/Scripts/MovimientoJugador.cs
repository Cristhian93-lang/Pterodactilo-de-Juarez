using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovimientoJugador : MonoBehaviour
{
    public float velocidad = 5f;

    private Rigidbody rb;
    private Animator animator;
    private Vector3 movimiento;
    private bool detenido = false;   // true cuando la partida terminó

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (detenido) return;

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        movimiento = new Vector3(x, 0f, z).normalized;

        // El script no elige la animación: solo informa. Decidir es del Animator.
        if (animator != null)
            animator.SetFloat("velocidad", movimiento.magnitude);

        // Mirar hacia donde camina
        if (movimiento.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Slerp(
                transform.rotation, Quaternion.LookRotation(movimiento), 12f * Time.deltaTime);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movimiento * velocidad * Time.fixedDeltaTime);
    }

    // Lo llama el GestorPartida al ganar: deja de moverse y vuelve a idle
    public void Detener()
    {
        detenido = true;
        movimiento = Vector3.zero;
        if (animator != null) animator.SetFloat("velocidad", 0f);
    }

    // Lo llama el GestorPartida al perder: se detiene y reproduce la animación "die"
    public void Atrapado()
    {
        Detener();
        if (animator != null) animator.SetTrigger("atrapado");
    }
}
