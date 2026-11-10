using System;
using System.Collections.Generic;

namespace Diyar.Game
{
    [Serializable]
    public sealed class GameSaveData
    {
        public int Version = 1;
        public int Balance;
        public List<string> CompletedJobIds = new List<string>();
        public List<PropertySaveData> Properties = new List<PropertySaveData>();
    }

    [Serializable]
    public sealed class PropertySaveData
    {
        public string PropertyId;
        public bool IsOwned;
        public bool IsSold;
        public List<string> RepairedIds = new List<string>();
    }
}