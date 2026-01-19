using System;

namespace ProjectClasses
{
    public class QuizResult
    {
        public string username { get; set; }
        public int groupNumber { get; set; }
        public float totalDurationSec { get; set; }

        public QuizResult(string username,int groupNumber, float totalDurationSec)
        {
            this.username = username;
            this.groupNumber = groupNumber;
            this.totalDurationSec = totalDurationSec;
        }
        public QuizResult(string username, float totalDurationSec) : this(username, -1, totalDurationSec)
        { }
    }

}