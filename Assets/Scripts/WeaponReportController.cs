using UnityEngine;
using System.Collections;

public class WeaponReportController : MonoBehaviour {
		
	public GameObject weaponText;
	public GameObject levelText;
	
	private Transform thisTransform;
	private Transform parentTransform;
	private FontController weaponTextController;
	private FontController levelTextController;
	
	
	void Awake () {
		thisTransform = transform;
		parentTransform = thisTransform.parent;
		thisTransform.position -= ( new Vector3( 0 , 0 , 200 ) );

		weaponTextController = weaponText.GetComponent< FontController >();
		levelTextController = levelText.GetComponent< FontController >();
	}
	
	public void StartChain () {
		StartCoroutine( ChainSequence () );
	}
	
	public float EntryDuration;
	public float ShowWeaponDuration;
	public float OutroDuration;
	
	IEnumerator ChainSequence () {
		yield return new WaitForSeconds ( 0.05f );
		thisTransform.rotation = Quaternion.AngleAxis( -90 , Vector3.up );
		thisTransform.position += ( new Vector3( 0 , 0 , 200 ) );
		Go.to( thisTransform , EntryDuration , new GoTweenConfig().rotation( Quaternion.AngleAxis( 0 , Vector3.up ) , false ).setEaseType( GoEaseType.ExpoOut ) );
		yield return new WaitForSeconds ( EntryDuration + ShowWeaponDuration );
		Go.to( transform , OutroDuration , new GoTweenConfig().rotation( Quaternion.AngleAxis( -90 , Vector3.up ) , false ).setEaseType( GoEaseType.ExpoInOut ) );
		yield return new WaitForSeconds ( OutroDuration + 0.05f );
		thisTransform.rotation = thisTransform.rotation = Quaternion.AngleAxis( 0 , Vector3.up );
		thisTransform.position -= ( new Vector3( 0 , 0 , 200 ) );
	}

	
	public void PrepareText ( string weapon , string level ) {
		weaponTextController.SetText( weapon );
		levelTextController.SetText( level );
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
