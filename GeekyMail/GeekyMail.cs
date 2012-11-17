// Copyright (c) 2012, Geeekzor
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted (subject to the limitations in the
// disclaimer below) provided that the following conditions are met:
// 
//  * Redistributions of source code must retain the above copyright
//    notice, this list of conditions and the following disclaimer.
// 
//  * Redistributions in binary form must reproduce the above copyright
//    notice, this list of conditions and the following disclaimer in the
//    documentation and/or other materials provided with the
//    distribution.
// 
//  * Neither the name of <Owner Organization> nor the names of its
//    contributors may be used to endorse or promote products derived
//    from this software without specific prior written permission.
// 
// NO EXPRESS OR IMPLIED LICENSES TO ANY PARTY'S PATENT RIGHTS ARE
// GRANTED BY THIS LICENSE.  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT
// HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
// MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR
// BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE
// OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN
// IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// Created 2012 04 17 23:36
// Updated 2012 04 27 14:57
// 
// GeekyMail

using Styx.Common;
using Styx.Common.Helpers;
using Styx.CommonBot;
using Styx.Plugins;

namespace GeekyMail
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using Bots.Gatherbuddy;
    using Styx;
    using Styx.Helpers;
    using Styx.WoWInternals;

    #region Form

    public class MailForm : Form
    {
        private Button _dumpChat;
        private Button _dumpStatus;
        private List<string> _hList;
        private PropertyGrid _pGrid;
        private Panel _panel;
        private TabPage _tabConfig;
        private TabControl _tabControl;
        private Button _sendTestMail;
        private Button _restoreDefaultSettings;
        private IContainer components = null;
        private TabPage tabPage1;

        public MailForm()
        {
            InitializeComponent();

            _pGrid.SelectedObject = Config.Instance;

            GridItem root = _pGrid.SelectedGridItem;
            while (root.Parent != null)
                root = root.Parent;

            foreach (GridItem g in root.GridItems)
                g.Expanded = false;
        }

        public static MailForm Instance { get; private set; }

        public static bool IsValid { get { return Instance != null && Instance.Visible && !Instance.Disposing && !Instance.IsDisposed; } }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this._panel = new System.Windows.Forms.Panel();
            this._dumpStatus = new System.Windows.Forms.Button();
            this._dumpChat = new System.Windows.Forms.Button();
            this._pGrid = new System.Windows.Forms.PropertyGrid();
            this._tabControl = new System.Windows.Forms.TabControl();
            this._tabConfig = new System.Windows.Forms.TabPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this._restoreDefaultSettings = new System.Windows.Forms.Button();
            this._sendTestMail = new System.Windows.Forms.Button();
            this._panel.SuspendLayout();
            this._tabControl.SuspendLayout();
            this._tabConfig.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _panel
            // 
            this._panel.Controls.Add(this._dumpStatus);
            this._panel.Controls.Add(this._dumpChat);
            this._panel.Dock = System.Windows.Forms.DockStyle.Top;
            this._panel.Location = new System.Drawing.Point(0, 0);
            this._panel.Name = "_panel";
            this._panel.Size = new System.Drawing.Size(329, 22);
            this._panel.TabIndex = 1;
            // 
            // _dumpStatus
            // 
            this._dumpStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dumpStatus.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this._dumpStatus.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this._dumpStatus.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this._dumpStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._dumpStatus.Location = new System.Drawing.Point(162, 0);
            this._dumpStatus.Name = "_dumpStatus";
            this._dumpStatus.Size = new System.Drawing.Size(167, 22);
            this._dumpStatus.TabIndex = 0;
            this._dumpStatus.Text = "Dump Status List";
            this._dumpStatus.UseVisualStyleBackColor = false;
            this._dumpStatus.Click += new System.EventHandler(this.DumpStatusClick);
            // 
            // _dumpChat
            // 
            this._dumpChat.Dock = System.Windows.Forms.DockStyle.Left;
            this._dumpChat.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this._dumpChat.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this._dumpChat.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this._dumpChat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._dumpChat.Location = new System.Drawing.Point(0, 0);
            this._dumpChat.Name = "_dumpChat";
            this._dumpChat.Size = new System.Drawing.Size(162, 22);
            this._dumpChat.TabIndex = 0;
            this._dumpChat.Text = "Dump Chat/Events List";
            this._dumpChat.UseVisualStyleBackColor = false;
            this._dumpChat.Click += new System.EventHandler(this.DumpChatClick);
            // 
            // _pGrid
            // 
            this._pGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this._pGrid.Location = new System.Drawing.Point(3, 3);
            this._pGrid.Name = "_pGrid";
            this._pGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this._pGrid.Size = new System.Drawing.Size(315, 299);
            this._pGrid.TabIndex = 0;
            this._pGrid.ToolbarVisible = false;
            this._pGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.PGridSettingsChanged);
            // 
            // _tabControl
            // 
            this._tabControl.Controls.Add(this._tabConfig);
            this._tabControl.Controls.Add(this.tabPage1);
            this._tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tabControl.Location = new System.Drawing.Point(0, 22);
            this._tabControl.Name = "_tabControl";
            this._tabControl.SelectedIndex = 0;
            this._tabControl.Size = new System.Drawing.Size(329, 331);
            this._tabControl.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this._tabControl.TabIndex = 2;
            // 
            // _tabConfig
            // 
            this._tabConfig.Controls.Add(this._pGrid);
            this._tabConfig.Location = new System.Drawing.Point(4, 22);
            this._tabConfig.Name = "_tabConfig";
            this._tabConfig.Padding = new System.Windows.Forms.Padding(3);
            this._tabConfig.Size = new System.Drawing.Size(321, 305);
            this._tabConfig.TabIndex = 0;
            this._tabConfig.Text = "Config";
            this._tabConfig.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this._restoreDefaultSettings);
            this.tabPage1.Controls.Add(this._sendTestMail);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(321, 305);
            this.tabPage1.TabIndex = 1;
            this.tabPage1.Text = "Debug";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // _restoreDefaultSettings
            // 
            this._restoreDefaultSettings.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this._restoreDefaultSettings.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this._restoreDefaultSettings.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this._restoreDefaultSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._restoreDefaultSettings.Location = new System.Drawing.Point(8, 265);
            this._restoreDefaultSettings.Name = "_restoreDefaultSettings";
            this._restoreDefaultSettings.Size = new System.Drawing.Size(166, 32);
            this._restoreDefaultSettings.TabIndex = 0;
            this._restoreDefaultSettings.Text = "Reset to Default Settings";
            this._restoreDefaultSettings.UseVisualStyleBackColor = false;
            this._restoreDefaultSettings.Click += new System.EventHandler(this.ResetSettings);
            // 
            // _sendTestMail
            // 
            this._sendTestMail.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this._sendTestMail.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this._sendTestMail.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this._sendTestMail.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._sendTestMail.Location = new System.Drawing.Point(8, 6);
            this._sendTestMail.Name = "_sendTestMail";
            this._sendTestMail.Size = new System.Drawing.Size(113, 32);
            this._sendTestMail.TabIndex = 0;
            this._sendTestMail.Text = "Send a Test Mail";
            this._sendTestMail.UseVisualStyleBackColor = false;
            this._sendTestMail.Click += new System.EventHandler(this.SendTestMail);
            // 
            // MailForm
            // 
            this.ClientSize = new System.Drawing.Size(329, 353);
            this.Controls.Add(this._tabControl);
            this.Controls.Add(this._panel);
            this.Name = "MailForm";
            this._panel.ResumeLayout(false);
            this._tabControl.ResumeLayout(false);
            this._tabConfig.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void PGridSettingsChanged(object s, PropertyValueChangedEventArgs e)
        {
            Config.Instance.Save();

            if (!String.IsNullOrEmpty(e.ChangedItem.Label))
            {
                GeekyMail.Log
                    (string.Format
                         ("{0}{1} changed to {2}",
                          string.IsNullOrEmpty(e.ChangedItem.Parent.Label) ? "" : e.ChangedItem.Parent.Label + ", ",
                          e.ChangedItem.Label,
                          e.ChangedItem.Label.ToLower().Contains("password") ? "******" : e.ChangedItem.Value));
            }

            Config.Instance.Load();
        }

        private void DumpChatClick(object sender, EventArgs e)
        {
            if (GeekyMail.ChatList.Count > 0)
            {
                foreach (var item in GeekyMail.ChatList)
                    GeekyMail.Log("{0}", item);
            }
            else
                GeekyMail.Log("Chat/Events List is Empty.");
        }

        private void DumpStatusClick(object sender, EventArgs e)
        {
            _hList = new List<string>();

            try
            {
                _hList.Add(String.Format("Class: {0}", StyxWoW.Me.Class));
                _hList.Add(String.Format("We are running {0}!", BotManager.Current.Name));
                _hList.Add
                    (String.Format
                         ("I'm at {0} in {1}",
                          Lua.GetReturnVal<string>("return GetMinimapZoneText()", 0),
                          Lua.GetReturnVal<string>("return GetZoneText()", 0)));

                _hList.Add(String.Format("Money : {0}g", StyxWoW.Me.Gold));

                _hList.Add(TreeRoot.StatusText);
                _hList.Add(TreeRoot.GoalText);

                if (StyxWoW.Me.Level < 85)
                {
                    _hList.Add(String.Format("EXP/h: {0}", GameStats.XPPerHour));
                    _hList.Add(String.Format("Time to Level: {0}", GeekyMail.ExtendedTimeFormat(GameStats.TimeToLevel)));
                }

                if (BotManager.Current.Name == BotManager.Instance.Bots["Grind Bot"].Name)
                {
                    _hList.Add(String.Format("Loots: {0}, Per/hr: {1}", GameStats.Loots, GameStats.LootsPerHour));
                    _hList.Add(String.Format("Kills: {0}", GameStats.MobsKilled));
                    _hList.Add(String.Format("Kills per hour: {0}", GameStats.MobsPerHour));
                    _hList.Add(String.Format("Deaths: {0}", GameStats.Deaths));
                }
                else if (BotManager.Current.Name == BotManager.Instance.Bots["Questing"].Name)
                {
                    _hList.Add(String.Format("Loots: {0}, Per/hr: {1}", GameStats.Loots, GameStats.LootsPerHour));
                    _hList.Add(String.Format("Kills: {0}", GameStats.MobsKilled));
                    _hList.Add(String.Format("Deaths: {0}", GameStats.Deaths));
                    //_hList.Add(String.Format("Poi {0}", Poi));
                }
                else if (BotManager.Current.Name == BotManager.Instance.Bots["BGBuddy"].Name)
                {
                    _hList.Add
                        (String.Format
                             ("BGs: {0} (Won:{1}, Lost:{2})",
                              GameStats.BGsCompleted,
                              GameStats.BGsWon,
                              GameStats.BGsLost));
                    _hList.Add
                        (String.Format
                             ("BGs/hour: {0} Lost/hr:{1} Won/hr:{2}",
                              GameStats.BGsPerHour,
                              GameStats.BGsLostPerHour,
                              GameStats.BGsWonPerHour));
                    _hList.Add
                        (String.Format
                             ("Honor Gained: {0}, Honor/hour : {1}", GameStats.HonorGained, GameStats.HonorPerHour));
                }
                else if (BotManager.Current.Name == BotManager.Instance.Bots["Gatherbuddy2"].Name)
                {
                    int i = GatherbuddyBot.NodeCollectionCount.Values.Sum(value => value);

                    try
                    {
                        _hList.Add(GeekyMail.TimeFormat(GatherbuddyBot.runningTime));
                        _hList.Add(String.Format("Nodes per hour {0}", GeekyMail.PerHour(i)));
                        _hList.Add(String.Format("Total Nodes: {0}", i));
                        _hList.AddRange
                            (GatherbuddyBot.NodeCollectionCount.Select
                                 (value => String.Format("{0} : {1}", value.Key, value.Value)));
                    }
                    catch (Exception ex)
                    {
                        Logging.Write(ex.Message);
                    }
                }
            }
            catch (Exception listEx)
            {
                Logging.Write(String.Format("{0}", listEx));
            }
            finally
            {
                Logging.Write(GeekyMail.GetColor(Color.YellowGreen), "{0}", DateTime.Now);
                foreach (var item in _hList.Where(item => !String.IsNullOrEmpty(item)))
                {
                    Logging.Write(GeekyMail.GetColor(Color.YellowGreen), item);
                }
            }
        }

        private void SendTestMail(object sender, EventArgs e)
        {
            try
            {
                GeekyMail.Instance.SendMailDirectly("Hey");
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
            }
        }

        private void ResetSettings(object sender, EventArgs e)
        {
            Config.Instance.InitializeDefaultValues();
        }
    }

    #endregion

    #region Geeky

    public class GeekyMail : HBPlugin
    {
        #region HBPlugin

        private static GeekyMail _instance = new GeekyMail();
        public static GeekyMail Instance { get { return _instance; } }

        public static List<string> ChatList = new List<string>();
        private static List<string> _hList;
        private MailAddress ReceiverAddress = new MailAddress(Config.Instance.YourEmailAddress);

        private MailAddress SenderAddress = new MailAddress
            (Config.Instance.PluginEmailAddress, Config.Instance.PluginName);

        private bool _isInitialized;
        private WaitTimer _shortTimer = new WaitTimer(ShortTimeSpan);
        private WaitTimer _statusTimer = new WaitTimer(StatusTimeSpan);

        private SmtpClient smtpClient = new SmtpClient
        {
            Host = Config.Instance.EmailHostAddress,
            Port = Config.Instance.EmailHostPort,
            EnableSsl = Config.Instance.EmailHostUseSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(Config.Instance.PluginEmailAddress, Config.Instance.PluginEmailPassword),
        };

        private static TimeSpan StatusTimeSpan { get { return new TimeSpan(0, 0, Config.Instance.HourlyStatusTime, 0); } }
        private static TimeSpan ShortTimeSpan { get { return new TimeSpan(0, 0, Config.Instance.SendTime, 0); } }

        public override string Name { get { return "GeekyMail"; } }
        private static string GName { get { return "GeekyMail"; } }
        public override string Author { get { return "Geeekzor"; } }
        public override Version Version { get { return new Version(1, 7, 0); } }
        public override bool WantButton { get { return true; } }
        public override string ButtonText { get { return Name + " Config"; } }

        public void DumpTime()
        {
            Log
                (string.Format
                     ("HourlyStatus Timeleft {0} of {1}",
                      ExtendedTimeFormat(_statusTimer.TimeLeft),
                      ExtendedTimeFormat(_statusTimer.WaitTime)));
            Log
                (string.Format
                     ("ShortTimer Timeleft {0} of {1}",
                      ExtendedTimeFormat(_shortTimer.TimeLeft),
                      ExtendedTimeFormat(_shortTimer.WaitTime)));

            _statusTimer.Update();
            _shortTimer.Update();

            Log(string.Format("{0}{1}", _shortTimer.TimeLeft, _statusTimer.TimeLeft));
        }

        public static void Log(string format, params object[] args)
        {
            Logging.Write(GetColor(Color.YellowGreen), "[" + GName + "] " + String.Format(format, args));
        }

        private static void Debug(string format, params object[] args)
        {
            if (Config.Instance.DebugLogging)
                Logging.Write(LogLevel.Normal,GetColor(Color.GreenYellow), "[" + GName + "] [DEBUG] " + String.Format(format, args));
            Logging.Write(LogLevel.Diagnostic, GetColor(Color.GreenYellow), "[" + GName + "] [DEBUG] " + String.Format(format, args));
        }

        public override void OnButtonPress()
        {
            if (!MailForm.IsValid)
                new MailForm().Show();
            else
                MailForm.Instance.Activate();
        }

        public override void Dispose()
        {
            _statusTimer.Finished -= StatusTimerFinished;
            _shortTimer.Finished -= ShortTimerFinished;

            Chat.Whisper -= WoWChatWhisper;
            Chat.Battleground -= WoWChatBattleground;
            Chat.BattlegroundLeader -= WoWChatBattleground;
            Chat.Guild -= WoWChatGuild;
            Chat.Officer -= WoWChatOfficer;
            Chat.Party -= WoWChatParty;
            Chat.PartyLeader -= WoWChatParty;
            Chat.Say -= WoWChatSay;
            Chat.Raid -= WoWChatRaid;
            Chat.RaidLeader -= WoWChatRaidLeader;

            Chat.Emote -= WoWChatEmote;
            Chat.Yell -= WoWChatYell;

            BotEvents.Player.OnLevelUp -= Player_OnLevelUp;
            BotEvents.Player.OnPlayerDied -= Player_OnPlayerDied;

            Lua.Events.DetachEvent("CHAT_MSG_LOOT", LootChatMonitor);
            Lua.Events.DetachEvent("DUEL_REQUESTED", DuelRequestMonitor);
        }

        public override void Pulse()
        {
            //if (_statusTimer.WaitTime != StatusTimeSpan)
            _statusTimer.WaitTime = StatusTimeSpan;
            //if (_shortTimer.WaitTime != ShortTimeSpan)
            _shortTimer.WaitTime = ShortTimeSpan;

            _statusTimer.Update();
            _shortTimer.Update();

            if (_statusTimer.IsFinished)
                _statusTimer.Reset();
            if (_shortTimer.IsFinished)
                _shortTimer.Reset();
        }

        /// <summary>
        /// Called when Honorbuddy builds the plugins
        /// </summary>
        public override void Initialize()
        {
            //Block double initializations.
            if (_isInitialized)
                return;

            //_StatusTimer.WaitTime = TimeSpan.
            _statusTimer.Finished += StatusTimerFinished;
            _shortTimer.Finished += ShortTimerFinished;

            // Register WoWChat Events
            Chat.Whisper += WoWChatWhisper;
            Chat.Battleground += WoWChatBattleground;
            Chat.BattlegroundLeader += WoWChatBattleground;
            Chat.Guild += WoWChatGuild;
            Chat.Officer += WoWChatOfficer;
            Chat.Party += WoWChatParty;
            Chat.PartyLeader += WoWChatParty;
            Chat.Say += WoWChatSay;
            Chat.Raid += WoWChatRaid;
            Chat.RaidLeader += WoWChatRaidLeader;
            Chat.Emote += WoWChatEmote;
            Chat.Yell += WoWChatYell;

            // Player Specific Events
            BotEvents.Player.OnLevelUp += Player_OnLevelUp;
            BotEvents.Player.OnPlayerDied += Player_OnPlayerDied;

            // Smtp Asynchronous Event.
            smtpClient.SendCompleted += smtpClient_SendCompleted;

            // Lua Events
            Lua.Events.AttachEvent("CHAT_MSG_LOOT", LootChatMonitor);
            Lua.Events.AttachEvent("DUEL_REQUESTED", DuelRequestMonitor);
            Lua.Events.AttachEvent("PLAYER_PVP_KILLS_CHANGED", PvpKillChanged);

            // Write some stuff when we have initialized.
            Logging.Write(GetColor(Color.YellowGreen), String.Format("[{0}] version {1} loaded", Name, Version));

            // Block double Initializations.
            _isInitialized = true;
        }

        /// <summary>
        /// Handle Smtp events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void smtpClient_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Logging.Write(GetColor(Color.YellowGreen), "Email Cancelled.");
            }
            if (e.Error != null)
            {
                Logging.Write(string.Format("Error, check your settings. \r\n{0}", e.Error.Message));
                Logging.Write(GetColor(Color.Tomato), string.Format("{0}", e.Error.Message));
                smtpClient.SendAsyncCancel();
            }
            else
            {
                Log(string.Format("{0} sent", e.UserState));
            }
        }

        #endregion

        /// <summary>
        /// Send a mail directly. 
        /// </summary>
        /// <param name="message">Message</param>
        public void SendMailDirectly(string message)
        {
            if (!Config.Instance.SendDirectly)
                return;

            var mailMessage = new MailMessage(SenderAddress, ReceiverAddress)
            {
                Subject = String.Format("[{0}] InstaMail from {1}", Name, StyxWoW.Me.Name),
                Body = message
            };
            try
            {
                
                smtpClient.SendAsync(mailMessage, "InstaMail");
                //smtpClient.Send(mailMessage);
            }
            catch (Exception x)
            {
                smtpClient.SendAsyncCancel();
                Logging.Write(GetColor(Color.Tomato), string.Format("Error: {0}", x.Message));
            }
        }

        /// <summary>
        /// Sends a mail
        /// </summary>
        /// <param name="message"></param>
        /// <param name="time"></param>
        /// <param name="args"></param>
        private void SendMail(IEnumerable<string> message, string time, params object[] args)
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append(time);
                foreach (var item in message)
                    sb.Append("\r\n").Append(item);

                Log(args != null ? String.Format("Sending Email {0}", args) : "Sending Email");
                var mailMessage = new MailMessage(SenderAddress, ReceiverAddress)
                {
                    Subject =
                        String.Format
                        ("[{0}] {3} from {1} {2}",
                         Name,
                         StyxWoW.Me.Name,
                         time,
                         args != null ? String.Format("{0}", args) : ""),
                    Body = sb.ToString()
                };
                AlternateView plainView = AlternateView.CreateAlternateViewFromString(sb.ToString(), null, "text/plain");
                plainView.TransferEncoding = TransferEncoding.QuotedPrintable;
                mailMessage.AlternateViews.Add(plainView);
                
                smtpClient.SendAsync(mailMessage, args);
                //smtpClient.Send(mailMessage);
            }
            catch (Exception exception)
            {
                Debug(string.Format("{0}", exception));
                smtpClient.SendAsyncCancel();
            }
        }

        #region TimerEvents

        private void ShortTimerFinished(object sender, WaitTimer.WaitTimerEventArgs e)
        {
            Debug
                (String.Format
                     ("Timer Finished : {0} Started : {1} : WaitTime : {2}",
                      e.TimeFinished,
                      e.TimeStarted,
                      ExtendedTimeFormat(e.WaitTime)));

            try
            {
                if (Config.Instance.UseSendTime)
                    Debug
                        (String.Format
                             ("{0} has passed! Checking if we should send email...",
                              ExtendedTimeFormat(_shortTimer.WaitTime)));
                if (Config.Instance.UseLinesBeforeSend)
                {
                    Debug
                        (String.Format
                             ("Chatlist contains {0} rows, minimum is set to {1}, {2}",
                              ChatList.Count,
                              Config.Instance.LinesBeforeSend,
                              (ChatList.Count >= Config.Instance.LinesBeforeSend ? "Sending Email" : " return")));
                }

                if (ChatList.Count < 1)
                {
                    _shortTimer.Reset();
                    return;
                }

                // Send email if the List count is above the value setting
                if (Config.Instance.UseLinesBeforeSend && ChatList.Count < Config.Instance.LinesBeforeSend)
                    return;

                try
                {
                    SendMail(ChatList, string.Format("{0}", e.TimeFinished), "Chat/Events List");
                }
                catch (Exception ex)
                {
                    Debug("{0}", ex);
                }
                finally
                {
                    // Clear list when we have sent it.
                    ChatList.Clear();
                }
            }
            catch (Exception ex)
            {
                Debug(ex.Message);
            }
        }

        private void StatusTimerFinished(object sender, WaitTimer.WaitTimerEventArgs e)
        {
            if (!Config.Instance.HourlyStatus)
                return;

            Debug
                (String.Format
                     ("HourlyTimer Finished : {0} Started : {1} : WaitTime : {2}",
                      e.TimeFinished,
                      e.TimeStarted,
                      ExtendedTimeFormat(e.WaitTime)));

            if (Config.Instance.HourlyStatus && _statusTimer.IsFinished)
            {
                _hList = new List<string>();

                try
                {
                    _hList.Add(String.Format("Class: {0}", StyxWoW.Me.Class));
                    _hList.Add(String.Format("We are running {0}!", BotManager.Current.Name));
                    _hList.Add
                        (String.Format
                             ("I'm at {0} in {1}",
                              Lua.GetReturnVal<string>("return GetMinimapZoneText()", 0),
                              Lua.GetReturnVal<string>("return GetZoneText()", 0)));

                    _hList.Add(String.Format("Money : {0}g", StyxWoW.Me.Gold));

                    _hList.Add(TreeRoot.StatusText);
                    _hList.Add(TreeRoot.GoalText);

                    if (StyxWoW.Me.Level < 85)
                    {
                        _hList.Add(String.Format("EXP/h: {0}", GameStats.XPPerHour));
                        _hList.Add(String.Format("Time to Level: {0}", ExtendedTimeFormat(GameStats.TimeToLevel)));
                    }

                    if (BotManager.Current.Name == BotManager.Instance.Bots["Grind Bot"].Name)
                    {
                        _hList.Add(String.Format("Loots: {0}, Per/hr: {1}", GameStats.Loots, GameStats.LootsPerHour));
                        _hList.Add(String.Format("Kills: {0}", GameStats.MobsKilled));
                        _hList.Add(String.Format("Kills per hour: {0}", GameStats.MobsPerHour));
                        _hList.Add(String.Format("Deaths: {0}", GameStats.Deaths));
                    }
                    else if (BotManager.Current.Name == BotManager.Instance.Bots["Questing"].Name)
                    {
                        _hList.Add(String.Format("Loots: {0}, Per/hr: {1}", GameStats.Loots, GameStats.LootsPerHour));
                        _hList.Add(String.Format("Kills: {0}", GameStats.MobsKilled));
                        _hList.Add(String.Format("Deaths: {0}", GameStats.Deaths));
                        //_hList.Add(String.Format("Poi {0}", BotPoi.Current));
                    }
                    else if (BotManager.Current.Name == BotManager.Instance.Bots["BGBuddy"].Name)
                    {
                        _hList.Add
                            (String.Format
                                 ("BGs: {0} (Won:{1}, Lost:{2})",
                                  GameStats.BGsCompleted,
                                  GameStats.BGsWon,
                                  GameStats.BGsLost));
                        _hList.Add
                            (String.Format
                                 ("BGs/hour: {0} Lost/hr:{1} Won/hr:{2}",
                                  GameStats.BGsPerHour,
                                  GameStats.BGsLostPerHour,
                                  GameStats.BGsWonPerHour));
                        _hList.Add
                            (String.Format
                                 ("Honor Gained: {0}, Honor/hour : {1}", GameStats.HonorGained, GameStats.HonorPerHour));
                    }
                    else if (BotManager.Current.Name == BotManager.Instance.Bots["Gatherbuddy2"].Name)
                    {
                        int i = GatherbuddyBot.NodeCollectionCount.Values.Sum(value => value);

                        try
                        {
                            _hList.Add(TimeFormat(GatherbuddyBot.runningTime));
                            _hList.Add(String.Format("Nodes per hour {0}", PerHour(i)));
                            _hList.Add(String.Format("Total Nodes: {0}", i));
                            _hList.AddRange
                                (GatherbuddyBot.NodeCollectionCount.Select
                                     (value => String.Format("{0} : {1}", value.Key, value.Value)));
                        }
                        catch (Exception x)
                        {
                            Log(x.Message);
                        }
                    }
                    else if (BotManager.Current.Name == BotManager.Instance.Bots["AutoAngler"].Name)
                    {
                        var fishList = new List<string>
                                       {
                                           "Striped Lurker",
                                           "Highland Guppy",
                                           "Deepsea Sagefish",
                                           "Algaefin Rockfish",
                                           "Blackbelly Mudfish"
                                       };

                        var itemList = new List<string>();
                        foreach (var bagItem in
                            StyxWoW.Me.BagItems.Where(bagItem => !itemList.Contains(bagItem.Name)).Where
                                (bagItem => bagItem.Name.ContainsAny(fishList)))
                            itemList.Add(bagItem.Name);

                        foreach (var item in itemList)
                        {
                            try
                            {
                                var count = Lua.GetReturnVal<int>
                                    (String.Format("return GetItemCount(\"{0}\")", item), 0);
                                _hList.Add(string.Format("{0} Count : {1}", item, count));
                            }
                            catch { }
                        }
                    }
                }
                catch (Exception listEx)
                {
                    Logging.Write(String.Format("{0}", listEx));
                }
                finally
                {
                    SendMail(_hList, string.Format("{0}", e.TimeFinished), "Hourly Status");
                }
            }
        }

        #endregion

        #region Events

        private void PvpKillChanged(object sender, LuaEventArgs args)
        {
            try
            {
                var hks = Lua.GetReturnVal<int>("return GetPVPLifetimeStats()", 0);
                var outMsg = String.Format("[PVP] Honorable Kill (Total Honorable Kills : {0}", hks);

                if (Config.Instance.SendPvpKillsDirectly)
                    SendMailDirectly(outMsg);

                if (!Config.Instance.LogPvpKills)
                    return;

                ChatList.Add(outMsg);
            }
            catch
            {
                Logging.Write(GetColor(Color.YellowGreen), String.Format("[{0}] Lua Error!", GName));
            }
        }

        private void DuelRequestMonitor(object sender, LuaEventArgs args)
        {
            var outMsg = String.Format("[Duel Request] from {0} ", args.Args[0]);

            if (Config.Instance.SendDuelDirectly)
                SendMailDirectly(outMsg);

            if (!Config.Instance.LogDuel)
                return;

            ChatList.Add(outMsg);
        }

        private void LootChatMonitor(object sender, LuaEventArgs args)
        {
            if (!Config.Instance.LogLoot)
                return;
            try
            {
                var lootname = Lua.GetReturnVal<string>("return GetItemInfo('" + args.Args[0] + "')", 0);
                var lootquality = Lua.GetReturnVal<int>("return GetItemInfo('" + args.Args[0] + "')", 2);

                if (lootquality < Config.Instance.LootFilter || !args.Args[0].ToString().Contains("You receive"))
                    return;

                var outMsg = String.Format("Looted {0}", lootname);
                ChatList.Add(outMsg);
            }
            catch
            {
                Logging.Write(GetColor(Color.YellowGreen), String.Format("[{0}] Lua Error!", GName));
            }
        }

        public static System.Windows.Media.Color GetColor(System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromRgb(color.R, color.G, color.B);
        }

        private void WoWChatYell(Chat.ChatLanguageSpecificEventArgs e)
        {
            var outMsg = String.Format("[Yell] {0} wrote {1}", e.Author, e.Message);

            if (Config.Instance.SendYellDirectly)
                SendMailDirectly(outMsg);

            if (!Config.Instance.LogYell)
                return;

            ChatList.Add(RemoveChatFormatting(outMsg));
        }

        private void WoWChatEmote(Chat.ChatAuthoredEventArgs e)
        {
            var outMsg = String.Format("[Emote] {0} wrote {1}", e.Author, e.Message);

            if (Config.Instance.SendEmoteDirectly)
                SendMailDirectly(outMsg);

            if (!Config.Instance.LogEmote)
                return;

            ChatList.Add(RemoveChatFormatting(outMsg));
        }

        private void Player_OnPlayerDied()
        {
            var outMsg = String.Format("Deaths: {0}", GameStats.Deaths);

            if (Config.Instance.SendDeathDirectly)
                SendMailDirectly(outMsg);

            if (!Config.Instance.DeathLog)
                return;

            ChatList.Add(RemoveChatFormatting(outMsg));
        }

        private void Player_OnLevelUp(BotEvents.Player.LevelUpEventArgs args)
        {
            var outMsg = String.Format("Leveled up : {0}", args.NewLevel);

            if (Config.Instance.SendLevelUpDirectly)
                SendMailDirectly(outMsg);

            if (!Config.Instance.LevelUpLog)
                return;

            ChatList.Add(RemoveChatFormatting(outMsg));
        }

        private void WoWChatOfficer(Chat.ChatLanguageSpecificEventArgs e)
        {
            var outMsg = String.Format("[Officer] {0} wrote {1}", e.Author, e.Message);

            if (Config.Instance.SendOfficerDirectly)
                SendMailDirectly(outMsg);

            if (!Config.Instance.LogOfficer)
                return;

            ChatList.Add(RemoveChatFormatting(outMsg));
        }

        private void WoWChatRaidLeader(Chat.ChatLanguageSpecificEventArgs e)
        {
            var outMsg = String.Format("[RaidLeader] {0} wrote {1}", e.Author, e.Message);

            if (Config.Instance.SendRaidDirectly)
                SendMailDirectly(outMsg);

            if (!Config.Instance.LogRaid)
                return;

            ChatList.Add(RemoveChatFormatting(outMsg));
        }

        private void WoWChatRaid(Chat.ChatLanguageSpecificEventArgs e)
        {
            var outMsg = String.Format("[Raid] {0} wrote {1}", e.Author, e.Message);

            if (Config.Instance.SendRaidDirectly)
                SendMailDirectly(outMsg);

            if (!Config.Instance.LogRaid)
                return;

            ChatList.Add(RemoveChatFormatting(outMsg));
        }

        private void WoWChatSay(Chat.ChatLanguageSpecificEventArgs e)
        {
            var outMsg = String.Format("[Say] {0} wrote {1}", e.Author, e.Message);

            if (Config.Instance.SendSayDirectly)
                SendMailDirectly(outMsg);

            if (!Config.Instance.LogSay)
                return;

            ChatList.Add(RemoveChatFormatting(outMsg));
        }

        private void WoWChatParty(Chat.ChatLanguageSpecificEventArgs e)
        {
            var outMsg = String.Format("[Party] {0} wrote {1}", e.Author, e.Message);

            if (Config.Instance.SendPartyDirectly)
                SendMailDirectly(outMsg);

            if (!Config.Instance.LogParty)
                return;

            ChatList.Add(RemoveChatFormatting(outMsg));
        }

        private void WoWChatGuild(Chat.ChatGuildEventArgs e)
        {
            var outMsg = String.Format("[Guild] {0} wrote {1}", e.Author, e.Message);

            if (Config.Instance.SendGuildDirectly)
                SendMailDirectly(outMsg);

            if (!Config.Instance.LogGuild)
                return;

            ChatList.Add(RemoveChatFormatting(outMsg));
        }

        private void WoWChatBattleground(Chat.ChatLanguageSpecificEventArgs e)
        {
            var outMsg = String.Format("[BG] {0} wrote {1}", e.Author, e.Message);

            if (Config.Instance.SendBattlegroundDirectly)
                SendMailDirectly(outMsg);

            if (!Config.Instance.LogBattleground)
                return;

            ChatList.Add(RemoveChatFormatting(outMsg));
        }

        private void WoWChatWhisper(Chat.ChatWhisperEventArgs e)
        {
            var outMsg = String.Format("[Whisper] {0} wrote {1}", e.Author, e.Message);

            if (Config.Instance.SendWhispersDirectly)
                SendMailDirectly(outMsg);

            if (!Config.Instance.LogWhispers)
                return;

            ChatList.Add(RemoveChatFormatting(outMsg));
        }

        #endregion

        #region Formatting

        private static readonly Regex ChatColorRegex = new Regex("\\|c[A-Za-z0-9]{6,8}"),
                                      ChatLinkRegex = new Regex("\\|H.*?\\|h");

        /// <summary>
        /// Formats a TimeSpan value.
        /// </summary>
        /// <param name="time">TimeSpan</param>
        /// <returns>"Been running for HH:MM"</returns>
        public static string TimeFormat(TimeSpan time)
        {
            return String.Format("{0:D2}:{1:D2}", time.Hours, time.Minutes);
        }

        public static string ExtendedTimeFormat(TimeSpan time)
        {
            string hours = String.Format("{0:D0} Hour{1}", time.Hours, (time.Hours > 1 ? "s" : ""));
            string minutes = String.Format("{0:D0} Minute{1}", time.Minutes, (time.Minutes > 1 ? "s" : ""));
            string seconds = String.Format("{0:D0} Second{1}", time.Seconds, (time.Seconds > 1 ? "s" : ""));
            if (time.Hours > 0 && time.Minutes > 0 && time.Seconds > 0)
            {
                return String.Format
                    ("{0}, {1} and {2}",
                     time.Hours > 0 ? hours : "",
                     time.Minutes > 0 ? minutes : "",
                     time.Seconds > 0 ? seconds : "");
            }
            if (time.Hours > 0 && time.Minutes > 0 && time.Seconds < 1)
                return String.Format("{0} and {1}", time.Hours > 0 ? hours : "", time.Minutes > 0 ? minutes : "");
            if (time.Hours > 0 && time.Minutes < 1 && time.Seconds < 1)
                return String.Format("{0}", time.Hours > 0 ? hours : "");
            if (time.Hours < 1 && time.Minutes > 0 && time.Seconds > 0)
                return String.Format("{0} and {1}", time.Minutes > 0 ? minutes : "", time.Seconds > 0 ? seconds : "");
            if (time.Hours > 0 && time.Minutes < 1 && time.Seconds > 1)
                return String.Format("{0} and {1}", time.Hours > 0 ? hours : "", time.Seconds > 0 ? seconds : "");
            if (time.Hours < 1 && time.Minutes > 0 && time.Seconds < 1)
                return String.Format("{0}", time.Minutes > 0 ? minutes : "");
            if (time.Hours < 1 && time.Minutes < 1 && time.Seconds < 1)
                return String.Format("Empty");
            return String.Format("{0}", time.Seconds > 0 ? seconds : "");
        }

        /// <summary>
        /// Timglide's Regex, remove that pesky formatting!
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string RemoveChatFormatting(string str)
        {
            str = ChatColorRegex.Replace(str, "");
            str = ChatLinkRegex.Replace(str, "");
            str = str.Replace("|h", "");
            str = str.Replace("|r", "");
            return str;
        }

        public static string PerHour(int item)
        {
            return String.Format("{0}", Round(item / GatherbuddyBot.runningTime.TotalSeconds * 3600));
        }

        /// <summary>
        /// Round the input
        /// </summary>
        /// <param name="d">Double type to round</param>
        /// <returns>Rounded value with 1 decimal</returns>
        private static double Round(double d)
        {
            return Math.Round(d, 1);
        }

        #endregion
    }

    #endregion

    #region Config

    public class Config : Settings
    {
        public static Config Instance = new Config();

        public Config()
            : base(
                Path.Combine
                    (CharacterSettingsDirectory, string.Format("\\GeekyMail-Settings-{0}.xml", StyxWoW.Me.Name))) { }

        #region Debug

        [Setting]
        [Category("Debug Logging")]
        [DisplayName("Debug Logging")]
        [Description("Writes Debug to the Main logging window.")]
        [Styx.Helpers.DefaultValue(false)]
        public bool DebugLogging { get; set; }

        #endregion

        #region EmailSettings

        [Setting]
        [Category("Email Settings")]
        [DisplayName("Plugin Email")]
        [Description("The Email Address the Plugin will use to send emails.")]
        [Styx.Helpers.DefaultValue("you@gmail.com")]
        public string PluginEmailAddress { get; set; }

        [Setting]
        [Category("Email Settings")]
        [DisplayName("Plugin Name")]
        [Description("The name the Plugin will use when sending emails.")]
        [Styx.Helpers.DefaultValue("name")]
        public string PluginName { get; set; }

        [Setting]
        [Category("Email Settings")]
        [DisplayName("Plugin Email Password")]
        [Description("The Password the Plugin will use to login to the email service.")]
        [Styx.Helpers.DefaultValue("password")]
        [PasswordPropertyText(true)]
        public string PluginEmailPassword { get; set; }

        [Setting]
        [Category("Email Settings")]
        [DisplayName("Your Email")]
        [Description("The Email Address the Plugin will send emails to.")]
        [Styx.Helpers.DefaultValue("you@gmail.com")]
        public string YourEmailAddress { get; set; }

        [Setting]
        [Category("Email Settings")]
        [DisplayName("Email Host Address")]
        [Description("The Address to the Email service, I prefer Gmail, its easy to use.")]
        [Styx.Helpers.DefaultValue("smtp.gmail.com")]
        public string EmailHostAddress { get; set; }

        [Setting]
        [Category("Email Settings")]
        [DisplayName("Email Host Port")]
        [Description("Email Host Port")]
        [Styx.Helpers.DefaultValue(587)]
        public int EmailHostPort { get; set; }

        [Setting]
        [Category("Email Settings")]
        [DisplayName("Use SSL")]
        [Description("Use SSL to send emails?")]
        [Styx.Helpers.DefaultValue(true)]
        public bool EmailHostUseSsl { get; set; }

        #endregion

        #region SendDirectly

        [Setting]
        [DisplayName("SendDirectly")]
        [Category("AntiSpam")]
        [Description("Bypass any other settings and send events directly.")]
        [Styx.Helpers.DefaultValue(false)]
        public bool SendDirectly { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("SendDirectly")]
        [DisplayName("Log Duel Requests")]
        [Description("Send a email directly on Duel Requests")]
        public bool SendDuelDirectly { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("SendDirectly")]
        [DisplayName("PvpKills")]
        [Description("Send a email directly on Pvp Kills")]
        public bool SendPvpKillsDirectly { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("SendDirectly")]
        [DisplayName("Emote Chat")]
        [Description("Send a email directly on Emotes")]
        public bool SendEmoteDirectly { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("SendDirectly")]
        [DisplayName("Yell Chat")]
        [Description("Send a email directly on Yells")]
        public bool SendYellDirectly { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("SendDirectly")]
        [DisplayName("Party Chat")]
        [Description("Send a email directly on Party Chat")]
        public bool SendPartyDirectly { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("SendDirectly")]
        [DisplayName("Whispers")]
        [Description("Send a email directly on whispers")]
        public bool SendWhispersDirectly { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("SendDirectly")]
        [DisplayName("Guild Chat")]
        [Description("Send a email directly on guild chat")]
        public bool SendGuildDirectly { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("SendDirectly")]
        [DisplayName("Officer Chat")]
        [Description("Send a email directly on officer chat")]
        public bool SendOfficerDirectly { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("SendDirectly")]
        [DisplayName("Raid Chat")]
        [Description("Send a email directly on raid chat")]
        public bool SendRaidDirectly { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("SendDirectly")]
        [DisplayName("Battleground Chat")]
        [Description("Send a email directly on battleground chat")]
        public bool SendBattlegroundDirectly { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("SendDirectly")]
        [DisplayName("Say Chat")]
        [Description("Send a email directly on Say chat")]
        public bool SendSayDirectly { get; set; }

        [Setting]
        [Category("SendDirectly")]
        [Description("Send a email directly if we level up")]
        [Styx.Helpers.DefaultValue(false)]
        public bool SendLevelUpDirectly { get; set; }

        [Setting]
        [Category("SendDirectly")]
        [Description("Send a email directly if we died")]
        [Styx.Helpers.DefaultValue(false)]
        public bool SendDeathDirectly { get; set; }

        #endregion

        #region SendSettings

        [Setting]
        [DisplayName("SendTime")]
        [Category("AntiSpam")]
        [Description(
            "To reduce spam, we will collect chat and events and send them all when a certain time has passed. 30 Mins by default"
            )]
        [Styx.Helpers.DefaultValue(30)]
        public int SendTime { get; set; }

        [Setting]
        [DisplayName("Hourly Status Time")]
        [Category("AntiSpam")]
        [Description("Time in minutes, sends a status update, includes bot specific information.")]
        [Styx.Helpers.DefaultValue(60)]
        public int HourlyStatusTime { get; set; }

        [Setting]
        [DisplayName("Lines")]
        [Category("AntiSpam")]
        [Description("To reduce spam, we will collect chat and send when we have enough chat lines.")]
        [Styx.Helpers.DefaultValue(5)]
        public int LinesBeforeSend { get; set; }

        [Setting]
        [DisplayName("Use Lines")]
        [Category("AntiSpam")]
        [Description("To reduce spam, we will collect chat and send when a time has passed. 30 Mins by default")]
        [Styx.Helpers.DefaultValue(true)]
        public bool UseLinesBeforeSend { get; set; }

        [Setting]
        [DisplayName("Use Send Time")]
        [Category("AntiSpam")]
        [Description("Use Send Time")]
        [Styx.Helpers.DefaultValue(true)]
        public bool UseSendTime { get; set; }

        [Setting]
        [Category("AntiSpam")]
        [Description("Send a Hourly Status")]
        [Styx.Helpers.DefaultValue(false)]
        public bool HourlyStatus { get; set; }

        #endregion

        #region Loot

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Loot")]
        [DisplayName("Log Looting")]
        [Description("Bot will log looting")]
        public bool LogLoot { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(3)]
        [Category("Loot")]
        [DisplayName("LootFilter")]
        [Description("0 for Grays, 1 for Whites, 2 for greens, 3 for blues (Default is 3 / Blues)")]
        public int LootFilter { get; set; }

        #endregion

        #region Logging

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Logging")]
        [DisplayName("Log Duel Requests")]
        [Description("Bot will log Duel Requests")]
        public bool LogDuel { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Logging")]
        [DisplayName("Log Pvp Kills")]
        [Description("Bot will log Pvp Kills, only Honorable Kills")]
        public bool LogPvpKills { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Logging")]
        [DisplayName("Log Emote Chat")]
        [Description("Bot will log Emotes")]
        public bool LogEmote { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Logging")]
        [DisplayName("Log Yell Chat")]
        [Description("Bot will log Yells")]
        public bool LogYell { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Logging")]
        [DisplayName("Log Party Chat")]
        [Description("Bot will log Party Chat")]
        public bool LogParty { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Logging")]
        [DisplayName("Log Whispers")]
        [Description("Bot will log whispers")]
        public bool LogWhispers { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Logging")]
        [DisplayName("Log Guild Chat")]
        [Description("Bot will log guild chat")]
        public bool LogGuild { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Logging")]
        [DisplayName("Log Officer Chat")]
        [Description("Bot will log officer chat")]
        public bool LogOfficer { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Logging")]
        [DisplayName("Log Raid Chat")]
        [Description("Bot will log raid chat")]
        public bool LogRaid { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Logging")]
        [DisplayName("Log Battleground Chat")]
        [Description("Bot will log battleground chat")]
        public bool LogBattleground { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Logging")]
        [DisplayName("Log Say Chat")]
        [Description("Bot will log Say chat")]
        public bool LogSay { get; set; }

        [Setting]
        [Category("Events")]
        [Description("Send a notification if we level up")]
        [Styx.Helpers.DefaultValue(false)]
        public bool LevelUpLog { get; set; }

        [Setting]
        [Category("Events")]
        [Description("Send a notification if we died")]
        [Styx.Helpers.DefaultValue(false)]
        public bool DeathLog { get; set; }

        #endregion
    }

    #endregion
}
