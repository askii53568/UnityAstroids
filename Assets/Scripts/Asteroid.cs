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

    private void OnCollisionEnter2D(Collision2D collision){
        
        if(!ProfileSingleton.instance.newPlayer){
            //verify if the asteroid collided with a bullet
            if(collision.gameObject.tag== "Bullet"){
                //check if the size of the asteroid is big enough to be split
                if((this.size * 0.5f)>= this.minSize){
                    CreateSplit();
                    CreateSplit();
                } else {
                    CreatePowerUp();
                }
                //Find the Game manager object in the scene if it exists and execute the asteroidDestroyed method
                FindObjectOfType<GameManager>().AsteroidDestroyed(this);
                //regardless of the condition destroy the old asteroid
                Destroy(this.gameObject);
            }
        }else{
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
        half.trajectory = Random.insideUnitCircle.normalized*1000.0f;
        half.SetTrajectory(half.trajectory); 
        Debug.Log("half asteroid position", half);   
        }

    private void CreatePowerUp()
    {
        PowerUp powerUp;
        int rnd = Random.Range(0,100);
        if (rnd < 16) {
            Vector3 position = this.transform.position;
            Quaternion rotation = new Quaternion();
            powerUp = Instantiate(powerUpPre, position, rotation);

            if (rnd < 3) {
                powerUp.powerUpEffect = new SpeedPowerUp(50);
                powerUp.spriteNum = 0;
            } else if (rnd < 4) {
                powerUp.powerUpEffect = new TripleShotPowerUp();
                powerUp.spriteNum = 1;
            } else if (rnd < 7) {
                powerUp.powerUpEffect = new TurnPowerUp(600);
                powerUp.spriteNum = 2;
            } else if (rnd < 9) {
                powerUp.powerUpEffect = new InvinciblePowerUp();
                powerUp.spriteNum = 3;
            } else if (rnd < 10) {
                powerUp.powerUpEffect = new HeartPowerUp();
                powerUp.spriteNum = 4;
            } else if (rnd < 11) {
                powerUp.powerUpEffect = new WipePowerUp();
                powerUp.spriteNum = 5;
            } else if (rnd < 16) {
                powerUp.powerUpEffect = new CashPowerUp();
                powerUp.spriteNum = 6;
            }
        } 
    }
}
