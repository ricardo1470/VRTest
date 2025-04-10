using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// Clase AssetLoader para cargar y liberar assets utilizando Addressables.
/// Esta clase permite cargar un asset de forma asíncrona y liberarlo cuando ya no es necesario.
/// </summary>
/// remarks>
/// Esta clase se encarga de gestionar la carga y liberación de assets utilizando el sistema Addressables de Unity.
/// Permite cargar un asset especificando su dirección y liberar los assets cargados al destruir el objeto.
/// </remarks>
/// version>1.0</version>
public class AssetLoader : MonoBehaviour
{
    [SerializeField]
    private string assetAddress1 = "asset1";

    [SerializeField]
    private List<GameObject> loadedObjects = new List<GameObject>();

    /// <summary>
    /// Método Awake se llama al inicio del script.
    /// Se asegura de que todos los componentes necesarios estén asignados en el Inspector.
    /// </summary>
    /// remarks>
    /// Este método se ejecuta antes de Start y se utiliza para inicializar variables o componentes.
    /// </remarks>
    /// <returns></returns>
    private void Awake()
    {
        if (string.IsNullOrEmpty(assetAddress1))
        {
            Debug.LogError("Por favor, asigna una dirección de asset en el Inspector.");
            return;
        }
    }

    /// <summary>
    /// Método Awake se llama al inicio del script.
    /// Se asegura de que todos los componentes necesarios estén asignados en el Inspector.
    /// </summary>
    /// remarks>
    /// Este método se ejecuta antes de Start y se utiliza para inicializar variables o componentes.
    /// </remarks>
    /// <returns></returns>
    private void Start()
    {
        LoadAssetAsync(assetAddress1);
    }

    /// <summary>
    /// Método para cargar un asset de forma asíncrona utilizando su dirección.
    /// Se utiliza Addressables para cargar el asset y se asigna un callback para manejar la carga.
    /// </summary>
    /// remarks>
    /// Este método se ejecuta al inicio del juego y carga el asset especificado en assetAddress1.
    /// </remarks>
    /// <param name="address">Dirección del asset a cargar.</param>
    /// <returns></returns>
    public void LoadAssetAsync(string address)
    {
        Addressables.InstantiateAsync(address).Completed += OnAssetLoaded;
    }

    /// <summary>
    /// Callback que se llama cuando el asset se ha cargado exitosamente.
    /// Se verifica el estado de la operación y se maneja el resultado.
    /// </summary>
    /// remarks>
    /// Este método se ejecuta al completar la carga del asset.
    /// Se utiliza para verificar si la carga fue exitosa y manejar el resultado.
    /// </remarks>
    /// <param name="obj">Operación asíncrona que contiene el resultado de la carga.</param>
    /// <returns></returns>
    private void OnAssetLoaded(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"Asset cargado exitosamente: {obj.Result.name}");
            loadedObjects.Add(obj.Result);
        }
        else
        {
            Debug.LogError($"Error al cargar asset: {obj.OperationException}");
        }
    }

    /// <summary>
    /// Método para liberar los assets cargados utilizando Addressables.
    /// Se itera sobre la lista de objetos cargados y se liberan uno por uno.
    /// </summary>
    /// remarks>
    /// Este método se ejecuta al destruir el objeto y libera todos los assets cargados.
    /// Se utiliza para liberar recursos y evitar fugas de memoria.
    /// </remarks>
    /// <returns></returns>
    public void ReleaseAssets()
    {
        foreach (var obj in loadedObjects)
        {
            Addressables.ReleaseInstance(obj);
        }
        loadedObjects.Clear();
        Debug.Log("Assets liberados");
    }

    /// <summary>
    /// Método OnDestroy se llama al destruir el objeto.
    /// Se utiliza para liberar los assets cargados y evitar fugas de memoria.
    /// </summary>
    /// remarks>
    /// Este método se ejecuta al destruir el objeto y libera todos los assets cargados.
    /// Se utiliza para liberar recursos y evitar fugas de memoria.
    /// </remarks>
    /// <returns></returns>
    private void OnDestroy()
    {
        ReleaseAssets();
    }
}
