﻿using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace Ffxiv2Mqtt.EventHandlers.JobGaugeTrackers
{
    internal class MonkGaugeTracker : BaseGaugeTracker
    {
        private byte chakra;
        private BeastChakra[] beastChakra;
        private ushort blitzTimeRemaining;


        public MonkGaugeTracker(MqttManager m) : base(m)
        {
            beastChakra = new BeastChakra[3];
        }

        public void Update(MNKGauge monkGauge)
        {
            mqttManager.TestValue(monkGauge.Chakra, ref chakra, "JobGauge/MNK/Chakra");

            for (var i = 0; i < 3; i++)
            {
                mqttManager.TestValue(monkGauge.BeastChakra[i], ref beastChakra[i], string.Format("JobGauge/MNK/BeastChakra{0}", i + 1));
            }

            mqttManager.TestCountDown(monkGauge.BlitzTimeRemaining, ref blitzTimeRemaining, 1000, "JobGauge/MNK/BlitzTimer");
        }
    }
}
