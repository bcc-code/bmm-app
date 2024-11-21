using System;
using System.Collections.Generic;
using BMM.Core.Models;
using BMM.Core.Models.POs.Other;
using BMM.Core.Models.POs.Transcriptions;
using Foundation;
using UIKit;

namespace BMM.UI.iOS
{
    public class TranscriptionsTableViewSource : BaseTableViewSource
    {
        public TranscriptionsTableViewSource(UITableView tableView)
            : base(tableView)
        {
            tableView.RegisterNibForCellReuse(UINib.FromName(ReadTranscriptionTableViewCell.Key, NSBundle.MainBundle), ReadTranscriptionTableViewCell.Key);
            tableView.RegisterNibForCellReuse(UINib.FromName(TranscriptionHeaderTableViewCell.Key, NSBundle.MainBundle), TranscriptionHeaderTableViewCell.Key);
        }

        protected override IEnumerable<ITableCellType> GetTableCellTypes()
        {
            return new[]
            {
                new TableCellType(typeof(ReadTranscriptionsPO), ReadTranscriptionTableViewCell.Key),
                new TableCellType(typeof(TranscriptionHeaderPO), TranscriptionHeaderTableViewCell.Key),
            };
        }
    }
}