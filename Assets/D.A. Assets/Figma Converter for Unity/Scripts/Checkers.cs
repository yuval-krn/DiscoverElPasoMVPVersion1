#if UNITY_EDITOR && JSON_NET_EXISTS
using DA_Assets.FCU.Exceptions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DA_Assets.FCU
{
    public class Checkers
    {
        public static bool IsValidSettings()
        {
            List<string> errors = new List<string>();

            bool validUrl = IsValidFigmaProjectUrl(FigmaConverterUnity.Instance.mainSettings.ProjectUrl);
            if (validUrl == false)
            {
                errors.Add("Invalid figma project url.");
            }

            if (errors.Count > 0)
            {
                throw new InvalidSettingsException(errors);
            }

            return true;
        }

        public static bool IsValidFigmaProjectUrl(string url)
        {
            bool result =
                Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) &&
                (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps) &&
                url.Contains("figma.com/file/");

            return result;
        }

        public static bool IsValidApiKey()
        {
            if (FigmaConverterUnity.Instance.mainSettings.ApiKey.Length < 30)
            {
                return false;
            }

            return true;
        }
    }
}
#endif
