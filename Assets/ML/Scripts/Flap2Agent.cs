using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Flap2Agent : Agent
{

    Vector3 startposition;  //initial position of bird
    private Animator anim;  //Reference to the Animator component.

    Bird birdScript;

    private int _wins;
    private int _losses;

    public override void InitializeAgent()
    {
        birdScript = GetComponent<Bird>();
        anim = GetComponent<Animator>();
        startposition = new Vector2(0, 2.33f);
    }


    public override void CollectObservations()
    {
        var mlGoalObjects = GameObject.FindGameObjectsWithTag("MLGoalTag");
        Transform[] transforms = mlGoalObjects.Select(y => y.transform).ToArray();
        Transform closestEnemy = GetClosestEnemy(transforms);
        if (closestEnemy != null)
        {
            AddVectorObs(gameObject.transform.position.x - closestEnemy.position.x);
            AddVectorObs(gameObject.transform.position.y - closestEnemy.position.y);
        }
        else
        {
            AddVectorObs(0f);
            AddVectorObs(0f);
        }
        AddVectorObs(gameObject.GetComponent<Rigidbody2D>().velocity.y);
    }


    public override void AgentAction(float[] vectorAction, string textAction)
    {
        float reward = 0.1f;

        if (Mathf.FloorToInt(vectorAction[0]) == 1)
            birdScript.Flap();

        if (birdScript.isDead)
        {
            reward = -1f;
            _losses++;
            Done();
        }
        else
        {
            //scored?
            if (GameControl.instance.gotScore)
            {
                GameControl.instance.gotScore = false;
                reward = 1f;
                _wins++;
            }
        }

        AddReward(reward);

        //Monitor.Log("Reward", reward, MonitorType.text, transform);
        //Monitor.Log("Wins", _wins, MonitorType.text);
        //Monitor.Log("Losses", _losses, MonitorType.text);
    }

    Transform GetClosestEnemy(Transform[] enemies)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Transform potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            //close and ahead of us
            if (dSqrToTarget < closestDistanceSqr && potentialTarget.position.x > currentPosition.x)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }


    public override void AgentReset()
    {
        _wins = 0;
        transform.position = startposition;

        anim.SetTrigger("Idle");
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 0);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        var poolingScript = Object.FindObjectOfType<ColumnPool>();
        poolingScript.ResetPositions();

        ScrollingObject[] scrollingObjects = 
            (ScrollingObject[])GameObject.FindObjectsOfType(typeof(ScrollingObject));
        foreach (var scrollingObject in scrollingObjects)
        {
            scrollingObject.InitScrollingObject();
        }

        birdScript.isDead = false;
        GameControl.instance.gameOver = false;
    }

    public override void AgentOnDone()
    {
    }
}
