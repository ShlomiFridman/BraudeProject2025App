using System.Collections.Generic;

namespace ProjectClasses
{
    public class QuizMessage
    {
        public QuizResult QuizResult { get; set; }
        public List<AnswerResult>  AnswerResults { get; set; }

        public QuizMessage(List<AnswerResult> answerResults, int groupNumber)
        {
            var totalDurationSec = 0.0f;
            AnswerResults = answerResults;

            foreach (var answerResult in answerResults)
            {
                totalDurationSec += answerResult.durationSec;
            }

            QuizResult = new QuizResult(AppUtils.GetUsername(), groupNumber, totalDurationSec);
        }
    }
}