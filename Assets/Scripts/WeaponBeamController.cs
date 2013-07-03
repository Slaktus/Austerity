using UnityEngine;
using System.Collections;

public class WeaponBeamController : MonoBehaviour {
	
	private Transform thisTransform;
	private Transform envelopeTransform;
	private Transform particles;
	
	void Awake () {
		thisTransform = transform;
		thisTransform.localScale = Vector3.one;
		envelopeTransform = thisTransform.GetChild( 0 ).GetChild( 0 );
		particles = thisTransform.GetChild( 1 ).GetChild( 0 );
	}
	
	public float scaleIncrementMultiplier = 1.0f;
	public float dragIncrementMultiplier = 1.0f;
	
	private Transform targetTransform;
	
	void OnTriggerStay ( Collider collision ) {
		targetTransform = collision.transform;
		if ( targetTransform.tag == "Enemy" ) {
			targetTransform.SendMessage( "IncrementScaleTween" , scaleIncrementMultiplier );
			targetTransform.parent.SendMessage( "IncrementDrag" , dragIncrementMultiplier );
		} else if ( targetTransform.tag == "Geometry" ) {
			targetTransform = targetTransform.parent.parent;
			targetTransform.SendMessage( "DecrementScaleTween" , scaleIncrementMultiplier );
		}
	}
	
	public float widthScaleDownDuration;
	
	public void ShrinkBeam () {
		scaleChain.destroy();
		Go.killAllTweensWithTarget( thisTransform );
		Go.to( thisTransform , widthScaleDownDuration , new GoTweenConfig().scale( new Vector3( thisTransform.localScale.x , 0 , thisTransform.localScale.z ) , false ).setEaseType( GoEaseType.BackInOut ) ).setOnCompleteHandler( cleanUp => CleanUpBeam() );
	}
	
	public void CleanUpBeam () {
		Go.killAllTweensWithTarget( envelopeTransform );
		particles.SendMessage( "DetachFromParent" );
		Destroy( gameObject );
	}
	
	public float beamHeight;
	public float beamWidth;
	public float heightScaleUpDuration;
	public float widthScaleUpDuration;
	
	private GoTweenChain scaleChain;
	private GoTween heightScaleTween;
	private GoTween widthScaleTween;

	
	// Use this for initialization
	void Start () {
		scaleChain = new GoTweenChain();
		heightScaleTween = new GoTween( thisTransform , heightScaleUpDuration , new GoTweenConfig().scale( new Vector3( thisTransform.localScale.x , thisTransform.localScale.y , beamHeight ) , false ).setEaseType( GoEaseType.ExpoOut ) );
		scaleChain.append( heightScaleTween );
		widthScaleTween = new GoTween( thisTransform , widthScaleUpDuration , new GoTweenConfig().scale( new Vector3( thisTransform.localScale.x , beamWidth , beamHeight ) , false ).setEaseType( GoEaseType.BackOut ) );
		scaleChain.setOnCompleteHandler( beamReady => scaleChain.destroy() );
		scaleChain.append( widthScaleTween );
		scaleChain.play();
	}
}
