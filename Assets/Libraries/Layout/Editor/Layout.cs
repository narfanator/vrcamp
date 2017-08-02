using UnityEngine;
using UnityEditor;
using Maptionary;

public class Layout : ScriptableObject
{
    [MenuItem("Tools/Layout/Basics")]
    static void DoIt()
    {
        string layout_description = @"
tower1:
    prefab: tower
    position: 1 2 3
";

        Node layout = Parser.Parse(layout_description);

        GameObject go = new GameObject("layout");

        ApplyLayout(layout, go);
    }

    static void ApplyLayout(Node layout, GameObject parent)
    {
        foreach(var item in layout)
        {
            //Basic layout - everything this level is an object description
            GameObject go;
            if(item.Value["prefab"])
            {
                Object[] objs = AssetDatabase.LoadAllAssetsAtPath("Assets/Prefabs/" + item.Value["prefab"] + ".prefab");
                Debug.Log(objs);
                go = Instantiate(
                    objs[0],
                    parent.transform
                ) as GameObject;
                go.name = item.Key;
            }
            else
            {
                go = new GameObject(item.Key);
            }
            go.transform.SetParent(parent.transform);
            go.transform.localPosition = ToVector3(item.Value["position"]);
        }
    }

    public static Vector3 ToVector3(Node n)
    {
        if(n)
        {
            string[] pos = n.leaf.Split(' ');
            return new Vector3(
                float.Parse(pos[0]),
                float.Parse(pos[1]),
                float.Parse(pos[2])
            );
        } else
        {
            return new Vector3(0, 0, 0);
        }
    }
}