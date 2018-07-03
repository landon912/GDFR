using UnityEngine;

public class TypeRestrictionAttribute : PropertyAttribute
{
    public System.Type type;
    public bool allowSceneObjects = true;

    public TypeRestrictionAttribute(System.Type tp)
    {
        if (tp == null) throw new System.ArgumentNullException(nameof(tp));
        this.type = tp;
    }
}