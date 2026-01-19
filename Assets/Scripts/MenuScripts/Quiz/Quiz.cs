using UnityEngine;
using System.Collections.Generic;

namespace ProjectClasses
{
    
    [System.Serializable]
    public class Quiz
    {
        public string title;
        public string title_he;
        public int groupNumber;
    
        // Initialize to avoid null reference
        public List<QuizQuestion> questions = new List<QuizQuestion>();

        public override string ToString()
        {
            return $"Quiz( title: '{title}', title_he: '{title_he}', questionsCount: {questions.Count} )";
        }
    }
}
