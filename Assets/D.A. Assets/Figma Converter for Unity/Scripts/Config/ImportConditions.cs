#if UNITY_EDITOR && JSON_NET_EXISTS
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using System.Linq;
using UnityEngine;

namespace DA_Assets.FCU
{
    public static class ImportConditions
    {
        public static bool IsDownloadable(this FObject fobject)
        {
            if (IsDownloadableImage(fobject) == false)
            {
                return false;
            }

            bool value = fobject.IsParent ||
                         fobject.FTag == FTag.Button ||
                         fobject.FTag == FTag.Text ||
                         fobject.FTag == FTag.Frame ||
                         fobject.FTag == FTag.HorizontalLayoutGroup ||
                         fobject.FTag == FTag.VerticalLayoutGroup ||
                         fobject.FTag == FTag.GridLayoutGroup ||
                         fobject.CustomTag != null;

            return !value;
        }
        public static bool IsDownloadableImage(FObject fobject)
        {
            if (fobject.ContainsBetterButtonBg())
            {
                Console.Log($"IsDownloadableImage | {fobject.GetFOBjectHierarchy()} | (fobject.ContainsBetterButtonBg())");
                return true;
            }
            else if(fobject.Type == "VECTOR")
            {
                Console.Log($"IsDownloadableImage | {fobject.GetFOBjectHierarchy()} | (fobject.Type == 'VECTOR')");
                return true;
            }
            else if(fobject.Children != null)
            {
                Console.Log($"IsDownloadableImage | {fobject.GetFOBjectHierarchy()} | (fobject.Children != null)");
                return true;
            }
            else if(fobject.Fills == null)
            {
                Console.Log($"IsDownloadableImage | {fobject.GetFOBjectHierarchy()} | (fobject.Fills == null)");
                return true;
            }
            else if(fobject.Type == "REGULAR_POLYGON")
            {
                Console.Log($"IsDownloadableImage | {fobject.GetFOBjectHierarchy()} | (fobject.Type == 'REGULAR_POLYGON')");
                return true;
            }
            else if (fobject.Type == "STAR")
            {
                Console.Log($"IsDownloadableImage | {fobject.GetFOBjectHierarchy()} | (fobject.Type == 'STAR')");
                return true;
            }
            else if (fobject.Type == "LINE")
            {
                Console.Log($"IsDownloadableImage | {fobject.GetFOBjectHierarchy()} | (fobject.Type == 'LINE')");
                return true;
            }

            bool solidFill = fobject.Fills.Count() == 1 && fobject.Fills[0].Type == "SOLID";
            bool linearFill = fobject.Fills.Count() == 1 && fobject.Fills[0].Type == "GRADIENT_LINEAR";

            if (FigmaConverterUnity.Instance.mainSettings.ImageComponent == ImageComponent.UnityImage)
            {
                if (fobject.Type == "ELLIPSE")
                {
                    Console.Log($"IsDownloadableImage | {fobject.GetFOBjectHierarchy()} | (fobject.Type == 'ELLIPSE')");
                    return true;
                }
                else if (solidFill == false)
                {
                    Console.Log($"IsDownloadableImage | {fobject.GetFOBjectHierarchy()} | (solidFill == false)");
                    return true;
                }
                else if (fobject.CornerRadius > 0)
                {
                    Console.Log($"IsDownloadableImage | {fobject.GetFOBjectHierarchy()} | (fobject.CornerRadius > 0)");
                    return true;
                }
                else if(fobject.RectangleCornerRadius == null)
                {
                    Console.Log($"IsDownloadableImage | {fobject.GetFOBjectHierarchy()} | (fobject.RectangleCornerRadius == null)");
                    return false;
                }

                foreach (var item in fobject.RectangleCornerRadius)
                {
                    if (item > 0)
                    {
                        Console.Log($"IsDownloadableImage | {fobject.GetFOBjectHierarchy()} | if (item > 0)");
                        return false;
                    }
                }

                return true;
            }
#if MPUIKIT_EXISTS
            else if(FigmaConverterUnity.Instance.mainSettings.ImageComponent == ImageComponent.MPImage)
            {
                if (solidFill == false && linearFill == false)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
#endif
#if PUI_EXISTS
            else if (FigmaConverterUnity.Instance.mainSettings.ImageComponent == ImageComponent.ProceduralImage)
            {
                if (solidFill == false)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
#endif
            return true;
        }
        public static bool NeedDeleteBackground(this FObject fobject)
        {
            bool value = fobject.IsParent ||
                fobject.FTag == FTag.Button ||
                fobject.FTag == FTag.InputField ||
                fobject.FTag == FTag.Frame ||
                fobject.FTag == FTag.HorizontalLayoutGroup ||
                fobject.FTag == FTag.VerticalLayoutGroup ||
                fobject.FTag == FTag.GridLayoutGroup;

            return value;
        }
    }
}
#endif
