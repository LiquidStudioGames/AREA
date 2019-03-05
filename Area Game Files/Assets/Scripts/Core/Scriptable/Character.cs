using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/Character")]
public class Character : ScriptableObject
{
  public string Name;         // Name of character
  public int Health;          // Amount of health character has
  public int Speed;           // Walk/Run speed
  public int GrenadeReload;   // Time before the character gets his/hers grenade back
  public Sprite Icon;         // Icon for the character
}
