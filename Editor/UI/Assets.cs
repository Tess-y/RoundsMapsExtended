﻿using UnityEngine;
using Jotunn.Utils;

namespace MapsExt.UI
{
	public static class Assets
	{
		private static readonly AssetBundle bundle;

		public static GameObject OpenDialogPrefab
		{
			get
			{
				return Assets.bundle.LoadAsset<GameObject>("assets/prefabs/file browser.prefab");
			}
		}

		public static GameObject SaveDialogPrefab
		{
			get
			{
				return Assets.bundle.LoadAsset<GameObject>("assets/prefabs/save dialog.prefab");
			}
		}

		public static GameObject ToolbarPrefab
		{
			get
			{
				return Assets.bundle.LoadAsset<GameObject>("assets/prefabs/toolbar.prefab");
			}
		}

		public static GameObject WindowPrefab
		{
			get
			{
				return Assets.bundle.LoadAsset<GameObject>("assets/prefabs/window.prefab");
			}
		}

		public static GameObject AnimationWindowPrefab
		{
			get
			{
				return Assets.bundle.LoadAsset<GameObject>("assets/prefabs/animation window.prefab");
			}
		}

		public static GameObject KeyframeSettingsPrefab
		{
			get
			{
				return Assets.bundle.LoadAsset<GameObject>("assets/prefabs/keyframe settings.prefab");
			}
		}

		public static GameObject FoldoutPrefab
		{
			get
			{
				return Assets.bundle.LoadAsset<GameObject>("assets/prefabs/foldout.prefab");
			}
		}

		public static Sprite SquareSprite
		{
			get
			{
				return Assets.bundle.LoadAsset<Sprite>("assets/squaresprite.png");
			}
		}

		static Assets()
		{
			Assets.bundle = AssetUtils.LoadAssetBundleFromResources("uielements", typeof(FileDialog).Assembly);
		}
	}
}
