using Airbnb.Lottie;
using BMM.Core.Constants;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.Utils;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;

namespace BMM.UI.iOS.TableViewCell.Base;

public abstract class BaseExternalRelationsPlayTableViewCell : BaseBMMTableViewCell
{
    private bool _isCurrentlyPlaying;
    private LOTAnimationView _animationView;
    private bool _hasListened;

    public BaseExternalRelationsPlayTableViewCell(IntPtr handle) : base(handle)
    {
        this.DelayBind(() =>
        {
            var set = this.CreateBindingSet<BaseExternalRelationsPlayTableViewCell, IBibleStudyExternalRelationPO>();
            Bind(set);
            set.Apply();
        });
    }

    protected virtual void Bind(MvxFluentBindingDescriptionSet<BaseExternalRelationsPlayTableViewCell, IBibleStudyExternalRelationPO> set)
    {
        set.Bind(LabelPlay)
            .To(po => po.Title);
                
        set.Bind(ButtonPlay)
            .For(v => v.BindTap())
            .To(po => po.ClickedCommand);

        set.Bind(this)
            .For(v => v.IsCurrentlyPlaying)
            .To(v => v.IsCurrentlyPlaying);
        
        set.Bind(this)
            .For(v => v.HasListened)
            .To(v => v.HasListened);
    }

    public bool HasListened
    {
        get => _hasListened;
        set
        {
            _hasListened = value;
            IconPlay.Image = UIImage.FromBundle(_hasListened
                ? ImageResourceNames.IconCheckmark.ToStandardIosImageName()
                : ImageResourceNames.IconPlay.ToStandardIosImageName());
        }
    }

    protected override bool HasHighlightEffect => false;
    protected abstract UILabel LabelPlay { get; }
    protected abstract UIView ButtonPlay { get; }
    protected abstract UIImageView IconPlay { get; }
    protected abstract UIView ViewAnimation { get; }
    
    public bool IsCurrentlyPlaying
    {
        get => _isCurrentlyPlaying;
        set
        {
            _isCurrentlyPlaying = value;

            if (_isCurrentlyPlaying)
                ShowPlayAnimation();
            else
                HidePlayAnimation();
        }
    }
    
    public override void AwakeFromNib()
    {
        base.AwakeFromNib();
        SetThemes();
    }
        
    protected virtual void SetThemes()
    {
        LabelPlay.ApplyTextTheme(AppTheme.Subtitle1Label1);
    }
    
    private void ShowPlayAnimation()
    {
        IconPlay.Hidden = true;
        _animationView = ThemeUtils.GetLottieAnimationFor(LottieAnimationsNames.PlayAnimationIconDark, LottieAnimationsNames.PlayAnimationIcon);
        _animationView.BackgroundColor = UIColor.Clear;
        _animationView.LoopAnimation = true;
        _animationView.TranslatesAutoresizingMaskIntoConstraints = false;
        
        ViewAnimation.AddSubview(_animationView);

        NSLayoutConstraint.ActivateConstraints(
            new[]
            {
                _animationView.LeadingAnchor.ConstraintEqualTo(ViewAnimation.LeadingAnchor),
                _animationView.TrailingAnchor.ConstraintEqualTo(ViewAnimation.TrailingAnchor),
                _animationView.TopAnchor.ConstraintEqualTo(ViewAnimation.TopAnchor),
                _animationView.BottomAnchor.ConstraintEqualTo(ViewAnimation.BottomAnchor)
            });
            
        _animationView.Play();
    }

    private void HidePlayAnimation()
    {
        IconPlay.Hidden = false;
        _animationView.Stop();
        _animationView.RemoveFromSuperview();;
    }
}