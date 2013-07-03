using UnityEngine;
using System.Collections;

public class AvatarCollisionController : MonoBehaviour {
	
	private Transform targetTransform;
	
	void OnTriggerStay ( Collider collision ) {
		Debug.Log( "This is my other collider" );
		targetTransform = collision.transform;
		if ( targetTransform.parent != null && targetTransform.parent.name == "Enemy" ) Application.LoadLevel( Application.loadedLevel );
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
