using UnityEngine;
using System.Collections;

public class AvatarSlowMoController : MonoBehaviour {
	
	public float minGrazeTime = 2;
	public float grazeCooldownDuration = 5;
	
	private Transform hitTransform;
	private bool isSlowMoActive;
	private Vector3 hitPosition;
	private float grazeTime;
	private float grazeCooldownTime;
	
	void OnTriggerEnter ( Collider collision ) {
		hitTransform = collision.transform;
		if ( !isSlowMoActive && hitTransform.parent != null && hitTransform.parent.name == "Enemy" && Time.time > grazeCooldownTime ) {
			Go.to( mainCamera.camera , 0.075f , new GoTweenConfig().floatProp( "orthographicSize" , 25 , false ).setEaseType( GoEaseType.BackIn ) );
			hitPosition = collision.ClosestPointOnBounds( hitTransform.position );
			isSlowMoActive = true;
			Time.timeScale = 0.1f;
			Time.fixedDeltaTime = 0.002f;
			grazeTime = Time.time + minGrazeTime;
			grazeCooldownTime = Time.time + grazeCooldownDuration;
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
	
		if ( readyToReset && Time.time > minGrazeTime ) {
			isSlowMoActive = false;
			Time.timeScale = 1f;
			Time.fixedDeltaTime = 0.02f;
			Go.killAllTweensWithTarget( mainCamera );
			Go.to( mainCamera.camera , 0.6f , new GoTweenConfig().floatProp( "orthographicSize" , 50 , false ).setEaseType( GoEaseType.BackOut ) );
			readyToReset = false;
		}
	}
}
