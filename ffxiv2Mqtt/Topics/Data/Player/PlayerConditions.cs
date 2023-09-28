﻿using System;
using System.Text.Json;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.IoC;
using Dalamud.Plugin.Services;

namespace Ffxiv2Mqtt.Topics.Data.Player;

internal sealed class PlayerConditions : Topic, IDisposable
{
    protected override string TopicPath => "Player/Conditions";
    protected override bool   Retained  => false;

    // ReSharper disable once MemberCanBePrivate.Global
    [PluginService] public ICondition? Conditions { get; set; }

    public override void Initialize()
    {
        Conditions!.ConditionChange += ConditionChange;
    }

    // Publish to each condition's topic whenever the state changes.
    private void ConditionChange(ConditionFlag flag, bool value)
    {
        Publish($"{TopicPath}/{flag}", JsonSerializer.Serialize(new
                                                                {
                                                                    Id     = (int)flag,
                                                                    Active = value,
                                                                }));
    }

    public void Dispose()
    {
        Conditions!.ConditionChange -= ConditionChange;
    }
}
