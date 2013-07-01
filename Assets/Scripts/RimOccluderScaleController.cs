using UnityEngine;
using System.Collections;

public class RimOccluderScaleController : MonoBehaviour {
	
	private Transform thisTransform;
	private Transform occluderMesh;
	private FromMeshColorController occluderColorScript;
	
	void Awake () {
		thisTransform = transform;
		occluderMesh = transform.GetChild( 0 );
		occluderMesh = occluderMesh.GetChild( 0 );
		occluderColorScript = occluderMesh.GetComponent< FromMeshColorController >();
	}
	
	public float scaleUpTweenDuration  = 0.3f;
	public float scaleDownTweenDuration  = 0.6f;
	public float maxScaleMultiplier = 1.5f;
	private GoTweenChain scaleChain;
	
	public void maxScaleTween () {
		ScaleTweenProperty targetScaleProperty = new ScaleTweenProperty( new Vector3( thisTransform.localScale.x * maxScaleMultiplier , thisTransform.localScale.y * maxScaleMultiplier , thisTransform.localScale.z ) , false );
		GoEaseType easeType = GoEaseType.ExpoOut;
		GoTweenConfig scaleUpConfig = new GoTweenConfig();
		scaleUpConfig.addTweenProperty( targetScaleProperty );
		scaleUpConfig.setEaseType( easeType );
		GoTween scaleUpTween = new GoTween( thisTransform , scaleUpTweenDuration , scaleUpConfig );
		if ( scaleChain == null ) scaleChain = new GoTweenChain();
		scaleChain.append( scaleUpTween );
		targetScaleProperty = new ScaleTweenProperty( transform.localScale , false );
		easeType = GoEaseType.BackOut;
		GoTweenConfig scaleDownConfig = new GoTweenConfig();
		scaleDownConfig.addTweenProperty( targetScaleProperty );
		scaleDownConfig.setEaseType( easeType );
		GoTween scaleDownTween = new GoTween( thisTransform , scaleDownTweenDuration , scaleDownConfig );
		scaleDownTween.setOnStartHandler( start => occluderColorScript.enabled = true );
		scaleChain.append( scaleDownTween );
		scaleChain.play();
	}
}
