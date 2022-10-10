using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace BMM.UI.iOS
{
    public class BrowseDetailsTableViewSource : NotSelectableDocumentsTableViewSource
    {
        private readonly Dictionary<int, nfloat> _offsetDictionary = new();
        private bool _preventSavingOffsets;

        public BrowseDetailsTableViewSource(UITableView tableView) : base(tableView)
        {
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            base.WillDisplay(tableView, cell, indexPath);

            _preventSavingOffsets = false;
            _offsetDictionary.TryGetValue(indexPath.Row, out var offset);

            if (cell is PlaylistsCollectionTableViewCell playlistsCollectionTableViewCell)
                playlistsCollectionTableViewCell.CollectionViewOffset = offset;
        }

        public override void CellDisplayingEnded(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            base.CellDisplayingEnded(tableView, cell, indexPath);

            if (cell is PlaylistsCollectionTableViewCell playlistsCollectionTableViewCell && !_preventSavingOffsets)
            {
                playlistsCollectionTableViewCell.CollectionViewOffset =
                _offsetDictionary[indexPath.Row] = playlistsCollectionTableViewCell.CollectionViewOffset;
            }
        }

        public void ClearOffsets()
        {
            _preventSavingOffsets = true;
            _offsetDictionary.Clear();
        }
    }
}