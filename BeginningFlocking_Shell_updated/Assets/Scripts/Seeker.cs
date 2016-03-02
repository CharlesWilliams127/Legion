using UnityEngine;
using System.Collections;

public class Seeker : Vehicle {

    //-----------------------------------------------------------------------
    // Class Fields
    //-----------------------------------------------------------------------
    public GameObject seekerTarget;

    //Seeker's steering force (will be added to acceleration)
    private Vector3 force;

    private int currentNode;
    private int nextNode;

    // boolean for if the last waypoint is triggered
    private bool pathEnded;

    public bool PathEnded
    {
        get { return pathEnded; }
    }

    //WEIGHTING!!!!
    public float seekWeight = 12.0f;
	public float safeDistance = 10.0f;
	public float avoidWeight = 10.0f;
    public float seperationWeight = 10.0f;
    public float cohesionWeight = 10.0f;
    public float alignmentWeight = 5.0f;



    //-----------------------------------------------------------------------
    // Start - No Update
    //-----------------------------------------------------------------------
	// Call Inherited Start and then do our own
	override public void Start () {
        //call parent's start
		base.Start();

        pathEnded = false;

        currentNode = 0;
        nextNode = 1;

        //initialize
        force = Vector3.zero;
	}

    //-----------------------------------------------------------------------
    // Class Methods
    //-----------------------------------------------------------------------

    protected override void CalcSteeringForces()
    {
        //reset value to (0, 0, 0)
        force = Vector3.zero;

        //got a seeking force
        if (pathEnded == false)
        {
            PathFollow();
            force += Seek(seekerTarget.transform.position);
        }
        if(pathEnded == true)
        {
            transform.rotation = new Quaternion(0f, -.9935f, 0f, -.1135f);
            velocity = Vector3.zero;
            acceleration = Vector3.zero;
        }

        //limited the seeker's steering force
        force = Vector3.ClampMagnitude(force, maxForce);

        //applied the steering force to this Vehicle's acceleration (ApplyForce)
        ApplyForce(force);
    }

    private void PathFollow()
    {
        if (Vector3.Distance(transform.position, seekerTarget.transform.position) <= 10)
        {
            if (nextNode != 7)
            {
                seekerTarget = gm.Waypoints[nextNode];

                nextNode += 1;

                currentNode += 1;
            }
            else
            {
                pathEnded = true;
            }
        }
    }

	// obstacle avoidance, self explanatory
	//protected Vector3 ObstacleAvoidance(GameObject obstacle) {
		// set the desired
		//desired = Vector3.zero;

		// get the distance from the seeker to the obstacle's center
		//Vector3 centerDist = obstacle.transform.position - transform.position;

		// get the radius of the obstacle
		//float obstacleRadius = obstacle.GetComponent<ObstacleScript>().Radius;

		// check if objects aren't in the safe zone
		//if (centerDist.magnitude > safeDistance) {
			//return desired;
		//}

		// if the obstacles are behind the seeker then don't worry about them
		//if (Vector3.Dot (centerDist, transform.forward) < 0) {
			//return desired;
		//}

		// if they're not within the seeker's movement zone then don't worry about them
		//if (Mathf.Abs (Vector3.Dot (centerDist, transform.right)) > radius + obstacleRadius) {
			//return desired;
		//}

		// if the code reaches here, there will be a collision!

		// check whether to steer right or left
		//if (Vector3.Dot (centerDist, transform.right) < 0) {
			//desired = transform.right * maxSpeed;
		//}

		//if (Vector3.Dot (centerDist, transform.right) >=0) {
			//desired = transform.right * -maxSpeed;
		//}

		//return desired;
	//}

}
