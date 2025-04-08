using System.Diagnostics.CodeAnalysis;
using KingdomRenderer.Shared.Zat;

namespace KingdomRenderer.KrSettings
{
    
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class RenderingSetting
    {
        [Setting("Enabled", "Whether or not to enable rendering")]
        [Toggle(true, "Enabled")]
        public InteractiveToggleSetting Enabled { get; private set; }
        
        [Setting("Rending Height", "How far above the map the render is taken from")]
        [Slider(2f, 15f, 4.5f, "4.5")]
        public InteractiveSliderSetting RendHeight { get; private set; }
        
        [Setting("Render Resolution Scaling Factor", "The scaling factor of the render where 1.0 is 1920x1080\n" +
                                                     "Larger number will impact performance while rendering but look better")]
        [Slider(0.1f, 16f, 1f, "1")]
        public InteractiveSliderSetting ResScaling { get; private set; }

        [Setting("Render Resolution X", "Larger resolutions will cause the rendering process to take longer")]
        [Select(6, "160", "320", "480", "512", "720", "1280", "1920", "3840", "4096", "7680", "15360")]
        public InteractiveSelectSetting ResolutionX { get; private set; }
        
        [Setting("Render Resolution Y", "Larger resolutions will cause the rendering process to take longer")]
        [Select(6, "120", "240", "272", "342", "480", "960", "1080", "2160", "3072", "4320", "8640")]
        public InteractiveSelectSetting ResolutionY { get; private set; }
        
        [Setting("Render Filetype", "Where do you want your images to be saved?")]
        [Select(0, nameof(global::KingdomRenderer.FileLocation.SteamApps), nameof(global::KingdomRenderer.FileLocation.AppData), nameof(global::KingdomRenderer.FileLocation.Pictures), nameof(global::KingdomRenderer.FileLocation.Desktop), nameof(global::KingdomRenderer.FileLocation.Documents))]
        public InteractiveSelectSetting FileLocation { get; private set; }
    }
}