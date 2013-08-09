using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {
	
	public GameObject gameContainer;
	
	private GameObject currentGameContainer;
	
	public void CreateNewGameSession () {
		currentGameContainer = Instantiate( gameContainer , transform.position , Quaternion.identity ) as GameObject;
		currentGameContainer.transform.parent = transform;
	}
	
	// Use this for initialization
	void Start () {
		CreateNewGameSession();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
