using UnityEngine;
using Unity.Netcode;

public class NetworkClientHandler : MonoBehaviour
{
    public NetworkManagerUI uiManager;
    public XRIPValidator ipValidator;

    public void IniciarComoCliente()
    {
        // Validar que tengamos una IP antes de intentar conectar
        if (ipValidator == null || string.IsNullOrEmpty(ipValidator.IPValidada))
        {
            MostrarMensaje("⚠️ Primero debes ingresar y validar una IP.", true);

            // Abrir panel de IP si no está ya abierto
            if (uiManager != null)
            {
                uiManager.AbrirPanelIP();
            }
            return;
        }

        if (NetworkManager.Singleton == null || NetworkManager.Singleton.NetworkConfig.NetworkTransport == null)
        {
            MostrarMensaje("⚠️ Error en la configuración de la red.", true);
            Debug.LogError("NetworkManager o NetworkTransport no están configurados.");
            return;
        }

        string ipAddress = ipValidator.IPValidada;

        // Configurar la IP en el transporte de red
        ConfigurarIPDeConexion(ipAddress);

        // Mostrar mensaje de conexión
        MostrarMensaje($"🟡 Conectando a {ipAddress}...", true);
        Debug.Log($"🔄 Intentando conectarse a {ipAddress} como Cliente...");

        // Iniciar cliente
        NetworkManager.Singleton.StartClient();
        Invoke("VerificarConexionCliente", 2f);
    }

    private void ConfigurarIPDeConexion(string ipAddress)
    {
        // Verifica si estás usando Unity Transport
        var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport;

        // Para Unity Transport Package
        if (transport is Unity.Netcode.Transports.UTP.UnityTransport utpTransport)
        {
            utpTransport.ConnectionData.Address = ipAddress;
            Debug.Log($"✅ IP configurada en el transporte: {ipAddress}");
        }
        else
        {
            Debug.LogWarning("⚠️ No se pudo configurar la IP. Transporte de red no reconocido.");
        }
    }

    private void VerificarConexionCliente()
    {
        if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
        {
            MostrarMensaje("✅ Conectado como Cliente.", true);
            ActualizarEstadoConexion("🟢 Cliente");
            Debug.Log("✅ Conectado como Cliente.");

            if (uiManager != null)
            {
                Invoke("CerrarPaneles", 2f);
            }
        }
        else
        {
            MostrarMensaje("❌ No se pudo conectar como Cliente.", true);
            ActualizarEstadoConexion("❌ Desconectado");
            Debug.LogError("No se pudo conectar como Cliente.");
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
            if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.Shutdown();
                MostrarMensaje("Desconectado del servidor.", true);
                ActualizarEstadoConexion("Estado: Desconectado");
                Debug.Log("🔌 Desconectado de la sesión.");
            }
        }
    }
}
