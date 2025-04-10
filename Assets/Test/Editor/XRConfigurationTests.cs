using NUnit.Framework;
using UnityEditor;
using UnityEditor.XR.Management;
using System.Linq;

public class XRConfigurationTests
{
    [Test]
    public void XR_Interaction_Toolkit_IsInstalled()
    {
        var request = UnityEditor.PackageManager.Client.List();
        while (!request.IsCompleted) { }
        bool isToolkitInstalled = request.Result.Any(package => package.name == "com.unity.xr.interaction.toolkit");
        Assert.IsTrue(isToolkitInstalled, "XR Interaction Toolkit no está instalado.");
    }

    [Test]
    public void XR_Plugin_Management_IsInstalled()
    {
        var request = UnityEditor.PackageManager.Client.List();
        while (!request.IsCompleted) { }
        bool isPluginInstalled = request.Result.Any(package => package.name == "com.unity.xr.management");
        Assert.IsTrue(isPluginInstalled, "XR Plugin Management no está instalado.");
    }

    [Test]
    public void XR_Settings_AreConfigured()
    {
        var settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Standalone);
        Assert.IsNotNull(settings, "XR General Settings no están configurados.");

        // Verifica que los loaders estén activos
        Assert.IsTrue(settings.Manager.activeLoaders.Count > 0, "No hay loaders activos en XR Plugin Management.");
    }

    [Test]
    public void XR_Settings_AreNotEmpty()
    {
        var settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Standalone);
        Assert.IsNotNull(settings, "XR General Settings no están configurados.");

        // Verifica que los loaders no estén vacíos
        Assert.IsTrue(settings.Manager.activeLoaders.Count > 0, "No hay loaders activos en XR Plugin Management.");
    }

    [Test]
    public void XR_Settings_AreNotNull()
    {
        var settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Standalone);
        Assert.IsNotNull(settings, "XR General Settings no están configurados.");

        // Verifica que los loaders no sean nulos
        Assert.IsNotNull(settings.Manager.activeLoaders, "Los loaders activos son nulos.");
    }

    [Test]
    public void XR_Settings_AreNotEmptyOrNull()
    {
        var settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Standalone);
        Assert.IsNotNull(settings, "XR General Settings no están configurados.");

        // Verifica que los loaders no estén vacíos o nulos
        Assert.IsTrue(settings.Manager.activeLoaders.Count > 0, "No hay loaders activos en XR Plugin Management.");
    }

    // openXR
    [Test]
    public void OpenXR_IsInstalled()
    {
        var request = UnityEditor.PackageManager.Client.List();
        while (!request.IsCompleted) { }
        bool isOpenXRInstalled = request.Result.Any(package => package.name == "com.unity.xr.openxr");
        Assert.IsTrue(isOpenXRInstalled, "OpenXR no está instalado.");
    }

    [Test]
    public void OpenXR_Settings_AreConfigured()
    {
        var settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Standalone);
        Assert.IsNotNull(settings, "XR General Settings no están configurados.");

        // Verifica que los loaders estén activos
        Assert.IsTrue(settings.Manager.activeLoaders.Count > 0, "No hay loaders activos en XR Plugin Management.");
    }
}