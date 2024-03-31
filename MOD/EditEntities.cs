using System.Collections.Generic;
using Extra.Lib.Helper;
using Extra.Lib;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using Colossal.Entities;
using Game.Net;
using Game.Objects;
using UnityEngine;

namespace ExtraNetworksAndAreas
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

			ExtraLib.AddOnEditEnities(new(OnEditSpacesEntities, spacesEntityQueryDesc));
			ExtraLib.AddOnEditEnities(new(OnEditPathwayEntities, pathwaysEntityQueryDesc));
			ExtraLib.AddOnEditEnities(new(OnEditTrackEntities, tracksEntityQueryDesc));
			// TransportStopData
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
			return Icons.GetIcon(prefab);
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
						prefabUI.m_Icon = GetIcon(prefab);
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
