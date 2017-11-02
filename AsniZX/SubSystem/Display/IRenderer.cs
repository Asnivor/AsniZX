
namespace AsniZX.SubSystem.Display
{
    /// <summary>
    /// Interface for different rendering solutions
    /// </summary>
    public interface IRenderer
    {
        // The renderer type
        RenderEngine Renderer { get; }

        // The draw method
        void Draw(FrameData fd);

        // The initialisation method
        void Initialise(DisplayHandler displayHandler); // ZXForm form);

        // The stop rendering method
        void StopRendering();


    }

    
}
