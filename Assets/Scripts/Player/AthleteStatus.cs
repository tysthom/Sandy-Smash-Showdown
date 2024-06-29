using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AthleteStatus : MonoBehaviour
{
    public enum teams {team1, team2};
    public teams team;
    public enum roles { settingUp, recieving, observing, engaging};
    public roles role;

    public GameObject servePoint;
    public GameObject digPoint;
    public GameObject bumpPoint;
    public GameObject setPoint;
    public GameObject spikePoint;

    public GameObject teamMate;
    public GameObject netBounds;
    public GameObject blockBoundary;

    public bool lockedIn; //Reached ball prediction area and can't move
    public bool isJumping;

    public bool canServe; //Holding ball
    public bool isServing; //Actively serving, can not move at all

    public bool isBumping;
    public bool isSetting;
    public bool isSpiking;

    public bool isDigging;   //Actively digging for a ball
    public bool isBlocking;
    public bool isRecovering; //Buffer time between certian attacks and being able to move

    /* ANIMATIONS
     * 0 - Idle
     * 1 - Run
     * 2 - Dive
     * 3 - Bump
     * 4 - Set
     * 5 - Jump Pre Spike
     * 6 - Jump Post Spike
     * 7 - Block
     * 8 - Serve Throw
     * 
     */
    

    private void Start()
    {
        lockedIn = true;
    }

    public void Reset()
    {
        isServing = false;
        isBumping = false;
        isSetting = false;
        isSpiking = false;
        isBlocking = false;
        isDigging = false;
        //isRecovering = false;
    }
}
