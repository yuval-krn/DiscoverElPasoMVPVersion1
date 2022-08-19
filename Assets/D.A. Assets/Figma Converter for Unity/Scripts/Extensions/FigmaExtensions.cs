#if UNITY_EDITOR && JSON_NET_EXISTS

using DA_Assets.FCU.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace DA_Assets.FCU.Extensions
{
    public static class FigmaExtensions
    {
        public static bool ContainsBetterButtonBg(this FObject fobject)
        {
            List<string> statesDescs = Enum.GetValues(typeof(ButtonStates))
               .Cast<ButtonStates>()
               .Select(v => v.GetDescription())
               .ToList();

            foreach (string desc in statesDescs)
            {
                if (fobject.Name.ToLower().Contains(desc))
                {
                    return true;
                }
            }

            return false;
        }
        public static bool GetIsParentByTag(FTag tag)
        {
            switch (tag)
            {
                case FTag.Container:
                case FTag.Frame:
                case FTag.Button:
                case FTag.Page:
                case FTag.InputField:
                case FTag.HorizontalLayoutGroup:
                case FTag.VerticalLayoutGroup:
                case FTag.GridLayoutGroup:
                    return true;
            }

            return false;
        }
        public static FTag GetFigmaType(this FObject fobject)
        {
            if (fobject.Name.Contains(GetTagSeparator()))
            {
                FTag[] allTypes = Enum.GetValues(typeof(FTag))
                   .Cast<FTag>()
                   .Where(x => x != FTag.Null)
                   .ToArray();
                foreach (FTag ftag in allTypes)
                {
                    string tag = ftag.GetDescription();

                    string[] nameParts = fobject.Name.ToLower().Replace(" ", "").Split(GetTagSeparator());

                    if (nameParts.Length >= 1)
                    {
                        string name = nameParts[0];

                        float sim = name.CalculateSimilarity(tag);

                        if (name == tag)
                        {
                            fobject.IsParent = GetIsParentByTag(ftag);
                            Console.Log($"GetFigmaType | fobject.Name: {fobject.Name} | name: {name} | tag: {tag}");
                            return ftag;
                        }
                        else if (sim >= Constants.PROBABILITY_MATCHING_TAGS)
                        {
                            fobject.IsParent = GetIsParentByTag(ftag);
                            Console.Log($"GetFigmaType | fobject.Name: {fobject.Name} | name: {name} | tag: {tag}");
                            return ftag;
                        }

                    }
                }
            }

            fobject.IsParent = FigmaParser.IsParent(fobject);

            if (fobject.IsParent == false)
            {
                fobject.IsImage = fobject.IsImage();
            }

            Console.Log($"GetFigmaType | fobject.Name: {fobject.Name} | IsImage: {fobject.IsImage} | IsParent: {fobject.IsParent}");

            if (fobject.Type == FTag.Text.ToString().ToUpper())
            {
                return FTag.Text;
            }
            else if (fobject.IsImage)
            {
                return FTag.Image;
            }
            else if (fobject.LayoutMode == "VERTICAL")
            {
                return FTag.VerticalLayoutGroup;
            }
            else if (fobject.LayoutMode == "HORIZONTAL")
            {
                return FTag.HorizontalLayoutGroup;
            }
            else if (fobject.IsParent)
            {
                return FTag.Container;
            }

            return FTag.Null;
        }
        public static bool IsImage(this FObject fobject)
        {
            bool solidFill = false;
            bool gradientFill = false;
            bool isImageTag = fobject.FTag == FTag.Image || fobject.FTag == FTag.Background;
            bool isFillImage = false;

            if (fobject.Fills != null)
            {
                solidFill = fobject.Fills.Any(x => x.Type == "SOLID");
                gradientFill = fobject.Fills.Any(x => x.Type == "GRADIENT_LINEAR");
                isFillImage = fobject.Fills.Select(x => x.Type).Contains("IMAGE");
            }

            return isFillImage || solidFill || gradientFill || isImageTag || FigmaConverterUnity.Instance.mainSettings.ImportVectors;
        }
        public static bool IsVector(this FObject fobject)
        {
            return fobject.Type == "VECTOR";
        }
        public static string GetFOBjectHierarchy(this FObject fobject)
        {
            List<string> hierarchy = new List<string>();

            GetParentRecursive(fobject, hierarchy);
            hierarchy.Reverse();
            return string.Join("/", hierarchy);
        }
        public static int GetImportSpeed()
        {
            return 256;
        }
        public static void GetParentRecursive(FObject fobject, List<string> hierarchy)
        {
            hierarchy.Add(fobject.Name);
            if (fobject.Parent != null)
            {

                GetParentRecursive(fobject.Parent, hierarchy);
            }
            else
            {
                //  Debug.LogError($"parent for {fobject.Name} null");
            }
        }
        public static string GetCustomTag(this FObject fobject)
        {
            string[] customTags = FigmaConverterUnity.Instance.customPrefabs.Select(x => x.Tag).ToArray();

            foreach (string tag in customTags)
            {
                string[] nameParts = fobject.Name.ToLower().Replace(" ", "").Split(GetTagSeparator());

                if (nameParts.Length >= 1)
                {
                    string name = nameParts[0];

                    float sim = name.CalculateSimilarity(tag);

                    if (name == tag)
                    {
                        return tag;
                    }
                    else if (sim >= Constants.PROBABILITY_MATCHING_TAGS)
                    {
                        return tag;
                    }
                }
            }

            return null;
        }
        public static char GetTagSeparator()
        {
            switch (FigmaConverterUnity.Instance.mainSettings.TagSeparator)
            {
                case TagSeparator.Slash:
                    return '/';
                case TagSeparator.Dash:
                    return '-';
                default:
                    return '-';
            }
        }
        public static string GetImageExtension()
        {
            return FigmaConverterUnity.Instance.mainSettings.ImagesFormat.ToString().ToLower();
        }

        public static float GetImageScale()
        {
            switch (FigmaConverterUnity.Instance.mainSettings.ImagesScale)
            {
                case ImageScale.X_0_5:
                    return 0.5f;
                case ImageScale.X_0_75:
                    return 0.75f;
                case ImageScale.X_1_0:
                    return 1f;
                case ImageScale.X_1_5:
                    return 1.5f;
                case ImageScale.X_2_0:
                    return 2.0f;
                case ImageScale.X_3_0:
                    return 3.0f;
                case ImageScale.X_4_0:
                    return 4.0f;
                default:
                    return 4.0f;
            }
        }
        public static string GetAssetPath(this FObject fobject, bool full)
        {
            string name = $"{fobject.Name}.{GetImageExtension()}";
            string spriteDir;

            if (fobject.IsMutual)
            {
                spriteDir = "Mutual";
            }
            else
            {
                spriteDir = fobject.RootFrameName;
            }

            string spritesPath = $"{Application.dataPath}/Sprites/{spriteDir}";

            DirectoryInfo dinfo = Directory.CreateDirectory(spritesPath);

            string fullPath = $"{dinfo.FullName}/{name}";
            string shortPath = $"Assets/Sprites/{spriteDir}/{name}";

            if (full)
            {
                return fullPath;
            }
            else
            {
                return shortPath;
            }
        }

        public static Color GetTextColor(this FObject text)
        {
            if (text.Fills[0].GradientStops != null)
            {
                return text.Fills[0].GradientStops[0].Color;
            }
            else
            {
                return text.Fills[0].Color;
            }
        }

        public static void SetImgTypeSprite(this FObject fobject)
        {
            try
            {
                while (true)
                {
                    if (string.IsNullOrEmpty(fobject.AssetPath))
                    {
                        break;
                    }

                    TextureImporter importer = AssetImporter.GetAtPath(fobject.AssetPath) as TextureImporter;
                    if (importer.textureType == TextureImporterType.Sprite && importer.isReadable == true)
                    {
                        break;
                    }

                    importer.isReadable = true;
                    importer.textureType = TextureImporterType.Sprite;
                    AssetDatabase.WriteImportSettingsIfDirty(fobject.AssetPath);
                    AssetDatabase.Refresh();
                }
            }
            catch
            {
                Console.Warning(Localization.SPRITE_NOT_FOUND);
            }
        }

    }
}
#endif
