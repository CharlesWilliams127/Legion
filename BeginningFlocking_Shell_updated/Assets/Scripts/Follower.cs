using UnityEngine;
using System.Collections;

public class Follower : Vehicle {

	//-----------------------------------------------------------------------
	// Class Fields
	//-----------------------------------------------------------------------

	// this is the radius around the follower's target that they will begin to slow down at
	public float slowRadius = 5f;
    public float avoidRadius = 3f;
	public float followDistance = -5f;

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
	override public void Start () {
		//call parent's start
		base.Start();
		
		//initialize
		force = Vector3.zero;
	}
	
	//-----------------------------------------------------------------------
	// Class Methods
	//-----------------------------------------------------------------------
	
	protected override void CalcSteeringForces() {
		//reset value to (0, 0, 0)
		force = Vector3.zero;

        // put arrival in here!
        arrivalDist = rankPos - transform.position;
		
		//got a seeking force
		//force += Seek(seekerTarget.transform.position) * seekWeight;
		
		force = FollowTheLeader () * seekWeight;

		//limited the seeker's steering force
		//force = Vector3.ClampMagnitude(force, maxForce);
		
		//applied the steering force to this Vehicle's acceleration (ApplyForce)

        //WallFollowing();
		
		// loop throught the obstacle array and avoid each obstacle
		//for (int i = 0; i < 5; i++) {
		//force = ObstacleAvoidance(gm.Obstacles[i]) * avoidWeight;
		
		// apply the force
		//ApplyForce (force);
		//}
		
		// FLOCKING CALCULATIONS
		
		force += LegionSeperation() * seperationWeight;

		//limited the seeker's steering force
		//force = Vector3.ClampMagnitude(force, maxForce);
		
		
		force += LegionCohesion() * cohesionWeight;

		//limited the seeker's steering force
		//force = Vector3.ClampMagnitude(force, maxForce);
		
		
		force += LegionAlignment() * alignmentWeight;

		//limited the seeker's steering force
		//force = Vector3.ClampMagnitude(force, maxForce);

        if (arrivalDist.magnitude < arrivalRadius)
        {
            maxSpeed = 6;
            // essentially stop all guys if they hit their point
            Vector3 targetOffset = rankPos - transform.position;
            float distance = targetOffset.magnitude;
            float rampedSpeed = maxSpeed * (distance / arrivalDist.magnitude);
            if (rampedSpeed > maxSpeed)
            {
                rampedSpeed = maxSpeed;
            }
            desired = ((rampedSpeed / distance) * targetOffset);
            force += (desired - velocity);
            ApplyForce(force);
        }
        else
        {
            maxSpeed = 9;
            ApplyForce(force);
        }
	}

	/// <summary>
	/// Follows the leader.
	/// should be quite similar to seek, but seeking the leader, and with arrival programmed into it
	/// </summary>
	/// <returns>The the desired vector that will seek the leader</returns>
	/// 
	public Vector3 FollowTheLeader()
	{
		// the first step will to be finding out the position that the followers will be seeking
		// this position will be 1 unit behind the leader
		// then slowly reduce the max speed as they get closer to the leader

		// find the point where the follower will be seeking, it should be one unit behind the leader
		Vector3 target = gm.leader.transform.position - gm.leader.transform.forward*(-1*followDistance);
		Debug.DrawLine (gm.Centroid, target, Color.yellow);

		// if the leader is outside of the slow radius then seek as normal
		Vector3 distance = target - transform.position;

        // the distance that the followers will keep from the leader
        Vector3 avoidDist = gm.leader.transform.position - transform.position;

        // check to see if the follower is too close to the leader
        if (avoidDist.magnitude < avoidRadius)
        {
            // if the follower is too close to the leader then flee
            desired = Flee(gm.leader.transform.position);

            return desired;
        }
        else
        {
            return Vector3.zero;
        }
        // check to see if the follower is outside of the arrival radius
            /*
		else if (distance.magnitude > slowRadius) {
            // if the follower is outside the slow radius, then seek as normally would
			desired = Seek (target);

			return desired;
		} 
		// if the follower is within the slow distance then 
		else {
            // the ramped speed should be speed scaling according to how close the follower is to the leader
			float rampedSpeed = maxSpeed * (distance.magnitude / slowRadius);

            // made using Craig Reynold's guide
			desired = (rampedSpeed / distance.magnitude) * distance;

			return desired - velocity;
		}
             */
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
