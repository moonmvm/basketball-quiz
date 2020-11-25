using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Question
{
    [JsonProperty("question")]
    public string question;

    [JsonProperty("answers")]
    public Dictionary<string, string> answers;

    [JsonProperty("correct_answer")]
    public string correctAnswer;

    public Question(string question, Dictionary<string, string> answers, string correctAnswer)
    {
        this.question = question;
        this.answers = answers;
        this.correctAnswer = correctAnswer;
    }

    public override string ToString()
    {
        return "Question: " + question + "; Answers: " + answers + "; Correct Answer: " + correctAnswer; 
    }
}
