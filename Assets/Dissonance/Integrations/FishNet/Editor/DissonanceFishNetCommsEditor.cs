using Dissonance.Editor;
using UnityEditor;

namespace Dissonance.Integrations.FishNet.Editor
{
    // Editor script for Dissonance Voice Comms
    [CustomEditor(typeof(DissonanceFishNetComms))]
    public class DissonanceFishNetCommsEditor : BaseDissonnanceCommsNetworkEditor<DissonanceFishNetComms, DissonanceFishNetServer, DissonanceFishNetClient, DissonanceFishNetConnection, Unit, Unit> { }
}