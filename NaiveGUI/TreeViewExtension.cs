using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace NaiveGUI
{
    // A useful paste lol
    // https://stackoverflow.com/questions/8308258/expand-selected-node-after-refresh-treeview-in-c-sharp
    // <s>面向stackoverflow编程</s>
    public static class TreeViewExtension
    {
        /// <summary>
        /// 读取 <see cref="TreeNode"/> 的展开状态并保存为 <see cref="List{string}"/>
        /// </summary>
        public static List<string> GetExpansionState(this TreeNodeCollection nodes)
        {
            return nodes.Descendants().Where(n => n.IsExpanded).Select(n => n.FullPath).ToList();
        }

        /// <summary>
        /// 还原 <see cref="TreeNode"/> 的展开状态
        /// </summary>
        public static void SetExpansionState(this TreeNodeCollection nodes, List<string> savedExpansionState)
        {
            foreach(var node in nodes.Descendants().Where(n => savedExpansionState.Contains(n.FullPath)))
            {
                node.Expand();
            }
        }

        /// <summary>
        /// 遍历获取整个 <see cref="TreeView"/> 中的 <see cref="TreeNode"/>
        /// </summary>
        public static IEnumerable<TreeNode> Descendants(this TreeNodeCollection c)
        {
            foreach(var node in c.OfType<TreeNode>())
            {
                yield return node;
                foreach(var child in node.Nodes.Descendants())
                {
                    yield return child;
                }
            }
        }
    }
}
