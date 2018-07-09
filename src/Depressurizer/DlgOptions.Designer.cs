﻿namespace Depressurizer {
    partial class DlgOptions {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing ) {
            if( disposing && ( components != null ) ) {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgOptions));
			this.cmdAccept = new System.Windows.Forms.Button();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.ttHelp = new Depressurizer.Lib.ExtToolTip();
			this.tabGeneral = new System.Windows.Forms.TabPage();
			this.grpSaving = new System.Windows.Forms.GroupBox();
			this.chkRemoveExtraEntries = new System.Windows.Forms.CheckBox();
			this.grpStartup = new System.Windows.Forms.GroupBox();
			this.txtDefaultProfile = new System.Windows.Forms.TextBox();
			this.cmdDefaultProfileBrowse = new System.Windows.Forms.Button();
			this.radLoad = new System.Windows.Forms.RadioButton();
			this.radCreate = new System.Windows.Forms.RadioButton();
			this.grpSteamDir = new System.Windows.Forms.GroupBox();
			this.txtSteamPath = new System.Windows.Forms.TextBox();
			this.cmdSteamPathBrowse = new System.Windows.Forms.Button();
			this.grpUILanguage = new System.Windows.Forms.GroupBox();
			this.cmbUILanguage = new System.Windows.Forms.ComboBox();
			this.grpDatabase = new System.Windows.Forms.GroupBox();
			this.chkUpdateAppInfoOnStartup = new System.Windows.Forms.CheckBox();
			this.chkAutosaveDB = new System.Windows.Forms.CheckBox();
			this.chkUpdateHltbOnStartup = new System.Windows.Forms.CheckBox();
			this.chkIncludeImputedTimes = new System.Windows.Forms.CheckBox();
			this.helpIncludeImputedTimes = new System.Windows.Forms.Label();
			this.lblScapePrompt1 = new System.Windows.Forms.Label();
			this.lblScrapePrompt2 = new System.Windows.Forms.Label();
			this.numScrapePromptDays = new System.Windows.Forms.NumericUpDown();
			this.grpDepressurizerUpdates = new System.Windows.Forms.GroupBox();
			this.chkCheckForDepressurizerUpdates = new System.Windows.Forms.CheckBox();
			this.grpStoreLanguage = new System.Windows.Forms.GroupBox();
			this.cmbStoreLanguage = new System.Windows.Forms.ComboBox();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabGeneral.SuspendLayout();
			this.grpSaving.SuspendLayout();
			this.grpStartup.SuspendLayout();
			this.grpSteamDir.SuspendLayout();
			this.grpUILanguage.SuspendLayout();
			this.grpDatabase.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numScrapePromptDays)).BeginInit();
			this.grpDepressurizerUpdates.SuspendLayout();
			this.grpStoreLanguage.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// cmdAccept
			// 
			resources.ApplyResources(this.cmdAccept, "cmdAccept");
			this.cmdAccept.Name = "cmdAccept";
			this.cmdAccept.UseVisualStyleBackColor = true;
			this.cmdAccept.Click += new System.EventHandler(this.cmdAccept_Click);
			// 
			// cmdCancel
			// 
			resources.ApplyResources(this.cmdCancel, "cmdCancel");
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.UseVisualStyleBackColor = true;
			this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
			// 
			// tabGeneral
			// 
			this.tabGeneral.Controls.Add(this.grpStoreLanguage);
			this.tabGeneral.Controls.Add(this.grpDepressurizerUpdates);
			this.tabGeneral.Controls.Add(this.grpDatabase);
			this.tabGeneral.Controls.Add(this.grpUILanguage);
			this.tabGeneral.Controls.Add(this.grpSteamDir);
			this.tabGeneral.Controls.Add(this.grpStartup);
			this.tabGeneral.Controls.Add(this.grpSaving);
			resources.ApplyResources(this.tabGeneral, "tabGeneral");
			this.tabGeneral.Name = "tabGeneral";
			this.tabGeneral.UseVisualStyleBackColor = true;
			// 
			// grpSaving
			// 
			resources.ApplyResources(this.grpSaving, "grpSaving");
			this.grpSaving.Controls.Add(this.chkRemoveExtraEntries);
			this.grpSaving.Name = "grpSaving";
			this.grpSaving.TabStop = false;
			// 
			// chkRemoveExtraEntries
			// 
			resources.ApplyResources(this.chkRemoveExtraEntries, "chkRemoveExtraEntries");
			this.chkRemoveExtraEntries.Name = "chkRemoveExtraEntries";
			this.chkRemoveExtraEntries.UseVisualStyleBackColor = true;
			// 
			// grpStartup
			// 
			resources.ApplyResources(this.grpStartup, "grpStartup");
			this.grpStartup.Controls.Add(this.radCreate);
			this.grpStartup.Controls.Add(this.radLoad);
			this.grpStartup.Controls.Add(this.cmdDefaultProfileBrowse);
			this.grpStartup.Controls.Add(this.txtDefaultProfile);
			this.grpStartup.Name = "grpStartup";
			this.grpStartup.TabStop = false;
			// 
			// txtDefaultProfile
			// 
			resources.ApplyResources(this.txtDefaultProfile, "txtDefaultProfile");
			this.txtDefaultProfile.Name = "txtDefaultProfile";
			// 
			// cmdDefaultProfileBrowse
			// 
			resources.ApplyResources(this.cmdDefaultProfileBrowse, "cmdDefaultProfileBrowse");
			this.cmdDefaultProfileBrowse.Name = "cmdDefaultProfileBrowse";
			this.cmdDefaultProfileBrowse.UseVisualStyleBackColor = true;
			this.cmdDefaultProfileBrowse.Click += new System.EventHandler(this.cmdDefaultProfileBrowse_Click);
			// 
			// radLoad
			// 
			resources.ApplyResources(this.radLoad, "radLoad");
			this.radLoad.Name = "radLoad";
			this.radLoad.TabStop = true;
			this.radLoad.UseVisualStyleBackColor = true;
			// 
			// radCreate
			// 
			resources.ApplyResources(this.radCreate, "radCreate");
			this.radCreate.Name = "radCreate";
			this.radCreate.TabStop = true;
			this.radCreate.UseVisualStyleBackColor = true;
			// 
			// grpSteamDir
			// 
			resources.ApplyResources(this.grpSteamDir, "grpSteamDir");
			this.grpSteamDir.Controls.Add(this.cmdSteamPathBrowse);
			this.grpSteamDir.Controls.Add(this.txtSteamPath);
			this.grpSteamDir.Name = "grpSteamDir";
			this.grpSteamDir.TabStop = false;
			// 
			// txtSteamPath
			// 
			resources.ApplyResources(this.txtSteamPath, "txtSteamPath");
			this.txtSteamPath.Name = "txtSteamPath";
			// 
			// cmdSteamPathBrowse
			// 
			resources.ApplyResources(this.cmdSteamPathBrowse, "cmdSteamPathBrowse");
			this.cmdSteamPathBrowse.Name = "cmdSteamPathBrowse";
			this.cmdSteamPathBrowse.UseVisualStyleBackColor = true;
			this.cmdSteamPathBrowse.Click += new System.EventHandler(this.cmdSteamPathBrowse_Click);
			// 
			// grpUILanguage
			// 
			resources.ApplyResources(this.grpUILanguage, "grpUILanguage");
			this.grpUILanguage.Controls.Add(this.cmbUILanguage);
			this.grpUILanguage.Name = "grpUILanguage";
			this.grpUILanguage.TabStop = false;
			// 
			// cmbUILanguage
			// 
			this.cmbUILanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbUILanguage.FormattingEnabled = true;
			resources.ApplyResources(this.cmbUILanguage, "cmbUILanguage");
			this.cmbUILanguage.Name = "cmbUILanguage";
			// 
			// grpDatabase
			// 
			resources.ApplyResources(this.grpDatabase, "grpDatabase");
			this.grpDatabase.Controls.Add(this.numScrapePromptDays);
			this.grpDatabase.Controls.Add(this.lblScrapePrompt2);
			this.grpDatabase.Controls.Add(this.lblScapePrompt1);
			this.grpDatabase.Controls.Add(this.helpIncludeImputedTimes);
			this.grpDatabase.Controls.Add(this.chkIncludeImputedTimes);
			this.grpDatabase.Controls.Add(this.chkUpdateHltbOnStartup);
			this.grpDatabase.Controls.Add(this.chkAutosaveDB);
			this.grpDatabase.Controls.Add(this.chkUpdateAppInfoOnStartup);
			this.grpDatabase.Name = "grpDatabase";
			this.grpDatabase.TabStop = false;
			// 
			// chkUpdateAppInfoOnStartup
			// 
			resources.ApplyResources(this.chkUpdateAppInfoOnStartup, "chkUpdateAppInfoOnStartup");
			this.chkUpdateAppInfoOnStartup.Name = "chkUpdateAppInfoOnStartup";
			this.chkUpdateAppInfoOnStartup.UseVisualStyleBackColor = true;
			// 
			// chkAutosaveDB
			// 
			resources.ApplyResources(this.chkAutosaveDB, "chkAutosaveDB");
			this.chkAutosaveDB.Name = "chkAutosaveDB";
			this.chkAutosaveDB.UseVisualStyleBackColor = true;
			// 
			// chkUpdateHltbOnStartup
			// 
			resources.ApplyResources(this.chkUpdateHltbOnStartup, "chkUpdateHltbOnStartup");
			this.chkUpdateHltbOnStartup.Name = "chkUpdateHltbOnStartup";
			this.chkUpdateHltbOnStartup.UseVisualStyleBackColor = true;
			// 
			// chkIncludeImputedTimes
			// 
			resources.ApplyResources(this.chkIncludeImputedTimes, "chkIncludeImputedTimes");
			this.chkIncludeImputedTimes.Name = "chkIncludeImputedTimes";
			this.chkIncludeImputedTimes.UseVisualStyleBackColor = true;
			// 
			// helpIncludeImputedTimes
			// 
			resources.ApplyResources(this.helpIncludeImputedTimes, "helpIncludeImputedTimes");
			this.helpIncludeImputedTimes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.helpIncludeImputedTimes.Name = "helpIncludeImputedTimes";
			// 
			// lblScapePrompt1
			// 
			resources.ApplyResources(this.lblScapePrompt1, "lblScapePrompt1");
			this.lblScapePrompt1.Name = "lblScapePrompt1";
			// 
			// lblScrapePrompt2
			// 
			resources.ApplyResources(this.lblScrapePrompt2, "lblScrapePrompt2");
			this.lblScrapePrompt2.Name = "lblScrapePrompt2";
			// 
			// numScrapePromptDays
			// 
			resources.ApplyResources(this.numScrapePromptDays, "numScrapePromptDays");
			this.numScrapePromptDays.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
			this.numScrapePromptDays.Name = "numScrapePromptDays";
			// 
			// grpDepressurizerUpdates
			// 
			resources.ApplyResources(this.grpDepressurizerUpdates, "grpDepressurizerUpdates");
			this.grpDepressurizerUpdates.Controls.Add(this.chkCheckForDepressurizerUpdates);
			this.grpDepressurizerUpdates.Name = "grpDepressurizerUpdates";
			this.grpDepressurizerUpdates.TabStop = false;
			// 
			// chkCheckForDepressurizerUpdates
			// 
			resources.ApplyResources(this.chkCheckForDepressurizerUpdates, "chkCheckForDepressurizerUpdates");
			this.chkCheckForDepressurizerUpdates.Name = "chkCheckForDepressurizerUpdates";
			this.chkCheckForDepressurizerUpdates.UseVisualStyleBackColor = true;
			// 
			// grpStoreLanguage
			// 
			resources.ApplyResources(this.grpStoreLanguage, "grpStoreLanguage");
			this.grpStoreLanguage.Controls.Add(this.cmbStoreLanguage);
			this.grpStoreLanguage.Name = "grpStoreLanguage";
			this.grpStoreLanguage.TabStop = false;
			// 
			// cmbStoreLanguage
			// 
			this.cmbStoreLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbStoreLanguage.FormattingEnabled = true;
			resources.ApplyResources(this.cmbStoreLanguage, "cmbStoreLanguage");
			this.cmbStoreLanguage.Name = "cmbStoreLanguage";
			// 
			// tabControl
			// 
			resources.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.Controls.Add(this.tabGeneral);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			// 
			// DlgOptions
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ControlBox = false;
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.cmdAccept);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "DlgOptions";
			this.ShowInTaskbar = false;
			this.Load += new System.EventHandler(this.OptionsForm_Load);
			this.tabGeneral.ResumeLayout(false);
			this.grpSaving.ResumeLayout(false);
			this.grpSaving.PerformLayout();
			this.grpStartup.ResumeLayout(false);
			this.grpStartup.PerformLayout();
			this.grpSteamDir.ResumeLayout(false);
			this.grpSteamDir.PerformLayout();
			this.grpUILanguage.ResumeLayout(false);
			this.grpDatabase.ResumeLayout(false);
			this.grpDatabase.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numScrapePromptDays)).EndInit();
			this.grpDepressurizerUpdates.ResumeLayout(false);
			this.grpDepressurizerUpdates.PerformLayout();
			this.grpStoreLanguage.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion
        private Lib.ExtToolTip ttHelp;
        private System.Windows.Forms.Button cmdAccept;
        private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.TabPage tabGeneral;
		private System.Windows.Forms.GroupBox grpStoreLanguage;
		private System.Windows.Forms.ComboBox cmbStoreLanguage;
		private System.Windows.Forms.GroupBox grpDepressurizerUpdates;
		private System.Windows.Forms.CheckBox chkCheckForDepressurizerUpdates;
		private System.Windows.Forms.GroupBox grpDatabase;
		private System.Windows.Forms.NumericUpDown numScrapePromptDays;
		private System.Windows.Forms.Label lblScrapePrompt2;
		private System.Windows.Forms.Label lblScapePrompt1;
		private System.Windows.Forms.Label helpIncludeImputedTimes;
		private System.Windows.Forms.CheckBox chkIncludeImputedTimes;
		private System.Windows.Forms.CheckBox chkUpdateHltbOnStartup;
		private System.Windows.Forms.CheckBox chkAutosaveDB;
		private System.Windows.Forms.CheckBox chkUpdateAppInfoOnStartup;
		private System.Windows.Forms.GroupBox grpUILanguage;
		private System.Windows.Forms.ComboBox cmbUILanguage;
		private System.Windows.Forms.GroupBox grpSteamDir;
		private System.Windows.Forms.Button cmdSteamPathBrowse;
		private System.Windows.Forms.TextBox txtSteamPath;
		private System.Windows.Forms.GroupBox grpStartup;
		private System.Windows.Forms.RadioButton radCreate;
		private System.Windows.Forms.RadioButton radLoad;
		private System.Windows.Forms.Button cmdDefaultProfileBrowse;
		private System.Windows.Forms.TextBox txtDefaultProfile;
		private System.Windows.Forms.GroupBox grpSaving;
		private System.Windows.Forms.CheckBox chkRemoveExtraEntries;
		private System.Windows.Forms.TabControl tabControl;
	}
}