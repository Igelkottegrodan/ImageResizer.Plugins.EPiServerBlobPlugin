using System;
using System.IO;
using EPiServer.Core;
using EPiServer.Framework.Blobs;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

namespace ImageResizer.Plugins.EPiServerBlob
{
    public class EPiServerBlobMedia : IVirtualFileWithModifiedDate
    {
        private static readonly DateTime __minUniversalDateTime = DateTime.MinValue.ToUniversalTime();
        private Blob _blob;
        private MediaData _mediaData;
        protected readonly ContentRouteHelper ContentRouteHelper;

        public EPiServerBlobMedia(string virtualPath)
            : this(virtualPath, ServiceLocator.Current.GetInstance<ContentRouteHelper>())
        {

        }

        public EPiServerBlobMedia(string virtualPath, ContentRouteHelper contentRouteHelper)
        {
            this.ContentRouteHelper = contentRouteHelper;
            this.VirtualPath = virtualPath;
        }

        public Blob Blob
        {
            get
            {
                if (this._blob != null)
                {
                    return this._blob;
                }

                if (this.MediaData == null)
                {
                    return null;
                }

                var binaryStorable = MediaData as IBinaryStorable;
                if (binaryStorable == null || binaryStorable.BinaryData == null)
                {
                    return null;
                }

                this._blob = binaryStorable.BinaryData;

                return this._blob;
            }
        }

        public MediaData MediaData
        {
            get
            {
                if (this._mediaData != null)
                {
                    return this._mediaData;
                }

                this._mediaData = this.ContentRouteHelper.Content as MediaData;

                return this._mediaData;
            }
        }

        public bool Exists
        {
            get { return (this.MediaData != null && this.MediaData.BinaryData != null); }
        }

        public Stream Open()
        {
            if (this.Blob != null)
            {
                return this.Blob.OpenRead();
            }

            return null;
        }

        public string VirtualPath { get; private set; }

        public DateTime ModifiedDateUTC
        {
            get
            {
                if (this.MediaData == null)
                {
                    return __minUniversalDateTime;
                }

                var changeTrackable = this.MediaData as IChangeTrackable;
                if (changeTrackable == null)
                {
                    return __minUniversalDateTime;
                }

                return changeTrackable.Changed.ToUniversalTime();
            }
        }
    }
}
