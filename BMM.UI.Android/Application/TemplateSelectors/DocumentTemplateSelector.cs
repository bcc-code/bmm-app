using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Albums;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Carousels;
using BMM.Core.Models.POs.ContinueListening;
using BMM.Core.Models.POs.Contributors;
using BMM.Core.Models.POs.InfoMessages;
using BMM.Core.Models.POs.ListeningStreakPO;
using BMM.Core.Models.POs.Other;
using BMM.Core.Models.POs.Playlists;
using BMM.Core.Models.POs.Podcasts;
using BMM.Core.Models.POs.TrackCollections;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.Models.POs.YearInReview;
using BMM.Core.ViewModels;
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
        public const int ContinueListeningCollection = 21;
        public const int YearInReviewPreviewCollapsed = 22;
        public const int YearInReviewPreviewExpanded = 23;
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
                
                case ViewTypes.ContinueListeningCollection:
                    return Resource.Layout.listitem_continue_listening_collection;
                
                case ViewTypes.YearInReviewPreviewCollapsed:
                    return Resource.Layout.listitem_year_in_review_preview_collapsed;
                
                case ViewTypes.YearInReviewPreviewExpanded:
                    return Resource.Layout.listitem_year_in_review_preview_expanded;
                
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

                case ContinueListeningCollectionPO:
                    return ViewTypes.ContinueListeningCollection;

                case YearInReviewPreviewPO yearInReviewPreviewPO:
                {
                    if (yearInReviewPreviewPO.IsExpanded)
                        return ViewTypes.YearInReviewPreviewExpanded;

                    return ViewTypes.YearInReviewPreviewCollapsed;
                }

                default:
                        return ViewTypes.Unknown;
            }
        }
    }
}

