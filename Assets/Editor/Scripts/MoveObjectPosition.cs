using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class MoveObjectPosition
{
    [MenuItem("Tools/MoveObject/MoveToCircle")]
    public static void MoveToCircle()
    {
        var selection = Selection.gameObjects;
        if (selection.Length == 0) return;

        float angle = 180.0f / (float)selection.Length + 1.5f;

        for (int i = 0; i < selection.Length; i++)
        {
            var position = Vector3.left;
            var x = position.x * Mathf.Cos(Mathf.Deg2Rad * angle * i) - position.z * Mathf.Sin(Mathf.Deg2Rad * angle * i);
            var z = position.x * Mathf.Sin(Mathf.Deg2Rad * angle * i) + position.z * Mathf.Cos(Mathf.Deg2Rad * angle * i);

            position.x = x;
            position.z = z;
            position *= 5.0f;
            position.y = selection[i].transform.localPosition.y;

            selection[i].transform.localPosition = position;
        }
    }

    static GameObject toCopy = null;
    [MenuItem("Tools/Copy/SelectObjectToCopy")]
    public static void Select()
    {
        toCopy = Selection.activeGameObject;
    }
    [MenuItem("Tools/Copy/CopyObjectIntoSelection")]
    public static void CopyObjectIntoSelection()
    {
        if (toCopy != null)
        {
            foreach (var go in Selection.gameObjects)
            {
                var tmp = GameObject.Instantiate(toCopy);
                Undo.RegisterCreatedObjectUndo(tmp, "");
                tmp.name = toCopy.name;
                tmp.transform.SetParent(go.transform, false);
            }
        }
    }
}
