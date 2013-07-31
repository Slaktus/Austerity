using UnityEngine;
using System.Collections;

public class GenerationMeshColorController : MonoBehaviour {
	
	public bool isInverted;
	private Transform parentContainer;
	private Transform counterContainer;
	private GenerationColorController colorControllerScript;
	private Material thisMaterial;
	private Material meshMaterial;
	private bool chamberOverride;
	
	void Start () {
		parentContainer = transform.parent;
		counterContainer = parentContainer.parent;
		thisMaterial = renderer.material;
		if ( transform.name == "GenerationCap" && transform.parent.parent.parent.parent.parent.parent.name == "SkillTreeContainer" ) {
			chamberOverride = true;
		}
		else {
			colorControllerScript = counterContainer.gameObject.GetComponent< GenerationColorController >();
			meshMaterial = colorControllerScript.meshMaterial;

		}
	}
	
	// Update is called once per frame
	void Update () {
		if ( !chamberOverride ) {
			if ( meshMaterial != null ) {
				if ( isInverted ) thisMaterial.color = new Color( 1.0f - meshMaterial.color.r , 1.0f - meshMaterial.color.g , 1.0f - meshMaterial.color.b , 1.0f );
				else thisMaterial.color = new Color(  meshMaterial.color.r , meshMaterial.color.g , meshMaterial.color.b , 1.0f );
			}
			else if ( meshMaterial == null ) {
				colorControllerScript = counterContainer.gameObject.GetComponent< GenerationColorController >();
				meshMaterial = colorControllerScript.meshMaterial;
			}
		}
	}
}
