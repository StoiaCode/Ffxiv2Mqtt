﻿using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace Ffxiv2Mqtt.TopicTracker
{
    internal class MonkGaugeTracker : BaseTopicTracker, IUpdatable
    {
        public byte Chakra { get => chakra; }
        public BeastChakra[] BeastChakras { get => beastChakra; }
        public ushort BlitzTimeRemaining { get => blitzTimeRemaining; }

        private byte chakra;
        private BeastChakra[] beastChakra;
        private ushort blitzTimeRemaining;

        private const uint MonkId = 20;

        public MonkGaugeTracker(MqttManager m) : base(m)
        {
            topic = "Player/JobGauge/MNK";
            beastChakra = new BeastChakra[3];
        }

        public void Update()
        {
            if (Dalamud.ClientState.IsPvP)
                return;
            var localPlayer = Dalamud.ClientState.LocalPlayer;
            if (localPlayer is null)
                return;
            if (localPlayer.ClassJob.Id != MonkId)
                return;
            var gauge = Dalamud.JobGauges.Get<MNKGauge>();

            TestValue(gauge.Chakra, ref chakra);
            for (var i = 0; i < 3; i++)
            {
                TestValue(gauge.BeastChakra[i], ref beastChakra[i]);
            }
            TestCountDown(gauge.BlitzTimeRemaining, ref blitzTimeRemaining, 1000);

            PublishIfNeeded();
        }
    }
}
