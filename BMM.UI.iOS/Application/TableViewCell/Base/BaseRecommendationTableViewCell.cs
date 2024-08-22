using System.Data;
using BMM.Core.Extensions;
using BMM.Core.Models.POs.Recommendations;
using BMM.Core.Translation;
using BMM.UI.iOS.Constants;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Core;
using MvvmCross.Platforms.Ios.Binding;

namespace BMM.UI.iOS.TableViewCell.Base;

public abstract class BaseRecommendationTableViewCell : BaseBMMTableViewCell
{
    private bool _isDescriptionVisible;

    protected BaseRecommendationTableViewCell(ObjCRuntime.NativeHandle handle) : base(handle)
    {
        this.DelayBind(() =>
        {
            var set = this.CreateBindingSet<BaseRecommendationTableViewCell, RecommendationPO>();

            set.Bind(LabelTitle)
                .To(d => d.TextSource[Translations.ExploreNewestViewModel_Recommended]);

            set.Bind(ContentView)
                .For(v => v.BindTap())
                .To(po => po.ClickedCommand);

            set.Bind(ContainerRemoteTitle)
                .For(v => v.BindVisible())
                .To(po => po.IsDescriptionVisible);
            
            set.Bind(this)
                .For(v => v.IsDescriptionVisible)
                .To(po => po.IsDescriptionVisible);

            set.Bind(LabelRemoteTitle)
                .To(po => po.Recommendation.Title);
            
            set.Bind(LabelRemoteSubtitle)
                .To(po => po.Recommendation.Subtitle);
            
            Bind(set);
            set.Apply();
            SetThemes();
        });
    }

    public bool IsDescriptionVisible
    {
        get => _isDescriptionVisible;
        set
        {
            _isDescriptionVisible = value;
            ConstraintTitleToBottomView.Active = _isDescriptionVisible;
        }
    }

    private void SetThemes()
    {
        LabelRemoteTitle.ApplyTextTheme(AppTheme.Title1);
        LabelRemoteSubtitle.ApplyTextTheme(AppTheme.Subtitle3Label3);
    }

    protected override bool HasHighlightEffect => false;

    protected virtual void Bind(MvxFluentBindingDescriptionSet<BaseRecommendationTableViewCell, RecommendationPO> set)
    {
    }

    protected abstract UILabel LabelTitle { get; }
    protected abstract UILabel LabelRemoteTitle { get; }
    protected abstract UILabel LabelRemoteSubtitle { get; }
    protected abstract UIView ContainerRemoteTitle { get; }
    protected abstract NSLayoutConstraint ConstraintTitleToBottomView { get; }
}