namespace WarrantyItemScan
{
    partial class Form1
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
            if (disposing && (components != null))
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
            this.bottom = new System.Windows.Forms.Panel();
            this.pnl_txtScan = new System.Windows.Forms.Panel();
            this.tbx_txtScan = new System.Windows.Forms.TextBox();
            this.pnl_confirm = new System.Windows.Forms.Panel();
            this.btn_clear = new System.Windows.Forms.Button();
            this.btn_confirm = new System.Windows.Forms.Button();
            this.dgv_mWarranty = new System.Windows.Forms.DataGrid();
            this.menubar = new System.Windows.Forms.Panel();
            this.btn_exit = new System.Windows.Forms.Button();
            this.head = new System.Windows.Forms.Panel();
            this.pnl_selDO = new System.Windows.Forms.Panel();
            this.cbx_selDO = new System.Windows.Forms.ComboBox();
            this.pnl_item = new System.Windows.Forms.Panel();
            this.cbx_item = new System.Windows.Forms.ComboBox();
            this.pnl_refresh = new System.Windows.Forms.Panel();
            this.btn_refresh = new System.Windows.Forms.Button();
            this.main = new System.Windows.Forms.Panel();
            this.bottom.SuspendLayout();
            this.pnl_txtScan.SuspendLayout();
            this.pnl_confirm.SuspendLayout();
            this.menubar.SuspendLayout();
            this.head.SuspendLayout();
            this.pnl_selDO.SuspendLayout();
            this.pnl_item.SuspendLayout();
            this.pnl_refresh.SuspendLayout();
            this.main.SuspendLayout();
            this.SuspendLayout();
            // 
            // bottom
            // 
            this.bottom.Controls.Add(this.pnl_txtScan);
            this.bottom.Controls.Add(this.pnl_confirm);
            this.bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottom.Location = new System.Drawing.Point(0, 473);
            this.bottom.Name = "bottom";
            this.bottom.Size = new System.Drawing.Size(480, 115);
            // 
            // pnl_txtScan
            // 
            this.pnl_txtScan.Controls.Add(this.tbx_txtScan);
            this.pnl_txtScan.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnl_txtScan.Location = new System.Drawing.Point(0, 0);
            this.pnl_txtScan.Name = "pnl_txtScan";
            this.pnl_txtScan.Size = new System.Drawing.Size(480, 58);
            // 
            // tbx_txtScan
            // 
            this.tbx_txtScan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbx_txtScan.Font = new System.Drawing.Font("Tahoma", 28F, System.Drawing.FontStyle.Regular);
            this.tbx_txtScan.Location = new System.Drawing.Point(0, 0);
            this.tbx_txtScan.Name = "tbx_txtScan";
            this.tbx_txtScan.Size = new System.Drawing.Size(480, 52);
            this.tbx_txtScan.TabIndex = 0;
            this.tbx_txtScan.TextChanged += new System.EventHandler(this.tbx_txtScan_TextChanged);
            this.tbx_txtScan.GotFocus += new System.EventHandler(this.tbx_txtScan_GotFocus);
            this.tbx_txtScan.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbx_txtScan_KeyDown);
            // 
            // pnl_confirm
            // 
            this.pnl_confirm.Controls.Add(this.btn_clear);
            this.pnl_confirm.Controls.Add(this.btn_confirm);
            this.pnl_confirm.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnl_confirm.Location = new System.Drawing.Point(0, 59);
            this.pnl_confirm.Name = "pnl_confirm";
            this.pnl_confirm.Size = new System.Drawing.Size(480, 56);
            // 
            // btn_clear
            // 
            this.btn_clear.BackColor = System.Drawing.Color.DarkOrange;
            this.btn_clear.Dock = System.Windows.Forms.DockStyle.Left;
            this.btn_clear.Enabled = false;
            this.btn_clear.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold);
            this.btn_clear.ForeColor = System.Drawing.Color.White;
            this.btn_clear.Location = new System.Drawing.Point(0, 0);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(239, 56);
            this.btn_clear.TabIndex = 0;
            this.btn_clear.Text = "Clear Text";
            this.btn_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // btn_confirm
            // 
            this.btn_confirm.BackColor = System.Drawing.Color.Green;
            this.btn_confirm.Dock = System.Windows.Forms.DockStyle.Right;
            this.btn_confirm.Enabled = false;
            this.btn_confirm.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold);
            this.btn_confirm.ForeColor = System.Drawing.Color.White;
            this.btn_confirm.Location = new System.Drawing.Point(241, 0);
            this.btn_confirm.Name = "btn_confirm";
            this.btn_confirm.Size = new System.Drawing.Size(239, 56);
            this.btn_confirm.TabIndex = 12;
            this.btn_confirm.Text = "Confirm To Save";
            this.btn_confirm.Click += new System.EventHandler(this.btn_confirm_Click);
            // 
            // dgv_mWarranty
            // 
            this.dgv_mWarranty.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dgv_mWarranty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_mWarranty.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular);
            this.dgv_mWarranty.Location = new System.Drawing.Point(0, 0);
            this.dgv_mWarranty.Name = "dgv_mWarranty";
            this.dgv_mWarranty.RowHeadersVisible = false;
            this.dgv_mWarranty.Size = new System.Drawing.Size(480, 312);
            this.dgv_mWarranty.TabIndex = 9;
            this.dgv_mWarranty.Paint += new System.Windows.Forms.PaintEventHandler(this.dgv_mWarranty_Paint);
            this.dgv_mWarranty.CurrentCellChanged += new System.EventHandler(this.dgv_mWarranty_CurrentCellChanged);
            this.dgv_mWarranty.Click += new System.EventHandler(this.dgv_mWarranty_Click);
            // 
            // menubar
            // 
            this.menubar.Controls.Add(this.btn_exit);
            this.menubar.Dock = System.Windows.Forms.DockStyle.Top;
            this.menubar.Location = new System.Drawing.Point(0, 0);
            this.menubar.Name = "menubar";
            this.menubar.Size = new System.Drawing.Size(480, 40);
            // 
            // btn_exit
            // 
            this.btn_exit.BackColor = System.Drawing.Color.Crimson;
            this.btn_exit.Dock = System.Windows.Forms.DockStyle.Right;
            this.btn_exit.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Regular);
            this.btn_exit.ForeColor = System.Drawing.Color.White;
            this.btn_exit.Location = new System.Drawing.Point(326, 0);
            this.btn_exit.Name = "btn_exit";
            this.btn_exit.Size = new System.Drawing.Size(154, 40);
            this.btn_exit.TabIndex = 0;
            this.btn_exit.Text = "Exit";
            this.btn_exit.Click += new System.EventHandler(this.btn_exit_Click);
            // 
            // head
            // 
            this.head.Controls.Add(this.pnl_selDO);
            this.head.Controls.Add(this.pnl_item);
            this.head.Controls.Add(this.pnl_refresh);
            this.head.Dock = System.Windows.Forms.DockStyle.Top;
            this.head.Location = new System.Drawing.Point(0, 40);
            this.head.Name = "head";
            this.head.Size = new System.Drawing.Size(480, 142);
            // 
            // pnl_selDO
            // 
            this.pnl_selDO.Controls.Add(this.cbx_selDO);
            this.pnl_selDO.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnl_selDO.Location = new System.Drawing.Point(0, 49);
            this.pnl_selDO.Name = "pnl_selDO";
            this.pnl_selDO.Size = new System.Drawing.Size(480, 43);
            // 
            // cbx_selDO
            // 
            this.cbx_selDO.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.cbx_selDO.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular);
            this.cbx_selDO.Location = new System.Drawing.Point(0, -2);
            this.cbx_selDO.Name = "cbx_selDO";
            this.cbx_selDO.Size = new System.Drawing.Size(480, 45);
            this.cbx_selDO.TabIndex = 3;
            this.cbx_selDO.SelectedIndexChanged += new System.EventHandler(this.cbx_selDO_SelectedIndexChanged);
            // 
            // pnl_item
            // 
            this.pnl_item.Controls.Add(this.cbx_item);
            this.pnl_item.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnl_item.Location = new System.Drawing.Point(0, 92);
            this.pnl_item.Name = "pnl_item";
            this.pnl_item.Size = new System.Drawing.Size(480, 50);
            // 
            // cbx_item
            // 
            this.cbx_item.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.cbx_item.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular);
            this.cbx_item.Location = new System.Drawing.Point(0, 5);
            this.cbx_item.Name = "cbx_item";
            this.cbx_item.Size = new System.Drawing.Size(480, 45);
            this.cbx_item.TabIndex = 0;
            this.cbx_item.SelectedIndexChanged += new System.EventHandler(this.cbx_item_SelectedIndexChanged);
            // 
            // pnl_refresh
            // 
            this.pnl_refresh.Controls.Add(this.btn_refresh);
            this.pnl_refresh.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnl_refresh.Location = new System.Drawing.Point(0, 0);
            this.pnl_refresh.Name = "pnl_refresh";
            this.pnl_refresh.Size = new System.Drawing.Size(480, 46);
            // 
            // btn_refresh
            // 
            this.btn_refresh.BackColor = System.Drawing.Color.Green;
            this.btn_refresh.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_refresh.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold);
            this.btn_refresh.ForeColor = System.Drawing.Color.White;
            this.btn_refresh.Location = new System.Drawing.Point(0, 0);
            this.btn_refresh.Name = "btn_refresh";
            this.btn_refresh.Size = new System.Drawing.Size(480, 46);
            this.btn_refresh.TabIndex = 2;
            this.btn_refresh.Text = "Refresh Data";
            this.btn_refresh.Click += new System.EventHandler(this.btn_refresh_Click);
            // 
            // main
            // 
            this.main.Controls.Add(this.dgv_mWarranty);
            this.main.Dock = System.Windows.Forms.DockStyle.Top;
            this.main.Location = new System.Drawing.Point(0, 182);
            this.main.Name = "main";
            this.main.Size = new System.Drawing.Size(480, 312);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(480, 588);
            this.Controls.Add(this.main);
            this.Controls.Add(this.head);
            this.Controls.Add(this.menubar);
            this.Controls.Add(this.bottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.Text = " ";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.bottom.ResumeLayout(false);
            this.pnl_txtScan.ResumeLayout(false);
            this.pnl_confirm.ResumeLayout(false);
            this.menubar.ResumeLayout(false);
            this.head.ResumeLayout(false);
            this.pnl_selDO.ResumeLayout(false);
            this.pnl_item.ResumeLayout(false);
            this.pnl_refresh.ResumeLayout(false);
            this.main.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel bottom;
        private System.Windows.Forms.DataGrid dgv_mWarranty;
        private System.Windows.Forms.Panel pnl_confirm;
        private System.Windows.Forms.Button btn_clear;
        private System.Windows.Forms.Panel pnl_txtScan;
        private System.Windows.Forms.TextBox tbx_txtScan;
        private System.Windows.Forms.Button btn_confirm;
        private System.Windows.Forms.Panel menubar;
        private System.Windows.Forms.Button btn_exit;
        private System.Windows.Forms.Panel head;
        private System.Windows.Forms.Panel pnl_selDO;
        private System.Windows.Forms.ComboBox cbx_selDO;
        private System.Windows.Forms.Panel pnl_item;
        private System.Windows.Forms.ComboBox cbx_item;
        private System.Windows.Forms.Panel pnl_refresh;
        private System.Windows.Forms.Button btn_refresh;
        private System.Windows.Forms.Panel main;


    }
}

