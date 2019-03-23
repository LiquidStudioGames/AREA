using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/KBKeybinding")]
public class KeyboardLayout : ScriptableObject
{

    public KeyCode Jump = KeyCode.Space,          
                   Fire = KeyCode.Mouse0,
                   Forward = KeyCode.W,
                   Backwards = KeyCode.S,
                   StrafeLeft = KeyCode.A,
                   StrafeRight = KeyCode.D,
                   Spray = KeyCode.T,
                   Switch = KeyCode.Q;

}
