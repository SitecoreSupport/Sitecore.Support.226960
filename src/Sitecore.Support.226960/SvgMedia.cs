namespace Sitecore.Support.Resources.Media
{
    using Sitecore.Diagnostics;
    using Sitecore.Resources.Media;

    /// <summary>
    /// The SVG media.
    /// </summary>
    public class SvgMedia : Sitecore.Support.Resources.Media.ImageMedia
    {
        /// <summary>
        /// The clone.
        /// </summary>
        /// <returns>
        /// The <see cref="Media"/>.
        /// </returns>
        public override Media Clone()
        {
            Assert.IsTrue(this.GetType() == typeof(SvgMedia), "The Clone() method must be overridden to support prototyping.");

            return new SvgMedia();
        }

        /// <summary> The update meta data. </summary>
        /// <param name="mediaStream">The media stream.</param>
        protected override void UpdateImageMetaData([CanBeNull]MediaStream mediaStream)
        {
        }
    }
}