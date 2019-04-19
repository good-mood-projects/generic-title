using UnityEngine;
using System.Collections;

public class HealthPickup : Pickup {

    public int healAmount = 50;

    public int getHeal()
    {
        return healAmount;
    }

}
