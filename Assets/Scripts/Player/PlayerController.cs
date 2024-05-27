using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("General References")]
    [SerializeField] GameObject gameManager;
    GameManager gameManagerInstance;
    GameObject ball;
    Rigidbody rb;
    Animator anim;
    public TMP_Text nextMoveDynamicText;

    [Header("Script References")]
    [SerializeField] AthleteStatus athleteStatusReference;
    BallMovement ballMovementInstance;
    BallPrediction ballPredictionInstance;

    [Header("Input References")]
    private PlayerControls playerControls;
    private PlayerInput playerInput;
    private InputAction moveAction;
    InputAction serveAction;
    InputAction jumpAction;
    InputAction spikeAction;
    InputAction bumpAction;
    InputAction digAction;
    InputAction setAction;

    [Header("Stats")]
    [SerializeField] float speed = 25;
    public float turnSpeed = 15f;
    public float hitRange; //Range in which ball must be in order for the player to hit it
    public float lockInRange = 1.3f; //Range in which player must be to snap into position and be lockedIn
    public float recoveryTime = .5f;
    bool inAir;
    //Used to ensure that the player performs the correct hit when tasked with hitting the ball
    bool canDig, canBump, canSet, canSpike;

    private void Awake()
    {   
        gameManagerInstance = gameManager.GetComponent<GameManager>();
        athleteStatusReference = GetComponent<AthleteStatus>();
        ball = gameManager.GetComponent<GameManager>().ball;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        anim = transform.GetChild(0).GetComponent<Animator>();

        ballMovementInstance = ball.GetComponent<BallMovement>();
        ballPredictionInstance = ball.GetComponent<BallPrediction>();

        playerInput = GetComponent<PlayerInput>();
        playerControls = new PlayerControls();
        moveAction = playerInput.actions["Move"];
        serveAction = playerInput.actions["Serve"];
        jumpAction = playerInput.actions["Jump"];
        spikeAction = playerInput.actions["Spike"];
        bumpAction = playerInput.actions["Bump"];
        digAction = playerInput.actions["Dig"];
        setAction = playerInput.actions["Set"];

        hitRange = gameManagerInstance.hitRange;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();

        serveAction.performed -= ctx => StartCoroutine(Serve());
        jumpAction.performed -= ctx => StartCoroutine(Jump());
        spikeAction.performed -= ctx => StartCoroutine(Spike());
        bumpAction.performed -= ctx => StartCoroutine(Bump());
        digAction.performed -= ctx => StartCoroutine(Dig());
        setAction.performed -= ctx => StartCoroutine(Set());
    }

    void Start()
    {
        serveAction.performed += _ => StartCoroutine(Serve());
        jumpAction.performed += _ => StartCoroutine(Jump());
        spikeAction.performed += _ => StartCoroutine(Spike());
        bumpAction.performed += _ => StartCoroutine(Bump());
        digAction.performed += _ => StartCoroutine(Dig());
        setAction.performed += _ => StartCoroutine(Set());
    }

    void Update()
    {
        if(ballMovementInstance.ballInPlay == false)
        {
            ResetHitConstraints();
            return;
        }

        athleteStatusReference.lockedIn = LockedIn();

        if (!athleteStatusReference.lockedIn && !athleteStatusReference.isRecovering && !athleteStatusReference.isDigging 
            && !athleteStatusReference.isServing)
        {
            MovePlayer();
        }
        else if(athleteStatusReference.lockedIn && !athleteStatusReference.isServing && !athleteStatusReference.isBumping && !athleteStatusReference.isDigging 
            && !athleteStatusReference.isSetting && !athleteStatusReference.isSpiking && !athleteStatusReference.isRecovering )
        {
            anim.SetInteger("State", 0);
        }
        else if (!athleteStatusReference.lockedIn && athleteStatusReference.isDigging)
        {
            anim.SetInteger("State", 2);
        }
        else if (athleteStatusReference.lockedIn || athleteStatusReference.isRecovering)
        {
            if (athleteStatusReference.isBumping)
            {
                anim.SetInteger("State", 3);
            }
            else if (athleteStatusReference.isSetting)
            {
                Debug.Log("SET");
                anim.SetInteger("State", 4);
            }
            else if (athleteStatusReference.isSpiking)
            {
                if(ballMovementInstance.owner != gameObject) //Player has jumped but not yet spiked ball
                {
                    anim.SetInteger("State", 5);
                }
                else
                {
                    anim.SetInteger("State", 6);
                }
                
            }
        }
    }

    bool LockedIn()
    {
        if (ballMovementInstance.beingServed)
        {
            return true;
        }

        if ((ballPredictionInstance.predictionMarker != null
            && Vector3.Distance(transform.position, ballPredictionInstance.predictionMarker.transform.position) < lockInRange
            && ballMovementInstance.owner != gameObject) || inAir)
        {
            if (!inAir)
            {
                transform.position = new Vector3(ballPredictionInstance.predictionMarker.transform.position.x, 1.28f,
                   ballPredictionInstance.predictionMarker.transform.position.z);

            }

            LockRotation();
            return true;
        }
        else
        {
            return false;
        }
    }

    void LockRotation()
    {
        Vector3 targetDirection = new Vector3(0, 0, 0);
        transform.position += targetDirection * speed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
    }

    void MovePlayer()
    {
        Vector2 moveDirection = moveAction.ReadValue<Vector2>();
        
        if (!athleteStatusReference.canServe && !inAir)
        {
            Vector3 targetDirection = new Vector3(moveDirection.x, 0, moveDirection.y);
            transform.position += targetDirection * speed * Time.deltaTime;

            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            if (targetDirection != Vector3.zero)
            {
                targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
                anim.SetInteger("State", 1);
            }
            else
            {
                anim.SetInteger("State", 0);
            }
 
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
        else
        {
            Vector3 targetDirection = new Vector3(moveDirection.x, 0, Mathf.Abs(moveDirection.y));
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            athleteStatusReference.servePoint.transform.rotation = Quaternion.Slerp(athleteStatusReference.servePoint.transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
    }

    public void UpdateNextMoveIndicators()
    {
        ResetHitConstraints();
        //Opposing team served
        if(ballMovementInstance.mostRecentAttack == BallMovement.attacks.serve && ballMovementInstance.owner.GetComponent<AthleteStatus>().team != athleteStatusReference.team)
        {
            if (CloserThanTeamMate(ballPredictionInstance.predictionMarker))
            {
                nextMoveDynamicText.text = "BUMP/DIG";
                ballPredictionInstance.predictionMarker.GetComponent<Renderer>().material.color = gameManagerInstance.playerColor;
                canDig = canBump = true;
            }
            else
            {
                nextMoveDynamicText.text = "SET";
            }
        }
        //Teammate bumped ball
        else if(ballMovementInstance.mostRecentAttack == BallMovement.attacks.bump && ballMovementInstance.owner == athleteStatusReference.teamMate)
        {
            nextMoveDynamicText.text = "SET";
            ballPredictionInstance.predictionMarker.GetComponent<Renderer>().material.color = gameManagerInstance.playerColor;
            canSet = true;
        }
        //Teammate set ball
        else if (ballMovementInstance.mostRecentAttack == BallMovement.attacks.set && ballMovementInstance.owner == athleteStatusReference.teamMate)
        {
            nextMoveDynamicText.text = "SPIKE";
            ballPredictionInstance.predictionMarker.GetComponent<Renderer>().material.color = gameManagerInstance.playerColor;
            canSpike = true;
        }
        //Ball was blocked and landing on Player's court
        else if (ballMovementInstance.mostRecentAttack == BallMovement.attacks.block && ballMovementInstance.owner != gameObject && ballPredictionInstance.predictionMarker.transform.position.z < 0)
        {
            nextMoveDynamicText.text = "BUMP/DIG";
            ballPredictionInstance.predictionMarker.GetComponent<Renderer>().material.color = gameManagerInstance.playerColor;
            canDig = canBump = true;
        }
        //Opponents set ball
        else if (ballMovementInstance.mostRecentAttack == BallMovement.attacks.set && ballMovementInstance.owner.GetComponent<AthleteStatus>().team != athleteStatusReference.team)
        {
            //Player is closer to inevitable spiker
            if((!CloserThanTeamMate(ballMovementInstance.owner.GetComponent<AthleteStatus>().teamMate) && athleteStatusReference.teamMate.GetComponent<AINavigation>().primedToBlock) 
                || athleteStatusReference.teamMate.GetComponent<AINavigation>().primedToBlock)
            {
                nextMoveDynamicText.text = "BUMP/DIG";
            }
            else
            {
                nextMoveDynamicText.text = "BLOCK";
            }
        }
        //Opponents spiked ball
        else if (ballMovementInstance.mostRecentAttack == BallMovement.attacks.spike && ballMovementInstance.owner.GetComponent<AthleteStatus>().team != athleteStatusReference.team)
        {
            //Player is closer to spiker
            if ((!CloserThanTeamMate(ballMovementInstance.owner) && athleteStatusReference.teamMate.GetComponent<AINavigation>().primedToBlock) 
                || athleteStatusReference.teamMate.GetComponent<AINavigation>().primedToBlock)
            {
                nextMoveDynamicText.text = "BUMP/DIG";
                ballPredictionInstance.predictionMarker.GetComponent<Renderer>().material.color = gameManagerInstance.playerColor;
                canDig = canBump = true;
            }
            else
            {
                nextMoveDynamicText.text = "BLOCK";
            }
        }
        else
        {
            nextMoveDynamicText.text = "";
        }
    }

    bool CloserThanTeamMate(GameObject g)
    {
        if(Vector3.Distance(g.transform.position, transform.position) < Vector3.Distance(g.transform.position, athleteStatusReference.teamMate.transform.position))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void ResetHitConstraints()
    {
        canDig = false;
        canBump = false;
        canSet = false;
        canSpike = false;
    }

    IEnumerator Serve()
    {
        if (athleteStatusReference.canServe)
        {
            Vector2 moveDirection = moveAction.ReadValue<Vector2>();

            athleteStatusReference.canServe = false;
            athleteStatusReference.isServing = true;
            athleteStatusReference.teamMate.GetComponent<AINavigation>().primedToBlock = true;

            yield return new WaitForSeconds(.1f); //Serve buffer

            ballMovementInstance.rb.isKinematic = false;
            ballMovementInstance.Force(moveDirection.x * gameManagerInstance.serveHorizontalMultiplier, gameManagerInstance.serveHeightForce,
                gameManagerInstance.serveForwardForce, BallMovement.attacks.serve, gameObject);

            yield return new WaitForSeconds(.5f);
            athleteStatusReference.isServing = false;
            athleteStatusReference.lockedIn = false;
            ballMovementInstance.beingServed = false;
        }
    }

    IEnumerator Dig()
    {
        if (canDig && !athleteStatusReference.lockedIn && ballMovementInstance.owner != gameObject)
        {

            canDig = false;
            athleteStatusReference.isDigging = true;
            for (int i = 0; i < 10; i++)
            {
                if (ballPredictionInstance.predictionMarker != null)
                {

                    if (Vector3.Distance(athleteStatusReference.digPoint.transform.position, ball.transform.position) < hitRange) //Player has successfully reached ball
                    {
                        i = 10;
                        //ADDS FORCE
                        ballMovementInstance.Force(0 * gameManagerInstance.digHorizontalMultiplier, gameManagerInstance.digHeightForce,
                            gameManagerInstance.digForwardForce, BallMovement.attacks.dig, gameObject);
                    }
                    else
                    {
                        Vector3 targetPosition = new Vector3(ballPredictionInstance.predictionMarker.transform.position.x, transform.position.y, ballPredictionInstance.predictionMarker.transform.position.z);

                        Vector3 targetDirection = (targetPosition - transform.position).normalized;

                        transform.position += targetDirection * 125 * Time.deltaTime;

                        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                    }
                }
                else
                {
                    Debug.Log("DIG - Prediction Marker not found");
                }

                yield return new WaitForSeconds(.02f);
            }

            
            athleteStatusReference.isDigging = false;
            athleteStatusReference.isRecovering = true;

            yield return new WaitForSeconds(1);

            athleteStatusReference.isRecovering = false;
        }
    }

    IEnumerator Bump()
    {
        if (canBump && !athleteStatusReference.isBumping && !athleteStatusReference.isDigging && athleteStatusReference.lockedIn && ballMovementInstance.owner != gameObject)
        {
            Vector2 moveDirection = moveAction.ReadValue<Vector2>();
            anim.SetInteger("State", 3);
            if (Vector3.Distance(athleteStatusReference.bumpPoint.transform.position, ball.transform.position) < hitRange) //Player has successfully reached ball
            {
                athleteStatusReference.isBumping = true;

                float forwardMultiplier = (Mathf.Abs(transform.position.z - athleteStatusReference.netBounds.transform.position.z) / 12) * Random.Range(.75f, 1);
                float horizontalMultiplier = (Mathf.Abs(transform.position.x - athleteStatusReference.teamMate.transform.position.x) / 17) * Random.Range(.5f, 1.25f);

                ballMovementInstance.Force(moveDirection.x * gameManagerInstance.bumpHorizontalMultiplier * horizontalMultiplier, 
                    gameManagerInstance.bumpHeightForce, 
                    gameManagerInstance.bumpForwardForce * forwardMultiplier, BallMovement.attacks.bump, gameObject);

                StartCoroutine(Recover());
                yield return new WaitForSeconds(1);

                athleteStatusReference.isBumping = false;
                athleteStatusReference.lockedIn = false;
            }
        }
    }

    IEnumerator Set()
    {
        if (canSet && !athleteStatusReference.isSetting && athleteStatusReference.lockedIn && ballMovementInstance.owner != gameObject)
        {
            Vector2 moveDirection = moveAction.ReadValue<Vector2>();
            if (Vector3.Distance(athleteStatusReference.setPoint.transform.position, ball.transform.position) < hitRange + .25f) //Player has successfully reached ball
            {
                athleteStatusReference.isSetting = true;

                float forwardMultiplier = (Mathf.Abs(transform.position.z - athleteStatusReference.netBounds.transform.position.z) / 12) * Random.Range(.75f, 1);
                float horizontalMultiplier = (Mathf.Abs(transform.position.x - athleteStatusReference.teamMate.transform.position.x) / 17) * Random.Range(.5f, 1.25f);

                ballMovementInstance.Force(moveDirection.x * gameManagerInstance.setHorizontalMultiplier * horizontalMultiplier,
                    gameManagerInstance.setHeightForce,
                    gameManagerInstance.setForwardForce * forwardMultiplier, BallMovement.attacks.set, gameObject);

                StartCoroutine(Recover());
                yield return new WaitForSeconds(1);

                athleteStatusReference.isSetting = false;
                athleteStatusReference.lockedIn = false;
            }
        }
    }

    IEnumerator Jump()
    {
        if (!inAir && !athleteStatusReference.isServing && ballMovementInstance.ballInPlay)
        {
            inAir = true;
            rb.isKinematic = false;

            rb.AddForce(new Vector3(0, 10, 0), ForceMode.Impulse);

            if(canSpike)
            {
                athleteStatusReference.isSpiking = true;
                
            }
            else
            {
                athleteStatusReference.blockBoundary.SetActive(true);
            }

            yield return new WaitForSeconds(.5f);
            yield return new WaitUntil(() => transform.position.y <= 1.3f);
            transform.position = new Vector3(transform.position.x, 1.28f, transform.position.z);

            athleteStatusReference.blockBoundary.SetActive(false);
            rb.isKinematic = true;
            inAir = false;
        }
    }

    IEnumerator Spike()
    { 
        if (inAir && athleteStatusReference.isSpiking && !athleteStatusReference.canServe && athleteStatusReference.lockedIn && ballMovementInstance.owner != gameObject)
        { 
            Vector2 moveDirection = moveAction.ReadValue<Vector2>();
            if (Vector3.Distance(athleteStatusReference.spikePoint.transform.position, ball.transform.position) < hitRange) //Player has successfully reached ball
            {
                ballMovementInstance.Force(moveDirection.x * gameManagerInstance.spikeHorizontalMultiplier, gameManagerInstance.spikeHeightForce,
                        gameManagerInstance.spikeForwardForce, BallMovement.attacks.spike, gameObject);
            }

            yield return new WaitForSeconds(1);

            athleteStatusReference.isSpiking = false;
            athleteStatusReference.lockedIn = false;
        }
    }

    IEnumerator Recover()
    {
        athleteStatusReference.isRecovering = true;
        yield return new WaitForSeconds(recoveryTime);
        athleteStatusReference.isRecovering = false;
    }
}
