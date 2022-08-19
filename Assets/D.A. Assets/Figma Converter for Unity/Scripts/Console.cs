#if UNITY_EDITOR
using DA_Assets;
using System;
using UnityEditor;
using UnityEngine;

namespace DA_Assets.FCU
{
    public static class Console
    {
        public static string redColor = "red";
        public static string blackColor = "black";
        public static string whiteColor = "white";
        public static string violetColor = "#8b00ff";
        public static string orangeColor = "#ffa500";
        public static void Error(string log)
        {
            UnityEngine.Debug.LogError(log.TextColor(redColor).TextBold());
        }
        public static void Error(Exception ex)
        {
            UnityEngine.Debug.LogError(ex.ToString().TextColor(redColor).TextBold());
        }
        public static void Warning(string log)
        {
            UnityEngine.Debug.LogWarning(log.TextColor(orangeColor).TextBold());
        }

        public static void WriteLine(string log)
        {
            string color = EditorGUIUtility.isProSkin ? whiteColor : blackColor;
            UnityEngine.Debug.Log(log.TextColor(color).TextBold());
        }

        public static void Success(string log)
        {
            UnityEngine.Debug.Log(log.TextColor(whiteColor).TextBold());
        }
        public static void Log(string log)
        {
            if (FigmaConverterUnity.Instance.mainSettings.DebugMode)
            {
                UnityEngine.Debug.Log(log);
            }  
        }
    }
}
#endif