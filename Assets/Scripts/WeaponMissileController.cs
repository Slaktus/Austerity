using UnityEngine;
using System.Collections;

public class WeaponMissileController : MonoBehaviour {
	
	public float missileDuration;
	
	private Transform thisTransform;
	private Transform trailTransform;
	private GameObject gameContainer;
	private GameController gameControllerScript;
	private float expiryTime;
	
	void Awake () {
		thisTransform = transform;
		trailTransform = thisTransform.GetChild( 1 );
		gameContainer = GameObject.FindGameObjectWithTag( "GameContainer" );
		gameControllerScript = gameContainer.GetComponent< GameController >();
		expiryTime = Time.time + missileDuration;
	}
	
	public float pushbackForce = 1.0f;
	public float explosionForce = 1.0f;
	public float explosionRadius;
	
	private Vector3 impactDirection;
	private Vector3 explosionDirection;
	private float explosionStrength;
	private Transform hitObjectTransform;
	
	void OnCollisionEnter ( Collision collision ) {
		impactDirection = Vector3.Normalize( collision.transform.position - thisTransform.position );
		collision.gameObject.rigidbody.AddForceAtPosition( impactDirection * pushbackForce , collision.contacts[0].point );
		SimulateExplosion();
		CleanUpMissile();
	}
	
	public GameObject missileHitEffect;
	
	private GameObject hitEffect;
	
	public void CleanUpMissile () {
		hitEffect = Instantiate( missileHitEffect , thisTransform.position , Quaternion.identity ) as GameObject;
		trailTransform.SendMessage( "DetachFromParent" );
		Go.killAllTweensWithTarget( thisTransform );
		Destroy( gameObject );
	}
	
	public float scaleIncrementMultiplier = 1.0f;
	public float dragIncrementMultiplier = 1.0f;
	
	private void SimulateExplosion () {
		Collider[] colliders = Physics.OverlapSphere( thisTransform.position , explosionRadius );
		foreach ( Collider hitObject in colliders ) {
			if ( hitObject.tag == "Enemy" ) {
				hitObjectTransform = hitObject.transform;
				hitObjectTransform.SendMessage( "IncrementScaleTween" , scaleIncrementMultiplier );
				hitObjectTransform.parent.SendMessage( "IncrementDrag" , dragIncrementMultiplier );
				explosionDirection = Vector3.Normalize( hitObjectTransform.position - thisTransform.position );
				explosionStrength = Mathf.Clamp( explosionRadius - Vector3.Magnitude( thisTransform.position - hitObjectTransform.position ) , 0 , explosionRadius );
				explosionStrength /= explosionRadius;
				hitObjectTransform.parent.rigidbody.AddForce( ( explosionDirection * explosionForce ) * explosionStrength );
			}
		}
	}
	
	public float missileHomingDistance;
	public float timeBeforeHoming;
	
	private Vector3 launchDirection;
	private bool isHoming = false;
	
	// Use this for initialization
	void Start () {
		launchDirection = thisTransform.rotation * Vector3.forward;
		Go.to( thisTransform , timeBeforeHoming , new GoTweenConfig().position( thisTransform.position + ( launchDirection * missileHomingDistance ) , false ).setEaseType( GoEaseType.SineIn ) ).setOnCompleteHandler( homing => isHoming = true );
	}
	
	public float missileTargetRange;
	public float missileHomingSpeed;
	public float missileCruisingSpeed;
	public float missileHomingSpeedDecayRate;
	public float missileCruisingSpeedDecayRate;
	public float missileTurningSpeed;
	
	private Vector2 directionToTarget;
	private Vector2 homingDirection;
	private GameObject nearestTarget;
	private Transform targetTransform;
	
	// Update is called once per frame
	void Update () {
		if ( isHoming ) {
			if ( nearestTarget == null ) {
				transform.position += launchDirection * ( Time.deltaTime * missileCruisingSpeed );
				nearestTarget = gameControllerScript.FindNearestEnemy( gameObject );
				if ( nearestTarget != null ) {
					if ( Vector3.Distance( thisTransform.position , nearestTarget.transform.position ) > missileTargetRange ) nearestTarget = null;
					homingDirection = launchDirection;
					missileCruisingSpeed *= missileCruisingSpeedDecayRate;
				}
			} else {
				directionToTarget = nearestTarget.transform.position - thisTransform.position;
				homingDirection = Vector2.Lerp( homingDirection , directionToTarget , Time.deltaTime * missileTurningSpeed );
				homingDirection.Normalize();
				thisTransform.position += ( Vector3 ) homingDirection * ( ( Time.deltaTime * missileHomingSpeed ) );
				thisTransform.rotation = Quaternion.LookRotation( homingDirection );
				missileHomingSpeed *= missileHomingSpeedDecayRate;
			}
		}
		if (Time.time > expiryTime) {
				CleanUpMissile();
			}
	}
}
