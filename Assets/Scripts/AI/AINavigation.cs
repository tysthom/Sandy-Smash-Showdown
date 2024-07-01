using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AINavigation : MonoBehaviour
{
    [Header("Script References")]
    [SerializeField] GameObject gameManager;
    GameManager gameManagerInstance;
    [SerializeField] AthleteStatus athleteStatusReference;

    public Transform destination;
    private NavMeshAgent agent;
    private Rigidbody rb;
    Animator anim;
    GameObject ball;
    GameObject teamMate;
    BallMovement ballMovementInstance;
    BallPrediction ballPredictionInstance;



    [Header("Stats")]
    bool isIdle;
    bool isRunning;  //True when moving to next position
    public bool primedToBlock;     //Used to override and assign athlete to block next ball if they spiked most recently on their team. ONLY WITH PLAYER TEAMMATE

    private Vector3 jumpPosition; //Used to keep the x & z values of the AI's jump the same

    bool TeamOwnBall()
    {
        if (ballMovementInstance.owner.GetComponent<AthleteStatus>().team == athleteStatusReference.team)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool IOwnBall()
    {
        if (ballMovementInstance.owner == gameObject)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool MyCourt()
    {
        if (ballPredictionInstance.predictionMarker != null)
        {
            if (athleteStatusReference.team == AthleteStatus.teams.team1 && ballPredictionInstance.predictionMarker.transform.position.z < 0)
            {
                return true;
            }
            else if (athleteStatusReference.team == AthleteStatus.teams.team2 && ballPredictionInstance.predictionMarker.transform.position.z > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private void Awake()
    {
        gameManagerInstance = gameManager.GetComponent<GameManager>();
        ball = gameManager.GetComponent<GameManager>().ball;
        ballMovementInstance = ball.GetComponent<BallMovement>();
        ballPredictionInstance = ball.GetComponent<BallPrediction>();


        athleteStatusReference = GetComponent<AthleteStatus>();
        teamMate = athleteStatusReference.teamMate;
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        anim = transform.GetChild(0).GetComponent<Animator>();
    }

    void Start()
    {
        //SetUp();
    }

    public void SetUp()
    {
        rb.isKinematic = true;
        agent.enabled = true;
        agent.isStopped = false;
        agent.destination = transform.position;
        isIdle = true;
        primedToBlock = false;
        athleteStatusReference.Reset();
    }

    void Update()
    {
        if (isRunning && ReachedDestination())
        {
            isRunning = false;
            athleteStatusReference.lockedIn = true;
        }

        if (isRunning)
        {
            isIdle = false;
            anim.SetInteger("State", 1);
        }
        else
        {
            isIdle = true;

            if (athleteStatusReference.isBumping)
            {
                anim.SetInteger("State", 3);
            }
            else if (athleteStatusReference.isSetting)
            {
                anim.SetInteger("State", 4);
            }
            else if(!(athleteStatusReference.isJumping))
            { 
                anim.SetInteger("State", 0);
            }
            
            SetRotation();
        }

        if (!agent.enabled)
        {
            CorrectRotation();
            KeepHorizontalPosition();
        }
    }

    public void AssignRole()
    {
        if (ball.GetComponent<BallPrediction>().predictionMarker != null)
        {
            athleteStatusReference.lockedIn = false;
            athleteStatusReference.Reset();
            //Receiving ball and passing ball
            if ((!TeamOwnBall() && ballMovementInstance.numOfHits >= 3) || (TeamOwnBall() && (ballMovementInstance.numOfHits < 3)
                || ballMovementInstance.mostRecentAttack == BallMovement.attacks.block)) //Allows AI to pick new landing position after a block
            {
                if (athleteStatusReference.teamMate != null && !IOwnBall() && ((ballMovementInstance.owner == teamMate && MyCourt())
                || (teamMate.GetComponent<AthleteStatus>().isJumping && MyCourt())
                || (CloserThanTeamMate(ballPredictionInstance.predictionMarker) && ballMovementInstance.mostRecentAttack != BallMovement.attacks.spike && !athleteStatusReference.isJumping && MyCourt())
                || (ballMovementInstance.mostRecentAttack == BallMovement.attacks.spike && (!CloserThanTeamMate(ballMovementInstance.owner)
                    || (teamMate.tag == "Player") && !primedToBlock)) //Added in case player is not closer to spiker when ball is spiked, ensures AI still runs to recieve ball
                || (ballMovementInstance.mostRecentAttack == BallMovement.attacks.block && teamMate.GetComponent<AthleteStatus>().isJumping && MyCourt())))
                {
                    //Run to recieve ball 
                    StartCoroutine(RunToSetUp());
                }
                else
                {
                    if (!TeamOwnBall() && ballMovementInstance.mostRecentAttack == BallMovement.attacks.spike)
                    {
                        StartCoroutine(Block());
                    }
                    else
                    {
                        if ((IOwnBall() && ballMovementInstance.mostRecentAttack == BallMovement.attacks.set)
                            || (!TeamOwnBall() && ballMovementInstance.mostRecentAttack == BallMovement.attacks.block && !MyCourt())
                            || (!IOwnBall() && TeamOwnBall() && ballMovementInstance.mostRecentAttack == BallMovement.attacks.block && !MyCourt()))
                        {
                            StartCoroutine(RunToBack());
                        }
                        else
                        {
                            StartCoroutine(RunToNet());
                        }
                    }
                }

            }
            //Preparing to defend
            else
            {
                if ((teamMate.tag != "Player" && !CloserThanTeamMate(ballPredictionInstance.predictionMarker)) || (teamMate.tag == "Player" && !primedToBlock) || IOwnBall())
                {
                    //Prepare to recieve blocked ball
                    if (agent.enabled)
                    {
                        StartCoroutine(RunToBack());
                    }
                }
                else
                {
                    //Run to net and prepare to block
                    StartCoroutine(RunToBlock());
                }
            }

        }
    }

    bool ReachedDestination()
    {
        if (agent.isActiveAndEnabled && agent.remainingDistance > agent.stoppingDistance /*|| agent.velocity.sqrMagnitude != 0f*/)
        {
            isRunning = true;
            return false;
        }
        else
        {
            if (athleteStatusReference.role == AthleteStatus.roles.recieving)
            {
                if (!IOwnBall() && ballMovementInstance.ballInPlay)
                {
                    if (ballMovementInstance.numOfHits >= 3 && !athleteStatusReference.isBumping)
                    {
                        StartCoroutine(Bump());
                    }
                    else if (ballMovementInstance.numOfHits == 1 && !athleteStatusReference.isSetting)
                    {
                        StartCoroutine(Set());
                    }
                    else if (ballMovementInstance.numOfHits == 2 && !athleteStatusReference.isSpiking)
                    {
                        StartCoroutine(Spike());
                    }
                    else
                    {
                        Debug.Log("Fail " + name);
                    }
                }
            }
            return true;
        }
    }

    bool CloserThanTeamMate(GameObject g)
    {
        if (Vector3.Distance(transform.position, g.transform.position) <
           Vector3.Distance(athleteStatusReference.teamMate.transform.position, g.transform.position))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void SetRotation()
    {
        Vector3 direction = Vector3.forward; // Look forward direction
        if (athleteStatusReference.team == AthleteStatus.teams.team1)
        {
            direction = Vector3.forward;
        }
        else if (athleteStatusReference.team == AthleteStatus.teams.team2)
        {
            direction = -Vector3.forward;
        }
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 15f); // Smooth rotation
    }

    public IEnumerator ServeToss()
    {
        if (athleteStatusReference.canServe)
        {
            yield return new WaitForSeconds(.5f); //Serve buffer

            athleteStatusReference.canServe = false;
            athleteStatusReference.isServing = true;

            yield return new WaitForSeconds(.5f); //Serve buffer

            anim.speed = 1;
            anim.SetInteger("State", 8);
            ballMovementInstance.rb.isKinematic = false;
            ballMovementInstance.Force(gameManagerInstance.tossHorizontalMultiplier, gameManagerInstance.tossHeightForce,
                gameManagerInstance.tossForwardForce, BallMovement.attacks.toss, gameObject);

            yield return new WaitForSeconds(.85f);

            //JUMP
            StartCoroutine(Jump());

            anim.SetInteger("State", 5);
            yield return new WaitForSeconds(.65f);
            anim.SetInteger("State", 6);
            
            athleteStatusReference.canServe = false;
            StartCoroutine(Serve());
            yield return new WaitForSeconds(.7f);
            athleteStatusReference.isJumping = false;


        }
    }

    public IEnumerator Serve()
    {
        ballMovementInstance.beingServed = false;

        float horizontalMultiplier = Random.Range(0, transform.position.x) * -.13f;

        if (athleteStatusReference.team == AthleteStatus.teams.team1)
        {
            ballMovementInstance.Force(gameManagerInstance.serveHorizontalMultiplier * horizontalMultiplier, gameManagerInstance.serveHeightForce,
        gameManagerInstance.serveForwardForce, BallMovement.attacks.serve, gameObject);
        }
        else
        {
            ballMovementInstance.Force(gameManagerInstance.serveHorizontalMultiplier * horizontalMultiplier, gameManagerInstance.serveHeightForce,
        -gameManagerInstance.serveForwardForce, BallMovement.attacks.serve, gameObject);
        }

        yield return new WaitForSeconds(.5f);

        athleteStatusReference.isServing = false;
        athleteStatusReference.lockedIn = false;
        
        ball.transform.rotation = athleteStatusReference.servePoint.transform.rotation;
        ballMovementInstance.rb.isKinematic = false;
    }

    IEnumerator RunToSetUp()
    {
        yield return new WaitUntil(() => agent.isActiveAndEnabled == true);
        isRunning = true;
        primedToBlock = false;
        athleteStatusReference.role = AthleteStatus.roles.recieving;
        agent.destination = new Vector3(ballPredictionInstance.predictionMarker.transform.position.x, transform.position.y,
            ballPredictionInstance.predictionMarker.transform.position.z);
    }

    IEnumerator RunToNet()
    {
        yield return new WaitForSeconds(.1f);

        //isRunning = true;
        athleteStatusReference.role = AthleteStatus.roles.observing;
        yield return new WaitUntil(() => agent.isActiveAndEnabled == true);
        isRunning = true;

        float newX = Random.Range(-8, 8);
        for (int i = 0; i < 20; i++) //Up to 20 tries. Loop used to avoid crashing
        {
            Vector3 newPosition = new Vector3(newX, transform.position.y, athleteStatusReference.netBounds.transform.position.z);

            if (Mathf.Abs(ballPredictionInstance.predictionMarker.transform.position.x - newPosition.x) > 7)
            {
                break;
            }

            newX = Random.Range(-8, 8);
        }

        agent.destination = new Vector3(newX, transform.position.y, athleteStatusReference.netBounds.transform.position.z);
    }

    IEnumerator RunToBack()
    {
        yield return new WaitForSeconds(.1f);

        //isRunning = true;
        athleteStatusReference.role = AthleteStatus.roles.observing;
        yield return new WaitUntil(() => agent.isActiveAndEnabled == true);
        isRunning = true;

        if (athleteStatusReference.team == AthleteStatus.teams.team1)
        {
            if (ballPredictionInstance.predictionMarker.transform.position.x > 0)
            {
                agent.destination = new Vector3(Random.Range(1, -5), transform.position.y, Random.Range(-7, -11));
            }
            else
            {
                agent.destination = new Vector3(Random.Range(1, 5), transform.position.y, Random.Range(-7, -11));
            }
        }
        else
        {
            if (ballPredictionInstance.predictionMarker.transform.position.x > 0)
            {
                agent.destination = new Vector3(Random.Range(1, -5), transform.position.y, Random.Range(7, 11));
            }
            else
            {
                agent.destination = new Vector3(Random.Range(1, 5), transform.position.y, Random.Range(7, 11));
            }
        }
    }

    IEnumerator RunToBlock()
    {
        //isRunning = true;
        athleteStatusReference.role = AthleteStatus.roles.observing;

        yield return new WaitForSeconds(.1f);
        isRunning = true;

        if (ballMovementInstance.numOfHits >= 2)
        {
            agent.destination = new Vector3(ballPredictionInstance.predictionMarker.transform.position.x, transform.position.y,
            athleteStatusReference.netBounds.transform.position.z);
        }

    }

    IEnumerator Bump()
    {
        athleteStatusReference.isBumping = true;
        

        yield return new WaitUntil(() => (Vector3.Distance(athleteStatusReference.bumpPoint.transform.position, ball.transform.position)
            < 2 && ball.GetComponent<Rigidbody>().velocity.y < 0) || athleteStatusReference.isBumping == false);

        if (Vector3.Distance(athleteStatusReference.bumpPoint.transform.position, ball.transform.position)
            < 2 && athleteStatusReference.isBumping && ballMovementInstance.ballInPlay)
        {
            ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
            anim.SetInteger("State", 3);

            if (athleteStatusReference.team == AthleteStatus.teams.team1)
            {
                float forwardMultiplier = (Mathf.Abs(transform.position.z - athleteStatusReference.netBounds.transform.position.z) / 12) * Random.Range(.75f, 1);
                float horizontalMultiplier = ((transform.position.x - athleteStatusReference.teamMate.transform.position.x) / 17) * Random.Range(.85f, 1.1f);

                ball.GetComponent<BallMovement>().Force(gameManagerInstance.bumpHorizontalMultiplier * horizontalMultiplier * -1, gameManagerInstance.bumpHeightForce,
                 forwardMultiplier * gameManagerInstance.bumpForwardForce, BallMovement.attacks.bump, gameObject);
            }
            else
            {
                float forwardMultiplier = (Mathf.Abs(transform.position.z - athleteStatusReference.netBounds.transform.position.z) / 12) * Random.Range(.75f, 1);
                float horizontalMultiplier = ((transform.position.x - athleteStatusReference.teamMate.transform.position.x) / 17) * Random.Range(.85f, 1.1f);

                ballMovementInstance.Force(gameManagerInstance.bumpHorizontalMultiplier * horizontalMultiplier * -1, gameManagerInstance.bumpHeightForce,
                 forwardMultiplier * -gameManagerInstance.bumpForwardForce, BallMovement.attacks.bump, gameObject);
            }

            /*if(athleteStatusReference.teamMate.tag == "AI")
            {
                //Used to correct variabale if blocking teammate was not close enough to the blocked ball to jump and block it
                athleteStatusReference.teamMate.GetComponent<AthleteStatus>().isBlocking = false;
            }*/
        }

        athleteStatusReference.isBumping = false;
        yield return new WaitForSeconds(.5f);

    }

    IEnumerator Set()
    {
        athleteStatusReference.isSetting = true;
        

        yield return new WaitUntil(() => (Vector3.Distance(athleteStatusReference.setPoint.transform.position, ball.transform.position)
            < 2 && ball.GetComponent<Rigidbody>().velocity.y < 0) || athleteStatusReference.isSetting == false);

        if (Vector3.Distance(athleteStatusReference.setPoint.transform.position, ball.transform.position)
            < 2 && athleteStatusReference.isSetting && ballMovementInstance.ballInPlay) 
        {
            ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
            anim.SetInteger("State", 4);


            if (athleteStatusReference.team == AthleteStatus.teams.team1)
            {
                float forwardMultiplier = (Mathf.Abs(transform.position.z - athleteStatusReference.netBounds.transform.position.z) / 12) * Random.Range(.85f, 1);
                float horizontalMultiplier = ((transform.position.x - athleteStatusReference.teamMate.transform.position.x) / 17) * Random.Range(.9f, 1.2f);

                ballMovementInstance.Force(gameManagerInstance.setHorizontalMultiplier * horizontalMultiplier * -1, gameManagerInstance.setHeightForce,
                 forwardMultiplier * gameManagerInstance.setForwardForce, BallMovement.attacks.set, gameObject);
            }
            else
            {
                float forwardMultiplier = (Mathf.Abs(transform.position.z - athleteStatusReference.netBounds.transform.position.z) / 12) * Random.Range(.85f, 1);
                float horizontalMultiplier = ((transform.position.x - athleteStatusReference.teamMate.transform.position.x) / 17) * Random.Range(.9f, 1.2f);

                ballMovementInstance.Force(gameManagerInstance.setHorizontalMultiplier * horizontalMultiplier * -1, gameManagerInstance.setHeightForce,
                 forwardMultiplier * -gameManagerInstance.setForwardForce, BallMovement.attacks.set, gameObject);
            }
        }

        athleteStatusReference.isSetting = false;
        yield return new WaitForSeconds(.5f);
    }

    IEnumerator Jump()
    {
        athleteStatusReference.isJumping = true;
        agent.enabled = false;
        rb.isKinematic = false;
        jumpPosition = transform.position;

        rb.AddForce(new Vector3(0, 10, 0), ForceMode.Impulse);

        yield return new WaitForSeconds(1.5f);

        athleteStatusReference.isJumping = false;
        agent.enabled = true;
        rb.isKinematic = true;
    }

    IEnumerator Spike()
    {
        athleteStatusReference.isSpiking = true;

        yield return new WaitUntil(() => (Vector3.Distance(athleteStatusReference.spikePoint.transform.position, ball.transform.position)
            < 6 && ball.GetComponent<Rigidbody>().velocity.y < 0) || athleteStatusReference.isSpiking == false);

        if (athleteStatusReference.isSpiking) StartCoroutine(Jump());
        anim.SetInteger("State", 5);


        yield return new WaitUntil(() => (Vector3.Distance(athleteStatusReference.spikePoint.transform.position, ball.transform.position)
            < 2 && ball.GetComponent<Rigidbody>().velocity.y < 0) || athleteStatusReference.isSpiking == false);

        if (Vector3.Distance(athleteStatusReference.spikePoint.transform.position, ball.transform.position)
            < 2 && athleteStatusReference.isSpiking && ballMovementInstance.ballInPlay)
        {
            anim.SetInteger("State", 6);

            float posXBound = 8.1f;
            float negXBound = -posXBound;

            float posHorMultiplier = (posXBound - transform.position.x);

            float negHorMultiplier = (negXBound - transform.position.x);

            /*float horizontalMultiplier = Random.Range(0, transform.position.x) * -.35f; //Negative to hit the other way*/
            float horizontalMultiplier = Random.Range(negHorMultiplier, posHorMultiplier) * .15f;

            if (athleteStatusReference.team == AthleteStatus.teams.team1)
            {
                ballMovementInstance.Force(gameManagerInstance.spikeHorizontalMultiplier * horizontalMultiplier,
                    gameManagerInstance.spikeHeightForce,
                 gameManagerInstance.spikeForwardForce, BallMovement.attacks.spike, gameObject);
            }
            else
            {
                ballMovementInstance.Force(gameManagerInstance.spikeHorizontalMultiplier * horizontalMultiplier,
                    gameManagerInstance.spikeHeightForce,
                 -gameManagerInstance.spikeForwardForce, BallMovement.attacks.spike, gameObject);
            }
        }

        primedToBlock = true;
        athleteStatusReference.isSpiking = false;
        yield return new WaitForSeconds(.5f);
        //activelyPerforming = false;
    }

    IEnumerator Block()
    {
        athleteStatusReference.isBlocking = true;

        yield return new WaitUntil(() => Vector3.Distance(athleteStatusReference.servePoint.transform.position, ball.transform.position)
            < 7 || athleteStatusReference.isBlocking == false);

        anim.SetInteger("State", 7);

        if (athleteStatusReference.isBlocking)
        {

            StartCoroutine(Jump());

            athleteStatusReference.blockBoundary.SetActive(true);
            yield return new WaitForSeconds(1);
            athleteStatusReference.blockBoundary.SetActive(false);
        }

        athleteStatusReference.isBlocking = false;
    }

    void CorrectRotation()
    {
        if(athleteStatusReference.team == AthleteStatus.teams.team1)
        {
            transform.rotation = Quaternion.Euler(0, transform.rotation.y * 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, transform.rotation.y * 180, 0);
        }
    }

    void KeepHorizontalPosition()
    {
        transform.position = new Vector3(jumpPosition.x, transform.position.y, jumpPosition.z);
    }

    public void Stop()
    {
        if (agent.isActiveAndEnabled)
        {
            athleteStatusReference.Reset();
            agent.destination = transform.position;
            agent.isStopped = true;
        }
    }
}
