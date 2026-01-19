namespace ProjectClasses
{
    
    public class AnswerResult
    {
        
        public int question_id { get; set; }
        public int selectedAnswer { get; set; }
        public int correctAnswer { get; set; }
        public float durationSec { get; set; }

        public AnswerResult(int question_id, int selectedAnswer, int correctAnswer, float durationSec)
        {
            this.question_id = question_id;
            this.selectedAnswer = selectedAnswer;
            this.correctAnswer = correctAnswer;
            this.durationSec = durationSec;
        }
    }

}