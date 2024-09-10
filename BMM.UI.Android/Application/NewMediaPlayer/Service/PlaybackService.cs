using AndroidX.Media3.Common;
using AndroidX.Media3.Session;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Service;

public class PlaybackService : MediaLibraryService
{
    MediaLibrarySession.Builder _mediaLibrarySession;
    
    public override MediaSession OnGetSession(MediaSession.ControllerInfo p0)
    {
        throw new NotImplementedException();
    }

    public override void OnCreate()
    {
        base.OnCreate();
    }

    public void InitializeSessionAndPlayer()
    {
        // ToDo: we actually allow Music and Speeches within one playlist. Now it's always music.
        var audioAttributes = new AudioAttributes.Builder()
            .SetContentType(C.AudioContentTypeMusic)!
            .SetUsage(C.UsageMedia)!.Build();

        var player = new AndroidX.Media3.ExoPlayer.ExoPlayerBuilder(this)
            .SetAudioAttributes(audioAttributes, true)!
            .Build();
        //player.AddAnalyticsListener();

        
        _mediaLibrarySession = new MediaLibrarySession.Builder(this, player, new MediaLibrarySessionCallback());
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        //_mediaLibrarySession;
    }
}