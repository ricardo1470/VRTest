using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class SimulacionFisicaAvanzadaPlaymodeTests
{
    private GameObject objetoFisico;
    private SimulacionFisicaAvanzada simulacion;
    private Rigidbody rb;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        objetoFisico = new GameObject("ObjetoFisico");
        simulacion = objetoFisico.AddComponent<SimulacionFisicaAvanzada>();
        rb = objetoFisico.AddComponent<Rigidbody>();
        rb.useGravity = true;

        simulacion.desactivarInput = true; // 👈 esto evita el error

        yield return null;
    }


    [UnityTest]
    public IEnumerator AplicarFuerza_Impulso_Aleatorio_Mueve_Objeto()
    {
        Vector3 posicionInicial = rb.position;

        simulacion.AplicarFuerza();
        yield return new WaitForFixedUpdate(); // Esperar un paso de física

        Assert.AreNotEqual(posicionInicial, rb.position, "El objeto no se movió tras aplicar fuerza.");
    }

    [UnityTest]
    public IEnumerator SimularFisicaAvanzada_No_Afecta_Posicion_Final()
    {
        Physics.simulationMode = SimulationMode.Script;

        Vector3 posicionOriginal = rb.position;
        Quaternion rotacionOriginal = rb.rotation;

        simulacion.AplicarFuerza();
        yield return new WaitForFixedUpdate();

        // Verificar que la posición y rotación se restablecen después de SimularFisicaAvanzada
        Assert.AreEqual(posicionOriginal, rb.position, "La posición no se restableció correctamente.");
        Assert.AreEqual(rotacionOriginal, rb.rotation, "La rotación no se restableció correctamente.");

        Physics.simulationMode = SimulationMode.FixedUpdate;
    }

    [UnityTest]
    public IEnumerator AplicarFuerzaDesdeVR_Impulso_Direccional()
    {
        Vector3 direccion = new Vector3(0, 1, 0); // Fuerza hacia arriba
        Vector3 posicionInicial = rb.position;

        simulacion.AplicarFuerzaDesdeVR(direccion);
        yield return new WaitForFixedUpdate();

        Assert.IsTrue(rb.position.y > posicionInicial.y, "El objeto no se movió hacia arriba con la fuerza VR.");
    }

    [UnityTest]
    public IEnumerator Fuerza_Extrema_No_Rompe_Simulacion()
    {
        simulacion.fuerzaImpulso = 1000f; // Fuerza muy alta
        simulacion.AplicarFuerza();

        yield return new WaitForFixedUpdate();
        Assert.DoesNotThrow(() => Physics.Simulate(Time.fixedDeltaTime), "La simulación colapsó con fuerza extrema.");
    }

    [UnityTearDown]
    public IEnumerator Teardown()
    {
        Object.Destroy(objetoFisico);
        yield return null;
    }
}
