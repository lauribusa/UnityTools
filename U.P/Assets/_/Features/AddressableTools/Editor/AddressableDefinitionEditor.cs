using _.Features.AddressableTools.Data;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace AddressableTools.Editor
{
    [CustomEditor(typeof(AddressableDefinition))]
    public class AddressableDefinitionEditor: UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var obj = serializedObject.targetObject as AddressableDefinition;
            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            if (obj == null) return root;
            var label = new Label($"Name: {obj.AddressableName}");
            root.Add(label);
            return root;
        }
    }
}