using Android.Runtime;
using Android.Views;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.UI.Droid.Application.Extensions;
using FFImageLoading.Cross;
using Microsoft.IdentityModel.Tokens;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.ViewHolders;

public class AchievementViewHolder : MvxRecyclerViewHolder
{
    public const string StandardModeIcon = "res:icon_achievement_locked";
    public const string NightModeIcon = "res:icon_achievement_locked_night";
    
    private MvxCachedImageView _imageView;
    private string _imagePath;

    public AchievementViewHolder(View itemView, IMvxAndroidBindingContext context) : base(itemView, context)
    {
        this.DelayBind(Bind);
    }

    protected AchievementViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
    {
    }
    
    private void Bind()
    {
        _imageView = ItemView.FindViewById<MvxCachedImageView>(Resource.Id.ImageView);

        var set = this.CreateBindingSet<AchievementViewHolder, IAchievementPO>();

        set.Bind(this)
            .For(v => v.ImagePath)
            .To(po => po.ImagePath);

        set.Apply();
    }

    public string ImagePath
    {
        get => _imagePath;
        set
        {
            _imagePath = value;
            
            if (_imagePath.IsNullOrEmpty())
            {
                _imageView.ImagePath  = ItemView.Context.IsNightMode()
                    ? NightModeIcon
                    : StandardModeIcon;
            }
        }
    }
}