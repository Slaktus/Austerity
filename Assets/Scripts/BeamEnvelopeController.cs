using UnityEngine;
using System.Collections;

public class BeamEnvelopeController : MonoBehaviour {
	
	private void ScaleUpTween () {
		Go.to( transform , 0.15f , new GoTweenConfig().scale( new Vector3( transform.parent.localScale.x  , transform.parent.localScale.y * 1.25f , transform.parent.localScale.z ) , false ).setEaseType( GoEaseType.SineInOut )  ).setOnCompleteHandler( complete => ScaleDownTween() );
	}
	
	private void ScaleDownTween () {
		Go.to( transform , 0.3f , new GoTweenConfig().scale( new Vector3( transform.parent.localScale.x  , transform.parent.localScale.y * 0.25f , transform.parent.localScale.z ) , false ).setEaseType( GoEaseType.SineInOut )  ).setOnCompleteHandler( complete => ScaleUpTween() );
	}
	
	// Use this for initialization
	void Start () {
		ScaleUpTween();
	}
}
