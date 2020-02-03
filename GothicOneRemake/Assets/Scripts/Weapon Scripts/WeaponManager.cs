using UnityEngine;

public class WeaponManager : MonoBehaviour {

    [SerializeField]
    private WeaponHandler[] weapons;

    private int current_Weapon_Index;

    private void Start() {
        current_Weapon_Index = 0;
        weapons[current_Weapon_Index].gameObject.SetActive(true);
    }

    private void Update() {
        switch (Input.inputString) {
            case "1":
                TurnOnSelectedWeapon(0);
                break;
            case "2":
                TurnOnSelectedWeapon(1);
                break;
            case "3":
                TurnOnSelectedWeapon(2);
                break;
            case "4":
                TurnOnSelectedWeapon(3);
                break;
            case "5":
                TurnOnSelectedWeapon(4);
                break;
            case "6":
                TurnOnSelectedWeapon(5);
                break;
        }
    }

    void TurnOnSelectedWeapon(int weaponIndex) {
        if (weaponIndex != current_Weapon_Index) {
            weapons[current_Weapon_Index].gameObject.SetActive(false);
            current_Weapon_Index = weaponIndex;
            weapons[current_Weapon_Index].gameObject.SetActive(true);
        }
    }

    public WeaponHandler GetCurrentSelectedWeapon() {
        return weapons[current_Weapon_Index];
    }
} // class