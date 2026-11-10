using System;
using System.IO;
using UnityEngine;

namespace Diyar.Game
{
    public static class SaveManager
    {
        private const string SaveFileName = "diyar_save.json";

        private static string SavePath =>
            Path.Combine(Application.persistentDataPath, SaveFileName);

        public static void Save(GameSaveData data)
        {
            if (data == null)
            {
                Debug.LogError("Save-Daten fehlen.");
                return;
            }

            try
            {
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(SavePath, json);
                Debug.Log("Spielstand gespeichert.");
            }
            catch (Exception exception)
            {
                Debug.LogError($"Speichern fehlgeschlagen: {exception.Message}");
            }
        }

        public static GameSaveData Load()
        {
            if (!File.Exists(SavePath))
            {
                return new GameSaveData();
            }

            try
            {
                string json = File.ReadAllText(SavePath);
                return JsonUtility.FromJson<GameSaveData>(json)
                    ?? new GameSaveData();
            }
            catch (Exception exception)
            {
                Debug.LogError($"Laden fehlgeschlagen: {exception.Message}");
                return new GameSaveData();
            }
        }

        public static bool HasSave => File.Exists(SavePath);
        public static void SaveBalance(int balance)
        {
            GameSaveData data = Load();
            data.Balance = Mathf.Max(0, balance);
            Save(data);
        }
        public static bool IsJobCompleted(string jobId)
        {
            if (string.IsNullOrWhiteSpace(jobId))
            {
                return false;
            }

            GameSaveData data = Load();

            return data.CompletedJobIds != null
                && data.CompletedJobIds.Contains(jobId);
        }

        public static void MarkJobCompleted(string jobId)
        {
            if (string.IsNullOrWhiteSpace(jobId))
            {
                return;
            }

            GameSaveData data = Load();

            if (data.CompletedJobIds == null)
            {
                data.CompletedJobIds = new System.Collections.Generic.List<string>();
            }

            if (!data.CompletedJobIds.Contains(jobId))
            {
                data.CompletedJobIds.Add(jobId);
                Save(data);
            }
        }
        public static PropertySaveData FindProperty(string propertyId)
        {
            if (string.IsNullOrWhiteSpace(propertyId))
            {
                return null;
            }

            GameSaveData data = Load();

            if (data.Properties == null)
            {
                return null;
            }

            return data.Properties.Find(
                item => item != null && item.PropertyId == propertyId);
        }

        public static void SavePropertyState(
            string propertyId,
            bool isOwned,
            bool isSold)
        {
            if (string.IsNullOrWhiteSpace(propertyId))
            {
                return;
            }

            GameSaveData data = Load();

            if (data.Properties == null)
            {
                data.Properties =
                    new System.Collections.Generic.List<PropertySaveData>();
            }

            PropertySaveData propertyData = data.Properties.Find(
                item => item != null && item.PropertyId == propertyId);

            if (propertyData == null)
            {
                propertyData = new PropertySaveData { PropertyId = propertyId };
                data.Properties.Add(propertyData);
            }

            propertyData.IsOwned = isOwned;
            propertyData.IsSold = isSold;

            Save(data);
        }
        
    }
}