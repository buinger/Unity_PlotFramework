using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
using System.Text;

/// <summary>
/// Excel解析基类
/// </summary>
public class ExcelManager : MonoBehaviour, IBaseComponent
{
    //行：分割数据标识 \n
    private readonly static char[] SPLIT_ROW = new char[] { '\n' };
    //列：分割数据标识 \t
    private readonly static char[] SPLIT_COL = new char[] { '\t' };

    private static Dictionary<string, Excel> loadedExcel = new Dictionary<string, Excel>();

    public static ExcelManager instance;


    /// <summary>
    /// 按行读取表格数据
    /// </summary>
    public static string[] GetTxtLines(string fileTxt)
    {
        fileTxt = fileTxt.Replace("\r\n", "\n");
        string[] aimString = fileTxt.Split(SPLIT_ROW, StringSplitOptions.RemoveEmptyEntries);
        //Debug.Log(aimString[0]);
        //Debug.Log(aimString[1]);
        return aimString;
    }

    /// <summary>
    /// 按列读取表格数据 
    /// </summary>
    protected static string[] GetLineCells(string value, bool trimEmpty = false)
    {
        StringSplitOptions option = trimEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None;
        return value.Split(SPLIT_COL, option);
    }

    private static Excel StringToExcel(string path, string txtContent)
    {
        List<List<string>> cells = new List<List<string>>();
        string[] lines = GetTxtLines(txtContent);
        foreach (var item in lines)
        {
            List<string> lineCells = GetLineCells(item).ToList();
            cells.Add(lineCells);
        }
        return new Excel(path, cells);
    }



    /// <summary>
    /// 加载表格数据
    /// </summary>
    public static Excel GetExcel(string path, bool force = false)
    {
        if (loadedExcel.ContainsKey(path))
        {
            return loadedExcel[path];
        }
        else
        {
            if (force == true)
            {
                LoadExcel_MainThread(path);
                return loadedExcel[path];
            }
            else
            {
                Debug.LogError("尚未加载此表");
                return null;
            }
        }
    }



    public static void LoadExcel_MainThread(string path)
    {
        if (!loadedExcel.ContainsKey(path))
        {
            string filePath = path;
            // 检查文件是否存在
            if (File.Exists(filePath))
            {
                // 读取文件的所有文本内容
                //Encoding.GetEncoding("GB2312")
                string fileContent = File.ReadAllText(filePath, Encoding.GetEncoding("GB2312"));
                loadedExcel.Add(path, StringToExcel(path, fileContent));
                //Debug.Log("加载excel成功：" + path);


                // 输出读取到的文本内容到控制台
                //Debug.Log("File Content: " + fileContent);
            }
            else
            {
                Debug.LogError("File not found at path: " + filePath);
            }
        }
    }

    public static IEnumerator LoadExcel_IEnumerator(string path)
    {
        if (!loadedExcel.ContainsKey(path))
        {
            using (UnityWebRequest request = UnityWebRequest.Get(PathTool.GetStreamingAssetsUrlPath() + path))
            {
                yield return request.SendWebRequest();

                if (request.isDone)
                {
                    string fileContent = request.downloadHandler.text;
                    loadedExcel.Add(path, StringToExcel(path, fileContent));
                   // Debug.Log("注入excel成功：" + path);
                }
                else
                {
                    Debug.LogError("注入excel失败：" + path);
                    Debug.Log(request.error);
                }
            }
        }
    }


    private static string ExcelToString(Excel excel)
    {
        string txtContent = "";
        for (int i = 0; i < excel.rowCount; i++)
        {
            for (int j = 0; j < excel[i].Count; j++)
            {
                txtContent += excel[i, j];
                if (j != excel[i].Count - 1)
                {
                    txtContent += '\t';
                }
            }
            txtContent += "\r\n";
        }
        return txtContent;
    }

    public static void WriteExcel(Excel target, string aimFilePath = "")
    {
        string path;
        if (aimFilePath == "")
        {
            path = target.sourcePath;
        }
        else
        {
            path = aimFilePath;
        }
        string fullPath = path;
        BackupFile(fullPath);

        string content = ExcelToString(target);
        try
        {
            // 创建一个 StreamWriter 实例来写入文件
            //false, Encoding.GetEncoding("GB2312")
            using (StreamWriter writer = new StreamWriter(fullPath, false, Encoding.GetEncoding("GB2312")))
            {
                // 写入文本内容
                writer.Write(content);
            }

            Debug.Log("表格文本已写入到文件: " + fullPath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("写入文件时出错: " + e.Message);
        }
    }


    public static bool BackupFile(string filePath)
    {

        if (!File.Exists(filePath))
        {
            Debug.Log("无需备份，文件不存在: " + filePath);
            return false;
        }

        string fileName = Path.GetFileNameWithoutExtension(filePath);

        string saBackPath = PathTool.GetStreamingAssetsBackUpPath();

        string backupPath = saBackPath + fileName + "(" + TimeTool.GetTimeStamp() + ")" + ".txt"; // 备份文件的路径

        try
        {
            File.Copy(filePath, backupPath, true); // 备份文件
            Debug.Log("文件已备份到: " + backupPath);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("备份文件时出错: " + e.Message);
            return false;
        }
    }



    private void PreloadTables()
    {
        //Excel excel = ExcelManager.GetExcel(PathTool.preloadTablePath, true);
        //List<string> paths = excel.GetColumnList()[0];
        //for (int i = 1; i < paths.Count; i++)
        //{
        //    string fullPath = PathTool.excelFolderPath + paths[i];
        //    if (paths[i].Contains(".txt"))
        //    {
        //        StartCoroutine(ExcelManager.LoadExcel_IEnumerator(fullPath));
        //    }
        //    else
        //    {
        //        string[] txtFiles = Directory.GetFiles(fullPath, "*.txt");
        //        foreach (string filePath in txtFiles)
        //        {
        //            StartCoroutine(ExcelManager.LoadExcel_IEnumerator(filePath));
        //        }
        //    }
        //}
    }

    public void OnSceneChangeBegin()
    {

    }

    public void OnSceneChangeOver()
    {

    }

    public IEnumerator Init()
    {
        //预加载表格
        PreloadTables();
        yield return null;
    }
}

public interface ExcelObject
{
    void CreateExcel();
    void SetFromExcel();
}

public class Excel
{
    List<List<string>> allRows = new List<List<string>>();

    public string sourcePath;
    public Excel(string path, List<List<string>> cells)
    {
        allRows = cells;
        sourcePath = path;
    }

    public List<List<string>> GetColumnList()
    {
        List<List<string>> allColumns = new List<List<string>>();
        for (int i = 0; i < allRows[0].Count; i++)
        {
            allColumns.Add(new List<string>());
        }

        for (int i = 0; i < allRows.Count; i++)
        {
            for (int j = 0; j < allColumns.Count; j++)
            {
                allColumns[j].Add(allRows[i][j]);
            }
        }
        return allColumns;
    }

    public void RemoveRow(int index)
    {
        if (allRows.Count != 0)
        {
            if (index == 0 || index < allRows.Count)
            {
                allRows.RemoveAt(index);
            }
            else
            {
                Debug.LogError("移除失败，下标越界---当前长度" + allRows.Count + "   目标下标:" + index);
            }
        }
        else
        {
            Debug.LogError("没有可移除的行");
        }
    }

    public void RemoveColumn(int index)
    {
        if (allRows.Count == 0 || allRows[0].Count == 0)
        {
            Debug.LogError("表没有数据");
            return;
        }
        else
        {
            if (index == 0 || index < allRows[0].Count)
            {
                for (int i = 0; i < allRows.Count; i++)
                {
                    allRows[i].RemoveAt(index);
                }
            }
            else
            {
                Debug.LogError("移除失败，没有可移除的列");
            }
        }

    }

    public void AddRow(int index, List<string> newRow)
    {
        if (allRows.Count != 0)
        {
            if (index == 0 || index < allRows.Count)
            {
                allRows.Insert(index, newRow);
            }
            else
            {
                Debug.LogError("插入失败，下标大于行的长度");
            }
        }
        else
        {
            if (index == 0)
            {
                allRows.Add(newRow);
            }
            else
            {
                Debug.LogError("无法插入行" + index + "，表为空");
            }
        }

    }

    public void AddColumn(int index, List<string> newColumn)
    {

        if (allRows.Count != 0)
        {
            if (newColumn.Count != allRows.Count)
            {
                Debug.LogError("插入失败，新列数量不对");
                return;
            }
            if (index == 0 || index < allRows[0].Count)
            {
                for (int i = 0; i < allRows.Count; i++)
                {
                    allRows[i].Insert(index, newColumn[i]);
                }
            }
            else
            {
                Debug.LogError("插入失败，下标越界");
            }
        }
        else
        {
            if (index == 0)
            {
                allRows.Add(new List<string>());
                for (int i = 0; i < newColumn.Count; i++)
                {
                    allRows[0].Add(newColumn[i]);
                }
            }
            else
            {
                Debug.LogError("无法插入列" + index + "，表为空");
            }
        }
    }


    public int rowCount
    {
        get { return allRows.Count; }
    }

    public int columnCount
    {
        get { return allRows[0].Count; }
    }

    public string this[int rowIndex, int columnIndex]
    {
        get
        {
            if (rowIndex < allRows.Count && columnIndex < allRows[rowIndex].Count)
            {
                return allRows[rowIndex][columnIndex];
            }
            else
            {
                Debug.LogError("获取值失败，数组越界");
                return null;
            }
        }
        set
        {
            if (rowIndex < allRows.Count && columnIndex < allRows[rowIndex].Count)
            {
                allRows[rowIndex][columnIndex] = value;
            }
            else
            {
                Debug.LogError("设置值失败，数组越界");
            }
        }
    }

    public List<string> this[int rowIndex]
    {
        get
        {
            if (rowIndex < allRows.Count)
            {
                return allRows[rowIndex];
            }
            else
            {
                Debug.LogError("获取行数组失败，数组越界");
                return null;
            }
        }
    }


}



