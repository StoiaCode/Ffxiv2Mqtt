﻿using Ffxiv2Mqtt.TopicTracker.Interfaces;

namespace Ffxiv2Mqtt.TopicTracker.Events
{
    internal class TerritoryTopic : Topic, ICleanable
    {
        public string Name
        {
            get
            {
                var resolvedTerritory = Dalamud.GameData?.Excel?.GetSheet<Lumina.Excel.GeneratedSheets.TerritoryType>()?.GetRow(territory)?.PlaceName?.Value?.Name;
                if (resolvedTerritory is not null)
                    return resolvedTerritory.ToString();
                else
                    return "Unknown";
            }
        }

        ushort territory;

        public TerritoryTopic(MqttManager mqttManager) : base(mqttManager)
        {
            topic = "Event/TerritoryChanged";
            Dalamud.ClientState.TerritoryChanged += TerritoryChanged;
        }

        internal void TerritoryChanged(object? s, ushort e)
        {
            territory = e;
            Publish(true);
        }

        public void Cleanup()
        {
            mqttManager.PublishMessage(topic, "");
            Dalamud.ClientState.TerritoryChanged -= TerritoryChanged;
        }
    }
}