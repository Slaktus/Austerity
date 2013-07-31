using UnityEngine;
using System.Collections;

public class AvatarWeaponController : MonoBehaviour {
	
	public AvatarProjectileWeaponConfig[] projectileWeaponConfigs;
	public AvatarMissileWeaponConfig[] missileWeaponConfigs;
	public AvatarBeamWeaponConfig[] beamWeaponConfigs;
	public int currentChamberId;
	
	private AvatarProjectileWeaponConfig currentProjectileConfig;
	private Transform thisTransform;
	private ScaleController scaleControllerScript;
	private GameObject nearestChamber;
	private Transform chamberTransform = null;
	private AvatarMovementController movementController;
	private float defaultDrag;
	
	void Awake () {
		thisTransform = transform;
		movementController = gameObject.GetComponent<AvatarMovementController>();
		defaultDrag = movementController.movementDrag;
	}
	
	private Vector3 mouseAimDirection;
	private Vector3 cursorWorldPosition;
	
	private Vector3 AimTowardsMouse () {
		Vector3 cursorPosition = Input.mousePosition;
		cursorWorldPosition = Camera.mainCamera.ScreenToWorldPoint(cursorPosition);
		cursorWorldPosition.z = transform.position.z;
		mouseAimDirection = Vector3.Normalize( cursorWorldPosition - transform.position );
		return mouseAimDirection;
	}
	
	public GameObject bullet;
	private Vector3 launchDirection;
	private Vector3 destinationPoint;
	private GameObject newBullet;
	private float angleIncrement;
	private float currentAngle;
	private WeaponBulletController bulletScript;
	private GoTween bulletMovementTween;
	private float bulletFireTime;
	private GameObject fireEffect;
	
	private void FireBullet ( Vector3 fireDirection ) {
		fireEffect = Instantiate( projectileWeaponConfigs[ currentChamberId ].fireEffect , thisTransform.position + Vector3.back * 2 , Quaternion.LookRotation( mouseDirection , Vector3.forward ) ) as GameObject;
		fireEffect.transform.parent = thisTransform.parent;
		if ( projectileWeaponConfigs[ currentChamberId ].bulletAngleRange != 0 ) {
			angleIncrement = projectileWeaponConfigs[ currentChamberId ].bulletAngleRange / projectileWeaponConfigs[ currentChamberId ].bulletsPerShot;
			currentAngle = ( -projectileWeaponConfigs[ currentChamberId ].bulletAngleRange / 2 ) + ( angleIncrement / 2 );
		}
		for ( int i = 0 ; projectileWeaponConfigs[ currentChamberId ].bulletsPerShot > i ; i++ ) {
			currentAngle += Random.Range( -projectileWeaponConfigs[ currentChamberId ].bulletAngleJitter , projectileWeaponConfigs[ currentChamberId ].bulletAngleJitter );
			launchDirection = Quaternion.AngleAxis( currentAngle , Vector3.forward ) * fireDirection;
			currentAngle += angleIncrement;
			newBullet = Instantiate( bullet , thisTransform.position , Quaternion.LookRotation( launchDirection.normalized ) ) as GameObject;
			newBullet.transform.parent = thisTransform.parent;
			bulletScript = newBullet.GetComponent< WeaponBulletController >();
			bulletScript.pushbackForce = projectileWeaponConfigs[ currentChamberId ].pushbackForce;
			bulletScript.scaleIncrementMultiplier = projectileWeaponConfigs[ currentChamberId ].scaleIncrementMultiplier;
			bulletScript.dragIncrementMultiplier = projectileWeaponConfigs[ currentChamberId ].dragIncrementMultiplier;
		}
		bulletFireTime = Time.time;
	}
	
	public GameObject missile;
	
	private float missileFireTime;
	private GameObject newMissile;
	private WeaponMissileController missileScript;
	
	private void FireMissile (Vector3 fireDirection) {
		fireEffect = Instantiate( missileWeaponConfigs[ currentChamberId ].fireEffect , thisTransform.position + Vector3.back * 2 , Quaternion.LookRotation( mouseDirection , Vector3.forward ) ) as GameObject;
		fireEffect.transform.parent = thisTransform.parent;
		if ( missileWeaponConfigs[ currentChamberId ].missileAngleRange != 0 ) {
			angleIncrement = missileWeaponConfigs[ currentChamberId ].missileAngleRange / missileWeaponConfigs[ currentChamberId ].missilesPerShot;
			currentAngle = ( -missileWeaponConfigs[ currentChamberId ].missileAngleRange / 2 ) + ( angleIncrement / 2 );
		}
		for ( int i = 0; missileWeaponConfigs[ currentChamberId ].missilesPerShot > i; i++ ) {
			currentAngle += Random.Range( -missileWeaponConfigs[ currentChamberId ].missileAngleJitter , missileWeaponConfigs[ currentChamberId ].missileAngleJitter );
			launchDirection = Quaternion.AngleAxis(currentAngle, Vector3.forward) * fireDirection;
			newMissile = Instantiate( missile, transform.position, Quaternion.LookRotation( launchDirection.normalized ) ) as GameObject;
			newMissile.transform.parent = thisTransform.parent;
			missileScript = newMissile.GetComponent< WeaponMissileController >();
			missileScript.missileDuration = missileWeaponConfigs[ currentChamberId ].missileDuration;
			missileScript.explosionForce = missileWeaponConfigs[ currentChamberId ].explosionForce;
			missileScript.explosionRadius = missileWeaponConfigs[ currentChamberId ].explosionRadius;
			missileScript.missileHomingDistance = missileWeaponConfigs[ currentChamberId ].missileHomingDistance;
			missileScript.timeBeforeHoming = missileWeaponConfigs[ currentChamberId ].timeBeforeHoming;
			missileScript.missileTargetRange = missileWeaponConfigs[ currentChamberId ].missileTargetRange;
			missileScript.missileHomingSpeed = missileWeaponConfigs[ currentChamberId ].missileHomingSpeed;
			missileScript.missileCruisingSpeed = missileWeaponConfigs[ currentChamberId ].missileCruisingSpeed;
			missileScript.missileHomingSpeedDecayRate = missileWeaponConfigs[ currentChamberId ].missileHomingSpeedDecayRate;
			missileScript.missileCruisingSpeedDecayRate = missileWeaponConfigs[ currentChamberId ].missileCruisingSpeedDecayRate;
			missileScript.missileTurningSpeed = missileWeaponConfigs[ currentChamberId ].missileTurningSpeed;
			missileScript.scaleIncrementMultiplier = missileWeaponConfigs[ currentChamberId ].scaleIncrementMultiplier;
			missileScript.dragIncrementMultiplier = missileWeaponConfigs[ currentChamberId ].dragIncrementMultiplier;
			currentAngle += angleIncrement;
		}
		missileFireTime = Time.time;
	}
	
	public GameObject beam;
	
	private GameObject currentBeam;
	private WeaponBeamController beamScript;
	
	private void FireBeam (Vector3 fireDirection) {
		if ( currentBeam == null ) {
			currentBeam = Instantiate( beam , transform.position, Quaternion.LookRotation( fireDirection ) ) as GameObject;
			currentBeam.transform.parent = thisTransform.parent;
			rigidbody.AddForce( -fireDirection * beamWeaponConfigs[ currentChamberId ].initialBeamRecoilStrength );
			beamScript = currentBeam.GetComponent< WeaponBeamController >();
			beamScript.beamHeight = beamWeaponConfigs[ currentChamberId ].beamHeight;
			beamScript.beamWidth = beamWeaponConfigs[ currentChamberId ].beamWidth;
			beamScript.heightScaleUpDuration = beamWeaponConfigs[ currentChamberId ].heightScaleUpDuration;
			beamScript.widthScaleUpDuration = beamWeaponConfigs[ currentChamberId ].widthScaleUpDuration;
			beamScript.widthScaleDownDuration = beamWeaponConfigs[ currentChamberId ].widthScaleDownDuration;
			beamScript.scaleIncrementMultiplier = beamWeaponConfigs[ currentChamberId ].scaleIncrementMultiplier;
			beamScript.dragIncrementMultiplier = beamWeaponConfigs[ currentChamberId ].dragIncrementMultiplier;
		} else if ( Input.GetMouseButton( 0 ) ) {
			rigidbody.AddForce( -fireDirection * beamWeaponConfigs[ currentChamberId ].ongoingBeamRecoilStrength );
			movementController.movementDrag = beamWeaponConfigs[ currentChamberId ].beamDrag;
			currentBeam.transform.position = transform.position;
			currentBeam.transform.rotation = Quaternion.LookRotation( fireDirection );
		}
	}
	
	private void RemoveBeam () {
		beamScript.ShrinkBeam();
		movementController.movementDrag = defaultDrag;
	}
	
	private GameObject gameContainer;
	private GameController gameControllerScript;
	private GameObject weaponReport;
	private GameObject weaponType;
	private GameObject weaponLevel;
	
	
	// Use this for initialization
	void Start () {
		weaponReport = GameObject.FindGameObjectWithTag( "WeaponReport" );
		gameContainer = GameObject.FindGameObjectWithTag( "GameContainer" );
		gameControllerScript = gameContainer.GetComponent< GameController >();
		nearestChamber = gameControllerScript.FindNearestChamber( gameObject );
		chamberTransform = nearestChamber.transform;
		scaleControllerScript = nearestChamber.GetComponent< ScaleController >();
		currentChamberId = scaleControllerScript.chamberId;
	}
	
	public GameObject bulletFireEffect;

	
	private Vector3 mouseDirection;
	private Transform nearestArena;
	private string chamberType;
	private string bufferedChamberType;
	private float combinedRadii;
	private GameObject bufferedChamber;
	
	// Update is called once per frame
	void Update () {
		if ( gameControllerScript.gameObject != null ) nearestArena = gameControllerScript.FindNearestArena( gameObject ).transform;
		if ( nearestChamber != null ) {
			bufferedChamber = nearestChamber;
			combinedRadii = nearestArena.GetComponent< ScaleController >().maxScale.x + nearestChamber.GetComponent< ScaleController >().maxScale.x;
			if ( Vector2.Distance( nearestArena.position , nearestChamber.transform.position ) > combinedRadii ) {
				Debug.Log( "Out of distance" );
				nearestChamber.GetComponent< ChamberMeshColorController >().DeactivateChamber();
			} else nearestChamber.GetComponent< ChamberMeshColorController >().ActivateChamber();
		}
		if ( nearestArena != null )	{
			combinedRadii = nearestArena.GetComponent< ScaleController >().maxScale.x + ( thisTransform.localScale.x * 2 );
			if ( combinedRadii > Vector2.Distance( nearestArena.position , thisTransform.position ) ) {
				nearestChamber = nearestArena.GetComponent< ArenaMeshColorController >().nearestChamber;
				if ( nearestChamber != null ) {
					chamberType = nearestChamber.transform.parent.parent.name;
					if ( string.IsNullOrEmpty( bufferedChamberType ) ) {
						bufferedChamberType = chamberType;
					}
					if ( chamberType != bufferedChamberType ) {
						weaponReport.GetComponent< WeaponReportController >().PrepareText( chamberType.ToUpper() , "LEVEL" + ( scaleControllerScript.chamberId + 1 ) );
						weaponReport.GetComponent< WeaponReportController >().StartChain();
					}
					bufferedChamberType = chamberType;
				}
				else nearestChamber = gameControllerScript.FindNearestChamber( gameObject );
				if (nearestChamber != bufferedChamber ) nearestChamber.GetComponent< ChamberMeshColorController >().ActivateChamber();
				Debug.Log( "Something is up here" );
			}
		}
		if ( bufferedChamber != null && bufferedChamber != nearestChamber ) {
			bufferedChamber.GetComponent< ChamberMeshColorController >().DeactivateChamber();
		}
		if ( Input.GetKey( KeyCode.Mouse0 ) && chamberType == "Malkut" && Time.time > ( bulletFireTime +  projectileWeaponConfigs[ currentChamberId ].bulletFireInterval ) ) {
			mouseDirection = AimTowardsMouse();
			FireBullet( mouseDirection );
		}
		else if ( Input.GetKey( KeyCode.Mouse0 ) && chamberType == "Sefirot" && Time.time > ( missileFireTime +  missileWeaponConfigs[ currentChamberId ].missileFireInterval ) ) {
			mouseDirection = AimTowardsMouse();
			FireMissile( mouseDirection );
		}
		else if ( Input.GetKey( KeyCode.Mouse0 ) && chamberType == "Kelipot" ) {
			mouseDirection = AimTowardsMouse();
			FireBeam( mouseDirection );
		} else if ( currentBeam != null && Input.GetKeyUp( KeyCode.Mouse0 ) && chamberType == "Kelipot" ) {
			RemoveBeam();
		} else if ( currentBeam != null && chamberType != "Kelipot" ) {
			beamScript.CleanUpBeam();
			movementController.movementDrag = defaultDrag;
		}
	}
}
