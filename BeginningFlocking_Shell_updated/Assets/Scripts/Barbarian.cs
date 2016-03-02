using UnityEngine;
using System.Collections;

public class Barbarian : Vehicle {

    //-----------------------------------------------------------------------
    // Class Fields
    //-----------------------------------------------------------------------
    public GameObject seekerTarget;

    //Seeker's steering force (will be added to acceleration)
    private Vector3 force;

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
    override public void Start()
    {
        //call parent's start
        base.Start();

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

        force += Seek(seekerTarget.transform.position) * seekWeight;

        // FLOCKING CALCULATIONS

        force += BarbSeperation() * seperationWeight;

        //limited the seeker's steering force
        //force = Vector3.ClampMagnitude(force, maxForce);


        force += BarbCohesion() * cohesionWeight;

        //limited the seeker's steering force
        //force = Vector3.ClampMagnitude(force, maxForce);


        force += BarbAlignment() * alignmentWeight;

        //limited the seeker's steering force
        //force = Vector3.ClampMagnitude(force, maxForce);

        ApplyForce(force);
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
