using System;
using System.Collections;
using System.Collections.Specialized;
using MapsExt.Editor.ActionHandlers;
using MapsExt.MapObjects;
using UnityEngine;

namespace MapsExt.Editor.Tests
{
	public class EditorTestUtils
	{
		private readonly MapEditor editor;
		private readonly SimulatedInputSource inputSource;

		public EditorTestUtils(MapEditor editor) : this(editor, null) { }

		public EditorTestUtils(MapEditor editor, SimulatedInputSource inputSource)
		{
			this.editor = editor;
			this.inputSource = inputSource;
		}

		public IEnumerator SpawnMapObject<T>() where T : MapObjectData
		{
			bool spawned = false;

			void OnChange(object sender, NotifyCollectionChangedEventArgs e)
			{
				if (this.editor.SelectedObjects.Count > 0)
				{
					this.editor.SelectedObjects.CollectionChanged -= OnChange;
					spawned = true;
				}
			}

			this.editor.SelectedObjects.CollectionChanged += OnChange;
			this.editor.CreateMapObject(typeof(T));

			while (!spawned)
			{
				yield return null;
			}
		}

		public IEnumerator MoveSelectedWithMouse(Vector2 delta)
		{
			var go = this.editor.ActiveObject;
			yield return this.DragMouse(go.transform.position, delta);
		}

		public IEnumerator ResizeSelectedWithMouse(Vector2 delta, int anchorPosition)
		{
			if (anchorPosition == 0)
			{
				throw new ArgumentException("anchorPosition cannot be 0");
			}

			IEnumerator DoResizeSelectedWithMouse()
			{
				var resizeInteractionContent = this.editor.ActiveObject.GetComponent<SizeHandler>().Content;
				var resizeHandle = resizeInteractionContent.transform.Find("Resize Handle " + anchorPosition).gameObject;
				yield return this.DragMouse(resizeHandle.transform.position, delta);
			}

			return DoResizeSelectedWithMouse();
		}

		public IEnumerator RotateSelectedWithMouse(float degrees)
		{
			var go = this.editor.ActiveObject;
			var resizeInteractionContent = go.GetComponent<MapsExt.Editor.ActionHandlers.RotationHandler>().Content;
			var handle = resizeInteractionContent.transform.Find("Rotation Handle").gameObject;

			var from = handle.transform.position;
			var rotated = Quaternion.Euler(0, 0, degrees) * from;

			yield return this.DragMouse(from, rotated - from);
		}

		public IEnumerator DragMouse(Vector3 worldPosition, Vector3 delta)
		{
			this.inputSource.SetMousePosition(MainCam.instance.cam.WorldToScreenPoint(worldPosition));
			this.inputSource.SetMouseButtonDown(0);
			yield return null;
			this.inputSource.SetMousePosition(MainCam.instance.cam.WorldToScreenPoint(worldPosition + delta));
			yield return null;
			this.inputSource.SetMouseButtonUp(0);
			yield return null;
		}
	}
}
