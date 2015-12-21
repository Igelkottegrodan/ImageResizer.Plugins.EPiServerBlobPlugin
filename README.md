# ImageResizer.Plugins.EPiServerBlobPlugin
First based on my initial blog post (http://world.episerver.com/blogs/Andre-Hedberg/Dates/2013/12/Get-ImageResizer-to-play-along-with-EPiServer-75/) and now later adjusted based on excellent feedback from Martin Pickering and source code (http://world.episerver.com/Code/Martin-Pickering/ImageResizingNet-integration-for-CMS75/).

There are some differences:
- Not bound to image types (plugins such as PdfRrenderer http://imageresizing.net/docs/v4/plugins/pdfrenderer now works)
- URL check for edit/preview mode is not regex based, it uses EPiServers native methods.
 
Enjoy!
