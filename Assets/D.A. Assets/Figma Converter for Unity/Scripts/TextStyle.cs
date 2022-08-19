#if UNITY_EDITOR && JSON_NET_EXISTS
using System;
using System.Collections.Generic;
using DA_Assets.FCU.Model;
using UnityEngine.UI;
using UnityEngine;
#if TRUESHADOW_EXISTS
#endif
#if TMPRO_EXISTS
using TMPro;
#endif
namespace DA_Assets.FCU.Extensions
{
    public static class TextStyle
    {

#if TMPRO_EXISTS
        public static void SetTextMeshProStyle(this TextMeshProUGUI text, FObject fobject)
        {
            text.text = fobject.Characters;
            text.fontSize = fobject.Style.FontSize;

            if (fobject.Fills[0].GradientStops != null)
            {
                text.color = fobject.Fills[0].GradientStops[0].Color;
            }
            else
            {
                text.color = fobject.Fills[0].Color;
            }

            text.overrideColorTags = FigmaConverterUnity.Instance.textMeshProSettings.OverrideTags;
            text.enableAutoSizing = FigmaConverterUnity.Instance.textMeshProSettings.AutoSize;
            text.enableWordWrapping = FigmaConverterUnity.Instance.textMeshProSettings.Wrapping;
            text.richText = FigmaConverterUnity.Instance.textMeshProSettings.RichText;
            text.raycastTarget = FigmaConverterUnity.Instance.textMeshProSettings.RaycastTarget;
            text.parseCtrlCharacters = FigmaConverterUnity.Instance.textMeshProSettings.ParseEscapeCharacters;
            text.useMaxVisibleDescender = FigmaConverterUnity.Instance.textMeshProSettings.VisibleDescender;
            text.enableKerning = FigmaConverterUnity.Instance.textMeshProSettings.Kerning;
            text.extraPadding = FigmaConverterUnity.Instance.textMeshProSettings.ExtraPadding;
            text.overflowMode = FigmaConverterUnity.Instance.textMeshProSettings.Overflow;
            text.horizontalMapping = FigmaConverterUnity.Instance.textMeshProSettings.HorizontalMapping;
            text.verticalMapping = FigmaConverterUnity.Instance.textMeshProSettings.VerticalMapping;
            text.geometrySortingOrder = FigmaConverterUnity.Instance.textMeshProSettings.GeometrySorting;

            text.SetTextMeshProAligment(fobject);
            text.SetFigmaFont(fobject);
        }
        public static void SetTextMeshProAligment(this TextMeshProUGUI text, FObject fobject)
        {
            string textAligment = fobject.Style.TextAlignVertical + " " + fobject.Style.TextAlignHorizontal;

            switch (textAligment)
            {
                case "BOTTOM CENTER":
                    text.alignment = TextAlignmentOptions.Bottom;
                    break;
                case "BOTTOM LEFT":
                    text.alignment = TextAlignmentOptions.BottomLeft;
                    break;
                case "BOTTOM RIGHT":
                    text.alignment = TextAlignmentOptions.BottomRight;
                    break;
                case "CENTER CENTER":
                    text.alignment = TextAlignmentOptions.Center;
                    break;
                case "CENTER LEFT":
                    text.alignment = TextAlignmentOptions.Left;
                    break;
                case "CENTER RIGHT":
                    text.alignment = TextAlignmentOptions.Right;
                    break;
                case "TOP CENTER":
                    text.alignment = TextAlignmentOptions.Top;
                    break;
                case "TOP LEFT":
                    text.alignment = TextAlignmentOptions.TopLeft;
                    break;
                case "TOP RIGHT":
                    text.alignment = TextAlignmentOptions.TopRight;
                    break;
                default:
                    text.alignment = TextAlignmentOptions.Center;
                    break;
            }
        }
#endif
        public static void SetDefaultTextStyle(this Text text, FObject fobject)
        {
            text.resizeTextForBestFit = FigmaConverterUnity.Instance.defaultTextSettings.BestFit;
            text.text = fobject.Characters;
            text.resizeTextMinSize = 4;

            text.resizeTextMaxSize = Convert.ToInt32(fobject.Style.FontSize);
            text.fontSize = Convert.ToInt32(fobject.Style.FontSize);

            text.verticalOverflow = FigmaConverterUnity.Instance.defaultTextSettings.VerticalWrapMode;
            text.horizontalOverflow = FigmaConverterUnity.Instance.defaultTextSettings.HorizontalWrapMode;
            text.lineSpacing = FigmaConverterUnity.Instance.defaultTextSettings.FontLineSpacing;

            if (fobject.Fills[0].GradientStops != null)
            {
                text.color = fobject.Fills[0].GradientStops[0].Color;
            }
            else
            {
                text.color = fobject.Fills[0].Color;
            }

            text.SetFigmaFont(fobject);
            text.SetDefaultTextFontStyle(fobject);
            text.SetDefaultTextAligment(fobject);
        }
        public static void SetDefaultTextFontStyle(this Text text, FObject fobject)
        {
            string fontStyleRaw = fobject.Style.FontPostScriptName;

            if (fontStyleRaw != null)
            {
                if (fontStyleRaw.Contains(FontStyle.Bold.ToString()))
                {
                    if (fobject.Style.Italic)
                    {
                        text.fontStyle = FontStyle.BoldAndItalic;
                    }
                    else
                    {
                        text.fontStyle = FontStyle.Bold;
                    }
                }
                else if (fobject.Style.Italic)
                {
                    text.fontStyle = FontStyle.Italic;
                }
                else
                {
                    text.fontStyle = FontStyle.Normal;
                }
            }
        }
        public static void SetDefaultTextAligment(this Text text, FObject fobject)
        {
            string textAligment = fobject.Style.TextAlignVertical + " " + fobject.Style.TextAlignHorizontal;

            switch (textAligment)
            {
                case "BOTTOM CENTER":
                    text.alignment = TextAnchor.LowerCenter;
                    break;
                case "BOTTOM LEFT":
                    text.alignment = TextAnchor.LowerLeft;
                    break;
                case "BOTTOM RIGHT":
                    text.alignment = TextAnchor.LowerRight;
                    break;
                case "CENTER CENTER":
                    text.alignment = TextAnchor.MiddleCenter;
                    break;
                case "CENTER LEFT":
                    text.alignment = TextAnchor.MiddleLeft;
                    break;
                case "CENTER RIGHT":
                    text.alignment = TextAnchor.MiddleRight;
                    break;
                case "TOP CENTER":
                    text.alignment = TextAnchor.UpperCenter;
                    break;
                case "TOP LEFT":
                    text.alignment = TextAnchor.UpperLeft;
                    break;
                case "TOP RIGHT":
                    text.alignment = TextAnchor.UpperRight;
                    break;
                default:
                    text.alignment = TextAnchor.MiddleCenter;
                    break;
            }
        }

#if TMPRO_EXISTS
        /// <summary> This method written by <see href="https://github.com/HyperLethalVector"/> </summary>
        public static void SetFigmaFont(this TextMeshProUGUI text, FObject fobject)
        {
            List<TMP_FontAsset> fonts = FigmaConverterUnity.Instance.textMeshProFonts;

            if (fonts == null)
            {
                return;
            }

            foreach (TMP_FontAsset font in fonts)
            {
                if (font == null)
                    continue;

                float _sim = fobject.Style.FontPostScriptName.CalculateSimilarity(font.name.Replace(" SDF",""));

                if (_sim >= Constants.PROBABILITY_MATCHING_FONS)
                {
                    text.font = font;
                    text.fontSharedMaterial = font.material;
                    return;
                }
            }
        }
#endif

        public static void SetFigmaFont(this Text text, FObject fobject)
        {
            List<Font> fonts = UnityEngine.Object
               .FindObjectOfType<Canvas>()
               .GetComponent<FigmaConverterUnity>().fonts;

            if (fonts == null)
            {
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                return;
            }

            foreach (Font font in fonts)
            {
                if (font == null)
                    continue;

                float _sim = fobject.Style.FontPostScriptName.CalculateSimilarity(font.name);
                if (_sim >= Constants.PROBABILITY_MATCHING_FONS)
                {
                    text.font = font;
                    return;
                }
            }

            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
    }
}
#endif
