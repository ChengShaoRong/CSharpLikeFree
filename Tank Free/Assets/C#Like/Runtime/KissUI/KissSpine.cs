///*
// *           C#Like
// * Copyright © 2022-2024 RongRong. All right reserved.
// */
//using Spine;
//using Spine.Unity;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.U2D;
//using UnityEngine.UI;
//using Spine.Unity.AttachmentTools;

//namespace CSharpLike
//{
//	[AddComponentMenu("KissUI/KissSpine")]
//	public class KissSpine : MonoBehaviour
//	{
//		[System.Serializable]
//		public class AttachmentInfo
//		{
//			public string slot;
//			public SpriteAtlas spriteAtlas;
//			public string spriteName;
//			public Attachment GetAttachment2(float scale)
//			{
//				Atlas atlas = GetAtlas(spriteAtlas);
//				AtlasRegion atlasRegion = atlas.FindRegion(spriteName);
//				if (atlasRegion == null)
//					return null;
//				return atlasRegion.ToRegionAttachment(spriteName, scale);
//			}
//			public Attachment GetAttachment(float scale)
//            {
//				AtlasRegion atlasRegion = GetAtlasRegion();
//				if (atlasRegion == null)
//					return null;
//				return atlasRegion.ToRegionAttachment(spriteName, scale);
//            }
//			AtlasRegion GetAtlasRegion()
//            {
//				Sprite sprite = spriteAtlas.GetSprite(spriteName);
//				if (sprite == null)
//					return null;

//				AtlasPage page = new AtlasPage();
//				page.name = spriteAtlas.name;
//				page.width = sprite.texture.width;
//				page.height = sprite.texture.height;
//				page.format = Spine.Format.RGBA8888;

//				page.minFilter = TextureFilter.Linear;
//				page.magFilter = TextureFilter.Linear;
//				page.uWrap = TextureWrap.ClampToEdge;
//				page.vWrap = TextureWrap.ClampToEdge;
//				Material material = new Material(Shader.Find("Spine/SkeletonGraphic"));
//				material.mainTexture = sprite.texture;
//				page.rendererObject = material;

//				AtlasRegion region = new AtlasRegion();
//				region.name = sprite.name.Replace("(Clone)", "");
//				region.page = page;
//				region.degrees = sprite.packingRotation == SpritePackingRotation.None ? 0 : 90;
//				region.rotate = region.degrees != 0;

//				region.u2 = 1;
//				region.v2 = 1;
//				region.width = page.width;
//				region.height = page.height;
//				region.originalWidth = page.width;
//				region.originalHeight = page.height;

//				region.index = 0;
//				return region;
//			}

//			static Dictionary<SpriteAtlas, Atlas> mAtlas = new Dictionary<SpriteAtlas, Atlas>();
//			static Atlas GetAtlas(SpriteAtlas sa)
//            {
//				if (!mAtlas.TryGetValue(sa, out Atlas atlas))
//				{
//					atlas = LoadAtlas(sa);
//					mAtlas[sa] = atlas;
//				}
//				return atlas;
//			}
//			protected class SavedRegionInfo
//			{
//				public float x, y, width, height;
//				public SpritePackingRotation packingRotation;
//			}
//			static Atlas LoadAtlas(SpriteAtlas spriteAtlas)
//			{

//				List<AtlasPage> pages = new List<AtlasPage>();
//				List<AtlasRegion> regions = new List<AtlasRegion>();

//				Sprite[] sprites = new UnityEngine.Sprite[spriteAtlas.spriteCount];
//				spriteAtlas.GetSprites(sprites);
//				if (sprites.Length == 0)
//					return new Atlas(pages, regions);

//				Texture2D texture = sprites[0].texture;

//				Material material = new Material(Shader.Find("Spine/SkeletonGraphic"));
//				material.mainTexture = texture;

//				Spine.AtlasPage page = new AtlasPage();
//				page.name = spriteAtlas.name;
//				page.width = texture.width;
//				page.height = texture.height;
//				page.format = Spine.Format.RGBA8888;

//				page.minFilter = TextureFilter.Linear;
//				page.magFilter = TextureFilter.Linear;
//				page.uWrap = TextureWrap.ClampToEdge;
//				page.vWrap = TextureWrap.ClampToEdge;
//				page.rendererObject = material;
//				pages.Add(page);

//				sprites = SpineSpriteAtlasAsset.AccessPackedSprites(spriteAtlas);

//				SavedRegionInfo[] savedRegions = new SavedRegionInfo[sprites.Length];
//				int i = 0;
//				for (; i < sprites.Length; ++i)
//				{
//					var sprite = sprites[i];
//					AtlasRegion region = new AtlasRegion();
//					region.name = sprite.name.Replace("(Clone)", "");
//					region.page = page;
//					region.degrees = sprite.packingRotation == SpritePackingRotation.None ? 0 : 90;
//					region.rotate = region.degrees != 0;

//					region.u2 = 1;
//					region.v2 = 1;
//					region.width = page.width;
//					region.height = page.height;
//					region.originalWidth = page.width;
//					region.originalHeight = page.height;

//					region.index = i;
//					regions.Add(region);


//					var rect = sprite.rect;
//					float x = rect.min.x;
//					float y = rect.min.y;
//					float width = rect.width;
//					float height = rect.height;

//					var savedRegion = new SavedRegionInfo();
//					savedRegion.x = x;
//					savedRegion.y = y;
//					savedRegion.width = width;
//					savedRegion.height = height;
//					savedRegion.packingRotation = sprite.packingRotation;
//					savedRegions[i] = savedRegion;
//				}

//				var atlas = new Atlas(pages, regions);
//				AssignRegionsFromSavedRegions(sprites, atlas, savedRegions);

//				return atlas;
//			}
//			protected static void AssignRegionsFromSavedRegions(Sprite[] sprites, Atlas usedAtlas, SavedRegionInfo[] savedRegions)
//			{
//				int i = 0;
//				foreach (var region in usedAtlas)
//				{
//					var savedRegion = savedRegions[i];
//					var page = region.page;

//					region.degrees = savedRegion.packingRotation == SpritePackingRotation.None ? 0 : 90;
//					region.rotate = region.degrees != 0;

//					float x = savedRegion.x;
//					float y = savedRegion.y;
//					float width = savedRegion.width;
//					float height = savedRegion.height;

//					region.u = x / (float)page.width;
//					region.v = y / (float)page.height;
//					if (region.rotate)
//					{
//						region.u2 = (x + height) / (float)page.width;
//						region.v2 = (y + width) / (float)page.height;
//					}
//					else
//					{
//						region.u2 = (x + width) / (float)page.width;
//						region.v2 = (y + height) / (float)page.height;
//					}
//					region.x = (int)x;
//					region.y = (int)y;
//					region.width = Mathf.Abs((int)width);
//					region.height = Mathf.Abs((int)height);

//					// flip upside down
//					var temp = region.v;
//					region.v = region.v2;
//					region.v2 = temp;

//					region.originalWidth = (int)width;
//					region.originalHeight = (int)height;

//					// note: currently sprite pivot offsets are ignored.
//					// var sprite = sprites[i];
//					region.offsetX = 0;//sprite.pivot.x;
//					region.offsetY = 0;//sprite.pivot.y;

//					++i;
//				}
//			}

//			//private AtlasRegion CreateRegion(Texture2D texture)
//			//{
//			//	Spine.AtlasRegion region = new AtlasRegion();
//			//	region.width = texture.width;
//			//	region.height = texture.height;
//			//	region.originalWidth = texture.width;
//			//	region.originalHeight = texture.height;
//			//	region.rotate = false;
//			//	region.page = new AtlasPage();
//			//	region.page.name = texture.name;
//			//	region.page.width = texture.width;
//			//	region.page.height = texture.height;
//			//	region.page.uWrap = TextureWrap.ClampToEdge;
//			//	region.page.vWrap = TextureWrap.ClampToEdge;
//			//	return region;
//			//}
//			//Material CreateRegionAttachmentByTexture(Slot slot, Texture2D texture)
//			//{
//			//	if (slot == null) { return null; }
//			//	if (texture == null) { return null; }

//			//	RegionAttachment attachment = slot.Attachment as RegionAttachment;
//			//	if (attachment == null) { return null; }

//			//	attachment.RendererObject = CreateRegion(texture);
//			//	attachment.SetUVs(0f, 1f, 1f, 0f, false);

//			//	Material mat = new Material(Shader.Find("Sprites/Default"));
//			//	mat.mainTexture = texture;
//			//	(attachment.RendererObject as AtlasRegion).page.rendererObject = mat;

//			//	slot.Attachment = attachment;
//			//	return mat;
//			//}
//			//Material CreateMeshAttachmentByTexture(Spine.Slot slot, Texture2D texture)
//			//{
//			//	if (slot == null) return null;
//			//	MeshAttachment oldAtt = slot.Attachment as MeshAttachment;
//			//	if (oldAtt == null || texture == null) return null;

//			//	MeshAttachment att = new MeshAttachment(oldAtt.Name);
//			//	att.RendererObject = CreateRegion(texture);
//			//	att.Path = oldAtt.Path;

//			//	att.Bones = oldAtt.Bones;
//			//	att.Edges = oldAtt.Edges;
//			//	att.Triangles = oldAtt.Triangles;
//			//	att.Vertices = oldAtt.Vertices;
//			//	att.WorldVerticesLength = oldAtt.WorldVerticesLength;
//			//	att.HullLength = oldAtt.HullLength;
//			//	att.RegionRotate = false;

//			//	att.RegionU = 0f;
//			//	att.RegionV = 1f;
//			//	att.RegionU2 = 1f;
//			//	att.RegionV2 = 0f;
//			//	att.RegionUVs = oldAtt.RegionUVs;

//			//	att.UpdateUVs();

//			//	Material mat = new Material(Shader.Find("Sprites/Default"));
//			//	mat.mainTexture = texture;
//			//	(att.RendererObject as Spine.AtlasRegion).page.rendererObject = mat;
//			//	slot.Attachment = att;
//			//	return mat;
//			//}
//		}
//		public List<AttachmentInfo> attachments = new List<AttachmentInfo>();
//		private void Awake()
//		{
//			return;
//			var skeletonGraphic = GetComponent<SkeletonGraphic>();
//			skeletonGraphic.OnRebuild += Apply;
//			if (skeletonGraphic.IsValid) Apply(skeletonGraphic);
//		}

//		void Apply(SkeletonGraphic skeletonGraphic)
//        {
//			if (!enabled || attachments.Count == 0)
//				return;
//			float scale = skeletonGraphic.skeletonDataAsset.scale;
//			foreach(var attachment in attachments)
//			{
//				Slot slot = skeletonGraphic.Skeleton.FindSlot(attachment.slot);
//				if (slot == null)
//					continue;
//				if (attachment.spriteAtlas == null)
//					continue;
//				slot.Attachment = attachment.GetAttachment2(scale);
//			}
//		}
//	}
//}

