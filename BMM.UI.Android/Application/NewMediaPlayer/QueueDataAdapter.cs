using System.Linq.Expressions;
using Android.Support.V4.Media;
using BMM.UI.Droid.Application.NewMediaPlayer.Controller;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;

namespace BMM.UI.Droid.Application.NewMediaPlayer
{
    /// <summary>
    /// The <see cref="QueueDataAdapter"/> allows to change the backing data of the queue. But since we do it directly in <see cref="AndroidMediaPlayer"/>
    /// we don't need to do anything here. It might make sense to change that for easier support of Chromecast.
    /// </summary>
    public class QueueDataAdapter : Java.Lang.Object, TimelineQueueEditor.IQueueDataAdapter
    {
        public void Add(int index, MediaDescriptionCompat description)
        {
            Expression.Empty();
        }

        public void Move(int from, int to)
        {
            Expression.Empty();
        }

        public void Remove(int position)
        {
            Expression.Empty();
        }
    }
}