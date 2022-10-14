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
	public MeshRenderer PLS;
	public float dist;
	NavMeshAgent nav;
	public float radius = 35;
	public float fireRadius = 25;
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
			nav.enabled = false;
			transform.LookAt(Player.transform.position);
			transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
/*			gameObject.GetComponent<Animator>().SetBool("idle", false);
			gameObject.GetComponent<Animator>().SetBool("Fire", true);*/
			elapsed += Time.deltaTime;
			if (elapsed >= 0.17f)
			{
				elapsed = elapsed % 0.17f;
				Shoot.shot();
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

}
