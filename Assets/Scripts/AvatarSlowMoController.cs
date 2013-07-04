using UnityEngine;
using System.Collections;

public class AvatarSlowMoController : MonoBehaviour {
	
	public float minGrazeTime = 2f;
	public float grazeCooldownDuration = 5f;
	public float zoomInDuration = 0.075f;
	public float zoomOutDuration = 0.6f;
	public float zoomSize = 25f;
	public float zoomTimeScale = 0.1f;
	public float zoomFixedDelta = 0.002f;
	
	private Transform hitTransform;
	private bool isSlowMoActive;
	private Vector3 hitPosition;
	private float grazeTime;
	private float grazeCooldownTime;
	private Transform grazedEnemy;
	
	void OnTriggerEnter ( Collider collision ) {
		hitTransform = collision.transform;
		if ( !isSlowMoActive && hitTransform.parent != null && hitTransform.parent.name == "Enemy" && Time.time > grazeCooldownTime ) {
			Go.to( mainCamera.camera , zoomInDuration , new GoTweenConfig().floatProp( "orthographicSize" , zoomSize , false ).setEaseType( GoEaseType.BackIn ) );
			hitPosition = collision.ClosestPointOnBounds( hitTransform.position );
			isSlowMoActive = true;
			Time.timeScale = zoomTimeScale;
			Time.fixedDeltaTime = zoomFixedDelta;
			grazeTime = Time.time;
			grazeCooldownTime = Time.time + grazeCooldownDuration;
			grazedEnemy = hitTransform;
		}
	}
	
	private bool readyToReset = false;
	
	void OnTriggerExit () {
		readyToReset = true;
	}

	
	private GameObject mainCamera;
	
	void Awake () {
		mainCamera = GameObject.FindGameObjectWithTag( "MainCamera" );
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	public float cameraRotationSpeed = 50;
	
	// Update is called once per frame
	void Update () {
		if ( isSlowMoActive ) {
				mainCamera.transform.rotation = Quaternion.Slerp( mainCamera.transform.rotation , Quaternion.LookRotation( Vector3.Normalize( hitPosition - mainCamera.transform.position ) ) , cameraRotationSpeed * Time.deltaTime );
			} else mainCamera.transform.rotation = Quaternion.Slerp( mainCamera.transform.rotation , Quaternion.LookRotation( Vector3.zero ) , ( cameraRotationSpeed * 2 ) * Time.deltaTime );
	
		if ( readyToReset && Time.time > grazeTime + minGrazeTime ) {
			isSlowMoActive = false;
			Time.timeScale = 1f;
			Time.fixedDeltaTime = 0.02f;
			Go.killAllTweensWithTarget( mainCamera );
			Go.to( mainCamera.camera , zoomOutDuration , new GoTweenConfig().floatProp( "orthographicSize" , 50 , false ).setEaseType( GoEaseType.BackOut ) );
			readyToReset = false;
		} else if (  grazedEnemy == null && isSlowMoActive ) {
			isSlowMoActive = false;
			Time.timeScale = 1f;
			Time.fixedDeltaTime = 0.02f;
			Go.killAllTweensWithTarget( mainCamera );
			Go.to( mainCamera.camera , zoomOutDuration , new GoTweenConfig().floatProp( "orthographicSize" , 50 , false ).setEaseType( GoEaseType.BackOut ) );
			readyToReset = false;
		}
	}
}
