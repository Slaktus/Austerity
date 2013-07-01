using UnityEngine;
using System.Collections;

public class FromMeshColorController : MonoBehaviour {

	public GameObject mesh;
	public bool isInverted;
	public bool manualColorIsSet;
	private Material thisMaterial;
	private Material meshMaterial;
	
	// Use this for initialization
	void Start () {
		if ( !manualColorIsSet ) {
			meshMaterial = mesh.renderer.material;
			thisMaterial = renderer.material;
			if ( isInverted ) thisMaterial.color = new Color( 1.0f - meshMaterial.color.r , 1.0f - meshMaterial.color.g , 1.0f - meshMaterial.color.b , 1.0f );
			if ( !isInverted ) thisMaterial.color = new Color(  meshMaterial.color.r , meshMaterial.color.g , meshMaterial.color.b , 1.0f );
		}
	}
	
	// Update is called once per frame
	void Update () {
		if ( !manualColorIsSet ) {
			if ( isInverted ) thisMaterial.color = new Color( 1.0f - meshMaterial.color.r , 1.0f - meshMaterial.color.g , 1.0f - meshMaterial.color.b , 1.0f );
			else thisMaterial.color = new Color(  meshMaterial.color.r , meshMaterial.color.g , meshMaterial.color.b , 1.0f );
		}
	}
}
