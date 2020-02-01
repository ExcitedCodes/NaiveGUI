namespace NaiveGUI
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.listView_listeners = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList_remote = new System.Windows.Forms.ImageList(this.components);
            this.textBox_host = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox_protocol = new System.Windows.Forms.ComboBox();
            this.textBox_username = new System.Windows.Forms.TextBox();
            this.textBox_password = new System.Windows.Forms.TextBox();
            this.checkBox_username = new System.Windows.Forms.CheckBox();
            this.checkBox_password = new System.Windows.Forms.CheckBox();
            this.checkBox_host = new System.Windows.Forms.CheckBox();
            this.button_remote_save = new System.Windows.Forms.Button();
            this.button_remote_discard = new System.Windows.Forms.Button();
            this.checkBox_padding = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox_remote_config = new System.Windows.Forms.GroupBox();
            this.button_remote_delete = new System.Windows.Forms.Button();
            this.textBox_group = new System.Windows.Forms.TextBox();
            this.textBox_name = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox_quic = new System.Windows.Forms.ComboBox();
            this.button_remote_add = new System.Windows.Forms.Button();
            this.textBox_listener_endpoint = new System.Windows.Forms.TextBox();
            this.button_listener_add = new System.Windows.Forms.Button();
            this.groupBox_listener = new System.Windows.Forms.GroupBox();
            this.groupBox_remotes = new System.Windows.Forms.GroupBox();
            this.button_subscription = new System.Windows.Forms.Button();
            this.tree_remotes = new System.Windows.Forms.TreeView();
            this.checkBox_logging = new System.Windows.Forms.CheckBox();
            this.textBox_log = new System.Windows.Forms.TextBox();
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip_tray = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripSeparator_listeners = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem_exit = new System.Windows.Forms.ToolStripMenuItem();
            this.timer_main = new System.Windows.Forms.Timer(this.components);
            this.checkBox_autorun = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip_listener = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox_remote_config.SuspendLayout();
            this.groupBox_listener.SuspendLayout();
            this.groupBox_remotes.SuspendLayout();
            this.contextMenuStrip_tray.SuspendLayout();
            this.contextMenuStrip_listener.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView_listeners
            // 
            this.listView_listeners.CheckBoxes = true;
            this.listView_listeners.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.listView_listeners.ContextMenuStrip = this.contextMenuStrip_listener;
            this.listView_listeners.FullRowSelect = true;
            this.listView_listeners.HideSelection = false;
            this.listView_listeners.LabelEdit = true;
            this.listView_listeners.Location = new System.Drawing.Point(6, 20);
            this.listView_listeners.Name = "listView_listeners";
            this.listView_listeners.Size = new System.Drawing.Size(196, 190);
            this.listView_listeners.TabIndex = 0;
            this.listView_listeners.UseCompatibleStateImageBehavior = false;
            this.listView_listeners.View = System.Windows.Forms.View.Details;
            this.listView_listeners.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listView_listeners_AfterLabelEdit);
            this.listView_listeners.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView_listeners_ItemChecked);
            this.listView_listeners.SelectedIndexChanged += new System.EventHandler(this.listView_listeners_SelectedIndexChanged);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "EndPoint";
            this.columnHeader2.Width = 175;
            // 
            // imageList_remote
            // 
            this.imageList_remote.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_remote.ImageStream")));
            this.imageList_remote.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_remote.Images.SetKeyName(0, "Horse");
            this.imageList_remote.Images.SetKeyName(1, "Dots");
            this.imageList_remote.Images.SetKeyName(2, "Naive");
            // 
            // textBox_host
            // 
            this.textBox_host.Location = new System.Drawing.Point(71, 100);
            this.textBox_host.Name = "textBox_host";
            this.textBox_host.PasswordChar = '*';
            this.textBox_host.Size = new System.Drawing.Size(169, 21);
            this.textBox_host.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "Protocol:";
            // 
            // comboBox_protocol
            // 
            this.comboBox_protocol.FormattingEnabled = true;
            this.comboBox_protocol.Items.AddRange(new object[] {
            "HTTPS",
            "QUIC"});
            this.comboBox_protocol.Location = new System.Drawing.Point(71, 74);
            this.comboBox_protocol.Name = "comboBox_protocol";
            this.comboBox_protocol.Size = new System.Drawing.Size(97, 20);
            this.comboBox_protocol.TabIndex = 12;
            // 
            // textBox_username
            // 
            this.textBox_username.Location = new System.Drawing.Point(90, 127);
            this.textBox_username.Name = "textBox_username";
            this.textBox_username.PasswordChar = '*';
            this.textBox_username.Size = new System.Drawing.Size(150, 21);
            this.textBox_username.TabIndex = 13;
            // 
            // textBox_password
            // 
            this.textBox_password.Location = new System.Drawing.Point(90, 154);
            this.textBox_password.Name = "textBox_password";
            this.textBox_password.PasswordChar = '*';
            this.textBox_password.Size = new System.Drawing.Size(150, 21);
            this.textBox_password.TabIndex = 15;
            // 
            // checkBox_username
            // 
            this.checkBox_username.AutoSize = true;
            this.checkBox_username.Location = new System.Drawing.Point(6, 129);
            this.checkBox_username.Name = "checkBox_username";
            this.checkBox_username.Size = new System.Drawing.Size(78, 16);
            this.checkBox_username.TabIndex = 17;
            this.checkBox_username.Text = "UserName:";
            this.checkBox_username.UseVisualStyleBackColor = true;
            this.checkBox_username.CheckedChanged += new System.EventHandler(this.checkBox_username_CheckedChanged);
            // 
            // checkBox_password
            // 
            this.checkBox_password.AutoSize = true;
            this.checkBox_password.Location = new System.Drawing.Point(6, 156);
            this.checkBox_password.Name = "checkBox_password";
            this.checkBox_password.Size = new System.Drawing.Size(78, 16);
            this.checkBox_password.TabIndex = 18;
            this.checkBox_password.Text = "Password:";
            this.checkBox_password.UseVisualStyleBackColor = true;
            this.checkBox_password.CheckedChanged += new System.EventHandler(this.checkBox_password_CheckedChanged);
            // 
            // checkBox_host
            // 
            this.checkBox_host.AutoSize = true;
            this.checkBox_host.Location = new System.Drawing.Point(6, 102);
            this.checkBox_host.Name = "checkBox_host";
            this.checkBox_host.Size = new System.Drawing.Size(54, 16);
            this.checkBox_host.TabIndex = 19;
            this.checkBox_host.Text = "Host:";
            this.checkBox_host.UseVisualStyleBackColor = true;
            this.checkBox_host.CheckedChanged += new System.EventHandler(this.checkBox_host_CheckedChanged);
            // 
            // button_remote_save
            // 
            this.button_remote_save.Location = new System.Drawing.Point(177, 207);
            this.button_remote_save.Name = "button_remote_save";
            this.button_remote_save.Size = new System.Drawing.Size(64, 23);
            this.button_remote_save.TabIndex = 20;
            this.button_remote_save.Text = "Save";
            this.button_remote_save.UseVisualStyleBackColor = true;
            this.button_remote_save.Click += new System.EventHandler(this.button_remote_save_Click);
            // 
            // button_remote_discard
            // 
            this.button_remote_discard.Location = new System.Drawing.Point(107, 207);
            this.button_remote_discard.Name = "button_remote_discard";
            this.button_remote_discard.Size = new System.Drawing.Size(64, 23);
            this.button_remote_discard.TabIndex = 21;
            this.button_remote_discard.Text = "Discard";
            this.button_remote_discard.UseVisualStyleBackColor = true;
            this.button_remote_discard.Click += new System.EventHandler(this.button_remote_discard_Click);
            // 
            // checkBox_padding
            // 
            this.checkBox_padding.AutoSize = true;
            this.checkBox_padding.Location = new System.Drawing.Point(174, 76);
            this.checkBox_padding.Name = "checkBox_padding";
            this.checkBox_padding.Size = new System.Drawing.Size(66, 16);
            this.checkBox_padding.TabIndex = 22;
            this.checkBox_padding.Text = "Padding";
            this.checkBox_padding.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 184);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 12);
            this.label3.TabIndex = 24;
            this.label3.Text = "QUIC Version:";
            // 
            // groupBox_remote_config
            // 
            this.groupBox_remote_config.Controls.Add(this.button_remote_delete);
            this.groupBox_remote_config.Controls.Add(this.textBox_group);
            this.groupBox_remote_config.Controls.Add(this.textBox_name);
            this.groupBox_remote_config.Controls.Add(this.label6);
            this.groupBox_remote_config.Controls.Add(this.label5);
            this.groupBox_remote_config.Controls.Add(this.comboBox_quic);
            this.groupBox_remote_config.Controls.Add(this.comboBox_protocol);
            this.groupBox_remote_config.Controls.Add(this.label3);
            this.groupBox_remote_config.Controls.Add(this.textBox_host);
            this.groupBox_remote_config.Controls.Add(this.label4);
            this.groupBox_remote_config.Controls.Add(this.checkBox_padding);
            this.groupBox_remote_config.Controls.Add(this.textBox_username);
            this.groupBox_remote_config.Controls.Add(this.button_remote_discard);
            this.groupBox_remote_config.Controls.Add(this.textBox_password);
            this.groupBox_remote_config.Controls.Add(this.button_remote_save);
            this.groupBox_remote_config.Controls.Add(this.checkBox_username);
            this.groupBox_remote_config.Controls.Add(this.checkBox_host);
            this.groupBox_remote_config.Controls.Add(this.checkBox_password);
            this.groupBox_remote_config.Enabled = false;
            this.groupBox_remote_config.Location = new System.Drawing.Point(553, 12);
            this.groupBox_remote_config.Name = "groupBox_remote_config";
            this.groupBox_remote_config.Size = new System.Drawing.Size(255, 243);
            this.groupBox_remote_config.TabIndex = 25;
            this.groupBox_remote_config.TabStop = false;
            this.groupBox_remote_config.Text = "Remote Config";
            // 
            // button_remote_delete
            // 
            this.button_remote_delete.Location = new System.Drawing.Point(37, 207);
            this.button_remote_delete.Name = "button_remote_delete";
            this.button_remote_delete.Size = new System.Drawing.Size(64, 23);
            this.button_remote_delete.TabIndex = 30;
            this.button_remote_delete.Text = "Delete";
            this.button_remote_delete.UseVisualStyleBackColor = true;
            this.button_remote_delete.Click += new System.EventHandler(this.button_remote_delete_Click);
            // 
            // textBox_group
            // 
            this.textBox_group.Location = new System.Drawing.Point(53, 47);
            this.textBox_group.Name = "textBox_group";
            this.textBox_group.Size = new System.Drawing.Size(187, 21);
            this.textBox_group.TabIndex = 29;
            // 
            // textBox_name
            // 
            this.textBox_name.Location = new System.Drawing.Point(53, 20);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(187, 21);
            this.textBox_name.TabIndex = 28;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 50);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 27;
            this.label6.Text = "Group:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 12);
            this.label5.TabIndex = 26;
            this.label5.Text = "Name:";
            // 
            // comboBox_quic
            // 
            this.comboBox_quic.FormattingEnabled = true;
            this.comboBox_quic.Items.AddRange(new object[] {
            "-",
            "39",
            "43",
            "46",
            "47",
            "48",
            "49"});
            this.comboBox_quic.Location = new System.Drawing.Point(95, 181);
            this.comboBox_quic.Name = "comboBox_quic";
            this.comboBox_quic.Size = new System.Drawing.Size(145, 20);
            this.comboBox_quic.TabIndex = 25;
            // 
            // button_remote_add
            // 
            this.button_remote_add.Location = new System.Drawing.Point(260, 216);
            this.button_remote_add.Name = "button_remote_add";
            this.button_remote_add.Size = new System.Drawing.Size(55, 21);
            this.button_remote_add.TabIndex = 26;
            this.button_remote_add.Text = "Add";
            this.button_remote_add.UseVisualStyleBackColor = true;
            this.button_remote_add.Click += new System.EventHandler(this.button_remote_add_Click);
            // 
            // textBox_listener_endpoint
            // 
            this.textBox_listener_endpoint.Location = new System.Drawing.Point(6, 216);
            this.textBox_listener_endpoint.MaxLength = 255;
            this.textBox_listener_endpoint.Name = "textBox_listener_endpoint";
            this.textBox_listener_endpoint.Size = new System.Drawing.Size(169, 21);
            this.textBox_listener_endpoint.TabIndex = 0;
            // 
            // button_listener_add
            // 
            this.button_listener_add.Font = new System.Drawing.Font("宋体", 10F);
            this.button_listener_add.Location = new System.Drawing.Point(181, 216);
            this.button_listener_add.Name = "button_listener_add";
            this.button_listener_add.Size = new System.Drawing.Size(19, 21);
            this.button_listener_add.TabIndex = 2;
            this.button_listener_add.Text = "+";
            this.button_listener_add.UseVisualStyleBackColor = true;
            this.button_listener_add.Click += new System.EventHandler(this.button_listener_add_Click);
            // 
            // groupBox_listener
            // 
            this.groupBox_listener.Controls.Add(this.listView_listeners);
            this.groupBox_listener.Controls.Add(this.textBox_listener_endpoint);
            this.groupBox_listener.Controls.Add(this.button_listener_add);
            this.groupBox_listener.Location = new System.Drawing.Point(12, 12);
            this.groupBox_listener.Name = "groupBox_listener";
            this.groupBox_listener.Size = new System.Drawing.Size(208, 243);
            this.groupBox_listener.TabIndex = 27;
            this.groupBox_listener.TabStop = false;
            this.groupBox_listener.Text = "Listener";
            // 
            // groupBox_remotes
            // 
            this.groupBox_remotes.Controls.Add(this.button_subscription);
            this.groupBox_remotes.Controls.Add(this.button_remote_add);
            this.groupBox_remotes.Controls.Add(this.tree_remotes);
            this.groupBox_remotes.Location = new System.Drawing.Point(226, 12);
            this.groupBox_remotes.Name = "groupBox_remotes";
            this.groupBox_remotes.Size = new System.Drawing.Size(321, 243);
            this.groupBox_remotes.TabIndex = 28;
            this.groupBox_remotes.TabStop = false;
            this.groupBox_remotes.Text = "Remotes";
            // 
            // button_subscription
            // 
            this.button_subscription.Location = new System.Drawing.Point(6, 216);
            this.button_subscription.Name = "button_subscription";
            this.button_subscription.Size = new System.Drawing.Size(168, 21);
            this.button_subscription.TabIndex = 27;
            this.button_subscription.Text = "Subscription";
            this.button_subscription.UseVisualStyleBackColor = true;
            this.button_subscription.Click += new System.EventHandler(this.button_subscription_Click);
            // 
            // tree_remotes
            // 
            this.tree_remotes.HideSelection = false;
            this.tree_remotes.ImageIndex = 0;
            this.tree_remotes.ImageList = this.imageList_remote;
            this.tree_remotes.Location = new System.Drawing.Point(6, 20);
            this.tree_remotes.Name = "tree_remotes";
            this.tree_remotes.SelectedImageIndex = 0;
            this.tree_remotes.Size = new System.Drawing.Size(309, 190);
            this.tree_remotes.TabIndex = 7;
            this.tree_remotes.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tree_remotes_AfterCheck);
            this.tree_remotes.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tree_remotes_AfterSelect);
            // 
            // checkBox_logging
            // 
            this.checkBox_logging.AutoSize = true;
            this.checkBox_logging.Location = new System.Drawing.Point(12, 261);
            this.checkBox_logging.Name = "checkBox_logging";
            this.checkBox_logging.Size = new System.Drawing.Size(108, 16);
            this.checkBox_logging.TabIndex = 30;
            this.checkBox_logging.Text = "Enable Logging";
            this.checkBox_logging.UseVisualStyleBackColor = true;
            // 
            // textBox_log
            // 
            this.textBox_log.BackColor = System.Drawing.Color.Black;
            this.textBox_log.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_log.ForeColor = System.Drawing.Color.Silver;
            this.textBox_log.Location = new System.Drawing.Point(12, 283);
            this.textBox_log.Multiline = true;
            this.textBox_log.Name = "textBox_log";
            this.textBox_log.Size = new System.Drawing.Size(796, 192);
            this.textBox_log.TabIndex = 31;
            // 
            // trayIcon
            // 
            this.trayIcon.ContextMenuStrip = this.contextMenuStrip_tray;
            this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
            this.trayIcon.Text = "NaiveGUI";
            this.trayIcon.Visible = true;
            this.trayIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.trayIcon_MouseClick);
            // 
            // contextMenuStrip_tray
            // 
            this.contextMenuStrip_tray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator_listeners,
            this.toolStripMenuItem_exit});
            this.contextMenuStrip_tray.Name = "contextMenuStrip_tray";
            this.contextMenuStrip_tray.Size = new System.Drawing.Size(97, 32);
            this.contextMenuStrip_tray.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_tray_Opening);
            // 
            // toolStripSeparator_listeners
            // 
            this.toolStripSeparator_listeners.Name = "toolStripSeparator_listeners";
            this.toolStripSeparator_listeners.Size = new System.Drawing.Size(93, 6);
            // 
            // toolStripMenuItem_exit
            // 
            this.toolStripMenuItem_exit.Name = "toolStripMenuItem_exit";
            this.toolStripMenuItem_exit.Size = new System.Drawing.Size(96, 22);
            this.toolStripMenuItem_exit.Text = "Exit";
            this.toolStripMenuItem_exit.Click += new System.EventHandler(this.toolStripMenuItem_exit_Click);
            // 
            // timer_main
            // 
            this.timer_main.Enabled = true;
            this.timer_main.Interval = 50;
            this.timer_main.Tick += new System.EventHandler(this.timer_main_Tick);
            // 
            // checkBox_autorun
            // 
            this.checkBox_autorun.AutoSize = true;
            this.checkBox_autorun.Location = new System.Drawing.Point(126, 261);
            this.checkBox_autorun.Name = "checkBox_autorun";
            this.checkBox_autorun.Size = new System.Drawing.Size(84, 16);
            this.checkBox_autorun.TabIndex = 32;
            this.checkBox_autorun.Text = "Auto Start";
            this.checkBox_autorun.UseVisualStyleBackColor = true;
            this.checkBox_autorun.CheckedChanged += new System.EventHandler(this.checkBox_autorun_CheckedChanged);
            // 
            // contextMenuStrip_listener
            // 
            this.contextMenuStrip_listener.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.contextMenuStrip_listener.Name = "contextMenuStrip_listener";
            this.contextMenuStrip_listener.Size = new System.Drawing.Size(114, 48);
            this.contextMenuStrip_listener.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_endpoint_Opening);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 487);
            this.Controls.Add(this.checkBox_autorun);
            this.Controls.Add(this.textBox_log);
            this.Controls.Add(this.checkBox_logging);
            this.Controls.Add(this.groupBox_remotes);
            this.Controls.Add(this.groupBox_listener);
            this.Controls.Add(this.groupBox_remote_config);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "Naïve Proxy";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox_remote_config.ResumeLayout(false);
            this.groupBox_remote_config.PerformLayout();
            this.groupBox_listener.ResumeLayout(false);
            this.groupBox_listener.PerformLayout();
            this.groupBox_remotes.ResumeLayout(false);
            this.contextMenuStrip_tray.ResumeLayout(false);
            this.contextMenuStrip_listener.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ColumnHeader columnHeader2;
        internal System.Windows.Forms.TreeView tree_remotes;
        private System.Windows.Forms.TextBox textBox_host;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox_protocol;
        private System.Windows.Forms.TextBox textBox_username;
        private System.Windows.Forms.TextBox textBox_password;
        private System.Windows.Forms.CheckBox checkBox_username;
        private System.Windows.Forms.CheckBox checkBox_password;
        private System.Windows.Forms.CheckBox checkBox_host;
        private System.Windows.Forms.Button button_remote_save;
        private System.Windows.Forms.Button button_remote_discard;
        private System.Windows.Forms.CheckBox checkBox_padding;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox_remote_config;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBox_quic;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_group;
        private System.Windows.Forms.TextBox textBox_name;
        private System.Windows.Forms.Button button_remote_add;
        private System.Windows.Forms.ImageList imageList_remote;
        private System.Windows.Forms.TextBox textBox_listener_endpoint;
        private System.Windows.Forms.Button button_listener_add;
        private System.Windows.Forms.GroupBox groupBox_listener;
        private System.Windows.Forms.ListView listView_listeners;
        private System.Windows.Forms.GroupBox groupBox_remotes;
        internal System.Windows.Forms.CheckBox checkBox_logging;
        internal System.Windows.Forms.TextBox textBox_log;
        private System.Windows.Forms.Button button_remote_delete;
        private System.Windows.Forms.Button button_subscription;
        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_tray;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator_listeners;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_exit;
        private System.Windows.Forms.Timer timer_main;
        internal System.Windows.Forms.CheckBox checkBox_autorun;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_listener;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
    }
}

