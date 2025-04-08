using System.Diagnostics.CodeAnalysis;
using KingdomRenderer.Shared.Zat;

namespace KingdomRenderer.KrSettings
{
    
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class AutomaticRenderingSettings
    {
        [Setting("Automatic Rending Enabled", "Whether or not to enable automatic rendering (Requires rendering to be enabled)")]
        [Toggle(true, "Enabled")]
        public InteractiveToggleSetting Enabled { get; private set; }
        
        [Setting("Number of renders per 10 years", "Number of renderings per 10 in-game year")]
        [Slider(1, 150, 30, "30", true)]
        public InteractiveSliderSetting RendPer10Year { get; private set; }
        
    }
}