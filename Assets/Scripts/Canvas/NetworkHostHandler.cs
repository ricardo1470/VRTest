using UnityEngine;
using Unity.Netcode;

public class NetworkHostHandler : MonoBehaviour
{
    public NetworkManagerUI uiManager;
    public XRIPValidator ipValidator;

    public void IniciarComoHost()
    {
        if (NetworkManager.Singleton == null || NetworkManager.Singleton.NetworkConfig.NetworkTransport == null)
        {
            MostrarMensaje("⚠️ Error en la configuración de la red.", true);
            Debug.LogError("NetworkManager o NetworkTransport no están configurados.");
            return;
        }

        MostrarMensaje("🟡 Iniciando como Host...", false);
        Debug.Log("🚀 Iniciando como Host...");

        // Si hay un validador de IP y tiene una IP válida, usarla
        if (ipValidator != null && !string.IsNullOrEmpty(ipValidator.IPValidada))
        {
            ConfigurarIPDeConexion(ipValidator.IPValidada);
        }

        NetworkManager.Singleton.StartHost();

        if (NetworkManager.Singleton.IsHost)
        {
            MostrarMensaje("✅ Conectado como Host.", true);
            ActualizarEstadoConexion("🔷 Host");
            Debug.Log("✅ Conectado como Host.");

            if (uiManager != null)
            {
                Invoke("CerrarPaneles", 2f);
            }
        }
        else
        {
            MostrarMensaje("❌ Error al iniciar como Host.", true);
            Debug.LogError("Error al iniciar como Host.");
        }
    }

    private void ConfigurarIPDeConexion(string ipAddress)
    {
        var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport;

        if (transport is Unity.Netcode.Transports.UTP.UnityTransport utpTransport)
        {
            utpTransport.ConnectionData.Address = ipAddress;
            utpTransport.ConnectionData.Port = 7777; // Puerto por defecto o configurable

            Debug.Log($"✅ IP configurada: {ipAddress} en puerto: {utpTransport.ConnectionData.Port}");
        }
        else
        {
            Debug.LogWarning("⚠️ No se pudo configurar la IP. Transporte de red no reconocido.");
        }
    }

    public string ObtenerIPLocal()
    {
        string ipLocal = "";
        var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                ipLocal = ip.ToString();
                break;
            }
        }
        return ipLocal;
    }

    // Método para mostrar la IP local en UI
    public void MostrarIPEnUI()
    {
        string ipLocal = ObtenerIPLocal();
        if (uiManager != null)
        {
            uiManager.ActualizarMensajeEstado($"Tu IP local: {ipLocal}");
        }
    }

    private void MostrarMensaje(string mensaje, bool mostrarEnValidador)
    {
        if (uiManager != null)
        {
            uiManager.ActualizarMensajeEstado(mensaje);
        }

        if (mostrarEnValidador && ipValidator != null)
        {
            if (mensaje.StartsWith("✅"))
                ipValidator.ActualizarEstadoConexion(true, mensaje);
            else if (mensaje.StartsWith("❌"))
                ipValidator.ActualizarEstadoConexion(false, mensaje);
            else
                ipValidator.MostrarInfo(mensaje);
        }
    }

    private void OnEnable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        }
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"👤 Cliente conectado: {clientId}");
        MostrarMensaje($"Cliente conectado: {clientId}", false);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"👋 Cliente desconectado: {clientId}");
        MostrarMensaje($"Cliente desconectado: {clientId}", false);
    }

    private void OnServerStarted()
    {
        Debug.Log("🚀 Servidor iniciado correctamente");
        MostrarMensaje("Servidor iniciado correctamente", false);
    }

    private void ActualizarEstadoConexion(string estado)
    {
        if (uiManager != null)
        {
            uiManager.ActualizarEstadoConexion(estado);
        }
    }

    private void CerrarPaneles()
    {
        if (uiManager != null)
        {
            uiManager.CerrarPaneles();
        }
    }

    public void Desconectar()
    {
        if (NetworkManager.Singleton != null)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.Shutdown();
                MostrarMensaje("Desconectado del servidor.", true);
                ActualizarEstadoConexion("Estado: Desconectado");
                Debug.Log("🔌 Desconectado de la sesión.");
            }
        }
    }
}
