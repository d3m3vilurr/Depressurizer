/*
This file is part of Depressurizer.
Copyright 2011, 2012, 2013 Steve Labbe.

Depressurizer is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Depressurizer is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Depressurizer.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using System.Xml;
using Rallion;

namespace Depressurizer
{
	internal class CDlgUpdateProfile : CancelableDlg
	{
		#region Fields

		private readonly bool _custom;

		private readonly string _customUrl;

		private readonly GameList _data;

		private readonly SortedSet<int> _ignore;

		private readonly bool _overwrite;

		private readonly long _steamId;

		private XmlDocument _doc;

		#endregion

		#region Constructors and Destructors

		public CDlgUpdateProfile(GameList data, long accountId, bool overwrite, SortedSet<int> ignore) : base(GlobalStrings.CDlgUpdateProfile_UpdatingGameList, true)
		{
			_custom = false;
			_steamId = accountId;

			_data = data;

			_overwrite = overwrite;
			_ignore = ignore;

			SetText(GlobalStrings.CDlgFetch_DownloadingGameList);
		}

		public CDlgUpdateProfile(GameList data, string customUrl, bool overwrite, SortedSet<int> ignore) : base(GlobalStrings.CDlgUpdateProfile_UpdatingGameList, true)
		{
			_custom = true;
			_customUrl = customUrl;

			_data = data;

			_overwrite = overwrite;
			_ignore = ignore;

			SetText(GlobalStrings.CDlgFetch_DownloadingGameList);
		}

		#endregion

		#region Public Properties

		public int Added { get; private set; } = 0;

		public int Fetched { get; private set; } = 0;

		public int Removed { get; } = 0;

		#endregion

		#region Methods

		protected override void Finish()
		{
			if (Canceled || (Error != null) || (_doc == null))
			{
				return;
			}

			SetText(GlobalStrings.CDlgFetch_FinishingDownload);
			Fetched = _data.IntegrateXmlGameList(_doc, _overwrite, _ignore, out int newItems);
			Added = newItems;

			OnJobCompletion();
		}

		protected override void RunProcess()
		{
			_doc = _custom ? GameList.FetchXmlGameList(_customUrl) : GameList.FetchXmlGameList(_steamId);

			OnThreadCompletion();
		}

		#endregion
	}
}
