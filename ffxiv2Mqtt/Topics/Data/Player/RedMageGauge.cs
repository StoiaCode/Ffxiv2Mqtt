﻿using System;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Ffxiv2Mqtt.Enums;
using Ffxiv2Mqtt.Extensions;

namespace Ffxiv2Mqtt.Topics.Data.Player;

internal class RedMageGauge : Topic, IDisposable
{
    private byte blackMana;
    private byte whiteMana;
    private byte manaStacks;

    protected override string TopicPath => "Player/JobGauge/RDM";
    protected override bool   Retained  => false;

    public RedMageGauge() { Service.PlayerEvents.LocalPlayerUpdated += PlayerUpdated; }

    private void PlayerUpdated(IPlayerCharacter localPlayer)
    {
        if (Service.ClientState.IsPvP)
            return;
        if (!localPlayer.IsJob(Job.RedMage))
            return;
        var gauge = Service.JobGauges.Get<RDMGauge>();

        var shouldPublish = false;
        TestValue(gauge.ManaStacks, ref manaStacks, ref shouldPublish);
        TestValue(gauge.BlackMana,  ref blackMana,  ref shouldPublish);
        TestValue(gauge.WhiteMana,  ref whiteMana,  ref shouldPublish);

        if (shouldPublish)
            Publish(new
                    {
                        gauge.BlackMana,
                        gauge.WhiteMana,
                        gauge.ManaStacks,
                    });
    }

    public void Dispose() { Service.PlayerEvents.LocalPlayerUpdated -= PlayerUpdated; }
}
