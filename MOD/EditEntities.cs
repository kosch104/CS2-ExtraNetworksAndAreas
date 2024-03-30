using Extra.Lib.Helper;
using Extra.Lib;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using Colossal.Entities;
using Game.Objects;

namespace ExtraNetworksAndAreas
{
	internal static class EditEntities
	{
		internal static void SetupEditEntities()
		{
			EntityQueryDesc surfaceEntityQueryDesc = new()
			{
				All = [ComponentType.ReadOnly<SurfaceData>()],
				None = [ComponentType.ReadOnly<PlaceholderObjectElement>()]
				
			};

			EntityQueryDesc decalsEntityQueryDesc = new()
			{
				All = [
					ComponentType.ReadOnly<StaticObjectData>(),
					ComponentType.ReadOnly<SpawnableObjectData>(),
				],
				None = [ComponentType.ReadOnly<PlaceholderObjectElement>()]
			};

			EntityQueryDesc netLaneEntityQueryDesc = new()
			{
				All = [
					ComponentType.ReadOnly<NetLaneData>(),
				],
				Any = [
					ComponentType.ReadOnly<LaneDeteriorationData>(),
					ComponentType.ReadOnly<SpawnableObjectData>(),
					ComponentType.ReadOnly<SecondaryLaneData>(),
				],
				None = [
					ComponentType.ReadOnly<PlaceholderObjectElement>(),
					//ComponentType.ReadOnly<TrackLaneData>(),
				]
			};

			EntityQueryDesc spacesEntityQueryDesc = new()
			{
				All = [
					ComponentType.ReadOnly<SpaceData>(),
				],
				None = [
					ComponentType.ReadOnly<PlaceholderObjectElement>(),
				]
			};

			EntityQueryDesc pathwaysEntityQueryDesc = new()
			{
				All = [
					ComponentType.ReadOnly<PathwayData>(),
				],
				None = [
					ComponentType.ReadOnly<PlaceholderObjectElement>(),
				]
			};

			ExtraLib.AddOnEditEnities(new(OnEditSpacesEntities, spacesEntityQueryDesc));
			ExtraLib.AddOnEditEnities(new(OnEditPathwayEntities, pathwaysEntityQueryDesc));
		}

		private static void OnEditPathwayEntities(NativeArray<Entity> entities)
		{
			foreach (Entity entity in entities)
			{
				if (ExtraLib.m_PrefabSystem.TryGetPrefab(entity, out PathwayPrefab prefab))
				{
					var prefabUI = prefab.GetComponent<UIObject>();
					if (prefabUI == null)
					{
						prefabUI = prefab.AddComponent<UIObject>();
						prefabUI.active = true;
						prefabUI.m_IsDebugObject = false;
						prefabUI.m_Icon = Icons.GetIcon(prefab);
						prefabUI.m_Priority = 1;
					}

					prefabUI.m_Group?.RemoveElement(entity);
					prefabUI.m_Group = PrefabsHelper.GetOrCreateUIAssetCategoryPrefab("Landscaping", "Pathways", Icons.GetIcon);
					prefabUI.m_Group.AddElement(entity);

					ExtraLib.m_EntityManager.AddOrSetComponentData(entity, prefabUI.ToComponentData());
				}
			}
		}

		private static void OnEditSpacesEntities(NativeArray<Entity> entities)
		{
			foreach (Entity entity in entities)
			{
				if (ExtraLib.m_PrefabSystem.TryGetPrefab(entity, out SpacePrefab prefab))
				{
					var prefabUI = prefab.GetComponent<UIObject>();
					if (prefabUI == null)
					{
						prefabUI = prefab.AddComponent<UIObject>();
						prefabUI.active = true;
						prefabUI.m_IsDebugObject = false;
						prefabUI.m_Icon = Icons.GetIcon(prefab);
						prefabUI.m_Priority = 1;
					}

					prefabUI.m_Group?.RemoveElement(entity);
					prefabUI.m_Group = PrefabsHelper.GetOrCreateUIAssetCategoryPrefab("Landscaping", "Spaces", Icons.GetIcon, "Pathways");
					prefabUI.m_Group.AddElement(entity);

					ExtraLib.m_EntityManager.AddOrSetComponentData(entity, prefabUI.ToComponentData());
				}
			}
		}

		private static void OnEditSurfacesEntities(NativeArray<Entity> entities)
		{
			foreach (Entity entity in entities)
			{
				if (ExtraLib.m_PrefabSystem.TryGetPrefab(entity, out SurfacePrefab prefab))
				{

					var prefabUI = prefab.GetComponent<UIObject>();
					if (prefabUI == null)
					{
						prefabUI = prefab.AddComponent<UIObject>();
						prefabUI.active = true;
						prefabUI.m_IsDebugObject = false;
						prefabUI.m_Icon = Icons.GetIcon(prefab);
						prefabUI.m_Priority = 1;
					}

					prefabUI.m_Group?.RemoveElement(entity);
					prefabUI.m_Group = PrefabsHelper.GetOrCreateUIAssetCategoryPrefab("Landscaping", "Surfaces", Icons.GetIcon, "Terraforming");
					prefabUI.m_Group.AddElement(entity);

					ExtraLib.m_EntityManager.AddOrSetComponentData(entity, prefabUI.ToComponentData());
				}
			}
		}

		private static void OnEditDecalsEntities(NativeArray<Entity> entities)
		{
			foreach (Entity entity in entities)
			{
				if (ExtraLib.m_PrefabSystem.TryGetPrefab(entity, out StaticObjectPrefab prefab))
				{

					if (!prefab.name.ToLower().Contains("decal") && !prefab.name.ToLower().Contains("roadarrow") && !prefab.name.ToLower().Contains("lanemarkings")) continue;

					if(ExtraLib.m_EntityManager.TryGetComponent(entity, out ObjectGeometryData objectGeometryData))
					{
                        objectGeometryData.m_Flags &= ~GeometryFlags.Overridable;
                        ExtraLib.m_EntityManager.SetComponentData(entity, objectGeometryData);
                    }

					var prefabUI = prefab.GetComponent<UIObject>();
					if (prefabUI == null)
					{
						prefabUI = prefab.AddComponent<UIObject>();
						prefabUI.active = true;
						prefabUI.m_IsDebugObject = false;
						prefabUI.m_Icon = Icons.GetIcon(prefab);
						prefabUI.m_Priority = 1;
					}

					prefabUI.m_Group?.RemoveElement(entity);
					prefabUI.m_Group = PrefabsHelper.GetOrCreateUIAssetCategoryPrefab("Landscaping", "Decals", Icons.GetIcon, "Pathways");
					prefabUI.m_Group.AddElement(entity);

					ExtraLib.m_EntityManager.AddOrSetComponentData(entity, prefabUI.ToComponentData());
				}
			}
		}

		private static void OnEditNetLaneEntities(NativeArray<Entity> entities)
		{
			foreach (Entity entity in entities)
			{
				if (ExtraLib.m_PrefabSystem.TryGetPrefab(entity, out NetLanePrefab prefab))
				{

					var prefabUI = prefab.GetComponent<UIObject>();
					if (prefabUI == null)
					{
						prefabUI = prefab.AddComponent<UIObject>();
						prefabUI.active = true;
						prefabUI.m_IsDebugObject = false;
						prefabUI.m_Icon = Icons.GetIcon(prefab);
						prefabUI.m_Priority = 1;
					}

					prefabUI.m_Group?.RemoveElement(entity);
					prefabUI.m_Group = PrefabsHelper.GetOrCreateUIAssetCategoryPrefab("Landscaping", "NetLane", Icons.GetIcon, "Pathways");
					prefabUI.m_Group.AddElement(entity);

					ExtraLib.m_EntityManager.AddOrSetComponentData(entity, prefabUI.ToComponentData());
				}
			}
		}

	}
}
