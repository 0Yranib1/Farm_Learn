using System.Collections;
using System.Collections.Generic;
using MFarm.Transition;
using UnityEngine;

namespace MFarm.Save
{
    /// <summary>
    /// 进度条 string 是guid
    /// </summary>
    public class DataSlot 
    {
        public Dictionary<string, GameSaveData> dataDict = new Dictionary<string, GameSaveData>();

        #region 显示UI进度详情

        public string DataTime
        {
            get
            {
                var key = TimeManager.Instance.GUID;

                if (dataDict.ContainsKey(key))
                {
                    var timeData=dataDict[key];
                    return timeData.timeDict["gameYear"]+"年/"+(Season)timeData.timeDict["gameSeason"]+"/"+timeData.timeDict["gameMonth"]+"月/"+timeData.timeDict["gameDay"]+"日/"+timeData.timeDict["gameHour"]+":"+timeData.timeDict["gameMinute"];
                }
                else
                {
                    return string.Empty;
                }
                
            }
        }

        public string DataScene
        {
            get
            {
                var key = TransitionManager.Instance.GUID;

                if (dataDict.ContainsKey(key))
                {
                    var transitionData=dataDict[key];
                    return transitionData.dataSceneName switch { "01Field"=>"农场",
                        "02House"=>"房屋",
                        "03Town"=>"小镇",
                        _ => "未知"
                    };
                }
                else
                {
                    return string.Empty;
                }
                
            }
        }
        
        #endregion

        
    }
    
}

