using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NaiveGUI
{
    public static class TreeViewExtension
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct TV_ITEM
        {
            public int Mask;
            public IntPtr ItemHandle;
            public int State;
            public int StateMask;
            public IntPtr TextPtr;
            public int TextMax;
            public int Image;
            public int SelectedImage;
            public int Children;
            public IntPtr LParam;
        }

        public const int TVIF_STATE = 0x8;
        public const int TVIS_STATEIMAGEMASK = 0xF000;

        public const int TVM_SETITEMA = 0x110d;
        public const int TVM_SETITEMW = 0x113f;

        public const int TVM_GETITEM = 0x110C;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, ref TV_ITEM lParam);

        /// <summary>
        /// 读取 <see cref="TreeNode"/> 的展开状态并保存为 <see cref="List{string}"/>
        /// </summary>
        public static List<string> GetExpansionState(this TreeNodeCollection nodes) => nodes.Descendants().Where(n => n.IsExpanded).Select(n => n.FullPath).ToList();

        /// <summary>
        /// 还原 <see cref="TreeNode"/> 的展开状态
        /// </summary>
        public static void SetExpansionState(this TreeNodeCollection nodes, List<string> savedExpansionState)
        {
            foreach(var node in nodes.Descendants())
            {
                if(savedExpansionState.Contains(node.FullPath))
                {
                    node.Expand();
                }
            }
        }

        /// <summary>
        /// 遍历获取整个 <see cref="TreeView"/> 中的 <see cref="TreeNode"/>
        /// </summary>
        public static IEnumerable<TreeNode> Descendants(this TreeNodeCollection c)
        {
            foreach(TreeNode node in c)
            {
                yield return node;
                foreach(var child in node.Nodes.Descendants())
                {
                    yield return child;
                }
            }
        }

        /// <summary>
        /// 隐藏 <see cref="TreeView"/> 中指定 <see cref="TreeNode"/> 的 <see cref="CheckBox"/>
        /// </summary>
        public static void HideCheckBox(this TreeView tree, TreeNode node)
        {
            var updatedTvItem = new TV_ITEM
            {
                State = 0,
                Mask = TVIF_STATE,
                ItemHandle = node.Handle,
                StateMask = TVIS_STATEIMAGEMASK
            };
            SendMessage(tree.Handle, TVM_SETITEMW, 0, ref updatedTvItem);
        }
    }
}
