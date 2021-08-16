﻿using UnityEngine;

namespace MapsExtended.MapObjects
{
    public class Ground : PhysicalMapObject { }

    [MapsExtendedMapObject(typeof(Ground))]
    public class GroundSpecification : PhysicalMapObjectSpecification<Ground>
    {
        public override GameObject Prefab => MapObjectManager.LoadCustomAsset<GameObject>("Ground");
    }
}
