#if UNITY_EDITOR
using DA_Assets.FCU.Exceptions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
#if JSON_NET_EXISTS
using DA_Assets.FCU.Plugins;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
#if TMPRO_EXISTS
using TMPro;
#endif
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DA_Assets.FCU
{
    public static class CanvasDrawer
    {
#if JSON_NET_EXISTS
        private static List<FObject> buttons;
#if TMPRO_EXISTS
        private static List<TMP_InputField> tmInputFields;
#endif
        private static List<InputField> inputFields;
        public static void DrawToCanvas(List<FObject> fobjects)
        {
            buttons = new List<FObject>();

            switch (FigmaConverterUnity.Instance.mainSettings.TextComponent)
            {
                case TextComponent.Standard:
                    inputFields = new List<InputField>();
                    break;
#if TMPRO_EXISTS                
                case TextComponent.TextMeshPro:
                    tmInputFields = new List<TMP_InputField>();
                    break;
#endif
            }

            int objCount = 0;
            foreach (FObject fobject in fobjects)
            {
                try
                {
                    switch (fobject.FTag)
                    {
                        case FTag.Frame:
                            GameObject frameGO = InstantiateFrame(fobject);
                            int frameId = frameGO.GetInstanceID();
                            FCU_Prefs.AddFrameToCurrentImportList(FigmaConverterUnity.Instance.instanceInfo.Id, frameId);
                            break;
                        case FTag.Button:
                            InstantiateButton(fobject);
                            break;
                        case FTag.InputField:
                            InstantiateInputField(fobject);
                            break;
                        case FTag.Text:
                            InstantiateText(fobject);
                            break;
                        case FTag.Placeholder:
                            InstantiateText(fobject);
                            break;
                        case FTag.HorizontalLayoutGroup:
                        case FTag.VerticalLayoutGroup:
                            InstantiateHorizontalOrVerticalLayoutGroup(fobject, fobject.FTag);
                            break;
                        case FTag.GridLayoutGroup:
                            InstantiateGridLayoutGroup(fobject);
                            break;
                        default:
                            CustomPrefab customPrefab = FigmaConverterUnity.Instance.customPrefabs.FirstOrDefault(x => x.Tag == fobject.CustomTag);

                            if (customPrefab != default)
                            {
                                InstantiateCustomPrefab(fobject, customPrefab.Prefab);
                            }
                            else
                            {
                                InstantiateImage(fobject);
                            }
                            break;
                    }
                }
                catch
                {
                    Console.Log($"Fail to instantiate '{fobject.Name}'");
                }

                objCount++;
            }

            SetTargetGraphicForButtons();

            switch (FigmaConverterUnity.Instance.mainSettings.TextComponent)
            {
                case TextComponent.Standard:
                    SetTargetGraphicForInputFields();
                    inputFields.Clear();
                    break;
#if TMPRO_EXISTS                
                case TextComponent.TextMeshPro:
                    SetTargetGraphicForTMInputFields();
                    tmInputFields.Clear();
                    break;
#endif
            }

#if I2LOC_EXISTS
            if (FigmaConverterUnity.Instance.mainSettings.UseI2Localization)
            {
                InstantiateI2LocalizationSource();
            }
#endif
            buttons.Clear();


            Console.WriteLine(string.Format(Localization.DRAWED, objCount));
        }
        private static void InstantiateCustomPrefab(FObject fobject, GameObject prefab)
        {
            fobject.SetFigmaSize();
        }


        private static void InstantiateHorizontalOrVerticalLayoutGroup(FObject horVertGroup, FTag layoutGroup)
        {
            GameObject gameObject = InstantiateImage(horVertGroup);
            HorizontalOrVerticalLayoutGroup _horVertGroup;

            if (layoutGroup == FTag.HorizontalLayoutGroup)
            {
                _horVertGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
            }
            else
            {
                _horVertGroup = gameObject.AddComponent<VerticalLayoutGroup>();
            }

            _horVertGroup.spacing = horVertGroup.ItemSpacing;
            _horVertGroup.padding = new RectOffset
            {
                bottom = (int)Mathf.Round(horVertGroup.PaddingBottom),
                top = (int)Mathf.Round(horVertGroup.PaddingTop),
                left = (int)Mathf.Round(horVertGroup.PaddingLeft),
                right = (int)Mathf.Round(horVertGroup.PaddingRight)
            };

            _horVertGroup.childControlWidth = true;
            _horVertGroup.childControlHeight = true;
            _horVertGroup.childForceExpandWidth = true;
            _horVertGroup.childForceExpandHeight = true;
            _horVertGroup.childAlignment = horVertGroup.GetChildAligment();

            foreach (FObject fobject in horVertGroup.Children)
            {
                LayoutElement layoutElement = fobject.GameObj.AddComponent<LayoutElement>();

                layoutElement.preferredWidth = fobject.Size.x;
                layoutElement.preferredHeight = fobject.Size.y;
            }
        }

        private static void InstantiateGridLayoutGroup(FObject gridGroup)
        {
            GameObject gameObject = InstantiateImage(gridGroup);
            GridLayoutGroup _gridGroup = gameObject.AddComponent<GridLayoutGroup>();

            string[] nameParts = gridGroup.Name.Split(FigmaExtensions.GetTagSeparator());
            string[] spacingArray = nameParts[nameParts.Length - 1].Split("x");
            string[] cellSizeArray = nameParts[nameParts.Length - 2].Split("x");

            int spacingX = Convert.ToInt32(spacingArray[0]);
            int spacingY = Convert.ToInt32(spacingArray[1]);

            int cellSizeX = Convert.ToInt32(cellSizeArray[0]);
            int cellSizeY = Convert.ToInt32(cellSizeArray[1]);

            _gridGroup.spacing = new Vector2(spacingX, spacingY);
            _gridGroup.cellSize = new Vector2(cellSizeX, cellSizeY);

            _gridGroup.childAlignment = TextAnchor.MiddleCenter;
        }

        private static void InstantiateInputField(FObject inputField)
        {
            GameObject gameObject = InstantiateImage(inputField);

            switch (FigmaConverterUnity.Instance.mainSettings.TextComponent)
            {
                case TextComponent.Standard:
                    InputField _inputField = gameObject.AddComponent<InputField>();
                    inputFields.Add(_inputField);
                    break;
#if TMPRO_EXISTS                
                case TextComponent.TextMeshPro:
                    TMP_InputField _tmInputField = gameObject.AddComponent<TMP_InputField>();
                    tmInputFields.Add(_tmInputField);
                    break;
#endif

            }
        }
        private static void SetTargetGraphicForInputFields()
        {
            foreach (InputField inputField in inputFields)
            {
                foreach (Transform child in inputField.GetComponentsInChildren<Transform>())
                {
                    FObject _tempFobject = new FObject
                    {
                        Name = child.name.ToLower(),
                        GameObj = child.gameObject
                    };

                    _tempFobject.FTag = _tempFobject.GetFigmaType();

                    if (_tempFobject.FTag != FTag.Placeholder)
                    {
                        if (child.TryGetComponent(out Text _text))
                        {
                            _tempFobject.FTag = FTag.Text;
                        }
                    }

                    _tempFobject.CustomTag = _tempFobject.GetCustomTag();

                    if (_tempFobject.FTag == FTag.Background)
                    {
                        if (child.TryGetComponent(out Image image))
                        {
                            inputField.targetGraphic = image;
                        }
                    }
                    else if (_tempFobject.FTag == FTag.Text)
                    {
                        if (child.TryGetComponent(out Text text))
                        {
                            inputField.textComponent = text;
                        }
                    }
                    else if (_tempFobject.FTag == FTag.Placeholder)
                    {
                        if (child.TryGetComponent(out Text text))
                        {
                            inputField.placeholder = text;
                        }
                    }
                }
            }
        }
#if TMPRO_EXISTS                
        private static void SetTargetGraphicForTMInputFields()
        {
            foreach (TMP_InputField inputField in tmInputFields)
            {
                foreach (Transform child in inputField.GetComponentsInChildren<Transform>())
                {
                    FObject _tempFobject = new FObject
                    {
                        Name = child.name.ToLower(),
                        GameObj = child.gameObject
                    };

                    _tempFobject.FTag = _tempFobject.GetFigmaType();

                    if (_tempFobject.FTag != FTag.Placeholder)
                    {
                        if (child.TryGetComponent(out TextMeshProUGUI _text))
                        {
                            _tempFobject.FTag = FTag.Text;
                        }
                    }

                    _tempFobject.CustomTag = _tempFobject.GetCustomTag();

                    if (_tempFobject.FTag == FTag.Background)
                    {
                        if (child.TryGetComponent(out Image image))
                        {
                            inputField.targetGraphic = image;
                        }
                    }
                    else if (_tempFobject.FTag == FTag.Text)
                    {
                        if (child.TryGetComponent(out TextMeshProUGUI tmText))
                        {
                            inputField.textComponent = tmText;
                        }
                    }
                    else if (_tempFobject.FTag == FTag.Placeholder)
                    {
                        if (child.TryGetComponent(out TextMeshProUGUI tmText))
                        {
                            inputField.placeholder = tmText;
                        }
                    }
                }
            }
        }
#endif
        private static void InstantiateText(FObject fobject)
        {
            if (FigmaConverterUnity.Instance.mainSettings.TextComponent == TextComponent.Standard)
            {
                InstantiateDefaultText(fobject);
            }
#if TMPRO_EXISTS
            else if (FigmaConverterUnity.Instance.mainSettings.TextComponent == TextComponent.TextMeshPro)
            {
                InstantiateTextMeshPro(fobject);
            }
#endif
        }
        private static void InstantiateDefaultText(FObject fobject)
        {
            if (fobject.GameObj == null)
            {
                return;
            }

            Text _text = fobject.GameObj.AddComponent<Text>();
#if I2LOC_EXISTS
            if (FigmaConverterUnity.Instance.mainSettings.UseI2Localization)
            {
                fobject.AddI2Localize();
            }
#endif

            fobject.SetFigmaSize();
            _text.SetDefaultTextStyle(fobject);
        }
#if TMPRO_EXISTS
        private static void InstantiateTextMeshPro(FObject fobject)
        {
            TextMeshProUGUI _textMesh = fobject.GameObj.AddComponent<TextMeshProUGUI>();
#if I2LOC_EXISTS
            if (FigmaConverterUnity.Instance.mainSettings.UseI2Localization)
            {
                fobject.AddI2Localize();
            }
#endif
            fobject.SetFigmaSize();
            _textMesh.SetTextMeshProStyle(fobject);
        }
#endif
        private static GameObject InstantiateFrame(FObject frame)
        {
            Image img = frame.GameObj.AddComponent<Image>();

            frame.SetFigmaSize();

            img.rectTransform.SetSmartAnchorPreset(AnchorType.StretchAll);
            img.rectTransform.offsetMin = new Vector2(0, 0);
            img.rectTransform.offsetMax = new Vector2(0, 0);
            img.rectTransform.localScale = Vector3.one;

            img.DestroyImmediate();

            return frame.GameObj;
        }
        private static GameObject InstantiateImage(FObject fobject)
        {
            if (fobject.GameObj == null)
            {
                //Debug.Log("GameObj is null");
                return null;
            }

            switch (FigmaConverterUnity.Instance.mainSettings.ImageComponent)
            {
#if MPUIKIT_EXISTS
                case ImageComponent.MPImage:
                    return MPImagePlugin.CreateMPImage(fobject);
#endif
#if PUI_EXISTS
                case ImageComponent.ProceduralImage:
                    return ProceduralImagePlugin.CreateProceduralUIImage(fobject);
#endif
                default:
                    return CreateImage(fobject);
            }
        }
        private static GameObject CreateImage(FObject fobject)
        {
            Image _img = fobject.GameObj.AddComponent<Image>();

            bool downloadable = ImportConditions.IsDownloadableImage(fobject);

            Console.Log($"CreateImage | IsDownloadableImage | {fobject.Name} | {downloadable}");

            if (downloadable)
            {
                try
                {
                    fobject.SetImgTypeSprite();

                    Sprite sprite = (Sprite)AssetDatabase.LoadAssetAtPath(fobject.AssetPath, typeof(Sprite));
                    _img.sprite = sprite;
                }
                catch
                {
                    _img.sprite = null;
                    _img.color = Color.white;

                    throw new MissingSpriteException(fobject.Name);
                }
            }
            else
            {
                bool deleted = AssetDatabase.DeleteAsset(fobject.AssetPath);

                if (deleted == false)
                {
                    //Console.Warning($"Cannot delete '{fobject.GetFOBjectHierarchy()}'.\n'{fobject.AssetPath}' is not a valid path.");
                }

                _img.color = fobject.Fills[0].Color;
            }

            fobject.SetFigmaSize();

            if (fobject.NeedDeleteBackground())
            {
                _img.DestroyImmediate();
                return fobject.GameObj;
            }

            _img.SetTrueShadow(fobject);

            return fobject.GameObj;
        }

        private static void InstantiateButton(FObject button)
        {
            GameObject gameObject = InstantiateImage(button);

            if (button.Children == null || button.Children == null)
            {
                return;
            }

            button.GameObj = gameObject;

            int textCount = button.Children.Where(x => x.FTag == FTag.Text).Count();
            int bgCount = button.Children.Where(x => x.FTag == FTag.Background).Count();

            //Default button always has one text and one background. BetterButton one may have several.
            if (bgCount > 1 && textCount > 1)
            {
                Console.Log($"InstantiateButton | '{button.Name}' is a BetterButton");
                BetterButton _btn = gameObject.AddComponent<BetterButton>();
                button.ButtonType = ButtonType.Better;

                foreach (FObject child in button.Children)
                {
                    if (child.FTag == FTag.Text)
                    {
                        if (child.Name.ToLower().Contains(ButtonStates.Default.GetDescription()))
                        {
                            _btn.textDefaultColor = child.GetTextColor();
                        }
                        else if (child.Name.ToLower().Contains(ButtonStates.Hover.GetDescription()))
                        {
                            _btn.textHoverColor = child.GetTextColor();
                            child.GameObj.DestroyImmediate();
                            child.GameObj = null;
                        }
                        else if (child.Name.ToLower().Contains(ButtonStates.Pressed.GetDescription()))
                        {
                            _btn.textPressedColor = child.GetTextColor();
                            child.GameObj.DestroyImmediate();
                            child.GameObj = null;
                        }
                        else if (child.Name.ToLower().Contains(ButtonStates.Selected.GetDescription()))
                        {
                            _btn.textSelectedColor = child.GetTextColor();
                            child.GameObj.DestroyImmediate();
                            child.GameObj = null;
                        }
                        else if (child.Name.ToLower().Contains(ButtonStates.Disabled.GetDescription()))
                        {
                            _btn.textDisabledColor = child.GetTextColor();
                            child.GameObj.DestroyImmediate();
                            child.GameObj = null;
                        }
                    }
                    else if (child.FTag == FTag.Background)
                    {
                        bgCount++;
                        child.SetImgTypeSprite();
                        SpriteState spriteState = new SpriteState();
                        spriteState = _btn.spriteState;
                        Sprite _sprite = (Sprite)AssetDatabase.LoadAssetAtPath(child.AssetPath, typeof(Sprite));

                        if (child.Name.ToLower().Contains(ButtonStates.Hover.GetDescription()))
                        {
                            spriteState.highlightedSprite = _sprite;
                            child.GameObj.DestroyImmediate();
                            child.GameObj = null;
                        }
                        else if (child.Name.ToLower().Contains(ButtonStates.Pressed.GetDescription()))
                        {
                            spriteState.pressedSprite = _sprite;
                            child.GameObj.DestroyImmediate();
                            child.GameObj = null;
                        }
                        else if (child.Name.ToLower().Contains(ButtonStates.Selected.GetDescription()))
                        {
                            spriteState.selectedSprite = _sprite;
                            child.GameObj.DestroyImmediate();
                            child.GameObj = null;
                        }
                        else if (child.Name.ToLower().Contains(ButtonStates.Disabled.GetDescription()))
                        {
                            spriteState.disabledSprite = _sprite;
                            child.GameObj.DestroyImmediate();
                            child.GameObj = null;
                        }

                        _btn.spriteState = spriteState;
                    }
                }

                _btn.transition = Selectable.Transition.SpriteSwap;
            }
            else
            {
                Console.Log($"InstantiateButton | '{button.Name}' is a Button");
                Button _btn = gameObject.AddComponent<Button>();
                button.ButtonType = ButtonType.Default;
            }

            buttons.Add(button);
        }
        private static void SetTargetGraphicForButtons()
        {
            foreach (FObject button in buttons)
            {
                Console.Log($"SetTargetGraphicForButtons | {button.Name}");

                int imgCount = button.GameObj.GetComponentsInChildren<Image>().Count();

                foreach (FObject child in button.Children)
                {
                    if (child.GameObj == null)
                    {
                        continue;
                    }

                    if (child.FTag == FTag.Background)
                    {
                        Image _img = child.GameObj.GetComponent<Image>();

                        if (button.ButtonType == ButtonType.Default)
                        {
                            button.GameObj.GetComponent<Button>().targetGraphic = _img;
                        }
                        else if (button.ButtonType == ButtonType.Better)
                        {
                            if (child.Name.ToLower().Contains(ButtonStates.Default.GetDescription()))
                            {
                                button.GameObj.GetComponent<BetterButton>().targetGraphic = _img;
                            }
                        }
                    }
                    else
                    {
                        switch (FigmaConverterUnity.Instance.mainSettings.TextComponent)
                        {
                            case TextComponent.Standard:
                                if (child.FTag == FTag.Text)
                                {
                                    Text _text = child.GameObj.GetComponent<Text>();

                                    if (button.ButtonType == ButtonType.Better)
                                    {
                                        if (child.Name.ToLower().Contains(ButtonStates.Default.GetDescription()))
                                        {
                                            button.GameObj.GetComponent<BetterButton>().buttonText = _text;
                                        }
                                    }
                                }
                                break;
#if TMPRO_EXISTS
                            case TextComponent.TextMeshPro:
                                if (child.GameObj.TryGetComponent(out TextMeshProUGUI tmText))
                                {
                                    if (tmText.name.ToLower().Contains(ButtonStates.Default.GetDescription()))
                                    {
                                        button.GameObj.GetComponent<BetterButton>().buttonText = tmText;
                                        continue;
                                    }
                                }
                                break;
#endif
                        }
                    }
                    
                }
            }
        }

#if I2LOC_EXISTS
        public static void InstantiateI2LocalizationSource()
        {
            I2LocalizationPlugin.InstantiateI2LocalizationSource();
        }
#endif
#endif
        public static void InstantiateCanvas(Vector2 refRes)
        {
            GameObject _gameObject = CreateEmptyGameObj();
            _gameObject.AddComponent<FigmaConverterUnity>();
            _gameObject.name = string.Format(Constants.CANVAS_GAMEOBJECT_NAME, _gameObject.GetInstanceID().ToString().Replace("-", ""));

            Canvas _canvas = _gameObject.AddComponent<Canvas>();
            Canvas[] canvases = UnityEngine.Object.FindObjectsOfType<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            if (canvases != null && canvases.Length > 1)
            {
                int sortingOrder = canvases.Select(x => x.sortingOrder).Max();
                _canvas.sortingOrder = sortingOrder + 1;
            }

            CanvasScaler _canvasScaler = _gameObject.AddComponent<CanvasScaler>();
            _canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _canvasScaler.referenceResolution = refRes;
            _canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            _canvasScaler.matchWidthOrHeight = 1f;
            _canvasScaler.referencePixelsPerUnit = 100f;

            _gameObject.AddComponent<GraphicRaycaster>();
        }
        public static void TryInstantiateEventSystem()
        {
            EventSystem[] findedES = UnityEngine.Object.FindObjectsOfType<EventSystem>();

            if (findedES.Length == 0)
            {
                GameObject _gameObject = CreateEmptyGameObj();
                _gameObject.AddComponent<EventSystem>();
                _gameObject.AddComponent<StandaloneInputModule>();
                _gameObject.name = Constants.EVENT_SYSTEM_GAMEOBJECT_NAME;
            }
        }
        public static GameObject CreateEmptyGameObj()
        {
            GameObject _temp = new GameObject();
            GameObject gameObj = UnityEngine.Object.Instantiate(_temp);
            UnityEngine.Object.DestroyImmediate(_temp);
            return gameObj;
        }
    }
}
#endif