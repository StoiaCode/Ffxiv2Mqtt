﻿using Ffxiv2Mqtt.TopicTracker.Interfaces;

namespace Ffxiv2Mqtt.TopicTracker.Events
{
    internal class ContentFinderTopic : Topic, ICleanable
    {
        internal ContentFinderTopic(MqttManager m) : base(m)
        {
            topic = "Event/ContentFinder";
            Dalamud.ClientState.CfPop += CfPop;
        }

        private void CfPop(object? s, Lumina.Excel.GeneratedSheets.ContentFinderCondition e)
        {
            mqttManager.PublishMessage(topic, "Pop");
        }

        public void Cleanup()
        {
            Dalamud.ClientState.CfPop -= CfPop;
        }
    }
}