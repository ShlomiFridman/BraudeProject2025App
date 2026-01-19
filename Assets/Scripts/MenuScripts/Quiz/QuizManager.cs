using UnityEngine;
using Unity.Services.Analytics;
using ProjectClasses;
using RTLTMPro;
using System.Collections.Generic;
using System.Threading.Tasks;
using MenuScripts.Badges;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.Serialization;
using UnityEngine.Localization.Settings;
using System.Text;

public class QuizManager : MonoBehaviour
{
    public static QuizManager Instance { get; private set; }

    private Quiz quiz;
    private int currentQuestionIndex;
    private bool IsEnglish =>
        LocalizationSettings.SelectedLocale.Identifier.Code == "en";

    public RTLTextMeshPro quizTitle;
    public RTLTextMeshPro questionTitleLabel;
    public GameObject answersGroup;
    public GameObject AnswerRowPrefab;
    public GameObject PreviousQuestionButton;
    public GameObject NextQuestionButton;
    public GameObject EndDemoButton;

    private AnalyticsManager analyticsManager;

    private int score;
    private List<AnswerResult> selectedAnswers = new List<AnswerResult>();
    private float startTime;
    public int CurrentGroup => quiz?.groupNumber ?? 1;
    

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        analyticsManager = AppUtils.GetAnalyticsManager();
    }

    // Fetch quiz from server
    public async Task FetchQuiz(int groupNumber)
    {
        quiz = await HttpService.Instance.GetQuiz(groupNumber);
        if (quiz == null)
        {
            Debug.LogError($"FetchQuiz: returned null for group {groupNumber}");
            return;
        }

        Debug.Log($"Fetched quiz: '{quiz.title}' (Locale: {LocalizationSettings.SelectedLocale.Identifier.Code})");
        SetQuiz();
    }

    // Setup quiz UI
    public void SetQuiz()
    {
        if (quiz == null)
        {
            Debug.LogError("SetQuiz called but quiz is null.");
            return;
        }

        if (quizTitle == null)
        {
            Debug.LogError("SetQuiz: quizTitle is not assigned in Inspector!");
            return;
        }

        currentQuestionIndex = 0;
        score = 0;
        selectedAnswers.Clear();
        EndDemoButton?.SetActive(false);

        togglePrevNextButtons(false);

        // Set quiz title and first question
        UpdateQuizText();
    }

    // Update quiz title and current question based on current locale
    public void UpdateQuizText()
    {
        quizTitle.text = IsEnglish ? quiz.title : quiz.title_he;
        setQuestion(quiz.questions[currentQuestionIndex], IsEnglish);
    }

    // Show question and answers
    private void setQuestion(QuizQuestion question, bool isEnglish)
    {
        string prefix = isEnglish ? "Question" : "שאלה";

        questionTitleLabel.text = $"{prefix} {currentQuestionIndex + 1}: " +
                                  (isEnglish ? question.title : question.title_he);

        bool alreadyAnswered = currentQuestionIndex < selectedAnswers.Count;

        // Clear old answers
        foreach (Transform child in answersGroup.transform)
            Destroy(child.gameObject);

        // Create answer buttons
        for (int i = 0; i < question.answers.Count; i++)
        {
            string answerText = isEnglish ? question.answers[i] : question.answers_he[i];
            GameObject answerRow = Instantiate(AnswerRowPrefab, answersGroup.transform);
            AnswerRowBehaviour rowBehaviour = answerRow.GetComponent<AnswerRowBehaviour>();
            rowBehaviour.setAnswer(i, answerText, onAnswerSelect);

            if (!alreadyAnswered) continue;

            rowBehaviour.disableButton();
            if (i == question.correctAnswerIndex)
                rowBehaviour.setBtnColor(Color.green);
            else if (i == selectedAnswers[currentQuestionIndex].selectedAnswer)
                rowBehaviour.setBtnColor(Color.red);
        }

        startTime = Time.time;
        togglePrevNextButtons(alreadyAnswered);
        if (currentQuestionIndex >= quiz.questions.Count)
        {
            Debug.LogWarning($"setQuestion called with invalid index {currentQuestionIndex}, questions count {quiz.questions.Count}");
            return;
        }
    }

    private void onAnswerSelect(int answerIndex)
    {
        if (currentQuestionIndex < selectedAnswers.Count) return;

        var question = quiz.questions[currentQuestionIndex];
        var answer = new AnswerResult(
            question.question_id,
            answerIndex,
            question.correctAnswerIndex,
            Time.time - startTime);

        bool isCorrect = answer.selectedAnswer == answer.correctAnswer;
        score += isCorrect ? 1 : 0;
        selectedAnswers.Add(answer);

        displayQuestionResult(answerIndex);
        togglePrevNextButtons(true);
    }

    private void displayQuestionResult(int selectedIndex)
    {
        for (int i = 0; i < answersGroup.transform.childCount; i++)
        {
            AnswerRowBehaviour row = answersGroup.transform.GetChild(i).GetComponent<AnswerRowBehaviour>();
            row.disableButton();
            if (i == quiz.questions[currentQuestionIndex].correctAnswerIndex)
                row.setBtnColor(Color.green);
            else if (i == selectedIndex)
                row.setBtnColor(Color.red);
        }
    }

private void togglePrevNextButtons(bool showBtns)
{
    if (quiz == null || quiz.questions == null) return;

    bool hasPrev = currentQuestionIndex > 0;

    // Always allow next button when showBtns = true
    NextQuestionButton.SetActive(showBtns);

    // Only allow prev when not the first question
    PreviousQuestionButton.SetActive(showBtns && hasPrev);
}

    public void onNextQuestionBtnClick()
    {
        togglePrevNextButtons(false);

        if (currentQuestionIndex < quiz.questions.Count - 1)
        {
            currentQuestionIndex++;
            setQuestion(quiz.questions[currentQuestionIndex], IsEnglish);
        }
        else
        {
            resultWindow(); // reached end, show results
        }
    }

    public void onPreviousQuestionBtnClick()
    {
        if (currentQuestionIndex > 0)
        {
            currentQuestionIndex--;
            setQuestion(quiz.questions[currentQuestionIndex], IsEnglish);
        }
    }
    
    public string BuildLastQuizSummary(bool isEnglish)
    {
        if (quiz == null || selectedAnswers.Count == 0)
            return isEnglish ? "No quiz was submitted" : "לא נשלח שאלון";

        var sb = new StringBuilder();

        if (isEnglish)
            sb.AppendLine($"Score: {score}/{quiz.questions.Count}");
        else
            sb.AppendLine($"ציון: {quiz.questions.Count}/{score}");

        for (int i = 0; i < selectedAnswers.Count; i++)
        {
            bool correct = selectedAnswers[i].selectedAnswer == selectedAnswers[i].correctAnswer;

            string mark = correct
                ? "<color=#00AA00>v</color>"   // green
                : "<color=#CC0000>x</color>";  // red

            if (isEnglish)
                sb.AppendLine($"Question {i + 1}: {mark}");
            else
                sb.AppendLine($"שאלה {i + 1}: {mark}");
        }

        return sb.ToString();
    }

    private void resultWindow()
    {
        var quizResult = new QuizMessage(selectedAnswers, quiz.groupNumber);
        _ = HttpService.Instance.PostQuiz(quizResult);

        float quizScore = (float)score / quiz.questions.Count;
        _ = BadgesManager.Instance.OnQuizComplete(quizScore);

        questionTitleLabel.text = BuildLastQuizSummary(IsEnglish);

        foreach (Transform child in answersGroup.transform)
            Destroy(child.gameObject);

        EndDemoButton.SetActive(true);
    }
}
