using UnityEngine;
using System.Collections;

public class LogoSpinner : MonoBehaviour {
	
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	public float aTweenDuration;
	public float bTweenDuration;
	public float dTweenDuration;
	public float fTweenDuration;
	public float gTweenDuration;
	public float hTweenDuration;
	public float alignVerticalDuration;
	public float alignHorizontalDuration;
	
	// Update is called once per frame
	void Update () {
		if ( Input.GetKeyDown( KeyCode.A ) ) Go.to( transform , aTweenDuration , new GoTweenConfig().rotation( Quaternion.AngleAxis( 0 , Vector3.up ) , false ).setEaseType( GoEaseType.CircOut ) );
		if ( Input.GetKeyDown( KeyCode.S ) ) Go.to( transform , bTweenDuration , new GoTweenConfig().rotation( Quaternion.AngleAxis( 0 , Vector3.up ) , false ).setEaseType( GoEaseType.QuadOut ) );
		if ( Input.GetKeyDown( KeyCode.D ) ) Go.to( transform , dTweenDuration , new GoTweenConfig().rotation( Quaternion.AngleAxis( 0 , Vector3.up ) , false ).setEaseType( GoEaseType.QuintOut ) );
		if ( Input.GetKeyDown( KeyCode.F ) ) Go.to( transform , fTweenDuration , new GoTweenConfig().rotation( Quaternion.AngleAxis( 0 , Vector3.up ) , false ).setEaseType( GoEaseType.BounceOut ) );
		if ( Input.GetKeyDown( KeyCode.G ) ) Go.to( transform , gTweenDuration , new GoTweenConfig().rotation( Quaternion.AngleAxis( 0 , Vector3.up ) , false ).setEaseType( GoEaseType.SineOut ) );
		if ( Input.GetKeyDown( KeyCode.H ) ) Go.to( transform , hTweenDuration , new GoTweenConfig().rotation( Quaternion.AngleAxis( 0 , Vector3.up ) , false ).setEaseType( GoEaseType.ExpoOut ) );
		if ( Input.GetKeyDown( KeyCode.Keypad0 ) ) Go.to( transform , alignHorizontalDuration , new GoTweenConfig().rotation( Quaternion.AngleAxis( 90 , Vector3.up ) , false ).setEaseType( GoEaseType.ExpoIn ) );
		if ( Input.GetKeyDown( KeyCode.Keypad1 ) ) Go.to( transform , alignVerticalDuration , new GoTweenConfig().rotation( Quaternion.AngleAxis( 90 , Vector3.right ) , false ).setEaseType( GoEaseType.ExpoIn ) );
	}
}
