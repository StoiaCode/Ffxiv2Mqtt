﻿using System;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.IoC;
using Dalamud.Plugin.Services;
using Ffxiv2Mqtt.Enums;
using Ffxiv2Mqtt.Services;

namespace Ffxiv2Mqtt.Topics.Data.Player;

internal class WarriorGauge : Topic, IDisposable
{
    private byte beastGauge;

    protected override string TopicPath => "Player/JobGauge/WAR";
    protected override bool   Retained  => false;

    [PluginService] public PlayerEvents? PlayerEvents { get; set; }
    [PluginService] public IJobGauges?    JobGauges    { get; set; }
    [PluginService] public IClientState?  ClientState  { get; set; }

    public override void Initialize()
    {
        PlayerEvents!.LocalPlayerUpdated += PlayerUpdated;
    }

    private void PlayerUpdated(PlayerCharacter localPlayer)
    {
        if (ClientState!.IsPvP)
            return;
        if ((Job)localPlayer.ClassJob.Id != Job.Warrior)
            return;
        var gauge = JobGauges?.Get<WARGauge>();
        if (gauge is null)
            return;

        var shouldPublish = false;
        TestValue(gauge.BeastGauge, ref beastGauge, ref shouldPublish);

        if (shouldPublish)
            Publish(new
                    {
                        gauge.BeastGauge,
                    });
    }

    public void Dispose()
    {
        PlayerEvents!.LocalPlayerUpdated -= PlayerUpdated;
    }
}
