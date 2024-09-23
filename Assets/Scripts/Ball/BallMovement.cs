using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    [Header("References")]
    public GameObject gameManager;
    GameManager gameManagerInstance;
    public Rigidbody rb;
    public GameObject owner;

    public int numOfHits = 3;
    public enum attacks {toss, serve, dig, bump, set, spike, block};
    public attacks mostRecentAttack;
    public bool ballInPlay;
    public bool beingServed;
    public bool outOfBounds;

    private void Awake()
    {
        gameManagerInstance = gameManager.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = Quaternion.Euler(0, 0, 0);

        if(owner != null)
        {
            if (owner.GetComponent<AthleteStatus>().canServe)
            {
                transform.position = owner.GetComponent<AthleteStatus>().servePoint.transform.position;
                rb.isKinematic = true; 
            }
        }

        
        if (transform.position.y <= -2 && ballInPlay)
        {
            outOfBounds = true;
            StopGame();
        }
    }

    public void Force(float hor, float ver, float frwd, attacks recent, GameObject newOwner)
    {
        rb.velocity = Vector3.zero;
        mostRecentAttack = recent;

        if (numOfHits >= 3 && !(mostRecentAttack == attacks.block || mostRecentAttack == attacks.toss || mostRecentAttack == attacks.serve))
        {
            numOfHits = 1;
        }
        else
        {
            numOfHits++;
        }
        owner = newOwner;
        rb.AddForce(new Vector3(hor, ver, frwd) * gameManagerInstance.hitMultiplier);
        if (mostRecentAttack != attacks.toss)
        {
            StartCoroutine(Predict());
        }
    }

    IEnumerator Predict()
    {
        yield return new WaitForSeconds(.1f);
        GetComponent<BallPrediction>().PredictBallLanding(transform.position, rb.velocity);
    }

    void StopGame()
    {
        if (ballInPlay)
        {
            ballInPlay = false;
            StartCoroutine(gameManagerInstance.AssignPoints());
        }
        
        for (int i = 0; i < gameManagerInstance.athletes.Length; i++)
        {
            if (gameManagerInstance.athletes[i].tag == "AI")
            {
                gameManagerInstance.athletes[i].GetComponent<AINavigation>().Stop();
            }
        }

        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Collides with athlete block barrier
        if(collision.gameObject.layer == 10)
        {
            GameObject colliderParent = collision.transform.parent.gameObject;
            collision.gameObject.SetActive(false);

            if (transform.position.y > collision.transform.position.y)
            {
                //FLY IN ORIGINAL DIRECTION
                float horizontalAmount = transform.position.x - collision.transform.position.x;
                if (colliderParent.GetComponent<AthleteStatus>().team == AthleteStatus.teams.team1)
                {
                    Force(horizontalAmount * gameManagerInstance.blockHorizontalMultiplier, gameManagerInstance.blockHeightForce,
                        -gameManagerInstance.blockForwardForce, BallMovement.attacks.block, colliderParent);
                }
                else
                {
                    Force(horizontalAmount * gameManagerInstance.blockHorizontalMultiplier, gameManagerInstance.blockHeightForce,
                        gameManagerInstance.blockForwardForce, BallMovement.attacks.block, colliderParent);
                }
            }
            else
            {
                //FLY IN OPPOSITE DIRECTION
                if (colliderParent.GetComponent<AthleteStatus>().team == AthleteStatus.teams.team1)
                {
                    Force(gameManagerInstance.blockHorizontalMultiplier, gameManagerInstance.blockHeightForce,
                        gameManagerInstance.blockForwardForce, BallMovement.attacks.block, colliderParent);
                }
                else
                {
                    Force(gameManagerInstance.blockHorizontalMultiplier, gameManagerInstance.blockHeightForce,
                        -gameManagerInstance.blockForwardForce, BallMovement.attacks.block, colliderParent);
                }
            }
        }
        else if (collision.gameObject.layer == 6)
        {
            outOfBounds = OutOfBounds();
            StopGame();
        }
    }

    bool OutOfBounds()
    {
        if((transform.position.x < -9 || transform.position.x > 9 || transform.position.z > 15 || transform.position.z < -15) && ballInPlay)
        {
            return true;
        }
        return false;
    }

}
