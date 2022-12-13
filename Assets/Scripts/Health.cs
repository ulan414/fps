using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InfimaGames.LowPolyShooterPack;

public class Health : MonoBehaviour
{
    public int health;
    public int maxHealth;
    public AI Bot;
    public AI Bot1;
    public HealthText helathtext;
    public HealthBar changeHealth;
    public bool isBot = true;
    public Vector3 playerTeleportAfterDeathPosition = new Vector3(0f,0f,0f);
    public GameOverScreen GameOverScreen;
    public GameObject canvas;
    float timer = 0f;
    //script myScript;
    // Start is called before the first frame update
    void Start()
    {
        changeHealth.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void TakeDammage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            if (isBot)
            {
                AI ai = gameObject.GetComponent<AI>();
                ai.dying = true;
            }
            else
            {

                //make smthng with player after death
                GameOverScreen.Setup(10);
                gameObject.GetComponent<Collider>().enabled = false;
                //myScript = gameObject.GetComponent<Movement>();
                //gameObject.GetComponent<Character>().enabled = false;
                Destroy(Bot.gameObject);
                Destroy(Bot1.gameObject); 
                Destroy(helathtext);
                gameObject.GetComponent<Character>().enabled = false;
                Destroy(gameObject.GetComponent<Movement>());
                //gameObject.GetComponent<Movement>().enabled = false;
                Destroy(gameObject.GetComponent<Rigidbody>(),0.1f);
                //gameObject.GetComponent<PlayerInput>().enabled = false;

                foreach (Transform t in transform)
                {
                    foreach (Component com in t.GetComponents<Component>())
                    {
                        if (com != t.GetComponent<Transform>())
                            Destroy(com);
                    }
                }
                //Destroy(gameObject,1);
                    //canvas.SetActive(false);
                }
        }
        changeHealth.SetHealth(health);
    }
    public void AddHealth(int bonusHealth)
    {
        health += bonusHealth;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        changeHealth.SetHealth(health);
    }
}
