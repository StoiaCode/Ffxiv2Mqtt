﻿using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Ffxiv2Mqtt.Topic.Interfaces;

namespace Ffxiv2Mqtt.Topic.Data
{
    internal class MonkGaugeTopic : JobGaugeTopic, IUpdatable
    {
        public byte Chakra { get => chakra; }
        public BeastChakra[] BeastChakras { get => beastChakra; }
        public ushort BlitzTimeRemaining { get => blitzTimeRemaining; }

        private byte chakra;
        private BeastChakra[] beastChakra;
        private ushort blitzTimeRemaining;

        private const uint MonkId = 20;

        public MonkGaugeTopic(MqttManager m) : base(m)
        {
            topic = "Player/JobGauge/MNK";
            beastChakra = new BeastChakra[3];
        }

        public void Update()
        {
            if (DalamudServices.ClientState.IsPvP)
                return;
            var localPlayer = DalamudServices.ClientState.LocalPlayer;
            if (localPlayer is null)
                return;
            if (localPlayer.ClassJob.Id != MonkId)
                return;
            var gauge = DalamudServices.JobGauges.Get<MNKGauge>();

            TestValue(gauge.Chakra, ref chakra);
            for (var i = 0; i < 3; i++)
            {
                TestValue(gauge.BeastChakra[i], ref beastChakra[i]);
            }
            TestCountDown(gauge.BlitzTimeRemaining, ref blitzTimeRemaining, syncTimer);

            PublishIfNeeded();
        }
    }
}