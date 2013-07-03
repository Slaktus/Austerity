using UnityEngine;
using System.Collections;

public class CursorController : MonoBehaviour {
	
	private Transform thisTransform;
	
	void Awake () {
		thisTransform = transform;
		Screen.showCursor = false;
	}
	
	public GameObject swarm;
	public int numberOfSwarmMembers;
	private ArrayList swarmList = new ArrayList();
	
	private void SwarmManager () {
		if ( numberOfSwarmMembers > swarmList.Count )	{
			GameObject newSwarmMember = Instantiate( swarm , thisTransform.position , Quaternion.identity ) as GameObject;
			newSwarmMember.transform.parent = transform;
			swarmList.Add( newSwarmMember );
		}
		else if ( swarmList.Count > numberOfSwarmMembers ) {
			GameObject swarmMember = ( GameObject ) swarmList[ numberOfSwarmMembers ];
			swarmList.Remove( swarmMember );
			Destroy( swarmMember );
		}
	}
	
	private void RemoveSwarm () {
		foreach ( GameObject member in swarmList ) {
			swarmList.Remove( member );
			Destroy( member );
		}
	}
	
	public float defaultDistance = 2.75f;
	public float chaseSpeed;
	public float orbitSpeed;
	public float defaultSize;
	private float currentAngle;
	private Vector3 memberPosition;
	private Vector3 memberDirection;
	private Transform memberTransform;
	
	private void PositionSwarmOnCursor () {
		float angleIncrement = 360 / numberOfSwarmMembers;
		foreach (GameObject member in swarmList) {
			memberTransform = member.transform;
			if ( memberTransform.localScale.x >= defaultSize ) {
				memberTransform.localScale = Vector3.Lerp( memberTransform.localScale , Vector3.one , Time.deltaTime * chaseSpeed );
			}
			memberPosition = thisTransform.TransformPoint( Quaternion.AngleAxis( currentAngle , Vector3.forward ) * ( Vector3.up * defaultDistance ) );
			memberTransform.position = Vector3.Slerp( memberTransform.position , memberPosition , Time.deltaTime * chaseSpeed );
			memberDirection = memberTransform.position - thisTransform.position;
			memberTransform.rotation = Quaternion.LookRotation( memberDirection.normalized );
			currentAngle += angleIncrement;
		}
		currentAngle += orbitSpeed * Time.deltaTime;
	}
	
	public float cursorTargetSpeed;
	private float distanceFromTarget;
	public float selectSize;
	private float targetZOffset;
	
	private void SelectTarget ( Vector3 targetPosition ) {
		float angleIncrement = 360 / numberOfSwarmMembers;
		foreach ( GameObject member in swarmList ) {
			memberTransform = member.transform;
			if ( memberTransform.localScale.x < selectSize ) {
				memberTransform.localScale = Vector3.Lerp( memberTransform.localScale , Vector3.one * selectSize , Time.deltaTime * cursorTargetSpeed );
			}
			Vector3 target = targetPosition - mousePositionOnPlane;
			target += Quaternion.AngleAxis( currentAngle , Vector3.forward) * ( target.normalized * distanceFromTarget );
			target.z = targetZOffset;
			memberTransform.position = Vector3.Slerp( memberTransform.position - mousePositionOnPlane, target , Time.deltaTime * cursorTargetSpeed ) + mousePositionOnPlane;
			memberDirection = memberTransform.position - targetPosition;
			memberTransform.rotation = Quaternion.LookRotation( -memberDirection.normalized );
			currentAngle += angleIncrement;
		}
		currentAngle -= orbitSpeed * Time.deltaTime;
	}
	
	private GameObject mainCamera;
	private Transform cameraTransform;
	private GameObject gameContainer;
	private GameController gameControllerScript;
	private ArrayList enemyList;
	
	// Use this for initialization
	void Start () {
		gameContainer = GameObject.FindGameObjectWithTag( "GameContainer" );
		gameControllerScript = gameContainer.GetComponent< GameController >();
		mainCamera = GameObject.FindGameObjectWithTag( "MainCamera" );
		cameraTransform = mainCamera.transform;
		enemyList = gameControllerScript.GetEnemyList();
	}
	
	public float damageInterval = 0.1f;
	public float enemyThrowForce;
	public float arenaThrowForce;
	public float distanceFromPlane;
	public float targetOrbitDistance;
	public float cameraRotationSpeed;
	public float cameraRepositionDivider = 1.0f;
	public float dragTimeScale;
	public float dragFixedDeltaTime;
	public bool dragAffectsCamera;
	private Vector3 mousePosition;
	private GameObject nearestArena;
	private GameObject nearestEnemy;
	private GameObject nearestChamber;
	private Vector3 targetPositionOnPlane;
	private Vector3 mousePositionOnPlane;
	private float combinedRadii;
	private bool holdingTarget = false;
	private bool targetIsEnemy = false;
	private Vector3 targetRelativePosition;
	private Transform targetTransform;
	private Vector3 targetNewPosition;
	private float damageTimer;
	
	// Update is called once per frame
	void Update () {
		mousePosition = mainCamera.camera.ScreenToWorldPoint( Input.mousePosition );
		mousePosition.z = -distanceFromPlane;
		thisTransform.position = mousePosition;
		SwarmManager();
		if ( !holdingTarget && Input.GetKeyDown( KeyCode.Mouse1 ) ) {
			nearestEnemy = null;
			nearestArena = null;
			targetTransform = null;
			mousePositionOnPlane = new Vector3( thisTransform.position.x , thisTransform.position.y , thisTransform.position.z + distanceFromPlane );
			if ( enemyList.Count > 0 ) nearestEnemy = gameControllerScript.FindNearestEnemy( gameObject );
			if ( nearestEnemy != null ) {
				targetTransform = nearestEnemy.transform;
				targetPositionOnPlane = new Vector3( targetTransform.position.x , targetTransform.position.y , thisTransform.position.z );
				combinedRadii = targetTransform.localScale.x + defaultDistance;
			}
			if ( nearestEnemy != null && combinedRadii > Vector2.Distance( targetTransform.position , thisTransform.position ) ) {
				targetRelativePosition = targetPositionOnPlane - mousePositionOnPlane;
				targetTransform.SendMessageUpwards( "EnableMovement" , false );
				holdingTarget = true;
				targetIsEnemy = true;
				Time.timeScale = dragTimeScale;
				Time.fixedDeltaTime = dragFixedDeltaTime;
				targetZOffset = 0;
			} else {
				nearestArena = gameControllerScript.FindNearestArena( gameObject );
				targetTransform = nearestArena.transform;
				targetPositionOnPlane = new Vector3( targetTransform.position.x , targetTransform.position.y , thisTransform.position.z );
				combinedRadii = targetTransform.localScale.x + defaultDistance;
				if ( combinedRadii > Vector2.Distance( targetTransform.position , thisTransform.position ) ) {
					targetRelativePosition = targetPositionOnPlane - mousePositionOnPlane;
					holdingTarget = true;
					Time.timeScale = dragTimeScale;
					Time.fixedDeltaTime = dragFixedDeltaTime;
					targetZOffset = 0;
				} else {
					nearestChamber = gameControllerScript.FindNearestChamber( gameObject );
					targetTransform = nearestChamber.transform;
					targetPositionOnPlane = new Vector3( targetTransform.position.x , targetTransform.position.y , thisTransform.position.z );
					combinedRadii = targetTransform.localScale.x + defaultDistance;
					if ( combinedRadii > Vector2.Distance( targetTransform.position , thisTransform.position ) ) {
						targetRelativePosition = targetPositionOnPlane - mousePositionOnPlane;
						holdingTarget = true;
						Time.timeScale = dragTimeScale;
						Time.fixedDeltaTime = dragFixedDeltaTime;
						targetZOffset = 90;
					}
				}
			}
		}
		
		if ( targetTransform != null && holdingTarget && Input.GetKey( KeyCode.Mouse1 ) ) {
			if ( dragAffectsCamera ) cameraTransform.rotation = Quaternion.Slerp( cameraTransform.rotation , Quaternion.LookRotation( Vector3.Normalize( targetTransform.position - cameraTransform.position ) ) , cameraRotationSpeed * Time.deltaTime );
			distanceFromTarget = targetTransform.localScale.x + targetOrbitDistance;
			targetPositionOnPlane = new Vector3( targetTransform.position.x , targetTransform.position.y , thisTransform.position.z + distanceFromPlane );
			mousePositionOnPlane = new Vector3( thisTransform.position.x , thisTransform.position.y , thisTransform.position.z + distanceFromPlane );
			SelectTarget( targetPositionOnPlane );
			targetNewPosition = new Vector3( mousePositionOnPlane.x + targetRelativePosition.x , mousePositionOnPlane.y + targetRelativePosition.y , targetTransform.parent.transform.position.z );
			if ( targetIsEnemy ) {
				targetTransform.parent.position = targetNewPosition;
			} else {
				targetTransform.parent.transform.position = targetNewPosition;
				targetTransform.SendMessage( "GetColorFromChamber" , SendMessageOptions.DontRequireReceiver );
			}
		} else {
			PositionSwarmOnCursor();
			if ( dragAffectsCamera && cameraTransform.rotation.eulerAngles != Vector3.zero ) cameraTransform.rotation = Quaternion.Slerp( cameraTransform.rotation , Quaternion.identity , ( cameraRotationSpeed / cameraRepositionDivider ) * Time.deltaTime );
		}
		
		if ( targetTransform != null && holdingTarget && Input.GetKeyUp( KeyCode.Mouse1 ) ) {
			holdingTarget = false;
			Time.timeScale = 1;
			Time.fixedDeltaTime = 0.02f;
			if ( targetIsEnemy ) {
				targetTransform.SendMessageUpwards( "EnableMovement" , true );
				targetTransform.parent.rigidbody.AddForce( ( ( thisTransform.position + targetRelativePosition ) - targetNewPosition ) * enemyThrowForce );
				targetIsEnemy = false;
			} else {
				targetTransform.parent.rigidbody.AddForce( ( ( thisTransform.position + targetRelativePosition ) - targetNewPosition ) * arenaThrowForce );
			}
		} else if ( targetTransform == null && holdingTarget ) {
			targetIsEnemy = false;
			holdingTarget = false;
			Time.timeScale = 1;
			Time.fixedDeltaTime = 0.02f;
		}
	}
}
