using UnityEditor;
using UnityEngine;
using com.zibra.liquid.Utilities;
using com.zibra.liquid.Editor.SDFObjects;
using com.zibra.liquid.Solver;

namespace com.zibra.liquid.Editor.Solver
{
    public static class ZibraLiquidHelpers
    {
        [MenuItem("Zibra AI/Zibra AI - Liquids/Copy diagnostic information to clipboard", false, 30)]
        public static void Copy()
        {
            string diagInfo = "";
            diagInfo += "////////////////////////////" + "\n";
            diagInfo += "Zibra Liquids Diagnostic Information" + "\n";
            diagInfo += "Plugin Version: " + ZibraLiquid.PluginVersion;
#if ZIBRA_LIQUID_PAID_VERSION
            diagInfo += " Paid";
#else
            diagInfo += " Free";
#endif
            diagInfo += "\n";
            diagInfo += "Unity Version: " + Application.unityVersion + "\n";
            diagInfo += "Render Pipeline: " + RenderPipelineDetector.GetRenderPipelineType() + "\n";
            diagInfo += "Render Pipelines Imported: SRP";
#if UNITY_PIPELINE_HDRP
            diagInfo += " HDRP";
#endif
#if UNITY_PIPELINE_URP
            diagInfo += " URP";
#endif
            diagInfo += "\n";
            diagInfo += "OS: " + SystemInfo.operatingSystem + "\n";
            diagInfo += "Target Platform: " + EditorUserBuildSettings.activeBuildTarget + "\n";
            diagInfo += "Graphic API: " + SystemInfo.graphicsDeviceType + "\n";
            diagInfo += "GPU: " + SystemInfo.graphicsDeviceName + "\n";
            diagInfo += "GPU Feature Level: " + SystemInfo.graphicsDeviceVersion + "\n";
#if ZIBRA_LIQUID_PAID_VERSION
            diagInfo += "Server status: " + ZibraServerAuthenticationManager.GetInstance().GetStatus() + "\n";
            diagInfo +=
                "Key: " + (ZibraServerAuthenticationManager.GetInstance().PluginLicenseKey == "" ? "Unset" : "Set") +
                "\n";
#endif

            if (RenderPipelineDetector.IsURPMissingRenderComponent())
            {
                diagInfo += "URP Liquid Rendering Component is missing!!!" + "\n";
            }
            diagInfo += "////////////////////////////" + "\n";
            GUIUtility.systemCopyBuffer = diagInfo;
        }
    }
}