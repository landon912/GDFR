using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(TypeRestrictionAttribute))]
public class TypeRestrictionPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //validate inputs - could draw a 'fail' message if not validated correctly
        if (property.propertyType != SerializedPropertyType.ObjectReference) return;

        var attrib = this.attribute as TypeRestrictionAttribute;
        if (attrib == null) return;

        //do draw
        EditorGUI.BeginChangeCheck();
        UnityEngine.Object obj = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(UnityEngine.Object), attrib.allowSceneObjects);
        if (EditorGUI.EndChangeCheck())
        {
            if (obj != null)
            {
                var tp = obj.GetType();
                if (!attrib.type.IsAssignableFrom(tp))
                {
                    if (obj is GameObject)
                    {
                        obj = (obj as GameObject).GetComponent(attrib.type);
                    }
                    else if (obj is Component)
                    {
                        obj = (obj as Component).gameObject.GetComponent(attrib.type);
                    }
                    else
                    {
                        obj = null;
                    }
                }
            }
            property.objectReferenceValue = obj;
        }
    }
}