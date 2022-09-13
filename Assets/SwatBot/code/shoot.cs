using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shoot : MonoBehaviour
{
    public Transform shotPoint;
    public GameObject bullet;
    public MeshRenderer PLS;
    [Header("Settings")]

    [Tooltip("Total Ammunition.")]
    [SerializeField]
    private int ammunitionTotal = 110;

    [Tooltip("Magazine ammo")]
    [SerializeField]
    private int Ammo = 30;

    [Tooltip("Magazine Ammunition")]
    [SerializeField]
    private int ammunitionCurrent = 30;
    public AI Bot;
    // Start is called before the first frame update
    public bool Canshoot = true;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CanShoot()
    {
        if(ammunitionCurrent < 1)
        {
            Canshoot = false;
            Debug.Log("false");
            /*Debug.Log(ammunitionCurrent);*/
        }
        else
        {
            /*Debug.Log(ammunitionCurrent);*/
            Canshoot = true;
        }
    }
    public void Reload()
    {
        if(ammunitionCurrent != Ammo)
        {
            if (ammunitionTotal !=0)
            {
                if (ammunitionCurrent < 1 && ammunitionTotal < 1)
                {
                    Bot.GetComponent<Animator>().SetBool("Fire", false);
                    /*Bot.GetComponent<Animator>().SetBool("idle", true);*/
                    Debug.Log("idle2");
                }
                else 
                {
                    // Add reload animation
                    Bot.GetComponent<Animator>().SetBool("Fire", false);
                    Bot.GetComponent<Animator>().SetBool("reload", true);
                    if (Bot.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Reloading") &&
                        Bot.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                    {
                        Bot.GetComponent<Animator>().SetBool("reload", false);
                        if (ammunitionCurrent + ammunitionTotal >= Ammo)
                        {
                            ammunitionTotal = ammunitionTotal - (Ammo - ammunitionCurrent);
                            //Update the value by a certain amount.
                            ammunitionCurrent = Ammo;
                        }
                        else
                        {
                            ammunitionCurrent = ammunitionCurrent + ammunitionTotal;
                            ammunitionTotal = 0;
                        }

                        shot();
                    }
                 }

                
            }
        }
    }

    public void shot()
    {
        CanShoot();
        Debug.Log(Canshoot);
        if(Canshoot)
        {
            Bot.GetComponent<Animator>().SetBool("Fire", true);
            PLS.enabled = true;
            Instantiate(bullet, shotPoint.position, shotPoint.rotation);
            ammunitionCurrent--;
            if(ammunitionCurrent < 1)
            {
                Canshoot = false;
                Reload();
            }
        }
        else
        {
            Reload();
        }
        if(ammunitionCurrent<1 && ammunitionTotal < 1)
        {
            Bot.GetComponent<Animator>().SetBool("Fire", false);
            Bot.GetComponent<Animator>().SetBool("idle", true);
            Debug.Log("idle");
        }
    }
}
