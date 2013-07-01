using UnityEngine;
using System.Collections;

public class ParticleController : MonoBehaviour {
	
	public float durationAfterDetaching;
	private float destroyTime;
	private bool isDetached;
	
	void DetachFromParent () {
		if ( !isDetached ) {
			particleSystem.loop = false;
			transform.GetChild( 0 ).particleSystem.loop = false;
			transform.parent = transform.parent.parent.parent;
			destroyTime = Time.time + durationAfterDetaching;
			isDetached = true;
			
		}
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
    void LateUpdate () 
    {
		if ( !isDetached && !particleSystem.IsAlive() ) Destroy( transform.parent.gameObject );
		if ( isDetached && Time.time > destroyTime ) Destroy( gameObject );
    }
}
