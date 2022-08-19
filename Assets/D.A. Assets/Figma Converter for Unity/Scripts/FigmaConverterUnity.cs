//
//███████╗██╗░██████╗░███╗░░░███╗░█████╗░  ░█████╗░░█████╗░███╗░░██╗██╗░░░██╗███████╗██████╗░████████╗███████╗██████╗░
//██╔════╝██║██╔════╝░████╗░████║██╔══██╗  ██╔══██╗██╔══██╗████╗░██║██║░░░██║██╔════╝██╔══██╗╚══██╔══╝██╔════╝██╔══██╗
//█████╗░░██║██║░░██╗░██╔████╔██║███████║  ██║░░╚═╝██║░░██║██╔██╗██║╚██╗░██╔╝█████╗░░██████╔╝░░░██║░░░█████╗░░██████╔╝
//██╔══╝░░██║██║░░╚██╗██║╚██╔╝██║██╔══██║  ██║░░██╗██║░░██║██║╚████║░╚████╔╝░██╔══╝░░██╔══██╗░░░██║░░░██╔══╝░░██╔══██╗
//██║░░░░░██║╚██████╔╝██║░╚═╝░██║██║░░██║  ╚█████╔╝╚█████╔╝██║░╚███║░░╚██╔╝░░███████╗██║░░██║░░░██║░░░███████╗██║░░██║
//╚═╝░░░░░╚═╝░╚═════╝░╚═╝░░░░░╚═╝╚═╝░░╚═╝  ░╚════╝░░╚════╝░╚═╝░░╚══╝░░░╚═╝░░░╚══════╝╚═╝░░╚═╝░░░╚═╝░░░╚══════╝╚═╝░░╚═╝
//
//███████╗░█████╗░██████╗░  ██╗░░░██╗███╗░░██╗██╗████████╗██╗░░░██╗
//██╔════╝██╔══██╗██╔══██╗  ██║░░░██║████╗░██║██║╚══██╔══╝╚██╗░██╔╝
//█████╗░░██║░░██║██████╔╝  ██║░░░██║██╔██╗██║██║░░░██║░░░░╚████╔╝░
//██╔══╝░░██║░░██║██╔══██╗  ██║░░░██║██║╚████║██║░░░██║░░░░░╚██╔╝░░
//██║░░░░░╚█████╔╝██║░░██║  ╚██████╔╝██║░╚███║██║░░░██║░░░░░░██║░░░
//╚═╝░░░░░░╚════╝░╚═╝░░╚═╝  ░╚═════╝░╚═╝░░╚══╝╚═╝░░░╚═╝░░░░░░╚═╝░░░
//

#if UNITY_EDITOR
using UnityEditor;
using DA_Assets.FCU.Model;
using System;
using System.Collections.Generic;
using System.Linq;
#if TMPRO_EXISTS
using TMPro;
#endif
using UnityEngine;

namespace DA_Assets.FCU
{
    [ExecuteInEditMode]
    public class FigmaConverterUnity : MonoBehaviour
    {
        public MainSettings mainSettings = DefaultSettings.mainSettings;
        public StandardTextSettings defaultTextSettings = DefaultSettings.defaultTextSettings;
        public ProceduralImageSettings proceduralImageSettings = DefaultSettings.proceduralImageSettings;
        public List<CustomPrefab> customPrefabs = new List<CustomPrefab>();
        /// <summary> Buffer for hamburger menu items. </summary>
        public bool[] itemsBuffer = new bool[32];

#if TMPRO_EXISTS
        public TextMeshProSettings textMeshProSettings = DefaultSettings.textMeshProSettings;
#endif
#if I2LOC_EXISTS
        public I2.Loc.LanguageSource languageSource;
#endif
#if TMPRO_EXISTS
        public List<TMP_FontAsset> textMeshProFonts;
#endif
        private static FigmaConverterUnity instance;
        public InstanceInfo instanceInfo = new InstanceInfo();
        public static FigmaConverterUnity Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<FigmaConverterUnity>();
                }
                else if (instance.instanceInfo.IsActive == false)
                {
                    FigmaConverterUnity[] figmaConverters = FindObjectsOfType<FigmaConverterUnity>();
                    figmaConverters = figmaConverters.Where(x => x.instanceInfo.IsActive).ToArray();
                    instance = figmaConverters[0];
                }

                instance.instanceInfo.Id = instance.gameObject.GetInstanceID();

                return instance;
            }
        }

#if JSON_NET_EXISTS
        public bool getPageFramesButtonVisible = false;
        public bool downloadFramesButtonVisible = false;
        /// <summary> Array with fonts used in figma layout. </summary>
        public List<Font> fonts;

        /// <summary> Downloadable figma frames. </summary>
        public List<SelectableFObject> framesToDownload = new List<SelectableFObject>();
        public List<SelectableFObject> pagesForSelect = new List<SelectableFObject>();

        public FObject selectedPage;

        /// <summary> Downloaded figma frames. </summary>
        public List<FObject> downloadedFrames = new List<FObject>();
        public List<FObject> downloadedPages = new List<FObject>();
#endif

        private Editor editor;
        public Editor Editor
        {
            get
            {
                if (editor == null)
                {
                    editor = ScriptableObject.CreateInstance<Editor>();
                }

                return editor;
            }
        }
    }
}

public class SelectableFObject
{
    public string Id;
    public string Name;
    public bool Selected;
}
[Serializable]
public class CustomPrefab
{
    public string Tag;
    public GameObject Prefab;
}
#endif
