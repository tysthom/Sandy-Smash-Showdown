using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public enum GameMode { playerVAi, aiVAi };
    public GameMode gameMode;

    public GameObject athlete1AI;   //AI model for Ai vs Ai mode only
    public GameObject[] athletes;
    
    public GameObject ball;

    [Header("Points")]
    public int team1Points = 0;
    public int team2Points = 0;
    public TMP_Text team1ScoreText, team2ScoreText, teamScoredText;

    [Header("Serving")]
    public GameObject[] starterPoints;
    public int serverIndex;
    public enum serveTeam { team1, team2 };
    public serveTeam servingTeam;
    public enum serveAthlete { athlete1, athlete2 };
    public serveAthlete servingAthlete;

    [Header("Hitting Stats")]
    public float hitRange = 2;
    public float hitMultiplier = 7;

    [Header("Toss Stats")]
    public float tossHorizontalMultiplier = 0;
    public float tossHeightForce = 100;
    public float tossForwardForce = 0;

    [Header("Serve Stats")]
    public float serveHorizontalMultiplier = 75;
    public float serveHeightForce = 100;
    public float serveForwardForce = 100;

    [Header("Dig Stats")]
    public float digHorizontalMultiplier = 0;
    public float digHeightForce = 120;
    public float digForwardForce = 5;

    [Header("Bump Stats")]
    public float bumpHorizontalMultiplier = 75;
    public float bumpHeightForce = 150;
    public float bumpForwardForce = 25;

    [Header("Set Stats")]
    public float setHorizontalMultiplier = 75;
    public float setHeightForce = 150;
    public float setForwardForce = 25;

    [Header("Spike Stats")]
    public float spikeHorizontalMultiplier = 75;
    public float spikeHeightForce = 150;
    public float spikeForwardForce = 25;

    [Header("Block Stats")]
    public float blockHorizontalMultiplier = 0;
    public float blockHeightForce = 150;
    public float blockForwardForce = 25;

    [Header("Prediction Materials")]
    public Color playerColor;

    void Start()
    {
        if (gameMode == GameMode.aiVAi)
        {
            athlete1AI.SetActive(true);
            athletes[0].SetActive(false);   //Deactivates player
            athletes[0] = athlete1AI;
            

            athletes[1].GetComponent<AthleteStatus>().teamMate = athlete1AI;
        }

        SetUp();
    }

    void SetUp()
    {
        teamScoredText.text = "";
        ball.GetComponent<BallMovement>().outOfBounds = false;

        if (servingTeam == serveTeam.team1)
        {
            if (servingAthlete == serveAthlete.athlete1)
            {
                athletes[0].GetComponent<AthleteStatus>().canServe = true;
                if(athletes[0].tag == "Player")
                {
                    athletes[0].transform.GetChild(0).GetComponent<Animator>().speed = 0;
                }
                else
                {

                }
                
                serverIndex = 0;
                ball.GetComponent<BallMovement>().owner = athletes[0];
                //Sets both athletes positions
                athletes[0].transform.position = starterPoints[0].transform.position;
                athletes[1].transform.position = starterPoints[2].transform.position;
                athletes[2].transform.position = starterPoints[4].transform.position;
                athletes[3].transform.position = starterPoints[5].transform.position;

                if (athletes[0].tag != "Player")
                {
                    StartCoroutine(athletes[0].GetComponent<AINavigation>().ServeToss());
                }
            }
            else
            {
                athletes[1].GetComponent<AthleteStatus>().canServe = true;
                serverIndex = 1;
                ball.GetComponent<BallMovement>().owner = athletes[1];
                athletes[1].transform.position = starterPoints[0].transform.position;
                athletes[0].transform.position = starterPoints[2].transform.position;
                athletes[2].transform.position = starterPoints[5].transform.position;
                athletes[3].transform.position = starterPoints[4].transform.position;

                StartCoroutine(athletes[1].GetComponent<AINavigation>().ServeToss());
            }

        }
        else
        {
            if (servingAthlete == serveAthlete.athlete1)
            {
                athletes[2].GetComponent<AthleteStatus>().canServe = true;
                serverIndex = 2;
                ball.GetComponent<BallMovement>().owner = athletes[2];
                athletes[2].transform.position = starterPoints[1].transform.position;
                athletes[3].transform.position = starterPoints[4].transform.position;
                athletes[0].transform.position = starterPoints[2].transform.position;
                athletes[1].transform.position = starterPoints[3].transform.position;

                StartCoroutine(athletes[2].GetComponent<AINavigation>().ServeToss());
            }
            else
            {
                athletes[3].GetComponent<AthleteStatus>().canServe = true;
                serverIndex = 3;
                ball.GetComponent<BallMovement>().owner = athletes[3];
                athletes[3].transform.position = starterPoints[1].transform.position;
                athletes[2].transform.position = starterPoints[4].transform.position;
                athletes[0].transform.position = starterPoints[3].transform.position;
                athletes[1].transform.position = starterPoints[2].transform.position;

                StartCoroutine(athletes[3].GetComponent<AINavigation>().ServeToss());
            }
        }

        ball.GetComponent<BallMovement>().ballInPlay = true;
        ball.GetComponent<BallMovement>().beingServed = true;
        ball.GetComponent<BallMovement>().numOfHits = 5;

        for (int i = 0; i < athletes.Length; i++)
        {
            athletes[i].GetComponent<AINavigation>().SetUp();
        }
    }

    public IEnumerator AssignPoints()
    {
        Destroy(ball.GetComponent<BallPrediction>().predictionMarker);

        if (ball.GetComponent<BallMovement>().owner.GetComponent<AthleteStatus>().team == AthleteStatus.teams.team1)
        {
            //If ball lands on oppoisite court
            if (!athletes[1].GetComponent<AINavigation>().MyCourt() && !ball.GetComponent<BallMovement>().outOfBounds)
            {
                team1Points++;
                teamScoredText.text = "IN";
                yield return new WaitForSeconds(2);
                teamScoredText.text = "Team 1 Scored!";
                //If athlete who served is not on the team that just scored
                if(athletes[serverIndex].GetComponent<AthleteStatus>().team != AthleteStatus.teams.team1)
                {
                    SwitchServer();
                }
            }
            //If ball lands own court
            if (athletes[1].GetComponent<AINavigation>().MyCourt() || ball.GetComponent<BallMovement>().outOfBounds)
            {
                team2Points++;
                if (!ball.GetComponent<BallMovement>().outOfBounds)
                {
                    teamScoredText.text = "IN";
                }
                else
                {
                    teamScoredText.text = "OUT";
                }
                yield return new WaitForSeconds(2);
                teamScoredText.text = "Team 2 Scored!";
                if (athletes[serverIndex].GetComponent<AthleteStatus>().team != AthleteStatus.teams.team2)
                {
                    SwitchServer();
                }
            }
        }
        else
        {
            //If ball lands on oppoisite court
            if (!athletes[3].GetComponent<AINavigation>().MyCourt() && !ball.GetComponent<BallMovement>().outOfBounds)
            {
                team2Points++;
                teamScoredText.text = "IN";
                yield return new WaitForSeconds(2);
                teamScoredText.text = "Team 2 Scored!";
                if (athletes[serverIndex].GetComponent<AthleteStatus>().team != AthleteStatus.teams.team2)
                {
                    SwitchServer();
                }
            }
            //If ball lands own court
            if (athletes[3].GetComponent<AINavigation>().MyCourt() || ball.GetComponent<BallMovement>().outOfBounds)
            {
                team1Points++;
                if (!ball.GetComponent<BallMovement>().outOfBounds)
                {
                    teamScoredText.text = "IN";
                }
                else
                {
                    teamScoredText.text = "OUT";
                }
                yield return new WaitForSeconds(2);
                teamScoredText.text = "Team 1 Scored!";
                if (athletes[serverIndex].GetComponent<AthleteStatus>().team != AthleteStatus.teams.team1)
                {
                    SwitchServer();
                }
            }
        }

        team1ScoreText.text = "Team 1: " + team1Points;
        team2ScoreText.text = "Team 2: " + team2Points;  

        yield return new WaitForSeconds(2);

        SetUp();
    }

    void SwitchServer()
    {
        if(serverIndex <= 1)
        {
            servingTeam = serveTeam.team2;
            serverIndex++;
        }
        else
        {
            servingTeam = serveTeam.team1;
            if (serverIndex == 2)
            {
                servingAthlete = serveAthlete.athlete2;
                serverIndex = 1;
            }
            else
            {
                servingAthlete = serveAthlete.athlete1;
                serverIndex = 0;
            }
        }

        
    }
}
