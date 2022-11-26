using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shoot : MonoBehaviour
{
    
    public Transform shotPoint;
    public GameObject bullet;
    public GameObject player;
    public MeshRenderer PLS;
    public float LastShootTime;
    public Transform gunBarrell;
    public TrailRenderer bulletTrail;
    AudioSource audioSource;
    public AudioClip shootClip;
    [Header("Settings")]

    [Tooltip("Total Ammunition.")]
    [SerializeField]
    private int ammunitionTotal = 110;

    [SerializeField]
    public bool AddBulletSpread = true;
    [SerializeField]
    public Vector3 BulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField]
    public ParticleSystem shootingSystem;
    [SerializeField]
    public ParticleSystem metalHit;
    [SerializeField]
    public ParticleSystem sandHit;
    [SerializeField]
    public TrailRenderer BulletTrail;
    [SerializeField]
    public float shootDelay = 0.5f;
    [SerializeField]
    public LayerMask Mask;
    

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
/*        audioSource = GetComponent<AudioSource>();
        muzzleFlash.Stop();*/
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
           /* Debug.Log("false");*/
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
/*                if (ammunitionCurrent < 1 && ammunitionTotal < 1)
                {
                    Bot.GetComponent<Animator>().SetBool("Fire", false);
                    Bot.GetComponent<Animator>().SetBool("idle", true);
                    Debug.Log("idle2");
                }
                else 
                {*/
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
                        if (ammunitionCurrent > 1)
                        {
                
                            shot();
                        }
                       
                    }
                 

                
            }
        }
    }

    public void shot()
    {
        CanShoot();
        if (Canshoot)
        {
            if (LastShootTime + shootDelay < Time.time)
            {
                
                
                Vector3 direction = GetDirection();
                Ray raySeePlayerM4 = new Ray();
                raySeePlayerM4.origin = shotPoint.transform.position;
                raySeePlayerM4.direction = shotPoint.forward;
                //Debug.DrawRay(raySeePlayerM4.origin, raySeePlayerM4.direction * 100f, Color.green);
                RaycastHit hittt;
                if (Physics.Raycast(raySeePlayerM4, out hittt))
                {
                   // if (hittt.collider.tag == "Player")//shoots only if m4 directed into player
                    //{
                        if (Physics.Raycast(shotPoint.position, direction, out RaycastHit hit, float.MaxValue, Mask) || true)
                        {

                            //Bot.GetComponent<Animator>().SetBool("Fire", true);
                            PLS.enabled = true;
                            shootingSystem.Play();
                            TrailRenderer trail = Instantiate(BulletTrail, shotPoint.position, Quaternion.identity);
                            trail.material.color = new Color(255, 215, 0);
                            StartCoroutine(SpawnTrail(trail, hit));
                            LastShootTime = Time.time;
                            ammunitionCurrent--;
                        }
                    //}
                }
                //
             
                /*muzzleBehaviour.Effect();*/
                if (ammunitionCurrent < 1)
                {
                    Canshoot = false;
                    Reload();
                }
            }
        }
        else
        {
            Reload();
        }
    }
    public Vector3 GetDirection()
    {
        Vector3 direction = shotPoint.transform.forward;
        if (AddBulletSpread)
        {
            direction += new Vector3(
                Random.Range(-BulletSpreadVariance.x, BulletSpreadVariance.x),
                Random.Range(-BulletSpreadVariance.y, BulletSpreadVariance.y),
                Random.Range(-BulletSpreadVariance.z, BulletSpreadVariance.z)
            );
            direction.Normalize();
        }
        return direction;
    }
    public IEnumerator SpawnTrail(TrailRenderer Trail, RaycastHit Hit)
    {
        float time = 0;
        Vector3 startPosition = Trail.transform.position;
        while (time < 1)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, Hit.point, time);
            /*            Trail.transform.position = transform.position + (shotPoint.transform.forward * 200); 
            */
            time += Time.deltaTime / Trail.time;

            yield return null;
        }
        /* Bot.GetComponent<Animator>().SetBool("Fire", false);*/
        Trail.transform.position = Hit.point;
        if (Hit.collider)
        {
            if (Hit.collider.tag == "metal")
            {
                Instantiate(metalHit, Hit.point, Quaternion.LookRotation(Hit.normal));
            }
            else if (Hit.collider.tag == "sand")
            {
                Instantiate(sandHit, Hit.point, Quaternion.LookRotation(Hit.normal));
            }
            else if (Hit.collider.tag == "Player")
            {
                Instantiate(sandHit, Hit.point, Quaternion.LookRotation(Hit.normal));
            }
        }


    
        
        

        Destroy(Trail.gameObject, Trail.time);
    }
}
