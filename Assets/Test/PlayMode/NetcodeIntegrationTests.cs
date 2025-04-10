using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetcodeIntegrationTests
{
    private GameObject networkManagerObject;
    private NetworkManager networkManager;
    //private XRIPValidator ipValidator;
    
    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Crear un GameObject con NetworkManager
        networkManagerObject = new GameObject("NetworkManager");
        networkManager = networkManagerObject.AddComponent<NetworkManager>();
        
        // Configurar NetworkManager con Unity Transport
        var transport = networkManagerObject.AddComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
        networkManager.NetworkConfig = new Unity.Netcode.NetworkConfig
        {
            NetworkTransport = transport
        };
        
        yield return null;
    }
    
    [UnityTearDown]
    public IEnumerator TearDown()
    {
        // Limpiar después de cada prueba
        if (networkManager != null && networkManager.IsListening)
        {
            networkManager.Shutdown();
        }
        
        Object.Destroy(networkManagerObject);
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator VerificarInicializacionNetcode()
    {
        // Verificar que NetworkManager se inicializó correctamente
        Assert.NotNull(networkManager, "NetworkManager no debería ser nulo");
        Assert.NotNull(networkManager.NetworkConfig, "NetworkConfig no debería ser nulo");
        Assert.NotNull(networkManager.NetworkConfig.NetworkTransport, "NetworkTransport no debería ser nulo");
        
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator VerificarInicioComoHost()
    {
        // Intentar iniciar como host
        networkManager.StartHost();
        
        // Esperar un frame para que se procese
        yield return null;
        
        // Verificar que se inició correctamente como host
        Assert.IsTrue(networkManager.IsHost, "NetworkManager debería estar en modo Host");
        Assert.IsTrue(networkManager.IsListening, "NetworkManager debería estar escuchando");
        
        // Limpiar
        networkManager.Shutdown();
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator VerificarConfiguracionTransporte()
    {
        // Verificar que el transporte es de tipo UnityTransport
        var transport = networkManager.NetworkConfig.NetworkTransport as Unity.Netcode.Transports.UTP.UnityTransport;
        Assert.NotNull(transport, "El transporte debería ser UnityTransport");
        
        // Configurar una IP de prueba
        transport.ConnectionData.Address = "192.168.1.1";
        transport.ConnectionData.Port = 7777;
        
        // Verificar que la configuración se aplicó correctamente
        Assert.AreEqual("192.168.1.1", transport.ConnectionData.Address, "La dirección IP no se configuró correctamente");
        Assert.AreEqual((ushort)7777, transport.ConnectionData.Port, "El puerto no se configuró correctamente");
        
        yield return null;
    }

}