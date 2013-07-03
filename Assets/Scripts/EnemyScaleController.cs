using UnityEngine;
using System.Collections;

public class EnemyScaleController : MonoBehaviour {
	
	public float awakeScaleDuration;
	public Vector3 initialScale;
	private Transform thisTransform;
	private GameObject avatar;
	private GameObject gameContainer;
	private GameController gameControllerScript;
	private EnemyMovementController movementScript;
	
	void Awake () {
		avatar = GameObject.FindGameObjectWithTag( "Avatar" );
		gameContainer = GameObject.FindGameObjectWithTag( "GameContainer" );
		gameControllerScript = gameContainer.GetComponent< GameController >();
		thisTransform = transform;
		movementScript = thisTransform.parent.GetComponent< EnemyMovementController >();
		transform.localScale = Vector3.zero;
		Go.to( thisTransform , awakeScaleDuration , new GoTweenConfig().scale ( initialScale , false ).setEaseType( GoEaseType.BackOut )).setOnCompleteHandler( complete => finishAwake() );
		targetScale = initialScale;
	}
	
	public GameObject newArena;
	public float incrementTweenDuration  = 1.0f;
	public float maxScaleMultiplier = 1.025f;
	private Vector3 targetScale;
	private GameObject nearestArena;
	private Transform targetTransform;
	private float combinedRadii;
	private GoTween incrementTween;
	
	public void IncrementScaleTween( float scaleIncrementMultiplier ) {
		if ( targetScale.sqrMagnitude <= maxScale.sqrMagnitude ) {
			gameObject.SendMessage( "TweenToDamageColor" );
			targetScale = targetScale + ( incrementSize * scaleIncrementMultiplier );
			if ( !hasReachedMaxScale ) {
				if ( targetScale.sqrMagnitude > maxScale.sqrMagnitude ) {
					targetScale *= maxScaleMultiplier;
				}
				if ( incrementTween != null ) incrementTween.destroy();
				ScaleTweenProperty targetScaleProperty = new ScaleTweenProperty( targetScale , false );
				GoEaseType easeType = GoEaseType.BackOut;
				GoTweenConfig incrementConfig = new GoTweenConfig();
				incrementConfig.addTweenProperty( targetScaleProperty );
				incrementConfig.setEaseType( easeType );
				incrementTween = new GoTween( thisTransform , incrementTweenDuration , incrementConfig );
				Go.addTween( incrementTween );
				if ( targetScale.sqrMagnitude > maxScale.sqrMagnitude ) DestroyEnemyTween( false );
			}
		}
	}
	
	public float zeroTweenDuration;
	private bool hasReachedMaxScale;
	private bool quickCleanup = false;
	private GoTweenChain destroyEnemyChain;
	
	public void DestroyEnemyTween ( bool destroyedByBackdrop ) {
		if ( !hasReachedMaxScale ) { //This is fucking nasty, fix it
			hasReachedMaxScale = true;
			Destroy( sphereCollider );
			destroyEnemyChain = new GoTweenChain();
			if ( destroyedByBackdrop ) {
				quickCleanup = true;
				ScaleTweenProperty maxScaleProperty = new ScaleTweenProperty( maxScale , false );
				GoEaseType maxEaseType = GoEaseType.BackOut;
				GoTweenConfig maxConfig = new GoTweenConfig();
				maxConfig.addTweenProperty( maxScaleProperty );
				maxConfig.setEaseType( maxEaseType );
				GoTween maxTween = new GoTween( thisTransform , incrementTweenDuration , maxConfig );
				destroyEnemyChain.append( maxTween );
			}
			thisTransform.parent.SendMessage( "EnableMovement" , false );
			ScaleTweenProperty zeroScaleProperty = new ScaleTweenProperty( Vector3.zero , false );
			GoEaseType zeroEaseType = GoEaseType.SineOut;
			GoTweenConfig zeroConfig = new GoTweenConfig();
			zeroConfig.addTweenProperty( zeroScaleProperty );
			zeroConfig.setEaseType( zeroEaseType );
			GoTween zeroTween = new GoTween( thisTransform , zeroTweenDuration , zeroConfig );
			destroyEnemyChain.append( zeroTween );
			destroyEnemyChain.setOnCompleteHandler( cleanUp => CleanUpEnemy() );
			destroyEnemyChain.play();
		}
	}
	
	public float sphereColliderRadius;
	private SphereCollider sphereCollider;
	
	private void finishAwake () {
		sphereCollider = gameObject.AddComponent( "SphereCollider" ) as SphereCollider;
		sphereCollider.radius = sphereColliderRadius;
		movementScript.enabled = true;
	}
	
	public GameObject[] geometry;
	public GameObject explosion;
	public GameObject explosionLite;
	private GameObject explosionEffect;
	private GameObject newGeometry;
	
	private void CleanUpEnemy () {
		if ( !quickCleanup ) {
			nearestArena = gameControllerScript.FindNearestArena( gameObject );
			targetTransform = nearestArena.transform;
			if ( targetScale.x > Vector2.Distance( targetTransform.position , thisTransform.position ) ) {
				targetTransform.SendMessage( "ScaleUpTween" );
				Debug.Log( nearestArena.GetComponent< ArenaMeshColorController >().chamberType );
				if ( nearestArena.GetComponent< ArenaMeshColorController >().chamberType == "Malkut" ) {
					newGeometry = Instantiate( geometry[0] , thisTransform.position , Quaternion.identity ) as GameObject;
					gameControllerScript.AddGeometry( newGeometry );
					newGeometry.transform.parent = targetTransform.parent.GetChild( 4 );
				}
				if ( nearestArena.GetComponent< ArenaMeshColorController >().chamberType == "Sefirot" ) {
					newGeometry = Instantiate( geometry[1] , thisTransform.position , Quaternion.identity ) as GameObject;
					gameControllerScript.AddGeometry( newGeometry );
					newGeometry.transform.parent = targetTransform.parent.GetChild( 4 );
				}
				if ( nearestArena.GetComponent< ArenaMeshColorController >().chamberType == "Kelipot" ) {
					newGeometry = Instantiate( geometry[2] , thisTransform.position , Quaternion.identity ) as GameObject;
					gameControllerScript.AddGeometry( newGeometry );
					newGeometry.transform.parent = targetTransform.parent.GetChild( 4 );
				}
			} else {
				gameControllerScript.AddArena( thisTransform.position );
			}
			AddNewGeneration();
		}
		if ( quickCleanup ) explosionEffect = Instantiate( explosionLite , thisTransform.position , Quaternion.identity ) as GameObject;
		else  explosionEffect = Instantiate( explosion , thisTransform.position , Quaternion.identity ) as GameObject;
		AddCollectables();
		Go.killAllTweensWithTarget( thisTransform );
		Go.killAllTweensWithTarget( thisTransform.GetChild( 0 ).renderer.material );
		if ( destroyEnemyChain != null ) destroyEnemyChain.destroy();
		gameControllerScript.RemoveEnemy( thisTransform.parent.gameObject , generation - 1 );
	}
	
	public float newEnemyEjectionAngle;
	public float newEnemyPushForce;
	private float currentAngle;
	private Vector3 directionToAvatar;
	private Vector3 newEnemyPushDirection;
	private GameObject newEnemy;
	
	private void AddNewGeneration() {
		currentAngle = newEnemyEjectionAngle;
		directionToAvatar = Vector3.Normalize( thisTransform.position - avatar.transform.position );
		for ( int i = 0 ; i < generation ; i++ ) {
			newEnemy = Instantiate( transform.parent.gameObject , thisTransform.position , Quaternion.identity ) as GameObject;
			newEnemy.name = "Enemy";
			newEnemy.transform.parent = transform.parent.transform.parent;

			gameControllerScript.AddEnemy( newEnemy );
			if ( i % 2 == 0 ) {
				if ( generation == i + 1 ) currentAngle = 0;
				newEnemyPushDirection = Quaternion.AngleAxis( currentAngle , Vector3.forward ) * directionToAvatar;
			}
			else {
				newEnemyPushDirection = Quaternion.AngleAxis( -currentAngle , Vector3.forward ) * directionToAvatar;
				if ( generation != i + 1 ) currentAngle -= ( newEnemyEjectionAngle / generation ) * 2;
			}
			newEnemy.rigidbody.drag = 2;
			newEnemy.rigidbody.AddForce( newEnemyPushDirection * newEnemyPushForce );
		}
	}
	
	public GameObject collectable;
	public int defaultNumberOfCollectables;
	public float collectableMovementDistance;
	public float collectableMovementDuration;
	public float collectableScale;
	private float angleIncrement;
	private Vector3 newPosition;
	private GameObject newCollectable;
	
	private void AddCollectables () {
		defaultNumberOfCollectables += generation;
		angleIncrement = 360 / defaultNumberOfCollectables;
		currentAngle = angleIncrement;
		for ( int i = 0 ; i < defaultNumberOfCollectables ; i++ ) {
			newPosition =  thisTransform.position + ( Quaternion.AngleAxis( currentAngle , Vector3.forward ) * ( Vector3.left * collectableMovementDistance ) );
			newCollectable = Instantiate( collectable , thisTransform.position , Quaternion.LookRotation( Vector3.Normalize( newPosition - thisTransform.position ) ) ) as GameObject;
			newCollectable.transform.parent = thisTransform.parent.transform.parent;
			newPosition.z = thisTransform.position.z - 2;
			Go.to( newCollectable.transform , collectableMovementDuration , new GoTweenConfig().position( newPosition , false ).scale( Vector3.one * collectableScale ).setEaseType( GoEaseType.BackOut ) );
			currentAngle += angleIncrement;
		}
	}
	
	public int generation;
	public float defaultHitPoints = 8.0f;
	public float currentHitPoints = 0;
	public Vector3 maxScale;
	private Vector3 incrementSize;
	
	// Use this for initialization
	void Start () {
		currentHitPoints = defaultHitPoints;
		currentHitPoints += generation;
		generation++;
		maxScale *= 1 + ( generation / 25 );
		incrementSize = ( maxScale - initialScale ) / currentHitPoints;
	}
	
	// Update is called once per frame
	void Update () {
	}
}
