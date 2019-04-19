using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

    //Actual class dedicated for a Gun
    public Transform projectileStartPosition;           //Point in which the projectiles are generated
	public Projectile projectile;                       //Projectile model & stats
    public Projectile secondShotProjectile;             //Second shot projectile model & stats
    public float timeBetweenShotsMS = 200f;              //Time between shots
    public float timeBetweenSecondFireTimeMS = 1000f;    //Time between second fire shots
    public float projectileRange    = 100f;              //Projectile speed

    float projectileDamage;		       
    float nextShotTime;
    float nextSecondFireTime;

    void Start()
    {
        nextShotTime = 0f;
        nextSecondFireTime = 0f;
    }

    public void Shoot(ref float nextShot, float timeBetweenShots, Projectile proyectileToCreate)
    {
        if (Time.time > nextShot)
        {
            projectileDamage = proyectileToCreate.damage;
            nextShot = Time.time + timeBetweenShots / 1000;
            RaycastHit hit;
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out hit,projectileRange))
            {
                //Debug.DrawLine(transform.position, hit.point, Color.green,2.0f,false); //See raycasts for debug purposes
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.GetComponent<AIBehaviour>() != null)
                    {
                        Debug.Log("Enemy hit");
                        AIBehaviour enemy = hit.collider.gameObject.GetComponent<AIBehaviour>();
                        enemy.DamageTaken((int)projectileDamage);
                    }

                    if (hit.collider.gameObject.GetComponent<Obstacle>() != null)
                    {
                        Obstacle obstacle = hit.collider.gameObject.GetComponent<Obstacle>();
                        if (obstacle.isDamageable)
                        {
                            obstacle.DamageTaken((int)projectileDamage);
                        }
                    }
                }
            }

            //{// Misc thing hapen here
                //TODO animation, fire light particles and sound of the gun
            //    Projectile newProjectile = Instantiate(proyectileToCreate, projectileStartPosition.position, projectileStartPosition.rotation) as Projectile;
           //     newProjectile.Created();    // Instantiation of speed etc will be made in the proyectile Created()
           // }
        }
    }

    public void PrimaryFire()
    {
        Shoot(ref nextShotTime, timeBetweenShotsMS, projectile);
    }

    public void SecondFire()
    {
        Shoot(ref nextSecondFireTime, timeBetweenSecondFireTimeMS, secondShotProjectile);
    }
}
