using UnityEngine;
using System.Collections;

public class GenerationCounterController : MonoBehaviour {
	
	private ScaleController scaleControllerScript;
	private ArrayList generationCounterList = new ArrayList();
	
	void Awake () {
		thisTransform = transform;
		scaleControllerScript = gameObject.GetComponent< ScaleController >();
	}
	
	public float counterMoveDuration;
	public float counterScaleDuration;
	private Vector3 newScale;
	private Vector3 newPosition;
	private float newXPosition;
	private float availableSpace;
	private Vector3 direction;
	private GameObject newCounter;
	private Transform thisTransform;
	private int countersToAdd;
	
	private void ScaleAndPositionCounters () {
		countersToAdd = scaleControllerScript.generation - generation;
		for ( int i = 0 ; i < countersToAdd ; i++ ) {
			newCounter = Instantiate( generationCounter , thisTransform.position , Quaternion.identity ) as GameObject;
			newCounter.transform.parent = counterContainer.transform;
			generationCounterList.Add( newCounter );
		}
		availableSpace = ( ( scaleControllerScript.maxScale.x - scaleControllerScript.initialScale.x ) / 2 ) - 0.25f;
		newScale = Vector3.one * ( availableSpace / scaleControllerScript.generation ) / 2;
		newScale.z = generationCounterHeight;
		newXPosition = scaleControllerScript.initialScale.x + availableSpace;
		direction = Quaternion.AngleAxis( 0 , Vector3.forward ) * Vector3.left;
		foreach ( GameObject counter in generationCounterList ) {
			newPosition =  thisTransform.position +  ( direction * ( newXPosition + ( ( newScale.x * 2 * scaleControllerScript.generation ) - ( availableSpace / scaleControllerScript.generation ) ) ) );
			newPosition.z = generationCounterZPosition;
			Go.to ( counter.transform , counterMoveDuration , new GoTweenConfig().position( newPosition , false ).setEaseType( GoEaseType.SineInOut ) );
			Go.to ( counter.transform , counterScaleDuration , new GoTweenConfig().scale( newScale , false ).setEaseType( GoEaseType.BackOut ) );
			newXPosition -= ( availableSpace / scaleControllerScript.generation ) * 2;
		}
		generation = scaleControllerScript.generation;
	}
	
	public GameObject generationCounter;
	public float generationStartAngle;
	public float generationCounterZPosition = 3.0f;
	public float generationCounterHeight = 1.0f;
	private GameObject counterContainer;
	private int generation;
	
	// Use this for initialization
	void Start () {
		generation = scaleControllerScript.generation;
		counterContainer = new GameObject();
		counterContainer.name = "GenerationCounterContainer";
		counterContainer.transform.parent = thisTransform.parent;
		for ( int i = 0 ; i < generation ; i++ ) {
			newCounter = Instantiate( generationCounter , thisTransform.position , Quaternion.identity ) as GameObject;
			newCounter.transform.parent = counterContainer.transform;
			generationCounterList.Add( newCounter );
		}
		ScaleAndPositionCounters();
	}
	
	// Update is called once per frame
	void Update () {
		if ( scaleControllerScript.generation != generation ) ScaleAndPositionCounters();
	}
}
