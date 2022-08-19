#if UNITY_EDITOR
namespace DA_Assets.FCU
{
    public class Constants
    {
        public const string PRODUCT_VERSION = "1.3.0";
        public const string PRODUCT_NAME = "Figma Converter for Unity";
        public const string PUBLISHER = "D.A. Assets";
        public const string JSON_FILE_NAME = "response.json";
        public const string LOGS_FOLDER_NAME = "Logs";
        public const string LOCALIZATION_FILE_NAME = "localization.csv";
        public const string TG_LINK = "t.me/da_assets_publisher";

        public const string JSONNET_DEFINE = "JSON_NET_EXISTS";
        public const string TRUESHADOW_DEFINE = "TRUESHADOW_EXISTS";
        public const string TEXTMESHPRO_DEFINE = "TMPRO_EXISTS";
        public const string MPUIKIT_DEFINE = "MPUIKIT_EXISTS";
        public const string PUI_DEFINE = "PUI_EXISTS";
        public const string I2LOC_DEFINE = "I2LOC_EXISTS";

        public const string EVENT_SYSTEM_GAMEOBJECT_NAME = "EventSystem";
        public const string CANVAS_GAMEOBJECT_NAME = "Canvas {0}";
        public const string I2LOC_GAMEOBJECT_NAME = "script - Language Source";
        public const float PROBABILITY_MATCHING_TAGS = 0.8f;
        public const float PROBABILITY_MATCHING_FONS = 0.9f;
        public const int GAMEOBJECT_NAME_MAX_LENGHT = 32;
        public const string IMPORTED_FRAMES_PREFS_KEY = "importedFrames";

        public const string API_LINK = "https://api.figma.com/v1/files/{0}?geometry=paths";
        public const string CLIENT_ID = "LaB1ONuPoY7QCdfshDbQbT";
        public const string CLIENT_SECRET = "E9PblceydtAyE7Onhg5FHLmnvingDp";
        public const string REDIRECT_URI = "http://localhost:1923/";
        public const string AUTH_URL = "https://www.figma.com/api/oauth/token?client_id={0}&client_secret={1}&redirect_uri={2}&code={3}&grant_type=authorization_code";
        public const string OAUTH_URL = "https://www.figma.com/oauth?client_id={0}&redirect_uri={1}&scope=file_read&state={2}&response_type=code";
    }
}
#endif