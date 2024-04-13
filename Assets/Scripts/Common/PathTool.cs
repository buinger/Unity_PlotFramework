using System.IO;
using UnityEngine;

public class PathTool
{
    //---------------AB_Resources路径-----------------------
    //Assets/AB_Resources/UI/
    //public static string uiPrefabPath = "Assets/HotResources/Plot/PlotResource/UI/";
    //public static string uiPrefabPath_Elements = uiPrefabPath + "Elements/";
    //public static string uiPrefabPath_Containers = uiPrefabPath + "Containers/";


    //public static string scenePath = "Assets/HotResources/Scene";


   // public static string plotPrefabPath = "Assets/AB_Resources/Plots/";


    //-------------StreamingAssets路径----------------------
    //加载本地streamingasset的url前序路径

    public static string streamingAssetsPath = Application.streamingAssetsPath + "/";
    //Excel路径
    //public static string excelFolderPath = streamingAssetsPath + "Excel/";
    //public static string preloadTablePath = excelFolderPath + "预加载相对路径.txt";
    //public static string trainingTablePath = excelFolderPath + "实操流程/";


    public static string GetStreamingAssetsBackUpPath()
    {
        string aimPath = Application.dataPath;
        aimPath = aimPath.Replace("Assets", "StreamingAssetsBackUp");
        if (!Directory.Exists(aimPath))
        {
            Directory.CreateDirectory(aimPath);
        }
        aimPath += "/";
        return aimPath;
    }


    public static string GetStreamingAssetsUrlPath()
    {
        string urlPath;
#if UNITY_EDITOR || UNITY_STANDALONE

        urlPath = "file://" + Application.streamingAssetsPath + "/";
        //否则如果在Iphone下……
#elif UNITY_IPHONE
 
            urlPath = "file://" + Application.dataPath + "/Raw/";
            //否则如果在android下……
#elif UNITY_ANDROID
            urlPath = "jar:file://" + Application.dataPath + "!/assets/";
#endif
        return urlPath;
    }

}
