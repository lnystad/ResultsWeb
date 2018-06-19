using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WebResultsClient.Viewmodels.BitMap
{
    public class DirTreeViewItem : TreeViewItem
    {
        public DirTreeViewItem():base()
        {

        }
        List<string> Files { get; set; }
    }
}
