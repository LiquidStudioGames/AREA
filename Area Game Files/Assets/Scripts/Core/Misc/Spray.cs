using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;

public class Spray : MonoBehaviour
{

    public GameObject SprayPrefab;

    public float SprayRange = 50f;

    public void DoSpray (Material SprayMaterial)
    {

        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit rayhit;

        if (Physics.Raycast (ray, out rayhit, SprayRange))
        {

            SprayPrefab.GetComponent<DecalProjectorComponent> ().m_Material = SprayMaterial;

            Instantiate (SprayPrefab, rayhit.point, Quaternion.FromToRotation (Vector3.up, rayhit.normal));

        }

    }

}
