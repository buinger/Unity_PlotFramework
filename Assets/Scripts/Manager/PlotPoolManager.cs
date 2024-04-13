using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Kmax;
using UnityEngine.SceneManagement;
using static UnityEngine.AddressableAssets.Addressables;
using UnityEngine.Playables;

public class PlotPoolManager : MonoBehaviour, IBaseComponent
{

    public static PlotPoolManager instance;
    public GameObject GetFromPool(bool active, string fullname)
    {
        string aimFullName = fullname;
        if (!fullname.Contains(".prefab"))
        {
            aimFullName += ".prefab";
        }

        GameObject aimGameObj;
        aimGameObj = PlotPool.OutPool(aimFullName);
        if (aimGameObj == null)
        {
            return null;
        }
        else
        {
            aimGameObj.transform.SetParent(null);
            aimGameObj.SetActive(active);
            return aimGameObj;
        }
    }

    public async Task<GameObject> GetFromPoolAsyncForce(bool active, string fullname)
    {
        string aimFullName = fullname;
        if (!fullname.Contains(".prefab"))
        {
            aimFullName += ".prefab";
        }

        GameObject aimGameObj;
        aimGameObj = PlotPool.OutPool(aimFullName);

        if (aimGameObj == null)
        {
            Task getResource = PlotPool.PreLoadPrefabToPoolAsync(aimFullName);
            await getResource;
            aimGameObj = PlotPool.OutPool(aimFullName);
            aimGameObj.SetActive(active);
            return aimGameObj;
        }
        else
        {
            aimGameObj.transform.SetParent(null);
            aimGameObj.SetActive(active);
            return aimGameObj;
        }
    }

    public GameObject GetFromPool(bool active, string fullname, Vector3 postion)
    {
        GameObject aimGameObj = GetFromPool(active, fullname);
        if (aimGameObj == null)
        {
            return null;
        }
        aimGameObj.transform.position = postion;
        return aimGameObj;
    }

    public GameObject GetFromPool(bool active, string fullname, Vector3 postion, Transform father, bool isLocalSet = false)
    {
        GameObject aimGameObj = GetFromPool(active, fullname);
        if (aimGameObj == null)
        {
            return null;
        }
        aimGameObj.transform.SetParent(father);
        aimGameObj.transform.localScale = Vector3.one;
        if (isLocalSet)
        {
            aimGameObj.transform.localPosition = postion;
        }
        else
        {
            aimGameObj.transform.position = postion;
        }
        return aimGameObj;
    }

    //public GameObject GetFromPoolLikeFather(string fullname, Transform tempTrans, bool beSon = false)
    //{
    //    GameObject aimGameObj = GetFromPool(fullname);
    //    if (aimGameObj == null)
    //    {
    //        return null;
    //    }
    //    if (beSon == true)
    //    {
    //        aimGameObj.transform.SetParent(tempTrans);
    //        aimGameObj.transform.position = Vector3.zero;
    //        aimGameObj.transform.rotation = Quaternion.identity;
    //    }
    //    else
    //    {
    //        aimGameObj.transform.position = tempTrans.position;
    //        aimGameObj.transform.rotation = tempTrans.rotation;
    //    }
    //    return aimGameObj;
    //}

    public void PutInPool(GameObject gameObj)
    {
        PlotPool.InPool(gameObj);
    }

    public void ReleaseAll()
    {
        PlotPool.ReleaseAll();
    }

    public IEnumerator Init()
    {
        //SceneManager.sceneUnloaded += (scene) =>
        //{
        //    PlotPool.ReleaseAll();
        //};
        Debug.Log("Plot预制件对象池初始化成功");
        instance = this;
        yield return null;
    }

    //-------------------------------Plot池

    public class PlotPool
    {
        //所有生成的池物件
        private static List<GameObject> instantiatedObj = new List<GameObject>();
        //池字典
        public static Dictionary<string, List<GameObject>> itemPool = new Dictionary<string, List<GameObject>>();
        //加载过的资源
        public static Dictionary<string, GameObject> loadedPrefabs = new Dictionary<string, GameObject>();

        public static void ReleaseAll()
        {

            foreach (var item in instantiatedObj)
            {
                if (item != null)
                {
                    Destroy(item);
                }
            }
            instantiatedObj.Clear();
        }

        public static void InPool(GameObject gameObj)
        {
            Button but = gameObj.GetComponent<Button>();
            if (but != null)
            {
                but.onClick.RemoveAllListeners();
            }

            if (itemPool.ContainsKey(gameObj.name) == false)
            {
                itemPool.Add(gameObj.name, new List<GameObject>());
            }

            gameObj.SetActive(false);

            if (!itemPool[gameObj.name].Contains(gameObj))
            {
                itemPool[gameObj.name].Add(gameObj);
            }
        }

        public static GameObject OutPool(string assetPath)
        {
            if (itemPool.ContainsKey(assetPath) == false)
            {
                itemPool.Add(assetPath, new List<GameObject>());
            }

            if (itemPool[assetPath].Count == 0)
            {
                return null;
            }
            else
            {
                GameObject outGo = itemPool[assetPath][0];
                if (outGo != null)
                {
                    //这里有竞争条件，可能要Lock      
                    itemPool[assetPath].Remove(outGo);
                    outGo.SetActive(true);
                }
                return outGo;
            }
        }


        /// <summary>
        /// 通过路径获取预制件名字
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetNameFromPrefabPath(string path)
        {
            string target = "";
            string[] strs = path.Split('/');
            if (strs.Length != 0)
            {
                string temp = strs[strs.Length - 1];
                if (temp.Contains(".prefab"))
                {
                    target = temp.Replace(".prefab", "");
                }
            }
            return target;
        }


        /// <summary>
        /// 协程
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerator PreLoadPrefabToPoolIE(string path)
        {
            string assetPath = path;
            if (!assetPath.Contains(".prefab"))
            {
                assetPath += ".prefab";
            }

            Task<GameObject> loadTask = LoadGameObjectAsync(path);
            while (!loadTask.IsCompleted)
            {
                yield return null;
            }

            GameObject loadedObject = Instantiate(loadTask.Result);
            loadedObject.name = path;
            InPool(loadedObject);
            if (!instantiatedObj.Contains(loadedObject))
            {
                instantiatedObj.Add(loadedObject);
            }
            yield return loadedObject;

        }




        /// <summary>
        /// 异步
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async static Task<GameObject> PreLoadPrefabToPoolAsync(string path)
        {
            string assetPath = path;
            if (!assetPath.Contains(".prefab"))
            {
                assetPath += ".prefab";
            }

            Task<GameObject> loadTask = LoadGameObjectAsync(path);
            await loadTask;
            GameObject loadedObject = Instantiate(loadTask.Result);
            loadedObject.name = path;
            InPool(loadedObject);
            if (!instantiatedObj.Contains(loadedObject))
            {
                instantiatedObj.Add(loadedObject);
            }
            return loadedObject;
        }

        public static async Task<GameObject> LoadGameObjectAsync(string path)
        {
            if (loadedPrefabs.ContainsKey(path))
            {
                return loadedPrefabs[path];
            }
            else
            {
                AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(path);
                await handle.Task;
                loadedPrefabs.Add(path, handle.Task.Result);
                return handle.Task.Result;
            }
        }

        public static Dictionary<string, object> LoadedObjects = new Dictionary<string, object>();

        public static async Task<T> LoadSomethingAsync<T>(string path)
        {
            if (LoadedObjects.ContainsKey(path))
            {
                if (LoadedObjects[path] is T result)
                {
                    return result;
                }
                else
                {
                    throw new InvalidCastException(path + ":类型不匹配");
                }
            }
            else
            {
                AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(path);
                await handle.Task;
                LoadedObjects.Add(path, handle.Task.Result);
                return handle.Task.Result;

            }
        }

        //public static async Task<PlayableAsset> LoadSomethingAsync(string path)
        //{
        //    if (LoadedObjects.ContainsKey(path))
        //    {
        //        return LoadedObjects[path] as PlayableAsset;
        //    }
        //    else
        //    {
        //        AsyncOperationHandle<PlayableAsset> handle = Addressables.LoadAssetAsync<PlayableAsset>(path);
        //        await handle.Task;
        //        LoadedObjects.Add(path, handle.Task.Result);
        //        return handle.Task.Result;

        //    }
        //}

    }

}

//private static List<GameObject> allPoolPrefabs = new List<GameObject>();
//public static void ReleaseAll()
//{
//    foreach (var item in allPoolPrefabs)
//    {
//        GameEntry.Pool.Release<GameObject>(AssetType.Prefab, item, () =>
//        {
//            if (item != null)
//            {
//                Destroy(item);
//            }
//        });
//    }
//    allPoolPrefabs.Clear();
//}

//public static void InPool(GameObject gameObj)
//{
//    PrefabInfo temp = gameObj.GetComponent<PrefabInfo>();
//    Button but = gameObj.GetComponent<Button>();
//    if (but != null)
//    {
//        but.onClick.RemoveAllListeners();
//    }
//    GameEntry.Pool.AddItemToPool(temp.path, temp.name, gameObj, AssetType.Prefab, false);
//    GameEntry.Pool.DeSpawn(AssetType.Prefab, gameObj);

//}

//public static GameObject OutPool(string assetPath)
//{
//    GameObject target = null;
//    string assetName = GetNameFromPrefabPath(assetPath);

//    bool exist = GameEntry.Pool.isExitObject<GameObject>(assetPath, assetName, AssetType.Prefab);
//    if (exist)
//    {
//        target = GameEntry.Pool.GetObject<GameObject>(assetPath, assetName, AssetType.Prefab);
//        if (!allPoolPrefabs.Contains(target))
//        {
//            allPoolPrefabs.Add(target);
//        }
//        Button but = target.GetComponent<Button>();
//        if (but != null)
//        {
//            but.onClick.RemoveAllListeners();
//        }
//    }
//    return target;
//}


///// <summary>
///// 通过路径获取预制件名字
///// </summary>
///// <param name="path"></param>
///// <returns></returns>
//private static string GetNameFromPrefabPath(string path)
//{
//    string target = "";
//    string[] strs = path.Split('/');
//    if (strs.Length != 0)
//    {
//        string temp = strs[strs.Length - 1];
//        if (temp.Contains(".prefab"))
//        {
//            target = temp.Replace(".prefab", "");
//        }
//    }
//    return target;
//}


///// <summary>
///// 协程注册模板
///// </summary>
///// <param name="assetPath"></param>
///// <returns></returns>
//public static IEnumerator PreLoadPrefabToPoolIE(string path)
//{
//    string assetPath = path;
//    if (!assetPath.Contains(".prefab"))
//    {
//        assetPath += ".prefab";
//    }
//    string assetName = GetNameFromPrefabPath(assetPath);
//    //当池里没有相关元素时加载一次
//    if (!GameEntry.Pool.isExitObject<GameObject>(assetPath, assetName, AssetType.Prefab))
//    {
//        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(assetPath);
//        while (!handle.IsDone)
//        {
//            yield return null; // 等待下一帧继续检查异步操作状态
//        }
//        if (handle.Status == AsyncOperationStatus.Succeeded)
//        {
//            if (handle.Result != null)
//            {
//                GameObject loadedObject = Instantiate(handle.Result);
//                loadedObject.name = assetName;
//                GameEntry.Pool.AddItemToPool<GameObject>(assetPath, assetName, loadedObject, AssetType.Prefab, false);
//                GameEntry.Pool.DeSpawn<GameObject>(AssetType.Prefab, loadedObject);
//                if (!allPoolPrefabs.Contains(loadedObject))
//                {
//                    allPoolPrefabs.Add(loadedObject);
//                }
//                // loadedObject.transform.SetParent(null);
//            }
//            else
//            {
//                Debug.LogError("加载ab资源失败");
//            }
//        }
//        else
//        {
//            Debug.LogError("异步加载操作失败：" + handle.DebugName);
//        }
//    }
//}




///// <summary>
///// 异步注册模板
///// </summary>
///// <param name="assetPath"></param>
///// <returns></returns>
//public async static Task PreLoadPrefabToPoolAsync(string path)
//{
//    string assetPath = path;
//    if (!assetPath.Contains(".prefab"))
//    {
//        assetPath += ".prefab";
//    }
//    string assetName = GetNameFromPrefabPath(assetPath);
//    //当池里没有相关元素时加载一次
//    if (!GameEntry.Pool.isExitObject<GameObject>(assetPath, assetName, AssetType.Prefab))
//    {
//        Task<GameObject> handle = LoadGameObjectAsync(assetPath);
//        await handle;

//        if (handle.Result != null)
//        {
//            GameObject loadedObject = Instantiate(handle.Result);
//            loadedObject.name = assetName;
//            GameEntry.Pool.AddItemToPool<GameObject>(assetPath, assetName, loadedObject, AssetType.Prefab, false);
//            GameEntry.Pool.DeSpawn<GameObject>(AssetType.Prefab, loadedObject);
//            if (!allPoolPrefabs.Contains(loadedObject))
//            {
//                allPoolPrefabs.Add(loadedObject);
//            }
//            // loadedObject.transform.SetParent(null);
//        }
//        else
//        {
//            Debug.LogError("加载ab资源失败");
//        }
//    }


//}

//public static async Task<GameObject> LoadGameObjectAsync(string path)
//{
//    AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(path);
//    await handle.Task;
//    return handle.Task.Result;
//}