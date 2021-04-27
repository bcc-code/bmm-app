using System;
using BMM.Api.Framework;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.NewMediaPlayer.Abstractions;
using MediaPlayer;
using MvvmCross;
using MvvmCross.Base;

namespace BMM.UI.iOS.NewMediaPlayer
{
    /// <summary>
    /// This implementation allows the usage of seeking in the command center.
    /// Unfortunately it does not work properly yet and needs additional work.
    /// </summary>
    public class SeekableRemoteCommandCenter : RemoteCommandCenter
    {
        public SeekableRemoteCommandCenter(IMvxMainThreadAsyncDispatcher dispatcher, IExceptionHandler exceptionHandler) : base(dispatcher, exceptionHandler)
        { }

        public override void Initialize()
        {
            base.Initialize();

            IMediaPlayer mediaPlayer = Mvx.IoCProvider.Resolve<IMediaPlayer>();

            MPRemoteCommandCenter.Shared.TogglePlayPauseCommand.Enabled = true;
            MPRemoteCommandCenter.Shared.TogglePlayPauseCommand.AddTarget(command => HandleRemoteCommand(() => mediaPlayer.PlayPause()));

            MPRemoteCommandCenter.Shared.PlayCommand.Enabled = true;
            MPRemoteCommandCenter.Shared.PlayCommand.AddTarget(command => HandleRemoteCommand(() => mediaPlayer.Play()));

            MPRemoteCommandCenter.Shared.PauseCommand.Enabled = true;
            MPRemoteCommandCenter.Shared.PauseCommand.AddTarget(command => HandleRemoteCommand(() => mediaPlayer.Pause()));

            MPRemoteCommandCenter.Shared.NextTrackCommand.Enabled = true;
            MPRemoteCommandCenter.Shared.NextTrackCommand.AddTarget(command => HandleRemoteCommand(() => mediaPlayer.PlayNext()));

            MPRemoteCommandCenter.Shared.PreviousTrackCommand.Enabled = true;
            MPRemoteCommandCenter.Shared.PreviousTrackCommand.AddTarget(command => HandleRemoteCommand(() => mediaPlayer.PlayPreviousOrSeekToStart()));

            MPRemoteCommandCenter.Shared.ChangeRepeatModeCommand.Enabled = true;
            MPRemoteCommandCenter.Shared.ChangeRepeatModeCommand.AddTarget(command => HandleRemoteCommand(() => mediaPlayer.ToggleRepeatType()));

            MPRemoteCommandCenter.Shared.ChangeShuffleModeCommand.Enabled = true;
            MPRemoteCommandCenter.Shared.ChangeShuffleModeCommand.AddTarget(command => HandleRemoteCommand(() => mediaPlayer.ToggleShuffle()));

            MPRemoteCommandCenter.Shared.SeekBackwardCommand.Enabled = true;
            MPRemoteCommandCenter.Shared.SeekBackwardCommand.AddTarget(command => HandleRemoteCommand(ActionHandledInMediaRemoteControl));

            MPRemoteCommandCenter.Shared.SeekForwardCommand.Enabled = true;
            MPRemoteCommandCenter.Shared.SeekForwardCommand.AddTarget(command => HandleRemoteCommand(ActionHandledInMediaRemoteControl));

            MPRemoteCommandCenter.Shared.ChangePlaybackPositionCommand.Enabled = true;
            MPRemoteCommandCenter.Shared.ChangePlaybackPositionCommand.AddTarget(commandEvent =>
            {
                return HandleRemoteCommand(() =>
                {
                    var command = (MPChangePlaybackPositionCommandEvent)commandEvent;

                    var newPositionInMs = command.PositionTime * 1000;
                    mediaPlayer.SeekTo((long)newPositionInMs);
                });
            });

            //// ToDo: #19205 we want to show skip commands for podcasts. That's an upcoming feature
            //MPRemoteCommandCenter.Shared.SkipBackwardCommand.Enabled = false;
            //MPRemoteCommandCenter.Shared.SkipForwardCommand.Enabled = false;
            //MPRemoteCommandCenter.Shared.SkipBackwardCommand.AddTarget(command =>
            //    HandleRemoteCommand(() =>
            //    {
            //        var skip = (MPSkipIntervalCommandEvent)command;
            //        _mediaPlayer.SkipBackward(skip.Interval);
            //    }));
            //MPRemoteCommandCenter.Shared.SkipForwardCommand.AddTarget(command =>
            //    HandleRemoteCommand(() =>
            //    {
            //        var skip = (MPSkipIntervalCommandEvent)command;
            //        _mediaPlayer.SkipForward(skip.Interval);
            //    }));
        }

        private MPRemoteCommandHandlerStatus HandleRemoteCommand(Action action)
        {
            IMediaPlayer mediaPlayer = Mvx.IoCProvider.Resolve<IMediaPlayer>();
            if (mediaPlayer.CurrentTrack == null)
            {
                return MPRemoteCommandHandlerStatus.NoActionableNowPlayingItem;
            }

            try
            {
                action();
            }
            catch (Exception e)
            {
                Mvx.IoCProvider.Resolve<ILogger>().Error("RemoteCommandCenter", "Error during remote command", e);
                return MPRemoteCommandHandlerStatus.CommandFailed;
            }
            return MPRemoteCommandHandlerStatus.Success;
        }

        /// <summary>
        /// In this class we add targets to the different commands. Since here is not possible to know when the user ends seeking,
        /// we implement the seeking commands in MediaRemoteControl where all commands from CommandCenter are received.
        /// </summary>
        private void ActionHandledInMediaRemoteControl()
        { }
    }
}