using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kmax;

namespace Training
{
    public class FuZhuJianCha_Plot : UiPlot
    {
        /// <summary>
        /// 是否是初始化
        /// </summary>
        private bool IsInit = true;

        protected override void Ini(Action onIniOver)
        {
            onIniOver.Invoke();
        }

        protected override IEnumerator MainLogic()
        {
            //  是否操作结束
            bool isEnd = false;
            Action action = () => {
                isEnd = true;
            };
            GameEntry.UI.ShowUI(UIFormId.FuZhuJianChaUIForm, p => { }, IsInit, action);
            //  初始化结束
            IsInit = false;
            yield return new WaitUntil(() => isEnd);
            GameEntry.UI.CloseCurUI(UIFormId.FuZhuJianChaUIForm);
        }

        protected override void ResetPlot()
        {
            GameEntry.UI.CloseCurUI(UIFormId.FuZhuJianChaUIForm);
        }

    }
}