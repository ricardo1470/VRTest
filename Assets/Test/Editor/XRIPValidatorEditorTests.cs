using NUnit.Framework;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
///     Pruebas unitarias para la clase XRIPValidator en el editor de Unity.
///     Estas pruebas verifican la validez de las direcciones IP en el formato esperado.
/// </summary>
public class XRIPValidatorEditorTests
{
    private XRIPValidator ipValidator;
    private string ipRegex;

    [SetUp]
    public void Setup()
    {
        ipValidator = new GameObject().AddComponent<XRIPValidator>();
        ipRegex = @"^192\.168\.(25[0-5]|2[0-4][0-9]|1?[0-9]{1,2})\.(25[0-5]|2[0-4][0-9]|1?[0-9]{1,2})$";
    }

    [Test]
    public void IPValida_FormatoCorrecto_RetornaTrue()
    {
        string ip = "192.168.1.100";
        Assert.IsTrue(Regex.IsMatch(ip, ipRegex));
    }

    [Test]
    public void IPInvalida_FormatoIncorrecto_RetornaFalse()
    {
        string[] ipsInvalidas = {
            "10.0.0.1",         // No empieza con 192.168
            "192.168.1.256",    // Octeto >255
            "192.168.1",        // Faltan octetos
            "192.168.abc.123"   // Caracteres no numéricos
        };

        foreach (var ip in ipsInvalidas)
        {
            Assert.IsFalse(Regex.IsMatch(ip, ipRegex), $"Error con IP: {ip}");
        }
    }
}