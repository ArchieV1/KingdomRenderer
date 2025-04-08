using System.Diagnostics.CodeAnalysis;
using KingdomRenderer.Shared.Zat;

namespace KingdomRenderer.KrSettings
{
    [Mod("KingdomRenderer", "v1.3", "ArchieV / greenking2000")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    public class KrSettings
    {
        public static KrSettings Inst;

        public KrSettings()
        {
            Inst = this;
        }
        
        [Category("Render Settings")]
        public RenderingSetting RendSettings { get; private set; }

        [Category("Automatic Rendering")]
        public AutomaticRenderingSettings AutoRend { get; private set; }
        
        [Category("Manual Rendering")]
        public ManualRenderingSettings ManRend { get; private set; }
    }
}