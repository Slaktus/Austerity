using UnityEngine;
using System.Collections;

public class ChainCounterController : MonoBehaviour {
	
	private Transform thisTransform;
	private Transform parentTransform;
	
	void Awake () {
		thisTransform = transform;
		parentTransform = thisTransform.parent;
		thisTransform.position -= ( new Vector3( 0 , 0 , 200 ) );
	}

	public GameObject chainNumber;
	public GameObject totalScore;
	public GameObject globalMultiplier;
	
	private float globalMultiplierCount;
	
	public void SetChainValues( float chainCount , float score ) {
		globalMultiplierCount = float.Parse( globalMultiplier.GetComponent< FontController >().text );
		Debug.Log( "Chain count: " + chainCount );
		Debug.Log( "Total score: " + score );
		chainNumber.GetComponent< FontController >().SetText( chainCount , true );
		totalScore.GetComponent< FontController >().SetText( score , true );
	}
	
	public void StartChain () {
		StartCoroutine( ChainSequence() );
	}
	
	public float ChainEntryDuration;
	public float ShowChainDuration;
	public float ScoreTransitionDuration;
	public float ShowScoreDuration;
	public float ScoreOutroDuration;
	
	IEnumerator ChainSequence () {
		yield return new WaitForSeconds ( 0.05f );
		thisTransform.rotation = Quaternion.AngleAxis( 90 , Vector3.right );
		thisTransform.position += ( new Vector3( 0 , 0 , 200 ) );
		Go.to( thisTransform , ChainEntryDuration , new GoTweenConfig().rotation( Quaternion.AngleAxis( 0 , Vector3.up ) , false ).setEaseType( GoEaseType.ExpoOut ) );
		yield return new WaitForSeconds ( ChainEntryDuration + ShowChainDuration );
		Go.to( transform , ScoreTransitionDuration , new GoTweenConfig().rotation( Quaternion.AngleAxis( 180 , Vector3.down ) , false ).setEaseType( GoEaseType.ExpoInOut ) );
		yield return new WaitForSeconds ( ScoreTransitionDuration + ShowScoreDuration );
		Go.to( transform , ScoreOutroDuration , new GoTweenConfig().rotation( Quaternion.AngleAxis( 90 , Vector3.right ) , false ).setEaseType( GoEaseType.ExpoInOut ) );
		yield return new WaitForSeconds ( ScoreOutroDuration + 0.05f );
		thisTransform.rotation = thisTransform.rotation = Quaternion.AngleAxis( 0 , Vector3.up );
		thisTransform.position -= ( new Vector3( 0 , 0 , 200 ) );
	}
}
