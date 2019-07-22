﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MediaManager.Media;
using Tizen.Multimedia;

namespace MediaManager.Platforms.Tizen
{
    public class MediaExtractor : IMediaExtractor
    {
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Tizen;

        public Task<IMediaItem> CreateMediaItem(string url)
        {
            IMediaItem mediaItem = new MediaItem(url);

            var extractor = new MetadataExtractor(url);
            SetMetadata(mediaItem, extractor);
            return Task.FromResult(mediaItem);
        }

        public Task<IMediaItem> CreateMediaItem(FileInfo file)
        {
            return null;
        }

        public Task<IMediaItem> CreateMediaItem(IMediaItem mediaItem)
        {
            return null;
        }

        public Task<object> RetrieveMediaItemArt(IMediaItem mediaItem)
        {
            return null;
        }

        private void SetMetadata(IMediaItem mediaItem, MetadataExtractor extractor)
        {
            Metadata metadata = extractor.GetMetadata();
            mediaItem.Title = metadata.Title;
            mediaItem.Artist = metadata.Artist;
            mediaItem.Album = metadata.Album;
            mediaItem.AlbumArtist = metadata.AlbumArtist;
            mediaItem.Author = metadata.Author;
            mediaItem.Duration = TimeSpan.FromSeconds(metadata?.Duration ?? 0);
            mediaItem.Genre = metadata.Genre;
            if (int.TryParse(metadata.TrackNumber, out var year))
            {
                mediaItem.TrackNumber = year;
                mediaItem.NumTracks = year;
            }

            var buffer = mediaItem.MediaType == MediaType.Video ? extractor.GetVideoThumbnail() : extractor.GetArtwork().Data;
            if (buffer.Length > 0)
            {
                Stream st = new MemoryStream(buffer);
                mediaItem.AlbumArt = st;
            }
        }

        private void SetMetadata(IMediaItem mediaItem, StreamInfo streamInfo)
        {
            mediaItem.Title = streamInfo.GetMetadata(StreamMetadataKey.Title);
            mediaItem.Artist = streamInfo.GetMetadata(StreamMetadataKey.Artist);
            mediaItem.AlbumArtist = streamInfo.GetMetadata(StreamMetadataKey.Album);
            mediaItem.Author = streamInfo.GetMetadata(StreamMetadataKey.Author);
            mediaItem.Duration = TimeSpan.FromSeconds(streamInfo.GetDuration());
            mediaItem.Genre = streamInfo.GetMetadata(StreamMetadataKey.Genre);
            if (long.TryParse(streamInfo.GetMetadata(StreamMetadataKey.Year), out var year))
            {
                mediaItem.Year = Convert.ToInt32(year);
            }
            mediaItem.AlbumArt = streamInfo.GetAlbumArt();
        }
    }
}
