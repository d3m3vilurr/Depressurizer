﻿#region License

//     This file (GameList.cs) is part of Depressurizer.
//     Copyright (C) 2011  Steve Labbe
//     Copyright (C) 2018  Martijn Vegter
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <https://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Xml;
using Depressurizer.Core;
using Depressurizer.Core.Enums;
using Depressurizer.Core.Helpers;
using Depressurizer.Core.Models;
using Depressurizer.Models;
using Depressurizer.Properties;
using ValueType = Depressurizer.Core.Enums.ValueType;

namespace Depressurizer
{
	public class GameList
	{
		#region Constants

		public const string FavoriteConfigValue = "favorite";

		public const string FavoriteNewConfigValue = "<Favorite>";

		#endregion

		#region Constructors and Destructors

		public GameList()
		{
			Categories.Add(FavoriteCategory);
		}

		#endregion

		#region Public Properties

		public List<Category> Categories { get; } = new List<Category>();

		public Category FavoriteCategory { get; } = new Category(FavoriteNewConfigValue);

		public List<Filter> Filters { get; } = new List<Filter>();

		public Dictionary<int, GameInfo> Games { get; } = new Dictionary<int, GameInfo>();

		#endregion

		#region Properties

		private static Database Database => Database.Instance;

		private static Logger Logger => Logger.Instance;

		#endregion

		#region Public Methods and Operators

		public static XmlDocument FetchXmlGameList(string customUrl)
		{
			return FetchXmlFromUrl(string.Format(Constants.SteamCustomProfileGameListXML, customUrl));
		}

		public static XmlDocument FetchXmlGameList(long steamId)
		{
			return FetchXmlFromUrl(string.Format(Constants.SteamProfileGameListXML, steamId));
		}

		/// <summary>
		///     Adds a new category to the list.
		/// </summary>
		/// <param name="name">Name of the category to add</param>
		/// <returns>The added category. Returns null if the category already exists.</returns>
		public Category AddCategory(string name)
		{
			if (string.IsNullOrEmpty(name) || CategoryExists(name))
			{
				return null;
			}

			Category newCat = new Category(name);
			Categories.Add(newCat);

			return newCat;
		}

		/// <summary>
		///     Adds a new Filter to the list.
		/// </summary>
		/// <param name="name">Name of the Filter to add</param>
		/// <returns>The added Filter. Returns null if the Filter already exists.</returns>
		public Filter AddFilter(string name)
		{
			if (string.IsNullOrEmpty(name) || FilterExists(name))
			{
				return null;
			}

			Filter newFilter = new Filter(name);
			Filters.Add(newFilter);

			return newFilter;
		}

		public void AddGameCategory(int appId, Category category)
		{
			GameInfo gameInfo = Games[appId];
			gameInfo.AddCategory(category);
		}

		public void AddGameCategory(int[] appIds, Category category)
		{
			foreach (int appId in appIds)
			{
				AddGameCategory(appId, category);
			}
		}

		/// <summary>
		///     Checks to see if a category with the given name exists
		/// </summary>
		/// <param name="name">Name of the category to look for</param>
		/// <returns>True if the name is found, false otherwise</returns>
		public bool CategoryExists(string name)
		{
			// Favorite category always exists
			if ((name == FavoriteNewConfigValue) || (name == FavoriteConfigValue))
			{
				return true;
			}

			foreach (Category c in Categories)
			{
				if (string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}

			return false;
		}

		public void ClearGameCategories(int appId, bool preserveFavorite)
		{
			Games[appId].ClearCategories(!preserveFavorite);
		}

		public void ClearGameCategories(int[] appIds, bool preserveFavorite)
		{
			foreach (int appId in appIds)
			{
				ClearGameCategories(appId, preserveFavorite);
			}
		}

		/// <summary>
		///     Writes Steam game category information to Steam user config file.
		/// </summary>
		/// <param name="steamId">Steam ID of user to save the config file for</param>
		/// <param name="discardMissing">
		///     If true, any pre-existing game entries in the file that do not have corresponding entries
		///     in the GameList are removed
		/// </param>
		/// <param name="includeShortcuts">If true, also saves the Steam shortcut category data</param>
		public void ExportSteamConfig(long steamId, bool discardMissing, bool includeShortcuts)
		{
			string filePath = string.Format(Constants.ConfigFilePath, Settings.Instance.SteamPath, Profile.ID64toDirName(steamId));
			ExportSteamConfigFile(filePath, discardMissing);
			if (includeShortcuts)
			{
				ExportSteamShortcuts(steamId);
			}
		}

		public void ExportSteamConfigFile(string filePath, bool discardMissing)
		{
			Logger.Instance.Info(GlobalStrings.GameData_SavingSteamConfigFile, filePath);

			VDFNode fileData = new VDFNode();
			try
			{
				using (StreamReader reader = new StreamReader(filePath, false))
				{
					fileData = VDFNode.LoadFromText(reader, true);
				}
			}
			catch (Exception e)
			{
				Logger.Instance.Warn(GlobalStrings.GameData_LoadingErrorSteamConfig, e.Message);
			}

			VDFNode appListNode = fileData.GetNodeAt(new[]
			{
				"Software",
				"Valve",
				"Steam",
				"apps"
			}, true);

			// Run through all Delete category data for any games not found in the GameList
			if (discardMissing)
			{
				Dictionary<string, VDFNode> gameNodeArray = appListNode.NodeArray;
				if (gameNodeArray != null)
				{
					foreach (KeyValuePair<string, VDFNode> pair in gameNodeArray)
					{
						if (!(int.TryParse(pair.Key, out int gameId) && Games.ContainsKey(gameId)))
						{
							Logger.Instance.Verbose(GlobalStrings.GameData_RemovingGameCategoryFromSteamConfig, gameId);
							pair.Value.RemoveSubNode("tags");
						}
					}
				}
			}

			// Force appListNode to be an array, we can't do anything if it's a value
			appListNode.MakeArray();

			foreach (GameInfo game in Games.Values)
			{
				if (game.Id > 0)
				{
					// External games have negative identifier
					Logger.Instance.Verbose(GlobalStrings.GameData_AddingGameToConfigFile, game.Id);
					VDFNode gameNode = appListNode[game.Id.ToString()];
					gameNode.MakeArray();

					VDFNode tagsNode = gameNode["tags"];
					tagsNode.MakeArray();

					Dictionary<string, VDFNode> tags = tagsNode.NodeArray;
					if (tags != null)
					{
						tags.Clear();
					}

					int key = 0;
					foreach (Category c in game.Categories)
					{
						string name = c.Name;
						if (name == FavoriteNewConfigValue)
						{
							name = FavoriteConfigValue;
						}

						tagsNode[key.ToString()] = new VDFNode(name);
						key++;
					}

					if (game.Hidden)
					{
						gameNode["hidden"] = new VDFNode("1");
					}
					else
					{
						gameNode.RemoveSubNode("hidden");
					}
				}
			}

			Logger.Instance.Verbose(GlobalStrings.GameData_CleaningUpSteamConfigTree);
			appListNode.CleanTree();

			Logger.Instance.Info(GlobalStrings.GameData_WritingToDisk);
			VDFNode fullFile = new VDFNode();
			fullFile["UserLocalConfigStore"] = fileData;
			try
			{
				Utility.BackupFile(filePath, Settings.Instance.ConfigBackupCount);
			}
			catch (Exception e)
			{
				Logger.Instance.Error(GlobalStrings.Log_GameData_ConfigBackupFailed, e.Message);
			}

			try
			{
				string filePathTmp = filePath + ".tmp";
				FileInfo f = new FileInfo(filePathTmp);
				f.Directory.Create();
				FileStream fStream = f.Open(FileMode.Create, FileAccess.Write, FileShare.None);
				using (StreamWriter writer = new StreamWriter(fStream))
				{
					fullFile.SaveAsText(writer);
				}

				fStream.Close();
				File.Delete(filePath);
				File.Move(filePathTmp, filePath);
			}
			catch (ArgumentException e)
			{
				Logger.Instance.Error(GlobalStrings.GameData_ErrorSavingSteamConfigFile, e.ToString());

				throw new ApplicationException(GlobalStrings.GameData_FailedToSaveSteamConfigBadPath, e);
			}
			catch (IOException e)
			{
				Logger.Instance.Error(GlobalStrings.GameData_ErrorSavingSteamConfigFile, e.ToString());

				throw new ApplicationException(GlobalStrings.GameData_FailedToSaveSteamConfigFile + e.Message, e);
			}
			catch (UnauthorizedAccessException e)
			{
				Logger.Instance.Error(GlobalStrings.GameData_ErrorSavingSteamConfigFile, e.ToString());

				throw new ApplicationException(GlobalStrings.GameData_AccessDeniedSteamConfigFile + e.Message, e);
			}
		}

		public void ExportSteamShortcuts(long steamId)
		{
			string filePath = string.Format(Constants.ShortCutsFilePath, Settings.Instance.SteamPath, Profile.ID64toDirName(steamId));
			Logger.Instance.Info("GameList: Saving Steam shortcuts file to '{0}'.", filePath);

			if (File.Exists(filePath))
			{
				Logger.Warn("GameList: Could not find shortcuts.vdf at '{0}'.", filePath);

				return;
			}

			VDFNode dataRoot = null;
			try
			{
				using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					dataRoot = VDFNode.LoadFromBinary(binaryReader);
				}
			}
			catch (FileNotFoundException e)
			{
				Logger.Instance.Error(GlobalStrings.GameData_ErrorOpeningConfigFileParam, e.ToString());
			}
			catch (IOException e)
			{
				Logger.Instance.Error(GlobalStrings.GameData_LoadingErrorSteamConfig, e.ToString());
			}

			if (dataRoot == null)
			{
				return;
			}

			List<GameInfo> gamesToSave = new List<GameInfo>();
			foreach (int id in Games.Keys)
			{
				if (id < 0)
				{
					gamesToSave.Add(Games[id]);
				}
			}

			LoadShortcutLaunchIds(steamId, out StringDictionary launchIds);

			VDFNode appsNode = dataRoot.GetNodeAt(new[]
			{
				"shortcuts"
			}, false);

			foreach (KeyValuePair<string, VDFNode> shortcutPair in appsNode.NodeArray)
			{
				VDFNode nodeGame = shortcutPair.Value;
				int.TryParse(shortcutPair.Key, out int nodeId);

				int matchingIndex = FindMatchingShortcut(nodeId, nodeGame, gamesToSave, launchIds);

				if (matchingIndex < 0)
				{
					continue;
				}

				GameInfo game = gamesToSave[matchingIndex];
				gamesToSave.RemoveAt(matchingIndex);

				Logger.Instance.Verbose(GlobalStrings.GameData_AddingGameToConfigFile, game.Id);

				VDFNode tagsNode = nodeGame.GetNodeAt(new[]
				{
					"tags"
				}, true);

				Dictionary<string, VDFNode> tags = tagsNode.NodeArray;
				tags?.Clear();

				int index = 0;
				foreach (Category c in game.Categories)
				{
					string name = c.Name;
					if (name == FavoriteNewConfigValue)
					{
						name = FavoriteConfigValue;
					}

					tagsNode[index.ToString()] = new VDFNode(name);
					index++;
				}

				nodeGame["hidden"] = new VDFNode(game.Hidden ? 1 : 0);
			}

			if (dataRoot.NodeType != ValueType.Array)
			{
				return;
			}

			{
				Logger.Instance.Info(GlobalStrings.GameData_SavingShortcutConfigFile, filePath);
				try
				{
					Utility.BackupFile(filePath, Settings.Instance.ConfigBackupCount);
				}
				catch (Exception e)
				{
					Logger.Instance.Error(GlobalStrings.Log_GameData_ShortcutBackupFailed, e.Message);
				}

				string filePathTmp = filePath + ".tmp";
				try
				{
					using (FileStream fileStream = new FileStream(filePathTmp, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
					using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
					{
						dataRoot.SaveAsBinary(binaryWriter);
					}

					File.Delete(filePath);
					File.Move(filePathTmp, filePath);
				}
				catch (ArgumentException e)
				{
					Logger.Instance.Error(GlobalStrings.GameData_ErrorSavingSteamConfigFile, e.ToString());

					throw new ApplicationException(GlobalStrings.GameData_FailedToSaveSteamConfigBadPath, e);
				}
				catch (IOException e)
				{
					Logger.Instance.Error(GlobalStrings.GameData_ErrorSavingSteamConfigFile, e.ToString());

					throw new ApplicationException(GlobalStrings.GameData_FailedToSaveSteamConfigFile + e.Message, e);
				}
				catch (UnauthorizedAccessException e)
				{
					Logger.Instance.Error(GlobalStrings.GameData_ErrorSavingSteamConfigFile, e.ToString());

					throw new ApplicationException(GlobalStrings.GameData_AccessDeniedSteamConfigFile + e.Message, e);
				}
			}
		}

		/// <summary>
		///     Checks to see if a Filter with the given name exists
		/// </summary>
		/// <param name="name">Name of the Filter to look for</param>
		/// <returns>True if the name is found, false otherwise</returns>
		public bool FilterExists(string name)
		{
			foreach (Filter f in Filters)
			{
				if (string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		///     Gets the category with the given name. If the category does not exist, creates it.
		/// </summary>
		/// <param name="name">Name to get the category for</param>
		/// <returns>A category with the given name. Null if any error is encountered.</returns>
		public Category GetCategory(string name)
		{
			// Categories must have a name
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}

			// Check for Favorite category
			if ((name == FavoriteNewConfigValue) || (name == FavoriteConfigValue))
			{
				return FavoriteCategory;
			}

			// Look for a matching category in the list and return if found
			foreach (Category c in Categories)
			{
				if (string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase))
				{
					return c;
				}
			}

			// Create a new category and return it
			return AddCategory(name);

			//Category newCat = new Category( name );
			//Categories.Add( newCat );
			//return newCat;
		}

		public Filter GetFilter(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				return null;
			}

			foreach (Filter filter in Filters)
			{
				if (filter.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					return filter;
				}
			}

			Filter newFilter = new Filter(name);
			Filters.Add(newFilter);

			return newFilter;
		}

		public void HideGames(int appId, bool hide)
		{
			Games[appId].SetHidden(hide);
		}

		public void HideGames(int[] appIds, bool hide)
		{
			foreach (int appId in appIds)
			{
				HideGames(appId, hide);
			}
		}

		/// <summary>
		///     Loads category info from the steam config file for the given Steam user.
		/// </summary>
		/// <param name="SteamId">Identifier of Steam user</param>
		/// <param name="ignore">Set of games to ignore</param>
		/// <param name="forceInclude">If true, include games that do not match the included types</param>
		/// <param name="includeShortcuts">If true, also import shortcut data</param>
		/// <returns>The number of game entries found</returns>
		public int ImportSteamConfig(long SteamId, SortedSet<int> ignore, bool includeShortcuts)
		{
			string filePath = string.Format(Constants.ConfigFilePath, Settings.Instance.SteamPath, Profile.ID64toDirName(SteamId));
			int result = ImportSteamConfigFile(filePath, ignore);
			if (includeShortcuts)
			{
				result += ImportSteamShortcuts(SteamId);
			}

			return result;
		}

		/// <summary>
		///     Loads category info from the given steam config file.
		/// </summary>
		/// <param name="filePath">The path of the file to open</param>
		/// <param name="ignore">Set of game IDs to ignore</param>
		/// <param name="forceInclude">If true, include games even if they are not of an included type</param>
		/// <returns>The number of game entries found</returns>
		public int ImportSteamConfigFile(string filePath, SortedSet<int> ignore)
		{
			Logger.Instance.Info(GlobalStrings.GameData_OpeningSteamConfigFile, filePath);
			VDFNode dataRoot;

			try
			{
				using (StreamReader reader = new StreamReader(filePath, false))
				{
					dataRoot = VDFNode.LoadFromText(reader, true);
				}
			}
			catch (InvalidDataException e)
			{
				Logger.Instance.Error(GlobalStrings.GameData_ErrorParsingConfigFileParam, e.Message);

				throw new ApplicationException(GlobalStrings.GameData_ErrorParsingSteamConfigFile + e.Message, e);
			}
			catch (IOException e)
			{
				Logger.Instance.Error(GlobalStrings.GameData_ErrorOpeningConfigFileParam, e.Message);

				throw new ApplicationException(GlobalStrings.GameData_ErrorOpeningSteamConfigFile + e.Message, e);
			}

			VDFNode appsNode = dataRoot.GetNodeAt(new[]
			{
				"Software",
				"Valve",
				"Steam",
				"apps"
			}, true);

			int count = IntegrateGamesFromVdf(appsNode, ignore);
			Logger.Instance.Info(GlobalStrings.GameData_SteamConfigFileLoaded, count);

			return count;
		}

		public int ImportSteamShortcuts(long SteamId)
		{
			if (SteamId <= 0)
			{
				return 0;
			}

			string filePath = string.Format(Constants.ShortCutsFilePath, Settings.Instance.SteamPath, Profile.ID64toDirName(SteamId));

			if (!File.Exists(filePath))
			{
				return 0;
			}

			int loadedGames = 0;

			FileStream fStream = null;
			BinaryReader binReader = null;

			try
			{
				fStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				binReader = new BinaryReader(fStream);

				VDFNode dataRoot = VDFNode.LoadFromBinary(binReader);

				VDFNode shortcutsNode = dataRoot.GetNodeAt(new[]
				{
					"shortcuts"
				}, false);

				if (shortcutsNode != null)
				{
					// Remove existing shortcuts
					List<int> oldShortcutIds = new List<int>();
					foreach (int id in Games.Keys)
					{
						if (id < 0)
						{
							oldShortcutIds.Add(id);
						}
					}

					foreach (int g in oldShortcutIds)
					{
						Games.Remove(g);
					}

					// Load launch IDs
					LoadShortcutLaunchIds(SteamId, out StringDictionary launchIds);

					// Load shortcuts
					foreach (KeyValuePair<string, VDFNode> shortcutPair in shortcutsNode.NodeArray)
					{
						VDFNode nodeGame = shortcutPair.Value;

						if (!int.TryParse(shortcutPair.Key, out int gameId))
						{
							continue;
						}

						if (IntegrateShortcut(gameId, nodeGame, launchIds))
						{
							loadedGames++;
						}
					}
				}
			}
			catch (FileNotFoundException e)
			{
				Logger.Instance.Error(GlobalStrings.GameData_ErrorOpeningConfigFileParam, e.ToString());
			}
			catch (IOException e)
			{
				Logger.Instance.Error(GlobalStrings.GameData_LoadingErrorSteamConfig, e.ToString());
			}
			catch (InvalidDataException e)
			{
				Logger.Instance.Error(e.ToString());
			}
			finally
			{
				if (binReader != null)
				{
					binReader.Close();
				}

				if (fStream != null)
				{
					fStream.Close();
				}
			}

			Logger.Instance.Info(GlobalStrings.GameData_IntegratedShortCuts, loadedGames);

			return loadedGames;
		}

		/// <summary>
		///     Integrates list of games from an XmlDocument into the loaded game list.
		/// </summary>
		/// <param name="doc">The XmlDocument containing the new game list</param>
		/// <param name="overWrite">If true, overwrite the names of games already in the list.</param>
		/// <param name="ignore">A set of item IDs to ignore.</param>
		/// <param name="ignoreDlc">Ignore any items classified as DLC in the database.</param>
		/// <param name="newItems">The number of new items actually added</param>
		/// <returns>Returns the number of games successfully processed and not ignored.</returns>
		public int IntegrateXmlGameList(XmlDocument doc, bool overWrite, SortedSet<int> ignore, out int newItems)
		{
			newItems = 0;
			int loadedGames = 0;

			if (doc == null)
			{
				return loadedGames;
			}

			XmlNodeList gameNodes = doc.SelectNodes("/gamesList/games/game");
			if (gameNodes != null)
			{
				foreach (XmlNode gameNode in gameNodes)
				{
					XmlNode appIdNode = gameNode["appID"];
					if ((appIdNode == null) || !int.TryParse(appIdNode.InnerText, out int appId))
					{
						continue;
					}

					XmlNode nameNode = gameNode["name"];
					if (nameNode == null)
					{
						continue;
					}

					GameInfo integratedGame = IntegrateGame(appId, nameNode.InnerText, overWrite, ignore, GameListingSource.WebProfile, out bool isNew);
					if (integratedGame == null)
					{
						continue;
					}

					loadedGames++;
					if (isNew)
					{
						newItems++;
					}
				}
			}

			Logger.Info("GameList: Integrated XML data into game list. {0} total items, {1} new.", loadedGames, newItems);

			return loadedGames;
		}

		/// <summary>
		///     Removes the given category.
		/// </summary>
		/// <param name="c">Category to remove.</param>
		/// <returns>True if removal was successful, false if it was not in the list anyway</returns>
		public bool RemoveCategory(Category c)
		{
			// Can't remove favorite category
			if (c == FavoriteCategory)
			{
				return false;
			}

			if (Categories.Remove(c))
			{
				foreach (GameInfo g in Games.Values)
				{
					g.RemoveCategory(c);
				}

				return true;
			}

			return false;
		}

		/// <summary>
		///     Remove all empty categories from the category list.
		/// </summary>
		/// <returns>Number of categories removed</returns>
		public int RemoveEmptyCategories()
		{
			Dictionary<Category, int> counts = new Dictionary<Category, int>();
			foreach (Category c in Categories)
			{
				if (c != FavoriteCategory)
				{
					counts.Add(c, 0);
				}
			}

			foreach (GameInfo g in Games.Values)
			{
				foreach (Category c in g.Categories)
				{
					if (counts.ContainsKey(c))
					{
						counts[c]++;
					}
				}
			}

			int removed = 0;
			foreach (KeyValuePair<Category, int> pair in counts)
			{
				if (pair.Value == 0)
				{
					if (Categories.Remove(pair.Key))
					{
						removed++;
					}
				}
			}

			return removed;
		}

		public void RemoveGameCategory(int appId, Category category)
		{
			GameInfo gameInfo = Games[appId];
			gameInfo.RemoveCategory(category);
		}

		public void RemoveGameCategory(int[] appIds, Category category)
		{
			foreach (int appId in appIds)
			{
				RemoveGameCategory(appId, category);
			}
		}

		public void RemoveGameCategory(int appId, ICollection<Category> categories)
		{
			GameInfo gameInfo = Games[appId];
			gameInfo.RemoveCategory(categories);
		}

		/// <summary>
		///     Renames the given category.
		/// </summary>
		/// <param name="c">Category to rename.</param>
		/// <param name="newName">Name to assign to the new category.</param>
		/// <returns>The new category, if the operation succeeds. Null otherwise.</returns>
		public Category RenameCategory(Category c, string newName)
		{
			if (c == FavoriteCategory)
			{
				return null;
			}

			Category newCat = AddCategory(newName);
			if (newCat != null)
			{
				Categories.Sort();
				foreach (GameInfo game in Games.Values)
				{
					if (game.ContainsCategory(c))
					{
						game.RemoveCategory(c);
						game.AddCategory(newCat);
					}
				}

				RemoveCategory(c);

				return newCat;
			}

			return null;
		}

		public void SetGameCategories(int[] gameIDs, Category cat, bool preserveFavorites)
		{
			SetGameCategories(gameIDs, new List<Category>
			{
				cat
			}, preserveFavorites);
		}

		/// <summary>
		///     Sets a game's categories to a particular set
		/// </summary>
		/// <param name="gameID">Game ID to modify</param>
		/// <param name="catSet">Set of categories to apply</param>
		/// <param name="preserveFavorites">If true, will not remove "favorite" category</param>
		public void SetGameCategories(int gameID, ICollection<Category> catSet, bool preserveFavorites)
		{
			Games[gameID].SetCategories(catSet, preserveFavorites);
		}

		/// <summary>
		///     Sets multiple games' categories to a particular set
		/// </summary>
		/// <param name="gameID">Game IDs to modify</param>
		/// <param name="catSet">Set of categories to apply</param>
		/// <param name="preserveFavorites">If true, will not remove "favorite" category</param>
		public void SetGameCategories(int[] gameIDs, ICollection<Category> catSet, bool preserveFavorites)
		{
			for (int i = 0; i < gameIDs.Length; i++)
			{
				SetGameCategories(gameIDs[i], catSet, preserveFavorites);
			}
		}

		public int UpdateGameListFromOwnedPackageInfo(long accountId, SortedSet<int> ignored, out int newApps)
		{
			newApps = 0;
			int totalApps = 0;

			Dictionary<int, PackageInfo> allPackages = PackageInfo.LoadPackages(string.Format(Constants.PackageInfoPath, Settings.Instance.SteamPath));

			Dictionary<int, GameListingSource> ownedApps = new Dictionary<int, GameListingSource>();

			string localConfigPath = string.Format(Constants.LocalConfigPath, Settings.Instance.SteamPath, Profile.ID64toDirName(accountId));
			VDFNode vdfFile = VDFNode.LoadFromText(new StreamReader(localConfigPath));
			if (vdfFile != null)
			{
				VDFNode licensesNode = vdfFile.GetNodeAt(new[]
				{
					"UserLocalConfigStore",
					"Licenses"
				}, false);

				if ((licensesNode != null) && (licensesNode.NodeType == ValueType.Array))
				{
					foreach (string key in licensesNode.NodeArray.Keys)
					{
						if (int.TryParse(key, out int ownedPackageId))
						{
							PackageInfo ownedPackage = allPackages[ownedPackageId];
							if (ownedPackageId != 0)
							{
								GameListingSource src = (ownedPackage.BillingType == PackageBillingType.FreeOnDemand) || (ownedPackage.BillingType == PackageBillingType.AutoGrant) ? GameListingSource.PackageFree : GameListingSource.PackageNormal;
								foreach (int ownedAppId in ownedPackage.AppIds)
								{
									if (!ownedApps.ContainsKey(ownedAppId) || ((src == GameListingSource.PackageNormal) && (ownedApps[ownedAppId] == GameListingSource.PackageFree)))
									{
										ownedApps[ownedAppId] = src;
									}
								}
							}
						}
					}
				}

				// update LastPlayed
				VDFNode appsNode = vdfFile.GetNodeAt(new[]
				{
					"UserLocalConfigStore",
					"Software",
					"Valve",
					"Steam",
					"apps"
				}, false);

				GetLastPlayedFromVdf(appsNode, ignored);
			}

			foreach (KeyValuePair<int, GameListingSource> kv in ownedApps)
			{
				string name = Database.GetName(kv.Key);
				GameInfo newGame = IntegrateGame(kv.Key, name, false, ignored, kv.Value, out bool isNew);
				if (newGame != null)
				{
					totalApps++;
				}

				if (isNew)
				{
					newApps++;
				}
			}

			return totalApps;
		}

		#endregion

		#region Methods

		private static XmlDocument FetchXmlFromUrl(string url)
		{
			Logger.Info("GameList: Starting download XML game list from URL '{0}'.", url);
			XmlDocument document = new XmlDocument();

			try
			{
				using (WebClient client = new WebClient())
				{
					client.Headers.Set("User-Agent", "Depressurizer");

					string xml = client.DownloadString(url);
					if (xml.Contains("This profile is private."))
					{
						throw new InvalidDataException("The specified profile is not public, please check your privacy settings.");
					}

					if (!xml.Contains("<games>") || !xml.Contains("</games>"))
					{
						throw new InvalidDataException("The specified profile does not have a public game list, please check your privacy settings.");
					}

					document.LoadXml(xml);
				}
			}
			catch (Exception e)
			{
				Logger.Error("GameList: Exception while downloading XML game list: {0}", e.Message);

				throw new ApplicationException(e.Message, e);
			}

			Logger.Info("GameList: Downloaded XML game list from URL '{0}'.", url);

			return document;
		}

		private int FindMatchingShortcut(int shortcutId, VDFNode shortcutNode, List<GameInfo> gamesToMatchAgainst, StringDictionary shortcutLaunchIds)
		{
			VDFNode nodeName = shortcutNode.GetNodeAt(new[]
			{
				"appname"
			}, false);

			string gameName = nodeName != null ? nodeName.NodeString : null;
			string launchId = shortcutLaunchIds[gameName];

			// First, look for games with matching launch IDs.
			for (int i = 0; i < gamesToMatchAgainst.Count; i++)
			{
				if (gamesToMatchAgainst[i].LaunchString == launchId)
				{
					return i;
				}
			}

			// Second, look for games with matching names AND matching shortcut IDs.
			for (int i = 0; i < gamesToMatchAgainst.Count; i++)
			{
				if ((gamesToMatchAgainst[i].Id == -(shortcutId + 1)) && (gamesToMatchAgainst[i].Name == gameName))
				{
					return i;
				}
			}

			// Third, just look for name matches
			for (int i = 0; i < gamesToMatchAgainst.Count; i++)
			{
				if (gamesToMatchAgainst[i].Name == gameName)
				{
					return i;
				}
			}

			return -1;
		}

		private void GetLastPlayedFromVdf(VDFNode appsNode, SortedSet<int> ignore)
		{
			Dictionary<string, VDFNode> gameNodeArray = appsNode.NodeArray;
			if (gameNodeArray == null)
			{
				return;
			}

			foreach (KeyValuePair<string, VDFNode> gameNodePair in gameNodeArray)
			{
				if (!int.TryParse(gameNodePair.Key, out int gameId))
				{
					continue;
				}

				if (((ignore != null) && ignore.Contains(gameId)) || !Database.IncludeItemInGameList(gameId))
				{
					Logger.Instance.Verbose(GlobalStrings.GameData_SkippedProcessingGame, gameId);
				}
				else if ((gameNodePair.Value != null) && (gameNodePair.Value.NodeType == ValueType.Array))
				{
					GameInfo game;

					// Add the game to the list if it doesn't exist already
					if (!Games.ContainsKey(gameId))
					{
						game = new GameInfo(gameId, Database.GetName(gameId), this);
						Games.Add(gameId, game);
						Logger.Instance.Verbose(GlobalStrings.GameData_AddedNewGame, gameId, game.Name);
					}
					else
					{
						game = Games[gameId];
					}

					if (!gameNodePair.Value.ContainsKey("LastPlayed") || (gameNodePair.Value["LastPlayed"].NodeInt == 0))
					{
						continue;
					}

					game.LastPlayed = gameNodePair.Value["LastPlayed"].NodeInt;
					Logger.Verbose(GlobalStrings.GameData_ProcessedGame, gameId, DateTimeOffset.FromUnixTimeSeconds(game.LastPlayed).DateTime);
				}
			}
		}

		/// <summary>
		///     Adds a new game to the database, or updates an existing game with new information.
		/// </summary>
		/// <param name="appId">App ID to add or update</param>
		/// <param name="appName">Name of app to add, or update to</param>
		/// <param name="overwriteName">If true, will overwrite any existing games. If false, will fail if the game already exists.</param>
		/// <param name="ignore">Set of games to ignore. Can be null. If the game is in this list, no action will be taken.</param>
		/// <param name="forceInclude">If true, include the game even if it is of an ignored type.</param>
		/// <param name="src">The listing source that this request came from.</param>
		/// <param name="isNew">If true, a new game was added. If false, an existing game was updated, or the operation failed.</param>
		/// <returns>True if the game was integrated, false otherwise.</returns>
		private GameInfo IntegrateGame(int appId, string appName, bool overwriteName, SortedSet<int> ignore, GameListingSource src, out bool isNew)
		{
			isNew = false;
			if (((ignore != null) && ignore.Contains(appId)) || !Database.IncludeItemInGameList(appId))
			{
				Logger.Verbose(GlobalStrings.GameData_SkippedIntegratingGame, appId, appName);

				return null;
			}

			GameInfo result = null;
			if (!Games.ContainsKey(appId))
			{
				result = new GameInfo(appId, appName, this);
				Games.Add(appId, result);
				isNew = true;
			}
			else
			{
				result = Games[appId];
				if (overwriteName)
				{
					result.Name = appName;
				}
			}

			result.ApplySource(src);

			Logger.Verbose(GlobalStrings.GameData_IntegratedGameIntoGameList, appId, appName, isNew);

			return result;
		}

		private int IntegrateGamesFromVdf(VDFNode appsNode, SortedSet<int> ignore)
		{
			int loadedGames = 0;

			Dictionary<string, VDFNode> gameNodeArray = appsNode.NodeArray;
			if (gameNodeArray == null)
			{
				return loadedGames;
			}

			foreach (KeyValuePair<string, VDFNode> gameNodePair in gameNodeArray)
			{
				if (!int.TryParse(gameNodePair.Key, out int gameId))
				{
					continue;
				}

				if (((ignore != null) && ignore.Contains(gameId)) || !Database.IncludeItemInGameList(gameId))
				{
					Logger.Instance.Verbose(GlobalStrings.GameData_SkippedProcessingGame, gameId);
				}
				else if ((gameNodePair.Value != null) && (gameNodePair.Value.NodeType == ValueType.Array))
				{
					GameInfo game = null;

					// Add the game to the list if it doesn't exist already
					if (!Games.ContainsKey(gameId))
					{
						game = new GameInfo(gameId, Database.GetName(gameId), this);
						Games.Add(gameId, game);
						Logger.Instance.Verbose(GlobalStrings.GameData_AddedNewGame, gameId, game.Name);
					}
					else
					{
						game = Games[gameId];
					}

					loadedGames++;

					game.ApplySource(GameListingSource.SteamConfig);

					game.Hidden = gameNodePair.Value.ContainsKey("hidden") && (gameNodePair.Value["hidden"].NodeInt != 0);

					VDFNode tagsNode = gameNodePair.Value["tags"];
					if (tagsNode == null)
					{
						Logger.Instance.Verbose(GlobalStrings.GameData_ProcessedGame, gameId, string.Join(",", game.Categories));

						continue;
					}

					Dictionary<string, VDFNode> tagArray = tagsNode.NodeArray;
					if (tagArray != null)
					{
						List<Category> cats = new List<Category>(tagArray.Count);
						foreach (VDFNode tag in tagArray.Values)
						{
							string tagName = tag.NodeString;
							if (tagName == null)
							{
								continue;
							}

							Category c = GetCategory(tagName);
							if (c != null)
							{
								cats.Add(c);
							}
						}

						if (cats.Count > 0)
						{
							SetGameCategories(gameId, cats, false);
						}
					}

					Logger.Instance.Verbose(GlobalStrings.GameData_ProcessedGame, gameId, string.Join(",", game.Categories));
				}
			}

			return loadedGames;
		}

		private bool IntegrateShortcut(int gameId, VDFNode gameNode, StringDictionary launchIds)
		{
			VDFNode nodeName = gameNode.GetNodeAt(new[]
			{
				"appname"
			}, false);

			string gameName = nodeName != null ? nodeName.NodeString : null;

			// The ID of the created game must be negative
			int newId = -(gameId + 1);

			// This should never happen, but just in case
			if (Games.ContainsKey(newId))
			{
				return false;
			}

			//Create the new GameInfo
			GameInfo game = new GameInfo(newId, gameName, this);
			Games.Add(newId, game);

			// Fill in the LaunchString
			game.LaunchString = launchIds[gameName];
			VDFNode nodeExecutable = gameNode.GetNodeAt(new[]
			{
				"exe"
			}, false);

			game.Executable = nodeExecutable != null ? nodeExecutable.NodeString : game.Executable;

			VDFNode nodeLastPlayTime = gameNode.GetNodeAt(new[]
			{
				"LastPlayTime"
			}, false);

			game.LastPlayed = nodeLastPlayTime != null ? nodeExecutable.NodeInt : game.LastPlayed;

			// Fill in categories
			VDFNode tagsNode = gameNode.GetNodeAt(new[]
			{
				"tags"
			}, false);

			foreach (KeyValuePair<string, VDFNode> tag in tagsNode.NodeArray)
			{
				string tagName = tag.Value.NodeString;
				game.AddCategory(GetCategory(tagName));
			}

			// Fill in Hidden
			game.Hidden = false;
			if (gameNode.ContainsKey("IsHidden"))
			{
				VDFNode hiddenNode = gameNode["IsHidden"];
				game.Hidden = (hiddenNode.NodeString == "1") || (hiddenNode.NodeInt == 1);
			}

			return true;
		}

		private bool LoadShortcutLaunchIds(long SteamId, out StringDictionary shortcutLaunchIds)
		{
			bool result = false;
			string filePath = string.Format(Constants.ScreenshotsFilePath, Settings.Instance.SteamPath, Profile.ID64toDirName(SteamId));

			shortcutLaunchIds = new StringDictionary();

			StreamReader reader = null;
			try
			{
				reader = new StreamReader(filePath, false);
				VDFNode dataRoot = VDFNode.LoadFromText(reader, true);

				VDFNode appsNode = dataRoot.GetNodeAt(new[]
				{
					"shortcutnames"
				}, false);

				foreach (KeyValuePair<string, VDFNode> shortcutPair in appsNode.NodeArray)
				{
					string launchId = shortcutPair.Key;
					string gameName = (string) shortcutPair.Value.NodeData;
					if (!shortcutLaunchIds.ContainsKey(gameName))
					{
						shortcutLaunchIds.Add(gameName, launchId);
					}
				}

				result = true;
			}
			catch (FileNotFoundException e)
			{
				Logger.Instance.Error(GlobalStrings.GameData_ErrorOpeningConfigFileParam, e.ToString());
			}
			catch (IOException e)
			{
				Logger.Instance.Error(GlobalStrings.GameData_LoadingErrorSteamConfig, e.ToString());
			}

			if (reader != null)
			{
				reader.Close();
			}

			return result;
		}

		#endregion
	}
}
