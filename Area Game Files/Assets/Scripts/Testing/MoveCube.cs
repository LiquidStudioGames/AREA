using UnityEngine;

[RequireComponent(typeof(NetworkTag))]
public class MoveCube : MonoBehaviour
{
    public float speed;
    public NetworkTarget target;

    private NetworkTag networkTag;

    private void Start()
    {
        networkTag = GetComponent<NetworkTag>();
    }

    private void FixedUpdate()
    {
        float x = 0f;
        if (Input.GetKey(KeyCode.A)) x -= speed;
        if (Input.GetKey(KeyCode.D)) x += speed;
        networkTag.Call(Move, target, new BitStream().Write(x));
    }

    [NetworkCall]
    private void Move(BitStream stream, SteamPlayer sender)
    {
        float x = stream.ReadFloat();
        transform.position += new Vector3(x, 0f);
    }
}
