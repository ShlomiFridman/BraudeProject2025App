using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using Unity.Services.Core.Analytics;
using System.Collections.Generic;
using System;

public class AnalyticsManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
    }


    public void recordQuiz(string quizName, float quizScore, float totalTimeSec)
    {

        QuizEvent quizEvent = new QuizEvent(quizName, quizScore, totalTimeSec);

        AnalyticsService.Instance.RecordEvent(quizEvent);
        AnalyticsService.Instance.Flush();

        Debug.Log($"Recorded {quizEvent}");

    }

    private class QuizEvent : Unity.Services.Analytics.Event
    {

        public string quizName;
        public float quizScore;
        public float totalTimeSec;

        public QuizEvent(string quizName, float quizScore, float totalTimeSec) : base("QuizResultEvent")
        {
            this.quizName = quizName;
            this.quizScore = (float)Math.Round(quizScore, 2);
            this.totalTimeSec = (float)Math.Round(totalTimeSec, 2);
            this.SetParameter("QuizName", this.quizName);
            this.SetParameter("QuizScore", this.quizScore);
            this.SetParameter("QuizTotalTimeSec", this.totalTimeSec);
        }


        public override string ToString()
        {
            return $"QuizEvent[ QuizName = {quizName}, QuizScore = {quizScore}, QuizTotalTimeSec = {totalTimeSec} ]";
        }
    }
}
