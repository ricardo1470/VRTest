using NUnit.Framework;
using UnityEngine;

public class SimulacionFisicaAvanzadaTests
{
    [Test]
    public void Objeto_Tiene_Rigidbody_Y_Gravedad_Activada()
    {
        var objeto = new GameObject("ObjetoFisico");
        var simulacion = objeto.AddComponent<SimulacionFisicaAvanzada>();
        var rb = objeto.AddComponent<Rigidbody>();

        Assert.IsNotNull(rb, "El objeto no tiene Rigidbody.");
        Assert.IsTrue(rb.useGravity, "La gravedad no está activada en el Rigidbody.");
    }
}
