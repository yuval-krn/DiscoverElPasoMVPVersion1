#if UNITY_EDITOR
using System;
#if TMPRO_EXISTS
using TMPro;
#endif
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public struct MainSettings
    {
        public string ApiKey;
        public string ProjectUrl;
        public TagSeparator TagSeparator;
        public ImageFormat ImagesFormat;
        public ImageScale ImagesScale;
        public ImageComponent ImageComponent;
        public PivotType PivotType;
        public ShadowType ShadowType;
        public TextComponent TextComponent;
#if I2LOC_EXISTS
        public bool UseI2Localization;
#endif
        public bool UseCustomPrefabs;
        public bool ImportVectors;
        public bool ReDownloadSprites;
        public bool HTTPS;
        public bool DebugMode;
    }
    [Serializable]
    public struct StandardTextSettings
    {
        public float FontLineSpacing;
        public HorizontalWrapMode HorizontalWrapMode;
        public VerticalWrapMode VerticalWrapMode;
        public bool BestFit;      
    }
    public struct ProceduralImageSettings
    {
        public UnityImageType Type;
        public bool RaycastTarget;
        public ModifierType ModifierType;
        public float BorderWidth;
        public float FalloffDistance;
    }
#if TMPRO_EXISTS
    [Serializable]
    public struct TextMeshProSettings
    {
        public bool AutoSize;
        public bool OverrideTags;
        public bool Wrapping;
        public bool RichText;
        public bool RaycastTarget;
        public bool ParseEscapeCharacters;
        public bool VisibleDescender;
        public bool Kerning;
        public bool ExtraPadding;
        public TextOverflowModes Overflow;
        public TextureMappingOptions HorizontalMapping;
        public TextureMappingOptions VerticalMapping;
        public VertexSortingOrder GeometrySorting;
    }
#endif
}
#endif