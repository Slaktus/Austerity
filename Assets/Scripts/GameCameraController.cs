using UnityEngine;
using System.Collections;

public class GameCameraController : MonoBehaviour {
	
	public float deadZoneRadius;
	public float cameraPaddingDistance;
	public float cameraVelocityMultiplier;
	public float dampeningFactor;
	private Vector3 directionToTarget;
	private Vector3 desiredPosition;
	
	private GameObject target;
	
	private void FollowCamera () {
		if ( target != null ) {
			directionToTarget = new Vector3( target.transform.position.x , target.transform.position.y , 0 ) - new Vector3( transform.position.x , transform.position.y + cameraHeight , 0 );
			if ( Vector2.Distance( target.transform.position, transform.position ) > deadZoneRadius ) desiredPosition = new Vector3( directionToTarget.x * ( ( Vector2.Distance(target.transform.position , transform.position ) ) / ( directionToTarget.magnitude ) ) , directionToTarget.y * ( ( Vector2.Distance( target.transform.position , transform.position ) ) / (directionToTarget.magnitude ) ) , -cameraDistance );
			directionToTarget *= cameraVelocityMultiplier;
			if ( Vector2.Distance( target.transform.position, transform.position ) > cameraPaddingDistance ) transform.position = Vector3.SmoothDamp( transform.position , desiredPosition , ref directionToTarget , Vector2.Distance(target.transform.position , transform.position ) * dampeningFactor );
		}
	}
	
	public float cameraDistance;
	public float cameraHeight;
	
	// Use this for initialization
	void Start () {
		target = GameObject.FindGameObjectWithTag("Avatar");
		if ( target != null ) transform.position = new Vector3( target.transform.position.x , target.transform.position.y - cameraHeight, -cameraDistance );
	}
	
	// Update is called once per frame
	void LateUpdate () {
	if ( target != null ) FollowCamera();
	}
}
