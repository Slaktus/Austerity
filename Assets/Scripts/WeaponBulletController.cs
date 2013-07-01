using UnityEngine;
using System.Collections;

public class WeaponBulletController : MonoBehaviour {
	
	private Transform thisTransform;
	private Transform trailTransform;
	
	void Awake () {
		thisTransform = transform;
		trailTransform = thisTransform.GetChild( 1 );
	}
	
	public float scaleIncrementMultiplier = 1.0f;
	public float dragIncrementMultiplier = 1.0f;
	public float pushbackForce = 1.0f;
	
	private Transform targetTransform;
	private GameObject hitEffect;
	
	void OnCollisionEnter ( Collision collision ) {
		targetTransform = collision.transform;
		ContactPoint contact = collision.contacts[0];
		hitEffect = Instantiate( bulletHitEffect , thisTransform.position , Quaternion.LookRotation( rigidbody.velocity.normalized , Vector3.forward ) ) as GameObject;
		if ( collision.transform.tag == "Enemy" ) {
			targetTransform.GetChild( 0 ).SendMessage( "IncrementScaleTween" , scaleIncrementMultiplier );
			targetTransform.SendMessage( "IncrementDrag" , dragIncrementMultiplier );
			targetTransform.gameObject.rigidbody.AddForceAtPosition( -rigidbody.velocity.normalized * pushbackForce , contact.point );
			CleanUpBullet();
		}
	}
	
	public GameObject bulletHitEffect;
	
	public void CleanUpBullet () {
		trailTransform.SendMessage( "DetachFromParent" );
		Go.killAllTweensWithTarget( thisTransform );
		Destroy( gameObject );
	}
	
	public float bulletRange;
	public float bulletDuration;
	private Vector3 launchDirection;
	
	// Use this for initialization
	void Start () {
		launchDirection = thisTransform.rotation * Vector3.forward;
		Go.to( thisTransform , bulletDuration , new GoTweenConfig().position( thisTransform.position + ( launchDirection * bulletRange ) , false ).setEaseType( GoEaseType.Linear ) ).setOnCompleteHandler( complete => CleanUpBullet() );
	}
	
	// Update is called once per frame
	void Update () {
	}
}
