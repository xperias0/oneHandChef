using UnityEditor;

namespace ProjectDawn.MeshSlicer.Editor
{
    [CustomEditor(typeof(MeshSlicerManager))]
    class MeshSlicerManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_Script");
            serializedObject.ApplyModifiedProperties();
        }
    }
}