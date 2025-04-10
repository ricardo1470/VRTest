using NUnit.Framework;
using UnityEngine;

public class PlataformaMovilEditorTests
{
    [Test]
    public void Plataforma_Tiene_Rigidbody_Y_Es_Kinematico()
    {
        var plataforma = new GameObject("Plataforma").AddComponent<PlataformaMovilController>();
        var rb = plataforma.gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        Assert.IsNotNull(rb, "El Rigidbody no está asignado.");
        Assert.IsTrue(rb.isKinematic, "El Rigidbody no es cinemático.");
    }

    [Test]
    public void Plataforma_Tiene_Puntos_Inicial_Y_Final()
    {
        var plataforma = new GameObject("Plataforma").AddComponent<PlataformaMovilController>();
        var puntoInicial = new GameObject("PuntoInicial").transform;
        var puntoFinal = new GameObject("PuntoFinal").transform;

        plataforma.puntoInicial = puntoInicial;
        plataforma.puntoFinal = puntoFinal;

        Assert.IsNotNull(plataforma.puntoInicial, "El Punto Inicial no está asignado.");
        Assert.IsNotNull(plataforma.puntoFinal, "El Punto Final no está asignado.");
    }
}
