#if UNITY_EDITOR
namespace DA_Assets.FCU.Exceptions
{
    public class MissingAssetException : FigmaException
    {
        public MissingAssetException(string assetName)
            : base($"Asset '{assetName.TextBold()}' is missing. You can download it from Package Manager, or any other safe source. After you import the asset into the project, enable it in 'SCRIPTING DEFINE SYMBOLS'")
        {

        }
    }
}
#endif