using System.Drawing;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Drawing.Exif;
using Sitecore.Drawing.Exif.Properties;
using SecurityCheck = Sitecore.SecurityModel.SecurityCheck;

namespace Sitecore.Support.Resources.Media
{
    using System.Globalization;
    using Sitecore.Resources.Media;

    /// <summary>
    /// ImageMedia class
    /// </summary>
    public class JpegMedia : Sitecore.Support.Resources.Media.ImageMedia
    {
        #region IPrototype implementation and initialization

        /// <summary>
        /// Clones the source object. Has the same effect as creating a
        /// new instance of the type.
        /// </summary>
        /// <returns></returns>
        public override Media Clone()
        {
            Assert.IsTrue(GetType() == typeof(JpegMedia), "The Clone() method must be overriden to support prototyping.");

            return new JpegMedia();
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Clears the meta data of a media item.
        /// </summary>
        protected override void ClearMetaData()
        {
            base.ClearMetaData();
            ClearFields("make", "dateTime", "imageDescription", "model", "software", "artist", "copyright");
        }

        /// <summary>
        /// Updates the meta data of a media item.
        /// </summary>
        public override void UpdateMetaData(MediaStream mediaStream)
        {
            base.UpdateMetaData(mediaStream);

            if (!mediaStream.AllowMemoryLoading)
            {
                Tracer.Error("Could not update JPEG meta data as the image is larger than the maximum size allowed for memory processing. Media item: {0}", mediaStream.MediaItem.Path);
                return;
            }

            Reader reader = null;

            using (Image image = GetImage())
            {
                if (image != null)
                {
                    reader = Reader.Parse(image);
                }

                Item item = MediaData.MediaItem.InnerItem;

                using (new EditContext(item, SecurityCheck.Disable))
                {
                    SetFieldValue(item, "make", reader, 271);
                    SetFieldValue(item, "dateTime", reader, 306);
                    SetFieldValue(item, "imageDescription", reader, 270);
                    SetFieldValue(item, "model", reader, 272);
                    SetFieldValue(item, "software", reader, 305);
                    SetFieldValue(item, "artist", reader, 315);
                    SetFieldValue(item, "copyright", reader, 33432);
                    SetGeoFieldValue(item, "latitude", reader, (int)Tag.GpsLatitude);
                    SetGeoFieldValue(item, "longitude", reader, (int)Tag.GpsLongitude);
                }
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Sets a field value.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="tagId">The tag id.</param>
        void SetFieldValue(Item item, string fieldName, Reader reader, int tagId)
        {
            item[fieldName] = string.Empty;

            if (reader == null)
            {
                return;
            }

            Property property = reader.GetProperty(tagId);

            if (property == null)
            {
                return;
            }

            StringProperty stringProperty = property as StringProperty;

            if (stringProperty == null || stringProperty.Value == null)
            {
                return;
            }

            item[fieldName] = stringProperty.Value;
        }

        /// <summary>
        /// Sets a field geolocation value.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="tagId">The tag id.</param>
        void SetGeoFieldValue(Item item, string fieldName, Reader reader, int tagId)
        {
            item[fieldName] = string.Empty;

            if (reader == null)
            {
                return;
            }

            Property property = reader.GetProperty(tagId);

            if (property == null)
            {
                return;
            }

            GeoLocationProperty geoProperty = property as GeoLocationProperty;

            if (geoProperty == null)
            {
                return;
            }

            var sign = 1;

            switch (tagId)
            {
                case (int)Tag.GpsLatitude:
                    var latitudeRef = reader.GetProperty((int)Tag.GpsLatitudeRef);
                    if (latitudeRef != null && ((StringProperty)latitudeRef).Value == GeoLocationProperty.SOUTH)
                    {
                        sign = -1;
                    }
                    break;
                case (int)Tag.GpsLongitude:
                    var longitudeRef = reader.GetProperty((int)Tag.GpsLongitudeRef);
                    if (longitudeRef != null && ((StringProperty)longitudeRef).Value == GeoLocationProperty.WEST)
                    {
                        sign = -1;
                    }
                    break;
            }

            item[fieldName] = (geoProperty.Value * sign).ToString(CultureInfo.InvariantCulture);
        }


        #endregion
    }
}
