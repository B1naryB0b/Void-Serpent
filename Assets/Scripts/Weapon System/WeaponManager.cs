using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{

    [SerializeField] private List<Weapon> weapons;

    [HideInInspector] public Weapon equippedWeapon;

    private void Start()
    {
        EquipWeapon(1);
    }

    public void EquipWeapon(int weaponIndex = 1)
    {
        if (weaponIndex > weapons.Count) { Debug.LogError("Weapon Index Out of Range (Check weapon list)"); }
    
        equippedWeapon = weapons[weaponIndex-1];
    }

    public int GetWeaponCount()
    {
        return weapons.Count;
    }
    

}
