using DA_Assets.FCU.Exceptions;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DA_Assets.FCU
{
    [CustomEditor(typeof(FigmaConverterUnity))]
    internal class FCU_Editor : Editor
    {
        private GuiElements gui;
        private FigmaConverterUnity figmaConverterUnity;
        private bool selectDeselectToggleSelected;
        private bool _selectDeselectToggleTemp = true;
        internal GUIContent selectDeselectToggleLabel = new GUIContent("DESELECT ALL");
        private string selectedPageId = null;
        private SerializedObject fcuSerializedObject;

        private bool
            defineJsonNet1, defineJsonNet2,
            defineTrueShadow1, defineTrueShadow2,
            defineTextMeshPro1, defineTextMeshPro2,
            defineMPImage1, defineMPImage2,
            defineProceduralUIImage1, defineProceduralUIImage2,
            defineI2Loc1, defineI2Loc2;

        private void OnEnable()
        {
            try
            {
                if (gui == null)
                {
                    gui = CreateInstance<GuiElements>();
                }

                figmaConverterUnity = (FigmaConverterUnity)target;
                fcuSerializedObject = new SerializedObject(figmaConverterUnity);

                SetTogglesToCurrentDefines();

                ReActivateInstances();
            }
            catch (InvalidCastException)
            {

            }
        }

        private void ReActivateInstances()
        {
            FigmaConverterUnity[] figmaConverters = FindObjectsOfType<FigmaConverterUnity>();

            foreach (FigmaConverterUnity fcu in figmaConverters)
            {
                fcu.instanceInfo.IsActive = false;
            }

            figmaConverterUnity.instanceInfo.IsActive = true;

            Console.WriteLine(string.Format(Localization.INSTANCE_SELECTED, figmaConverterUnity.gameObject.name));
        }
        private void SetTogglesToCurrentDefines()
        {
            defineJsonNet1 = HandleDefine.IsDefineExists(Constants.JSONNET_DEFINE);
            defineTextMeshPro1 = HandleDefine.IsDefineExists(Constants.TEXTMESHPRO_DEFINE);
            defineTrueShadow1 = HandleDefine.IsDefineExists(Constants.TRUESHADOW_DEFINE);
            defineMPImage1 = HandleDefine.IsDefineExists(Constants.MPUIKIT_DEFINE);
            defineProceduralUIImage1 = HandleDefine.IsDefineExists(Constants.PUI_DEFINE);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            SetTogglesToCurrentDefines();
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                GUIStyle = gui.GetCustomStyle(CustomStyle.Background),
                Body = () =>
                {

                    gui.DrawGroup(new Group
                    {
                        GroupType = GroupType.Vertical,
                        GUIStyle = gui.GetCustomStyle(CustomStyle.Logo),
                        Texture2D = gui.logo,
                        Body = () =>
                        {
                            GUILayout.Space(gui.BIG_SPACE);
                            gui.DrawGroup(new Group
                            {
                                GroupType = GroupType.Horizontal,
                                Body = () =>
                                {
                                    GUILayout.FlexibleSpace();
                                    GUILayout.Space(gui.BIG_SPACE);
                                    GUILayout.Label(Constants.PRODUCT_VERSION, gui.GetCustomStyle(CustomStyle.VersionLabel));
                                }
                            });

#if JSON_NET_EXISTS
                            gui.ProgressBar(WebClient.pbarProgress, WebClient.pbarContent);
#endif
                        }
                    });

#if JSON_NET_EXISTS
                    GUILayout.Space(gui.NORMAL_SPACE);
                    gui.DrawGroup(new Group
                    {
                        GroupType = GroupType.Hamburger,
                        Label = "MAIN SETTINGS",
                        Fold = Fold.MainSettings,
                        Body = () =>
                        {
                            GUILayout.Space(gui.NORMAL_SPACE);

                            figmaConverterUnity.mainSettings.ApiKey = gui.TextField(new GUIContent("Token", "A token that can be obtained using authentication inside the asset, or through the settings menu in Figma."),
                                figmaConverterUnity.mainSettings.ApiKey);

                            figmaConverterUnity.mainSettings.ProjectUrl = gui.TextField(new GUIContent("Project Url", "Link to your project in Figma."),
                                figmaConverterUnity.mainSettings.ProjectUrl);

                            figmaConverterUnity.mainSettings.TagSeparator = gui.EnumField(new GUIContent("Tag Separator", "The default is '/', but '-' is recommended. The tag sign must not appear in the component name following the tag."),
                                figmaConverterUnity.mainSettings.TagSeparator);

                            figmaConverterUnity.mainSettings.ImagesFormat = gui.EnumField(new GUIContent("Images Format", "Format of images imported from Figma."),
                                figmaConverterUnity.mainSettings.ImagesFormat);

                            figmaConverterUnity.mainSettings.ImagesScale = gui.EnumField(new GUIContent("Images Scale", "The size of the images imported from Figma."),
                                figmaConverterUnity.mainSettings.ImagesScale);

                            figmaConverterUnity.mainSettings.ImageComponent = gui.EnumField(new GUIContent("Image Component", " "),
                                figmaConverterUnity.mainSettings.ImageComponent);

                            figmaConverterUnity.mainSettings.ShadowType = gui.EnumField(new GUIContent("Shadow Type", "The type of shadow that is used when transferring the shadow effect from Figma. Currently the 'TrueShadow' asset is supported, you can also turn off the shadow."),
                                figmaConverterUnity.mainSettings.ShadowType);

                            figmaConverterUnity.mainSettings.TextComponent = gui.EnumField(new GUIContent("Text Component", "A text component to be used when wrapping text from Figma. Currently 'Unity.UI.Text' and 'TextMeshPro' are supported."),
                                figmaConverterUnity.mainSettings.TextComponent);

                            figmaConverterUnity.mainSettings.PivotType = gui.EnumField(new GUIContent("Pivot Type", "Applied to all components on import."),
                                figmaConverterUnity.mainSettings.PivotType);

                            figmaConverterUnity.mainSettings.UseCustomPrefabs = gui.Toggle(new GUIContent("Use Custom Tags", "You can add your own GameObject and assign an import tag to it."),
                                figmaConverterUnity.mainSettings.UseCustomPrefabs);

                            /*  figmaConverterUnity.mainSettings.CreateFrameScripts = gui.Toggle(new GUIContent("Create Frame Scripts", "Automatically generates scripts and adds them to imported frames. For example 'MainWindow.cs'"),
                                  figmaConverterUnity.mainSettings.CreateFrameScripts); */

#if I2LOC_EXISTS
                            figmaConverterUnity.mainSettings.UseI2Localization = gui.Toggle(new GUIContent("Use I2Localization", "Whether to add 'I2Localize' script to 'Unity.UI.Text' or 'TextMeshPro' component."),
                                figmaConverterUnity.mainSettings.UseI2Localization);
#endif

                            /*figmaConverterUnity.mainSettings.IncludeTagInGameObjectName = gui.Toggle(new GUIContent("Include Tag In GameObject Name", "At the beginning of the name of your game objects there will be a tag such as 'btn - Menu', where tag is 'btn'."),
                                figmaConverterUnity.mainSettings.IncludeTagInGameObjectName);*/

                            figmaConverterUnity.mainSettings.ImportVectors = gui.Toggle(new GUIContent("Import Vectors", "If enabled, the asset will import all of your layout's individual vectors. Not recommended."),
                                figmaConverterUnity.mainSettings.ImportVectors);

                            figmaConverterUnity.mainSettings.ReDownloadSprites = gui.Toggle(new GUIContent("Re-download Images", "If enabled, the downloadable images will replace those that were downloaded earlier when old downloading."),
                                figmaConverterUnity.mainSettings.ReDownloadSprites);

                            figmaConverterUnity.mainSettings.HTTPS = gui.Toggle(new GUIContent("HTTPS", "You can disable HTTPS if you have SSL error when downloading project."),
                                figmaConverterUnity.mainSettings.HTTPS);

                            figmaConverterUnity.mainSettings.DebugMode = gui.Toggle(new GUIContent("Debug Mode", "Feature for developers. Allows to save the received response from Figma in json format."),
                                figmaConverterUnity.mainSettings.DebugMode);

                            GUILayout.Space(gui.NORMAL_SPACE);
                        }
                    });

                    if (figmaConverterUnity.mainSettings.TextComponent == TextComponent.Standard)
                    {
                        gui.DrawGroup(new Group
                        {
                            GroupType = GroupType.Hamburger,
                            Label = "STANDARD TEXT SETTINGS",
                            Fold = Fold.TextSettings,
                            Body = () =>
                            {
                                GUILayout.Space(gui.NORMAL_SPACE);

                                figmaConverterUnity.defaultTextSettings.BestFit = gui.Toggle(new GUIContent("Best Fit", "Should Unity ignore the size properties and simply try to fit the text to the control's rectangle?"),
                                    figmaConverterUnity.defaultTextSettings.BestFit);

                                figmaConverterUnity.defaultTextSettings.FontLineSpacing = gui.FloatField(new GUIContent("Line Spacing", "The vertical separation between lines of text."),
                                    figmaConverterUnity.defaultTextSettings.FontLineSpacing);

                                figmaConverterUnity.defaultTextSettings.HorizontalWrapMode = gui.EnumField(new GUIContent("Horizontal Overflow", "The method used to handle the situation where the text is too wide to fit in the rectangle. The options are Wrap and Overflow."),
                                    figmaConverterUnity.defaultTextSettings.HorizontalWrapMode);

                                figmaConverterUnity.defaultTextSettings.VerticalWrapMode = gui.EnumField(new GUIContent("Vertical Overflow", "The method used to handle the situation where wrapped text is too tall to fit in the rectangle. The options are Truncate and Overflow."),
                                    figmaConverterUnity.defaultTextSettings.VerticalWrapMode);

                                GUILayout.Space(gui.NORMAL_SPACE);
                            }
                        });
                    }
#if TMPRO_EXISTS
                    else if (figmaConverterUnity.mainSettings.TextComponent == TextComponent.TextMeshPro)
                    {

                        gui.DrawGroup(new Group
                        {
                            GroupType = GroupType.Hamburger,
                            Label = "TEXTMESHPRO SETTINGS",
                            Fold = Fold.TextMeshProSettings,
                            Body = () =>
                            {
                                GUILayout.Space(gui.NORMAL_SPACE);

                                figmaConverterUnity.textMeshProSettings.AutoSize = gui.Toggle(new GUIContent("Auto Size", "Auto sizes the text to fit the available space."),
                                    figmaConverterUnity.textMeshProSettings.AutoSize);

                                figmaConverterUnity.textMeshProSettings.OverrideTags = gui.Toggle(new GUIContent("Override Tags", "Whether the color settings override the <color> tag."),
                                    figmaConverterUnity.textMeshProSettings.OverrideTags);

                                figmaConverterUnity.textMeshProSettings.Wrapping = gui.Toggle(new GUIContent("Wrapping", "Wraps text to the next line when reaching the edge of the container."),
                                    figmaConverterUnity.textMeshProSettings.Wrapping);

                                figmaConverterUnity.textMeshProSettings.RichText = gui.Toggle(new GUIContent("Rich Text", "Enables the use of rich text tags such as <color> and <font>."),
                                    figmaConverterUnity.textMeshProSettings.RichText);

                                figmaConverterUnity.textMeshProSettings.RaycastTarget = gui.Toggle(new GUIContent("Raycast Target", "Whether the text blocks raycasts from the Graphic Raycaster."),
                                    figmaConverterUnity.textMeshProSettings.RaycastTarget);

                                figmaConverterUnity.textMeshProSettings.ParseEscapeCharacters = gui.Toggle(new GUIContent("Parse Escape Characters", "Whether to display strings such as \"\\n\" as is or replace them by the character they represent."),
                                    figmaConverterUnity.textMeshProSettings.ParseEscapeCharacters);

                                figmaConverterUnity.textMeshProSettings.VisibleDescender = gui.Toggle(new GUIContent("Visible Descender", "Compute descender values from visible characters only. Used to adjust layout behavior when hiding and revealing characters dynamically."),
                                    figmaConverterUnity.textMeshProSettings.VisibleDescender);

                                figmaConverterUnity.textMeshProSettings.Kerning = gui.Toggle(new GUIContent("Kerning", "Enables character specific spacing between pairs of characters."),
                                    figmaConverterUnity.textMeshProSettings.Kerning);

                                figmaConverterUnity.textMeshProSettings.ExtraPadding = gui.Toggle(new GUIContent("Extra Padding", "Adds some padding between the characters and the edge of the text mesh. Can reduce graphical errors when displaying small text."),
                                    figmaConverterUnity.textMeshProSettings.ExtraPadding);

                                figmaConverterUnity.textMeshProSettings.Overflow = gui.EnumField(new GUIContent("Overflow", "How to display text which goes past the edge of the container."),
                                    figmaConverterUnity.textMeshProSettings.Overflow);

                                figmaConverterUnity.textMeshProSettings.HorizontalMapping = gui.EnumField(new GUIContent("Horizontal Mapping", "Horizontal UV mapping when using a shader with a texture face option."),
                                    figmaConverterUnity.textMeshProSettings.HorizontalMapping);

                                figmaConverterUnity.textMeshProSettings.VerticalMapping = gui.EnumField(new GUIContent("Vertical Mapping", "Vertical UV mapping when using a shader with a texture face option."),
                                    figmaConverterUnity.textMeshProSettings.VerticalMapping);

                                figmaConverterUnity.textMeshProSettings.GeometrySorting = gui.EnumField(new GUIContent("Geometry Sorting", "The order in which text geometry is sorted. Used to adjust the way overlapping characters are displayed."),
                                    figmaConverterUnity.textMeshProSettings.GeometrySorting);

                                GUILayout.Space(gui.NORMAL_SPACE);
                            }
                        });
                    }
#endif

#endif
                    gui.DrawGroup(new Group
                    {
                        GroupType = GroupType.Hamburger,
                        Label = "SCRIPTING DEFINE SYMBOLS",
                        Fold = Fold.Defines,
                        Body = () =>
                        {
                            GUILayout.Space(gui.NORMAL_SPACE);
                            ToggleForJsonNet();
                            ToggleForTrueShadow();
                            ToggleForTextMeshPro();
                            ToggleForMPImage();
                            ToggleForProceduralUIImage();
                            ToggleForI2Localization();
                        }
                    });

#if JSON_NET_EXISTS


                    if (figmaConverterUnity.getPageFramesButtonVisible && figmaConverterUnity.pagesForSelect.Count() > 0)
                    {
                        int pagesCount = figmaConverterUnity.pagesForSelect.Count();

                        gui.DrawGroup(new Group
                        {
                            GroupType = GroupType.Hamburger,
                            Label = $"SELECT PAGE TO PARSE ({pagesCount})",
                            Fold = Fold.SelectPage,
                            Body = () =>
                            {
                                GUILayout.Space(gui.NORMAL_SPACE);
                                for (int i = 0; i < figmaConverterUnity.pagesForSelect.Count(); i++)
                                {
                                    bool selected = gui.Toggle(new GUIContent(figmaConverterUnity.pagesForSelect[i].Name, figmaConverterUnity.pagesForSelect[i].Name), figmaConverterUnity.pagesForSelect[i].Selected);
                                    figmaConverterUnity.pagesForSelect[i].Selected = selected;
                                    if (figmaConverterUnity.pagesForSelect[i].Selected == true)
                                    {
                                        selectedPageId = figmaConverterUnity.pagesForSelect[i].Id;
                                    }

                                    for (int j = 0; j < figmaConverterUnity.pagesForSelect.Count(); j++)
                                    {
                                        if (figmaConverterUnity.pagesForSelect[j].Id != selectedPageId)
                                        {
                                            figmaConverterUnity.pagesForSelect[j].Selected = false;
                                        }
                                    }
                                }

                                GUILayout.Space(gui.NORMAL_SPACE);
                            }
                        });
                    }


                    if (figmaConverterUnity.downloadFramesButtonVisible && figmaConverterUnity.framesToDownload.Count() > 0)
                    {
                        int framesCount = figmaConverterUnity.framesToDownload.Count();
                        int framesToDownloadCount = figmaConverterUnity.framesToDownload.Where(x => x.Selected == true).Count();

                        gui.DrawGroup(new Group
                        {
                            GroupType = GroupType.Hamburger,
                            Label = $"FRAMES TO DOWNLOAD ({framesToDownloadCount}/{framesCount})",
                            Fold = Fold.FramesToDownload,
                            Body = () =>
                            {
                                GUILayout.Space(gui.NORMAL_SPACE);
                                selectDeselectToggleSelected = EditorGUILayout.Toggle(selectDeselectToggleLabel, _selectDeselectToggleTemp);
                                if (_selectDeselectToggleTemp != selectDeselectToggleSelected)
                                {
                                    if (selectDeselectToggleSelected)
                                    {
                                        selectDeselectToggleLabel = new GUIContent("SELECT ALL");
                                    }
                                    else
                                    {
                                        selectDeselectToggleLabel = new GUIContent("DESELECT ALL");
                                    }

                                    for (int i = 0; i < figmaConverterUnity.framesToDownload.Count(); i++)
                                    {
                                        figmaConverterUnity.framesToDownload[i].Selected = selectDeselectToggleSelected;
                                    }
                                }
                                _selectDeselectToggleTemp = selectDeselectToggleSelected;

                                GUILayout.Space(gui.NORMAL_SPACE);

                                for (int i = 0; i < figmaConverterUnity.framesToDownload.Count(); i++)
                                {
                                    bool selected = gui.Toggle(new GUIContent(figmaConverterUnity.framesToDownload[i].Name, figmaConverterUnity.framesToDownload[i].Name), figmaConverterUnity.framesToDownload[i].Selected);
                                    figmaConverterUnity.framesToDownload[i].Selected = selected;
                                }

                                GUILayout.Space(gui.NORMAL_SPACE);
                            }
                        });
                    }

                    GUILayout.Space(gui.NORMAL_SPACE);


                    if (figmaConverterUnity.mainSettings.TextComponent == TextComponent.Standard)
                    {
                        gui.SerializedPropertyField(nameof(figmaConverterUnity.fonts), fcuSerializedObject);
                    }
#if TMPRO_EXISTS
                    else if (figmaConverterUnity.mainSettings.TextComponent == TextComponent.TextMeshPro)
                    {
                        gui.SerializedPropertyField(nameof(figmaConverterUnity.textMeshProFonts), fcuSerializedObject);
                    }
#endif
                    if (figmaConverterUnity.mainSettings.UseCustomPrefabs)
                    {
                        gui.SerializedPropertyField(nameof(figmaConverterUnity.customPrefabs), fcuSerializedObject);
                    }

                    GUILayout.Space(gui.BIG_SPACE);

                    if (gui.CenteredButton(new GUIContent("Auth With Browser")))
                    {
                        FCU_Mehtods.Auth(figmaConverterUnity);
                    }

                    if (Checkers.IsValidApiKey() == false)
                    {
                        Footer();
                        return;
                    }

                    GUILayout.Space(gui.NORMAL_SPACE);
                    if (gui.CenteredButton(new GUIContent("Download Project")))
                    {
                        FCU_Mehtods.DownloadProject(figmaConverterUnity);
                    }

                    if (figmaConverterUnity.getPageFramesButtonVisible && figmaConverterUnity.pagesForSelect.Count() > 0)
                    {
                        GUILayout.Space(gui.NORMAL_SPACE);
                        if (gui.CenteredButton(new GUIContent("Get Frames From Page")))
                        {
                            FCU_Mehtods.GetFramesFromSelectedPage(figmaConverterUnity);
                        }
                    }

                    if (figmaConverterUnity.downloadFramesButtonVisible && figmaConverterUnity.framesToDownload.Count() > 0)
                    {
                        GUILayout.Space(gui.NORMAL_SPACE);
                        if (gui.CenteredButton(new GUIContent("Download Frames")))
                        {
                            FCU_Mehtods.DownloadSelectedFrames(figmaConverterUnity);
                        }
                    }

                    Footer();
#endif
                }
            });

#if !JSON_NET_EXISTS
            throw new MissingAssetException("JSON.NET");
#endif
        }
        internal void Footer()
        {
            GUILayout.Space(gui.BIG_SPACE);
            gui.CenteredLinkLabel($"https://{Constants.TG_LINK}", new GUIContent($"   made by {Constants.PUBLISHER}\n{Constants.TG_LINK}"));
        }
        private void ToggleForJsonNet()
        {
            string jsonDefine = Constants.JSONNET_DEFINE;

            defineJsonNet1 = gui.Toggle(new GUIContent(Constants.JSONNET_DEFINE, ""), defineJsonNet1);
            if (!defineJsonNet2 && defineJsonNet1)
            {
                HandleDefine.AddDefine(jsonDefine);
                defineJsonNet2 = true;
            }
            else if (defineJsonNet2 && !defineJsonNet1)
            {
                HandleDefine.RemoveDefine(jsonDefine);
                defineJsonNet2 = false;
            }
        }
        private void ToggleForTrueShadow()
        {
            string trueShadowDefine = Constants.TRUESHADOW_DEFINE;

            defineTrueShadow1 = gui.Toggle(new GUIContent(trueShadowDefine, "Preprocessor directive to activate 'Tai's TrueShadow' asset (if imported)."), defineTrueShadow1);
            if (!defineTrueShadow2 && defineTrueShadow1)
            {
                HandleDefine.AddDefine(trueShadowDefine);
                defineTrueShadow2 = true;
            }
            else if (defineTrueShadow2 && !defineTrueShadow1)
            {
                HandleDefine.RemoveDefine(trueShadowDefine);
                defineTrueShadow2 = false;
            }
        }
        private void ToggleForTextMeshPro()
        {
            string textmeshproDefine = Constants.TEXTMESHPRO_DEFINE;

            defineTextMeshPro1 = gui.Toggle(new GUIContent(textmeshproDefine, "Preprocessor directive to activate 'TextMeshPro' asset (if imported)."), defineTextMeshPro1);
            if (!defineTextMeshPro2 && defineTextMeshPro1)
            {
                HandleDefine.AddDefine(textmeshproDefine);
                defineTextMeshPro2 = true;
            }
            else if (defineTextMeshPro2 && !defineTextMeshPro1)
            {
                HandleDefine.RemoveDefine(textmeshproDefine);
                defineTextMeshPro2 = false;
            }
        }
        private void ToggleForMPImage()
        {
            string mpimageDefine = Constants.MPUIKIT_DEFINE;

            defineMPImage1 = gui.Toggle(new GUIContent(mpimageDefine, "Preprocessor directive to activate 'Modern Procedural UI Kit' asset (if imported)."), defineMPImage1);
            if (!defineMPImage2 && defineMPImage1)
            {
                HandleDefine.AddDefine(mpimageDefine);
                defineMPImage2 = true;
            }
            else if (defineMPImage2 && !defineMPImage1)
            {
                HandleDefine.RemoveDefine(mpimageDefine);
                defineMPImage2 = false;
            }
        }
        private void ToggleForProceduralUIImage()
        {
            string proceduraluiimageDefine = Constants.PUI_DEFINE;

            defineProceduralUIImage1 = gui.Toggle(new GUIContent(proceduraluiimageDefine, "Preprocessor directive to activate 'Procedural UI Image' asset (if imported)."), defineProceduralUIImage1);
            if (!defineProceduralUIImage2 && defineProceduralUIImage1)
            {
                HandleDefine.AddDefine(proceduraluiimageDefine);
                defineProceduralUIImage2 = true;
            }
            else if (defineProceduralUIImage2 && !defineProceduralUIImage1)
            {
                HandleDefine.RemoveDefine(proceduraluiimageDefine);
                defineProceduralUIImage2 = false;
            }
        }
        private void ToggleForI2Localization()
        {
            string i2lDefine = Constants.I2LOC_DEFINE;

            defineI2Loc1 = gui.Toggle(new GUIContent(i2lDefine, "Preprocessor directive to activate 'I2Localization' asset (if imported)."), defineI2Loc1);
            if (!defineI2Loc2 && defineI2Loc1)
            {
                HandleDefine.AddDefine(i2lDefine);
                defineI2Loc2 = true;
            }
            else if (defineI2Loc2 && !defineI2Loc1)
            {
                HandleDefine.RemoveDefine(i2lDefine);
                defineI2Loc2 = false;
            }
        }
    }
}