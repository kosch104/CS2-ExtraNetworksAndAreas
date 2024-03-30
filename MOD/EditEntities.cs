using Extra.Lib.Helper;
using Extra.Lib;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using Colossal.Entities;
using Game.Net;
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

			EntityQueryDesc markerPrefabsEntityQueryDesc = new()
			{
				All = [
					ComponentType.ReadOnly<ObjectGeometryData>(),
				],
				None = [
					ComponentType.ReadOnly<PlaceholderObjectElement>(),
				]
			};

			EntityQueryDesc tracksEntityQueryDesc = new()
			{
				All = [
					ComponentType.ReadOnly<TrackData>(),
				],
				None = [
					ComponentType.ReadOnly<PlaceholderObjectElement>(),
				]
			};

			ExtraLib.AddOnEditEnities(new(OnEditSpacesEntities, spacesEntityQueryDesc));
			ExtraLib.AddOnEditEnities(new(OnEditPathwayEntities, pathwaysEntityQueryDesc));
			ExtraLib.AddOnEditEnities(new(OnEditMarkerPrefabEntities, markerPrefabsEntityQueryDesc));
			ExtraLib.AddOnEditEnities(new(OnEditTrackEntities, tracksEntityQueryDesc));
			// TransportStopData
		}

		private static void OnEditTrackEntities(NativeArray<Entity> entities)
		{
			foreach (Entity entity in entities)
			{
				if (ExtraLib.m_PrefabSystem.TryGetPrefab(entity, out TrackPrefab prefab))
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
					if (prefab.m_TrackType == TrackTypes.Train) prefabUI.m_Group = PrefabsHelper.GetUIAssetCategoryPrefab("TransportationTrain");
					if (prefab.m_TrackType == TrackTypes.Subway) prefabUI.m_Group = PrefabsHelper.GetUIAssetCategoryPrefab("TransportationSubway");
					if (prefab.m_TrackType == TrackTypes.Tram) prefabUI.m_Group = PrefabsHelper.GetUIAssetCategoryPrefab("TransportationTram");
					prefabUI.m_Group.AddElement(entity);

					ExtraLib.m_EntityManager.AddOrSetComponentData(entity, prefabUI.ToComponentData());
				}
			}
		}

		private static void OnEditMarkerPrefabEntities(NativeArray<Entity> entities)
		{
			ENA.Logger.Info("Editing MarkerObjectPrefabEntities");
			foreach (Entity entity in entities)
			{
				ENA.Logger.Info("Editing MarkerObjectPrefab entity: " + entity);
				if (ExtraLib.m_PrefabSystem.TryGetPrefab(entity, out MarkerObjectPrefab prefab))
				{
					ENA.Logger.Info("Got Prefab: " + prefab.name + " for entity: " + entity);
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
					prefabUI.m_Group = PrefabsHelper.GetOrCreateUIAssetCategoryPrefab("Landscaping", "MarkerObjects", Icons.GetIcon, "Spaces");
					prefabUI.m_Group.AddElement(entity);

					ExtraLib.m_EntityManager.AddOrSetComponentData(entity, prefabUI.ToComponentData());
				}
			}
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

	}
}
