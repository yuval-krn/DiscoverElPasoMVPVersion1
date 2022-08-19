#if UNITY_EDITOR && JSON_NET_EXISTS
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using System.Linq;
using UnityEditor;
using UnityEngine;
#if PUI_EXISTS
using UnityEngine.UI.ProceduralImage;
#endif

namespace DA_Assets.FCU.Plugins
{
    static class ProceduralImagePlugin
    {
#if PUI_EXISTS
        public static GameObject CreateProceduralUIImage(FObject fobject)
        {
            ProceduralImage pImage = fobject.GameObj.AddComponent<ProceduralImage>();
            FreeModifier freeModifier = fobject.GameObj.AddComponent<FreeModifier>();

            fobject.SetImgTypeSprite();

            if (ImportConditions.IsDownloadableImage(fobject))
            {
                Sprite sprite = (Sprite)AssetDatabase.LoadAssetAtPath(fobject.AssetPath, typeof(Sprite));
                pImage.sprite = sprite;
            }
            else
            {
                if (fobject.Fills.Any(x => x.Type == "IMAGE") == false)
                {
                    AssetDatabase.DeleteAsset(fobject.AssetPath);
                }
                else
                {
                    Sprite sprite = (Sprite)AssetDatabase.LoadAssetAtPath(fobject.AssetPath, typeof(Sprite));
                    pImage.sprite = sprite;
                }

                foreach (Fill fill in fobject.Fills)
                {
                    if (fill.Type == "SOLID")
                    {
                        if (fill.Opacity != null)
                        {
                            Color _color = fill.Color;
                            _color.a = (float)fill.Opacity;
                            pImage.color = _color;
                        }
                        else
                        {
                            pImage.color = fill.Color;
                        }
                    }
                }
            }

            if (fobject.RectangleCornerRadius != null)
            {
                freeModifier.Radius = new Vector4
                {
                    x = fobject.RectangleCornerRadius[0],
                    y = fobject.RectangleCornerRadius[1],
                    z = fobject.RectangleCornerRadius[2],
                    w = fobject.RectangleCornerRadius[3]
                };
            }
            else if (fobject.CornerRadius != 0)
            {
                freeModifier.Radius = new Vector4
                {
                    x = fobject.CornerRadius,
                    y = fobject.CornerRadius,
                    z = fobject.CornerRadius,
                    w = fobject.CornerRadius,
                };
            }

            fobject.SetFigmaSize();

            if (fobject.NeedDeleteBackground())
            {
                pImage.DestroyImmediate();
                freeModifier.DestroyImmediate();
                return fobject.GameObj;
            }

            pImage.SetTrueShadow(fobject);

            return fobject.GameObj;
        }
#endif
    }
}
#endif
