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
  children:
    child1:
      prefab: tower
      position: 0 0 0
    child2:
      prefab: tower
      position: 0 1 0
      children:
        grandchild:
          prefab: tower
          position: 1 0 0
monkeygym:
  - tower1:
      prefab: tower
      position: 4 4 0
  - tower2:
      prefab: tower
      position: 4 5 0
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
            if(item.Value.isArray)
            {
                go = new GameObject(item.Key);
                foreach (var _item in item.Value)
                {
                    ApplyLayout(_item.Value, go);
                }
            }
            else if(item.Value["prefab"])
            {
                Object[] objs = AssetDatabase.LoadAllAssetsAtPath("Assets/Prefabs/" + item.Value["prefab"] + ".prefab");
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

            if(item.Value["children"])
            {
                ApplyLayout(item.Value["children"], go);
            }
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