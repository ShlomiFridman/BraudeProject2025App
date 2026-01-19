namespace ProjectClasses
{
    public static class AppConstants
    {
        public const string EmptyStationCartId = "cart_empty";

        public const string ServerLocalAddress = "192.168.1.218:5000";
        public const string ServerDeploymentAddress = "braudeproject2025server.onrender.com";
        public const bool IsLocalServer = false;

        public const string LinktreeURL = "https://linktr.ee/plaras";

        public const float CartDefaultSpeed = 0.25f;

        public const int ReconnectionWaitMs = 500;
        
        public const float ToastFadeDurationSec = 0.8f;
        public const float ToastDefaultVisibleDurationSec = 2f;
        
        public const string StationsInfoDataFile = "jsonFiles/stationsInfo";
        public const string AppQuizDataFile = "jsonFiles/AppQuiz";
        public const string GuidesDataFile = "jsonFiles/Guides";
    }
}