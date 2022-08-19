#if UNITY_EDITOR && JSON_NET_EXISTS

using DA_Assets.FCU.Model;
using UnityEngine.UI;
#if TRUESHADOW_EXISTS
using LeTai.TrueShadow;
#endif

namespace DA_Assets.FCU.Plugins
{
    public static class TrueShadowPlugin
    {
        public static void SetTrueShadow(this MaskableGraphic element, FObject fobject)
        {
            foreach (Effect effect in fobject.Effects)
            {
                if (effect.Type.Contains("SHADOW"))
                {
                    if (FigmaConverterUnity.Instance.mainSettings.ShadowType == ShadowType.None)
                    {
                        continue;
                    }
#if TRUESHADOW_EXISTS
                    else if (FigmaConverterUnity.Instance.mainSettings.ShadowType == ShadowType.TrueShadow)
                    {

                        TrueShadow trueShadow = element.gameObject.AddComponent<TrueShadow>();
                        trueShadow.Size = effect.Radius;
                        trueShadow.Offset.Set(effect.Offset.x, effect.Offset.y);
                        trueShadow.Color = effect.Color;

                        if (effect.Type.Contains("DROP"))
                        {
                            trueShadow.Inset = false;
                        }
                        else
                        {
                            trueShadow.Inset = true;
                        }
                    }
#endif
                }
            }
        }
    }
}
#endif
