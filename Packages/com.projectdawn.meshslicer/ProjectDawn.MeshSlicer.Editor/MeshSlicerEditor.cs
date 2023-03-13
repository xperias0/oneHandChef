using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ProjectDawn.MeshSlicer.Editor
{
    [CustomEditor(typeof(MeshSlicer))]
    class MeshSlicerEditor : UnityEditor.Editor
    {
        public static class Styles
        {
            public static readonly GUIContent Division = EditorGUIUtility.TrTextContent("Division");
            public static readonly GUIContent DivisionType = EditorGUIUtility.TrTextContent("Type", "Controls how mesh will be divided. Discrete will not slice triangles, where Linear will.");
            public static readonly GUIContent DivisionPrecision = EditorGUIUtility.TrTextContent("Precision", "To minimze floating point errors slices uses fixed precision.");

            public static readonly GUIContent Fill = EditorGUIUtility.TrTextContent("Fill");
            public static readonly GUIContent FillEnabled = EditorGUIUtility.TrTextContent("Enabled", "Is fill generated for each slice pieces.");
            public static readonly GUIContent FillConvex = EditorGUIUtility.TrTextContent("Convex", "Is fill surface convex shape.");
            public static readonly GUIContent FillSubMeshIndex = EditorGUIUtility.TrTextContent("SubMeshIndex", "Index of sub mesh that will be used for adding fill mesh.");
            public static readonly GUIContent FillMaterial = EditorGUIUtility.TrTextContent("Material", "Mesh slicer material used for fill.");

            public static readonly GUIContent Addative = EditorGUIUtility.TrTextContent("Addative");
            public static readonly GUIContent AdditiveMaxSliceCount = EditorGUIUtility.TrTextContent("MaxSliceCount", "Maximum number of slice jobs one MeshSlicer can contain.");
            public static readonly GUIContent AdditivePartialResults = EditorGUIUtility.TrTextContent("PartialResults", "If false pieces will be created only once all scheduled jobs for MeshSlicer completed. Otherwise it will generated new pieces as soon as any job completes.");
        }

        SerializedProperty m_Division;
        SerializedProperty m_DivisionType;
        SerializedProperty m_DivisionPrecision;

        SerializedProperty m_Fill;
        SerializedProperty m_FillEnabled;
        SerializedProperty m_FillConvex;
        
        SerializedProperty m_FillSubMeshIndex;
        SerializedProperty m_FillMaterial;

        SerializedProperty m_Additive;
        SerializedProperty m_AdditiveMaxSliceCount;
        SerializedProperty m_AdditivePartialResults;

        List<Material> m_Materials;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical();

            var slicer = target as MeshSlicer;

            if (EditorGUILayout.PropertyField(m_Division, Styles.Division, false))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_DivisionType, Styles.DivisionType);
                EditorGUILayout.PropertyField(m_DivisionPrecision, Styles.DivisionPrecision);
                EditorGUI.indentLevel--;
            }
            using (new EditorGUI.DisabledScope(m_DivisionType.enumValueIndex != (int)DivionType.Linear))
            {
                if (EditorGUILayout.PropertyField(m_Fill, Styles.Fill, false))
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(m_FillEnabled, Styles.FillEnabled);
                    using (new EditorGUI.DisabledScope(!m_FillEnabled.boolValue))
                    {
                        EditorGUILayout.PropertyField(m_FillConvex, Styles.FillConvex);
                        if (!m_FillConvex.boolValue)
                        {
                            EditorGUILayout.HelpBox("Only convex fill is currently supported!", MessageType.Error);
                        }
                        EditorGUILayout.PropertyField(m_FillSubMeshIndex, Styles.FillSubMeshIndex);

                        if (slicer.TryGetComponent(out Renderer renderer))
                        {
                            if (renderer is MeshRenderer meshRenderer)
                            {
                                meshRenderer.GetSharedMaterials(m_Materials);
                                if (m_FillSubMeshIndex.intValue == m_Materials.Count)
                                {
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.PropertyField(m_FillMaterial, Styles.FillMaterial);
                                    EditorGUI.indentLevel--;
                                }
                                else if (m_FillSubMeshIndex.intValue == m_Materials.Count - 1)
                                {
                                    EditorGUI.indentLevel++;
                                    using (new EditorGUI.DisabledScope(true))
                                        EditorGUILayout.ObjectField("Fill Material", m_Materials[m_Materials.Count - 1], typeof(Material), false);
                                    EditorGUI.indentLevel--;
                                }
                                else
                                {
                                    EditorGUILayout.HelpBox("Unsupported sub mesh index.", MessageType.Error);
                                }
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("Currently only MeshRenderer is supported.", MessageType.Warning);
                            }
                        }
                    }
                    EditorGUI.indentLevel--;
                }
            }

            if (EditorGUILayout.PropertyField(m_Additive, Styles.Addative, false))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_AdditiveMaxSliceCount, Styles.AdditiveMaxSliceCount);
                using (new EditorGUI.DisabledScope(m_AdditiveMaxSliceCount.intValue == 1))
                {
                    EditorGUILayout.PropertyField(m_AdditivePartialResults, Styles.AdditivePartialResults);
                }
                EditorGUI.indentLevel--;
            }

            if (slicer.TryGetComponent(out MeshFilter meshFilter))
            {
                if (!meshFilter.sharedMesh.isReadable)
                    EditorGUILayout.HelpBox($"Mesh '{meshFilter.sharedMesh.name}' must be readable (isReadable is false; Read/Write must be enabled in import settings).", MessageType.Error);
            }

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        void OnEnable()
        {
            m_Division = serializedObject.FindProperty("Division");
            m_DivisionType = m_Division.FindPropertyRelative("Type");
            m_DivisionPrecision = m_Division.FindPropertyRelative("Precision");

            m_Fill = serializedObject.FindProperty("Fill");
            m_FillEnabled = m_Fill.FindPropertyRelative("Enabled");
            m_FillConvex = m_Fill.FindPropertyRelative("Convex");
            m_FillSubMeshIndex = m_Fill.FindPropertyRelative("SubMeshIndex");
            m_FillMaterial = serializedObject.FindProperty("FillMaterial");

            m_Additive = serializedObject.FindProperty("Additive");
            m_AdditiveMaxSliceCount = m_Additive.FindPropertyRelative("MaxSliceCount");
            m_AdditivePartialResults = m_Additive.FindPropertyRelative("PartialResults");

            m_Materials = new List<Material>();
        }
    }
}