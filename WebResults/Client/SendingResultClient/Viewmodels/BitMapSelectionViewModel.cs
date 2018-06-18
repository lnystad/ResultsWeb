using FileUploaderService;
using FileUploaderService.Configuration;
using FileUploaderService.Utils;
using Microsoft.Practices.Prism.Commands;
using SendingResultClient.Viewmodels.BitMap;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SendingResultClient.Viewmodels
{

    public class BitMapSelectionViewModel : ViewModelBase
    {
        private object dummyNode = null;
        public BitMapSelectionViewModel()
        {
            m_remote15m = ConfigurationLoader.GetAppSettingsValue("RemoteBitMapDir15m");
            m_remote100m = ConfigurationLoader.GetAppSettingsValue("RemoteBitMapDir100m");
            m_remoteFinFelt = ConfigurationLoader.GetAppSettingsValue("RemoteBitMapDirFinFelt");
            string RemoteBitMapDir200m = ConfigurationLoader.GetAppSettingsValue("RemoteBitMapDir200m");
            string RemoteBitMapDirGrovFelt = ConfigurationLoader.GetAppSettingsValue("RemoteBitMapDir200m");
            m_OrionDirs = new ObservableCollection<string>();
            if(!string.IsNullOrEmpty(m_remote15m))
            {
                m_OrionDirs.Add(m_remote15m);
                m_SelectedOrionDir = m_remote15m;
                m_Is15Checked = true;
            }
            if (!string.IsNullOrEmpty(m_remote100m))
            {
                m_OrionDirs.Clear();
                m_OrionDirs.Add(m_remote100m);
                m_SelectedOrionDir = m_remote100m;
                m_Is15Checked = false;
                m_Is100Checked = true;
            }
            if (!string.IsNullOrEmpty(m_remoteFinFelt))
            {
                m_OrionDirs.Clear();
                m_OrionDirs.Add(m_remoteFinFelt);
                m_SelectedOrionDir = m_remoteFinFelt;
                m_Is15Checked = false;
                m_Is100Checked = false;
                m_IsFinFeltChecked = true;
            }

            m_remoteGrovFelt = new List<string>();
            if (!string.IsNullOrEmpty(RemoteBitMapDirGrovFelt))
            {
                m_remoteGrovFelt = RemoteBitMapDirGrovFelt.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                m_OrionDirs.Clear();
                foreach (var el in m_remoteGrovFelt)
                {
                    m_OrionDirs.Add(el.Trim());
                }
                m_SelectedOrionDir = m_OrionDirs[0];
                m_Is15Checked = false;
                m_Is100Checked = false;
                m_IsFinFeltChecked = false;
                m_IsGrovFeltChecked = true;
            }

            m_remote200m = new List<string>();
            if (!string.IsNullOrEmpty(RemoteBitMapDir200m))
            {
                m_remote200m = RemoteBitMapDir200m.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                m_OrionDirs.Clear();
                foreach (var el in m_remote200m)
                {
                    m_OrionDirs.Add(el.Trim());
                }
                m_SelectedOrionDir = m_OrionDirs[0];
                m_Is15Checked = false;
                m_Is100Checked = false;
                m_IsFinFeltChecked = false;
                m_IsGrovFeltChecked = false;
                m_Is200Checked = true;
            }
            


            


            m_UploadBitmap = false;
            string UploadBitmapXslt = ConfigurationLoader.GetAppSettingsValue("UploadBitmap");
            if (!string.IsNullOrEmpty(UploadBitmapXslt))
            {
                bool val;
                if (bool.TryParse(UploadBitmapXslt, out val))
                {
                    m_UploadBitmap = val;
                }
            }
            FoldersItems = new ObservableCollection<TreeViewItem>();
            string path=string.Empty;
            InitCommands();
            Init(m_SelectedOrionDir);
        }
        internal TreeViewItem FindDirTreeView(RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeViewItem;
            if (item == null)
            {
                return null;
            }
            if (item.Tag.GetType() == typeof(DirectoryInfo))
            {
                return item;
            }
            if (item.Tag.GetType() == typeof(FileInfo))
            {
                var parent = FindAncestor<TreeViewItem>(item);
                return parent;
            }

            return null;
        }
        internal void ExportFolder_Click(object sender, RoutedEventArgs e)
        {
            var item = m_DirectoryItem;
            if(item == null)
            {
                return;
            }
            string prefix = string.Empty;
            if (m_Is15Checked)
            {
                prefix = Constants.Prefix15m;
            }
            if (m_Is100Checked)
            {
                prefix = Constants.Prefix100m;
            }
            if (m_IsFinFeltChecked)
            {
                prefix = Constants.PrefixFinFelt;
            }
            if (m_Is200Checked)
            {
                prefix = Constants.Prefix200m;
            }
            if (m_IsGrovFeltChecked)
            {
                prefix = Constants.PrefixGrovFelt;
            }
            if (string.IsNullOrEmpty(m_StevneDir) ||
               string.IsNullOrEmpty(m_StevneNavn))
            {
                return;
            }
            string ToDir = Path.Combine(m_StevneDir, m_StevneNavn);
            ToDir = Path.Combine(ToDir, "BitMap");
            ToDir = Path.Combine(ToDir, prefix);
            if (!Directory.Exists(ToDir))
            {
                Directory.CreateDirectory(ToDir);
            }
            if (m_DirectoryItem == null)
            {
                return;
            }

            string startDir = ((DirectoryInfo)m_DirectoryItem.Tag).FullName;

            List<FileInfo> fileList = GetFiles(startDir);
            FileAccessHelper.MoveFiles(fileList, ToDir);
            m_DirectoryItem.Items.Clear();
            FoldersItems.Clear();
            Init(m_SelectedOrionDir);
            this.OnPropertyChanged("FoldersItems");
        }

        internal void DeleteFolder_Click(object sender, RoutedEventArgs e)
        {
            var item = m_DirectoryItem;
            if (item == null)
            {
                return;
            }

            string startDir = ((DirectoryInfo)m_DirectoryItem.Tag).FullName;

            
                if (Directory.Exists(startDir))
                {
                    try
                    {
                        if(System.Windows.Forms.MessageBox.Show(startDir, "Slett Katalog", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)== DialogResult.OK);
                        {
                            Directory.Delete(startDir);
                           FoldersItems.Clear();
                        Init(m_SelectedOrionDir);
                            this.OnPropertyChanged("FoldersItems");
                    }
                    }
                    catch(Exception ee)
                    {
                        string medd = ee.Message;
                        System.Windows.Forms.MessageBox.Show(medd, "Feil", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
            
        }

        internal void TreeViewItem_OnItemSelected(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeViewItem;
            m_DirectoryItem = null;
            if (item==null)
            {

            }
            if (item.Tag.GetType() == typeof(DirectoryInfo))
            {
                m_DirectoryItem = item;
            }
            if (item.Tag.GetType() == typeof(FileInfo))
            {
                var parent = FindAncestor<TreeViewItem>(item);
                m_DirectoryItem = parent;

                DisplayedImagePath = ((FileInfo)item.Tag).FullName;
                if (File.Exists(DisplayedImagePath))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(DisplayedImagePath, UriKind.Absolute);
                    bitmap.EndInit();
                    Image = bitmap;
                }
            }
        }
        public static T FindAncestor<T>(DependencyObject current)
    where T : DependencyObject
        {
            current = VisualTreeHelper.GetParent(current);

            while (current != null)
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            };
            return null;
        }

        private string m_StevneDir;
        public string StevneDir
        {
            get
            {
                return this.m_StevneDir;
            }
            set
            {
                this.m_StevneDir = value;
                this.OnPropertyChanged("StevneDir");
            }
        }

        private string m_StevneNavn;
        public string StevneNavn
        {
            get
            {
                return this.m_StevneNavn;
            }
            set
            {
                this.m_StevneNavn = value;
                this.OnPropertyChanged("StevneNavn");
            }
        }

        internal void HandleStevneChange(string stevneNavn, string path)
        {
            m_StevneDir = path;
            m_StevneNavn = stevneNavn;
            
        }

        private DelegateCommand m_MoveBitmapCommand;
        public DelegateCommand MoveBitmapCommand { get; private set; }
        private void InitCommands()
        {
            m_canExecute = true;
            MoveBitmapCommand = new DelegateCommand(OkExecute, OkCanExecute);

        }
        private bool m_canExecute;

        private bool OkCanExecute()
        {
            return m_canExecute;
        }

        private void OkExecute()
        {
            UploadSelectedBitmap();
        }

        private void UploadSelectedBitmap()
        {
            string prefix = string.Empty;
            if(m_Is15Checked)
            {
                prefix = Constants.Prefix15m;
            }
            if (m_Is100Checked)
            {
                prefix = Constants.Prefix100m;
            }
            if (m_IsFinFeltChecked)
            {
                prefix = Constants.PrefixFinFelt;
            }
            if (m_Is200Checked)
            {
                prefix = Constants.Prefix200m;
            }
            if (m_IsGrovFeltChecked)
            {
                prefix = Constants.PrefixGrovFelt;
            }
            if(string.IsNullOrEmpty(m_StevneDir)||
               string.IsNullOrEmpty(m_StevneNavn))
            {
                return;
            }
            string ToDir = Path.Combine(m_StevneDir, m_StevneNavn);
             ToDir = Path.Combine(ToDir, "BitMap");
            ToDir = Path.Combine(ToDir, prefix);
            if(!Directory.Exists(ToDir))
            {
                Directory.CreateDirectory(ToDir);
            }
            if(m_DirectoryItem == null)
            {
                return;
            }

            string startDir = ((DirectoryInfo)m_DirectoryItem.Tag).FullName;
            
            List<FileInfo> fileList=GetFiles(startDir);
            FileAccessHelper.MoveFiles(fileList, ToDir);
           
        }

        private List<FileInfo> GetFiles(string startDir)
        {
            List<FileInfo> list = new List<FileInfo>();
            DirectoryInfo inf = new DirectoryInfo(startDir);
            var dirName = Path.GetFileName(inf.FullName);
            if(dirName.StartsWith("Lag"))
            {
                var fileList = Directory.GetFiles(inf.FullName, "*.png", SearchOption.TopDirectoryOnly).ToList();
                foreach(var filename in fileList)
                {
                    list.Add(new FileInfo(filename));
                }
            }
            else
            {
                var dirlist = inf.GetDirectories();
                foreach(var direl in dirlist)
                {
                    var listofFiles = GetFiles(direl.FullName);
                    list.AddRange(listofFiles);
                }
            }
            return list;
        }

        //prate List<TreeViewItem> FindList(TreeViewItem itemInput)
        //{
        //    if(itemInput==null)
        //    {
        //        return null;
        //    }

        //    if(itemInput.Tag.GetType() == typeof(FileInfo))
        //    {
        //        return new List<TreeViewItem>() { itemInput };
        //    }
        //    if(itemInput.Items?.Count>0)
        //    {
        //        List<TreeViewItem> list = new List<TreeViewItem>();
        //        foreach (TreeViewItem item in itemInput.Items)
        //        {
        //            var outputList = FindList(item);
        //            if (outputList != null)
        //            {
        //                list.AddRange(outputList);
        //            }
        //        }
        //        return list;
        //    }
        //    return null;
        //}

        private string m_remote15m;
        private string m_remote100m;
        private string m_remoteFinFelt;
        private List<string> m_remote200m;
        private List<string> m_remoteGrovFelt;
        private bool m_Is15Checked { get; set; }
        public bool Is15Checked
        {
            get
            {
                return this.m_Is15Checked;
            }
            set
            {
                m_Is15Checked = value;
                m_Is200Checked = !m_Is15Checked;
                m_IsGrovFeltChecked = !m_Is15Checked;
                m_Is100Checked = !m_Is15Checked;
                m_IsFinFeltChecked = !m_Is15Checked;
                m_OrionDirs.Clear();
                m_SelectedOrionDir = null;
                if (string.IsNullOrEmpty(m_remote15m))
                {
                    m_OrionDirs.Add(m_remote15m);
                    m_SelectedOrionDir = m_OrionDirs[0];
                }

                OnPropertyChanged("Is15Checked");
                OnPropertyChanged("Is100Checked");
                OnPropertyChanged("IsFinFeltChecked");
                OnPropertyChanged("Is200Checked");
                OnPropertyChanged("IsGrovFeltChecked");
                OnPropertyChanged("OrionDirs");
                OnPropertyChanged("SelectedOrionDir");
            }
        }
        private bool m_Is200Checked { get; set; }
        public bool Is200Checked
        {
            get
            {
                return this.m_Is200Checked;
            }
            set
            {
                m_Is200Checked = value;
                m_Is15Checked = !m_Is200Checked;
                m_Is100Checked = !m_Is200Checked;
                m_IsGrovFeltChecked = !m_Is200Checked;
                m_IsFinFeltChecked = !m_Is200Checked;

                m_OrionDirs.Clear();
                m_SelectedOrionDir = null;
                if (m_remote200m.Count>0)
                {
                    foreach (var ewl in m_remote200m)
                    {
                        m_OrionDirs.Add(ewl);
                    }
                    m_SelectedOrionDir = m_OrionDirs[0];
                }
                OnPropertyChanged("Is15Checked");
                OnPropertyChanged("Is100Checked");
                OnPropertyChanged("IsFinFeltChecked");
                OnPropertyChanged("Is200Checked");
                OnPropertyChanged("IsGrovFeltChecked");
                OnPropertyChanged("OrionDirs");
                OnPropertyChanged("SelectedOrionDir");
            }
        }
        private bool m_IsGrovFeltChecked { get; set; }
        public bool IsGrovFeltChecked
        {
            get
            {
                return this.m_IsGrovFeltChecked;
            }
            set
            {
                m_IsGrovFeltChecked = value;
                m_Is15Checked = !m_IsGrovFeltChecked;
                m_Is100Checked = !m_IsGrovFeltChecked;
                m_Is200Checked = !m_IsGrovFeltChecked;
                m_IsFinFeltChecked = !m_IsGrovFeltChecked;

                m_OrionDirs.Clear();
                m_SelectedOrionDir = null;
                if (m_remoteGrovFelt.Count > 0)
                {
                    foreach (var ewl in m_remoteGrovFelt)
                    {
                        m_OrionDirs.Add(ewl);
                    }
                    m_SelectedOrionDir = m_OrionDirs[0];
                }
                OnPropertyChanged("Is15Checked");
                OnPropertyChanged("Is100Checked");
                OnPropertyChanged("IsFinFeltChecked");
                OnPropertyChanged("Is200Checked");
                OnPropertyChanged("IsGrovFeltChecked");
                OnPropertyChanged("OrionDirs");
                OnPropertyChanged("SelectedOrionDir");
            }
        }
        private bool m_Is100Checked { get; set; }
        public bool Is100Checked
        {
            get
            {
                return this.m_Is100Checked;
            }
            set
            {
                m_Is100Checked = value;
                m_Is15Checked = !m_Is100Checked;
                m_Is200Checked = !m_Is100Checked;
                m_IsGrovFeltChecked = !m_Is100Checked;
                m_IsFinFeltChecked = !m_Is100Checked;



                m_OrionDirs.Clear();
                m_SelectedOrionDir = null;
                if (string.IsNullOrEmpty(m_remote100m))
                {
                    m_OrionDirs.Add(m_remote100m);
                    m_SelectedOrionDir = m_OrionDirs[0];
                }
                OnPropertyChanged("Is15Checked");
                OnPropertyChanged("Is100Checked");
                OnPropertyChanged("IsFinFeltChecked");
                OnPropertyChanged("Is200Checked");
                OnPropertyChanged("IsGrovFeltChecked");
                OnPropertyChanged("OrionDirs");
                OnPropertyChanged("SelectedOrionDir");
            }
        }
        private bool m_IsFinFeltChecked { get; set; }
        public bool IsFinFeltChecked
        {
            get
            {
                return this.m_IsFinFeltChecked;
            }
            set
            {
                m_IsFinFeltChecked = value;
                m_Is15Checked = !m_IsFinFeltChecked;
                m_Is200Checked = !m_IsFinFeltChecked;
                m_IsGrovFeltChecked = !m_IsFinFeltChecked;
                m_Is100Checked = !m_IsFinFeltChecked;
                m_OrionDirs.Clear();
                m_SelectedOrionDir = null;
                if (string.IsNullOrEmpty(m_remoteFinFelt))
                {
                    m_OrionDirs.Add(m_remoteFinFelt);
                    m_SelectedOrionDir = m_OrionDirs[0];
                }
                OnPropertyChanged("Is15Checked");
                OnPropertyChanged("Is100Checked");
                OnPropertyChanged("IsFinFeltChecked");
                OnPropertyChanged("Is200Checked");
                OnPropertyChanged("IsGrovFeltChecked");
                OnPropertyChanged("OrionDirs");
                OnPropertyChanged("SelectedOrionDir");
            }
        }

        private string m_DisplayedImagePath;
        public string DisplayedImagePath
        {
            get
            {
                return this.m_DisplayedImagePath;
            }
            set
            {
                this.m_DisplayedImagePath = value;

                this.OnPropertyChanged("DisplayedImagePath");
            }
        }

        private BitmapImage m_Image;
        public BitmapImage Image
        {
            get
            {
                return this.m_Image;
            }
            set
            {
                this.m_Image = value;

                this.OnPropertyChanged("Image");
            }
        }
        private bool m_UploadBitmap { get; set; }
        public bool UploadBitmap
        {
            get
            {
                return this.m_UploadBitmap;
            }
            set
            {
                this.m_UploadBitmap = value;

                this.OnPropertyChanged("UploadBitmap");
            }
        }

        private string m_SelectedOrionDir;
        public string SelectedOrionDir
        {
            get
            {
                return m_SelectedOrionDir;
            }
            set
            {
                m_SelectedOrionDir = value;
                this.OnPropertyChanged("SelectedOrionDir");
            }
        }
        private ObservableCollection<string> m_OrionDirs;

        public ObservableCollection<String> OrionDirs
        {
            get
            {
                return m_OrionDirs;
            }
            set
            {
                m_OrionDirs = value;
                this.OnPropertyChanged("OrionDirs");
            }
        }

        public string SelectedImagePath { get; set; }

        private ObservableCollection<TreeViewItem> m_FoldersItems;

        public ObservableCollection<TreeViewItem> FoldersItems
        {
            get
            {
                return m_FoldersItems;
            }
            set
            {
                m_FoldersItems = value;
                this.OnPropertyChanged("FoldersItems");
            }
        }

        private TreeViewItem m_DirectoryItem;

        private void Init(string startPath)
        {
            if(string.IsNullOrEmpty(startPath))
            {
                return;
            }

            if (!Directory.Exists(startPath))
                {
                return;
                }
            m_DirectoryItem = null;
            FoldersItems.Clear();
            //foreach (string s in Directory.GetLogicalDrives())
            //{
                TreeViewItem item = new TreeViewItem();
                item.Header = startPath;
                item.Tag = new DirectoryInfo(startPath);
                item.FontWeight = FontWeights.Normal;
                item.Items.Add(dummyNode);
                item.Expanded += new RoutedEventHandler(folder_Expanded);
                //item.Selected += new RoutedEventHandler(Dir_Selected);
            FoldersItems.Add(item);
            //}
        }

       

        void folder_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if (item.Items.Count == 1 && item.Items[0] == dummyNode)
            {
                item.Items.Clear();
                try
                {

                    if (item.Tag.GetType() == typeof(DirectoryInfo))
                    {
                        var dirList = Directory.GetDirectories(item.Tag.ToString()).ToList();
                        if (dirList.Count > 0)
                        {
                            foreach (string s in dirList)
                            {
                                TreeViewItem subitem = new TreeViewItem();
                                subitem.Header = s.Substring(s.LastIndexOf("\\") + 1);
                                subitem.Tag = new DirectoryInfo(s);
                                subitem.FontWeight = FontWeights.Normal;
                                subitem.Items.Add(dummyNode);
                                subitem.Expanded += new RoutedEventHandler(folder_Expanded);
                                //item.Selected += new RoutedEventHandler(Dir_Selected);
                                item.Items.Add(subitem);
                            }
                        }
                        else
                        {
                            string path = item.Tag.ToString();
                            var fileList = Directory.GetFiles(path, "*.png", SearchOption.TopDirectoryOnly).ToList();
                            if (fileList.Count > 0 )
                            {
                                foreach (string s in fileList)
                                {
                                    TreeViewItem subitem = new TreeViewItem();
                                    subitem.Header = Path.GetFileName(s);
                                    subitem.Tag = new FileInfo(s);
                                    subitem.FontWeight = FontWeights.Normal;
                                    subitem.Items.Add(dummyNode);
                                    //subitem.Selected += new RoutedEventHandler(File_Selected);
                                    item.Items.Add(subitem);
                                }
                            }
                        }
                    }
                    
                }
                catch (Exception) { }
            }
        }

       

        public void foldersItem_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            System.Windows.Controls.TreeView tree = (System.Windows.Controls.TreeView)sender;
            TreeViewItem temp = ((TreeViewItem)tree.SelectedItem);

            if (temp == null)
                return;
            SelectedImagePath = "";
            string temp1 = "";
            string temp2 = "";
            while (true)
            {
                temp1 = temp.Header.ToString();
                if (temp1.Contains(@"\"))
                {
                    temp2 = "";
                }
                SelectedImagePath = temp1 + temp2 + SelectedImagePath;
                if (temp.Parent.GetType().Equals(typeof(System.Windows.Controls.TreeView)))
                {
                    break;
                }
                temp = ((TreeViewItem)temp.Parent);
                temp2 = @"\";
            }
            //show user selected path
            System.Windows.Forms.MessageBox.Show(SelectedImagePath);
        }

    }
}
   
