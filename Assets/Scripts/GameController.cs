using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public float slowMotionTimeScale;
	public float slowMotionFixedDeltaTime;
	
	IEnumerator SlowMotion( float duration ) {
		Debug.Log( "Slowing down time" );
		Time.timeScale = slowMotionTimeScale;
		Time.fixedDeltaTime = slowMotionFixedDeltaTime;
		yield return new WaitForSeconds( duration );
		Time.timeScale = 1;
		Time.fixedDeltaTime = 0.02f;
		Debug.Log( "Speeding up time" );
		
	}
	
	public GameObject arena;
	public GameObject treeContainer;
	public GameObject avatar;
	public GameObject cursor;
	public GameObject enemy;
	public float arenaStartHeight = 181.5f;
	public float enemyStartDistance = 60.0f;
	private GameObject arenaContainer;
	private GameObject avatarContainer;
	private GameObject skilltreeContainer;
	private GameObject enemyContainer;
	private GameObject initialArena;
	private GameObject currentAvatar;
	private GameObject currentCursor;
	private GameObject initialEnemy;
	private ArrayList arenaList = new ArrayList();
	private ArrayList chamberList = new ArrayList();
	private ArrayList enemyList = new ArrayList();
	private ArrayList geometryList = new ArrayList();
	private Transform thisTransform;
	
	void Awake () {
		Application.targetFrameRate = 60;
		Time.timeScale = 1.0f;
		Time.fixedDeltaTime = 0.02f;
		thisTransform = transform;
		skilltreeContainer = Instantiate( treeContainer , Vector3.zero , Quaternion.identity ) as GameObject;
		skilltreeContainer.transform.parent = thisTransform;
		skilltreeContainer.name = "SkillTreeContainer";
		arenaContainer = new GameObject();
		arenaContainer.gameObject.name = "ArenaContainer";
		arenaContainer.gameObject.transform.parent = thisTransform;
		initialArena = Instantiate( arena , Vector3.forward * arenaStartHeight  , Quaternion.identity ) as GameObject;
		initialArena.transform.parent = arenaContainer.gameObject.transform;
		initialArena.name = "Arena";
		arenaList.Add( initialArena );
		currentAvatar = Instantiate( avatar , Vector3.zero , Quaternion.identity ) as GameObject;
		currentCursor = Instantiate( cursor , Vector3.zero , Quaternion.identity ) as GameObject;
		avatarContainer = new GameObject();
		avatarContainer.gameObject.name = "AvatarContainer";
		avatarContainer.gameObject.transform.parent = thisTransform;
		currentAvatar.transform.parent = avatarContainer.gameObject.transform;
		currentCursor.transform.parent = avatarContainer.gameObject.transform;
		enemyContainer = new GameObject();
		enemyContainer.gameObject.name = "EnemyContainer";
		enemyContainer.gameObject.transform.parent = thisTransform;
		initialEnemy = Instantiate( enemy , Vector3.up * enemyStartDistance , Quaternion.identity ) as GameObject;
		initialEnemy.transform.parent = enemyContainer.transform;
		initialEnemy.name = "Enemy";
		enemyList.Add( initialEnemy );
	}
	
	public void StartSlowMotion ( float duration ) {
		StartCoroutine( SlowMotion ( duration ) );
	}
	
	private GameObject newArena;
	
	public ArrayList GetArenaList () {
		return arenaList;
	}
	
	public void AddArena( Vector3 position ) {
		if ( arena != null ) {
			newArena = Instantiate( arena , new Vector3( position.x , position.y , arenaStartHeight ) , Quaternion.identity ) as GameObject;
			newArena.transform.parent = arenaContainer.gameObject.transform;
			newArena.name = "Arena";
			arenaList.Add( newArena );
		}
	}
	
	public void RemoveArena( GameObject arena ) {
		if ( arena != null ) {
			Go.killAllTweensWithTarget( arena );
			arenaList.Remove( arena );
			Destroy( arena );
		}
	}
	
	private float shortestDistance;
	private Vector2 targetPosition;
	private Vector2 candidatePosition;
	private Transform backdropMesh;
	private GameObject nearestCandidate;
	private float candidateDistance;
	
	public GameObject FindNearestArena ( GameObject requestingObject ) {
		nearestCandidate = null;
		shortestDistance = Mathf.Infinity;
		targetPosition = requestingObject.transform.position;
		if ( arenaList.Count > 0 ) {
			foreach ( GameObject candidate in arenaList ) {
				backdropMesh = candidate.transform.GetChild( 0 );
				candidatePosition = backdropMesh.position;
				candidateDistance = Vector2.Distance( targetPosition , candidatePosition );
				if ( shortestDistance > candidateDistance && backdropMesh != requestingObject.transform ) {
					nearestCandidate = backdropMesh.gameObject;
					shortestDistance = candidateDistance;
				}
			}
		}
		if ( nearestCandidate != null ) return nearestCandidate;
		else return null;
	}
	
	private float distanceToSurface;
	private Vector3 directionToCandidate;
	
	public GameObject FindNearestArenaSurface ( GameObject requestingObject ) {
		nearestCandidate = null;
		shortestDistance = Mathf.Infinity;
		targetPosition = requestingObject.transform.position;
		if ( arenaList.Count > 0 ) {
			foreach ( GameObject candidate in arenaList ) {
				backdropMesh = candidate.transform.GetChild( 0 );
				candidatePosition = backdropMesh.position;
				directionToCandidate = ( Vector3 ) candidatePosition - thisTransform.position;
				distanceToSurface = directionToCandidate.magnitude - ( backdropMesh.localScale.x );
				if ( shortestDistance > distanceToSurface ) {
					nearestCandidate = backdropMesh.gameObject;
					shortestDistance = distanceToSurface;
				}				
			}
		}
		if ( nearestCandidate != null ) return nearestCandidate;
		else return null;
	}
	
	public ArrayList GetChamberList () {
		return chamberList;
	}
	
	public void AddChamber ( GameObject chamber ) {
		if ( chamber != null ) chamberList.Add( chamber );
	}
	
	public void RemoveChamber( GameObject chamber ) {
		if ( chamber != null ) {
			Go.killAllTweensWithTarget( chamber );
			chamberList.Remove( chamber );
			Destroy( chamber );
		}
	}
	
	private GameObject chamberToReturn;
	
	public GameObject GetChamberAt ( int index ) {
		chamberToReturn = chamberList[ index ] as GameObject;
		return chamberToReturn;
	}
	
	public GameObject FindNearestChamber ( GameObject requestingObject ) {
		nearestCandidate = null;
		shortestDistance = Mathf.Infinity;
		targetPosition = requestingObject.transform.position;
		if ( chamberList.Count > 0 ) {
			foreach ( GameObject candidate in chamberList ) {
				backdropMesh = candidate.transform.GetChild( 0 );
				candidatePosition = backdropMesh.position;
				candidateDistance = Vector2.Distance( targetPosition , candidatePosition );
				if ( shortestDistance > candidateDistance && backdropMesh.gameObject.activeInHierarchy ) {
					nearestCandidate = backdropMesh.gameObject;
					shortestDistance = candidateDistance;
				}				
			}
		}
		if ( nearestCandidate != null ) return nearestCandidate;
		else return null;
	}
	
	public ArrayList GetEnemyList () {
		return enemyList;
	}
	
	public void AddEnemy ( GameObject enemy ) {
		if ( enemy != null ) enemyList.Add( enemy );
	}
	
	private int highestGeneration;
	
	public void RemoveEnemy( GameObject enemy , int generation ) {
		if ( enemy != null ) {
			Go.killAllTweensWithTarget( enemy );
			enemyList.Remove( enemy );
			Destroy( enemy );
		}
		if ( generation > highestGeneration ) highestGeneration = generation;
	}
	
	private GameObject enemyToReturn;
	
	public GameObject GetEnemyAt ( int index ) {
		enemyToReturn = enemyList[ index ] as GameObject;
		return enemyToReturn;
	}
	
	private Transform enemyMeshContainer;
	
	public GameObject FindNearestEnemy ( GameObject requestingObject ) {
		nearestCandidate = null;
		shortestDistance = Mathf.Infinity;
		targetPosition = requestingObject.transform.position;
		if ( enemyList.Count > 0 ) {
			foreach ( GameObject candidate in enemyList ) {
				enemyMeshContainer = candidate.transform.GetChild( 0 );
				candidatePosition = candidate.transform.position;
				candidateDistance = Vector2.Distance( targetPosition , candidatePosition );
				if ( shortestDistance > candidateDistance ) {
					nearestCandidate = enemyMeshContainer.gameObject;
					shortestDistance = candidateDistance;
				}				
			}
		}
		if ( nearestCandidate != null ) return nearestCandidate;
		else return null;
	}
	
	public ArrayList GetGeometryList () {
		return geometryList;
	}
	
	public void AddGeometry ( GameObject geometry ) {
		if ( geometry != null ) geometryList.Add( geometry );
	}
	
	public void RemoveGeometry( GameObject geometry ) {
		if ( geometry != null ) {
			Go.killAllTweensWithTarget( geometry );
			geometryList.Remove( geometry );
			Destroy( geometry );
		}
	}
	
	private GameObject geometryToReturn;
	
	public GameObject GetGeometryAt ( int index ) {
		geometryToReturn = geometryList[ index ] as GameObject;
		return geometryToReturn;
	}
	
	private Transform geometryContainer;
	
	public GameObject FindNearestGeometry ( GameObject requestingObject ) {
		nearestCandidate = null;
		shortestDistance = Mathf.Infinity;
		targetPosition = requestingObject.transform.position;
		if ( geometryList.Count > 0 ) {
			foreach ( GameObject candidate in geometryList ) {
				geometryContainer = candidate.transform.GetChild( 0 );
				candidatePosition = candidate.transform.position;
				candidateDistance = Vector2.Distance( targetPosition , candidatePosition );
				if ( shortestDistance > candidateDistance ) {
					nearestCandidate = geometryContainer.gameObject;
					shortestDistance = candidateDistance;
				}				
			}
		}
		if ( nearestCandidate != null ) return nearestCandidate;
		else return null;
	}
	
	private EnemyScaleController enemyScaleScript;
	
	// Update is called once per frame
	void Update () {
		if ( enemyList.Count < 1 ) {
			initialEnemy = Instantiate( enemy , currentAvatar.transform.position + Vector3.up * enemyStartDistance , Quaternion.identity ) as GameObject;
			enemyScaleScript = initialEnemy.transform.GetChild( 0 ).GetComponent< EnemyScaleController >();
			//if ( highestGeneration > 7 ) highestGeneration = 7;
			//enemyScaleScript.generation = 0;
			initialEnemy.transform.parent = enemyContainer.transform;
			initialEnemy.name = "Enemy";
			enemyList.Add( initialEnemy );
		}
		if ( arenaList.Count < 1 ) {
			initialArena = Instantiate( arena , new Vector3( currentAvatar.transform.position.x , currentAvatar.transform.position.y , 1 * arenaStartHeight ) , Quaternion.identity ) as GameObject;
			initialArena.transform.parent = arenaContainer.gameObject.transform;
			initialArena.name = "Arena";
			arenaList.Add( initialArena );
		}
	}
}
