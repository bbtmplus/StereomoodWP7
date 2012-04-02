namespace TuneYourMood
{
    public class Constants
    {

        public const string CONSUMER_KEY = "5e1051156f4e8f3b8a0f6438cfbc0dc904f46cbb5";
        public const string CONSUMER_SECRET = "e78745fd6af765b320b21e351982b5d5";

        public const string BACKGROUND_URL = "http://stereomood.com/gui/backGround/bg.jpg";
        public const string REQUEST_TOKEN = "http://www.stereomood.com/api/oauth/request_token";
        public const string AUTHORIZE = "http://www.stereomood.com/api/oauth/authorize";
        public const string ACCESS_TOKEN = "http://www.stereomood.com/api/oauth/access_token";
        public const string SEARCH = "http://www.stereomood.com/api/search.json";
        public const string SELECTED_TAGS = "http://www.stereomood.com/api/tag/selected.json";
        public const string TOP_TAGS = "http://www.stereomood.com/api/tag/top.json";


        public const int METHOD_REQUEST_TOKEN = 0;
        public const int METHOD_AUTHORIZATION = 1;
        public const int METHOD_ACCESS_TOKEN = 2;
        public const int METHOD_SEARCH = 3;
        public const int METHOD_SELECTED_TAGS = 4;
        public const int METHOD_TOP_TAGS = 5;

        public const string TYPE_MOOD = "mood";
        public const string TYPE_ACTIVITY = "activity";
        public const string TYPE_SITE = "site";

    }
}