using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WeaponType
    {
        Rocket,
        SMG,
        Knife,
        Hammer,
        Minigun,
        Shotgun
    }

    public WeaponType weaponType; // 무기 타입 선택

    public string weaponName; // 실제 무기 이름 (UI 표시용)

    private void Start()
    {
        // 무기 이름을 무기 타입에 따라 자동으로 지정
        switch (weaponType)
        {
            case WeaponType.Rocket:
                weaponName = "Rocket Launcher";
                break;
            case WeaponType.SMG:
                weaponName = "SMG";
                break;
            case WeaponType.Knife:
                weaponName = "Knife";
                break;
            case WeaponType.Hammer:
                weaponName = "Hammer";
                break;
            case WeaponType.Minigun:
                weaponName = "Minigun";
                break;
            case WeaponType.Shotgun:
                weaponName = "Shotgun";
                break;            
            default:
                weaponName = "Unknown";
                break;
        }
    }
}
