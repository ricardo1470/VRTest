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
    
    [Test]
    public void NetworkManager_Instancia_NoEsNula()
    {
        networkManagerObject = new GameObject("NetworkManager");
        networkManager = networkManagerObject.AddComponent<NetworkManager>();
        
        Assert.IsNotNull(networkManager, "El NetworkManager no se ha creado correctamente.");
    }

    [Test]
    public void NetworkManager_Configuracion_ValoresPorDefecto()
    {
        networkManagerObject = new GameObject("NetworkManager");
        networkManager = networkManagerObject.AddComponent<NetworkManager>();
        
        Assert.IsNotNull(networkManager, "El NetworkManager no se ha creado correctamente.");
        Assert.AreEqual(0, networkManager.ConnectedClients.Count, "El número de clientes conectados no es cero.");
        Assert.IsFalse(networkManager.IsServer, "El NetworkManager no debería ser un servidor por defecto.");
        Assert.IsFalse(networkManager.IsClient, "El NetworkManager no debería ser un cliente por defecto.");
        Assert.IsFalse(networkManager.IsHost, "El NetworkManager no debería ser un host por defecto.");
    }
}
