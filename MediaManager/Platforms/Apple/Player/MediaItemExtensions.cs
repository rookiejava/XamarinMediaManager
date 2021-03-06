﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using AVFoundation;
using Foundation;
using MediaManager.Media;

namespace MediaManager.Platforms.Apple.Player
{
    public static class MediaItemExtensions
    {
        public static Dictionary<string, string> RequestHeaders => CrossMediaManager.Current.RequestHeaders;

        public static AVPlayerItem ToAVPlayerItem(this IMediaItem mediaItem)
        {
            AVAsset asset;

            if (mediaItem.MediaLocation == MediaLocation.Embedded)
            {
                var directory = Path.GetDirectoryName(mediaItem.MediaUri);
                var filename = Path.GetFileNameWithoutExtension(mediaItem.MediaUri);
                var extension = Path.GetExtension(mediaItem.MediaUri).Substring(1);
                var url = NSBundle.MainBundle.GetUrlForResource(filename, extension, directory);
                asset = AVAsset.FromUrl(url);
            }
            else if (mediaItem.MediaLocation == MediaLocation.FileSystem)
            {
                if (mediaItem.MediaUri.StartsWith("file:///"))
                {
                    asset = AVAsset.FromUrl(NSUrl.FromString(mediaItem.MediaUri));
                }
                else
                {
                    asset = AVAsset.FromUrl(NSUrl.FromFilename(mediaItem.MediaUri));
                }
            }
            else if (RequestHeaders != null && RequestHeaders.Any())
            {
                asset = AVUrlAsset.Create(NSUrl.FromString(mediaItem.MediaUri), GetOptionsWithHeaders(RequestHeaders));
            }
            else
            {
                asset = AVAsset.FromUrl(NSUrl.FromString(mediaItem.MediaUri));
            }

            var playerItem = AVPlayerItem.FromAsset(asset);

            return playerItem;
        }

        private static AVUrlAssetOptions GetOptionsWithHeaders(IDictionary<string, string> headers)
        {
            var nativeHeaders = new NSMutableDictionary();

            foreach (var header in headers)
            {
                nativeHeaders.Add((NSString)header.Key, (NSString)header.Value);
            }

            var nativeHeadersKey = (NSString)"AVURLAssetHTTPHeaderFieldsKey";

            var options = new AVUrlAssetOptions(NSDictionary.FromObjectAndKey(
                nativeHeaders,
                nativeHeadersKey
            ));

            return options;
        }
    }
}
