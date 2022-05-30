﻿using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game.Command;
using Dalamud.Game.ClientState;
using System.IO;
using Ffxiv2Mqtt.EventHandlers;

namespace Ffxiv2Mqtt
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "FFXIV2MQTT";

        private const string configCommandName = "/mqtt";
        private const string testCommandName = "/mtest";

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        private Configuration Configuration { get; init; }
        private PluginUI PluginUi { get; init; }
        
        
        private MqttManager mqttManager;


        private ClientStateHandler clientStateHandler;
        private ConditionHandler conditionHandler;
        private JobGaugeHandler jobGaugeHandler;



        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            pluginInterface.Create<MqttManager>();


            clientStateHandler = pluginInterface.Create<ClientStateHandler>();
            conditionHandler = pluginInterface.Create<ConditionHandler>();
            jobGaugeHandler = pluginInterface.Create<JobGaugeHandler>();
            
            this.Configuration.Initialize(this.PluginInterface);


            mqttManager.ConnectToBroker();

            this.PluginUi = new PluginUI(this.Configuration, mqttManager);

            this.CommandManager.AddHandler(configCommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Display MQTT Client Info"
            });
            this.CommandManager.AddHandler(testCommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Test"
            });


            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

            this.PluginUi.Visible = true;
        }



        public void Dispose()
        {
            this.PluginUi.Dispose();
            if (clientStateHandler != null) clientStateHandler.Dispose();
            if (conditionHandler != null) conditionHandler.Dispose();
            if (jobGaugeHandler != null) jobGaugeHandler.Dispose();
            this.CommandManager.RemoveHandler(configCommandName);
            this.CommandManager.RemoveHandler(testCommandName);
            if (mqttManager != null) this.mqttManager.Dispose();
        }

        private void OnCommand(string command, string args)
        {
            if (command == configCommandName)
            {
                this.PluginUi.Visible = true;
            }
            else if (command == testCommandName)
            {
                mqttManager.PublishMessage("test", "success");
            }
        }

        private void DrawUI()
        {
            this.PluginUi.Draw();
        }

        private void DrawConfigUI()
        {
            this.PluginUi.SettingsVisible = true;
        }
    }
}
