using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public bool gameInPlay;
    public enum GameMode { playerVAi, aiVAi };
    public GameMode gameMode;

    public GameObject player;
    public GameObject maleFriendlyAI;
    public GameObject femaleFriendlyAI;
    public GameObject maleEnemyAI;
    public GameObject femaleEnemyAI;
    public GameObject[] athletes;
    
    public GameObject ball;

    [Header("Cameras")]
    public Camera gameCamera;
    public Camera winnerCamera;

    [Header("Points")]
    public int winningAmount = 7;
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

    [Header("Post Game Positions")]
    public GameObject winnerPosition1;
    public GameObject winnerPosition2;

    void Start()
    {
        gameInPlay = true;
        gameCamera.enabled = true;
        winnerCamera.enabled = false;

        if (gameMode == GameMode.aiVAi)
        {
            /*athletes[0].GetComponent<PlayerController>().nextMoveHeaderText.text = "";
            athletes[0].GetComponent<PlayerController>().nextMoveDynamicText.text = "";*/

            for(int i = 0; i < 4; i++)
            {
                GameObject ath = null;
                switch (GetComponent<AppearanceManager>().athletesBodyType[i])  //Initiates proper AI type and body type
                {    
                    case 0:
                        ath = Instantiate(i == 0 || i == 1 ? maleFriendlyAI : maleEnemyAI);
                        ath.transform.position = new Vector3(5, 5, 0);
                        athletes[i] = ath;
                        break;
                    case 1:
                        ath = Instantiate(i == 0 || i == 1 ? femaleFriendlyAI : femaleEnemyAI);
                        ath.transform.position = new Vector3(5, 5, 0);
                        athletes[i] = ath;
                        break;
                }

                foreach (Transform child in athletes[i].transform.GetChild(0))  //Used forst child, then uses child's children
                {
                    if (child.name == "Skin")
                    {
                        GetComponent<AppearanceManager>().athletesSkinMesh[i] = child.GetComponent<SkinnedMeshRenderer>();
                        break;
                    }
                }           
            }
        }
        else
        {
            //Needs proper integration
            player.SetActive(true);
            athletes[0] = player;
            foreach (Transform child in athletes[0].transform.GetChild(0))
            {
                if (child.name == "Skin")
                {
                    GetComponent<AppearanceManager>().athletesSkinMesh[0] = child.GetComponent<SkinnedMeshRenderer>();
                    break;
                }
            }

            for (int i = 1; i < 4; i++)
            {
                GameObject ath = null;
                switch (GetComponent<AppearanceManager>().athletesBodyType[i])  //Initiates proper AI type and body type
                {
                    case 0:
                        ath = Instantiate(i == 0 || i == 1 ? maleFriendlyAI : maleEnemyAI);
                        ath.transform.position = new Vector3(5, 5, 0);
                        athletes[i] = ath;
                        break;
                    case 1:
                        ath = Instantiate(i == 0 || i == 1 ? femaleFriendlyAI : femaleEnemyAI);
                        ath.transform.position = new Vector3(5, 5, 0);
                        athletes[i] = ath;
                        break;
                }

                foreach (Transform child in athletes[i].transform.GetChild(0))  //Used forst child, then uses child's children
                {
                    if (child.name == "Skin")
                    {
                        GetComponent<AppearanceManager>().athletesSkinMesh[i] = child.GetComponent<SkinnedMeshRenderer>();
                        break;
                    }
                }
            }

        }

        SetUp();
        GetComponent<AppearanceManager>().AssignSkin();
        GetComponent<AppearanceManager>().AssignOutfits();
        GetComponent<AppearanceManager>().AssignHair();
    }

    void SetUp()
    {
        if (team1Points == 0 && team2Points == 0)
        {
            teamScoredText.text = "First to " + winningAmount + " Wins";
        }
        else if (team1Points == winningAmount-1 || team2Points == winningAmount-1)
        {
            teamScoredText.text = "Game Point";
        }
        else
        {
            teamScoredText.text = "";
        }

        ball.GetComponent<BallMovement>().outOfBounds = false;
        athletes[0].GetComponent<AthleteStatus>().teamMate = athletes[1];
        athletes[1].GetComponent<AthleteStatus>().teamMate = athletes[0];
        athletes[2].GetComponent<AthleteStatus>().teamMate = athletes[3];
        athletes[3].GetComponent<AthleteStatus>().teamMate = athletes[2];
        for(int i = 0; i < athletes.Length; i++)
        {
            athletes[i].GetComponent<AthleteStatus>().netBounds = (i <= 1 ? GameObject.Find("T1 Net Bounds") : 
                GameObject.Find("T2 Net Bounds"));
        }

        if (servingTeam == serveTeam.team1)
        {
            if (servingAthlete == serveAthlete.athlete1)
            {
                athletes[0].GetComponent<AthleteStatus>().canServe = true;
                if(athletes[0].tag == "Player")
                {
                    athletes[0].transform.GetChild(0).GetComponent<Animator>().speed = 0;
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
            if(athletes[i].tag != "Player") { athletes[i].GetComponent<AINavigation>().SetUp(); }
        }
    }

    public IEnumerator AssignPoints()
    {
        Destroy(ball.GetComponent<BallPrediction>().predictionMarker);

        if (ball.GetComponent<BallMovement>().owner.GetComponent<AthleteStatus>().team == AthleteStatus.teams.team1)
        {
            //If ball lands on oppoisite court
            if (!athletes[1].GetComponent<AINavigation>().LandMyCourt() && !ball.GetComponent<BallMovement>().outOfBounds)
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
            if (athletes[1].GetComponent<AINavigation>().LandMyCourt() || ball.GetComponent<BallMovement>().outOfBounds)
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
            if (!athletes[3].GetComponent<AINavigation>().LandMyCourt() && !ball.GetComponent<BallMovement>().outOfBounds)
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
            if (athletes[3].GetComponent<AINavigation>().LandMyCourt() || ball.GetComponent<BallMovement>().outOfBounds)
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

        team1ScoreText.text = team1Points.ToString();
        team2ScoreText.text = team2Points.ToString();  

        yield return new WaitForSeconds(3);

        if(team1Points == winningAmount)
        {
            teamScoredText.text = "Team 1 Wins!";
            StartCoroutine(GameWon());
        }
        else if(team2Points == winningAmount)
        {
            teamScoredText.text = "Team 2 Wins!";
            StartCoroutine(GameWon());
        }
        else
        {
            SetUp();
        }
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

    IEnumerator GameWon()
    {
        yield return new WaitForSeconds(2);

        gameInPlay = false;
        gameCamera.enabled = false;
        winnerCamera.enabled = true;
        teamScoredText.text = "";

        Vector3 direction = -Vector3.forward;

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        for (int i = 0; i < athletes.Length; i++)
        {
            if (athletes[i].tag == "Player")
            {
                athletes[i].GetComponent<PlayerController>().GameFinished(GetComponent<AppearanceManager>().athletesBodyType[i]);
            }
            else
            {
                athletes[i].GetComponent<AINavigation>().GameFinished(GetComponent<AppearanceManager>().athletesBodyType[i]);

            }
            
            athletes[i].transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1);
        }

        if (team1Points > team2Points)
        {
            athletes[0].transform.position = winnerPosition1.transform.position;
            athletes[1].transform.position = winnerPosition2.transform.position;

            athletes[2].transform.position = new Vector3(500,500, 500);
            athletes[3].transform.position = new Vector3(500, 500, 500);
        }
        else
        {
            athletes[2].transform.position = winnerPosition1.transform.position;
            athletes[3].transform.position = winnerPosition2.transform.position;

            athletes[0].transform.position = new Vector3(500, 500, 500);
            athletes[1].transform.position = new Vector3(500, 500, 500);
        }
    }
}
