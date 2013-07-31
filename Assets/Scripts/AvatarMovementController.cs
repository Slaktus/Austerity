using UnityEngine;
using System.Collections;

public class AvatarMovementController : MonoBehaviour {
	
	private Transform thisTransform;
	private Rigidbody thisRigidbody;
	private GameObject gameContainer;
	private GameObject mainCamera;
	private Transform cameraTransform = null;
	
	void Awake () {
		bufferedRotation = Quaternion.AngleAxis( -90 , Vector3.right );
		transform.rotation = bufferedRotation;
		thisTransform = transform;
		thisRigidbody = rigidbody;
		gameContainer = GameObject.FindGameObjectWithTag( "GameContainer" );
		mainCamera = GameObject.FindGameObjectWithTag( "MainCamera" );
		cameraTransform = mainCamera.transform;
	}
	
	public float inputLowerThreshold;
	public float inputUpperThreshold;
	private Vector2 rawInput;
	
	private Vector2 InputToMovementDirection () {
		rawInput.x = Input.GetAxisRaw( "Horizontal" );
		rawInput.y = Input.GetAxisRaw( "Vertical" );
		if ( Mathf.Abs( rawInput.x ) > inputLowerThreshold ) rawInput.x = Mathf.Clamp( rawInput.x , -inputUpperThreshold , inputUpperThreshold );
		else rawInput.x = 0;
		if (Mathf.Abs( rawInput.y ) > inputLowerThreshold ) rawInput.y = Mathf.Clamp( rawInput.y , -inputUpperThreshold , inputUpperThreshold );
		else rawInput.y = 0;
		if (Mathf.Abs(rawInput.sqrMagnitude) > 0) movementDirection = rawInput.normalized;
		else movementDirection = Vector2.zero;
		return movementDirection;
	}
	
	public GameObject breachEffect;
	public float movementSpeed;
	public float movementDrag;
	public float stationaryDrag;
	public float minGravityMagnitude;
	public float maxGravityMagnitude;
	public float gravityCoefficient;
	public float breachForceMultiplier;
	public float breachArcSpeedMultiplier;
	public float maxBreachSpeed;
	public float tangentialMovementSpeedMultiplier;
	public float dodgeSpeedMultiplier;
	
	private Vector2 vectorToCenter;
	private Vector2 gravityForce;
	private Vector2 bufferedDirection;
	private Vector2 velocityModifier;
	private float distanceToSurface;
	private bool isDodging = false;
	private bool hasBreached = false;
	private GameObject currentBreachEffect;
	
	private void HandleMovement ( Vector2 direction ) {
		if ( distanceToSurface > 0 ) {
			if ( bufferedDirection == Vector2.zero ) {
				if ( !hasBreached ) {
					currentBreachEffect = Instantiate( breachEffect , thisTransform.position , Quaternion.LookRotation( -thisRigidbody.velocity.normalized ) ) as GameObject;
					currentBreachEffect.transform.parent = thisTransform.parent;
					hasBreached = true;
				}
				if ( direction == Vector2.zero ) {
					velocityModifier = arenaTransform.position - thisTransform.position;
					rigidbody.velocity = velocityModifier * dodgeSpeedMultiplier;
					rigidbody.AddForce( velocityModifier.normalized * movementSpeed );
					direction = velocityModifier.normalized;
					isDodging = true;
				} else if ( !isDodging ) {
					bufferedDirection = rigidbody.velocity.normalized;
					rigidbody.AddForce( ( bufferedDirection * movementSpeed ) * breachForceMultiplier , ForceMode.Acceleration );
				}
			}
			Vector2 directionToCenter = arenaTransform.position - thisTransform.position;
			directionToCenter.Normalize();
			float gravityMagnitude = ( Mathf.Clamp( distanceToSurface / ( Mathf.Pow( distanceToSurface , 2 ) ), minGravityMagnitude , maxGravityMagnitude ) ) * gravityCoefficient;
			float componentTowardCentre = Vector2.Dot( direction , directionToCenter );
			Vector2 tangentialInputOnly = direction - ( directionToCenter * componentTowardCentre );
			directionToCenter /= gravityMagnitude;
			gravityForce = Vector2.ClampMagnitude( bufferedDirection * ( movementSpeed * breachArcSpeedMultiplier ) , maxBreachSpeed );
			gravityForce += directionToCenter * ( movementSpeed * breachArcSpeedMultiplier );
			rigidbody.AddForce( new Vector3( gravityForce.x , gravityForce.y , 0 ) );
			if ( !isDodging ) {
				rigidbody.AddForce( ( tangentialInputOnly * movementSpeed ) * tangentialMovementSpeedMultiplier , ForceMode.Acceleration );
				nearestArena = gameControllerScript.FindNearestArena( gameObject );
				arenaTransform = nearestArena.transform;
			}
		} else {
			nearestArena = gameControllerScript.FindNearestArena( gameObject );
			arenaTransform = nearestArena.transform;
			if ( bufferedDirection != Vector2.zero ) {
				if ( hasBreached ) {
					currentBreachEffect = Instantiate( breachEffect , thisTransform.position , Quaternion.LookRotation( -thisRigidbody.velocity.normalized ) ) as GameObject;
					currentBreachEffect.transform.parent = thisTransform.parent;
				}
				bufferedDirection = Vector2.zero;
				gravityForce = Vector2.zero;
			}
			if ( Mathf.Abs( direction.sqrMagnitude ) > 0) rigidbody.drag = movementDrag;
			else rigidbody.drag = stationaryDrag;
			rigidbody.AddForce( new Vector3( direction.x * movementSpeed, direction.y * movementSpeed, 0 ) );
			if ( isDodging ) isDodging = false;
			if ( hasBreached ) hasBreached = false;
		}
	}
	
	private GameController gameControllerScript;
	private GameObject nearestArena;
	private Transform arenaTransform;
	
	void Start () {
		gameControllerScript = gameContainer.GetComponent< GameController >();
		nearestArena = gameControllerScript.FindNearestArena( gameObject );
		arenaTransform = nearestArena.transform;
	}
	
	private Vector2 movementDirection;
	private string chamberType;
	
	// Update is called once per frame
	void Update () {
	}
	
	private Vector2 directionToArena;
	private Quaternion bufferedRotation;
	
	void FixedUpdate () {
		movementDirection = InputToMovementDirection();
		if ( arenaTransform != null ) {
			directionToArena = arenaTransform.position - thisTransform.position;
			distanceToSurface = directionToArena.magnitude - ( arenaTransform.localScale.x );
			HandleMovement( movementDirection );
			if ( thisRigidbody.velocity != Vector3.zero ) {
				thisTransform.rotation = Quaternion.LookRotation( thisRigidbody.velocity );
				bufferedRotation = thisTransform.rotation;
			} else thisTransform.rotation = bufferedRotation;
			//cameraTransform.rotation = Quaternion.Lerp( cameraTransform.rotation , Quaternion.LookRotation( Vector3.Normalize( thisTransform.position - cameraTransform.position ) ) , Time.deltaTime * 0.05f );
		} else {
			nearestArena = gameControllerScript.FindNearestArena( gameObject );
			if ( nearestArena != null ) arenaTransform = nearestArena.transform;
		}
	}
}
