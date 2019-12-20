namespace Sitecore.Support.Resources.Media
{
    using System.Drawing;
    using Sitecore.Resources.Media;

    /// <summary>
    /// ImageMedia class
    /// </summary>
    public class ImageMedia : Sitecore.Resources.Media.ImageMedia
    {
        public override Image GetImage()
        {
            MediaStream stream = this.GetStream();

            if (stream == null)
            {
                return null;
            }

            if (stream.Stream.Length == 0)
            {
                return null;
            }

            return Image.FromStream(stream.Stream);
        }
    }
}