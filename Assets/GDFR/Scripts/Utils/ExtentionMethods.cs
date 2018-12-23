using UnityEngine;

public static class ExtensionMethods
{
    public static void RemoveFromDontDestroyOnLoad(this GameObject go)
    {
        GameObject newGO = new GameObject();
        go.transform.parent = newGO.transform; // NO longer DontDestroyOnLoad();
    }
}