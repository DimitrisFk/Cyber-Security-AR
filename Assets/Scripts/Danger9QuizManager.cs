using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.DataModels;
using PlayFab.ProfilesModels;

public class Danger9QuizManager : MonoBehaviour
{
    public List<QuestionsAndAnswers> QnA;
    public GameObject[] options;
    public int currentQuestion;

    public GameObject Quizpanel;
    public GameObject GoPanel;
    public GameObject WrongGoPanel;

    public Text QuestionTxt;
    public Text ScoreTxt;

    int TotalQuestions = 0;
    public static int score;
    public static int prevscore;
    public static int qnascore;

    public Color startColor;
    public int dangersHighScore;

    private void Start()
    {
        prevscore = Danger8QuizManager.score;
        Debug.Log("prevscore is: " + prevscore);

        TotalQuestions = QnA.Count;
        GoPanel.SetActive(false);
        generateQuestion();
        GetStats();
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameOver()
    {
        Quizpanel.SetActive(false);

        qnascore = score;
        Debug.Log("qnascore is: " + qnascore);

        score = prevscore + score;
        Debug.Log("finalscore is: " + score);

        if (qnascore > 0)
        {
            GoPanel.SetActive(true);
        }
        else
        {
            WrongGoPanel.SetActive(true);
        }

        ScoreTxt.text = score + "/" + "9";
        SetStats();
    }

    public void correct()
    {
        //when your answer is right
        score += 1;
        QnA.RemoveAt(currentQuestion);
        StartCoroutine(WaitForNext());
    }

    public void wrong()
    {
        //when your answer is wrong
        QnA.RemoveAt(currentQuestion);
        StartCoroutine(WaitForNext());
    }

    IEnumerator WaitForNext()
    {
        yield return new WaitForSeconds(1);
        generateQuestion();
    }

    void SetAnswers()
    {
        for (int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<Image>().color = options[i].GetComponent<Danger9AnswerScript>().startColor;
            options[i].GetComponent<Danger9AnswerScript>().isCorrect = false;
            options[i].transform.GetChild(0).GetComponent<Text>().text = QnA[currentQuestion].Answers[i];

            if (QnA[currentQuestion].CorrectAnswer == i + 1)
            {
                options[i].GetComponent<Danger9AnswerScript>().isCorrect = true;
            }
        }
    }

    void generateQuestion()
    {
        if (QnA.Count > 0)
        {
            currentQuestion = Random.Range(0, QnA.Count);

            QuestionTxt.text = QnA[currentQuestion].Question;
            SetAnswers();
        }
        else
        {
            Debug.Log("Out of Question");
            GameOver();
        }
    }

    //*******************************************************************************************************************************************




    //set PlayerStats
    public void SetStats()
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            //request.statistics is a list, so multiple statisticUpdate objects can be defined if required  
            Statistics = new List<StatisticUpdate>{
                new StatisticUpdate{ StatisticName = "DangersHighScore", Value = score },
         }
        },
        result => { Debug.Log("User statistics updated"); },
        error => { Debug.Log(error.GenerateErrorReport()); });
    }

    //get PlayerStats
    void GetStats()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStats,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    void OnGetStats(GetPlayerStatisticsResult result)
    {
        Debug.Log("Received the following Statistics:");
        foreach (var eachStat in result.Statistics)
        {
            Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
            switch (eachStat.StatisticName)
            {
                case "DangersHighScore":
                    dangersHighScore = eachStat.Value;
                    break;
            }
        }
    }
}
