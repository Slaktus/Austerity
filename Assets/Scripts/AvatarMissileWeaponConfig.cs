using UnityEngine;
using System.Collections;

public class AvatarMissileWeaponConfig : MonoBehaviour {
	
	public float missileDuration;
	public int missilesPerShot;
	public float missileAngleRange;
	public float missileAngleJitter;
	public float missileFireInterval;
	public GameObject fireEffect;
	public float scaleIncrementMultiplier = 1.0f;
	public float dragIncrementMultiplier = 1.0f;
	public float pushbackForce;
	public float explosionForce;
	public float explosionRadius;
	public float missileHomingDistance;
	public float timeBeforeHoming;
	public float missileTargetRange;
	public float missileHomingSpeed;
	public float missileCruisingSpeed;
	public float missileHomingSpeedDecayRate;
	public float missileCruisingSpeedDecayRate;
	public float missileTurningSpeed;
	
}
