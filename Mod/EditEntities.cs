using System.Collections.Generic;
using Colossal.Localization;
using ExtraLib.ClassExtension;
using ExtraLib.Helpers;
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
				All = new [] {
					ComponentType.ReadOnly<SpaceData>(),
				},
				None = new [] {
					ComponentType.ReadOnly<PlaceholderObjectElement>(),
				}
			};

			EntityQueryDesc pathwaysEntityQueryDesc = new()
			{
				All = new[] {
					ComponentType.ReadOnly<PathwayData>(),
				},
				None = new [] {
					ComponentType.ReadOnly<PlaceholderObjectElement>(),
				}
			};
			
			EntityQueryDesc taxiwaysEntityQueryDesc = new()
			{
				All = new[] {
					ComponentType.ReadOnly<TaxiwayData>(),
				},
				None = new [] {
					ComponentType.ReadOnly<PlaceholderObjectElement>(),
				}
			};

			EntityQueryDesc tracksEntityQueryDesc = new()
			{
				All = new[] {
					ComponentType.ReadOnly<TrackData>(),
				},
				None = new [] {
					ComponentType.ReadOnly<PlaceholderObjectElement>(),
				}
			};

			EntityQueryDesc markerObjectsEntityQueryDesc = new EntityQueryDesc
			{
				/*All =
				new [] {
					ComponentType.ReadOnly<UIObjectData>()
				},*/
				Any =
				new [] {
					ComponentType.ReadOnly<TrafficSpawnerData>(),
					ComponentType.ReadOnly<CreatureSpawnData>(),
					ComponentType.ReadOnly<ElectricityConnectionData>(),
					ComponentType.ReadOnly<OutsideConnectionData>(),
					//ComponentType.ReadOnly<NetObjectData>(),
					//ComponentType.ReadOnly<NetData>(),
					ComponentType.ReadOnly<TransportStopData>(),
				}
			};

			ExtraLib.EL.AddOnEditEnities(OnEditSpacesEntities, spacesEntityQueryDesc);
			ExtraLib.EL.AddOnEditEnities(OnEditPathwayEntities, pathwaysEntityQueryDesc);
			ExtraLib.EL.AddOnEditEnities(OnEditTrackEntities, tracksEntityQueryDesc);
			ExtraLib.EL.AddOnEditEnities(OnEditMarkerObjectEntities, markerObjectsEntityQueryDesc);
			ExtraLib.EL.AddOnEditEnities(OnEditTaxiwayEntities, taxiwaysEntityQueryDesc);
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
				{ "Oneway Tram Track - Inside", "Media/Game/Icons/OnewayTramTrack.svg" },
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
				ExtraLib.EL.m_PrefabSystem.TryGetPrefab(entity, out MarkerObjectPrefab prefab);

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
					prefabUI.m_Priority = 1000;
				}

				if (prefab.name.Contains("Integrated"))
					prefabUI.m_Priority = 900;

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
				else if (prefab.name.Contains("Ship"))
						prefabUI.m_Group = PrefabsHelper.GetUIAssetCategoryPrefab("TransportationShip");
				else
				{
					prefabUI.m_Group = PrefabsHelper.GetOrCreateUIAssetCategoryPrefab("Landscaping", "Marker Object Prefabs", Icons.GetIcon, "Spaces");
					prefabUI.m_Icon = GetIcon(prefab);
				}


				prefabUI.m_Group.AddElement(entity);

				ExtraLib.EL.m_EntityManager.AddOrSetComponentData(entity, prefabUI.ToComponentData());
				var localizedName = ENA.LocalizationManager.GetLocalizedName(prefab.name);
				if (string.IsNullOrEmpty(localizedName.Trim()))
					Log($"[EMPTY] Edited Marker Entity: {prefab.name} with UI name: {localizedName}");
				else
					Log($"Edited Marker Entity: {prefab.name} with UI name: {localizedName}");
				
			}

			Log("Marker Object Entities Edited.");
		}

		private static void OnEditTrackEntities(NativeArray<Entity> entities)
		{
			foreach (Entity entity in entities)
			{
				if (ExtraLib.EL.m_PrefabSystem.TryGetPrefab(entity, out TrackPrefab prefab))
				{
					var prefabUI = prefab.GetComponent<UIObject>();
					if (prefabUI == null)
					{
						prefabUI = prefab.AddComponent<UIObject>();
						prefabUI.active = true;
						prefabUI.m_IsDebugObject = false;
						prefabUI.m_Icon = GetIcon(prefab);
						if (prefab.name.ToLower().Contains("bridge"))
							continue;
						/*if (prefab.name == "Double Train Track - Twoway") // Station tracks at the end
							prefabUI.m_Priority = 900;
						else*/
							prefabUI.m_Priority = 900;
					}
					else
					{
						var i = GetIcon(prefab);
						if (!i.Contains("placeholder"))
							prefabUI.m_Icon = i;
					}

					prefabUI.m_Group?.RemoveElement(entity);
					if (prefab.m_TrackType == TrackTypes.Train)
						prefabUI.m_Group = PrefabsHelper.GetUIAssetCategoryPrefab("TransportationTrain");
					if (prefab.m_TrackType == TrackTypes.Subway)
						prefabUI.m_Group = PrefabsHelper.GetUIAssetCategoryPrefab("TransportationSubway");
					if (prefab.m_TrackType == TrackTypes.Tram)
						prefabUI.m_Group = PrefabsHelper.GetUIAssetCategoryPrefab("TransportationTram");
					prefabUI.m_Group.AddElement(entity);

					ExtraLib.EL.m_EntityManager.AddOrSetComponentData(entity, prefabUI.ToComponentData());
					var localizedName = ENA.LocalizationManager.GetLocalizedName(prefab.name);
					if (string.IsNullOrEmpty(localizedName.Trim()))
						Log($"[EMPTY] Edited Track Entity: {prefab.name} with UI name: {localizedName}");
					else
						Log($"Edited Track Entity: {prefab.name} with UI name: {localizedName}");
				}
			}
			Log("Track Entities Edited.");
		}
		
		private static void OnEditTaxiwayEntities(NativeArray<Entity> entities)
		{
			foreach (Entity entity in entities)
			{
				if (ExtraLib.EL.m_PrefabSystem.TryGetPrefab(entity, out TaxiwayPrefab prefab))
				{
					if (!prefab.name.Contains("Airplane Taxiway"))
					{
						Log("Skipping entity: " + prefab.name + " as it is not a Taxiway prefab.");
						continue;
					}
					var prefabUI = prefab.GetComponent<UIObject>();
					if (prefabUI == null)
					{
						prefabUI = prefab.AddComponent<UIObject>();
						prefabUI.active = true;
						prefabUI.m_IsDebugObject = false;
						prefabUI.m_Icon = GetIcon(prefab);
						prefabUI.m_Priority = 900;
					}
					else
					{
						var i = GetIcon(prefab);
						if (!i.Contains("placeholder"))
							prefabUI.m_Icon = i;
					}
					

					var category = PrefabsHelper.GetUIAssetCategoryPrefab("TransportationAir");
					if (category == null)
					{
						return;
					}
					
					prefabUI.m_Group?.RemoveElement(entity);
					if (prefab.m_Taxiway)
						prefabUI.m_Group = category;
					prefabUI.m_Group.AddElement(entity);

					ExtraLib.EL.m_EntityManager.AddOrSetComponentData(entity, prefabUI.ToComponentData());
				}
			}
			Log("Taxiway Entities Edited.");
		}

		private static void OnEditPathwayEntities(NativeArray<Entity> entities)
		{
			foreach (Entity entity in entities)
			{
				if (ExtraLib.EL.m_PrefabSystem.TryGetPrefab(entity, out PathwayPrefab prefab))
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

					ExtraLib.EL.m_EntityManager.AddOrSetComponentData(entity, prefabUI.ToComponentData());
					var localizedName = ENA.LocalizationManager.GetLocalizedName(prefab.name);
					if (string.IsNullOrEmpty(localizedName.Trim()))
						Log($"[EMPTY] Edited Pathway Entity: {prefab.name} with UI name: {localizedName}");
					else
						Log($"Edited Pathway Entity: {prefab.name} with UI name: {localizedName}");
				}
			}
			
			Log("Pathway Entities Edited.");
		}

		private static void OnEditSpacesEntities(NativeArray<Entity> entities)
		{
			foreach (Entity entity in entities)
			{
				if (ExtraLib.EL.m_PrefabSystem.TryGetPrefab(entity, out SpacePrefab prefab))
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

					ExtraLib.EL.m_EntityManager.AddOrSetComponentData(entity, prefabUI.ToComponentData());
				}
			}
			Log("Space Entities Edited.");
		}

	}
}
