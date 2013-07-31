using UnityEngine;
using System.Collections;

public class EnemyMeshColorController : MonoBehaviour {

	private Transform thisTransform;
	private Transform mesh;
	private Material meshMaterial;
	private EnemyScaleController scaleScript;
	private Color targetColor;
	
	void Awake() {
		thisTransform = transform;
		mesh = thisTransform.GetChild( 0 );
		meshMaterial = mesh.renderer.material;
		meshMaterial.color = colors[0];
		targetColor = meshMaterial.color;
		scaleScript = gameObject.GetComponent< EnemyScaleController >();
	}
	
	public float damageTweenDuration;
	private GoTween damageTween;
	private GameObject nearestArena;
	private GameObject nearestChamber;
	private Transform targetTransform;
	private Transform targetMesh;
	private float combinedRadii;
	private Color damageColor;
	private Color bufferedColor;
	
	public void TweenToDamageColor () {
		nearestArena = gameControllerScript.FindNearestArena( gameObject );
		targetTransform = nearestArena.transform;
		combinedRadii = targetTransform.localScale.x + thisTransform.localScale.x;
		if ( combinedRadii > Vector2.Distance( targetTransform.position , thisTransform.position ) ) {
			targetTransform = nearestArena.transform.GetChild( 0 );
			bufferedColor = targetTransform.renderer.material.color;
			damageColor = new Color( 1.0f - bufferedColor.r , 1.0f - bufferedColor.g , 1.0f - bufferedColor.b , 1.0f );
		} else {
			nearestChamber = gameControllerScript.FindNearestChamber( gameObject );
			targetTransform = nearestChamber.transform;
			combinedRadii = targetTransform.localScale.x + thisTransform.localScale.x;
			if ( combinedRadii > Vector2.Distance( targetTransform.position , thisTransform.position ) ) {
				targetTransform = nearestArena.transform.GetChild( 0 );
				bufferedColor = targetTransform.renderer.material.color;
				damageColor = new Color( 1.0f - bufferedColor.r , 1.0f - bufferedColor.g , 1.0f - bufferedColor.b , 1.0f );
			} else {
				damageColor = Color.black;
			}
		}
		meshMaterial.color = damageColor;
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
	
	public Color[] colors;
	
	private Vector3 initialScale;
	private Vector3 maxScale;
	private Vector3 scaleRange;
	private Vector3 scaleIncrement;
	private GameObject gameContainer;
	private GameController gameControllerScript;
	
	// Use this for initialization
	void Start () {
		gameContainer = GameObject.FindGameObjectWithTag( "GameContainer" );
		gameControllerScript = gameContainer.GetComponent< GameController >();
		initialScale = scaleScript.initialScale;
		maxScale = scaleScript.maxScale;
		scaleRange = maxScale - initialScale;
		scaleIncrement = scaleRange / colors.Length;
	}
	
	public int currentColorLevel;
	
	private Vector3 currentMaxScale;
	
	// Update is called once per frame
	void Update () {
		if ( thisTransform.localScale.sqrMagnitude > currentMaxScale.sqrMagnitude ) currentMaxScale = thisTransform.localScale;
		for ( int i = 0 ; i < colors.Length ; i++ ) {
			if ( currentMaxScale.sqrMagnitude > Vector3.SqrMagnitude( initialScale + ( scaleIncrement * i ) ) ) {
				targetColor = colors[ i ];
				currentColorLevel = i;
			}
		}
		if ( meshMaterial.color != targetColor ) TweenToTargetColor();
	}
}
