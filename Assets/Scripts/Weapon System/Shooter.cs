using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{

    [SerializeField] private WeaponManager weaponManager;    

    [SerializeField] private RampingController rampingController;

    private float nextShootTime = 0.0f;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        ShootInput();
        SwitchWeaponInput();
    }

    private void ShootInput()
    {
        if (Input.GetKey(KeyCode.Mouse0) && Time.time > nextShootTime)
        {
            Weapon weapon = weaponManager.equippedWeapon;
            float shotShake = (1f / weapon.playerShootRate) * Mathf.Pow(1.05f, rampingController.CurrentRampingTier);

            if(ScreenShaker.Instance.GetCurrentTrauma() < 0.2f)
            {
                ScreenShaker.Instance.Shake(shotShake);  
            }

            rb.AddForce(-transform.up * weapon.recoil);
            weapon.Shoot(rampingController.CurrentRampingTier, gameObject.transform);
            nextShootTime = Time.time + (1f / weapon.playerShootRate);
        }
    }

    private void SwitchWeaponInput()
    {
        int numberOfWeapons = weaponManager.GetWeaponCount();

        for (int i = 0; i < numberOfWeapons; i++)
        {
            if (Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), "Alpha" + (i + 1))))
            {
                weaponManager.EquipWeapon(i + 1);
            }
        }
    }


}
