using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Bitmaprenamer
{
    public partial class Form1 : Form
    {
        private string path = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.DirList = new System.Windows.Forms.ListView();
            this.bitmap = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.txt_filename = new System.Windows.Forms.TextBox();
            this.btn_rename = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.bitmap)).BeginInit();
            this.SuspendLayout();
            // 
            // DirList
            // 
            this.DirList.Location = new System.Drawing.Point(31, 114);
            this.DirList.MultiSelect = false;
            this.DirList.Name = "DirList";
            this.DirList.Size = new System.Drawing.Size(154, 521);
            this.DirList.TabIndex = 0;
            this.DirList.UseCompatibleStateImageBehavior = false;
            this.DirList.View = System.Windows.Forms.View.Details;
            this.DirList.SelectedIndexChanged += new System.EventHandler(this.DirList_SelectedIndexChanged);
            this.DirList.Click += new System.EventHandler(this.DirList_Click);
            // 
            // bitmap
            // 
            this.bitmap.Location = new System.Drawing.Point(226, 114);
            this.bitmap.Name = "bitmap";
            this.bitmap.Size = new System.Drawing.Size(100, 50);
            this.bitmap.TabIndex = 1;
            this.bitmap.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(55, 24);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(98, 67);
            this.button1.TabIndex = 2;
            this.button1.Text = "Velg mappe";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txt_filename
            // 
            this.txt_filename.Location = new System.Drawing.Point(226, 42);
            this.txt_filename.Name = "txt_filename";
            this.txt_filename.Size = new System.Drawing.Size(275, 31);
            this.txt_filename.TabIndex = 3;
            // 
            // btn_rename
            // 
            this.btn_rename.Location = new System.Drawing.Point(518, 32);
            this.btn_rename.Name = "btn_rename";
            this.btn_rename.Size = new System.Drawing.Size(198, 51);
            this.btn_rename.TabIndex = 4;
            this.btn_rename.Text = "Endre filnavn";
            this.btn_rename.UseVisualStyleBackColor = true;
            this.btn_rename.Click += new System.EventHandler(this.btn_rename_Click);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1049, 766);
            this.Controls.Add(this.btn_rename);
            this.Controls.Add(this.txt_filename);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.bitmap);
            this.Controls.Add(this.DirList);
            this.Name = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.bitmap)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void clearList()
        {
            DirList.Clear();
            ColumnHeader header = new ColumnHeader();
            header.Text = "";
            header.Name = "col1";
            header.Width = DirList.Width;
            DirList.Columns.Add(header);
        }

        private void DirList_Click(object sender, EventArgs e)
        {

            string selected = DirList.SelectedItems[0].Text;
            if (selected.Contains(".PNG") || selected.Contains(".png")) // we chose an bitmap-file
            {
                txt_filename.Text = selected;
                if(bitmap.Image != null)
                    bitmap.Image.Dispose();
                bitmap.Image = new Bitmap(path + "/" + selected);
                bitmap.Height = bitmap.Image.Height;
                bitmap.Width = bitmap.Image.Width;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            DialogResult result = fbd.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                path = fbd.SelectedPath;

                clearList();
                DirectoryInfo dirInfo = new DirectoryInfo(fbd.SelectedPath);
                foreach (FileInfo fi in dirInfo.GetFiles())
                {
                    DirList.Items.Add(fi.ToString());
                }
            }
        }

        private void btn_rename_Click(object sender, EventArgs e)
        {
            if (DirList.SelectedItems.Count > 0)
            {
                string nyttnavn = txt_filename.Text;
                string gammeltnavn = DirList.SelectedItems[0].Text;

                bitmap.Image.Dispose();

                File.Move(path + "\\" + gammeltnavn, path + "\\" + nyttnavn);
                bitmap.Image = new Bitmap(path + "/" + nyttnavn);
                bitmap.Height = bitmap.Image.Height;
                bitmap.Width = bitmap.Image.Width;
                DirList.SelectedItems[0].Text = nyttnavn;
            }
        }

        private void DirList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(DirList.SelectedItems.Count <= 0)
            {
                txt_filename.Text = "";
                if (bitmap.Image != null)
                {
                    bitmap.Image.Dispose();
                    bitmap.Image = null;
                }
            }
        }
    }
}