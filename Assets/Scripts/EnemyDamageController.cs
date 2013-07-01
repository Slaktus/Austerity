using UnityEngine;
using System.Collections;

public class EnemyDamageController : MonoBehaviour {
	
	private Rigidbody thisRigidbody;
	private Transform thisTransform;
	private Transform meshContainer;
	private EnemyScaleController scaleControllerScript;
	
	void Awake () {
		thisTransform = transform;
		meshContainer = thisTransform.GetChild( 0 );
		scaleControllerScript = meshContainer.GetComponent< EnemyScaleController >();
		thisRigidbody = rigidbody;
	}
	
	public void IncrementDrag ( float dragIncrementMultiplier) {
		thisRigidbody.drag += dragIncrement * dragIncrementMultiplier;
	}
	
	public float maxDrag;
	private float dragIncrement;
	private float hitPoints;
	
	// Use this for initialization
	void Start () {
		hitPoints = scaleControllerScript.currentHitPoints;
		dragIncrement = ( maxDrag - thisRigidbody.drag ) / hitPoints;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
