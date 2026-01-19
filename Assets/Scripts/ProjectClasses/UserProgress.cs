using System.Collections.Generic;
using JetBrains.Annotations;

namespace ProjectClasses
{
    public class UserProgress
    {
        public string username;
        public double playDurationMin;
        public List<int> guidesRead = new();

        public UserProgress()
        {}

        public UserProgress(string username)
        {
            this.username = username;
        }
    }
}