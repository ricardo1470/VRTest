using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR;
using System.Collections.Generic;

public class NetworkManagerUI : MonoBehaviour
{
    public GameObject panelSeleccion;
    public GameObject panelIP;
    public Button botonHost;
    public Button botonCliente;
    public Button botonAbrirIP;
    public Button botonRegresar;
    public TextMeshProUGUI mensajeEstado;
    public TextMeshProUGUI estadoConexion;

    [Header("Component References")]
    public XRIPValidator ipValidator;
    public NetworkHostHandler hostHandler;
    public NetworkClientHandler clientHandler;

    [Header("VR Input Configuration")]
    public bool useXButton = true; // True para usar botón X, False para usar botón Y
    private bool lastButtonState = false;
    private List<InputDevice> leftHandDevices = new List<InputDevice>();
    private GameObject activeUIPanel = null;

    private void Start()
    {
        Debug.Log("Script NetworkManagerUI iniciado.");
        mensajeEstado.text = "Bienvenido. Selecciona una opción.";

        // Inicialmente mostramos solo el panel principal
        panelSeleccion.SetActive(true);
        panelIP.SetActive(false);
        activeUIPanel = panelSeleccion;

        estadoConexion.text = "Estado: Desconectado";

        // Configurar botones del panel principal
        botonHost.onClick.AddListener(() =>
        {
            if (hostHandler != null)
                hostHandler.IniciarComoHost();
        });

        botonCliente.onClick.AddListener(() =>
        {
            if (clientHandler != null)
                clientHandler.IniciarComoCliente();
        });

        botonAbrirIP.onClick.AddListener(AbrirPanelIP);
        botonRegresar.onClick.AddListener(RegresarAlPanelSeleccion);

        // Inicializar dispositivos XR
        InitializeXRDevices();
    }

    private void InitializeXRDevices()
    {
        // Buscar todos los dispositivos de la mano izquierda
        var desiredCharacteristics = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left;
        InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, leftHandDevices);

        if (leftHandDevices.Count > 0)
        {
            Debug.Log($"🎮 Controlador izquierdo detectado: {leftHandDevices[0].name}");
        }
        else
        {
            Debug.LogWarning("⚠️ No se detectó controlador izquierdo. Se intentará detectar en Update.");
        }
    }

    private void Update()
    {
        // Verificar botones en controlador izquierdo
        CheckVRButtonInput();

        // Mantener compatibilidad con teclado
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleUIPanel();
        }
    }

    private void CheckVRButtonInput()
    {
        // Si no hay dispositivos, intentar encontrarlos
        if (leftHandDevices.Count == 0)
        {
            var desiredCharacteristics = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left;
            InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, leftHandDevices);
            return;
        }

        // Referencia al controlador izquierdo
        var leftHandDevice = leftHandDevices[0];

        // Verificar si el controlador está activo/disponible
        if (!leftHandDevice.isValid)
        {
            leftHandDevices.Clear(); // Limpiar la lista para buscar de nuevo
            return;
        }

        bool isButtonPressed = false;
        InputFeatureUsage<bool> buttonToCheck;

        // Determinar qué botón usar basado en la configuración
        if (useXButton)
        {
            buttonToCheck = CommonUsages.primaryButton; // Botón X en Oculus Quest
        }
        else
        {
            buttonToCheck = CommonUsages.secondaryButton; // Botón Y en Oculus Quest
        }

        // Verificar el estado del botón
        if (leftHandDevice.TryGetFeatureValue(buttonToCheck, out isButtonPressed))
        {
            // Detectar cuando se presiona el botón (evitando repeticiones)
            if (isButtonPressed && !lastButtonState)
            {
                ToggleUIPanel();
            }

            // Actualizar el estado anterior
            lastButtonState = isButtonPressed;
        }
    }

    private void ToggleUIPanel()
    {
        if (activeUIPanel == null)
        {
            // Si no hay ningún panel activo, mostrar panel de selección
            panelSeleccion.SetActive(true);
            panelIP.SetActive(false);
            activeUIPanel = panelSeleccion;
            Debug.Log("📌 UI mostrada: Panel de selección");
        }
        else
        {
            // Si ya hay un panel activo, ocultarlo
            panelSeleccion.SetActive(false);
            panelIP.SetActive(false);
            activeUIPanel = null;
            Debug.Log("❌ UI ocultada");
        }
    }

    public void AbrirPanelIP()
    {
        panelIP.SetActive(true);
        panelSeleccion.SetActive(false);
        activeUIPanel = panelIP;
        Debug.Log("🌐 Panel de IP abierto.");

        // Cargar la última IP utilizada si existe
        if (ipValidator != null && ipValidator.inputIP != null)
        {
            string ultimaIP = PlayerPrefs.GetString("LastUsedIP", "192.168.1.1");
            ipValidator.inputIP.text = ultimaIP;
        }

        // Limpiar cualquier mensaje anterior
        if (ipValidator != null)
        {
            ipValidator.LimpiarMensajeEstado();
        }
    }

    public void RegresarAlPanelSeleccion()
    {
        panelIP.SetActive(false);
        panelSeleccion.SetActive(true);
        activeUIPanel = panelSeleccion;

        Debug.Log("🔙 Regresando al menú principal.");
    }

    // Métodos para actualizar UI desde los handlers
    public void ActualizarMensajeEstado(string mensaje)
    {
        if (mensajeEstado != null)
            mensajeEstado.text = mensaje;
    }

    public void ActualizarEstadoConexion(string estado)
    {
        if (estadoConexion != null)
            estadoConexion.text = estado;
    }

    public void CerrarPaneles()
    {
        panelSeleccion.SetActive(false);
        panelIP.SetActive(false);
        activeUIPanel = null;
    }
}
