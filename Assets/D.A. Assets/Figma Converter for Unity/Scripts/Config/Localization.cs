#if UNITY_EDITOR

namespace DA_Assets.FCU
{
    public class Localization
    {
        public const string PROJECT_DOWNLOADED = "The project has been downloaded. Select the page you want to get frames from.";
        public const string PROJECT_NOT_FOUND = "Either this file doesn’t exist or you don’t have permission to view it. Ask the file owner to verify the link and/or update permissions. If you are using a team project, read the 'Teamwork' section in 'Manual for designers.pdf'.";
        public const string FRAMES_FINDED = "Finded {0} frames in {1}. Download them?";
        public const string FRAMES_NOT_FINDED = "No frames found. Try check the selected page and the tags of its components are correct.";
        public const string SPRITE_NOT_FOUND = "Sprite not found. Skip.";
        public const string AUTH_COMPLETE = "Auth complete!";
        public const string STARTING_PROJECT_DOWNLOAD = "Starting project download...";
        public const string DRAWED = "Drawed {0} components";
        public const string PARSED = "Parsed {0} childs of {1}";
        public const string OPEN_AUTH_PAGE = "Opening authorization page in the web browser...";
        public const string TRY_GET_API_KEY = "Trying to get api key...";
        public const string START_ADDING_LINKS = "Start adding sprite-links";
        public const string GETTING_LINKS = "Getting {0} of {1} sprite-links...";
        public const string LINKS_ADDED = "Sprite-links has been added.";
        public const string START_SPRITES_DOWNLOAD = "Starting download sprites for UI components...";
        public const string DRAW_COMPONENTS = "{0} components are ready to be draw. Draw?";
        public const string IMPORT_COMPLETE = "Import is complete!";
        public const string LAST_IMPORTED_FRAMES_DESTROYED = "The last imported frames has been destroyed.\nTotal destroyed {0} frames.";
        public const string DESTROY_LAST_IMPORTED_FRAMES = "Destroy last imported frames";
        public const string INSTANCE_SELECTED = "FCU instance with name '{0}' is selected.";
        public const string CANT_GET_IMAGE_LINK = "Can't get image link for '{0}' component.";
        public const string WRONG_FILE_NAME = "The '{0}' component name consisted entirely of forbidden characters({1}), and therefore was renamed to '{2}'.";
        public const string SSL_ERROR = "{0}. Try disable HTTPS in the main settings.";
        public const string CLOUDFRONT_ERROR = "Cloudfront has blocked your request to get images. Don't download too many frames at once, or try again later.";
        public const string API_LIMIT = "Some of the images were not received. Cause: figma.com API request limit reached. Please try again later.";
        public const string UNKNOWN_ERROR = "Unknown error.\nStatus: '{0}'\nText: '{1}'\nPlease, report a bug at: {2}";
    }
}
#endif