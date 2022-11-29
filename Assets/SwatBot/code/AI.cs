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
	public float rotateAngle = 0f;
	private int ammunitionCurrent;
	private magazineBehaviorBot equippedMagazine;
	public shoot Shoot;
	float elapsed = 0f;
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
	bool searching = false;
	float needToMoveTime = 0f;
	float searchingTime = 0f;
	float rotationSpeed = 14f;
	// Start is called before the first frame update
	void Start()
	{
		nav = GetComponent<NavMeshAgent>();
	}

	// Update is called once per frame
	void Update()
	{
        if (needToMove || searching)
        {
			if(Time.time - needToMoveTime > 0.3f)
            {
				needToMove = false;
            }
			if(Time.time - searchingTime > 0.3f)
            {
				searching = false;
            }
        }
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
			Vector3 directionCor = (Player.transform.position - seePlayer.transform.position + new Vector3(0, 1.1f, 0)).normalized;
			//rotate character
			Vector3 directionCor1 = (Quaternion.AngleAxis(rotateAngle, transform.up) * (Player.transform.position - seePlayer.transform.position + new Vector3(0, 1.1f, 0))).normalized;
			Rotate(directionCor1);
			
			Ray raySeePlayer = new Ray();
			//raySeePlayer.origin = transform.position + new Vector3(0.18f, 1.1f, 0);
			raySeePlayer.origin = seePlayer.transform.position;
			raySeePlayer.direction = directionCor;
			//Debug.DrawRay(raySeePlayer.origin, directionCor * fireRadius, Color.blue);
			RaycastHit hit;
			if (Physics.Raycast(raySeePlayer, out hit))
			{
				if (hit.collider.tag == "Player" && !needToMove)
				{
					nav.enabled = false;
					hasSeenPlayer = true;
					lastSeenTime = Time.time;
					//shooting delay
					elapsed += Time.deltaTime;

					gameObject.GetComponent<Animator>().SetBool("run", false);
					gameObject.GetComponent<Animator>().SetBool("Fire", true);
					gameObject.GetComponent<Animator>().SetBool("idle", false);
					if (gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Firing_Rifle"))
					{
						Debug.Log("Firing");
					}
					else if (gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("idle"))
					{
						Debug.Log("idle");
					}
					else if (gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Run"))
					{
						Debug.Log("run");
					}
					else if (gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Reloading"))
					{
						Debug.Log("Reloading");
					}
					if (elapsed >= shootingDelay)
					{
						elapsed = elapsed % shootingDelay;
						Ray raySeePlayerM4 = new Ray();
						raySeePlayerM4.origin = shotPoint.transform.position;
						raySeePlayerM4.direction = shotPoint.forward;
						Debug.DrawRay(raySeePlayerM4.origin, raySeePlayerM4.direction * 100f, Color.green);
						RaycastHit hittt;
						if (Physics.Raycast(raySeePlayerM4, out hittt))
						{
							if (hittt.collider.tag == "Player") 
							{
								Shoot.shot();
								isShooting = true;
								lastShootingTime = Time.time;
                            }
                            else
                            {
								isShooting = false;
                            }
						}
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
					Debug.Log("Nottt");
					lastShootingTime = Time.time;
					gameObject.GetComponent<Animator>().SetBool("Fire", false);
					if (Time.time - lastSeenTime > 10f && hasSeenPlayer)
					{
						hasSeenPlayer = false;
					}
					if (hasSeenPlayer)
					{
						nav.enabled = true;
						nav.SetDestination(Player.transform.position);
						gameObject.GetComponent<Animator>().SetBool("Fire", false);
						gameObject.GetComponent<Animator>().SetBool("idle", false);
						gameObject.GetComponent<Animator>().SetBool("run", true);
					}
					else
					{
						gameObject.GetComponent<Animator>().SetBool("run", false);
						gameObject.GetComponent<Animator>().SetBool("idle", true);
						nav.enabled = false;
					}
				}
				//if raycast can see the player but bot isnt shooting because raycast from the gun cannot see the player, so he must move (0.3f secs)
				if (gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Firing_Rifle") && !isShooting && (Time.time - lastShootingTime > 2.3f) && lastShootingTime != 0f)
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


