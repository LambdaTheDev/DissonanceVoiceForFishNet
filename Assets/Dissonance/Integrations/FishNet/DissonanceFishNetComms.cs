using System;
using Dissonance.Integrations.FishNet.Constants;
using Dissonance.Networking;
using UnityEngine;

namespace Dissonance.Integrations.FishNet
{
    // Comms implementation for Dissonance Voice. Used Unit, due to FishNet is connected while using Dissonance Voice
    public sealed class DissonanceFishNetComms : BaseCommsNetwork<DissonanceFishNetServer, DissonanceFishNetClient, DissonanceFishNetConnection, Unit, Unit>
    {
        public static DissonanceFishNetComms Instance { get; private set; }
        
        public DissonanceComms Comms { get; private set; }
        

        private void Awake()
        {
            if(Instance != null)
                Debug.LogError("Dissonance Voice Chat for FishNet supports only one DissonanceComms object instance at time! " +
                               "If this is a big problem for you, contact me on Discord: " + DissonanceFishNetConstants.SupportDiscordServer);

            Instance = this;
            Comms = GetComponent<DissonanceComms>();
        }

        protected override DissonanceFishNetServer CreateServer(Unit connectionParameters)
        {
            return new DissonanceFishNetServer();
        }

        protected override DissonanceFishNetClient CreateClient(Unit connectionParameters)
        {
            return new DissonanceFishNetClient(this);
        }
    }
}