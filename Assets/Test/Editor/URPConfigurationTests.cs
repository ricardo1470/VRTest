using NUnit.Framework;
using UnityEditor;
using UnityEditor.Rendering;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;
using UnityEngine.TestTools;

public class URPConfigurationTests
{

    [Test]
    public static void URP_Package_IsInstalled()
    {
        // Crear una solicitud de lista de paquetes
        var listRequest = UnityEditor.PackageManager.Client.List(true);
        
        // Esperar a que la solicitud se complete (esto es un enfoque simple para pruebas)
        while (!listRequest.IsCompleted)
        {
            System.Threading.Thread.Sleep(100);
        }

        // Verificar si hubo un error
        Assert.IsFalse(listRequest.Status == UnityEditor.PackageManager.StatusCode.Failure, 
            $"Error al obtener la lista de paquetes: {listRequest.Error?.message}");
        
        // Ahora es seguro acceder a Result
        bool isURPInstalled = listRequest.Result.Any(package => package.name == "com.unity.render-pipelines.universal");
        Assert.IsTrue(isURPInstalled, "URP no está instalado. Instala el paquete desde el Package Manager.");
    }

    [Test]
    public void URP_IsActiveInGraphicsSettings()
    {
        var urpAsset = GraphicsSettings.defaultRenderPipeline;

        Assert.IsNotNull(urpAsset, "No hay un Render Pipeline Asset asignado en Graphics Settings.");
        Assert.IsTrue(urpAsset.GetType().Name.Contains("UniversalRenderPipelineAsset"),
            "El Render Pipeline activo no es URP.");
    }

    [Test]
    public void TestScene_HasURPComponents()
    {
        var testScene = EditorBuildSettings.scenes[0].path;
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene(testScene);

        var mainCamera = Camera.main;
        Assert.IsNotNull(mainCamera, "No hay una cámara principal en la escena.");

        var cameraData = mainCamera.GetComponent<UniversalAdditionalCameraData>();
        Assert.IsNotNull(cameraData, "La cámara no tiene UniversalAdditionalCameraData (requerido para URP).");
    }

    [Test]
    public void URP_QualitySettings()
    {
        var qualitySettings = QualitySettings.GetQualityLevel();
        Assert.IsTrue(qualitySettings >= 0, "No hay configuraciones de calidad definidas.");

        var urpAsset = GraphicsSettings.defaultRenderPipeline as UniversalRenderPipelineAsset;
        Assert.IsNotNull(urpAsset, "URP no está configurado.");

    }
}
