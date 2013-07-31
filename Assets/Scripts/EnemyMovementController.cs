using UnityEngine;
using System.Collections;

public class EnemyMovementController : MonoBehaviour {
	
	public float timeUntilMovement = 0.5f;
	
	IEnumerator WaitToEnableMovement() {
		yield return new WaitForSeconds( timeUntilMovement );
		EnableMovement( true );
		Debug.Log( "Movement enabled" );
	}

	private Transform thisTransform;
	private Transform meshContainer;
	private EnemyScaleController scaleControllerScript;
	private EnemyMeshColorController colorControllerScript;
	private GameObject nearestArena;
	private Transform arenaTransform;
	private GameObject avatar;
	private Transform avatarTransform;
	private Rigidbody thisRigidbody;
	
	void Awake () {
		thisTransform = transform;
		meshContainer = thisTransform.GetChild( 0 );
		scaleControllerScript = meshContainer.GetComponent< EnemyScaleController >();
		colorControllerScript = meshContainer.GetComponent< EnemyMeshColorController >();
		thisRigidbody = rigidbody;
		avatar = GameObject.FindGameObjectWithTag( "Avatar" );
		if ( avatar != null ) avatarTransform = avatar.transform;
		bufferedDirection = Vector3.Normalize( avatar.transform.position - transform.position );
		StartCoroutine( WaitToEnableMovement() );
	}
	
	private bool isMovementEnabled = false;
	
	public void EnableMovement( bool trueOrFalse ) {
		if ( trueOrFalse ) isMovementEnabled = true;
		else isMovementEnabled = false;
	}
	
	public float movementSpeed;
	public float minGravityMagnitude;
	public float maxGravityMagnitude;
	public float gravityCoefficient;
	public float breachForceMultiplier;
	public float breachArcSpeedMultiplier;
	public float maxBreachSpeed;
	public float outsideCircleMovementSpeedMultiplier;
	
	private Vector2 gravityForce;
	private Vector2 bufferedDirection;
	
	private void outsideArenaMovement ( Vector2 direction ) {
		Vector2 directionToCenter = arenaTransform.position - thisTransform.position;
		directionToCenter.Normalize();
		float gravityMagnitude = ( Mathf.Clamp( distanceToSurface / ( Mathf.Pow( distanceToSurface , 2 ) ), minGravityMagnitude, maxGravityMagnitude ) ) * gravityCoefficient;
		float componentTowardCentre = Vector2.Dot( direction , directionToCenter );
		Vector2 tangentialInputOnly = direction - ( directionToCenter * componentTowardCentre );
		directionToCenter /= gravityMagnitude;
		gravityForce = Vector2.ClampMagnitude( bufferedDirection * ( ( Time.deltaTime * movementSpeed ) * breachArcSpeedMultiplier ), maxBreachSpeed );
		gravityForce += directionToCenter * ( ( Time.deltaTime * movementSpeed ) * breachArcSpeedMultiplier );
		rigidbody.AddForce(new Vector3(gravityForce.x, gravityForce.y, 0));
		rigidbody.AddForce( ( tangentialInputOnly * ( Time.deltaTime * movementSpeed ) ) * outsideCircleMovementSpeedMultiplier );
		rigidbody.AddForce((direction * (Time.deltaTime * movementSpeed / 2)) * outsideCircleMovementSpeedMultiplier);
	}
	
	public GameObject breachEffect;

	private float distanceToSurface;
	private GameObject currentBreachEffect;
	private bool hasBreached = true;
	private bool insideCircle = false;
	private void HandleMovement (Vector2 direction) {
		
		if ( insideCircle ) {
			rigidbody.AddForce( new Vector3( direction.x * ( Time.deltaTime * movementSpeed ), direction.y * ( Time.deltaTime * movementSpeed ), 0 ) );
			if ( !hasBreached && distanceToSurface + meshContainer.localScale.x > 0 ) {
				hasBreached = true;
				bufferedDirection = direction;
				thisRigidbody.AddForce( ( bufferedDirection * ( movementSpeed * Time.deltaTime ) ) * breachForceMultiplier );
				currentBreachEffect = Instantiate( breachEffect , thisTransform.position + thisRigidbody.velocity.normalized * meshContainer.localScale.x , Quaternion.LookRotation( -thisRigidbody.velocity.normalized ) ) as GameObject;
				currentBreachEffect.transform.parent = thisTransform.parent;
			} else if ( hasBreached && distanceToSurface - meshContainer.localScale.x > 0 ) {
				insideCircle = false;
			} else if ( !hasBreached && distanceToSurface + meshContainer.localScale.x > 0 ) {
				thisRigidbody.AddForce( ( bufferedDirection * ( movementSpeed * Time.deltaTime ) ) * breachForceMultiplier );
				currentBreachEffect = Instantiate( breachEffect , thisTransform.position + thisRigidbody.velocity.normalized * meshContainer.localScale.x , Quaternion.LookRotation( -thisRigidbody.velocity.normalized ) ) as GameObject;
				currentBreachEffect.transform.parent = thisTransform.parent;
				insideCircle = true;
				hasBreached = false;
			}
		} else if ( !insideCircle ) {
			outsideArenaMovement( direction );
			if ( hasBreached && distanceToSurface - meshContainer.localScale.x < 0 ) {
				hasBreached = false;
				bufferedDirection = Vector2.zero;
				gravityForce = Vector2.zero;
				currentBreachEffect = Instantiate( breachEffect , thisTransform.position + thisRigidbody.velocity.normalized * meshContainer.localScale.x , Quaternion.LookRotation( -thisRigidbody.velocity.normalized ) ) as GameObject;
				currentBreachEffect.transform.parent = thisTransform.parent;
			} else if ( !hasBreached && distanceToSurface + meshContainer.localScale.x < 0 ) {
				insideCircle = true;
			} else if ( hasBreached && distanceToSurface - meshContainer.localScale.x < 0 ) {
				insideCircle = false;
				currentBreachEffect = Instantiate( breachEffect , thisTransform.position + thisRigidbody.velocity.normalized * meshContainer.localScale.x , Quaternion.LookRotation( -thisRigidbody.velocity.normalized ) ) as GameObject;
				currentBreachEffect.transform.parent = thisTransform.parent;
				hasBreached = true;
			}
		}
	}
	
	public float generationSpeedBonus;
	
	private float generation;
	private GameObject gameContainer;
	private GameController gameControllerScript;
	
	// Use this for initialization
	void Start () {
		gameContainer = GameObject.FindGameObjectWithTag( "GameContainer" );
		gameControllerScript = gameContainer.GetComponent< GameController >();
		nearestArena = gameControllerScript.FindNearestArena( gameObject );
		arenaTransform = nearestArena.transform;
		generation = scaleControllerScript.generation;
		movementSpeed += ( generationSpeedBonus * generation );
	}
	
	// Update is called once per frame
	void Update () {
		if ( Time.time > avatarPositionTimer ) {
			if ( avatarTransform != null ) movementDirection = Vector3.Normalize(avatarTransform.position - thisTransform.position);
			avatarPositionTimer = Time.time + positionPollTimer;
		}
	
	}
	
	public float positionPollTimer;
	private float avatarPositionTimer;
	private Vector2 directionToArena;
	private Quaternion bufferedRotation;
	private Vector3 movementDirection;
	
	void FixedUpdate () {
		if ( isMovementEnabled ) {
			nearestArena = gameControllerScript.FindNearestArena( gameObject );
			if ( nearestArena != null ) arenaTransform = nearestArena.transform;
			else {
				nearestArena = gameControllerScript.FindNearestArena( gameObject );
				if ( nearestArena != null ) arenaTransform = nearestArena.transform;
			}
			if ( arenaTransform != null ) {
				directionToArena = arenaTransform.position - thisTransform.position;
				distanceToSurface = directionToArena.magnitude - arenaTransform.localScale.x;
				HandleMovement(movementDirection);
			}
		}
	}
}
