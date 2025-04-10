using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class AddressablesManager : MonoBehaviour
{
    [Header("Configuración de UI")]
    [SerializeField] private Button loadButton;
    [SerializeField] private Button addButton;
    [SerializeField] private Button removeButton;
    [SerializeField] private TMP_Text fpsText;
    [SerializeField] private TMP_Text objectCountText;
    [SerializeField] private TMP_Dropdown loadAmountDropdown;
    [SerializeField] private Transform spawnParent;

    [Header("Configuración de Addressables")]
    [SerializeField] private string[] addressableKeys;
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(100f, 10f, 100f);

    [Header("Configuración de Rendimiento")]
    [SerializeField] private bool useGPUInstancing = true;
    [SerializeField] private bool enableBatching = true;
    [SerializeField] private int targetFrameRate = 60;

    // Variables de seguimiento
    private List<GameObject> instantiatedObjects = new List<GameObject>();
    private int loadMultiplier = 10; // Cantidad base a cargar (x10, x100)
    private float frameRateUpdateInterval = 0.5f; // Intervalo de actualización de FPS
    private float lastFrameRateUpdate;
    private int frameCount;
    private float timeElapsed;

    private void Awake()
    {
        // Configuración de rendimiento
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
    }

    private void Start()
    {
        // Inicializar UI
        SetupUI();

        // Configurar instancing en los materiales si está activado
        if (useGPUInstancing)
        {
            ConfigureGPUInstancing();
        }
    }

    private void Update()
    {
        // Actualizar contador de FPS
        UpdateFPSCounter();
    }

    private void SetupUI()
    {
        // Configurar botón de carga
        if (loadButton) loadButton.onClick.AddListener(LoadAssets);

        // Configurar botón de agregar objetos
        if (addButton) addButton.onClick.AddListener(() => AddObjects(loadMultiplier));

        // Configurar botón de eliminar objetos
        if (removeButton) removeButton.onClick.AddListener(() => RemoveObjects(loadMultiplier));

        // Configurar dropdown de cantidad
        if (loadAmountDropdown)
        {
            loadAmountDropdown.ClearOptions();
            loadAmountDropdown.AddOptions(new List<string> { "x10", "x100", "x1000" });
            loadAmountDropdown.onValueChanged.AddListener(OnLoadAmountChanged);
            OnLoadAmountChanged(0); // Valor por defecto (x10)
        }
        // Actualizar textos iniciales
        UpdateObjectCountText();
    }

    private void OnLoadAmountChanged(int index)
    {
        switch (index)
        {
            case 0: loadMultiplier = 10; break;
            case 1: loadMultiplier = 100; break;
            case 2: loadMultiplier = 1000; break;
            default: loadMultiplier = 10; break;
        }
    }

    public void LoadAssets()
    {
        // Desactivar botón durante la carga
        if (loadButton) loadButton.interactable = false;

        StartCoroutine(LoadAssetsCoroutine());
    }

    private IEnumerator LoadAssetsCoroutine()
    {
        Debug.Log($"Iniciando carga de assets Addressables...");

        foreach (string key in addressableKeys)
        {
            AsyncOperationHandle<GameObject> loadHandle = Addressables.LoadAssetAsync<GameObject>(key);
            yield return loadHandle;

            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject prefab = loadHandle.Result;
                Debug.Log($"Asset cargado exitosamente: {key}");

                // Instanciar el objeto inicial para verificar que todo funciona
                InstantiateAddressableObject(prefab);
            }
            else
            {
                Debug.LogError($"Error al cargar asset: {key}");
            }
        }

        // Reactivar botón
        if (loadButton) loadButton.interactable = true;
        UpdateObjectCountText();
    }

    public void AddObjects(int amount)
    {
        if (addressableKeys == null || addressableKeys.Length == 0)
        {
            Debug.LogWarning("No hay addressables configurados para cargar");
            return;
        }

        StartCoroutine(AddObjectsCoroutine(amount));
    }

    private IEnumerator AddObjectsCoroutine(int amount)
    {
        Debug.Log($"Añadiendo {amount} objetos...");

        // Limitar cantidad para evitar problemas de rendimiento
        int safeAmount = Mathf.Min(amount, 5000);
        int batchSize = 100; // Procesar en lotes para no bloquear el hilo principal

        List<AsyncOperationHandle<GameObject>> handles = new List<AsyncOperationHandle<GameObject>>();

        // Cargar todos los prefabs primero
        foreach (string key in addressableKeys)
        {
            AsyncOperationHandle<GameObject> loadHandle = Addressables.LoadAssetAsync<GameObject>(key);
            handles.Add(loadHandle);
        }

        // Esperar a que se carguen todos
        foreach (var handle in handles)
        {
            yield return handle;
        }

        // Instanciar los objetos en lotes
        for (int i = 0; i < safeAmount; i++)
        {
            if (i > 0 && i % batchSize == 0)
            {
                // Pausa cada cierto número de instancias para evitar congelar la UI
                yield return null;
            }

            // Seleccionar un prefab aleatorio de los cargados
            int randomIndex = Random.Range(0, handles.Count);
            if (handles[randomIndex].Status == AsyncOperationStatus.Succeeded)
            {
                InstantiateAddressableObject(handles[randomIndex].Result);
            }
        }

        UpdateObjectCountText();
    }

    public void RemoveObjects(int amount)
    {
        if (instantiatedObjects.Count == 0) return;

        int countToRemove = Mathf.Min(amount, instantiatedObjects.Count);

        for (int i = 0; i < countToRemove; i++)
        {
            int lastIndex = instantiatedObjects.Count - 1;
            GameObject obj = instantiatedObjects[lastIndex];
            instantiatedObjects.RemoveAt(lastIndex);

            if (obj != null)
            {
                Destroy(obj);
            }
        }

        UpdateObjectCountText();
    }

    private void InstantiateAddressableObject(GameObject prefab)
    {
        // Posición aleatoria dentro del área especificada
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnAreaSize.x/2, spawnAreaSize.x/2),
            Random.Range(-spawnAreaSize.y/2, spawnAreaSize.y/2),
            Random.Range(-spawnAreaSize.z/2, spawnAreaSize.z/2)
        );

        // Rotación aleatoria
        Quaternion randomRotation = Quaternion.Euler(
            Random.Range(0, 360),
            Random.Range(0, 360),
            Random.Range(0, 360)
        );

        // Instanciar el objeto
        GameObject instance = Instantiate(prefab, randomPosition, randomRotation);

        // Asignar parent si existe
        if (spawnParent != null)
        {
            instance.transform.SetParent(spawnParent, true);
        }

        // Configurar GPU Instancing si está habilitado
        if (useGPUInstancing)
        {
            ConfigureInstancedRenderers(instance);
        }

        // Añadir a la lista de seguimiento
        instantiatedObjects.Add(instance);
    }

    private void ConfigureGPUInstancing()
    {
        Debug.Log("Configurando GPU Instancing global...");

        // Asegurar que las opciones de batching estén activadas si corresponde
        if (enableBatching)
        {
            StaticBatchingUtility.Combine(gameObject);
        }
    }

    private void ConfigureInstancedRenderers(GameObject obj)
    {
        // Configurar GPU Instancing en todos los renderers del objeto
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;

            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] != null)
                {
                    materials[i].enableInstancing = true;
                }
            }

            renderer.materials = materials;
        }
    }

    private void UpdateFPSCounter()
    {
        // Incrementar contadores
        frameCount++;
        timeElapsed += Time.deltaTime;

        // Actualizar FPS cada cierto intervalo
        if (timeElapsed > lastFrameRateUpdate + frameRateUpdateInterval)
        {
            // Calcular FPS
            float fps = frameCount / timeElapsed;
            frameCount = 0;
            timeElapsed = 0;

            // Actualizar texto
            if (fpsText != null)
            {
                string fpsString = string.Format("FPS: {0:F1}", fps);
                fpsText.text = fpsString;

                // Cambiar color según rendimiento
                if (fps < 30)
                {
                    fpsText.color = Color.red;
                }
                else if (fps < 50)
                {
                    fpsText.color = Color.yellow;
                }
                else
                {
                    fpsText.color = Color.green;
                }
            }

            lastFrameRateUpdate = Time.time;
        }
    }

    private void UpdateObjectCountText()
    {
        if (objectCountText != null)
        {
            objectCountText.text = $"Objetos: {instantiatedObjects.Count}";
        }
    }

    private void OnDrawGizmos()
    {
        // Dibujar el área de spawn
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawCube(transform.position, spawnAreaSize);
    }
}
