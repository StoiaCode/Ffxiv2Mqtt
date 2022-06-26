﻿using Ffxiv2Mqtt.TopicTracker.Interfaces;

namespace Ffxiv2Mqtt.TopicTracker.Data
{
    internal class PlayerCombatStatsTracker : BaseTopicTracker, IUpdatable
    {
        public uint HP { get => hp; }
        public uint MP { get => mp; }
        uint hp;
        uint mp;

        internal PlayerCombatStatsTracker(MqttManager mqttManager) : base(mqttManager)
        {
            topic = "Player/Combat/CurrentStats";
        }

        public void Update()
        {
            var localPlayer = Dalamud.ClientState.LocalPlayer;
            if (localPlayer is null)
                return;

            TestValue(localPlayer.CurrentHp, ref hp);
            TestValue(localPlayer.CurrentMp, ref mp);

            PublishIfNeeded();
        }
    }
}
