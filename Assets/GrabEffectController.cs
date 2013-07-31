using UnityEngine;
using System.Collections;

public class GrabEffectController : MonoBehaviour {
	
	private Transform thisTransform;
	private Transform currentCursor;
	
	// Use this for initialization
	void Start () {
		thisTransform = transform;
		currentCursor = GameObject.FindGameObjectWithTag( "Cursor" ).transform;
	}
	
	public float angleIncrement;
	
	private float currentAngle;
	
	// Update is called once per frame
	void Update () {
		currentAngle += angleIncrement;
		if ( currentCursor == null ) currentCursor = GameObject.FindGameObjectWithTag( "Cursor" ).transform;
		else thisTransform.rotation = Quaternion.LookRotation( thisTransform.position - currentCursor.position ) * Quaternion.AngleAxis( currentAngle , Vector3.forward );
		if ( currentAngle > 360 ) currentAngle -= 360;
	}
}
