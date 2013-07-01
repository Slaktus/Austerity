using UnityEngine;
using System.Collections;

public class TOOCOMPLEXGenerationCounterController : MonoBehaviour {
	
	public GameObject generationCounter;
	public int maxGenerationsPerDirection;
	public float generationStartAngle;
	public float generationCounterZPosition = 3.0f;
	private GameObject counterContainer;
	private ScaleController scaleControllerScript;
	private int generation;
	private Vector3 newScale;
	private Vector3 newPosition;
	private float newXPosition;
	private float bufferedXPosition;
	private float availableSpace;
	private GameObject newCounter;
	private Transform thisTransform;
	private Vector3 generationDirection;
	private int bufferedGeneration;
	private int generationCount;
	private int remainingGenerations;
	private int numberOfCounters;
	private float currentGenerationAngle = 180.0f;
	
	void Awake () {
		thisTransform = transform;
		scaleControllerScript = gameObject.GetComponent< ScaleController >();
		generation = scaleControllerScript.generation;
		counterContainer = new GameObject();
		counterContainer.name = "GenerationCounterContainer";
		counterContainer.transform.parent = thisTransform.parent;
		if ( maxGenerationsPerDirection > 0 ) {
			availableSpace = ( ( scaleControllerScript.maxScale.x - scaleControllerScript.initialScale.x ) / 2 ) - 0.25f;
			newXPosition = scaleControllerScript.initialScale.x + availableSpace;
			bufferedGeneration = generation;
			while ( bufferedGeneration > 0 ) {
				generationCount++;
				bufferedXPosition = newXPosition;
				if ( bufferedGeneration >= maxGenerationsPerDirection ) numberOfCounters = maxGenerationsPerDirection;
				else numberOfCounters = bufferedGeneration;
				newScale = Vector3.one * ( availableSpace / numberOfCounters ) / 2;
				newScale.z = 1;
				if ( generationCount % 2 == 0 ) generationDirection = Quaternion.AngleAxis( currentGenerationAngle , Vector3.forward ) * Vector3.left;
				else generationDirection = Quaternion.AngleAxis( currentGenerationAngle - 180 , Vector3.forward ) * Vector3.left;
				Debug.Log( generationDirection );
				for ( int i = 0 ; i < numberOfCounters ; i++ ) {
					newPosition =  thisTransform.position +  ( generationDirection * ( bufferedXPosition + ( ( newScale.x * 2 * numberOfCounters ) - ( availableSpace / numberOfCounters ) ) ) );
					newPosition.z = generationCounterZPosition;
					newCounter = Instantiate( generationCounter , newPosition , Quaternion.identity ) as GameObject;
					newCounter.transform.localScale = newScale;
					newCounter.transform.parent = counterContainer.transform;
					bufferedXPosition -= ( availableSpace / numberOfCounters ) * 2;
				}
				bufferedGeneration -= numberOfCounters;
				if ( generationCount % 2 == 0 ) currentGenerationAngle /= 2;
			}
		} else Debug.Log( "Make sure maxGenerationsPerDirection is above 0" );
	}
	
	private void ScaleAndPositionCounter () {
		//
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
