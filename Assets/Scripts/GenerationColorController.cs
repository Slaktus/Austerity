using UnityEngine;
using System.Collections;

public class GenerationColorController : MonoBehaviour {
	
	public Material meshMaterial;
	private Transform generationContainer;
	private Transform meshContainer;
	private Transform mesh;
	
	void Start () {
		generationContainer = transform.parent;
		meshContainer = generationContainer.parent.GetChild ( 0 );
		mesh = meshContainer.GetChild ( 0 );
		meshMaterial = mesh.renderer.material;
	}
}
