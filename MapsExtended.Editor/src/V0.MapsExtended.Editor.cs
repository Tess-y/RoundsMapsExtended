﻿using System;
using System.Linq;
using System.Reflection;
using BepInEx;
using UnityEngine;
using MapsExt.MapObjects;

#pragma warning disable CS0618

namespace MapsExt.Editor
{
	public sealed partial class MapsExtendedEditor : BaseUnityPlugin
	{
		class VirtualMapObject : IMapObject
		{
			public GameObject Prefab { get; }

			public VirtualMapObject(GameObject prefab)
			{
				this.Prefab = prefab;
			}

			public void OnInstantiate(GameObject instance) { }
		}

		private void RegisterV0MapObjects(Assembly assembly)
		{
			Type[] types;
			try
			{
				types = assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException e)
			{
				types = e.Types.Where(t => t != null).ToArray();
			}

			foreach (var type in types.Where(t => Attribute.IsDefined(t, typeof(EditorMapObjectSpec))))
			{
				try
				{
					var attr = type.GetCustomAttribute<EditorMapObjectSpec>();
					var prefab =
						ReflectionUtils.GetAttributedProperty<GameObject>(type, typeof(EditorMapObjectPrefab)) ??
						ReflectionUtils.GetAttributedProperty<GameObject>(type, typeof(MapObjectPrefab));
					var serializerAction =
						ReflectionUtils.GetAttributedMethod<SerializerAction<MapObject>>(type, typeof(EditorMapObjectSerializer)) ??
						ReflectionUtils.GetAttributedMethod<SerializerAction<MapObject>>(type, typeof(MapObjectSerializer));
					var deserializerAction =
						ReflectionUtils.GetAttributedMethod<DeserializerAction<MapObject>>(type, typeof(EditorMapObjectDeserializer)) ??
						ReflectionUtils.GetAttributedMethod<DeserializerAction<MapObject>>(type, typeof(MapObjectDeserializer));

					var serializer = new MapObjectSpecSerializer(deserializerAction, serializerAction);
					this._mapObjectManager.RegisterMapObject(attr.dataType, new VirtualMapObject(prefab), serializer);
					this._mapObjectAttributes.Add((attr.dataType, attr.label, attr.category));
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogError($"Could not register legacy editor map object {type.Name}: {ex.Message}");

#if DEBUG
					UnityEngine.Debug.LogException(ex);
#endif
				}
			}
		}
	}
}

#pragma warning restore CS0618
