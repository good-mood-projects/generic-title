using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class GunReloadUI : MonoBehaviour
{
    bool reloading = false;

    public GunController _gunController;

    Image rotSlider;
    float fireTime;
    float currentTime;

    void Start()
    {
        rotSlider = GetComponent<Image>();
        _gunController.OnGunsChanged += GunsChanged;
        currentTime = 0.0f;
        _gunController.OnSecondFireShot += onReloading;
        //Invoke("setGunParameters", 1.0f);
    }

    void setGunParameters()
    {
        Gun[] guns = _gunController.getGuns();
        fireTime = guns[0].timeBetweenSecondFireTimeMS;
    }

    void Update()
    {
        if (reloading)
        {
            currentTime += Time.deltaTime;
            float angle = currentTime / fireTime * 1000;        
            if (angle >= 1.0f)
            {
                rotSlider.fillAmount = 0.0f;
                reloading = false;
                currentTime = 0.0f;
            }
            else
            {
                rotSlider.fillAmount = angle;
            }
        }
    }
    void GunsChanged()
    {
        setGunParameters();
    }
    void onReloading()
    {
        reloading = true;
    }
}
