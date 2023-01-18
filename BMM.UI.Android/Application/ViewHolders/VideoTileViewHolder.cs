using System;
using Android.Graphics;
using Android.Media;
using Android.Runtime;
using Android.Views;
using BMM.Core.Models.POs.Tiles;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Path = System.IO.Path;

namespace BMM.UI.Droid.Application.ViewHolders
{
    public class VideoTileViewHolder : MvxRecyclerViewHolder, ISurfaceHolderCallback, MediaPlayer.IOnPreparedListener
    {
        private const string AndroidResourcePrefix = "android.resource://";
        private string _videoUrl;
        private MediaPlayer _videoPlayer;

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
            
            var videoSurfaceView = ItemView.FindViewById<SurfaceView>(Resource.Id.VideoSurfaceView);
            videoSurfaceView.Holder.AddCallback(this);
        }

        public void SurfaceChanged(ISurfaceHolder holder, Format format, int width, int height)
        {
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            string videoName = Path.GetFileNameWithoutExtension(VideoUrl);
            object resourceFieldInfo = typeof(Resource.Raw).GetField(videoName.ToLower()).GetRawConstantValue();
            _videoPlayer = MediaPlayer.Create(ItemView.Context, Android.Net.Uri.Parse($"{AndroidResourcePrefix}{ItemView.Context!.PackageName}/{resourceFieldInfo}"));
            _videoPlayer.SetDisplay(holder);
            _videoPlayer.Looping = true;
            _videoPlayer.SetOnPreparedListener(this);
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            if (_videoPlayer != null)
            {
                _videoPlayer.Reset();
                _videoPlayer.Release();
                _videoPlayer = null;
            }
        }

        public void OnPrepared(MediaPlayer mp)
        {
            _videoPlayer.Start();
        }
    }
}