using UnityEngine;
using System.Collections;

public class Damageable : MonoBehaviour {

    // Parent class in charge of managing HP and death events.
    public bool godMode
    {
        get
        {
            return _godMode;
        }
    }
    public int startingHp;	//Damageable starting health
    public bool godModeSelector;
	public int hp 
	{
		get 
		{
			return _hp;
		}
	}

	protected int _hp;		//Damageable actual health
    protected bool _godMode;

	//Manages the Death animation and destruction of the damageable, allows overriding in child objects
	public virtual void Death()
	{
        Debug.Log(gameObject.name + " is dead");
	}

	//Manages the hp value changes
	public void DamageTaken(int amount)
	{
		_hp -= amount;
        Debug.Log(gameObject.name + " takes " + amount + " damage.");
		if(_hp <= 0)
		{
			Death();
		}
	}
}
