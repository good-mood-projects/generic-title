using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour {

//GunController instantiates guns, and handlers shooting
//Needs models for guns and Transform to indicate positions in order to hold gun.
    public Transform[] holder;
    public Gun[] initialGuns;
    int numGuns;
    public Gun[] equippedGuns;
    int nextGunToShoot;


    public event System.Action OnSecondFireShot;
    public event System.Action OnGunsChanged;

    // Use this for initialization
    void Start () {
        numGuns = initialGuns.Length;
        if (numGuns > 0)
        {
            equippedGuns = new Gun[numGuns];
            if (initialGuns != null)
            {
                changeGuns(initialGuns);
            }
            nextGunToShoot = initialGuns.Length - 1;
        }
    }

    public void changeGuns(Gun[] gunsToEquip)
    {
        /*foreach (Gun gun in equippedGuns){
            if (gun != null)
            {
                Destroy(gun.gameObject);
            }
        }*/
        for (int i = 0; i < gunsToEquip.Length; i++)
        {
            //equippedGuns[i] = Instantiate(gunsToEquip[i], holder[i].position, holder[i].rotation) as Gun;
            //Debug.Log("Gun instatiated");
            gunsToEquip[i].transform.position = holder[i].position;
            gunsToEquip[i].transform.rotation = holder[i].rotation;
            gunsToEquip[i].transform.parent = holder[i];
        }
        equippedGuns = gunsToEquip;
        OnGunsChanged();
    }

    public void PrimaryFire()
    {
        //Currently implemented: Alternate fire when guns>1
        nextGunToShoot--;
        if (nextGunToShoot < 0) {
            nextGunToShoot = equippedGuns.Length - 1;
        }
        if (equippedGuns[nextGunToShoot] != null)
        {
            equippedGuns[nextGunToShoot].PrimaryFire();
        }
    }

    public void SecondFire()
    {
        OnSecondFireShot();
        //Currently implemented: Alternate fire when guns>1
        nextGunToShoot--;
        if (nextGunToShoot < 0)
        {
            nextGunToShoot = equippedGuns.Length - 1;
        }
        if (equippedGuns[nextGunToShoot] != null)
        {
            equippedGuns[nextGunToShoot].SecondFire();
        }
    }

    public Gun[] getGuns()
    {
        return equippedGuns;
    }
    public int getGunNum()
    {
        return numGuns;
    }
}
