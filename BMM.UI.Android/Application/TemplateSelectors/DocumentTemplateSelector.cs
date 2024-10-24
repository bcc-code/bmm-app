using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Albums;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Models.POs.Carousels;
using BMM.Core.Models.POs.Contributors;
using BMM.Core.Models.POs.InfoMessages;
using BMM.Core.Models.POs.ListeningStreaks;
using BMM.Core.Models.POs.Other;
using BMM.Core.Models.POs.Playlists;
using BMM.Core.Models.POs.Podcasts;
using BMM.Core.Models.POs.Recommendations;
using BMM.Core.Models.POs.Tiles;
using BMM.Core.Models.POs.TrackCollections;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.Models.POs.YearInReview;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;

namespace BMM.UI.Droid.Application.TemplateSelectors
{
    public static class ViewTypes
    {
        public const int Track = 0;
        public const int Contributor = 2;
        public const int TrackCollection = 3;
        public const int TrackList = 4;
        public const int NullObject = 5;
        public const int Unknown = 6;
        public const int PinnedItem = 7;
        public const int Header = 10;
        public const int ChapterHeader = 14;
        public const int DiscoverSectionHeader = 15;
        public const int Streak = 16;
        public const int SharedTrackCollectionHeader = 17;
        public const int PlaylistsCollection = 18;
        public const int InfoMessage = 19;
        public const int SimpleMargin = 20;
        public const int TilesCollection = 21;
        public const int YearInReviewTeaserCollapsed = 22;
        public const int YearInReviewTeaserExpanded = 23;
        public const int TopSongsCollectionHeader = 24;
        public const int HighlightedTextTrack = 25;
        public const int RecommendationTrack = 26;
        public const int RecommendationAlbumPlaylist = 27;
        public const int RecommendationContributor = 28;
        public const int ProjectBoxCollapsed = 29;
        public const int ProjectBoxExpanded = 30;
        public const int GibraltarProjectBoxExpanded = 31;
    }

    public class DocumentTemplateSelector : MvxTemplateSelector<DocumentPO>
    {
        public override int GetItemLayoutId(int fromViewType)
        {
            switch (fromViewType)
            {
                case ViewTypes.Track:
                    return Resource.Layout.listitem_track;

                case ViewTypes.Contributor:
                    return Resource.Layout.listitem_contributor;

                case ViewTypes.TrackCollection:
                    return Resource.Layout.listitem_trackcollection;

                case ViewTypes.PinnedItem:
                    return Resource.Layout.listitem_pinned_item;

                case ViewTypes.TrackList:
                    return Resource.Layout.listitem_featured_playlist;

                case ViewTypes.DiscoverSectionHeader:
                    return Resource.Layout.listitem_discover_section_header;

                case ViewTypes.Streak:
                    return Resource.Layout.listitem_streak;

                case ViewTypes.NullObject:
                    return Resource.Layout.listitem_isfullyloaded;

                case ViewTypes.ChapterHeader:
                    return Resource.Layout.listitem_chapter_header;

                case ViewTypes.PlaylistsCollection:
                    return Resource.Layout.listitem_covers_carousel_collection;

                case ViewTypes.InfoMessage:
                    return Resource.Layout.listitem_info_message;
                
                case ViewTypes.SimpleMargin:
                    return Resource.Layout.listitem_simple_margin;
                
                case ViewTypes.TilesCollection:
                    return Resource.Layout.listitem_tiles_collection;
                
                case ViewTypes.YearInReviewTeaserCollapsed:
                    return Resource.Layout.listitem_year_in_review_teaser_collapsed;
                
                case ViewTypes.YearInReviewTeaserExpanded:
                    return Resource.Layout.listitem_year_in_review_teaser_expanded;
                
                case ViewTypes.HighlightedTextTrack:
                    return Resource.Layout.listitem_highlighted_text_track;
                
                case ViewTypes.RecommendationTrack:
                    return Resource.Layout.listitem_recommendation_track;
                
                case ViewTypes.RecommendationAlbumPlaylist:
                    return Resource.Layout.listitem_recommendation_album;
                
                case ViewTypes.RecommendationContributor:
                    return Resource.Layout.listitem_recommendation_contributor;
                
                case ViewTypes.ProjectBoxCollapsed:
                    return Resource.Layout.listitem_project_box_collapsed;
                
                case ViewTypes.ProjectBoxExpanded:
                    return Resource.Layout.listitem_project_box_expanded;
                
                case ViewTypes.GibraltarProjectBoxExpanded:
                    return Resource.Layout.listitem_gibraltar_project_box;
                
                default:
                    return Resource.Layout.listitem_track;
            }
        }

        protected override int SelectItemViewType(DocumentPO forItemObject)
        {
            if (forItemObject == null)
                return ViewTypes.NullObject;

            switch (forItemObject)
            {
                case TrackPO:
                    return ViewTypes.Track;

                case ContributorPO:
                    return ViewTypes.Contributor;

                case TrackCollectionPO:
                    return ViewTypes.TrackCollection;

                case PinnedItemPO:
                    return ViewTypes.PinnedItem;

                case AlbumPO:
                case PlaylistPO:
                case PodcastPO:
                    return ViewTypes.TrackList;

                case ChapterHeaderPO:
                    return ViewTypes.ChapterHeader;

                case DiscoverSectionHeaderPO:
                    return ViewTypes.DiscoverSectionHeader;

                case ListeningStreakPO:
                    return ViewTypes.Streak;

                case CoverCarouselCollectionPO:
                    return ViewTypes.PlaylistsCollection;

                case InfoMessagePO:
                    return ViewTypes.InfoMessage;

                case SimpleMarginPO:
                    return ViewTypes.SimpleMargin;

                case TileCollectionPO:
                    return ViewTypes.TilesCollection;

                case YearInReviewTeaserPO yearInReviewTeaserPO:
                {
                    if (yearInReviewTeaserPO.IsExpanded)
                        return ViewTypes.YearInReviewTeaserExpanded;

                    return ViewTypes.YearInReviewTeaserCollapsed;
                }
                
                case HighlightedTextTrackPO:
                    return ViewTypes.HighlightedTextTrack;
                
                case RecommendationPO recommendationPO:
                {
                    if (recommendationPO.TrackPO != null)
                        return ViewTypes.RecommendationTrack;

                    if (recommendationPO.TrackListHolder != null)
                        return ViewTypes.RecommendationAlbumPlaylist;
                    
                    return ViewTypes.RecommendationContributor;
                }
                
                case ProjectBoxPO projectBoxPO:
                {
                    if (projectBoxPO.ProjectBox.DocumentType == DocumentType.GibraltarProjectBox)
                        return ViewTypes.GibraltarProjectBoxExpanded;
                    
                    return projectBoxPO.IsExpanded
                        ? ViewTypes.ProjectBoxExpanded
                        : ViewTypes.ProjectBoxCollapsed;
                }
                
                default:
                        return ViewTypes.Unknown;
            }
        }
    }
}

