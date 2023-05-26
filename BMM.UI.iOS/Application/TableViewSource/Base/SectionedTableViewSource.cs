using System;
using System.Collections.Generic;
using System.Linq;
using BMM.Core.Models;
using BMM.UI.iOS.Constants;
using Foundation;
using UIKit;

namespace BMM.UI.iOS
{
    public abstract class SectionedTableViewSource : VariableRowHeightTableViewSource
    {
        protected SectionedTableViewSource(UITableView tableView) : base(tableView)
        { }

        private IEnumerable<ListSection<IListContentItem>> _sections;

        public IEnumerable<ListSection<IListContentItem>> Sections
        {
            get { return _sections; }
            set
            {
                _sections = value;
                ItemsSource = Sections.SelectMany(section => section.Items);
                TableView.Source = this;
            }
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return Sections.Count();
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return Sections.ToList()[(int)section].Items.Count();
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return RenderHeaderForSection(section) ? 48 : 0;
        }

        protected override object GetItemAt(NSIndexPath indexPath)
        {
            var section = indexPath.Section;
            var row = indexPath.Row;

            return Sections.ElementAt(section).Items.ElementAt(row);
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            var listSection = Sections.ToList()[(int)section];
            return GetSectionHeaderView(listSection);
        }

        private bool RenderHeaderForSection(nint section)
        {
            var listSection = Sections.ToList()[(int)section];
            return RenderHeaderForSection(listSection);
        }

        private bool RenderHeaderForSection(IListItem section)
        {
            return !string.IsNullOrEmpty(section.Title);
        }

        protected virtual UIView GetSectionHeaderView(IListItem section)
        {
            if (!RenderHeaderForSection(section))
                return null;

            var title = section.Title;

            var view = new UIView
            {
                BackgroundColor = AppColors.BackgroundOneColor
            };

            var label = new UILabel
            {
                Opaque = false,
                Text = title,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            label.ApplyTextTheme(AppTheme.Subtitle3Label3);
            view.AddSubview(label);

            label.TopAnchor.ConstraintEqualTo(view.TopAnchor, 20).Active = true;
            label.LeadingAnchor.ConstraintEqualTo(view.LeadingAnchor, 16).Active = true;
            label.TrailingAnchor.ConstraintEqualTo(view.TrailingAnchor).Active = true;
            label.BottomAnchor.ConstraintEqualTo(view.BottomAnchor, 8).Active = true;

            return view;
        }
    }
}