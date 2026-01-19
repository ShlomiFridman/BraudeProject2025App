using UnityEngine;
using System.Collections.Generic;

namespace ProjectClasses
{
    [System.Serializable]
    public class QuizQuestion
    {

        public int question_id;
        public string title;
        public string title_he;
        public List<string> answers;
        public List<string> answers_he;
        public int correctAnswerIndex;

        public QuizQuestion()
        {
            this.answers = new List<string>();
            this.answers_he = new List<string>();
        }
        public QuizQuestion(int question_id, string title, string title_he) : this()
        {
            this.question_id = question_id;
            this.title = title;
            this.title_he = title_he;
        }
    }
}
