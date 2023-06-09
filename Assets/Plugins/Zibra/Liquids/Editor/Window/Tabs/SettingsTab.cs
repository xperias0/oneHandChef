﻿#if UNITY_2019_4_OR_NEWER
using com.zibra.liquid.Plugins.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace com.zibra.liquid
{
    internal class SettingsTab : BaseTab
    {
        readonly VisualElement m_StackVisualizersRoot;

        public SettingsTab() : base($"{ZibraAIPackage.WindowTabsPath}/SettingsTab")
        {
        }

        void ModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                m_StackVisualizersRoot.Clear();
            }
        }
    }
}
#endif