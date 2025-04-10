using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
///     Clase InstancedObjectSpawner para cargar y generar objetos instanciados en Unity utilizando Addressables.
///     Esta clase permite cargar un prefab de forma asíncrona y generar múltiples instancias de él en posiciones aleatorias dentro de un radio especificado.
/// </summary>
/// <remarks>
///     Esta clase se encarga de gestionar la carga y generación de objetos instanciados utilizando el sistema Addressables de Unity.
///     Permite cargar un prefab especificando su dirección y generar múltiples instancias de él en posiciones aleatorias dentro de un radio especificado.
/// </remarks>
/// version>1.0</version>
public class InstancedObjectSpawner : MonoBehaviour
{
    [SerializeField]
    private string instancedPrefabAddress = "instancedObject";
    [SerializeField]
    private int objectCount = 100;
    [SerializeField]
    private float spawnRadius = 20f;

    private GameObject instancedPrefab;
    private List<GameObject> spawnedObjects = new List<GameObject>();

    /// <summary>
    ///     metodo start se llama al inicio del script.
    ///     Se asegura de que todos los componentes necesarios estén asignados en el Inspector.
    /// </summary>
    /// <remarks>
    ///     Este método se ejecuta una vez al inicio del juego.
    ///     Se utiliza para inicializar el prefab y asignar la función de carga asíncrona.
    /// </remarks>
    /// <returns></returns>
    private void Start()
    {
        LoadPrefabAsync();
    }

    /// <summary>
    ///     Método para cargar un prefab de forma asíncrona utilizando su dirección.
    ///     Se utiliza Addressables para cargar el prefab y se asigna un callback para manejar la carga.
    /// </summary>
    /// <remarks>
    ///     Este método se ejecuta al inicio del juego y carga el prefab especificado en instancedPrefabAddress.
    /// </remarks>
    /// <returns></returns>
    private void LoadPrefabAsync()
    {
        Addressables.LoadAssetAsync<GameObject>(instancedPrefabAddress).Completed += OnPrefabLoaded;
    }

    /// <summary>
    ///     Método que se llama cuando el prefab se ha cargado correctamente.
    ///     Se instancia el prefab en posiciones aleatorias dentro del radio especificado.
    /// </summary>
    /// <remarks>
    ///     Este método se ejecuta una vez que el prefab ha sido cargado correctamente.
    ///     Se generan múltiples instancias del prefab en posiciones aleatorias.
    /// </remarks>
    /// <param name="obj">El objeto AsyncOperationHandle que contiene el resultado de la carga.</param>
    /// <returns></returns>
    private void OnPrefabLoaded(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            instancedPrefab = obj.Result;
            SpawnObjects();
        }
        else
        {
            Debug.LogError($"Error al cargar el prefab: {obj.OperationException}");
        }
    }

    /// <summary>
    ///     Método para instanciar objetos en posiciones aleatorias dentro de un radio especificado.
    ///     Se generan múltiples instancias del prefab cargado.
    /// </summary>
    /// <remarks>
    ///     Este método se ejecuta una vez que el prefab ha sido cargado correctamente.
    ///     Se generan múltiples instancias del prefab en posiciones aleatorias.
    /// </remarks>
    /// <returns></returns>
    private void SpawnObjects()
    {
        if (instancedPrefab == null) return;

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPos = Random.insideUnitSphere * spawnRadius;
            randomPos.y = 0;

            Quaternion randomRot = Quaternion.Euler(0, Random.Range(0, 360), 0);

            GameObject instance = Instantiate(instancedPrefab, randomPos, randomRot);
            spawnedObjects.Add(instance);
        }

        Debug.Log($"Spawned {objectCount} instanced objects");
    }

    /// <summary>
    ///     Método OnDestroy se llama al destruir el objeto.
    ///     Se utiliza para liberar los assets cargados y evitar fugas de memoria.
    /// </summary>
    /// <remarks>
    ///     Este método se ejecuta cuando el objeto es destruido.
    ///    Se utiliza para liberar los assets cargados y evitar fugas de memoria.
    /// </remarks>
    /// <returns></returns>
    private void OnDestroy()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null) Destroy(obj);
        }

        Addressables.Release(instancedPrefab);
    }
}
