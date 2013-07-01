using UnityEngine;
using System.Collections;

public class ChamberMeshColorController : MonoBehaviour {
	
	private GameObject gameContainer;
	private Transform mesh;
	private Material meshMaterial;
	private GameObject currentAvatar;
	private Transform avatarTransform;
	private Transform thisTransform;
	private GoTweenChain avatarEnterTween;
	private GoTween scaleUpTween;
	private GoTween scaleDownTween;
	private ScaleController scaleControllerScript;
	
	void Awake () {
		mesh = transform.GetChild( 0 );
		meshMaterial = mesh.renderer.material;
		thisTransform = transform;
		scaleControllerScript = gameObject.GetComponent< ScaleController >();
	}
	
	public float flashInDuration;
	public float flashOutDuration;
	
	private Color flashColor;
	private GoTween flashColorTween;
	private GoTweenChain flashInvertChain;
	public Color defaultColor = Color.white;
	
	public void FlashInvert () {
		if ( flashInvertChain == null ) flashInvertChain = new GoTweenChain();
		flashColor = new Color( 1.0f - meshMaterial.color.r , 1.0f - meshMaterial.color.g , 1.0f - meshMaterial.color.b , 1.0f );
		flashColorTween = new GoTween( meshMaterial , flashInDuration , new GoTweenConfig().materialColor( flashColor , GoMaterialColorType.Color , false ).setEaseType( GoEaseType.ExpoOut ) );
		flashInvertChain.append( flashColorTween );
		flashColorTween = new GoTween( meshMaterial , flashOutDuration , new GoTweenConfig().materialColor( meshMaterial.color , GoMaterialColorType.Color , false ).setEaseType( GoEaseType.ExpoOut ) );
		flashInvertChain.append( flashColorTween );
		flashInvertChain.play();
	}
	
	public void InitialToTargetColor () {
		targetColor = initialColor;
	}
	
	public void ActivateChamber () {
		Debug.Log( "Activate chamber" );
		targetColor = initialColor;
		currentAvatar.GetComponent< AvatarWeaponController >().currentChamberId = scaleControllerScript.chamberId;
	}
	
	public void DeactivateChamber () {
		Debug.Log( "Deactivate chamber" );
		if (!scaleControllerScript.isTriggered ) targetColor = Color.white;
	}
	
	public float colorTweenDuration;
	
	private GoTween colorTween;
	
	private void TweenToTargetColor () {
		if ( colorTween == null ) {
			colorTween = new GoTween( meshMaterial , colorTweenDuration , new GoTweenConfig().materialColor( targetColor , GoMaterialColorType.Color , false ).setEaseType( GoEaseType.SineOut ) );
			colorTween.setOnCompleteHandler( complete => colorTween = null );
			Go.addTween( colorTween );
		}
	}
	
	private Color initialColor;
	
	void Start() {
		targetColor = defaultColor;
		initialColor = meshMaterial.color;
		currentAvatar = GameObject.FindGameObjectWithTag( "Avatar" );
		avatarTransform = currentAvatar.transform;
	}
	
	private float combinedRadii;
	private Color targetColor;
	
	void Update () {
		if ( meshMaterial.color != targetColor ) TweenToTargetColor();
	}
}
