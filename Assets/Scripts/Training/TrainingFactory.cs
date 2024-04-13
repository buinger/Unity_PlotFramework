using Kmax;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using System;

namespace Training
{

    public enum TrainingPartType
    {
        JieZhen,
        WenZhen,
        TiGeJianCha,
        FuZhuJianCha,
        BianBingBianZhen,
        ZhiLiao,
        XuanJiao,
    }

    public class TrainingFactory : Modelbase
    {

        public int targetGuestInfoId = 1;

        [Header("接诊模块")]
        public JieZhen jieZhen = new JieZhen();

        [Header("体格检查模块")]
        public TiGeJianCha tiGeJianCha = new TiGeJianCha();

        [Header("治疗模块")]
        public ZhiLiao zhiLiao = new ZhiLiao();

        [Header("宣教模块")]
        public XuanJiao xuanJiao = new XuanJiao();

        [Header("辅助检查")]
        public FuZhuJianCha fuZhuJianCha = new FuZhuJianCha();

        [Header("病历单物体")]
        public GameObject bingLiDan;

        List<TrainingPart> allPart = new List<TrainingPart>();
        [HideInInspector]
        public GameObject guest;

        [HideInInspector]
        public PlotController plotController;

        bool iniFlag = false;


     
        public void UpdateScore(TrainingPartType type, int id, bool additive, object other = null)
        {
            switch (type)
            {
                case TrainingPartType.JieZhen:
                    jieZhen.UpdateScore(id, additive, other);
                    break;
                case TrainingPartType.WenZhen:

                    break;
                case TrainingPartType.TiGeJianCha:
                    tiGeJianCha.UpdateScore(id, additive);
                    break;
                case TrainingPartType.FuZhuJianCha:
                    fuZhuJianCha.UpdateScore(id, additive);
                    break;
                case TrainingPartType.BianBingBianZhen:

                    break;
                case TrainingPartType.ZhiLiao:
                    zhiLiao.UpdateScore(id, additive);
                    break;
                case TrainingPartType.XuanJiao:
                    xuanJiao.UpdateScore(id, additive);
                    break;
                default:
                    Debug.LogError($"没有此类型模块{type}");
                    break;
            }
        }


        [ContextMenu("初始化实训场景")]
        public async void Ini()
        {
            if (iniFlag == true) return;
            //先获取PlotController
            plotController = transform.GetComponentInChildren<PlotController>();

            iniFlag = true;

            Task<GameObject> getBingli = GameEntry.PlotPool.GetFromPoolAsyncForce(false, "Assets/HotResources/UI/PlotUI/Training/BingLiDan.prefab"); ;
            await getBingli;
            bingLiDan = getBingli.Result;
            bingLiDan.transform.SetParent(GameEntry.PlotUiManager.CoverUI);
            bingLiDan.transform.localPosition = Vector3.zero;
            bingLiDan.transform.localScale = Vector3.one;
            (bingLiDan.transform as RectTransform).sizeDelta = new Vector2(Screen.width, Screen.height);
            bingLiDan.SetActive(false);
            //将所有模块添加入数组
            allPart.Add(jieZhen);
            allPart.Add(tiGeJianCha);
            allPart.Add(zhiLiao);
            allPart.Add(xuanJiao);
            allPart.Add(fuZhuJianCha);
            //统一初始化
            foreach (var item in allPart)
            {
                item.Ini();
            }
            ////隐藏客人
            //Modelbase guestModel = GameEntry.Course.GetModelSystem.GetModel(1);
            //guestModel.gameObject.SetActive(false);
            //guestPlayableDirector = guestModel.GetComponent<PlayableDirector>();

            plotController.Ini();

        }


        public void ReleaseTraining()
        {
            PlotController.ReleaseStaticData();                 
            GameEntry.PlotPool.ReleaseAll();
        }



        // 解析坐标字符串并返回Unity中的三维坐标
        public static Vector3 ParseCoordinates(string coordinateStr)
        {

            string coordinateString = coordinateStr.Replace("coordinateStr", "");
            coordinateString = coordinateString.Replace(")", "");
            string[] coordinates = coordinateString.Split(',');

            if (coordinates.Length == 3)
            {
                float x, y, z;

                if (float.TryParse(coordinates[0], out x) &&
                    float.TryParse(coordinates[1], out y) &&
                    float.TryParse(coordinates[2], out z))
                {
                    return new Vector3(x, y, z);
                }
                else
                {
                    Debug.LogError("无法解析坐标。无效的格式。");
                }
            }
            else
            {
                Debug.LogError("无法解析坐标。组件数量无效。");
            }

            // 在失败的情况下返回默认的Vector3
            return Vector3.zero;
        }


        public static async Task<PlotActionPair> GetPlotActionPair(string plotStr)
        {
            Task<Plot> getPlotTemp = GetPlotTempByTableStr(plotStr);
            await getPlotTemp;
            ChildPlotInformation headPlotTarget = new ChildPlotInformation(true, getPlotTemp.Result);
            Action<Plot> action = GetPlotInstActionByTableStr(plotStr);

            return new PlotActionPair(headPlotTarget, action);
        }


        /// <summary>
        /// 表Plot描述转Plot预制件
        /// </summary>
        /// <param name="plotStrInfo"></param>
        /// <returns></returns>
        public static async Task<Plot> GetPlotTempByTableStr(string plotStrInfo)
        {
            if (plotStrInfo == "")
            {
                Debug.LogError("字符数据为空！");
                return null;
            }

            string[] splitResult = plotStrInfo.Split('*');
            if (splitResult.Length == 2)
            {
                Task<GameObject> loadTempPlot;
                string head = splitResult[0];
                switch (head)
                {
                    case "0":
                        loadTempPlot = PlotPoolManager.PlotPool.LoadGameObjectAsync(splitResult[1]);
                        break;
                    case "1":
                        loadTempPlot = PlotPoolManager.PlotPool.LoadGameObjectAsync("Assets/HotResources/Plot/TrainingPlot/Common/TwoManTalk.prefab");
                        break;
                    case "2":
                        loadTempPlot = PlotPoolManager.PlotPool.LoadGameObjectAsync("Assets/HotResources/Plot/TrainingPlot/Common/Animation.prefab");
                        Debug.Log(loadTempPlot+"++++++++++");
                        break;
                    default:
                        Debug.LogError("字符串格式不对：" + plotStrInfo);
                        return null;
                }
                await loadTempPlot;
                return loadTempPlot.Result.GetComponent<Plot>();
            }
            else
            {
                Debug.LogError("字符串格式不对：" + plotStrInfo);
                return null;
            }

        }



        /// <summary>
        /// 表Plot描述转Plot路径
        /// </summary>
        /// <param name="plotStrInfo"></param>
        /// <returns></returns>
        public static Action<Plot> GetPlotInstActionByTableStr(string plotStrInfo)
        {
            if (plotStrInfo == "")
            {
                return null;
            }

            string[] splitResult = plotStrInfo.Split('*');
            if (splitResult.Length == 2)
            {
                string head = splitResult[0];
                int id;
                switch (head)
                {
                    case "0":
                        return null;
                    case "1":
                        id = int.Parse(splitResult[1]);
                        return (target) =>
                        {
                            TrainingTalk_2manTalk twoManTalkPlot = target as TrainingTalk_2manTalk;
                            twoManTalkPlot.id = id;
                        };

                    case "2":
                        id = int.Parse(splitResult[1]);
                        return (target) =>
                        {
                            Training_Timeline animationPlot = target as Training_Timeline;
                            animationPlot.id = id;
                        };
                    default:
                        Debug.LogError("字符串格式不对：" + plotStrInfo);
                        return null;
                }

            }
            else
            {
                Debug.LogError("字符串格式不对：" + plotStrInfo);
                return null;
            }
        }

        public void SwitchBingLiDan(bool active)
        {
            if (bingLiDan != null)
                bingLiDan.SetActive(active);
        }

    }


    public class PlotActionPair
    {
       public ChildPlotInformation plotInfo;
       public Action<Plot> onPlotInst;
        // 构造函数
        public PlotActionPair(ChildPlotInformation plotInfo, Action<Plot> onPlotInst)
        {
            this.plotInfo = plotInfo;
            this.onPlotInst = onPlotInst;
        }
    }


    public abstract class TrainingPart
    {
        [Header("总分设置")]
        public float fullScore = 10;

        protected IDataRow[] commonRows;

        protected IDataRow[] courseRows;

        public virtual void Ini()
        {
            SetTable();
        }

        protected abstract string GetName();

        protected abstract void SetCommonRows();

        protected abstract void SetCourseRows();

        public abstract void UpdateScore(int key, bool isPlus, object other = null);

        private void SetTable()
        {
            SetCommonRows();
            SetCourseRows();
            //IDataTable<DRCommonJieZhen> dRCommonEntrys = GameEntry.DataTable.GetDataTable<DRCommonJieZhen>();
            //rows= dRCommonEntrys.GetAllDataRows();
            if (commonRows.Length == 0)
            {
                Debug.LogError(GetName() + "表，未预加载");
            }
        }


        public abstract float GetScore();


        public class OperationInfo
        {
            public int id;
            public string name;
            public float scoreWeight;

            // 构造函数
            public OperationInfo(int id, string name, float weight)
            {
                this.id = id;
                this.name = name;
                this.scoreWeight = weight;
            }
        }
    }


    public abstract class SelectionWordsPart : TrainingPart
    {
        //------------------接诊------------------
        protected Dictionary<int, Dictionary<string, float>> allOperation = new Dictionary<int, Dictionary<string, float>>();
        protected Dictionary<int, Dictionary<string, float>> targetOperation = new Dictionary<int, Dictionary<string, float>>();
        protected Dictionary<int, string> selectionResult = new Dictionary<int, string>();

        protected float fullWeight = 0;





        public override float GetScore()
        {
            float nowWeight = 0;
            foreach (var item in selectionResult)
            {
                nowWeight += allOperation[item.Key][item.Value];
            }
            float result = nowWeight / fullWeight * fullScore;
            return result;
        }
    }


    public abstract class SelectionPart : TrainingPart
    {
        protected Dictionary<int, OperationInfo> allOperation = new Dictionary<int, OperationInfo>();

        protected Dictionary<int, OperationInfo> bingoOperation = new Dictionary<int, OperationInfo>();

        protected Dictionary<int, OperationInfo> targetOperation = new Dictionary<int, OperationInfo>();

        protected float fullWeight = 0;

        [Header("复杂的Plot（有内部计分方式）")]
        public List<string> independentPlotName = new List<string>();


        public override void Ini()
        {
            base.Ini();
            for (int i = 0; i < commonRows.Length; i++)
            {
                OperationInfo temp = GetOperationInfo(i);
                AddToAlloperation(temp.id, temp);
            }

            for (int i = 0; i < courseRows.Length; i++)
            {
                AddToTargetOperation(GetTargetOperationId(i));
            }
        }

        protected abstract OperationInfo GetOperationInfo(int index);

        protected abstract int GetTargetOperationId(int index);


        public override void UpdateScore(int key, bool isPlus, object other = null)
        {
            if (allOperation.ContainsKey(key))
            {
                if (targetOperation.ContainsKey(key))
                {
                    if (bingoOperation.ContainsKey(key))
                    {
                        if (!isPlus)
                        {
                            Debug.Log(GetName() + "去除项目得分:" + bingoOperation[key].name);
                            bingoOperation.Remove(key);
                        }
                    }
                    else
                    {
                        if (isPlus)
                        {
                            bingoOperation.Add(key, allOperation[key]);
                            Debug.Log(GetName() + "项目得分:" + bingoOperation[key].name);
                        }
                    }

                }
            }
        }

        protected void AddToAlloperation(int id, OperationInfo op)
        {
            if (allOperation.ContainsKey(id))
            {

                Debug.LogError(GetName() + "(总表)存在相同的操作项目id:" + id);
            }
            else
            {
                allOperation.Add(id, op);
            }
        }

        protected void AddToTargetOperation(int id)
        {
            if (allOperation.ContainsKey(id))
            {
                if (targetOperation.ContainsKey(id))
                {
                    Debug.LogError(GetName() + $"(课程表{GameEntry.Course.GetCurrentCourseId})存在相同的操作项目id:" + id);
                }
                else
                {
                    targetOperation.Add(id, allOperation[id]);
                    //加入总权重统计
                    fullWeight += allOperation[id].scoreWeight;
                }
            }
            else
            {
                Debug.LogError(GetName() + $"(课程表{GameEntry.Course.GetCurrentCourseId})无法在父表中找到相同id，报错id:" + id);
            }

        }


        protected abstract float GetIndependentPlotScoreWeight(float operationScoreWeight, string operationName);

        public override float GetScore()
        {
            float nowWeight = 0;

            foreach (var item in bingoOperation)
            {
                if (targetOperation.ContainsKey(item.Key))
                {
                    if (independentPlotName.Contains(allOperation[item.Key].name))
                    {
                        nowWeight += GetIndependentPlotScoreWeight(allOperation[item.Key].scoreWeight, allOperation[item.Key].name);
                    }
                    else
                    {
                        nowWeight += allOperation[item.Key].scoreWeight;
                    }
                }
            }

            float score = fullScore * (nowWeight / fullWeight);
            return score;
        }
    }
}



