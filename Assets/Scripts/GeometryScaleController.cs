using UnityEngine;
using System.Collections;

public class GeometryScaleController : MonoBehaviour {
	
	public Vector3 containerInitialScale;
	public Vector3 horizontalInitialScale;
	public Vector3 verticalInitialScale;
	private Transform thisTransform;
	private GameObject avatar;
	private GameObject gameContainer;
	private GameController gameControllerScript;
	private EnemyMovementController movementScript;
	
	void Awake () {
		gameContainer = GameObject.FindGameObjectWithTag( "GameContainer" );
		gameControllerScript = gameContainer.GetComponent< GameController >();
		thisTransform = transform;
	}
	
	public Transform[] geometryComponents;
	public float decrementTweenDuration  = 1.0f;
	public float minScale = 5.0f;
	
	private Vector3 targetScale;
	private Transform targetTransform;
	private float combinedRadii;
	private GoTween horizontalDecrementTween;
	private GoTween verticalDecrementTween;
	private bool hasReachedMinScale;
	private Vector3 horizontalTargetScale;
	private Vector3 verticalTargetScale;
	
	public void DecrementScaleTween( float scaleDecrementMultiplier ) {
		if ( hasReachedInitialScale && !hasReachedMinScale ) {
			if ( horizontalDecrementTween != null ) horizontalDecrementTween.destroy();
			if ( verticalDecrementTween != null ) verticalDecrementTween.destroy();
			if ( horizontalLine != null ) {
				horizontalTargetScale = horizontalLine.localScale - horizontalDecrementSize;
				horizontalDecrementTween = new GoTween( horizontalLine , decrementTweenDuration , new GoTweenConfig().scale( horizontalTargetScale , false ).setEaseType( GoEaseType.BackOut ) );
				Go.addTween( horizontalDecrementTween );
			}
			if ( verticalLine != null ) {
				verticalTargetScale = verticalLine.localScale - verticalDecrementSize;
				verticalDecrementTween = new GoTween( verticalLine , decrementTweenDuration , new GoTweenConfig().scale( verticalTargetScale , false ).setEaseType( GoEaseType.BackOut ) );
				Go.addTween( verticalDecrementTween );
			}
			if ( !hasReachedMinScale && horizontalTargetScale.x < minScale && horizontalLine != null ) {
				hasReachedMinScale = true;
				DestroyGeometryTween();
			}
			if ( !hasReachedMinScale && verticalTargetScale.y < minScale && verticalLine != null ) {
				hasReachedMinScale = true;
				DestroyGeometryTween();
			}
		}
	}
	
	public float destroyGeometryDuration;
	
	private void DestroyGeometryTween () {
		Go.killAllTweensWithTarget( thisTransform );
		Go.to( thisTransform , destroyGeometryDuration , new GoTweenConfig().scale( Vector3.zero , false ).setEaseType( GoEaseType.BackIn ) ).setOnCompleteHandler( destroy => CleanUpGeometry() );
	}
	
	public GameObject geometryExplosion;
	private GameObject newExplosion = null;
	
	private void CleanUpGeometry () {
		newExplosion = Instantiate( geometryExplosion , thisTransform.position , Quaternion.identity ) as GameObject;
		gameControllerScript.RemoveGeometry( gameObject );
	}
	
	private bool hasReachedInitialScale;
	
	private void ReachedInitialScale () {
		hasReachedInitialScale = true;
	}
	
	public float startScaleDuration;
	public float defaultHitPoints = 8.0f;
	public float currentHitPoints = 0;
	
	private Vector3 verticalDecrementSize;
	private Vector3 horizontalDecrementSize;
	private Transform horizontalLine;
	private Transform verticalLine;
	
	// Use this for initialization
	void Start () {
		currentHitPoints = defaultHitPoints;
		horizontalDecrementSize = new Vector3( horizontalInitialScale.x / currentHitPoints , 0.0f , 0.0f );
		verticalDecrementSize = new Vector3( 0.0f , verticalInitialScale.y / currentHitPoints , 0.0f );
		if ( geometryComponents.Length > 0 ) horizontalLine = geometryComponents[ 0 ];
		if ( geometryComponents.Length > 1 ) verticalLine = geometryComponents[ 1 ];
		if ( horizontalLine != null ) Go.to( horizontalLine , startScaleDuration , new GoTweenConfig().scale( horizontalInitialScale , false ).setEaseType( GoEaseType.BackOut ) ).setOnCompleteHandler( complete => ReachedInitialScale() ) ;
		if ( verticalLine != null ) Go.to( verticalLine , startScaleDuration , new GoTweenConfig().scale( verticalInitialScale , false ).setEaseType( GoEaseType.BackOut ) );
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
