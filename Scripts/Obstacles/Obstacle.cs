using UnityEngine;
using System.Collections;

/*
 * An Obstacle that has damageable capabilities. Overrides Death method.
 */ 

[RequireComponent(typeof(Rigidbody))]
public class Obstacle : Damageable 
{
	public int damageToDeal;
	public bool high_obstacle = false;
	public bool low_obstacle = false;
    public bool isDamageable;
    public int initialHp;

	Rigidbody _rigidBody;
	Collider _collider;

	// Use this pre initialization
	void Awake () 
	{
        _hp = initialHp;
		_collider = GetComponent<Collider>();
		if(_collider == null)
		{
			Debug.LogError("An obstacle has no collider atached to it (" + gameObject.name + ")");
		}

		_rigidBody = GetComponent<Rigidbody>();
		_rigidBody.isKinematic = true; //Avoids the object to push or be pushed by other rigidbodys
	}

	// Use this for initialization
	void Start()
	{
		if(damageToDeal == 0) //Gives info about wich object is wrong set and why to avoid future unspotted problems
		{
			Debug.LogError("An obstacle cant be dealing 0 damage on collision (" + gameObject.name + ")");
		}
	}

    // In case of hitting another Damageable object
	public void Hitting(Damageable unit)
	{
        int damage = damageToDeal;
        if (isDamageable)
        {
            // Function that implements dealed damage based on current HP. TODO: Decide which function to implement
            damage = damageToDeal * hp / startingHp;
        }
		unit.DamageTaken(damage);
		Avoided();
	}

    //Function that disables the collider (i.e.: to avoid hitting an object twice)
	public void Avoided()
	{
		_collider.enabled = false;
	}

    // Death function calls base function and destroys the obstacle.
    public override void Death()
    {
        base.Death();
        damageToDeal = 0;
        Destroy(gameObject);
    }

    protected void moveObstacle(Vector3 pos)
    {
        _rigidBody.MovePosition(pos);
    }
}
