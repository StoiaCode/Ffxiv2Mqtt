﻿using Dalamud.Game.ClientState.JobGauge.Types;
using Ffxiv2Mqtt.TopicTracker.Interfaces;

namespace Ffxiv2Mqtt.TopicTracker.Data
{
    internal class DancerGaugeTracker : BaseGaugeTracker, IUpdatable
    {
        public bool Dancing { get => isDancing; }
        public uint[] Steps { get => steps; }
        public uint NextStep { get => nextStep; }
        public byte CompletedSteps { get => completedSteps; }
        public byte Esprit { get => esprit; }
        public byte Feathers { get => feathers; }

        private bool isDancing;
        private uint[] steps;
        private uint nextStep;
        private byte completedSteps;
        private byte esprit;
        private byte feathers;

        private const uint DancerId = 38;

        public DancerGaugeTracker(MqttManager m) : base(m)
        {
            topic = "Player/JobGauge/DNC";
            steps = new uint[4];
        }

        public void Update()
        {
            if (Dalamud.ClientState.IsPvP)
                return;
            var localPlayer = Dalamud.ClientState.LocalPlayer;
            if (localPlayer is null)
                return;
            if (localPlayer.ClassJob.Id != DancerId)
                return;
            var gauge = Dalamud.JobGauges.Get<DNCGauge>();

            TestValue(gauge.CompletedSteps, ref completedSteps);
            TestValue(gauge.Esprit, ref esprit);
            TestValue(gauge.Feathers, ref feathers);
            TestValue(gauge.IsDancing, ref isDancing);
            TestValue(gauge.NextStep, ref nextStep);
            for (int i = 0; i < steps.Length; i++)
            {
                TestValue(gauge.Steps[i], ref steps[i]);
            }

            PublishIfNeeded();
        }
    }
}
