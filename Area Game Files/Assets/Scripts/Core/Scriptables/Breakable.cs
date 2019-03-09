using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/Breakable")]
public class Breakable : ScriptableObject
{
  public int Health;          // Amount of health the object has
  public Animation Animation; // Animation for when it breaks
}
