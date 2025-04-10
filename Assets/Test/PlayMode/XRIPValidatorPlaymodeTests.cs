using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using TMPro;
using System.Collections;

public class XRIPValidatorPlaymodeTests
{
    private XRIPValidator ipValidator;
    private TMP_InputField inputField;
    private TextMeshProUGUI mensajeEstado;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Crear objetos temporales
        ipValidator = new GameObject().AddComponent<XRIPValidator>();
        inputField = new GameObject().AddComponent<TMP_InputField>();
        mensajeEstado = new GameObject().AddComponent<TextMeshProUGUI>();

        // Asignar referencias
        ipValidator.inputIP = inputField;
        ipValidator.mensajeEstado = mensajeEstado;

        // Simular Start()
        yield return null;
    }
}
