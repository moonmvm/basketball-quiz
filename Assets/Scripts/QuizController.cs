using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Newtonsoft.Json;
using UnityEngine.UI;
using Random = System.Random;

public class QuizController : MonoBehaviour
{
    public Text currentQuestionNum;
    public Text currentQuestionText;
    public Text resultText;
    public Button[] answerButtons;
    public GameObject quizCanvas;
    public GameObject resultCanvas;

    private List<Question> questions = new List<Question>();
    private List<Question> questionsCopy;
    private Dictionary<string, string> rates = new Dictionary<string, string>();
    private string currentQuestionSample = "{0}/{1}";
    private string resultSample = "{0}!\nCorrect answers: {1}";
    private string currectCorrectAnswer;
    private int correctAnswers = 0;
    private int currentQuestion;
    private int questionsCount;
    private int questionsLeft;

    void Start()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseDatabase.DefaultInstance
            .GetReference("Questions")
            .GetValueAsync().ContinueWith(task => {
                if (task.IsFaulted)
                {
                    Debug.Log("Fail");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    foreach (DataSnapshot question in snapshot.Children)
                    {
                        Question newQuestion = JsonConvert.DeserializeObject<Question>(question.GetRawJsonValue());
                        questions.Add(newQuestion);
                    }

                }
            });

        FirebaseDatabase.DefaultInstance
            .GetReference("Rates")
            .GetValueAsync().ContinueWith(task => {
                if (task.IsFaulted)
                {
                    Debug.Log("Fail");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    foreach (DataSnapshot rate in snapshot.Children)
                    {
                        rates.Add(rate.Key, rate.Value.ToString());
                    }
                }
            });
    }

    public void OnStartButtonPressed()
    {
        questionsCopy = new List<Question>(questions);
        questionsCount = questionsCopy.Count;
        currentQuestion = 1;
        LoadQuestion();
    }

    public void OnAnswerButtonPressed(string answer)
    {
        if (answer == currectCorrectAnswer)
        {
            correctAnswers++;
        }

        if (questionsLeft > 0)
        {
            LoadQuestion();
        }
        else
        {
            quizCanvas.SetActive(false);
            resultCanvas.SetActive(true);
            string resultRate = rates[correctAnswers.ToString()];
            resultText.text = string.Format(resultSample, resultRate, correctAnswers);
        }
    }

    private void LoadQuestion()
    {
        questionsLeft = questionsCopy.Count - 1;
        currentQuestionNum.text = string.Format(currentQuestionSample, currentQuestion, questionsCount);
        Question question = GetQuestion();
        FillFieldsByQuestionData(question);
        currentQuestion++;
    }

    private void FillFieldsByQuestionData(Question question)
    {
        currentQuestionText.text = question.question;
        currectCorrectAnswer = question.correctAnswer;

        int answersCount = 0;
        foreach (Button answerButton in answerButtons)
        {
            string answerChar = GetAnswerChar(answersCount);
            answerButton.GetComponentInChildren<Text>().text = question.answers[answerChar];
            answersCount++;
        }
    }

    private Question GetQuestion()
    {
        int randomQuestion = 0;
        Random random = new Random();

        if (questionsLeft > 1)
        {
            randomQuestion = random.Next(1, questionsLeft);
        }

        Question question = questionsCopy[randomQuestion];
        questionsCopy.RemoveAt(randomQuestion);
        return question;
    }

    private string GetAnswerChar(int buttonIndex)
    {
        if (buttonIndex == 0)
        {
            return "A";
        }
        else if (buttonIndex == 1)
        {
            return "B";
        }
        else if (buttonIndex == 2)
        {
            return "C";
        }
        else
        {
            return "D";
        }
    }
}
