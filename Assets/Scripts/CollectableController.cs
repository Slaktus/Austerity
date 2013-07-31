using UnityEngine;
using System.Collections;

public class CollectableController : MonoBehaviour {
	
	public float lifeTime;
	
	private float targetTime;
	private Transform thisTransform;
	private GameObject avatar;
	private FontController multiplierCounterScript;
	private Transform avatarTransform;
	private Transform trail;
	private GameController gameControllerScript;
	
	void Awake () {
		thisTransform = transform;
		avatar = GameObject.FindGameObjectWithTag( "Avatar" );
		if ( avatar != null ) avatarTransform = avatar.transform;
		targetTime = Time.time + lifeTime;
		trail = transform.GetChild( 1 );
	}
	
	// Use this for initialization
	void Start () {
		multiplierCounterScript = GameObject.FindGameObjectWithTag( "MultiplierCounter" ).GetComponent< FontController >();
		gameControllerScript = GameObject.FindGameObjectWithTag( "GameContainer" ).GetComponent< GameController >();
		gameControllerScript.AddMisc( gameObject );
	}
	
	public float avatarDetectionDistance;
	public float destroyCollectableDistance;
	public float shrinkDuration;
	public float movementSpeed;
	public float rotationSpeed;
	public GameObject chaseEffect;
	
	private bool isChasingAvatar;
	private bool isShrinking;
	private GameObject chaseParticles;
	
	// Update is called once per frame
	void Update () {
		if ( !isChasingAvatar && avatarTransform != null && Vector3.Distance( thisTransform.position , avatarTransform.position ) < avatarDetectionDistance ) {
			chaseParticles = Instantiate( chaseEffect , thisTransform.position , Quaternion.identity ) as GameObject;
			chaseParticles.transform.parent = thisTransform.parent;
			isChasingAvatar = true;
			Go.killAllTweensWithTarget( thisTransform );
		}
		
		if ( !isShrinking && !isChasingAvatar && Time.time > targetTime ) {
			isShrinking = true;
			Go.to( thisTransform , shrinkDuration , new GoTweenConfig().scale( Vector3.zero , false ).setEaseType( GoEaseType.BackIn ) ).setOnCompleteHandler( complete => Destroy( gameObject ) );
		}
		
		if ( isChasingAvatar && avatarTransform != null ) {
			thisTransform.position = Vector3.Lerp( thisTransform.position , avatarTransform.position , Time.deltaTime * movementSpeed );
			thisTransform.rotation = Quaternion.Slerp( thisTransform.rotation , Quaternion.LookRotation( avatarTransform.position - thisTransform.position ) , Time.deltaTime * rotationSpeed );
		}
		if ( isChasingAvatar  && avatarTransform != null && Vector3.Distance( thisTransform.position , avatarTransform.position ) < destroyCollectableDistance ) {
			chaseParticles = Instantiate( chaseEffect , thisTransform.position , Quaternion.LookRotation( thisTransform.position - avatarTransform.position ) ) as GameObject;
			chaseParticles.transform.parent = thisTransform.parent;
			Go.killAllTweensWithTarget( thisTransform );
			trail.SendMessage( "DetachFromParent" );
			multiplierCounterScript.SetText( 0.1f , false );
			Destroy( gameObject );
		}
	}
}
