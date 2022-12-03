using System;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.AI;
public sealed class AI : AIBehavior
{
	private float timer;
	public GameObject Player;
	public Transform shotPoint;
	public Transform seePlayer;
	public TrailRenderer bulletTrailFirstPart;
	public float rotateAngle = 0f;
	private int ammunitionCurrent;
	private magazineBehaviorBot equippedMagazine;
	public shoot Shoot;
	public Rotate rotateCs;
	float elapsed = 0f;
	float elapsedFirstPart = 0f;
	public float LastShootTimeFirstPart;
	public float shootingDelay;
	float lastSeenTime = 0f;
	public float dist;
	NavMeshAgent nav;
	public float radius = 35;
	public float fireRadius = 25;
	bool firstshoot = true;
	bool hasSeenPlayer;
	float lastShootingTime = 0f;
	bool isShooting = false;
	bool needToMove = false;
	float needToMoveTime = 0f;
	float rotationSpeed = 14f;
	float countEven = 0;
	[SerializeField]
	public TrailRenderer BulletTrailFirstPart;
	[SerializeField]
	public ParticleSystem shootingSystem;
	//Laser(red)
	public LineRenderer laserLineRenderer;
	public float laserWidth = 0.01f;
	public float laserMaxLength = 5f;
	public bool LaserEnabled = false;
	bool mustShoot = false;
	//Bots
	public bool swat = false;
	Vector3 directionCor = new Vector3(0, 0, 0);
	Vector3 directionCor1 = new Vector3(0, 0, 0);
	Vector3 directionCorMust = new Vector3(0, 0, 0);
	Vector3 directionCorMust1 = new Vector3(0, 0, 0);
	Vector3 playerPositionWhenShoot = new Vector3(0, 0, 0);
	float lastLaserTime = 0f;
	// Start is called before the first frame update
	void Start()
	{
		nav = GetComponent<NavMeshAgent>();
		Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };
		laserLineRenderer.SetPositions(initLaserPositions);
		laserLineRenderer.SetWidth(laserWidth, laserWidth);

	}

	// Update is called once per frame
	void Update()
	{
		//laser

		dist = Vector3.Distance(Player.transform.position, transform.position);

		if (dist > radius)
		{
			nav.enabled = false;
			gameObject.GetComponent<Animator>().SetBool("Fire", false);
			gameObject.GetComponent<Animator>().SetBool("run", false);
			gameObject.GetComponent<Animator>().SetBool("idle", true);
		}
		if (dist < radius && dist > fireRadius)
		{
			nav.enabled = true;
			nav.SetDestination(Player.transform.position);
			gameObject.GetComponent<Animator>().SetBool("Fire", false);
			gameObject.GetComponent<Animator>().SetBool("idle", false);
			gameObject.GetComponent<Animator>().SetBool("run", true);
		}
		if (dist < fireRadius)
		{
			if (needToMove)
			{
				if (Time.time - needToMoveTime > 0.3f)
				{
					needToMove = false;
				}
			}
            if (mustShoot)
            {
				if(Time.time - lastLaserTime > 1.5f)
				{
					mustShoot = false;
					laserLineRenderer.enabled = false;
				}
			}
			gameObject.GetComponent<Animator>().SetBool("run", false);
			if (Shoot.ammunitionCurrent != Shoot.Ammo)
			{
				if (Shoot.ammunitionCurrent < 1)
				{
					if (Shoot.ammunitionTotal != 0)
					{
						Shoot.Reload();
						lastShootingTime = Time.time;
					}
				}
			}
			/*			transform.LookAt(Player.transform.position);
						transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);*/
			//shoot triger(raycast)

			if (!mustShoot)
			{
				directionCor = (Player.transform.position - seePlayer.transform.position + new Vector3(0, 1.7f, 0)).normalized;
				//rotate character
				directionCor1 = (Quaternion.AngleAxis(rotateAngle, transform.up) * (Player.transform.position - seePlayer.transform.position + new Vector3(0, 1.1f, 0))).normalized;
			}
			else	
            {
				directionCor = directionCorMust;
				directionCor1 = directionCorMust1;
			}
			Rotate(directionCor1);
			rotateCs.SetDirection(directionCor);
			//Debug.Log(Time.time - lastLaserTime);
			/*			if (mustShoot)
						{
							Debug.Log("true");
						}
						else
						{
							Debug.Log("false");
						}*/
			//laser

			Ray raySeePlayer = new Ray();
			//raySeePlayer.origin = transform.position + new Vector3(0.18f, 1.1f, 0);
			raySeePlayer.origin = seePlayer.transform.position;
			raySeePlayer.direction = directionCor;
			//Debug.DrawRay(raySeePlayer.origin, directionCor * fireRadius, Color.blue);
			RaycastHit hit;
			if (Physics.Raycast(raySeePlayer, out hit))
			{
				if ((hit.collider.tag == "Player"))
				{
					nav.enabled = false;
					hasSeenPlayer = true;
					lastSeenTime = Time.time;
					//shooting delay
					gameObject.GetComponent<Animator>().SetBool("run", false);
					gameObject.GetComponent<Animator>().SetBool("Fire", true);
					gameObject.GetComponent<Animator>().SetBool("idle", false);
					elapsed += Time.deltaTime;


						if (LaserEnabled)
						{
							if (elapsed >= LastShootTimeFirstPart)
							{
							ShootLaserFromTargetPosition(shotPoint.transform.position, directionCor, fireRadius);
                            Ray raySeePlayerM4Laser = new Ray();
                            raySeePlayerM4Laser.origin = shotPoint.transform.position;
                            raySeePlayerM4Laser.direction = shotPoint.forward;
                            //Debug.DrawRay(raySeePlayerM4Laser.origin, raySeePlayerM4Laser.direction * 100f, Color.green);
                            RaycastHit hitLaser;
                            if (Physics.Raycast(raySeePlayerM4Laser, out hitLaser))
                            {
                                playerPositionWhenShoot = hitLaser.point;
								//Debug.Log(playerPositionWhenShoot);
                                Shoot.setPlayerPosition(playerPositionWhenShoot);
                            }
                            laserLineRenderer.enabled = true;
							mustShoot = true;
							directionCorMust = directionCor;
							directionCorMust1 = directionCor1;
							
							//Shoot.setPlayerPosition(directionCorMust);
							lastLaserTime = Time.time;
							}
						}
					if (elapsed >= shootingDelay)
					{
						elapsed = elapsed % shootingDelay;
						Ray raySeePlayerM4 = new Ray();
						raySeePlayerM4.origin = shotPoint.transform.position;
						raySeePlayerM4.direction = shotPoint.forward;
						//Debug.DrawRay(raySeePlayerM4.origin, raySeePlayerM4.direction * 100f, Color.green);
						RaycastHit hittt;
						if (Physics.Raycast(raySeePlayerM4, out hittt))
							{
								if (hittt.collider.tag == "Player")
								{
								Shoot.shot();
								mustShoot = false;
								isShooting = true;
								lastShootingTime = Time.time;
								gameObject.GetComponent<Animator>().SetBool("Fire", false);
								gameObject.GetComponent<Animator>().SetBool("idle", true);
								laserLineRenderer.enabled = false;
							}
							else
								{
									isShooting = false;
								}
							}
					}
					
				}
				else if (mustShoot)
				{
					//elapsed += Time.deltaTime;
					elapsed += Time.deltaTime;
					if (elapsed >= shootingDelay)
					{
						elapsed = elapsed % shootingDelay;
						Shoot.shot();
						mustShoot = false;
						isShooting = true;
						lastShootingTime = Time.time;
						gameObject.GetComponent<Animator>().SetBool("Fire", false);
						gameObject.GetComponent<Animator>().SetBool("idle", true);
						laserLineRenderer.enabled = false;
					}
				}
				else
				{
					//searching = true;
					//searchingTime = Time.time;
					if (!needToMove)
					{
						needToMove = true;
						needToMoveTime = Time.time;
					}
					isShooting = false;
					lastShootingTime = Time.time;
					//gameObject.GetComponent<Animator>().SetBool("Fire", false);
					if (Time.time - lastSeenTime > 10f && hasSeenPlayer)
					{
						hasSeenPlayer = false;
					}
					if (hasSeenPlayer && !mustShoot)
					{
						nav.enabled = true;
						nav.SetDestination(Player.transform.position);
						gameObject.GetComponent<Animator>().SetBool("Fire", false);
						gameObject.GetComponent<Animator>().SetBool("idle", false);
						gameObject.GetComponent<Animator>().SetBool("run", true);
					}
					else if (!hasSeenPlayer && !mustShoot)
					{
						gameObject.GetComponent<Animator>().SetBool("run", false);
						gameObject.GetComponent<Animator>().SetBool("idle", true);
						nav.enabled = false;
					}
				}
				//if raycast can see the player but bot isnt shooting because raycast from the gun cannot see the player, so he must move (0.3f secs)
				if (gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Firing_Rifle") && !isShooting && (Time.time - lastShootingTime > 2.3f) && lastShootingTime != 0f && !mustShoot)
				{
					needToMove = true;
					needToMoveTime = Time.time;
					nav.enabled = true;
					nav.SetDestination(Player.transform.position);
					gameObject.GetComponent<Animator>().SetBool("Fire", false);
					gameObject.GetComponent<Animator>().SetBool("idle", false);
					gameObject.GetComponent<Animator>().SetBool("run", true);
				}
			}
			else if (mustShoot)
            {
				elapsed += Time.deltaTime;
                if (elapsed >= shootingDelay)
                {
					elapsed = elapsed % shootingDelay;
					Shoot.shot();
					mustShoot = false;
					isShooting = true;
					lastShootingTime = Time.time;
					gameObject.GetComponent<Animator>().SetBool("Fire", false);
					gameObject.GetComponent<Animator>().SetBool("idle", true);
					laserLineRenderer.enabled = false;
				}
			}
		}
	}
	void Rotate(Vector3 directionCor)
	{

		if (directionCor == Vector3.zero)
			return;

		Quaternion rotation = Quaternion.LookRotation(directionCor);

		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
		transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
	}
	public IEnumerator SpawnTrail(TrailRenderer Trail, RaycastHit Hit)
	{
		float time = 0;
		Vector3 startPosition = Trail.transform.position;
		while (time < 5)
		{
			Trail.transform.position = Vector3.Lerp(startPosition, Hit.point, time);
			/*            Trail.transform.position = transform.position + (shotPoint.transform.forward * 200); 
            */
			time += Time.deltaTime / Trail.time;

			yield return null;
		}
		Destroy(Trail.gameObject, Trail.time);
	}
	void ShootLaserFromTargetPosition(Vector3 targetPosition, Vector3 direction, float length)
	{
		Ray ray = new Ray(targetPosition, direction);
		RaycastHit raycastHit;
		Vector3 endPosition = targetPosition + (length * direction);

		if (Physics.Raycast(ray, out raycastHit, length))
		{
			endPosition = raycastHit.point;
		}

		laserLineRenderer.SetPosition(0, targetPosition);
		laserLineRenderer.SetPosition(1, endPosition);
	}
}
		

/*	void OnTriggerStay(Collider col)
	{

		if (col.tag == "Player")
		{
			elapsed += Time.deltaTime;
			Player = col.gameObject;
            *//*gameObject.GetComponent<Animator>().SetBool("Fire", true);*//*
            transform.LookAt(col.transform.position);
			transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

			if (elapsed >= 0.2f)
			{
				elapsed = elapsed % 0.2f;
				Shoot.shot();
			}
		}
	}
	void OnTriggerExit(Collider col)
	{
		if (col.tag == "Player")
		{
			Debug.Log("exit");
			gameObject.GetComponent<Animator>().SetBool("Fire", false);
			*//*Bullet.destroy = true;*//*
			PLS.enabled = false;
		}
	}*/


