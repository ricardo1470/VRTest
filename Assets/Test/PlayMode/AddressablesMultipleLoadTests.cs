using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.TestTools;

public class AddressablesMultipleLoadTests
{
    // Lista de keys o labels de los assets en Addressables
    private string[] assetKeys = new string[]
    {
        "Assets/Prefabs/Addressables/Capsule.prefab", // reemplaza con el nombre real en Addressables
        "Assets/Prefabs/Addressables/Cube.prefab",
        "Assets/Prefabs/Addressables/Cylinder.prefab",
        "Assets/Prefabs/Addressables/Sphere.prefab"
    };

    [UnityTest]
    public IEnumerator TodosLosAssetsSeCarganCorrectamente()
    {
        foreach (string key in assetKeys)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(key);
            yield return handle;

            Assert.AreEqual(AsyncOperationStatus.Succeeded, handle.Status, $"Fallo al cargar: {key}");
            Assert.IsNotNull(handle.Result, $"Asset nulo: {key}");

            GameObject instancia = Object.Instantiate(handle.Result);
            Assert.IsNotNull(instancia.GetComponent<Renderer>(), $"El asset {key} no tiene Renderer.");

            Object.DestroyImmediate(instancia);

            Addressables.Release(handle);
        }
    }
}
