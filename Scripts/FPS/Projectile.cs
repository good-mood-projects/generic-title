using UnityEngine;

public class Projectile : MonoBehaviour {

    //public Color trail; <-- Maybe in the future

    public float speed = 100f;
    public float damage = 50f;
    float timeToDestroy = 2f;


    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

	public void SetDamage(float newDamage)
	{
		damage = newDamage;
	}

    public void Created()
    {
        SetSpeed(speed);
        Destroy(gameObject, timeToDestroy);
        //Destroy it after X seconds
    }

    void FixedUpdate()
    {
        float moveDistance = speed * Time.fixedDeltaTime;
        transform.Translate(Vector3.forward * moveDistance);
    }


    //The proyectile is only miscelaneous, so it wont manage collisions
    /* void OnCollisionEnter(Collision other)
    {
        //TODO: How to interact with other gameobjects 
        if (other.gameObject.GetComponent<Enemy>() != null)
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            enemy.DamageTaken((int)damage);
            GameObject.Destroy(gameObject);
        }

        if (other.gameObject.GetComponent<Obstacle>() != null)
        {
            Obstacle obstacle = other.gameObject.GetComponent<Obstacle>();
            if (obstacle.isDamageable){
                obstacle.DamageTaken((int)damage);
            }
            GameObject.Destroy(gameObject);
        }

    }*/
    

}
