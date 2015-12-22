using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using EPiServer.Web.Routing.Segments;
using ImageResizer.Configuration;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web;

namespace ImageResizer.Plugins.EPiServerBlob
{
    public class EPiServerBlobPlugin : IVirtualImageProvider, IPlugin
    {
        private readonly UrlResolver __urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();

        public bool FileExists(string virtualPath, NameValueCollection queryString)
        {
            EPiServerBlobMedia blobImage = this.GetBlobFile(virtualPath);

            return (blobImage != null && blobImage.Exists);
        }

        public IVirtualFile GetFile(string virtualPath, NameValueCollection queryString)
        {
            return this.GetBlobFile(virtualPath);
        }

        private EPiServerBlobMedia GetBlobFile(string virtualPath)
        {
            EPiServerBlobMedia blobFile = new EPiServerBlobMedia(virtualPath);

            return blobFile;
        }

        public IPlugin Install(Config config)
        {
            config.Plugins.add_plugin(this);
            config.Pipeline.PostAuthorizeRequestStart += OnPostAuthorizeRequestStart;

            return this;
        }

        public bool Uninstall(Config config)
        {
            config.Plugins.remove_plugin(this);
            config.Pipeline.PostAuthorizeRequestStart -= OnPostAuthorizeRequestStart;

            return true;
        }

        private void OnPostAuthorizeRequestStart(IHttpModule sender, HttpContext context)
        {
            string absolutePath = context.Request.Url.AbsolutePath;
            IContent resolvedContent = __urlResolver.Route(new UrlBuilder(absolutePath));

            if (resolvedContent == null)
            {                
                return;
            }

            bool isMediaContent = resolvedContent is MediaData;

            if (!isMediaContent)
            {
                return;
            }

            Config.Current.Pipeline.SkipFileTypeCheck = true;

            bool previewOrEditMode = (
                RequestSegmentContext.CurrentContextMode == ContextMode.Edit ||
                RequestSegmentContext.CurrentContextMode == ContextMode.Preview
            );

            // Disable cache if editing or previewing
            if (previewOrEditMode)
            {
                Config.Current.Pipeline.PreRewritePath = this.CleanEditModePath(absolutePath);
                NameValueCollection modifiedQueryString = new NameValueCollection(Config.Current.Pipeline.ModifiedQueryString);

                modifiedQueryString.Add("process", ProcessWhen.Always.ToString());
                modifiedQueryString.Add("cache", ServerCacheMode.No.ToString());
                Config.Current.Pipeline.ModifiedQueryString = modifiedQueryString;
            }                    
        }

        private string CleanEditModePath(string path)
        {
            return Regex.Replace(path, @",.*$", string.Empty);
        }
    }
}