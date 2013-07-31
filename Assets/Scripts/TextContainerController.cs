using UnityEngine;
using System.Collections;

public class TextContainerController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	public float targetAngle;
	
	public void SetRotation () {
		transform.rotation = Quaternion.AngleAxis( targetAngle , Vector3.forward );
	}
	
	public void ResetRotation () {
		transform.rotation = Quaternion.AngleAxis( 0 , Vector3.forward );
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
