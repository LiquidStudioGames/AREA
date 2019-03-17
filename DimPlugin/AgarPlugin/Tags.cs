using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dim
{
    static class DimTag
    {
        public static readonly ushort SpawnPlayerTag = 0;
        public static readonly ushort MovePlayerTag = 1;
        public static readonly ushort DespawnPlayerTag = 2;
        public static readonly ushort TimeTag = 3;

        public static readonly ushort DeliveryDamageInfo = 4;
        public static readonly ushort DeliveryDamageBreakStart = 5;
        public static readonly ushort DeliveryDamageMatchStart = 6;
        public static readonly ushort DeliveryDamageMatchEndStart = 7;


    }

    [Serializable]
    public class ColliderItem
    {
        public float posX, posY, posZ;

        public ColliderItem(float x, float y, float z)
        {
            posX = x;
            posY = y;
            posZ = z;
        }
    }

    [Serializable]
    public class SphereColliderItem : ColliderItem
    {
        public float radius;

        public SphereColliderItem(float x, float y, float z, float r) : base(x, y, z)
        {
            radius = r;
        }
    }

    [Serializable]
    public class CubeColliterItem : ColliderItem
    {
        public float width, height, longg;

        public CubeColliterItem(float x, float y, float z, float w, float h, float t) : base(x, y, z)
        {
            width = w;
            height = h;
            longg = t;
        }
    }

    [Serializable]
    public class SaveJSONColliders
    {
        public List<SphereColliderItem> circleColliderList = new List<SphereColliderItem>();
        public List<CubeColliterItem> boxColliderList = new List<CubeColliterItem>();

    }

}
