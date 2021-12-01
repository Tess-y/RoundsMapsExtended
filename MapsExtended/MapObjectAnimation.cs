using MapsExt.MapObjects;
using System.Collections.Generic;
using UnityEngine;
using UnboundLib;
using System.Collections;

namespace MapsExt
{
	public class MapObjectAnimation : MonoBehaviour
	{
		private class RigidBodyParams
		{
			public float gravityScale;
			public bool isKinematic;
			public bool useFullKinematicContacts;

			public RigidBodyParams(Rigidbody2D rb)
			{
				this.gravityScale = rb.gravityScale;
				this.isKinematic = rb.isKinematic;
				this.useFullKinematicContacts = rb.useFullKinematicContacts;
			}
		}

		public List<AnimationKeyframe> keyframes = new List<AnimationKeyframe>();
		public bool playOnAwake = true;
		public bool IsPlaying { get; private set; }

		private RigidBodyParams originalRigidBody;

		public void OnEnable()
		{
			this.ExecuteAfterFrames(1, () =>
			{
				var photonMapObject = this.gameObject.GetComponent<PhotonMapObject>();
				if (photonMapObject && (bool) photonMapObject.GetFieldValue("photonSpawned") == false)
				{
					return;
				}

				var rb = this.gameObject.GetComponent<Rigidbody2D>();

				if (rb)
				{
					this.originalRigidBody = new RigidBodyParams(rb);
				}
				else
				{
					rb = this.gameObject.AddComponent<Rigidbody2D>();
				}

				rb.gravityScale = 0;
				rb.constraints = RigidbodyConstraints2D.FreezeAll;
				rb.isKinematic = true;
				rb.useFullKinematicContacts = true;

				if (this.playOnAwake)
				{
					this.ExecuteAfterFrames(1, () =>
					{
						this.Play();
					});
				}
			});
		}

		public void OnDisable()
		{
			this.Stop();

			var rb = this.gameObject.GetComponent<Rigidbody2D>();

			if (this.originalRigidBody == null)
			{
				GameObject.Destroy(rb);
			}
			else
			{
				rb.gravityScale = this.originalRigidBody.gravityScale;
				rb.constraints = RigidbodyConstraints2D.None;
				rb.isKinematic = this.originalRigidBody.isKinematic;
				rb.useFullKinematicContacts = this.originalRigidBody.useFullKinematicContacts;
			}
		}

		public void Initialize(SpatialMapObject mapObject)
		{
			this.keyframes.Clear();
			this.keyframes.Add(new AnimationKeyframe(mapObject));
		}

		public void AddKeyframe()
		{
			this.keyframes.Add(new AnimationKeyframe(this.keyframes[this.keyframes.Count - 1]));
		}

		public void DeleteKeyframe(int index)
		{
			this.keyframes.RemoveAt(index);
		}

		public void Play()
		{
			this.IsPlaying = true;
			this.StartCoroutine(this.PlayCoroutine());
		}

		public void Stop()
		{
			this.StopAllCoroutines();
			this.ApplyKeyframe(0);
			this.IsPlaying = false;
		}

		private IEnumerator PlayCoroutine()
		{
			this.ApplyKeyframe(0);

			for (int i = 1; i < this.keyframes.Count; i++)
			{
				yield return this.AnimateKeyframe(i);
			}

			if (this.keyframes.Count > 1)
			{
				this.Play();
			}
		}

		private IEnumerator AnimateKeyframe(int frameIndex)
		{
			var frame = this.keyframes[frameIndex];

			float frameLength = frame.curve.keys[frame.curve.keys.Length - 1].time;
			float elapsedTime = 0;

			while (elapsedTime < frameLength)
			{
				this.ApplyKeyframe(frameIndex, elapsedTime);
				elapsedTime += TimeHandler.deltaTime * frame.animationSpeed;
				yield return null;
			}

			this.ApplyKeyframe(frameIndex, frameLength);
		}

		private void ApplyKeyframe(int frameIndex, float time = 0)
		{
			if (frameIndex > this.keyframes.Count - 1)
			{
				return;
			}

			var startFrame = frameIndex > 0 ? this.keyframes[frameIndex - 1] : this.keyframes[0];
			var endFrame = this.keyframes[frameIndex];

			float curveValue = endFrame.curve.Evaluate(time);
			this.transform.position = Vector3.Lerp(startFrame.position, endFrame.position, curveValue);
			this.transform.localScale = Vector3.Lerp(startFrame.scale, endFrame.scale, curveValue);
			this.transform.rotation = Quaternion.Lerp(startFrame.rotation, endFrame.rotation, curveValue);
		}
	}
}