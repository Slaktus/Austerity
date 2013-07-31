using UnityEngine;
using System.Collections;

public class SkillTreeController : MonoBehaviour {
	
	public SkillTreeConfiguration[] treeConfigurations;
	private GameObject gameContainer;
	private GameController gameControllerScript;
	private Transform thisTransform;
	private int treeCount;
	
	void Start () {
		thisTransform = transform;
		gameContainer = GameObject.FindGameObjectWithTag( "GameContainer" );
		gameControllerScript = gameContainer.GetComponent< GameController >();
		foreach( SkillTreeConfiguration treeConfig in treeConfigurations ) {
			generateSkillTree( treeConfig.treeName );
			treeCount++;
		}
	}
	
	public Transform chamber;
	public Transform hubChamber;
	public float chamberStartHeight = 300;
	private Transform currentChamber;
	private Transform previousChamber;
	private Transform meshContainer;
	private Transform currentMesh;
	private Transform occluderContainer;
	private Vector3 chamberPosition;
	private ScaleController scaleControllerScript;
	private ArrayList rootList = new ArrayList();
	private ArrayList currentTreeList = new ArrayList();
	
	private void generateSkillTree ( string treeName ) {
		currentTreeList.Clear();
		GameObject treeRoot = new GameObject();
		treeRoot.name = treeName;
		treeRoot.transform.parent = thisTransform;
		previousChamber = treeRoot.transform;
		rootList.Add( treeRoot );
		if ( rootList.Count > 0 ) {
			for ( int i = 0 ; i < treeConfigurations[ rootList.Count - 1 ].numberOfChambers ; i++ ) {
				SkillTreeConfiguration currentTreeConfig = treeConfigurations[ rootList.Count - 1 ];
				chamberPosition = Vector3.forward * chamberStartHeight;
				if ( rootList.Count == 1 ) currentChamber = Instantiate( hubChamber , chamberPosition + ( Vector3.up * 45 ), Quaternion.AngleAxis( 90 , Vector3.forward ) ) as Transform;
				if ( rootList.Count == 2 ) currentChamber = Instantiate( hubChamber , chamberPosition + ( Vector3.right * 40 ) + ( Vector3.down * 24.5f ), Quaternion.identity ) as Transform;
				if ( rootList.Count == 3 ) currentChamber = Instantiate( hubChamber , chamberPosition + ( Vector3.left * 40 ) + ( Vector3.down * 24.5f ) , Quaternion.identity ) as Transform;
				if ( i != 0 ) currentChamber.gameObject.SetActive( false );
				else previousChamber = currentChamber;
				currentChamber.name = treeName + "Chamber0" + ( i + 1 );
				scaleControllerScript = currentChamber.GetChild( 0 ).GetComponent< ScaleController >();
				scaleControllerScript.maxScale = Vector3.one * treeConfigurations[ rootList.Count - 1 ].chamberRadii[ i ];
				scaleControllerScript.maxScale.z = 31;
				scaleControllerScript.initialScale = scaleControllerScript.maxScale / 2;
				scaleControllerScript.initialScale.z = 31;
				scaleControllerScript.chamberId = i;
				scaleControllerScript.generation = i + 1;
				gameControllerScript.AddChamber( currentChamber.gameObject );
				currentChamber.parent = treeRoot.transform;
				meshContainer = currentChamber.GetChild( 0 );
				currentMesh = meshContainer.GetChild( 0 );
				currentMesh.renderer.material.color = currentTreeConfig.colorScheme[ i ];
				meshContainer =  currentChamber.GetChild( 1 );
				scaleControllerScript = meshContainer.GetChild( 0 ).GetComponent< ScaleController >();
				scaleControllerScript.initialScale = Vector3.one * ( treeConfigurations[ rootList.Count - 1 ].chamberRadii[ i ] - 0.5f);
				scaleControllerScript.initialScale.z = 90;
				scaleControllerScript = meshContainer.GetChild( 1 ).GetComponent< ScaleController >();
				scaleControllerScript.initialScale = Vector3.one * ( treeConfigurations[ rootList.Count - 1 ].chamberRadii[ i ] );
				scaleControllerScript.initialScale.z = 180;
			}
		}
		else {
			for ( int i = 0 ; i < treeConfigurations[ rootList.Count - 1 ].numberOfChambers ; i++ ) {
				SkillTreeConfiguration currentTreeConfig = treeConfigurations[ rootList.Count - 1 ];
				if ( i == 2 ) previousChamber = currentTreeList[ 0 ] as Transform;
				if ( i == 3 ) previousChamber = currentTreeList[ 1 ] as Transform;
				if ( i == 4 ) previousChamber = currentTreeList[ 2 ] as Transform;
				if ( i == 5 ) previousChamber = currentTreeList[ 0 ] as Transform;
				if ( i == 7 ) previousChamber = currentTreeList[ 5 ] as Transform;
				if ( i == 8 ) previousChamber = currentTreeList[ 5 ] as Transform;
				if ( i == 9 ) {
					previousChamber = currentTreeList[ 0 ] as Transform;
					currentChamber = currentTreeList[ 8 ] as Transform;
					float chamberDistance = Vector2.Distance( previousChamber.transform.position , currentChamber.transform.position );
					previousChamber = previousChamber.transform.GetChild( 0 );
					currentChamber = currentChamber.transform.GetChild( 0 );
					chamberDistance -= previousChamber.localScale.x;
					chamberDistance += currentChamber.localScale.x;
					//previousChamber = currentTreeList[ 0 ] as Transform;
					chamberPosition = previousChamber.TransformPoint( Quaternion.AngleAxis( currentTreeConfig.chamberAngles[ i ] , Vector3.forward ) * ( Vector3.up * ( chamberDistance / 2 + currentTreeConfig.chamberDistances[ i ] ) ) );
					chamberPosition.z = chamberStartHeight - 20;
				} else {
					chamberPosition = previousChamber.TransformPoint( Quaternion.AngleAxis( currentTreeConfig.chamberAngles[ i ] , Vector3.forward ) * ( Vector3.up * currentTreeConfig.chamberDistances[ i ] ) );
					chamberPosition.z = chamberStartHeight;
				}
				currentChamber = Instantiate( chamber , chamberPosition , Quaternion.identity ) as Transform;
				if ( i > 0 ) currentChamber.gameObject.SetActive( false );
				currentChamber.name = treeName + "Chamber0" + ( i + 1 );
				scaleControllerScript = currentChamber.GetChild( 0 ).GetComponent< ScaleController >();
				scaleControllerScript.maxScale = Vector3.one * treeConfigurations[ rootList.Count - 1 ].chamberRadii[ i ];
				scaleControllerScript.maxScale.z = 31;
				scaleControllerScript.initialScale = scaleControllerScript.maxScale / 2;
				scaleControllerScript.initialScale.z = 31;
				scaleControllerScript.chamberId = i;
				if ( i == 0 ) {
					scaleControllerScript.generation = 2;
					scaleControllerScript.chamberActivationIndex = 1;
				}
				if ( i == 1 ) {
					scaleControllerScript.generation = 3;
					scaleControllerScript.chamberActivationIndex = 3;
				}
				if ( i == 2 ) {
					scaleControllerScript.generation = 3;
					scaleControllerScript.chamberActivationIndex = 4;
				}
				if ( i == 3 ) {
					scaleControllerScript.generation = 4;
					scaleControllerScript.chamberActivationIndex = 5;
				}
				if ( i == 4 ) {
					scaleControllerScript.generation = 4;
					scaleControllerScript.chamberActivationIndex = 5;
				}
				if ( i == 5 ) {
					scaleControllerScript.generation = 5;
					scaleControllerScript.chamberActivationIndex = 6;
				}
				if ( i == 6 || i == 7 ) scaleControllerScript.generation = 6;
				if ( i == 8 ) scaleControllerScript.generation = 7;
				if ( i == 9 ) scaleControllerScript.generation = 8;
				currentTreeList.Add( currentChamber );
				gameControllerScript.AddChamber( currentChamber.gameObject );
				currentChamber.parent = treeRoot.transform;
				meshContainer = currentChamber.GetChild( 0 );
				currentMesh = meshContainer.GetChild( 0 );
				currentMesh.renderer.material.color = currentTreeConfig.colorScheme[ i ];
				meshContainer =  currentChamber.GetChild( 1 );
				scaleControllerScript = meshContainer.GetChild( 0 ).GetComponent< ScaleController >();
				scaleControllerScript.initialScale = Vector3.one * ( treeConfigurations[ rootList.Count - 1 ].chamberRadii[ i ] - 0.5f);
				scaleControllerScript = meshContainer.GetChild( 1 ).GetComponent< ScaleController >();
				scaleControllerScript.initialScale = Vector3.one * ( treeConfigurations[ rootList.Count - 1 ].chamberRadii[ i ] );
				previousChamber = currentChamber;
				meshContainer =  currentChamber.GetChild( 1 );
				scaleControllerScript = meshContainer.GetChild( 0 ).GetComponent< ScaleController >();
				scaleControllerScript.initialScale = Vector3.one * ( treeConfigurations[ rootList.Count - 1 ].chamberRadii[ i ] - 0.5f);
				scaleControllerScript.initialScale.z = 90;
				scaleControllerScript = meshContainer.GetChild( 1 ).GetComponent< ScaleController >();
				scaleControllerScript.initialScale = Vector3.one * ( treeConfigurations[ rootList.Count - 1 ].chamberRadii[ i ] );
				scaleControllerScript.initialScale.z = 180;
			}
			if ( rootList.Count == 2 ) {
				treeRoot = rootList[ 1 ] as GameObject;
				treeRoot.transform.rotation = Quaternion.AngleAxis( 90 , Vector3.forward );
			}
			if ( rootList.Count == 3 ) {
				treeRoot = rootList[ 2 ] as GameObject;
				treeRoot.transform.rotation = Quaternion.AngleAxis( -90 , Vector3.forward );
			}
		}
	}
}
