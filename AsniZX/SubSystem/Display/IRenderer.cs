
namespace AsniZX.SubSystem.Display
{
    /// <summary>
    /// Interface for different rendering solutions
    /// </summary>
    public interface IRenderer
    {
        /// <summary>
        /// Renderer type
        /// </summary>
        RenderEngine Renderer { get; }

        /// <summary>
        /// Accepts a framedata object and attempts to draw it to the screen
        /// </summary>
        /// <param name="fd"></param>
        void Draw(FrameData fd);

        /// <summary>
        /// Draw to the screen without framedata object (uses defaults)
        /// </summary>
        void Draw();

        /// <summary>
        /// The init routine - called everytime a display size change or renderer change happens
        /// </summary>
        /// <param name="displayHandler"></param>
        void Initialise(DisplayHandler displayHandler); // ZXForm form);

        /// <summary>
        /// Stops rendering
        /// </summary>
        void StopRendering();


    }

    
}
