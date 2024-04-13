using System.Collections.Generic;
using Extra.Lib;
using Extra.Lib.Helper;
using Game.Net;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;

namespace ExtraNetworksAndAreas.Mod
{
	internal static class EditEntities
	{
		internal static void SetupEditEntities()
		{

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

			EntityQueryDesc tracksEntityQueryDesc = new()
			{
				All = [
					ComponentType.ReadOnly<TrackData>(),
				],
				None = [
					ComponentType.ReadOnly<PlaceholderObjectElement>(),
				]
			};

			EntityQueryDesc markerObjectsEntityQueryDesc = new EntityQueryDesc
			{
				All =
				[
					ComponentType.ReadOnly<UIObjectData>()
				],
				Any =
				[
					ComponentType.ReadOnly<TrafficSpawnerData>(),
					ComponentType.ReadOnly<CreatureSpawnData>(),
					ComponentType.ReadOnly<ElectricityConnectionData>(),
					ComponentType.ReadOnly<OutsideConnectionData>(),
					//ComponentType.ReadOnly<NetObjectData>(),
					//ComponentType.ReadOnly<NetData>(),
					ComponentType.ReadOnly<TransportStopData>(),
				]
			};

			ExtraLib.AddOnEditEnities(new(OnEditSpacesEntities, spacesEntityQueryDesc));
			ExtraLib.AddOnEditEnities(new(OnEditPathwayEntities, pathwaysEntityQueryDesc));
			ExtraLib.AddOnEditEnities(new(OnEditTrackEntities, tracksEntityQueryDesc));
			ExtraLib.AddOnEditEnities(new(OnEditMarkerObjectEntities, markerObjectsEntityQueryDesc));
			// TransportStopData
		}

		private static void Log(string message)
		{
			ENA.Logger.Info(message);
		}

		private static string GetIcon(PrefabBase prefab)
		{
			Dictionary<string, string> overrideIcons = new()
			{
				{ "Double Train Track - Twoway", "Media/Game/Icons/DoubleTrainTrack.svg" },
				{ "Oneway Tram Track - Inside", "Media/Game/Icons/OnewayTramTrack.svg" },
				{ "Double Subway Track - Twoway", "Media/Game/Icons/DoubleTrainTrack.svg" },
				{ "Twoway Subway Track", "Media/Game/Icons/TwoWayTrainTrack.svg" },
			};
			if (overrideIcons.TryGetValue(prefab.name, out string icon))
			{
				return icon;
			}
			return Icons.GetIcon(prefab);
		}

		private static void OnEditMarkerObjectEntities(NativeArray<Entity> entities)
		{
			foreach (Entity entity in entities)
			{
				ExtraLib.m_PrefabSystem.TryGetPrefab(entity, out MarkerObjectPrefab prefab);

				if (prefab == null)
				{
					continue;
				}

				var prefabUI = prefab.GetComponent<UIObject>();
				if (prefabUI == null)
				{
					prefabUI = prefab.AddComponent<UIObject>();
					prefabUI.active = true;
					prefabUI.m_IsDebugObject = false;
					prefabUI.m_Priority = 1;
				}

				prefabUI.m_Group?.RemoveElement(entity);

				if (prefab.name.Contains("Bus") || prefab.name.Contains("Taxi"))
					prefabUI.m_Group = PrefabsHelper.GetUIAssetCategoryPrefab("TransportationRoad");
				else if (prefab.name.Contains("Train"))
					prefabUI.m_Group = PrefabsHelper.GetUIAssetCategoryPrefab("TransportationTrain");
				else if (prefab.name.Contains("Subway"))
					prefabUI.m_Group = PrefabsHelper.GetUIAssetCategoryPrefab("TransportationSubway");
				else if (prefab.name.Contains("Tram"))
					prefabUI.m_Group = PrefabsHelper.GetUIAssetCategoryPrefab("TransportationTram");
				else if (prefab.name.Contains("Airplane"))
					prefabUI.m_Group = PrefabsHelper.GetUIAssetCategoryPrefab("TransportationAir");
				// Ship stops don't work until the asset editor comes out, they need to be placed within a building prefab
				else if (prefab.name.Contains("Ship")); // Empty statement to prevent the else from catching the ship stops
					//	prefabUI.m_Group = PrefabsHelper.GetUIAssetCategoryPrefab("TransportationWater");
				else
				{
					prefabUI.m_Group = PrefabsHelper.GetOrCreateUIAssetCategoryPrefab("Landscaping", "Marker Object Prefabs", Icons.GetIcon, "Spaces");
					prefabUI.m_Icon = GetIcon(prefab);
				}


				prefabUI.m_Group.AddElement(entity);

				ExtraLib.m_EntityManager.AddOrSetComponentData(entity, prefabUI.ToComponentData());
			}

			Log("Marker Object Entities Edited.");
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
						prefabUI.m_Icon = GetIcon(prefab);
						prefabUI.m_Priority = 1;
					}

					prefabUI.m_Group?.RemoveElement(entity);
					if (prefab.m_TrackType == TrackTypes.Train)
						prefabUI.m_Group = PrefabsHelper.GetUIAssetCategoryPrefab("TransportationTrain");
					if (prefab.m_TrackType == TrackTypes.Subway)
						prefabUI.m_Group = PrefabsHelper.GetUIAssetCategoryPrefab("TransportationSubway");
					if (prefab.m_TrackType == TrackTypes.Tram)
						prefabUI.m_Group = PrefabsHelper.GetUIAssetCategoryPrefab("TransportationTram");
					prefabUI.m_Group.AddElement(entity);

					ExtraLib.m_EntityManager.AddOrSetComponentData(entity, prefabUI.ToComponentData());
				}
			}
			Log("Track Entities Edited.");
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
						prefabUI.m_Icon = GetIcon(prefab);
						prefabUI.m_Priority = 1;

					}
					prefabUI.m_Group?.RemoveElement(entity);
					prefabUI.m_Group = PrefabsHelper.GetUIAssetCategoryPrefab("Pathways");
					prefabUI.m_Group.AddElement(entity);

					ExtraLib.m_EntityManager.AddOrSetComponentData(entity, prefabUI.ToComponentData());
				}
			}
			Log("Pathway Entities Edited.");
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
						prefabUI.m_Icon = GetIcon(prefab);
						prefabUI.m_Priority = 1;
					}

					prefabUI.m_Group?.RemoveElement(entity);
					prefabUI.m_Group = PrefabsHelper.GetOrCreateUIAssetCategoryPrefab("Landscaping", "Spaces", Icons.GetIcon, "Pathways");
					prefabUI.m_Group.AddElement(entity);

					ExtraLib.m_EntityManager.AddOrSetComponentData(entity, prefabUI.ToComponentData());
				}
			}
			Log("Space Entities Edited.");
		}

	}
}
