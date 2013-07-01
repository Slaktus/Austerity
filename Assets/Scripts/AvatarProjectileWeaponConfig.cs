using UnityEngine;
using System.Collections;

public class AvatarProjectileWeaponConfig : MonoBehaviour {

	public int bulletsPerShot;
	public float bulletAngleRange;
	public float bulletAngleJitter;
	public float bulletFireInterval;
	public float bulletRange;
	public float travelDuration;
	public GameObject fireEffect;
	public float scaleIncrementMultiplier = 1.0f;
	public float dragIncrementMultiplier = 1.0f;
	public float pushbackForce;
	
}
