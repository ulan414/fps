using System;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.AI;
public sealed class AI : AIBehavior
{
    private float timer;
    public GameObject Player;
    private int ammunitionCurrent;
    private magazineBehaviorBot equippedMagazine;
	public shoot Shoot;
	float elapsed = 0f;
	public float shootingDelay;
	float lastSeenTime = 0f;
	public MeshRenderer PLS;
	public float dist;
	NavMeshAgent nav;
	public float radius = 35;
	public float fireRadius = 25;
	bool firstshoot = true;
	bool hasSeenPlayer;
	// Start is called before the first frame update
	void Start()
    {
		nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
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
			transform.LookAt(Player.transform.position);
			transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
			/*			gameObject.GetComponent<Animator>().SetBool("idle", false);
						gameObject.GetComponent<Animator>().SetBool("Fire", true);*/

			//Setting up Vector3's for rays
			/*			Vector3 rayPosition = new Vector3(transform.position.x, 1.1f, transform.position.z);
                        Vector3 leftRayRotation = Quaternion.AngleAxis(-5, transform.up) * transform.forward;
                        Vector3 rightRayRotation = Quaternion.AngleAxis(5, transform.up) * transform.forward;

                        //Constructing rays
                        Ray rayCenter = new Ray(rayPosition, transform.forward);
                        Ray rayLeft = new Ray(rayPosition, leftRayRotation);
                        Ray rayRight = new Ray(rayPosition, rightRayRotation);

                        Debug.DrawRay(rayPosition, transform.forward * fireRadius, Color.red);
                        Debug.DrawRay(rayPosition, leftRayRotation * fireRadius, Color.blue);
                        Debug.DrawRay(rayPosition, rightRayRotation * fireRadius, Color.blue);

                        RaycastHit hit1;
                        RaycastHit hit2;
                        RaycastHit hit3;*/


            Ray raySeePlayer = new Ray();
            raySeePlayer.origin = transform.position + new Vector3(0, 1.1f, 0);
			Vector3 directionCor = (Player.transform.position - raySeePlayer.origin + new Vector3(0, 1.1f, 0)).normalized;
			raySeePlayer.direction = directionCor;
			Debug.DrawRay(raySeePlayer.origin, directionCor * fireRadius, Color.red);
            RaycastHit hit;
			Debug.Log(Player.transform.position);
			if (Physics.Raycast(raySeePlayer, out hit))
			{


				/*            Ray raySeePlayer2 = new Ray();
							raySeePlayer2.origin = transform.position + new Vector3(0, 1.1f, 0);
							raySeePlayer2.direction = (Player.transform.position) * fireRadius;
							Debug.DrawRay(raySeePlayer2.origin, raySeePlayer2.direction * fireRadius, Color.red);
							RaycastHit hitt;
							Physics.Raycast(raySeePlayer, out hitt);
							Debug.Log(Player.transform.position);*/
				/*            Physics.Raycast(rayCenter, out hit1);
							Physics.Raycast(rayLeft, out hit2);
							Physics.Raycast(rayRight, out hit3);
							if (Physics.Raycast(rayCenter, out hit1) || Physics.Raycast(rayLeft, out hit2) || Physics.Raycast(rayRight, out hit3))
							{*/
				if (hit.collider.tag == "Player")
				{
					Debug.Log("seen");
					nav.enabled = false;
					hasSeenPlayer = true;
					lastSeenTime = Time.time;
					//shooting delay
					elapsed += Time.deltaTime;
					/*					if (firstshoot) {
											shootingDelay = 0.25f;
											gameObject.GetComponent<Animator>().SetBool("Fire", true);
											gameObject.GetComponent<Animator>().SetBool("idle", false);
										}*/
					if (elapsed >= shootingDelay)
					{
						elapsed = elapsed % 0.17f;
						gameObject.GetComponent<Animator>().SetBool("Fire", true);
						gameObject.GetComponent<Animator>().SetBool("idle", false);
						Shoot.shot();
						//firstshoot = false;
					}
				}
				else
				{
					firstshoot = true;
					gameObject.GetComponent<Animator>().SetBool("Fire", false);
					gameObject.GetComponent<Animator>().SetBool("idle", true);
					if (Time.time - lastSeenTime > 10f && hasSeenPlayer)
					{
						hasSeenPlayer = false;
					}
					if (hasSeenPlayer)
					{
						nav.enabled = true;
					}
					else
					{
						nav.enabled = false;
					}
					if (nav.enabled)
					{
						nav.enabled = true;
						nav.SetDestination(Player.transform.position);
						gameObject.GetComponent<Animator>().SetBool("Fire", false);
						gameObject.GetComponent<Animator>().SetBool("idle", false);
						gameObject.GetComponent<Animator>().SetBool("run", true);
					}
				}
			}
		}
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


