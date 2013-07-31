using UnityEngine;
using System.Collections;

public class TrailController : MonoBehaviour {
	
	public bool isCollectableTrail;
	public bool isProjectileTrail;
	public bool isArena;
	private Transform meshContainer;
	private Transform mesh;
	private Material meshMaterial;
	private TrailRenderer thisTrail;
	private Material trailMaterial;
	private Transform thisTransform;
	private Transform parentTransform;
	
	void Awake () {
		thisTrail = gameObject.GetComponent< TrailRenderer >();
		thisTransform = transform;
		parentTransform = transform.parent;
	}
	
	public float durationAfterDetaching;
	private float destroyTime;
	private bool isDetached;
	
	void DetachFromParent () {
		if ( !isDetached ) {
			thisTransform.parent = parentTransform.parent;
			destroyTime = Time.time + durationAfterDetaching;
			isDetached = true;
		}
	}
	
	// Use this for initialization
	void Start () {
		if ( isCollectableTrail ) {
			mesh = transform.parent.transform;
			meshContainer = mesh;
		} else if ( !isProjectileTrail && !isArena ) {
			meshContainer = transform.parent.GetChild( 0 );
			mesh = meshContainer.GetChild( 0 );
			meshMaterial = mesh.renderer.material;
			thisTrail.startWidth = meshContainer.localScale.x * 2;
			trailMaterial = thisTrail.material;
			trailMaterial.color = meshMaterial.color;
		}
	}
	
	public float trailAlpha;
	
	// Update is called once per frame
	void Update () {
		if ( !isDetached && isCollectableTrail ) {
			thisTrail.startWidth = meshContainer.localScale.x;
		} else if ( !isDetached && !isProjectileTrail && !isArena ) {
			thisTrail.startWidth = meshContainer.localScale.x * 2;
			trailMaterial.color = new Color( meshMaterial.color.r , meshMaterial.color.g , meshMaterial.color.b , trailAlpha );
			//thisTrail.time = meshContainer.localScale.x / 1.5f;
		}
		if ( isDetached && Time.time > destroyTime ) Destroy( gameObject );
	}
}
