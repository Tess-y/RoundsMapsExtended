﻿using HarmonyLib;
using MapsExt.MapObjects;
using MapsExt.MapObjects.Properties;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnboundLib;
using UnityEngine;

namespace MapsExt
{
	public class PropertyCompositeSerializer : IMapObjectSerializer
	{
		private readonly PropertyManager propertyManager;
		private readonly Dictionary<Type, List<(MemberInfo, IPropertySerializer)>> memberSerializerCache
			= new Dictionary<Type, List<(MemberInfo, IPropertySerializer)>>();

		public PropertyCompositeSerializer(PropertyManager propertyManager)
		{
			this.propertyManager = propertyManager;
		}

		public void Deserialize(MapObjectData data, GameObject target)
		{
			try
			{
				var mapObjectInstance = target.GetOrAddComponent<MapObjectInstance>();
				mapObjectInstance.mapObjectId = data.mapObjectId ?? Guid.NewGuid().ToString();
				mapObjectInstance.dataType = data.GetType();
				target.SetActive(data.active);

				this.CacheMemberSerializers(mapObjectInstance.dataType);

				foreach (var (memberInfo, serializer) in this.memberSerializerCache[mapObjectInstance.dataType])
				{
					var prop = (IProperty) memberInfo.GetFieldOrPropertyValue(data);
					serializer.Deserialize(prop, mapObjectInstance.gameObject);
				}
			}
			catch (Exception ex)
			{
				throw new MapObjectSerializationException($"Could not deserialize {data.GetType()} into {target.name}", ex);
			}
		}

		public MapObjectData Serialize(MapObjectInstance mapObjectInstance)
		{
			try
			{
				var data = (MapObjectData) AccessTools.CreateInstance(mapObjectInstance.dataType);

				data.mapObjectId = mapObjectInstance.mapObjectId;
				data.active = mapObjectInstance.gameObject.activeSelf;

				this.CacheMemberSerializers(mapObjectInstance.dataType);

				foreach (var (memberInfo, serializer) in this.memberSerializerCache[mapObjectInstance.dataType])
				{
					var prop = (IProperty) memberInfo.GetFieldOrPropertyValue(data);
					serializer.Serialize(mapObjectInstance.gameObject, prop);
				}

				return data;
			}
			catch (Exception ex)
			{
				throw new MapObjectSerializationException($"Could not serialize map object: {mapObjectInstance.gameObject.name}", ex);
			}
		}

		private void CacheMemberSerializers(Type type)
		{
			if (this.memberSerializerCache.ContainsKey(type))
			{
				return;
			}

			var serializableMembers = this.propertyManager.GetSerializableMembers(type);
			var serializers = serializableMembers.Select(p => (p, this.propertyManager.GetSerializer(p.GetReturnType())));
			this.memberSerializerCache[type] = serializers.ToList();
		}
	}
}
