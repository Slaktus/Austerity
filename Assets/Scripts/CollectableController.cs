using UnityEngine;
using System.Collections;

public class CollectableController : MonoBehaviour {
	
	public float lifeTime;
	private float targetTime;
	private Transform thisTransform;
	private GameObject avatar;
	private Transform avatarTransform;
	private Transform trail;
	
	void Awake () {
		thisTransform = transform;
		avatar = GameObject.FindGameObjectWithTag( "Avatar" );
		avatarTransform = avatar.transform;
		targetTime = Time.time + lifeTime;
		trail = transform.GetChild( 1 );
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	public float avatarDetectionDistance;
	public float destroyCollectableDistance;
	public float shrinkDuration;
	public float movementSpeed;
	public float rotationSpeed;
	private bool isChasingAvatar;
	private bool isShrinking;
	
	// Update is called once per frame
	void Update () {
		if ( !isChasingAvatar && Vector3.Distance( thisTransform.position , avatarTransform.position ) < avatarDetectionDistance ) {
			isChasingAvatar = true;
			Go.killAllTweensWithTarget( thisTransform );
		}
		
		if ( !isShrinking && !isChasingAvatar && Time.time > targetTime ) {
			isShrinking = true;
			Go.to( thisTransform , shrinkDuration , new GoTweenConfig().scale( Vector3.zero , false ).setEaseType( GoEaseType.BackIn ) ).setOnCompleteHandler( complete => Destroy( gameObject ) );
		}
		
		if ( isChasingAvatar ) {
			thisTransform.position = Vector3.Lerp( thisTransform.position , avatarTransform.position , Time.deltaTime * movementSpeed );
			thisTransform.rotation = Quaternion.Slerp( thisTransform.rotation , Quaternion.LookRotation( avatarTransform.position - thisTransform.position ) , Time.deltaTime * rotationSpeed );
		}
		if ( isChasingAvatar && Vector3.Distance( thisTransform.position , avatarTransform.position ) < destroyCollectableDistance ) {
			Go.killAllTweensWithTarget( thisTransform );
			trail.SendMessage( "DetachFromParent" );
			Destroy( gameObject );
		}
	}
}
