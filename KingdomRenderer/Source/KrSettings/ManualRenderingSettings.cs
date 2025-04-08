using System.Diagnostics.CodeAnalysis;
using KingdomRenderer.Shared.Zat;
using UnityEngine;

namespace KingdomRenderer.KrSettings
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class ManualRenderingSettings
    {
        [Setting("Manual Rending Enabled", "Whether or not to enable manual rendering (Requires rendering to be enabled)")]
        [Toggle(true, "Enabled")]
        public InteractiveToggleSetting Enabled { get; private set; }
        
        [Setting("Manual Render Hotkey", "Which button to press to manually render the map")]
        [Hotkey(KeyCode.N)]
        public InteractiveHotkeySetting RenderKey { get; private set; }
    }
}