using MapsExt.MapObjects;
using UnityEngine;
using System;

namespace MapsExt
{
	public class AnimationKeyframe
	{
		public Vector3 position;
		public Vector3 scale;
		public Quaternion rotation;
		public float animationSpeed;
		public CurveType curveType;

		[NonSerialized]
		public AnimationCurve curve;

		public AnimationKeyframe()
		{
			this.position = Vector3.zero;
			this.scale = Vector3.one;
			this.rotation = Quaternion.identity;
			this.animationSpeed = 1;
			this.curveType = CurveType.Linear;

			this.UpdateCurve();
		}

		public AnimationKeyframe(SpatialMapObject mapObject)
		{
			this.position = mapObject.position;
			this.scale = mapObject.scale;
			this.rotation = mapObject.rotation;
			this.animationSpeed = 1;
			this.curveType = CurveType.Linear;

			this.UpdateCurve();
		}

		public AnimationKeyframe(AnimationKeyframe frame)
		{
			this.position = frame.position;
			this.scale = frame.scale;
			this.rotation = frame.rotation;
			this.animationSpeed = frame.animationSpeed;
			this.curveType = frame.curveType;

			this.UpdateCurve();
		}

		public void UpdateCurve()
		{
			if (this.curveType == CurveType.Linear)
			{
				this.curve = AnimationCurve.Linear(0, 0, 1, 1);
			}

			if (this.curveType == CurveType.EaseIn)
			{
				this.curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 0.2f), new Keyframe(1, 1));
			}

			if (this.curveType == CurveType.EaseOut)
			{
				this.curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 0.8f), new Keyframe(1, 1));
			}

			if (this.curveType == CurveType.EaseInOut)
			{
				this.curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
			}
		}

		public enum CurveType
		{
			Linear,
			EaseIn,
			EaseOut,
			EaseInOut
		}
	}
}