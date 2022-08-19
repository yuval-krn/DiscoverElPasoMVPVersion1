#if UNITY_EDITOR
#if JSON_NET_EXISTS
using Newtonsoft.Json;
#endif
using System.Collections.Generic;
using UnityEngine;

namespace DA_Assets.FCU
{
#if JSON_NET_EXISTS
    public class FCU_Prefs
    {
        public static void AddFrameToCurrentImportList(int instanceId, int frameId)
        {
            List<FrameInfo> frameIds = GetImportedFramesList();
            frameIds.Add(new FrameInfo
            {
                InstanceId = instanceId,
                FrameId = frameId
            });
            string sz = JsonConvert.SerializeObject(frameIds);
            PlayerPrefs.SetString(Constants.IMPORTED_FRAMES_PREFS_KEY, sz);
        }
        public static void ClearImportedFramesList()
        {
            PlayerPrefs.DeleteKey(Constants.IMPORTED_FRAMES_PREFS_KEY);
        }
        public static List<FrameInfo> GetImportedFramesList()
        {
            string importedFrames = PlayerPrefs.GetString(Constants.IMPORTED_FRAMES_PREFS_KEY, null);

            if (string.IsNullOrWhiteSpace(importedFrames))
            {
                return new List<FrameInfo>();
            }
            else
            {
                List<FrameInfo> frameIds = JsonConvert.DeserializeObject<List<FrameInfo>>(importedFrames);
                return frameIds;
            }
        }
    }
#endif
    public struct FrameInfo
    {
        public int InstanceId;
        public int FrameId;    
    }
}
#endif