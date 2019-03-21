using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AssetObject))]
public class AssetObjectEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string name = property.displayName;
        SerializedProperty asset = property.FindPropertyRelative("asset");
        SerializedProperty path = property.FindPropertyRelative("path");
        SerializedProperty bundle = property.FindPropertyRelative("bundle");

        position.height = 16f;
        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, new GUIContent(name), true);

        if (property.isExpanded)
        {
            position.y += 18f;
            EditorGUI.indentLevel += 1;
            
            EditorGUI.BeginProperty(position, label, path);
            {
                EditorGUI.BeginChangeCheck();
                CheckObject(asset, path, bundle);
                Object obj = EditorGUI.ObjectField(position, new GUIContent("Asset"), asset.objectReferenceValue, typeof(Object), false);
                if (EditorGUI.EndChangeCheck()) ChangeObject(obj, asset, path, bundle);
            }
            EditorGUI.EndProperty();

            position.y += 18f;
            EditorGUI.BeginProperty(position, label, path);
            {
                EditorGUI.BeginChangeCheck();
                string newPath = EditorGUI.TextField(position, new GUIContent("Path"), path.stringValue);
                if (EditorGUI.EndChangeCheck()) path.stringValue = newPath;
            }
            EditorGUI.EndProperty();

            position.y += 18f;
            EditorGUI.BeginProperty(position, label, bundle);
            {
                EditorGUI.BeginChangeCheck();
                string newBundle = EditorGUI.TextField(position, new GUIContent("Bundle"), bundle.stringValue);
                if (EditorGUI.EndChangeCheck()) bundle.stringValue = newBundle;
            }
            EditorGUI.EndProperty();
            EditorGUI.indentLevel -= 1;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float result = 16f;
        if (property.isExpanded) result += 3f * 18f;
        return result;
    }

    private void CheckObject(SerializedProperty asset, SerializedProperty path, SerializedProperty bundle)
    {
        if (string.IsNullOrWhiteSpace(path.stringValue) || string.IsNullOrWhiteSpace(bundle.stringValue))
        {
            asset = null;
            return;
        }

        if (!asset.objectReferenceValue)
        {
            if (AssetImporter.GetAtPath(path.stringValue)?.assetBundleName == bundle.stringValue)
            {
                asset.objectReferenceValue = AssetDatabase.LoadAssetAtPath<Object>(path.stringValue);
            }
        }
    }

    private void ChangeObject(Object temp, SerializedProperty asset, SerializedProperty path, SerializedProperty bundle)
    {
        if (temp != asset.objectReferenceValue)
        {
            if (temp != null)
            {
                string p = AssetDatabase.GetAssetPath(temp);
                Debug.Log(p);
                string b = AssetImporter.GetAtPath(p).assetBundleName;
                Debug.Log(b);

                if (!string.IsNullOrWhiteSpace(b))
                {
                    asset.objectReferenceValue = AssetDatabase.LoadAssetAtPath<Object>(p);
                    path.stringValue = p;
                    bundle.stringValue = b;
                    return;
                }

                else Debug.LogError($"Asset at {p} is not in an AssetBundle.");
            }

            asset = null;
            path.stringValue = "";
            bundle.stringValue = "";
        }
    }
}