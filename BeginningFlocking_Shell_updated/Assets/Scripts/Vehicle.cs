using UnityEngine;
using System.Collections;

//use the Generic system here to make use of a Flocker list later on
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]

abstract public class Vehicle : MonoBehaviour {

    //-----------------------------------------------------------------------
    // Class Fields
    //-----------------------------------------------------------------------

    //movement
    protected Vector3 acceleration;
    protected Vector3 velocity;
    protected Vector3 desired;
    protected bool hasHit = false;

    // radius for arrival
    public float arrivalRadius;
    protected Vector3 arrivalDist;

    // rank position
    protected Vector3 rankPos;

    public Vector3 Velocity {
        get { return velocity; }
    }

    public Vector3 RankPos
    {
        set { rankPos = value; }

        get { return rankPos; }
    }

    //public for changing in Inspector
    //define movement behaviors
    public float maxSpeed = 6.0f;
    public float maxForce = 12.0f;
    public float mass = 1.0f;
    public float radius = 1.0f;
    public float wallFollowDistance = 5.0f;

    // values for flocking
    public float sepRadius = 2.0f;

    //access to Character Controller component
    CharacterController charControl;

	// access to the game mangager script
	protected GameManager gm;
    

    abstract protected void CalcSteeringForces();


    //-----------------------------------------------------------------------
    // Start and Update
    //-----------------------------------------------------------------------
	virtual public void Start(){
        //acceleration = new Vector3 (0, 0, 0);     
        acceleration = Vector3.zero;
        velocity = transform.forward;
        charControl = GetComponent<CharacterController>();

		// instantiate the GameManager object by using the find method
		gm = GameObject.Find("GameManagerGO").GetComponent<GameManager> ();
	}

	
	// Update is called once per frame
	protected void Update () {
		// calculate the right steering forces
		CalcSteeringForces ();

		// add the acceleration to the velocity
		velocity += acceleration * Time.deltaTime;
		// this keeps the guy on the same plane
		velocity.y = 0;

		// limit the velocity to the max speed
		velocity = Vector3.ClampMagnitude (velocity, maxSpeed);

		// orient the dude
		transform.forward = velocity.normalized;

		// move the character based on the calculated velocity
		charControl.Move (velocity * Time.deltaTime);

		// reset the acceleration
		acceleration = Vector3.zero;
	}


    //-----------------------------------------------------------------------
    // Class Methods
    //-----------------------------------------------------------------------

    protected void ApplyForce(Vector3 steeringForce) {
        acceleration += steeringForce / mass;
    }

    protected Vector3 Seek(Vector3 targetPos) {
        desired = targetPos - transform.position;
        desired = desired.normalized * maxSpeed;
        desired -= velocity;
        desired.y = 0;
        return desired;
    }

    protected Vector3 Flee(Vector3 targetPos)
    {
        desired = targetPos - transform.position;
        desired = desired.normalized * maxSpeed;
        desired -= velocity;
        desired.y = 0;
        return desired * -1;
    }

    ///
    /// FLOCKING METHODS BELOW
    ///

    ///
    /// Seperation - this will keep all the dudes from bumping into each other
    ///

    public Vector3 LegionSeperation()
    {
        // this is the total flee force that will be the sum of any number of flockers fleeing
        Vector3 totalFleeForce = new Vector3(0, 0, 0);

        // loop through all vehicles and see if any are close to this one
        for(int i = 0; i < gm.numFlockers; i++)
        {
            // calculate the distance between this flocker and another
            Vector3 distance = gm.Flock[i].transform.position - transform.position;

            // check if this distance is greater than zero (we don't want the flocker fleeing from itself!)
            // and if it less than the seperation distance then this flocker will flee this flocker
            if(distance.magnitude > 0 && distance.magnitude < sepRadius)
            {
                Vector3 partialFleeForce = Flee(gm.Flock[i].transform.position);

                // add the partial flee force to the total one
                totalFleeForce = partialFleeForce + totalFleeForce;
            }
        }
        // this should be the combined seperation force
        return totalFleeForce;
    }
    public Vector3 BarbSeperation()
    {
        // this is the total flee force that will be the sum of any number of flockers fleeing
        Vector3 totalFleeForce = new Vector3(0, 0, 0);

        // loop through all vehicles and see if any are close to this one
        for (int i = 0; i < gm.numBarbFlockers; i++)
        {
            // calculate the distance between this flocker and another
            Vector3 distance = gm.BarbFlock[i].transform.position - transform.position;

            // check if this distance is greater than zero (we don't want the flocker fleeing from itself!)
            // and if it less than the seperation distance then this flocker will flee this flocker
            if (distance.magnitude > 0 && distance.magnitude < sepRadius)
            {
                Vector3 partialFleeForce = Flee(gm.BarbFlock[i].transform.position);

                // add the partial flee force to the total one
                totalFleeForce = partialFleeForce + totalFleeForce;
            }
        }
        // this should be the combined seperation force
        return totalFleeForce;
    }

    ///
    /// Cohesion - this will keep all the dudes together
    ///

    public Vector3 LegionCohesion()
    {

        Debug.DrawLine(transform.position, rankPos);

        // seek the rank position
        Vector3 centerForce = Seek(rankPos);

        // return the central seeking force
        return centerForce;
        

    }

    ///
    /// Barbarian cohesion, this is the traditional cohesion method
    ///
    public Vector3 BarbCohesion()
    {
        Vector3 centerForce = Seek(gm.barbCentroidObject.transform.position);

        return centerForce;
    }

    ///
    /// Alignment - this will make sure the dudes go in the same direction
    ///

    public Vector3 LegionAlignment()
    {
        // the steering force is the difference between the flock's average and this flocker's forward vector
        desired = gm.FlockDirection * maxSpeed;

        // return the alignment force
        return desired - velocity;
    }

    public Vector3 BarbAlignment()
    {
        // the steering force is the difference between the flock's average and this flocker's forward vector
        desired = gm.BarbFlockDirection * maxSpeed;

        // return the alignment force
        return desired - velocity;
    }

    ///
    /// Wall Following!
    /// This method will calculate the appropriate vectors to make any vehicle follow walls
    /// will take the current wall being aligned to as a parameter
    ///

   public void WallFollowing()
   {
        // first use the velocity vector to determine whether or not the vehicle will collide with a wall
        // RAYCASTING!
        //if(Physics.Raycast(transform.position, transform.forward, wallFollowDistance))
        //{
            // if the ray strikes something then the hasHit is set to true!
            //hasHit = true;
        //}

        // if the hasHit is true
        //if(hasHit == true)
        //{
            //Debug.Log("is hitting!");
        //}

       // if we want to keep using rays, use layer masks to determine walls from googly eye guys

       // maybe raycasting isn't the right way to go about things
       // how about looking at obstacle avoidance

        // shoot a ray from the vehicle and see if it smacks into the wall
        // if so, then do:
        // if(ray.magnitude <= velocity.magnitude)
        //if()
        // then that means the vehicle will crash next time
        // this means we need to steer it along the wall
        // how?
        // find the right vector of the wall and project the forward vector onto that with dot product?
        // would that work?
    }
}