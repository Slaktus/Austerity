using UnityEngine;
using System.Collections;

public class GeometryPositionController : MonoBehaviour {
	
	private Vector3 relativePosition;
	private Transform thisTransform;
	private Transform parentTransform;
	
	// Use this for initialization
	void Start () {
		thisTransform = transform;
		parentTransform = thisTransform.parent.parent;
		relativePosition = parentTransform.position - thisTransform.position;
		Debug.Log( parentTransform );
	}
	
	private Vector3 newPosition;
	
	// Update is called once per frame
	void Update () {
		newPosition = parentTransform.position + relativePosition;
		newPosition.z = 0;
		thisTransform.position = newPosition;
	}
}
