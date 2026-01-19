namespace MenuScripts.Guide
{
    [System.Serializable]
    public class GuideData
    {
        public int guideId;
        public string title;
        public string title_he;
        public string imageSpritePath;
        public string text;
        public string text_he;
        
        public override string ToString()
        {
            return $"StationGuide( guideId={guideId}, title='{title}', title_he='{title_he}', imageSpriteFileName='{imageSpritePath}', hasText='{!string.IsNullOrEmpty(text)}')";
        }
    }
}