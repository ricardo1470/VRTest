using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class PlataformaMovilPlaymodeTests
{
    private PlataformaMovilController plataforma;
    private Transform puntoInicial;
    private Transform puntoFinal;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Crear plataforma y puntos
        plataforma = new GameObject("Plataforma").AddComponent<PlataformaMovilController>();
        plataforma.gameObject.AddComponent<Rigidbody>().isKinematic = true;

        puntoInicial = new GameObject("PuntoInicial").transform;
        puntoFinal = new GameObject("PuntoFinal").transform;

        // Configurar posiciones
        puntoInicial.position = Vector3.zero;
        puntoFinal.position = new Vector3(0, 5, 0);

        plataforma.puntoInicial = puntoInicial;
        plataforma.puntoFinal = puntoFinal;
        plataforma.velocidad = 5f;
        plataforma.tiempoEspera = 0.5f;

        yield return null;
    }

    [UnityTest]
    public IEnumerator MoverHacia_PuntoFinal_Actualiza_Posicion()
    {
        Vector3 destino = puntoFinal.position;
        plataforma.MoverHacia(destino);

        yield return new WaitForSeconds(1f); // Tiempo suficiente para moverse
        float distancia = Vector3.Distance(plataforma.transform.position, destino);
        Assert.Less(distancia, 0.01f, $"La plataforma no llegó al punto final. Distancia: {distancia}");
    }

    [UnityTest]
    public IEnumerator Plataforma_Se_Detiene_Al_Llegar_A_Destino()
    {
        plataforma.MoverHacia(puntoFinal.position);
        yield return new WaitForSeconds(2f);

        // Verificar que enMovimiento es false
        Assert.IsFalse(plataforma.enMovimiento, "La plataforma no se detuvo al llegar al destino.");
    }

    [UnityTest]
    public IEnumerator Cambiar_Direccion_Movimiento()
    {
        plataforma.MoverHacia(puntoFinal.position);
        yield return new WaitForSeconds(1f); // Esperar un tiempo para que inicie el movimiento

        // Cambiar dirección
        plataforma.MoverHacia(puntoInicial.position);
        yield return new WaitForSeconds(1f); // Esperar un tiempo para que inicie el movimiento

        float distancia = Vector3.Distance(plataforma.transform.position, puntoInicial.position);
        Assert.Less(distancia, 0.01f, $"La plataforma no llegó al punto inicial. Distancia: {distancia}");
    }

    [UnityTearDown]
    public IEnumerator Teardown()
    {
        Object.Destroy(plataforma.gameObject);
        Object.Destroy(puntoInicial.gameObject);
        Object.Destroy(puntoFinal.gameObject);
        yield return null;
    }
}