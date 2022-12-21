using System;
using System.IO;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BMM.Core.Models.POs.Tiles;
using BMM.UI.Droid.Application.Listeners;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.ViewHolders
{
    public class VideoTileViewHolder : MvxRecyclerViewHolder
    {
        private const string AndroidResourcePrefix = "android.resource://";
        private string _videoUrl;

        public VideoTileViewHolder(View itemView, IMvxAndroidBindingContext context) : base(itemView, context)
        {
            this.DelayBind(Bind);
        }

        protected VideoTileViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }
        
        private void Bind()
        {
            var set = this.CreateBindingSet<VideoTileViewHolder, VideoTilePO>();

            set.Bind(this)
                .For(v => v.VideoUrl)
                .To(po => po.Tile.VideoFileName);
            
            set.Apply();
        }

        public string VideoUrl
        {
            get => _videoUrl;
            set
            {
                _videoUrl = value;
                SetVideoView();
            }
        }

        private void SetVideoView()
        {
            if (string.IsNullOrEmpty(VideoUrl))
                return;
            
            var videoView = ItemView.FindViewById<VideoView>(Resource.Id.VideoView);
            string videoName = Path.GetFileNameWithoutExtension(VideoUrl);
            object resourceFieldInfo = typeof(Resource.Raw).GetField(videoName.ToLower()).GetRawConstantValue();
            videoView!.SetOnPreparedListener(new OnPreparedListeners(mp =>
            {
                mp.Looping = true;
            }));
            videoView.SetVideoURI(Android.Net.Uri.Parse($"{AndroidResourcePrefix}{ItemView.Context!.PackageName}/{resourceFieldInfo}"));
            videoView.Start();
        }
    }
}