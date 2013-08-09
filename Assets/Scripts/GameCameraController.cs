using UnityEngine;
using System.Collections;

public class GameCameraController : MonoBehaviour {
	
	public float orthographicSize;
	public float cameraResetDuration;
	
	private bool isDeathCamera;
	
	public void SetDeathCameraActive ( bool trueOrFalse ) {
		if ( trueOrFalse ) {
			Go.to( screenFurnitureContainer , 0.05f , new GoTweenConfig().position( new Vector3( 0 , 110 , 0 ) , false ).setEaseType( GoEaseType.BackIn ) );
			Go.to( screenFurnitureContainer , 0.025f , new GoTweenConfig().scale( Vector3.one * 2 , false ).setEaseType( GoEaseType.ExpoIn ) );
			Vector3 cameraTargetPoint = bufferedTargetPosition - camera.ScreenToWorldPoint( Input.mousePosition );
			cameraTargetPoint.z = -thisTransform.position.z;
			Quaternion cameraRotationTarget = Quaternion.LookRotation( cameraTargetPoint );
			//Go.to( thisTransform , 0.025f , new GoTweenConfig().rotation( cameraRotationTarget , false ).setEaseType( GoEaseType.ExpoInOut ) ).setOnCompleteHandler( deathCam => isDeathCamera = true );
			isDeathCamera = true;
		} else {
			Go.killAllTweensWithTarget( camera );
			Go.to( camera , cameraResetDuration , new GoTweenConfig().floatProp( "orthographicSize" , orthographicSize , false ).setEaseType( GoEaseType.ExpoInOut ) );
			Go.to( screenFurnitureContainer , 1 , new GoTweenConfig().position( Vector3.zero , false ).setEaseType( GoEaseType.BackOut ) );
			Go.to( screenFurnitureContainer , 0.25f , new GoTweenConfig().scale( Vector3.one , false ).setEaseType( GoEaseType.BackOut ) );
			isDeathCamera = false;
		}
	}
	
	public float deathCameraMovementSpeed;
	public float deathCameraRotationSpeed;
	public float deathCameraDistance;
	
	private Vector3 newPosition;
	
	private void DeathCamera () {
		Quaternion bufferedRotation = thisTransform.rotation;
		Quaternion newRotation = Quaternion.Slerp( bufferedRotation , Quaternion.LookRotation( bufferedTargetPosition - camera.ScreenToWorldPoint( Input.mousePosition ) ) , Time.deltaTime * deathCameraRotationSpeed );
		thisTransform.rotation = newRotation;
	}
	
	public float deadZoneRadius;
	public float cameraPaddingDistance;
	public float cameraVelocityMultiplier;
	public float dampeningFactor;
	private Vector3 directionToTarget;
	private Vector3 desiredPosition;
	
	private GameObject target;
	
	private void FollowCamera () {
		if ( target != null ) {
			if ( targetTransform == null ) targetTransform = target.transform;
			directionToTarget = new Vector3( targetTransform.position.x , targetTransform.position.y , 0 ) - new Vector3( thisTransform.position.x , thisTransform.position.y + cameraHeight , 0 );
			if ( Vector2.Distance( targetTransform.position, thisTransform.position ) > deadZoneRadius ) desiredPosition = new Vector3( directionToTarget.x * ( ( Vector2.Distance(targetTransform.position , thisTransform.position ) ) / ( directionToTarget.magnitude ) ) , directionToTarget.y * ( ( Vector2.Distance( targetTransform.position , thisTransform.position ) ) / ( directionToTarget.magnitude ) ) , -cameraDistance );
			directionToTarget *= cameraVelocityMultiplier;
			if ( Vector2.Distance( targetTransform.position, transform.position ) > cameraPaddingDistance ) thisTransform.position = Vector3.SmoothDamp( thisTransform.position , desiredPosition , ref directionToTarget , Vector2.Distance(targetTransform.position , thisTransform.position ) * dampeningFactor );
		}
	}
	
	public float cameraDistance;
	public float cameraHeight;
	
	private Transform thisTransform;
	private Transform targetTransform;
	private Transform cursorTransform;
	private Transform uiContainer;
	private Transform screenFurnitureContainer;
	
	// Use this for initialization
	void Start () {
		target = GameObject.FindGameObjectWithTag("Avatar");
		targetTransform = target.transform;
		thisTransform = transform;
		cursorTransform = GameObject.FindGameObjectWithTag( "Cursor" ).transform;
		if ( target != null ) thisTransform.position = new Vector3( targetTransform.position.x , targetTransform.position.y - cameraHeight, -cameraDistance );
		uiContainer = thisTransform.GetChild( 1 );
		screenFurnitureContainer = uiContainer.GetChild( 1 );
		Go.to( screenFurnitureContainer , 1 , new GoTweenConfig().position( Vector3.zero , false ).setEaseType( GoEaseType.BackOut ) );
		Go.to( camera , cameraResetDuration , new GoTweenConfig().floatProp( "orthographicSize" , orthographicSize , false ).setEaseType( GoEaseType.ExpoInOut ) );
	}
	
	private Vector3 bufferedTargetPosition;
	
	// Update is called once per frame
	void Update () {
		if ( target != null && !isDeathCamera ) {
				if ( targetTransform != null ) bufferedTargetPosition = targetTransform.position;
				FollowCamera();
		} else if ( isDeathCamera ) DeathCamera();
		if ( !isDeathCamera && target == null ) target = GameObject.FindGameObjectWithTag("Avatar");
		if ( !isDeathCamera && cursorTransform == null ) cursorTransform = GameObject.FindGameObjectWithTag( "Cursor" ).transform;


	}
}
