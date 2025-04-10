using UnityEngine;

/// <summary>
///     Clase que simula la física avanzada de un objeto utilizando Rigidbody en Unity.
///     Permite aplicar fuerzas y simular varios pasos de física para predecir y ajustar la trayectoria del objeto.
/// </summary>
/// <remarks>
///     Este script es parte de un sistema de simulación física avanzada en Unity. Permite aplicar fuerzas a un objeto y simular varios pasos de física para predecir y ajustar su trayectoria.
///     Se utiliza principalmente para objetos que requieren una simulación física más precisa, como proyectiles o vehículos.
/// </remarks>
/// <version>1.0</version>
public class SimulacionFisicaAvanzada : MonoBehaviour
{
    private Rigidbody rb;
    public float fuerzaImpulso = 5f;
    public int pasosFisicos = 3;

    public bool desactivarInput = false;


    /// <summary>
    ///     Método de inicialización del script.
    ///    Se obtiene el componente Rigidbody y se inicializa la simulación física.
    /// </summary>
    /// <remarks>
    ///    Este método se llama al inicio del juego y es responsable de la configuración inicial del script.
    ///   Se asegura de que el Rigidbody esté presente y se inicializa la simulación física.
    /// </remarks>
    /// <returns></returns>
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    ///     Método que se llama en cada frame del juego.
    ///     Se utiliza para aplicar una fuerza al objeto cuando se presiona la tecla de espacio.
    /// </summary>
    /// <remarks>
    ///     Este método se llama en cada frame del juego y se utiliza para aplicar una fuerza al objeto cuando se presiona la tecla de espacio.
    /// </remarks>
    /// <returns></returns>
    private void Update()
    {
        if (desactivarInput) return;

        if (EstaEspacioPresionado())
        {
            AplicarFuerza();
        }
    }

    protected virtual bool EstaEspacioPresionado()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    /// <summary>
    ///     Método que aplica una fuerza al objeto en una dirección aleatoria.
    ///     Se utiliza para simular un impulso en el objeto.
    /// </summary>
    /// <remarks>
    ///    Este método se llama cuando se presiona la tecla de espacio y se utiliza para aplicar una fuerza al objeto en una dirección aleatoria.
    ///    Se utiliza para simular un impulso en el objeto.
    /// </remarks>
    /// <returns></returns>
    public void AplicarFuerza()
    {
        Vector3 direccionAleatoria = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(0.5f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;

        rb.AddForce(direccionAleatoria * fuerzaImpulso, ForceMode.Impulse);

        SimularFisicaAvanzada();
    }

    /// <summary>
    ///     Método que simula la física avanzada del objeto.
    ///    Se utiliza para predecir y ajustar la trayectoria del objeto.
    /// </summary>
    /// <remarks>
    ///     Este método simula la física avanzada del objeto utilizando el componente Rigidbody.
    ///    Se utiliza para predecir y ajustar la trayectoria del objeto en función de la fuerza aplicada.
    /// </remarks>
    /// <returns></returns>
    private void SimularFisicaAvanzada()
    {
        Vector3 posicionOriginal = rb.position;
        Quaternion rotacionOriginal = rb.rotation;
        Vector3 velocidadOriginal = rb.linearVelocity;
        Vector3 velocidadAngularOriginal = rb.angularVelocity;

        for (int i = 0; i < pasosFisicos; i++)
        {
            Physics.Simulate(Time.fixedDeltaTime / pasosFisicos);
        }


        rb.position = posicionOriginal;
        rb.rotation = rotacionOriginal;
        rb.linearVelocity = velocidadOriginal;
        rb.angularVelocity = velocidadAngularOriginal;

    }

    /// <summary>
    ///     Método que aplica una fuerza al objeto desde un controlador VR.
    ///     Se utiliza para simular un impulso en el objeto desde un controlador VR.
    /// </summary>
    /// <remarks>
    ///     Este método se llama desde un controlador VR y se utiliza para aplicar una fuerza al objeto en una dirección específica.
    ///    Se utiliza para simular un impulso en el objeto desde un controlador VR.
    /// </remarks>
    /// <param name="direccion">La dirección en la que se aplica la fuerza.</param>
    /// <returns></returns>
    public void AplicarFuerzaDesdeVR(Vector3 direccion)
    {
        GetComponent<Rigidbody>().AddForce(direccion.normalized * fuerzaImpulso, ForceMode.Impulse);
    }
}
