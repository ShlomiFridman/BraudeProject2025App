namespace ProjectClasses
{
    public class AddBadgeMessage
    {
        public string username;
        public int badge_id;

        public AddBadgeMessage(string username, int badge_id)
        {
            this.username = username;
            this.badge_id = badge_id;
        }
    }
}