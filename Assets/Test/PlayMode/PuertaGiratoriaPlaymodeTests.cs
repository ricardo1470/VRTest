using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class PuertaGiratoriaPlaymodeTests
{
    private GameObject puerta;
    private HingeJoint hinge;
    private Rigidbody rb;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        puerta = new GameObject("Puerta");
        rb = puerta.AddComponent<Rigidbody>();
        hinge = puerta.AddComponent<HingeJoint>();
        puerta.AddComponent<BoxCollider>();

        rb.isKinematic = false;
        rb.mass = 10f;
        rb.angularDamping = 0.05f;

        hinge.useLimits = true;
        hinge.limits = new JointLimits { min = -90, max = 90 };
        hinge.anchor = Vector3.zero;
        hinge.axis = Vector3.up;



        yield return null;
    }


    [UnityTest]
    public IEnumerator Puerta_No_Excede_Limites_Con_Fuerza_Aplicada()
    {
        rb.AddTorque(Vector3.up * 50f, ForceMode.Impulse);
        yield return new WaitForSeconds(2);

        float anguloActual = hinge.angle;

        // Comparaciones tolerantes
        Assert.That(anguloActual, Is.LessThanOrEqualTo(90f + 0.01f), "La puerta superó el límite máximo.");
        Assert.That(anguloActual, Is.GreaterThanOrEqualTo(-90f - 0.01f), "La puerta superó el límite mínimo.");
    }


    [UnityTest]
    public IEnumerator Puerta_Gira_Cuando_Se_Activa()
    {
        // Simular activación de la puerta
        rb.AddTorque(Vector3.up * 50f, ForceMode.Impulse);
        yield return new WaitForSeconds(1); // Esperar a que gire

        float anguloActual = hinge.angle;
        Assert.Greater(anguloActual, 0, "La puerta no giró al ser activada.");
    }

    [UnityTest]
    public IEnumerator Puerta_No_Gira_Cuando_Desactivada()
    {
        // Desactivar la puerta (simular que no se puede mover)
        rb.isKinematic = true;
        rb.angularVelocity = Vector3.zero; // Asegurarse de que no haya movimiento

        yield return new WaitForSeconds(1); // Esperar un tiempo

        float anguloActual = hinge.angle;
        Assert.AreEqual(0, anguloActual, "La puerta giró cuando debería estar desactivada.");
    }

    [UnityTest]
    public IEnumerator Puerta_Gira_En_Direccion_Correcta()
    {
        // Aplicar torque en dirección opuesta
        rb.AddTorque(Vector3.down * 50f, ForceMode.Impulse);
        yield return new WaitForSeconds(1); // Esperar a que gire

        float anguloActual = hinge.angle;
        Assert.Less(anguloActual, 0, "La puerta no giró en la dirección correcta.");
    }

    [UnityTearDown]
    public IEnumerator Teardown()
    {
        Object.Destroy(puerta);
        yield return null;
    }
}