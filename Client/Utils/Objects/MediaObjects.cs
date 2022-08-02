using Blaze.Modules;
using Blaze.Utils.Managers;
using Newtonsoft.Json;
using System;

namespace Blaze.Utils.Objects
{
    internal class MediaObjects
    {
		public enum Commands
		{
			SkipPrevious,
			SkipNext,
			Stop,
			Play,
			Pause
		}

		public class CurrentStatus : IEquatable<CurrentStatus>
		{
			public CurrentStatus()
			{
				Source = new Source
				{
					Name = ".\\KEYBOARD"
				};
				SourceState = "New";
				PlaybackState = "N/A";
				Title = "-";
				Artist = "-";
				Thumbnail = null;
			}

			public CurrentStatus(SourceChange _searchStrings)
			{
				SourceChange(_searchStrings);
			}

			public void SourceChange(SourceChange set_SecurityEnabled)
			{
				Source = set_SecurityEnabled.Source;
				PlaybackState = set_SecurityEnabled.PlaybackState;
			}

			public void SongChange(SongChange net_log_server_issuers_look_for_matching_certs)
			{
				Source = net_log_server_issuers_look_for_matching_certs.Source;
				Title = net_log_server_issuers_look_for_matching_certs.Title;
				Artist = net_log_server_issuers_look_for_matching_certs.Artist;
				Thumbnail = net_log_server_issuers_look_for_matching_certs.Thumbnail;
			}

			public void PlaybackChange(PlaybackChange SystemMaxDBCSCharSize)
			{
				Source = SystemMaxDBCSCharSize.Source;
				PlaybackState = SystemMaxDBCSCharSize.PlaybackState;
			}

			public static bool operator ==(CurrentStatus _stopOnFirstFailure, CurrentStatus GetByteCount)
			{
				return InvalidPrimitive(_stopOnFirstFailure, GetByteCount);
			}

			public static bool operator !=(CurrentStatus numberGroupSizes, CurrentStatus CalendarType)
			{
				return !InvalidPrimitive(numberGroupSizes, CalendarType);
			}

			public override bool Equals(object Explicit)
			{
				return Explicit != null && !(GetType() != Explicit.GetType()) && InvalidPrimitive(this, (CurrentStatus)Explicit);
			}

			public bool Equals(CurrentStatus get_AssemblyFormat)
			{
				return InvalidPrimitive(this, get_AssemblyFormat);
			}

			private static bool InvalidPrimitive(CurrentStatus DirectoryInfos, CurrentStatus UpgradeToWriterLock)
			{
				return DirectoryInfos.Source == UpgradeToWriterLock.Source;
			}

			public override int GetHashCode()
			{
				return Source.Name.GetHashCode();
			}

			public Source Source;
			public string SourceState;
			public string PlaybackState;
			public string Title;
			public string Artist;
			public byte[] Thumbnail;
		}

		public sealed class GlobalSystemMediaTransportControlsSessionPlaybackControls
		{
			public bool IsChannelDownEnabled { get; set; }
			public bool IsChannelUpEnabled { get; set; }
			public bool IsFastForwardEnabled { get; set; }
			public bool IsNextEnabled { get; set; }
			public bool IsPauseEnabled { get; set; }
			public bool IsPlayEnabled { get; set; }
			public bool IsPlayPauseToggleEnabled { get; set; }
			public bool IsPlaybackPositionEnabled { get; set; }
			public bool IsPlaybackRateEnabled { get; set; }
			public bool IsPreviousEnabled { get; set; }
			public bool IsRecordEnabled { get; set; }
			public bool IsRepeatEnabled { get; set; }
			public bool IsRewindEnabled { get; set; }
			public bool IsShuffleEnabled { get; set; }
			public bool IsStopEnabled { get; set; }
		}

		public class Item
		{
			public override string ToString()
			{
				return JsonConvert.SerializeObject(this);
			}
		}

		public class PlaybackChange : Item
		{
			public Source Source { get; set; }
			public string PlaybackState { get; set; }
		}

		public class PlaybackChangeRequest : Item
		{
			public PlaybackChangeRequest(string m_HostContext, Commands net_sockets_disconnectedConnect)
			{
				Source = m_HostContext;
				PlaybackState = net_sockets_disconnectedConnect;
			}

			public string Source;
			public Commands PlaybackState;
		}

		public class SongChange : Item
		{
			public Source Source { get; set; }
			public string Title { get; set; }
			public string Artist { get; set; }
			public byte[] Thumbnail { get; set; }
		}

		public class Source : Item, IEquatable<Source>
		{
			public static bool operator ==(Source bRecord, Source Ldc_I4_2)
			{
				return CodePageDLLKorean(bRecord, Ldc_I4_2);
			}

			public static bool operator !=(Source TraceSwitchInvalidLevel, Source PostconditionOnExceptionFailed_Cnd)
			{
				return !CodePageDLLKorean(TraceSwitchInvalidLevel, PostconditionOnExceptionFailed_Cnd);
			}

			public override bool Equals(object GenitiveMonthNames)
			{
				return GenitiveMonthNames != null && !(GetType() != GenitiveMonthNames.GetType()) && CodePageDLLKorean(this, (Source)GenitiveMonthNames);
			}

			public bool Equals(Source CanBeCanceled)
			{
				return CodePageDLLKorean(this, CanBeCanceled);
			}

			private static bool CodePageDLLKorean(Source Parallel_ForEach_PartitionerNotDynamic, Source reftype)
			{
				return Parallel_ForEach_PartitionerNotDynamic.Name == reftype.Name;
			}

			public override int GetHashCode()
			{
				return Name.GetHashCode();
			}

			public string Name;
			public GlobalSystemMediaTransportControlsSessionPlaybackControls Controls { get; set; }
		}

		public class SourceChange : Item
		{
			public Source Source { get; set; }
			public string SourceState { get; set; }
			public string PlaybackState { get; set; }
		}
	}
}
