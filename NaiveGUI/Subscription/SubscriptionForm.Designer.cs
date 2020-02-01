namespace NaiveGUI.Subscription
{
    partial class SubscriptionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ListViewItem listViewItem41 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem42 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem43 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem44 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem45 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem46 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem47 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem48 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem49 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem50 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem51 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem52 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem53 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem54 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem55 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem56 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem57 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem58 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem59 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem60 = new System.Windows.Forms.ListViewItem("");
            this.listView_sub = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button_add = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_url = new System.Windows.Forms.TextBox();
            this.button_save = new System.Windows.Forms.Button();
            this.checkBox_auto_update = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_update = new System.Windows.Forms.Button();
            this.button_save_global = new System.Windows.Forms.Button();
            this.textBox_interval = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView_sub
            // 
            this.listView_sub.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader3});
            this.listView_sub.FullRowSelect = true;
            this.listView_sub.GridLines = true;
            this.listView_sub.HideSelection = false;
            this.listView_sub.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem41,
            listViewItem42,
            listViewItem43,
            listViewItem44,
            listViewItem45,
            listViewItem46,
            listViewItem47,
            listViewItem48,
            listViewItem49,
            listViewItem50,
            listViewItem51,
            listViewItem52,
            listViewItem53,
            listViewItem54,
            listViewItem55,
            listViewItem56,
            listViewItem57,
            listViewItem58,
            listViewItem59,
            listViewItem60});
            this.listView_sub.Location = new System.Drawing.Point(12, 12);
            this.listView_sub.Name = "listView_sub";
            this.listView_sub.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.listView_sub.Size = new System.Drawing.Size(441, 208);
            this.listView_sub.TabIndex = 0;
            this.listView_sub.UseCompatibleStateImageBehavior = false;
            this.listView_sub.View = System.Windows.Forms.View.Details;
            this.listView_sub.SelectedIndexChanged += new System.EventHandler(this.listView_sub_SelectedIndexChanged);
            this.listView_sub.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listView_sub_KeyUp);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "URL";
            this.columnHeader2.Width = 300;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Last Update";
            this.columnHeader3.Width = 120;
            // 
            // button_add
            // 
            this.button_add.Location = new System.Drawing.Point(378, 226);
            this.button_add.Name = "button_add";
            this.button_add.Size = new System.Drawing.Size(75, 21);
            this.button_add.TabIndex = 1;
            this.button_add.Text = "Add";
            this.button_add.UseVisualStyleBackColor = true;
            this.button_add.Click += new System.EventHandler(this.button_add_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 230);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "URL:";
            // 
            // textBox_url
            // 
            this.textBox_url.Location = new System.Drawing.Point(47, 226);
            this.textBox_url.Name = "textBox_url";
            this.textBox_url.Size = new System.Drawing.Size(325, 21);
            this.textBox_url.TabIndex = 2;
            this.textBox_url.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_url_KeyDown);
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(378, 226);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(75, 21);
            this.button_save.TabIndex = 3;
            this.button_save.Text = "Save";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Visible = false;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // checkBox_auto_update
            // 
            this.checkBox_auto_update.AutoSize = true;
            this.checkBox_auto_update.Location = new System.Drawing.Point(6, 22);
            this.checkBox_auto_update.Name = "checkBox_auto_update";
            this.checkBox_auto_update.Size = new System.Drawing.Size(168, 16);
            this.checkBox_auto_update.TabIndex = 0;
            this.checkBox_auto_update.Text = "Auto Update Interval(s):";
            this.checkBox_auto_update.UseVisualStyleBackColor = true;
            this.checkBox_auto_update.CheckedChanged += new System.EventHandler(this.checkBox_auto_update_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_update);
            this.groupBox1.Controls.Add(this.button_save_global);
            this.groupBox1.Controls.Add(this.textBox_interval);
            this.groupBox1.Controls.Add(this.checkBox_auto_update);
            this.groupBox1.Location = new System.Drawing.Point(12, 253);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(441, 47);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Global Config";
            // 
            // button_update
            // 
            this.button_update.Location = new System.Drawing.Point(335, 19);
            this.button_update.Name = "button_update";
            this.button_update.Size = new System.Drawing.Size(100, 21);
            this.button_update.TabIndex = 3;
            this.button_update.Text = "Update Now";
            this.button_update.UseVisualStyleBackColor = true;
            this.button_update.Click += new System.EventHandler(this.button_update_Click);
            // 
            // button_save_global
            // 
            this.button_save_global.Enabled = false;
            this.button_save_global.Location = new System.Drawing.Point(254, 19);
            this.button_save_global.Name = "button_save_global";
            this.button_save_global.Size = new System.Drawing.Size(75, 21);
            this.button_save_global.TabIndex = 2;
            this.button_save_global.Text = "Save";
            this.button_save_global.UseVisualStyleBackColor = true;
            this.button_save_global.Click += new System.EventHandler(this.button_save_global_Click);
            // 
            // textBox_interval
            // 
            this.textBox_interval.Enabled = false;
            this.textBox_interval.Location = new System.Drawing.Point(180, 20);
            this.textBox_interval.Name = "textBox_interval";
            this.textBox_interval.Size = new System.Drawing.Size(68, 21);
            this.textBox_interval.TabIndex = 1;
            this.textBox_interval.Text = "-1";
            this.textBox_interval.TextChanged += new System.EventHandler(this.textBox_interval_TextChanged);
            // 
            // SubscriptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(465, 312);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.textBox_url);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_add);
            this.Controls.Add(this.listView_sub);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SubscriptionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Subscriptions";
            this.Load += new System.EventHandler(this.SubscriptionForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView_sub;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Button button_add;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_url;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.CheckBox checkBox_auto_update;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox_interval;
        private System.Windows.Forms.Button button_save_global;
        private System.Windows.Forms.Button button_update;
    }
}