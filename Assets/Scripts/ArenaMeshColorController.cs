using UnityEngine;
using System.Collections;

public class ArenaMeshColorController : MonoBehaviour {
	
	public float colorCheckInterval;
	
	IEnumerator CheckColor () {
		GetColorFromChamber();
		yield return new WaitForSeconds( colorCheckInterval );
		StartCoroutine( CheckColor() );
	}
	
	public GameObject nearestChamber;
	
	private GameObject gameContainer;
	private Transform mesh;
	private GameController gameControllerScript;
	private Material meshMaterial;
	private Transform thisTransform;
	private ScaleController scaleControllerScript;
	
	void Awake () {
		mesh = transform.GetChild( 0 );
		meshMaterial = mesh.renderer.material;
		thisTransform = transform;
		gameContainer = GameObject.FindGameObjectWithTag( "GameContainer" );
		gameControllerScript = gameContainer.GetComponent< GameController >();
		scaleControllerScript = gameObject.GetComponent< ScaleController >();
	}
	
	public bool isInverted;
	
	private Transform targetChamber;
	private Transform targetMesh;
	private GameObject nearestArena;
	private Transform targetArena;
	private float combinedRadii;
	private Color targetColor;
	private ArenaMeshColorController colorScript;
	public string chamberType;
	
	public void GetColorFromChamber () {
		nearestChamber = gameControllerScript.FindNearestChamber( gameObject );
		targetChamber = nearestChamber.transform;
		combinedRadii = ( targetChamber.localScale.x ) + ( thisTransform.localScale.x * 2 );
		if ( targetChamber != null && combinedRadii > Vector2.Distance( targetChamber.position , thisTransform.position ) ) {
			targetMesh = targetChamber.GetChild( 0 );
			targetColor = targetMesh.renderer.material.color;
			if ( isInverted ) targetColor = new Color( 1.0f - targetColor.r , 1.0f - targetColor.g , 1.0f - targetColor.b , 1.0f );
			else targetColor = new Color( targetColor.r , targetColor.g , targetColor.b , 1.0f );
			chamberType = targetMesh.parent.parent.parent.name;
		} else {
			targetColor = Color.black;
			chamberType = "";
		}
			/*{
			//This is horrible. Do not want.
			nearestArena = gameControllerScript.FindNearestArena( gameObject );
			if ( nearestArena != null ) {
				targetArena = nearestArena.transform;
				combinedRadii = ( targetArena.localScale.x * 2 ) + ( thisTransform.localScale.x );
				if ( combinedRadii > Vector2.Distance( targetArena.position , thisTransform.position ) ) {
					targetMesh = targetArena.GetChild( 0 );
					targetColor = targetMesh.renderer.material.color;
					if ( targetMesh.renderer.material.color != Color.black ) targetColor = targetMesh.renderer.material.color;
				} else {
					targetColor = Color.black;
				}
			} else {
				targetColor = Color.black;
			}
		}*/
	}
	
	public float flashInvertDuration;
	private Color flashColor;
	private GoTween flashColorTween;
	private bool isFlashing = false;
	
	public void FlashInvert () {
		flashColor = new Color( 1.0f - targetColor.r , 1.0f - targetColor.g , 1.0f - targetColor.b , 1.0f );
		isFlashing = true;
		flashColorTween = new GoTween( meshMaterial , flashInvertDuration , new GoTweenConfig().materialColor( flashColor , GoMaterialColorType.Color , false ).setEaseType( GoEaseType.ExpoOut ) );
		flashColorTween.setOnCompleteHandler( complete => isFlashing = false );
		Go.addTween( flashColorTween );
	}
	
	public float colorTweenDuration;
	private GoTween colorTween;
	
	private void TweenToTargetColor () {
		if ( colorTween == null ) {
			colorTween = new GoTween( meshMaterial , colorTweenDuration , new GoTweenConfig().materialColor( targetColor , GoMaterialColorType.Color , false ).setEaseType( GoEaseType.SineOut ) );
			colorTween.setOnCompleteHandler( complete => colorTween = null );
			Go.addTween( colorTween );
		} else GetColorFromChamber();
	}
	
	private GameObject currentAvatar;
	private Transform avatarTransform;
	
	// Use this for initialization
	void Start () {
		currentAvatar = GameObject.FindGameObjectWithTag( "Avatar" );
		avatarTransform = currentAvatar.transform;
		StartCoroutine( CheckColor() );
	}

	// Update is called once per frame
	void Update () {
		if ( !isFlashing && meshMaterial.color != targetColor ) TweenToTargetColor();
		combinedRadii = avatarTransform.localScale.x + scaleControllerScript.maxScale.x;
	}
}
