using FileUploaderService;
using FileUploaderService.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.MessageBox;

namespace WebResultsClient.Viewmodels
{

    public class BitMapSelectionViewModel : ViewModelBase
    {
        private readonly IConfiguration m_configuration;

        private object dummyNode = null;
        public BitMapSelectionViewModel(IConfiguration configuration, string selectedcompetition, string selectedPath)
        {
            m_configuration = configuration;

            m_remote15m = m_configuration["RemoteBitMapDir15m"];
            m_remote100m = m_configuration["RemoteBitMapDir100m"];
            m_remoteFinFelt = m_configuration["RemoteBitMapDirFinFelt"];
            string RemoteBitMapDir200m = m_configuration["RemoteBitMapDir200m"];
            string RemoteBitMapDirGrovFelt = m_configuration["RemoteBitMapDirGrovFelt"];

            

            m_OrionDirs = new ObservableCollection<string>();
            

            m_remoteGrovFelt = new List<string>();
            if (!string.IsNullOrEmpty(RemoteBitMapDirGrovFelt))
            {
                m_remoteGrovFelt = RemoteBitMapDirGrovFelt.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            }

            m_remote200m = new List<string>();
            if (!string.IsNullOrEmpty(RemoteBitMapDir200m))
            {
                m_remote200m = RemoteBitMapDir200m.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            }

            m_UploadBitmap = false;
            string UploadBitmapXslt = m_configuration["UploadBitmap"];
            if (!string.IsNullOrEmpty(UploadBitmapXslt))
            {
                bool val;
                if (bool.TryParse(UploadBitmapXslt, out val))
                {
                    m_UploadBitmap = val;
                }
            }
            FoldersItems = new ObservableCollection<TreeViewItem>();
            string path = string.Empty;
            InitCommands();
            m_Is100Checked = false;
            m_IsFinFeltChecked = false;
            m_IsGrovFeltChecked = false;
            m_Is200Checked = false;
            m_Is15Checked = false;
            string LastStevne = m_configuration["LastStevneType"];
            if (!string.IsNullOrEmpty(LastStevne))
            {
                

                switch (LastStevne)
                {
                    case Constants.Prefix15m:
                        Is15Checked = true;
                        break;
                    case Constants.Prefix100m:
                        Is100Checked = true;
                        break;
                    case Constants.Prefix200m:
                        Is200Checked = true;
                        break;
                    case Constants.PrefixOnly200m:
                        Is200Checked = true;
                        break;
                    case Constants.PrefixOnly300m:
                        Is200Checked = true;
                        break;
                    case Constants.PrefixFinFelt:
                        IsFinFeltChecked = true;
                        break;
                    case Constants.PrefixGrovFelt:
                        IsGrovFeltChecked = true;
                        break;
                }
            }
            else
            {
                Is15Checked = true;
                
            }
            m_StevneDir = selectedPath;
            m_StevneNavn = selectedcompetition;
        }
        internal TreeViewItem FindDirTreeView(RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeViewItem;
            if (item == null)
            {
                return null;
            }

            if (item.Tag == null)
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

        public Visibility IsElementFolderVisibility {
            get
            {
                var item = m_SelectedItem;
                if (item == null)
                {
                    return Visibility.Collapsed;
                }

                if (item.Tag.GetType() == typeof(DirectoryInfo))
                {
                    var dirinfo=item.Tag as DirectoryInfo;
                    if (dirinfo!=null && !string.IsNullOrEmpty(dirinfo.Name) 
                                      && dirinfo.Name.ToUpper().StartsWith("LAG"))
                    {
                        return Visibility.Visible;
                    }
                }
                

                return Visibility.Collapsed;
            }
        }
        public Visibility IsElementFileVisibility
        {
            get
            {
                var item = m_SelectedItem;
                if (item == null)
                {
                    return Visibility.Collapsed;
                }

                if (item.Tag.GetType() == typeof(FileInfo))
                {
                        return Visibility.Visible;
                }


                return Visibility.Collapsed;
            }
        }

        public void ExportFile_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            var item = m_SelectedItem;
            if (item == null)
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
            

            if (item.Tag.GetType() != typeof(FileInfo))
            {
                return;
               
            }
            var parent = FindAncestor<TreeViewItem>(item);
            if (parent == null)
            {
                return;
            }


            string startDir = ((FileInfo)item.Tag).FullName;

            List<FileInfo> fileList = new List<FileInfo>();
            fileList.Add((FileInfo)item.Tag);
            Image = null;
            FileAccessHelper.MoveFiles(fileList, ToDir);
            parent.Items.Remove(item);
            this.OnPropertyChanged("FoldersItems");
        }

        public void RefreshFolder_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            var item = m_DirectoryItem;
            if (item == null)
            {
                return;
            }
        }

        internal void ExportFolder_Click(object sender, RoutedEventArgs e)
        {
            var item = m_DirectoryItem;
            if (item == null)
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
            Image = null;
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
                    if (MessageBox.Show(startDir, "Slett Katalog", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK);
                    {

                        var listofSubDirs = Directory.GetDirectories(startDir);
                        if (listofSubDirs != null)
                        {
                            foreach (var onesub in listofSubDirs)
                            {
                                Directory.Delete(onesub);
                            }
                        }
                        Image = null;
                        Directory.Delete(startDir);
                        FoldersItems.Clear();
                        Init(m_SelectedOrionDir);
                        this.OnPropertyChanged("FoldersItems");
                    }
                }
                catch (Exception ee)
                {
                    string medd = ee.Message;
                    MessageBox.Show(medd, "Feil", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }

        }

        internal void TreeViewItem_OnItemSelected(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeViewItem;
            
            
            m_DirectoryItem = null; 
            m_SelectedItem = null;
            if (item == null)
            {
                this.OnPropertyChanged("IsElementFolderVisibility");
                this.OnPropertyChanged("IsElementFileVisibility");
                return;
            }
            if (item.Tag == null)
            {
                this.OnPropertyChanged("IsElementFolderVisibility");
                this.OnPropertyChanged("IsElementFileVisibility");
                return;
            }

            if (item.Tag.GetType() == typeof(DirectoryInfo))
            {
                m_DirectoryItem = item;
                m_SelectedItem = item;
            }
            if (item.Tag.GetType() == typeof(FileInfo))
            {
                var parent = FindAncestor<TreeViewItem>(item);
                m_DirectoryItem = parent;
                m_SelectedItem = item;
                DisplayedImagePath = ((FileInfo)item.Tag).FullName;
                Image = LoadBitMap(DisplayedImagePath);
                //if (File.Exists(DisplayedImagePath))
                //{
                //    BitmapImage bitmap = new BitmapImage();
                //    bitmap.BeginInit();
                //    bitmap.UriSource = new Uri(DisplayedImagePath, UriKind.Absolute);
                //    bitmap.EndInit();
                //    Image = bitmap;
                //}
            }
            this.OnPropertyChanged("IsElementFolderVisibility");
            this.OnPropertyChanged("IsElementFileVisibility");
        }

        public BitmapImage LoadBitMap(string path)
        {
            if (File.Exists(DisplayedImagePath))
            {
                var bi = new BitmapImage();

                using (var fs = new FileStream(path, FileMode.Open))
                {
                    bi.BeginInit();
                    bi.StreamSource = fs;
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.EndInit();
                }

                bi.Freeze();
                return bi;
            }
            return null;
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
            m_configuration["LastStevne"] = stevneNavn;
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
                if (string.IsNullOrEmpty(m_StevneNavn))
                {
                    MessageBox.Show("StevneNavn må være satt", "Feil", MessageBoxButton.OK);
                }


                if (string.IsNullOrEmpty(m_StevneDir))
                {
                    MessageBox.Show("StevneDir må være satt", "Feil", MessageBoxButton.OK);
                }

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
            FoldersItems.Clear();
            Init(m_SelectedOrionDir);
            this.OnPropertyChanged("FoldersItems");
        }

        private List<FileInfo> GetFiles(string startDir)
        {
            List<FileInfo> list = new List<FileInfo>();
            DirectoryInfo inf = new DirectoryInfo(startDir);
            var dirName = Path.GetFileName(inf.FullName);
            if (dirName.StartsWith("Lag"))
            {
                var fileList = Directory.GetFiles(inf.FullName, "*.png", SearchOption.TopDirectoryOnly).ToList();
                foreach (var filename in fileList)
                {
                    list.Add(new FileInfo(filename));
                }
            }
            else
            {
                var dirlist = inf.GetDirectories();
                foreach (var direl in dirlist)
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
                if (m_Is15Checked == value)
                {
                    return;
                }

                m_Is15Checked = value;
                
                if (m_Is15Checked == true)
                {
                    m_OrionDirs.Clear();
                    m_SelectedOrionDir = null;
                    if (!string.IsNullOrEmpty(m_remote15m))
                    {
                        m_OrionDirs.Add(m_remote15m);
                        SelectedOrionDir = m_OrionDirs[0];
                    }
                    m_configuration["LastStevneType"] = Constants.Prefix15m;
                    OnPropertyChanged("OrionDirs");
                    OnPropertyChanged("SelectedOrionDir");
                }
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
                if (m_Is200Checked == value)
                {
                    return;
                }
                m_Is200Checked = value;
                OnPropertyChanged("Is200Checked");

                if (m_Is200Checked == true)
                {
                    m_configuration["LastStevneType"] = Constants.Prefix200m;
                    m_OrionDirs.Clear();
                    m_SelectedOrionDir = null;
                    if (m_remote200m.Count > 0)
                    {
                        foreach (var ewl in m_remote200m)
                        {
                            m_OrionDirs.Add(ewl);
                        }

                        SelectedOrionDir = m_OrionDirs[0];
                    }
                }

                OnPropertyChanged("OrionDirs");
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
                if (m_IsGrovFeltChecked == value)
                {
                    return;
                }
                m_IsGrovFeltChecked = value;
                OnPropertyChanged("IsGrovFeltChecked");


                if (m_IsGrovFeltChecked == true)
                {
                    m_configuration["LastStevneType"] = Constants.PrefixGrovFelt;
                    m_OrionDirs.Clear();
                    m_SelectedOrionDir = null;
                    if (m_remoteGrovFelt.Count > 0)
                    {
                        foreach (var ewl in m_remoteGrovFelt)
                        {
                            m_OrionDirs.Add(ewl);
                        }

                        SelectedOrionDir = m_OrionDirs[0];
                    }
                }




                OnPropertyChanged("OrionDirs");
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
                if (m_Is100Checked == value)
                {
                    return;
                }
                m_Is100Checked = value;
                OnPropertyChanged("Is100Checked");

                if (m_Is100Checked == true)
                {
                    m_configuration["LastStevneType"] = Constants.Prefix100m;
                    m_OrionDirs.Clear();
                    m_SelectedOrionDir = null;
                    if (!string.IsNullOrEmpty(m_remote100m))
                    {
                        m_OrionDirs.Add(m_remote100m);
                        SelectedOrionDir = m_OrionDirs[0];
                    }

                    OnPropertyChanged("OrionDirs");
                }
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
                if (m_IsFinFeltChecked == value)
                {
                    return;
                }
                m_IsFinFeltChecked = value;
                OnPropertyChanged("IsFinFeltChecked");

                if (m_IsFinFeltChecked == true)
                {
                    m_configuration["LastStevneType"] = Constants.PrefixFinFelt;
                    m_OrionDirs.Clear();
                    m_SelectedOrionDir = null;
                    if (!string.IsNullOrEmpty(m_remoteFinFelt))
                    {
                        m_OrionDirs.Add(m_remoteFinFelt);
                        SelectedOrionDir = m_OrionDirs[0];
                    }

                    OnPropertyChanged("OrionDirs");
                }
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
                Init(m_SelectedOrionDir);
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
        private TreeViewItem m_SelectedItem;

        private void Init(string startPath)
        {
            ClearFolderItems();
            if (string.IsNullOrEmpty(startPath))
            {
                return;
            }

            if (!Directory.Exists(startPath))
            {
                return;
            }
            m_DirectoryItem = null;
            m_SelectedItem = null;
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

        private void ClearFolderItems()
        {
            m_DirectoryItem = null;
            FoldersItems.Clear();
            TreeViewItem item = new TreeViewItem();
            item.Header = "No Valid Folder Found";
            item.FontWeight = FontWeights.Normal;
            item.Items.Add(dummyNode);
            FoldersItems.Add(item);
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
                            if (fileList.Count > 0)
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

