using UnityEngine;

/// <summary>
///     Clase que detecta la entrada de un objeto en un trigger y activa el movimiento de una plataforma móvil.
///     Se utiliza para activar el movimiento de la plataforma cuando el jugador o un objeto interactivo entra en el trigger.
/// </summary>
/// <remarks>
///     Esta clase se utiliza para detectar la entrada de un objeto en un trigger y activar el movimiento de una plataforma móvil.
///     Se utiliza en combinación con la clase PlataformaMovilController para controlar el movimiento de la plataforma.
/// </remarks>
/// <version>1.0</version>
public class DetectorTrigger : MonoBehaviour
{
    public PlataformaMovilController plataforma;

    /// <summary>
    ///     Método que se llama al iniciar el script.
    /// </summary>
    /// <remarks>
    ///    Este método se llama al iniciar el script y se utiliza para inicializar la variable plataforma.
    /// </remarks>
    /// <returns></returns>
    private void Start()
    {
        if (plataforma == null)
        {
            plataforma = Object.FindFirstObjectByType<PlataformaMovilController>();
        }
    }

    /// <summary>
    ///     Método que se llama cuando un objeto entra en el trigger.
    /// </summary>
    /// <remarks>
    ///     Este método se llama cuando un objeto entra en el trigger y activa el movimiento de la plataforma móvil.
    /// </remarks>
    /// <param name="other">El objeto que ha entrado en el trigger.</param>
    /// <returns></returns>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("XRRig") || other.CompareTag("XRController"))
        {
            if (plataforma != null)
            {
                plataforma.AlternarDestino();
            }
        }
    }
}
