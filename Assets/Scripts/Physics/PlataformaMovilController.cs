using System.Collections;
using UnityEngine;

/// <summary>
///     Clase que controla el movimiento de una plataforma móvil en Unity utilizando Rigidbody y corutinas.
///     Permite mover la plataforma entre dos puntos definidos y alternar su destino al entrar en un trigger.
/// </summary>
/// <remarks>
///     Esta clase se utiliza para controlar el movimiento de una plataforma móvil en Unity utilizando Rigidbody y corutinas.
///     Permite mover la plataforma entre dos puntos definidos y alternar su destino al entrar en un trigger.
/// </remarks>
/// <version>1.0</version>
public class PlataformaMovilController : MonoBehaviour
{
    public Transform puntoInicial;
    public Transform puntoFinal;
    public float velocidad = 2.0f;
    public float tiempoEspera = 1.0f;

    public bool enMovimiento = false;
    private Vector3 posicionDestino;
    private Rigidbody rb;

    /// <summary>
    ///     Método que se llama al iniciar el script.
    /// </summary>
    /// <remarks>
    ///    Este método se llama al iniciar el script y se utiliza para inicializar la variable rb y los puntos de destino.
    /// </remarks>
    /// <returns></returns>
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (puntoInicial == null)
        {
            GameObject inicio = new GameObject("PuntoInicial");
            inicio.transform.position = transform.position;
            puntoInicial = inicio.transform;
        }

        if (puntoFinal == null)
        {
            GameObject final = new GameObject("PuntoFinal");
            final.transform.position = transform.position + new Vector3(0, 3, 0);
            puntoFinal = final.transform;
        }

        posicionDestino = puntoInicial.position;
    }

    /// <summary>
    ///     Método que se llama en cada frame fijo del juego.
    /// </summary>
    /// <remarks>
    ///    Este método se llama en cada frame fijo del juego y se utiliza para mover la plataforma hacia el destino definido.
    /// </remarks>
    /// <returns></returns>
    void FixedUpdate()
    {
        if (enMovimiento)
        {
            float paso = velocidad * Time.fixedDeltaTime;
            rb.MovePosition(Vector3.MoveTowards(rb.position, posicionDestino, paso));

            if (Vector3.Distance(rb.position, posicionDestino) < 0.01f)
            {
                enMovimiento = false;
            }
        }
    }

    /// <summary>
    ///     Método que mueve la plataforma hacia un destino específico.
    /// </summary>
    /// <remarks>
    ///     Este método mueve la plataforma hacia un destino específico y activa el movimiento de la plataforma.
    /// </remarks>
    /// <param name="destino">El destino al que se moverá la plataforma.</param>
    /// <returns></returns>
    public void MoverHacia(Vector3 destino)
    {
        posicionDestino = destino;
        enMovimiento = true;
    }

    /// <summary>
    ///     Método que alterna el destino de la plataforma entre los puntos inicial y final.
    /// </summary>
    /// <remarks>
    ///    Este método alterna el destino de la plataforma entre los puntos inicial y final y activa el movimiento de la plataforma.
    /// </remarks>
    /// <returns></returns>
    public void AlternarDestino()
    {
        if (Vector3.Distance(rb.position, puntoInicial.position) < 0.01f)
        {
            StartCoroutine(EsperarYMover(puntoFinal.position));
        }
        else if (Vector3.Distance(rb.position, puntoFinal.position) < 0.01f)
        {
            StartCoroutine(EsperarYMover(puntoInicial.position));
        }
        else
        {
            float distanciaInicio = Vector3.Distance(rb.position, puntoInicial.position);
            float distanciaFinal = Vector3.Distance(rb.position, puntoFinal.position);

            if (distanciaInicio <= distanciaFinal)
            {
                StartCoroutine(EsperarYMover(puntoFinal.position));
            }
            else
            {
                StartCoroutine(EsperarYMover(puntoInicial.position));
            }
        }
    }

    /// <summary>
    ///     Método que espera un tiempo definido antes de mover la plataforma hacia un destino específico.
    /// </summary>
    /// <remarks>
    ///     Este método espera un tiempo definido antes de mover la plataforma hacia un destino específico.
    /// </remarks>
    /// <param name="destino">El destino al que se moverá la plataforma.</param>
    IEnumerator EsperarYMover(Vector3 destino)
    {
        yield return new WaitForSeconds(tiempoEspera);
        MoverHacia(destino);
    }
}
