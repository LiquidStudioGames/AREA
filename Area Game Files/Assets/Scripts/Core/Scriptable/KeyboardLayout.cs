using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/KBKeybinding")]
public class KeyboardLayout : ScriptableObject
{
    public KeyCode Jump = KeyCode.Space;          
    public KeyCode Fire = KeyCode.Mouse0;

}
