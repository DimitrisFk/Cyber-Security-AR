using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.DataModels;
using PlayFab.ProfilesModels;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats PS;

    public int quizNum;
    public int quizScore;
    public int playerHighScore;

    private void OnEnable()
    {
        if(PlayerStats.PS == null)
        {
            PlayerStats.PS = this;
        }
        else
        {
            if(PlayerStats.PS != this)
            {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    //passing playerStats
    public void SetStats()
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            //request.statistics is a list, so multiple statisticUpdate objects can be defined if required  
            Statistics = new List<StatisticUpdate>{
                new StatisticUpdate{ StatisticName = "QuizNumber", Value = quizNum }, 
                new StatisticUpdate{ StatisticName = "QuizScore", Value = quizScore }, 
                new StatisticUpdate{ StatisticName = "PlayerHighScore", Value = playerHighScore },
         }
        },
        result => { Debug.Log("User statistics updated"); },
        error => { Debug.Log(error.GenerateErrorReport()); });
    }

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
                case "QuizNumber":
                    quizNum = eachStat.Value;
                    break;
                case "QuizScore":
                    quizScore = eachStat.Value;
                    break;
                case "PlayerHighScore":
                    playerHighScore = eachStat.Value;
                    break;
            }
        }
    }
}
