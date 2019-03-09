using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/Weapon")]
public class Weapon : ScriptableObject
{
  public string Name;           // The name of the weapon
  public int ClipSize;          // Amount of bullets before reload
  public int ReloadSpeed;       // How long it takes to reload
  public int DamagePH;          // Damage Per Hit
  public int SprayDamagePT;     // Spray Damage in %
  public GameObject Model;      // Gun Model
}
