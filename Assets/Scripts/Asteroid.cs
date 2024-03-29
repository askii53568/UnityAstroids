using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Asteroid : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public Sprite[] sprites;
    public PowerUp powerUpPre;

    public float size = 100.0f;
    public int sprite = 1;

    public float minSize = 50.0f;
    public float maxSize = 150.0f;

    public static float speed = 200.0f;

    public float maxLifetime = 30.0f;

    private Rigidbody2D asteroid;

    public Vector2 trajectory;


    private void Awake()
    {
        speed = 300.0f;
        spriteRenderer = GetComponent<SpriteRenderer>();
        asteroid = GetComponent<Rigidbody2D>();

    }
    void Start()
    {
        spriteRenderer.sprite = sprites[this.sprite];
        //Change the scale of the spawned asteroid instead of the size
        //If the size is changed, then the mass is changed which will affect other physical properties
        this.transform.localScale = Vector3.one * this.size;
        //the larger the size the larger the mass
        asteroid.mass = this.size;

    }

    public void SetTrajectory(Vector2 direction)
    {
        asteroid.AddForce(direction * speed);

        Destroy(this.gameObject, this.maxLifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (FindObjectOfType<TutorialGameManager>() == null)
        {
            //verify if the asteroid collided with a bullet
            if (collision.gameObject.tag == "Bullet")
            {
                //check if the size of the asteroid is big enough to be split
                if ((this.size * 0.5f) >= this.minSize)
                {
                    CreateSplit();
                    CreateSplit();
                }
                else
                {
                    CreatePowerUp();
                }
                //Find the Game manager object in the scene if it exists and execute the asteroidDestroyed method
                FindObjectOfType<GameManager>().AsteroidDestroyed(this);
                //regardless of the condition destroy the old asteroid
                Destroy(this.gameObject);
            }
        }
        else
        {
            FindObjectOfType<TutorialGameManager>().AsteroidDestroyed(this);
            //regardless of the condition destroy the old asteroid
            Destroy(this.gameObject);
        }

    }

    //Split big asteroids into small ones on collission
    private void CreateSplit()
    {
        //get the position of the old asteroid
        Vector2 position = this.transform.position;
        //offset the position of the new half asteroids by a little
        position += Random.insideUnitCircle * 200.0f;
        Asteroid half = Instantiate(this, position, this.transform.rotation);

        //Cut the size in half
        half.size = this.size * 0.5f;
        //Give the new asteroids a random new trajectory
        half.trajectory = Random.insideUnitCircle.normalized * 1000.0f;
        half.SetTrajectory(half.trajectory);
        Debug.Log("half asteroid position", half);
    }

    private void CreatePowerUp()
    {
        PowerUp powerUp;

        Vector3 position = this.transform.position;
        Quaternion rotation = new Quaternion();
        powerUp = Instantiate(powerUpPre, position, rotation);
        System.Random rnd = new System.Random();
        int chance = rnd.Next(0, 4);
        switch (chance)
        {
            default:
                powerUp.powerUpEffect = new ShieldPowerUp();
                powerUp.spriteNum = 4;
                break;
            case 0:
                powerUp.powerUpEffect = new FreezePowerUp();
                powerUp.spriteNum = 0;
                break;
            case 1:
                powerUp.powerUpEffect = new SpeedPowerUp(100);
                powerUp.spriteNum = 1;
                break;
            case 2:
                powerUp.powerUpEffect = new InvinciblePowerUp();
                powerUp.spriteNum = 3;
                break;
            case 3:
                powerUp.powerUpEffect = new ShieldPowerUp();
                powerUp.spriteNum = 4;
                break;   
        }
    }
}
