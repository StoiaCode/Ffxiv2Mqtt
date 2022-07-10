﻿using System;
using System.Text.Json;
using Dalamud.IoC;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Ffxiv2Mqtt.Services;

namespace Ffxiv2Mqtt.Topics.Data
{
    internal class PlayerCast : Topic, IDisposable
    {
        // ReSharper disable once MemberCanBePrivate.Global
        [PluginService] public PlayerEvents? PlayerEvents { get; set; }

        protected override string TopicPath => "Player/Casting";
        protected override bool   Retained  => false;

        private bool isCasting;

        public override void Initialize()
        {
            PlayerEvents!.LocalPlayerUpdated += PlayerUpdated;
        }

        // Publish a message if the player either starts or stops casting.
        private void PlayerUpdated(PlayerCharacter localPlayer)
        {
            var shouldPublish = false;

            TestValue(localPlayer.IsCasting, ref isCasting, ref shouldPublish);

            if (shouldPublish) {
                Publish(JsonSerializer.Serialize(new
                                                 {
                                                     IsCasting = isCasting,
                                                     CastId    = localPlayer.CastActionId,
                                                     CastTime  = localPlayer.TotalCastTime,
                                                 }));
            }
        }

        public void Dispose()
        {
            PlayerEvents!.LocalPlayerUpdated -= PlayerUpdated;
        }
    }
}
