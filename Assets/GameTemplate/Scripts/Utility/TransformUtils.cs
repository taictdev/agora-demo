using Cysharp.Threading.Tasks;
using UnityEngine;

public static class TransformUtils
{
    public static async UniTask DestroyAllChildren(Transform parent)
    {
        for (var i = 0; i < parent.childCount; i++)
            Object.Destroy(parent.GetChild(i)
                .gameObject);

        await UniTask.WaitUntil(() => parent.childCount == 0);
    }

    public static void HideAllChildren(Transform parent)
    {
        for (var i = 0; i < parent.childCount; i++)
            parent.GetChild(i)
                .gameObject.SetActive(false);
    }

    public static T GetFirstComponentInUpper<T>(Transform t) where T : MonoBehaviour
    {
        var parent = t.parent;
        while (parent != null)
        {
            var com = parent.GetComponent<T>();
            if (com != null)
            {
                return com;
            }

            parent = parent.parent;
        }

        return null;
    }

    public static Transform RecursiveFindChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }
            else
            {
                Transform found = RecursiveFindChild(child, childName);
                if (found != null)
                {
                    return found;
                }
            }
        }

        return null;
    }
}