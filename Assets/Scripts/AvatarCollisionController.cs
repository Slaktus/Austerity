using UnityEngine;
using System.Collections;

public class AvatarCollisionController : MonoBehaviour {
	
	public GameObject avatarParticles;
	
	private Transform targetTransform;
	private GameObject avatarExplosion = null;
	
	void OnTriggerEnter ( Collider collision ) {
		Debug.Log( "This is my other collider" );
		targetTransform = collision.transform;
		if ( targetTransform.parent != null && targetTransform.parent.name == "Enemy" ) {
			parentTransform.GetChild( 1 ).SendMessage( "DetachFromParent" );
			avatarExplosion = Instantiate( avatarParticles , transform.position , Quaternion.LookRotation( Vector3.back ) ) as GameObject;
			GameObject.FindGameObjectWithTag( "GameContainer" ).GetComponent< GameController >().EndSession();
			Go.killAllTweensWithTarget( parentTransform );
			parentTransform.GetComponent< AvatarMovementController >().enabled = false;
			parentTransform.GetComponent< AvatarWeaponController >().enabled = false;
			parentTransform.GetComponent< Collider >().enabled = false;
			collider.enabled = false;
			Go.to ( parentTransform , 0.025f , new GoTweenConfig().scale( Vector3.zero , false ).setEaseType( GoEaseType.BackIn ) ).setOnCompleteHandler( destroy => Destroy( parentTransform.gameObject ) );
		}
	}
	
	private Transform parentTransform;
				
	// Use this for initialization
	void Start () {
		parentTransform = transform.parent;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
