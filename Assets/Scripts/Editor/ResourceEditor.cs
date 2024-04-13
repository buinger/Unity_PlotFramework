using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ResourceEditor : MonoBehaviour
{
    // 快捷键设置文本物体名字
    [MenuItem("资源操作/一键配置Plot资源信息")]
    public static void SetAllPlotName()
    {
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/HotResources" });

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Log(path);

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                PrefabInfo temp = prefab.GetComponent<PrefabInfo>();
                if (temp == null)
                {
                    temp = prefab.AddComponent<PrefabInfo>();
                }

                bool ifChange = false;

                string[] strs = path.Split('/');
                string name = strs[strs.Length - 1];
                name = name.Replace(".prefab", "");
                if (name != temp.shortName)
                {
                    temp.shortName = name;
                    ifChange = true;
                }

                string aimPath = path.Replace(Application.persistentDataPath + "/", "");

                if (temp.path != aimPath)
                {
                    temp.path = aimPath;
                    ifChange = true;
                }

                if (ifChange)
                {
                    EditorUtility.SetDirty(prefab);
                    AssetDatabase.SaveAssets();
                }
            }
        }
        AssetDatabase.Refresh();
        Debug.Log("预制件信息设置完毕");
    }
}
