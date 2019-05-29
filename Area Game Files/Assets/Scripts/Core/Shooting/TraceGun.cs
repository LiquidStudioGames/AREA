using UnityEngine;

public class TraceGun : AbsGun
{
    public Transform barrel;

    private RaycastHit hit;

    public override void Fire()
    {
        Debug.DrawRay (cam.position, cam.forward, Color.red, 1f);

        // Raycast through the camera
        if (Physics.Raycast (cam.position, cam.forward, out hit, float.PositiveInfinity, mask))
        {
            Vector3 direction = hit.point - barrel.position;
            Debug.DrawRay (barrel.position, direction, Color.red, 1f);

            // Raycast through the gunbarrel to the point hit by the camera
            if (Physics.Raycast (barrel.position, direction, out hit, float.PositiveInfinity, mask))
            {
                Debug.Log ("Hit: " + hit.transform.name);
                networkTag.Call (Hit, NetworkTarget.Others, new BitStream().Write(hit.transform.name), SendType.Reliable);
            }
        }
    }

    [NetworkCall]
    public void Hit (BitStream stream, SteamPlayer sender)
    {
        string name = stream.ReadString();

        if (sender == networkTag.Owner)
            Debug.Log("Hit: " + name);
    }
}
