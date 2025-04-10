using NUnit.Framework;
using UnityEngine;

public class PuertaGiratoriaEditorTests
{
    [Test]
    public void Puerta_Tiene_HingeJoint_Y_Limites_Configurados()
    {
        var puerta = new GameObject("Puerta").AddComponent<Rigidbody>();
        var hinge = puerta.gameObject.AddComponent<HingeJoint>();

        // Configurar límites de rotación (ejemplo: -90° a 90°)
        hinge.useLimits = true;
        JointLimits limits = new JointLimits { min = -90, max = 90 };
        hinge.limits = limits;

        Assert.IsTrue(hinge.useLimits, "Los límites del HingeJoint no están activados.");
        Assert.AreEqual(-90, hinge.limits.min, "Límite mínimo incorrecto.");
        Assert.AreEqual(90, hinge.limits.max, "Límite máximo incorrecto.");
    }
}