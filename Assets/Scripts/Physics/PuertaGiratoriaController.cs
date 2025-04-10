using UnityEngine;

/// <summary>
///     Controlador para una puerta giratoria que permite la interacción física y la limitación de rotación.
///     Este script utiliza Rigidbody para aplicar fuerzas y torque a la puerta, y permite la interacción VR a través de una manija.
/// </summary>
/// <remarks>
///     Este script es parte de un sistema de puertas giratorias en un entorno de realidad virtual. Permite a los usuarios interactuar con la puerta giratoria utilizando una manija, y limita la rotación de la puerta dentro de un rango específico.
///     El script también aplica una fuerza de retorno para mantener la puerta en su posición inicial cuando no está siendo manipulada.
/// </remarks>
/// version>1.0</version>
public class PuertaGiratoriaController : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Configuración de Límites")]
    public float anguloMaximo = 90f;
    public float anguloMinimo = 0f;
    public float fuerzaRetorno = 50f;

    [Header("Configuración de Movimiento")]
    public float amortiguacion = 0.5f;
    public float velocidadMaxima = 90f;

    [Header("Interacción VR")]
    public bool usarManija = true;
    public Transform manija;

    private float anguloActual = 0f;
    private Quaternion rotacionInicial;

    /// <summary>
    ///     Método de inicialización del script.
    ///     Se obtiene el componente Rigidbody y se establece la rotación inicial de la puerta.
    ///     Si se usa una manija, se crea un objeto de manija y se configura para la interacción VR.
    /// </summary>
    /// <remarks>
    ///     Este método se llama al inicio del juego y es responsable de la configuración inicial del script.
    ///     Se asegura de que el Rigidbody esté presente y se inicializa la rotación inicial de la puerta.
    ///     Si se ha habilitado la opción de usar una manija, se crea un objeto de manija y se configura para la interacción VR.
    /// </remarks>
    /// <returns></returns>
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rotacionInicial = transform.rotation;

        if (usarManija && manija == null)
        {
            GameObject nuevaManija = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            nuevaManija.name = "Manija";
            nuevaManija.transform.parent = transform;
            nuevaManija.transform.localPosition = new Vector3(0.45f, 0, 0.4f);
            nuevaManija.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            Destroy(nuevaManija.GetComponent<Collider>());
            SphereCollider manijaCollider = nuevaManija.AddComponent<SphereCollider>();
            manijaCollider.radius = 1.2f;

            var grabInteractable = nuevaManija.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            grabInteractable.movementType = UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.MovementType.VelocityTracking;
            grabInteractable.throwOnDetach = false;

            manija = nuevaManija.transform;
        }
    }

    /// <summary>
    ///     Método de actualización física del script.
    ///     Se calcula el ángulo actual de la puerta y se aplica la limitación de rotación y la velocidad máxima.
    /// </summary>
    /// <remarks>
    ///     Este método se llama en cada frame de física y es responsable de actualizar el estado de la puerta giratoria.
    ///     Calcula el ángulo actual de la puerta en función de su rotación y aplica las limitaciones de rotación y velocidad máxima.
    ///     También aplica una fuerza de retorno para mantener la puerta en su posición inicial cuando no está siendo manipulada.
    /// </remarks>
    /// <returns></returns>
    void FixedUpdate()
    {
        anguloActual = Quaternion.Angle(rotacionInicial, transform.rotation) * (IsRotatingClockwise() ? 1 : -1);

        AplicarLimitesRotacion();

        LimitarVelocidad();
    }

    /// <summary>
    ///     Método para determinar si la puerta está girando en sentido horario o antihorario.
    /// </summary>
    /// <remarks>
    ///     Este método utiliza el producto cruzado entre el vector hacia adelante de la rotación inicial y el vector hacia adelante actual para determinar la dirección de rotación.
    ///     Si el producto cruzado apunta hacia arriba, se considera que la puerta está girando en sentido horario.
    /// </remarks>
    /// <returns></returns>
    private bool IsRotatingClockwise()
    {
        Vector3 forward = rotacionInicial * Vector3.forward;
        Vector3 currentForward = transform.rotation * Vector3.forward;
        Vector3 right = rotacionInicial * Vector3.right;

        return Vector3.Dot(Vector3.Cross(forward, currentForward), Vector3.up) > 0;
    }

    /// <summary>
    ///     Método para aplicar límites de rotación a la puerta giratoria.
    /// </summary>
    /// <remarks>
    ///     Este método verifica si el ángulo actual de la puerta está fuera de los límites establecidos (ángulo máximo y mínimo).
    ///     Si es así, calcula el ángulo objetivo y aplica un torque hacia ese ángulo.
    ///     También aplica una amortiguación a la velocidad angular actual para suavizar el movimiento de la puerta.
    /// </remarks>
    /// <returns></returns>
    private void AplicarLimitesRotacion()
    {
        if (anguloActual > anguloMaximo || anguloActual < anguloMinimo)
        {
            float targetAngle = (anguloActual > anguloMaximo) ? anguloMaximo : anguloMinimo;

            Quaternion targetRotation = rotacionInicial * Quaternion.Euler(0, targetAngle, 0);

            Vector3 directionToTarget = Vector3.Cross(transform.forward, targetRotation * Vector3.forward);
            rb.AddTorque(directionToTarget.normalized * fuerzaRetorno);

            rb.angularVelocity *= (1 - amortiguacion * Time.fixedDeltaTime);
        }
    }

    /// <summary>
    ///     Método para limitar la velocidad angular de la puerta giratoria.
    /// </summary>
    /// <remarks>
    ///     Este método verifica la velocidad angular actual de la puerta y la limita a un valor máximo establecido.
    ///     Si la velocidad angular actual excede el valor máximo, se ajusta la velocidad angular a ese valor máximo.
    /// </remarks>
    /// <returns></returns>
    private void LimitarVelocidad()
    {
        float velocidadAngularActual = rb.angularVelocity.magnitude * Mathf.Rad2Deg;

        if (velocidadAngularActual > velocidadMaxima)
        {
            rb.angularVelocity = rb.angularVelocity.normalized * (velocidadMaxima * Mathf.Deg2Rad);
        }
    }

    /// <summary>
    ///     Método para aplicar una fuerza de torque a la puerta giratoria.
    /// </summary>
    /// <remarks>
    ///     Este método permite aplicar una fuerza de torque a la puerta giratoria desde un script externo.
    ///     Se utiliza el método AddTorque del Rigidbody para aplicar la fuerza en la dirección deseada.
    /// </remarks>
    /// <param name="fuerza">La fuerza de torque a aplicar.</param>
    /// <returns></returns>
    public void AplicarFuerza(float fuerza)
    {
        rb.AddTorque(Vector3.up * fuerza, ForceMode.Impulse);
    }

    /// <summary>
    ///     Método para dibujar gizmos en la escena para visualizar la puerta giratoria.
    /// </summary>
    /// <remarks>
    ///     Este método se utiliza para dibujar gizmos en la escena de Unity, lo que permite visualizar la posición y rotación de la puerta giratoria.
    ///     Se dibujan líneas que representan la rotación inicial y los límites de rotación de la puerta.
    /// </remarks>
    /// <returns></returns>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 2);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Vector3 min = rotacionInicial * Quaternion.Euler(0, anguloMinimo, 0) * Vector3.forward;
            Vector3 max = rotacionInicial * Quaternion.Euler(0, anguloMaximo, 0) * Vector3.forward;
            Gizmos.DrawLine(transform.position, transform.position + min);
            Gizmos.DrawLine(transform.position, transform.position + max);
        }
    }
}
