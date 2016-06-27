namespace Bitmaprenamer
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

        private System.Windows.Forms.ListView DirList;
        private System.Windows.Forms.PictureBox bitmap;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txt_filename;
        private System.Windows.Forms.Button btn_rename;
    }
}

