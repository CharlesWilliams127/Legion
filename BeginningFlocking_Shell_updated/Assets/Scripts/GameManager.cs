
using UnityEngine;
using System.Collections;

//add using System.Collections.Generic; to use the generic list format
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    //-----------------------------------------------------------------------
    // Class Fields
    //-----------------------------------------------------------------------
	// this is the leader googly guy, he's an albino
	public GameObject leader;
    public GameObject target;

    public GameObject dudePrefab;
	public GameObject leaderPrefab;
    public GameObject targetPrefab;
    public GameObject obstaclePrefab;
    public GameObject wallPrefab;

    // barbarian stuff
    public GameObject barbCentroidObject;
    public GameObject barbPrefab;

    //private GameObject[] obstacles;

    // barbarian flocking variables
    private Vector3 pos;
    private Vector3 pos2;
    private Vector3 barbCentroid;
    private Vector3 barbFlockDirection;
    private List<GameObject> barbFlock;

    public Vector3 BarbCentroid
    {
        get { return barbCentroid; }
    }

    public Vector3 BarbFlockDirection
    {
        get { return barbFlockDirection; }
    }

    public List<GameObject> BarbFlock
    {
        get { return barbFlock; }
    }

    private bool pos1;

    //Declare the camera array and counter

    ///<summary>
    /// Game Cameras for the player to switch between
    /// The second camera is coded as a follow the guy camera
    /// </summary>
    public Camera[] cameras;
    private int currentCameraIndex;

    //-----------------------------------------------------------------------
    // RANKING SYSTEM
    // HOW IT WORKS
    // there will be numerous vectors perpendicular to the leaders forward (i.e they run along his right vector), and they will be apart from each other determined by a given distance
    // currently that number is hard-coded at 3, may make this adjustable depending on difficulty
    // each flocker will be assigned to a rank, this is represented by adding them to a list of all flockers for a given rank
    // this is why the amount of ranks is hardcoded at 3, if not, I would have to create a whole bunch of lists that represent each rank
    // if I have time I may change this
    // anyway
    // each flocker will be given a rank and each rank will have a number of "positions" in it, these will coorespond to the amount
    // of flockers in the rank.
    // the number of flockers in the rank will be calculated by dividing the total flock by 3
    // if the number of flockers is not divisible by 3 then the extras will be added to the third rank
    //----------------------------------------------------------------------

    // rank forming variables
    //public int numRanks = 3; // the actual number of ranks
    private int flockersPerRank; // the number of flockers allowed in a single rank
    public int rankDist; // variable for determining distance between ranks
    private List<GameObject> rank1;
    private List<GameObject> rank2;
    private List<GameObject> rank3; 

    // variables to hold the centroids for each rank
    private Vector3 rank1Centroid;
    private Vector3 rank2Centroid;
    private Vector3 rank3Centroid;

    // GameObjects for the rank's centroids
    public GameObject rank1Object;
    public GameObject rank2Object;
    public GameObject rank3Object;

    // these vectors will run along their respective object's right vector and be centered on the rank object
    private Vector3 rank1Vector;
    private Vector3 rank2Vector;
    private Vector3 rank3Vector;

    // this is the distance that between positions in ranks
    public float distBetweenRankers;

    // this list is the list of positions, the number should be equal to the amount of flockers there are
    // each position represents a position on a rank, these will be calculated in the CalcRank method
    // then essentially replace alignment with the men seeking their corresponding position in the rank
    private Vector3[] positions;

    // this is the array of waypoints that the leader should seek, currently there are 7
    private GameObject[] waypoints;

    // make an array of walls
    private GameObject[] walls;

    // accessor for walls array
    public GameObject[] Walls
    {
        get { return walls; }
    }

    public GameObject[] Waypoints
    {
        get { return waypoints; }
    }

	// accessor for the obstacles array
	//public GameObject[] Obstacles {
		//get { return obstacles; }
	//}

	// flocking stuff
	private Vector3 centroid;
    private Vector3 flockDirection;

	public Vector3 Centroid {
		get { return centroid; }
	}

	public Vector3 FlockDirection {
		get { return flockDirection; }
	}

	// list of flockers
	private List<GameObject> flock;

	public List<GameObject> Flock {
		get { return flock; }
	}

	// how many flockers are there?
	public int numFlockers;

    public int numBarbFlockers;


    //-----------------------------------------------------------------------
    // Start and Update
    //-----------------------------------------------------------------------
	void Start () {
        pos1 = true;
        
        //In start initialize both
        currentCameraIndex = 0;

        //Turn all cameras off, except the first default one
        for (int i = 1; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(false);
        }

        //If any cameras were added to the controller, enable the first one
        if (cameras.Length > 0)
        {
            cameras[0].gameObject.SetActive(true);
            //write to the console which camera is enabled
            Debug.Log("Camera with name: " + cameras[0].GetComponent<Camera>().name + ", is now enabled");
        }
        
        // instantiate the array of waypoints
        waypoints = new GameObject[7];

        // loop through the waypoints array and assign waypoints to values
        for (int i = 0; i < waypoints.Length; i ++)
        {
            waypoints[i] = GameObject.Find("WayPoint" + (i + 1).ToString());
        }

            // set up the rank lists
        rank1 = new List<GameObject>();
        rank2 = new List<GameObject>();
        rank3 = new List<GameObject>();

        // initialize positions
        positions = new Vector3[numFlockers];

        // set the centroid and flock direction

		// create the flock
		flock = new List<GameObject> ();

        barbFlock = new List<GameObject>();

        //Create the target (noodle)
        pos = new Vector3(394f, 4.0f, 77.8f);
        pos2 = new Vector3(381.1f, 4f, 230.4f);

        target = (GameObject)Instantiate(targetPrefab, pos, Quaternion.identity);
        //target = Instantiate(targetPrefab, pos, Quaternion.identity) as GameObject;

        //Create the GooglyEye Guy at (10, 1, 10)
        pos = new Vector3(10, 1, 10);
        leader = (GameObject)Instantiate(leaderPrefab, transform.position, Quaternion.identity);

        //set the camera's target 
        Camera.main.GetComponent<SmoothFollow>().target = leader.transform;

		// set the googly eye guy's target
		leader.GetComponent<Seeker> ().seekerTarget = waypoints[0];

		// create multiple flockers
		for (int i = 0; i < numFlockers; i++) {
			// create a position
			Vector3 randPos = new Vector3(Random.Range(-30, 30), 1, Random.Range(-30, 30));

			flock.Add((GameObject)Instantiate(dudePrefab, transform.position, Quaternion.identity));
		}

        // create multiple flockers
        for (int i = 0; i < numBarbFlockers; i++)
        {
            // create a position
            Vector3 randPos = new Vector3(Random.Range(-30, 30), 1, Random.Range(-30, 30));
            
            barbFlock.Add((GameObject)Instantiate(barbPrefab, barbCentroidObject.transform.position, Quaternion.identity));
            barbFlock[i].GetComponent<Barbarian>().seekerTarget = target;
        }

        //---------------------RANKING-----------------------
        // ADDING FOLLOWERS TO RANKS WORKS BEAUTIFULLY
        // GOOD JOB M8
        // yes I am talking to myself, no I'm not absoultely insane

        // get the number of flockers to a rank
        flockersPerRank = numFlockers / 3;

        // only add flockers to three ranks if there is at least 3 flockers
        if (numFlockers > 3)
        {
            // assign each of the dudes to a rank
            for (int i = 0; i < flockersPerRank; i++)
            {
                // add the current flocker to the first rank
                rank1.Add(flock[i]);
            }

            // once the appropriate amount of flockers has been added to the first rank then work on adding them to the second
            // say the number of flockers per rank is 5, then the first list will add flock elements 0 - 4 to rank 1
            // this means we have to start from the number of flockers per rank (5) and add flockers 5 - 9 to rank 2
            // we then stop at the number of flockers per rank * 2, as this will stop us at 10
            for (int j = flockersPerRank; j < flockersPerRank * 2; j++)
            {
                // add the current flocker to the second rank
                rank2.Add(flock[j]);
            }

            // then do the same to the third, but start at flockers per rank * 2, and go until we hit the number of flockers
            // this will mean if the number of flockers is not divisable by 3 then the extras will get added to this rank
            for (int k = flockersPerRank * 2; k < numFlockers; k++)
            {
                // add the current flocker to the third rank
                rank3.Add(flock[k]);
            }
        }
            // if there is less than 3 flockers, add them all to the first rank to make it simple
            // keep in mind that when calculating positions for the flockers of all ranks to seek, include if else's or try catch's to
            // make sure that if ranks 2 and 3 are empty we don't get errors
        else
        {
            for(int i = 0; i < numFlockers; i++)
            {
                // add the current flocker to rank 1
                rank1.Add(flock[i]);
            }
        }

            // create the obstacles 
            //for (int i = 0; i < 5; i++) {
            //pos = new Vector3(Random.Range(-30, 30), 1.1f, Random.Range (-30, 30));
            //Quaternion rot = Quaternion.Euler(0f, Random.Range(0, 180), 0f);
            //obstacles[i] = (GameObject)Instantiate(obstaclePrefab, pos, rot);
            //}

            // create the walls
            //for (int i = 0; i < walls.Length; i++)
            //{
                //pos = new Vector3(Random.Range(-30, 30), 1f, Random.Range(-30, 30));
                //Quaternion rot = Quaternion.Euler(0f, Random.Range(0, 180), 0f);
                //walls[i] = (GameObject)Instantiate(wallPrefab, pos, rot);
            //}
        
	}
	

	void Update () {
        
        //In Update, code to cycle through your cameras
        if (Input.GetKeyDown(KeyCode.C)) //can be any key you want
        {
            currentCameraIndex++;
            Debug.Log("C button has been pressed. Switching to the next camera");
            if (currentCameraIndex < cameras.Length)
            {
                cameras[currentCameraIndex - 1].gameObject.SetActive(false);
                cameras[currentCameraIndex].gameObject.SetActive(true);
                Debug.Log("Camera with name: " + cameras[currentCameraIndex].GetComponent<Camera>().name + ", is now enabled");
            }
            else
            {
                cameras[currentCameraIndex - 1].gameObject.SetActive(false);
                currentCameraIndex = 0;
                cameras[currentCameraIndex].gameObject.SetActive(true);
                Debug.Log("Camera with name: " + cameras[currentCameraIndex].GetComponent<Camera>().name + ", is now enabled");
            }
        }
        
        // calculate the barbarian centroid
        CalcBarbCentroid();

        // calculate the barbarian direction
        CalcBarbDirection();

        // if the legion has reached its waypoint then attack!
        if (leader.GetComponent<Seeker>().PathEnded == true)
        {
            for (int i = 0; i < numFlockers; i++)
            {
                // kill the legionnaires!
                barbFlock[i].GetComponent<Barbarian>().seekerTarget = flock[i];

                // if the barbarians get close enough then fighting!
                // eh, doesn't work right for now
                /*
                if(Vector3.Distance(barbFlock[i].transform.position, barbFlock[i].GetComponent<Barbarian>().seekerTarget.transform.position) < 3)
                { 
                    // roll to see who wins!
                    float killChance = Random.Range(0, 1);

                    if (killChance >= 0.5)
                    {
                        barbFlock.RemoveAt(i);
                    }
                    else
                    {
                        flock.RemoveAt(i);

                        barbFlock[i].GetComponent<Barbarian>().seekerTarget = leader;
                    }
                }
                 * */
            }
        }

        // calculate the centroid
        CalcCentroid();

        // calculate the ranks
        CalcRanks();

        // calculate the average flock direction
        CalcFlockDirection();

		// find and compare the distance between the guy and the noodle
		// if the noodle is too close then move it
		float dist = Vector3.Distance (target.transform.position, leader.transform.position);
        
		// randomize the noodles position
		if (dist < 5f) {
			target.transform.position = new Vector3(Random.Range(-30, 30), 4f, Random.Range(-30, 30));
		}

		for (int i = 0; i < numBarbFlockers; i++) {
			// find the distance from one flocker to the noodle
			float flockerDist = Vector3.Distance(target.transform.position, barbFlock[i].transform.position);

			// randomize the noodles position
            if (flockerDist < 5f && pos1 == true)
            {
                target.transform.position = new Vector3(381.1f, 4f, 230.4f);
                pos1 = false;
			}
            else if (flockerDist < 5f && pos1 == false)
            {
                target.transform.position = new Vector3(394f, 4.0f, 77.8f);
                pos1 = true;
			}
		}
		// change the noodle's location if it is too close
		//if (dist < 5f) {
			//do {
				//target.transform.position = new Vector3 (Random.Range (-30, 30), 4f, Random.Range (-30, 30));
			//} while(NearAnObstacle());
		//}
         
	}
        

    // These will be for calculating the barbarians flock directinon and centroid
    /// <summary>
    /// Calculates the centroid.
    /// sets the GameManagerGO object's position to the center
    /// </summary>
    private void CalcBarbCentroid()
    {
        // average up all of the positions
        // check to see if it will work when x, y and z coordinates are averaged

        // set variables for the centers values
        float totalX = 0;
        float totalZ = 0;

        // add up both x and y positions
        for (int i = 0; i < numBarbFlockers; i++)
        {
            // average up all of the x values and assign them to the totalX attribute
            totalX = barbFlock[i].transform.position.x + totalX;

            // average up all of the z values and assign them to the totalZ attribute
            totalZ = barbFlock[i].transform.position.z + totalZ;
        }

        // assign the average of the totalX and totalZ attributes to the centroid
        barbCentroid.x = totalX / numBarbFlockers;
        barbCentroid.y = 1.18f;
        barbCentroid.z = totalZ / numBarbFlockers;

        // set the game manager's position to that of the centroid
        barbCentroidObject.transform.position = barbCentroid;
    }

    /// <summary>
    /// Calculates the flock direction.
    /// Sets the GameManagerGO object's forward to the average flock direction
    /// </summary>
    private void CalcBarbDirection()
    {
        // average up all of the positions
        // check to see if it will work when x, y and z coordinates are averaged

        // set variables for the centers values
        Vector3 totalForward = new Vector3(0, 0, 0);

        // add up both x and y positions
        for (int i = 0; i < numBarbFlockers; i++)
        {
            totalForward = barbFlock[i].transform.forward + totalForward;
        }

        barbFlockDirection = totalForward / numBarbFlockers;

        barbFlockDirection.Normalize();
        // assign the average of the totalX and totalZ attributes to the centroid
        //flockDirection.x = totalX / numFlockers;
        //flockDirection.y = 0f;
        //flockDirection.z = totalZ / numFlockers;

        // set the game manager's position to that of the centroid
        barbCentroidObject.transform.forward = barbFlockDirection;
    }
    
    //-----------------------------------------------------------------------
    // Ranking Methods
    //-----------------------------------------------------------------------

    ///<summary>
    /// Calculates the center of each rank
    /// Also should calculate where each position on the rank is
    ///</summary>
    ///once the leader reaches this position
    ///rank 3 centroid pos: 340.09, 1.08, 312.66
    ///rot: Y:-.9935, W:-.1135
    ///rank 1 centroid pos: 363.98, 1.08, 317.99
    ///rot: Y:-.9935, W:-.1135
    ///rank 2 centroid pos: 388.64, 1.08, 323.7
    ///Rot; Y:-.9935, W:


    private void CalcRanks()
    {
        if (leader.GetComponent<Seeker>().PathEnded == false)
        {
            // determine the center of the first rank 
            rank1Centroid = leader.transform.position - (leader.transform.forward * (rankDist));

            rank1Object.transform.rotation = leader.transform.rotation;
        }
        else
        {
            rank1Centroid = new Vector3(363.98f, 1.08f, 317.99f);

            rank1Object.transform.rotation = new Quaternion(0f, -.9935f, 0f, -.1135f);
        }
        // assign the rank object to this position
        rank1Object.transform.position = rank1Centroid;

        // determine the position of the second rank
        if (leader.GetComponent<Seeker>().PathEnded == false)
        {
            rank2Centroid = leader.transform.position - (leader.transform.forward * (2 * rankDist));

            rank2Object.transform.rotation = leader.transform.rotation;
        }
        else
        {
            rank2Centroid = new Vector3(388.64f, 1.08f, 323.7f);

            rank2Object.transform.rotation = new Quaternion(0f, -.9935f, 0f, -.1135f);
        }
        // assign the rank object to this position
        rank2Object.transform.position = rank2Centroid;

        // determine the position of the third rank
        if (leader.GetComponent<Seeker>().PathEnded == false)
        {
            rank3Centroid = leader.transform.position - (leader.transform.forward * (3 * rankDist));

            rank3Object.transform.rotation = leader.transform.rotation;
        }
        else
        {
            rank3Centroid = new Vector3(340.09f, 1.08f, 312.66f);

            rank3Object.transform.rotation = new Quaternion(0f, -.9935f, 0f, -.1135f);
        }
        // assign the rank object to this position
        rank3Object.transform.position = rank3Centroid;

        // this is the part where we calculate the individual positions on the ranks
        // oh god
        // first use the TransformPoint method to get the right vector from the rank object
        // this is the first point between on the rank
        // for loop to assign positions
        rank1Vector = rank1Object.transform.TransformPoint(Vector3.right * distBetweenRankers);

        // assign the first position in each rank to the centroid
        //positions[0] = rank1Centroid;

        //-------------------RANK 1------------------------------

        // this loop is for flockers on the right of the centroid
        for (int i = 0; i < flockersPerRank / 2; i++)
        {
            positions[i] = (rank1Object.transform.TransformPoint(Vector3.right * distBetweenRankers * (i + 1)));

            // set the ranker's rank pos variable to the rank pos position
            flock[i].GetComponent<Vehicle>().RankPos = positions[i];

            //Debug.DrawLine(rank1Centroid, positions[i]);
        }

        // if the flockers per rank is not an even number
        if (flockersPerRank % 2 != 0)
        {
            // essentially assign the left hand positions to all but one flocker
            for (int i = flockersPerRank / 2; i < flockersPerRank - 1; i++)
            {
                positions[i] = (rank1Object.transform.TransformPoint(Vector3.right * -distBetweenRankers * (i + 1)));

                // set the ranker's rank pos variable to the rank pos position
                flock[i].GetComponent<Vehicle>().RankPos = positions[i];

                //Debug.DrawLine(rank1Centroid, positions[i]);
            }

            // assign the last flocker to the centroid's position
            positions[flockersPerRank - 1] = rank1Centroid;

            flock[flockersPerRank - 1].GetComponent<Vehicle>().RankPos = positions[flockersPerRank - 1]; 
        }
        else
        {
            // essentially assign the left hand positions to the rest of the flockers
            for (int i = flockersPerRank / 2; i < flockersPerRank; i++)
            {
                positions[i] = (rank1Object.transform.TransformPoint(Vector3.right * -distBetweenRankers * (i - 1)));

                // set the ranker's rank pos variable to the rank pos position
                flock[i].GetComponent<Vehicle>().RankPos = positions[i];

                //Debug.DrawLine(rank1Centroid, positions[i]);
            }
        }

        //--------------------RANK 2-------------------------

        // if the flockers per rank is not an even number
        if (flockersPerRank % 2 != 0)
        {
            // this loop is for flockers on the right of the centroid
            for (int i = flockersPerRank; i < 6; i++)
            {
                positions[i] = (rank2Object.transform.TransformPoint(Vector3.right * distBetweenRankers * (i - (flockersPerRank - 1))));

                // set the ranker's rank pos variable to the rank pos position
                flock[i].GetComponent<Vehicle>().RankPos = positions[i];

                //Debug.DrawLine(rank1Centroid, positions[i]);
            }

            //(numFlockers - flockersPerRank) - ((flockersPerRank / 2) + 1)

            // essentially assign the left hand positions to all but one flocker
            for (int i = 6; i < flockersPerRank * 2; i++)
            {
                positions[i] = (rank2Object.transform.TransformPoint(Vector3.right * -distBetweenRankers * (i - (flockersPerRank + 1))));

                // set the ranker's rank pos variable to the rank pos position
                flock[i].GetComponent<Vehicle>().RankPos = positions[i];

                //Debug.DrawLine(rank1Centroid, positions[i]);
            }

            // assign the last flocker to the centroid's position
            positions[(flockersPerRank * 2) - 1] = rank2Centroid;

            flock[(flockersPerRank * 2) - 1].GetComponent<Vehicle>().RankPos = positions[(flockersPerRank * 2) - 1];
        }
        else
        {
            // this loop is for flockers on the right of the centroid
            for (int i = flockersPerRank; i < 6; i++)
            {
                positions[i] = (rank2Object.transform.TransformPoint(Vector3.right * distBetweenRankers * (i - (flockersPerRank - 1))));

                // set the ranker's rank pos variable to the rank pos position
                flock[i].GetComponent<Vehicle>().RankPos = positions[i];

                //Debug.DrawLine(rank1Centroid, positions[i]);
            }
            // essentially assign the left hand positions to the rest of the flockers
            for (int i = 6; i < flockersPerRank * 2; i++)
            {
                positions[i] = (rank2Object.transform.TransformPoint(Vector3.right * -distBetweenRankers * (i - (flockersPerRank + 1))));

                // set the ranker's rank pos variable to the rank pos position
                flock[i].GetComponent<Vehicle>().RankPos = positions[i];

                //Debug.DrawLine(rank1Centroid, positions[i]);
            }
        }

        //--------------------RANK 3---------------------------

        // if the flockers per rank is not an even number
        if (flockersPerRank % 2 != 0)
        {
            // this loop is for flockers on the right of the centroid
            for (int i = flockersPerRank; i < numFlockers - ((flockersPerRank / 2) + 1); i++)
            {
                positions[i] = (rank3Object.transform.TransformPoint(Vector3.right * distBetweenRankers * (flockersPerRank - 1)));

                // set the ranker's rank pos variable to the rank pos position
                flock[i].GetComponent<Vehicle>().RankPos = positions[i];

                //Debug.DrawLine(rank1Centroid, positions[i]);
            }

            // essentially assign the left hand positions to all but one flocker
            for (int i = numFlockers - ((flockersPerRank / 2) + 1); i < numFlockers; i++)
            {
                positions[i] = (rank3Object.transform.TransformPoint(Vector3.right * -distBetweenRankers * (flockersPerRank + 1)));

                // set the ranker's rank pos variable to the rank pos position
                flock[i].GetComponent<Vehicle>().RankPos = positions[i];

                //Debug.DrawLine(rank1Centroid, positions[i]);
            }

            // assign the last flocker to the centroid's position
            positions[(numFlockers)] = rank3Centroid;

            flock[(numFlockers)].GetComponent<Vehicle>().RankPos = positions[(numFlockers)];
        }
        else
        {
            // this loop is for flockers on the right of the centroid
            for (int i = flockersPerRank * 2; i < numFlockers - (flockersPerRank / 2); i++)
            {
                positions[i] = (rank3Object.transform.TransformPoint(Vector3.right * distBetweenRankers * (i - (flockersPerRank + 3))));

                // set the ranker's rank pos variable to the rank pos position
                flock[i].GetComponent<Vehicle>().RankPos = positions[i];

                //Debug.DrawLine(rank1Centroid, positions[i]);
            }
            // essentially assign the left hand positions to the rest of the flockers
            for (int i = numFlockers - (flockersPerRank / 2); i < numFlockers; i++)
            {
                positions[i] = (rank3Object.transform.TransformPoint(Vector3.right * -distBetweenRankers * (i -(flockersPerRank + 5))));

                // set the ranker's rank pos variable to the rank pos position
                flock[i].GetComponent<Vehicle>().RankPos = positions[i];

                //Debug.DrawLine(rank1Centroid, positions[i]);
            }
        }
    }

    //-----------------------------------------------------------------------
    // Flocking Methods
    //-----------------------------------------------------------------------

	/// <summary>
	/// Calculates the centroid.
	/// sets the GameManagerGO object's position to the center
	/// </summary>
	private void CalcCentroid() {
        // average up all of the positions
        // check to see if it will work when x, y and z coordinates are averaged

        // set variables for the centers values
        float totalX = 0;
        float totalZ = 0;

        // add up both x and y positions
        for(int i = 0; i < numFlockers; i++)
        {
            // average up all of the x values and assign them to the totalX attribute
            totalX = flock[i].transform.position.x + totalX;

            // average up all of the z values and assign them to the totalZ attribute
            totalZ = flock[i].transform.position.z + totalZ;
        }

        // assign the average of the totalX and totalZ attributes to the centroid
        centroid.x = totalX / numFlockers;
        centroid.y = 1.18f;
        centroid.z = totalZ / numFlockers;

        // set the game manager's position to that of the centroid
        this.transform.position = centroid;
	}

	/// <summary>
	/// Calculates the flock direction.
	/// Sets the GameManagerGO object's forward to the average flock direction
	/// </summary>
	private void CalcFlockDirection() {
        // average up all of the positions
        // check to see if it will work when x, y and z coordinates are averaged

        // set variables for the centers values
        Vector3 totalForward = new Vector3(0, 0, 0);

        // add up both x and y positions
        for (int i = 0; i < numFlockers; i++)
        {
            totalForward = flock[i].transform.forward + totalForward;
        }

        flockDirection = totalForward / numFlockers;

        flockDirection.Normalize();
        // assign the average of the totalX and totalZ attributes to the centroid
        //flockDirection.x = totalX / numFlockers;
        //flockDirection.y = 0f;
        //flockDirection.z = totalZ / numFlockers;

        // set the game manager's position to that of the centroid
        this.transform.forward = flockDirection;
	}

}
