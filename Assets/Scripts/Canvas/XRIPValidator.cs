using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.Events;

public class XRIPValidator : MonoBehaviour
{
    [Header("Referencias UI")]
    public TMP_InputField inputIP;
    public Button botonTestearIP;
    public Button botonRegresar;
    public Button botonBorrar;
    public TextMeshProUGUI mensajeEstado;
    public GameObject[] botonesNumericos; // Botones del 0-9 y el punto "."

    [Header("Configuración")]
    [SerializeField] private string ipRegex = @"^192\.168\.\d{1,3}\.\d{1,3}$";
    [SerializeField] private Color colorExito = Color.green;
    [SerializeField] private Color colorError = Color.red;
    [SerializeField] private Color colorInfo = Color.white;

    [Header("Eventos")]
    public UnityEvent onIPValidada;
    public UnityEvent onRegresarSeleccionado;

    // Variable para almacenar la IP actual validada
    private string ipValidada = "";
    public string IPValidada => ipValidada;

    private void Awake()
    {
        ValidarComponentes();
        ConfigurarBotones();
    }

    private void Start()
    {
        // Inicializa la UI
        LimpiarCampoIP();

        // Carga la última IP usada si existe
        if (PlayerPrefs.HasKey("LastUsedIP"))
        {
            inputIP.text = PlayerPrefs.GetString("LastUsedIP");
            MostrarInfo("Última IP utilizada cargada");
        }
    }

    private void ValidarComponentes()
    {
        if (inputIP == null)
        {
            Debug.LogError("Error: Asigna el campo TMP_InputField para la IP.");
        }

        if (botonTestearIP == null)
        {
            Debug.LogError("Error: Asigna el botón para testear la IP.");
        }

        if (botonRegresar == null)
        {
            Debug.LogError("Error: Asigna el botón para regresar.");
        }

        if (botonBorrar == null)
        {
            Debug.LogError("Error: Asigna el botón para borrar.");
        }

        if (mensajeEstado == null)
        {
            Debug.LogError("Error: Asigna el TextMeshProUGUI para mostrar mensajes de estado.");
        }
    }

    private void ConfigurarBotones()
    {
        // Limpiar listeners previos para evitar duplicados
        if (botonTestearIP != null)
        {
            botonTestearIP.onClick.RemoveAllListeners();
            botonTestearIP.onClick.AddListener(ValidarIP);
        }

        if (botonRegresar != null)
        {
            botonRegresar.onClick.RemoveAllListeners();
            botonRegresar.onClick.AddListener(() =>
            {
                if (onRegresarSeleccionado != null)
                    onRegresarSeleccionado.Invoke();
            });
        }

        if (botonBorrar != null)
        {
            botonBorrar.onClick.RemoveAllListeners();
            botonBorrar.onClick.AddListener(BorrarUltimoCaracter);
        }

        // Configurar botones numéricos y punto
        ConfigurarBotonesNumericos();
    }

    private void ConfigurarBotonesNumericos()
    {
        // Verificar que tenemos botones para configurar
        if (botonesNumericos == null || botonesNumericos.Length == 0)
        {
            Debug.LogError("No se han asignado botones numéricos en el Inspector");
            return;
        }

        // Configurar botones del 0 al 9 y el punto
        foreach (GameObject botonObj in botonesNumericos)
        {
            if (botonObj == null)
            {
                Debug.LogWarning("Uno de los elementos en botonesNumericos es null");
                continue;
            }

            Button boton = botonObj.GetComponent<Button>();
            TextMeshProUGUI textoBoton = botonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (boton != null && textoBoton != null)
            {
                // Importante: creamos una variable local para capturar el valor correcto
                string valorActual = textoBoton.text;

                // Limpiamos listeners previos
                boton.onClick.RemoveAllListeners();

                // Añadimos el nuevo listener
                boton.onClick.AddListener(() =>
                {
                    Debug.Log($"Botón presionado: {valorActual}");
                    AgregarCaracter(valorActual);
                });
            }
            else
            {
                Debug.LogWarning($"El botón {botonObj.name} no tiene Button o TextMeshProUGUI configurado.");
            }
        }
    }

    public void AgregarCaracter(string caracter)
    {
        if (inputIP != null)
        {
            // Asegúrate de que el inputField tenga el foco
            inputIP.Select();

            // Añadir el carácter y forzar una actualización
            inputIP.text += caracter;

            // Actualiza la posición del cursor al final
            inputIP.caretPosition = inputIP.text.Length;

            // Forzar actualización visual
            inputIP.ForceLabelUpdate();

            Debug.Log($"Texto actual en InputField: {inputIP.text}");
        }
        else
        {
            Debug.LogError("El InputField no está asignado en XRIPValidator");
        }
    }

    public void BorrarUltimoCaracter()
    {
        if (inputIP != null && inputIP.text.Length > 0)
        {
            // Eliminar el último carácter
            inputIP.text = inputIP.text.Substring(0, inputIP.text.Length - 1);

            // Actualiza la posición del cursor al final
            inputIP.caretPosition = inputIP.text.Length;

            // Forzar actualización visual
            inputIP.ForceLabelUpdate();

            // Selecciona el campo para mantener el foco
            inputIP.Select();

            Debug.Log($"Carácter borrado. Texto actual: {inputIP.text}");
        }
    }

    public void ValidarIP()
    {
        if (inputIP == null || mensajeEstado == null)
        {
            Debug.LogError("Referencias faltantes en XRIPValidator");
            return;
        }

        string ipIngresada = inputIP.text.Trim();
        ipValidada = ""; // Reiniciar la IP validada

        Debug.Log($"Validando IP: {ipIngresada}");

        // Asegurarse de que el mensaje de estado sea visible
        mensajeEstado.gameObject.SetActive(true);

        if (Regex.IsMatch(ipIngresada, ipRegex))
        {
            bool esValida = true;
            string[] octetos = ipIngresada.Split('.');

            if (octetos.Length == 4)
            {
                for (int i = 2; i < 4; i++)
                {
                    if (int.TryParse(octetos[i], out int valor))
                    {
                        if (valor < 0 || valor > 255)
                        {
                            esValida = false;
                            Debug.Log($"Octeto {i + 1} fuera de rango: {valor}");
                            break;
                        }
                    }
                    else
                    {
                        esValida = false;
                        Debug.Log($"Octeto {i + 1} no es un número válido: {octetos[i]}");
                        break;
                    }
                }
            }

            if (esValida)
            {
                ipValidada = ipIngresada;
                mensajeEstado.text = $"✅ IP válida: {ipIngresada}";
                mensajeEstado.color = colorExito;

                PlayerPrefs.SetString("LastUsedIP", ipValidada);
                PlayerPrefs.Save();

                onIPValidada?.Invoke();
            }
            else
            {
                mensajeEstado.text = "❌ IP fuera de rango (0-255).";
                mensajeEstado.color = colorError;
            }
        }
        else
        {
            MostrarError("IP inválida. Formato esperado: 192.168.x.x");
        }
    }

    public void MostrarError(string mensaje)
    {
        if (mensajeEstado != null)
        {
            mensajeEstado.color = colorError;
            mensajeEstado.text = "❌ " + mensaje;
            Debug.Log("❌ " + mensaje);
        }
    }

    public void MostrarInfo(string mensaje)
    {
        if (mensajeEstado != null)
        {
            mensajeEstado.color = colorInfo;
            mensajeEstado.text = "ℹ️ " + mensaje;
            Debug.Log("ℹ️ " + mensaje);
        }
    }

    public void LimpiarCampoIP()
    {
        if (inputIP != null)
        {
            inputIP.text = "";
        }

        LimpiarMensajeEstado();
    }

    public void LimpiarMensajeEstado()
    {
        if (mensajeEstado != null)
        {
            mensajeEstado.text = "";
        }
    }

    public void ActualizarEstadoConexion(bool conectado, string mensaje = "")
    {
        if (mensajeEstado != null)
        {
            mensajeEstado.text = mensaje;
            mensajeEstado.color = conectado ? colorExito : colorError;
            mensajeEstado.gameObject.SetActive(true);
        }
    }
}
