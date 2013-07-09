using UnityEngine;
using System.Collections;

public class ScaleController : MonoBehaviour {
	
	public int arenaMaxGeneration = 3;
	public float destroyArenaDuration = 0.3f;
	public GameObject arenaDetonationParticles;
	
	private GameObject detonationParticles;
	private GameObject arenaNearestAvatar;
	
	IEnumerator DelayedTrigger() {
		if ( collider != null ) collider.enabled = false;
		yield return new WaitForSeconds( scaleUpTweenDuration / 2 );
		TriggerEnemiesAndArenas();
		gameObject.SendMessage( "FlashInvert" );
		if ( thisTransform.parent.name == "Arena" ) detonationParticles = Instantiate( arenaDetonationParticles , thisTransform.position , Quaternion.identity ) as GameObject;
		if ( generation >= arenaMaxGeneration && type == 0 ) {
			arenaNearestAvatar = gameControllerScript.FindNearestArena( currentAvatar );
			if ( gameObject != arenaNearestAvatar ) {
				Go.killAllTweensWithTarget( thisTransform.parent );
				canReset = false;
				Go.to( thisTransform.parent , destroyArenaDuration , new GoTweenConfig().scale( Vector3.zero , false ).setEaseType( GoEaseType.ExpoIn ) ).setOnCompleteHandler( destroy => DestroyArena() );
			} else ResetScaleTween();
		}
		if ( canReset) StartCoroutine( DelayedTriggerReset() );
		if ( isHubChamber ) {
			StartCoroutine( DestroyHubChamber() );
		}
		//else if ( canReset ) ResetScaleTween();
	}
	
	public float triggerResetDuration;
	
	IEnumerator DelayedTriggerReset() {
		//generation++;
		yield return new WaitForSeconds( triggerResetDuration );
		if ( isTriggered ) isTriggered = false;
		incrementSize = ( maxScale - initialScale ) / generation;
		scaleCount = 0;
		collider.enabled = true;
	}
	
	public float destroyChamberDuration = 0.6f;
	
	IEnumerator DestroyHubChamber() {
		//gameControllerScript.StartSlowMotion( 0.002f );
		Go.to( thisTransform.parent , destroyChamberDuration , new GoTweenConfig().scale( Vector3.zero , false ).setEaseType( GoEaseType.ExpoIn ) );
		yield return new WaitForSeconds( destroyChamberDuration + 0.25f );
		Go.killAllTweensWithTarget( thisTransform.parent );
		thisTransform.parent.parent.GetChild( 1 ).gameObject.SetActive( true );
		gameControllerScript.RemoveChamber( thisTransform.parent.gameObject );
	}
	
	public Vector3 initialScale;
	public bool canReset;
	public Vector3 maxScale;
	public int generation;
	public int chamberId;
	private Transform thisTransform;
	private GameObject gameContainer;
	private GameController gameControllerScript;
	
	void Awake () {
		gameContainer = GameObject.FindGameObjectWithTag( "GameContainer" );
		gameControllerScript = gameContainer.GetComponent< GameController >();
		thisTransform = transform;
	}
	
	public float scaleUpTweenDuration  = 1.0f;
	public float maxScaleMultiplier = 1.025f;
	public Vector3 targetScale;
	public GameObject growParticleEffect;
	
	private GoTweenChain scaleChain;
	private GameObject nearestChamber;
	private Transform targetTransform;
	private float combinedRadii;
	private ArrayList enemyList;
	private ArrayList arenaList;
	private ArrayList enemyCandidateList = new ArrayList();
	private ArrayList arenaCandidateList = new ArrayList();
	private int scaleCount;
	private GameObject growParticles;
	private Vector3 growParticlePosition;
	
	public void ScaleUpTween () {
		if ( scaleCount < generation && !isTriggered ) {
			if ( isHubChamber ) gameObject.SendMessage( "InitialToTargetColor" , SendMessageOptions.DontRequireReceiver );
			scaleCount++;
			targetScale = targetScale + incrementSize;
			if ( targetScale.sqrMagnitude >= maxScale.sqrMagnitude ) targetScale *= maxScaleMultiplier;
			ScaleTweenProperty targetScaleProperty = new ScaleTweenProperty( targetScale , false );
			GoEaseType easeType = GoEaseType.BackOut;
			GoTweenConfig scaleUpConfig = new GoTweenConfig();
			scaleUpConfig.addTweenProperty( targetScaleProperty );
			scaleUpConfig.setEaseType( easeType );
			GoTween scaleUpTween = new GoTween( thisTransform , scaleUpTweenDuration , scaleUpConfig );
			scaleUpTween.setOnCompleteHandler( complete => gameObject.SendMessage( "GetColorFromChamber" , SendMessageOptions.DontRequireReceiver ) );
			if ( scaleChain == null ) scaleChain = new GoTweenChain();
			scaleChain.append( scaleUpTween );
			scaleChain.play();
			if ( scaleCount >= generation ) {
				isTriggered = true;
				StartCoroutine( DelayedTrigger() );
			} else if ( growParticleEffect != null ) {
				growParticlePosition = thisTransform.position;
				growParticlePosition.z = 0;
				growParticles = Instantiate( growParticleEffect , growParticlePosition , Quaternion.identity ) as GameObject;
			}
		}
	}
	
	public bool isTriggered = false;
	
	public void TriggeredByArena () {
		if ( !isTriggered ) {
			targetScale = maxScale * maxScaleMultiplier;
			ScaleTweenProperty targetScaleProperty = new ScaleTweenProperty( targetScale , false );
			GoEaseType easeType = GoEaseType.BackOut;
			GoTweenConfig scaleUpConfig = new GoTweenConfig();
			scaleUpConfig.addTweenProperty( targetScaleProperty );
			scaleUpConfig.setEaseType( easeType );
			GoTween scaleUpTween = new GoTween( thisTransform , scaleUpTweenDuration , scaleUpConfig );
			scaleUpTween.setOnCompleteHandler( complete => gameObject.SendMessage( "GetColorFromChamber" , SendMessageOptions.DontRequireReceiver ) );
			if ( scaleChain == null ) scaleChain = new GoTweenChain();
			scaleChain.append( scaleUpTween );
			scaleChain.play();
			isTriggered = true;
			gameObject.SendMessage( "FlashInvert" );
			StartCoroutine( DelayedTrigger() );
		}
	}
	
	public bool isHubChamber;
	public float resetTweenDuration = 1.0f;
	public int chamberActivationIndex;
	
	private GoTween resetTween;
	
	public void ResetScaleTween () {
		rimOccluderContainer.SendMessage( "maxScaleTween" );
		if ( canReset ) targetScale = initialScale;
		else {
			targetScale = maxScale;
			if ( chamberId < 9 ) {/*
				if ( chamberActivationIndex == 1 ) {
					thisTransform.parent.parent.GetChild( 1 ).gameObject.SetActive( true );
					thisTransform.parent.parent.GetChild( 2 ).gameObject.SetActive( true );
				} else if ( chamberActivationIndex == 6 ) {
					thisTransform.parent.parent.GetChild( 6 ).gameObject.SetActive( true );
					thisTransform.parent.parent.GetChild( 7 ).gameObject.SetActive( true );
				}
				else {*/
					thisTransform.parent.parent.GetChild( chamberActivationIndex ).gameObject.SetActive( true );
				//}
			}
		}
		ScaleTweenProperty targetScaleProperty = new ScaleTweenProperty( targetScale , false );
		GoTweenConfig resetConfig = new GoTweenConfig();
		resetConfig.addTweenProperty( targetScaleProperty );
		resetConfig.setEaseType( GoEaseType.BackOut );
		GoTween resetTween = new GoTween( thisTransform , scaleUpTweenDuration , resetConfig );
		resetTween.setOnCompleteHandler( complete => gameObject.SendMessage( "GetColorFromChamber" , SendMessageOptions.DontRequireReceiver ) );
		scaleChain.append( resetTween );
		scaleChain.play();
	}
	
	private void TriggerEnemiesAndArenas () {
		if ( enemyList == null ) enemyList = gameControllerScript.GetEnemyList();
		foreach ( GameObject enemyCandidate in enemyList ) {
			targetTransform = enemyCandidate.transform.GetChild( 0 );
			combinedRadii = targetTransform.localScale.x + targetScale.x;
			if ( combinedRadii > Vector2.Distance( targetTransform.position , thisTransform.position ) ) enemyCandidateList.Add ( enemyCandidate );
		}
		if ( enemyCandidateList.Count > 0 ) {
			foreach ( GameObject enemyToDestroy in enemyCandidateList ) {
				enemyToDestroy.transform.GetChild( 0 ).SendMessage( "DestroyEnemyTween" , true );
			}
			enemyCandidateList.Clear();
		}
		if ( thisTransform.parent.name == "Arena" /*|| thisTransform.parent.parent.parent.name == "SkillTreeContainer"*/ ) {
			if ( arenaList == null ) arenaList = gameControllerScript.GetArenaList();
			foreach ( GameObject arenaCandidate in arenaList ) {
				if ( arenaCandidate != thisTransform.parent ) {
					targetTransform = arenaCandidate.transform.GetChild( 0 );
					combinedRadii = targetTransform.localScale.x + targetScale.x;
					if ( combinedRadii > Vector2.Distance( targetTransform.position , thisTransform.position ) ) arenaCandidateList.Add ( arenaCandidate );
				}
			}
			if ( arenaCandidateList.Count > 0 ) {
				foreach ( GameObject arenaToTrigger in arenaCandidateList ) {
					arenaToTrigger.transform.GetChild( 0 ).SendMessage( "TriggeredByArena" );
				}
				arenaCandidateList.Clear();
			}
			nearestChamber = gameControllerScript.FindNearestChamber( gameObject );
			targetTransform = nearestChamber.transform;
			combinedRadii = targetTransform.localScale.x + targetScale.x;
			if ( combinedRadii > Vector2.Distance( targetTransform.position , thisTransform.position ) ) {
				targetTransform.SendMessage( "ScaleUpTween" );
			}
		}
	}
	
	public GameObject destroyArenaEffect;
	public GameObject[] geometry;
	private GameObject newGeometry;
	private Vector3 newGeometryPosition;
	
	private void DestroyArena() {
		Instantiate( destroyArenaEffect , new Vector3( thisTransform.position.x , thisTransform.position.y , 0 ) , Quaternion.identity );
		/*if ( gameObject.GetComponent< ArenaMeshColorController >().chamberType == "Malkut" ) {
			newGeometryPosition = thisTransform.parent.position;
			newGeometryPosition.z = 0;
			newGeometry = Instantiate( geometry[0] , newGeometryPosition , Quaternion.identity ) as GameObject;
			gameControllerScript.AddGeometry( newGeometry );
		}
		if ( gameObject.GetComponent< ArenaMeshColorController >().chamberType == "Sefirot" ) {
			newGeometryPosition = thisTransform.parent.position;
			newGeometryPosition.z = 0;
			newGeometry = Instantiate( geometry[1] , newGeometryPosition , Quaternion.identity ) as GameObject;
			gameControllerScript.AddGeometry( newGeometry );
		}
		if ( gameObject.GetComponent< ArenaMeshColorController >().chamberType == "Kelipot" ) {
			newGeometryPosition = thisTransform.parent.position;
			newGeometryPosition.z = 0;
			newGeometry = Instantiate( geometry[2] , newGeometryPosition , Quaternion.identity ) as GameObject;
			gameControllerScript.AddGeometry( newGeometry );
		}*/
		gameControllerScript.RemoveArena( thisTransform.parent.gameObject );
	}
	
	private Vector3 incrementSize;
	private GameObject rimOccluderContainer;
	private GenerationCounterController generationScript;
	private int type; //0 = arena, 1 = chamber
	private GameObject currentAvatar;
	
	void Start () {
		incrementSize = ( maxScale - initialScale ) / generation;
		rimOccluderContainer = transform.parent.GetChild( 1 ).gameObject;
		thisTransform.localScale = initialScale;
		targetScale = initialScale;
		generationScript = gameObject.GetComponent< GenerationCounterController >();
		if ( generationScript != null ) generationScript.enabled = true;
		if ( thisTransform.parent.name == "Arena" ) type = 0;
		else type = 1;
		currentAvatar = GameObject.FindGameObjectWithTag( "Avatar" );
	}
}
