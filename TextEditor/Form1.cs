using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextEditor
{
    public partial class Form1 : Form
    {
        Editor editor = new Editor();
        RecentList recentList;
        public Form1()
        {
            InitializeComponent();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            recentList.RecentListAddNotify += AddFileNamesToRecentMenu;
            editor.SaveDocument();
        }

        private void savveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            recentList.RecentListAddNotify += AddFileNamesToRecentMenu;
            editor.SaveDocumentAs();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editor.NewDocument();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panel1.Controls.Add(editor);
            editor.Dock = DockStyle.Fill;
            editor.NewDocument();


            // Передача обекта RecentList из Editor в Form1
            editor.EditorNotify += GetRecentListObject;
            editor.TransferObject();

            // Загрузка данных с текстового файла в список
            recentList.LoadData();

            // Добавление названий файлов в меню Resent
            AddFileNamesToRecentMenu();
        }

        private void AddFileNamesToRecentMenu()
        {
            recentToolStripMenuItem.DropDownItems.Clear();
            for (int i = 0; i < recentList.Count; i++)
            {
                recentToolStripMenuItem.DropDownItems.Add(recentList[i]).Click += new EventHandler(RecentListItem_Click);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            recentList.RecentListAddNotify += AddFileNamesToRecentMenu;

            editor.OpenDocument();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editor.CloseActiveDocument();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editor.Exit();
        }

        private void RecentListItem_Click(object sender, EventArgs e)
        {
            recentList.RecentListAddNotify += AddFileNamesToRecentMenu;

            var recentListItem = (ToolStripMenuItem) sender;
            int index = 0;

            foreach (ToolStripMenuItem item in recentToolStripMenuItem.DropDownItems)
            {
                if (item.Text.Equals(recentListItem.Text))
                {
                    break;
                }
                index++;
            }

            editor.OpenDocumentByRecentIndex(index);
        }

        private void GetRecentListObject(RecentList recentList)
        {
            this.recentList = recentList;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Если пользователь нажимает на кнопку закрытия
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel =  editor.Exit();
            }

            // Если происходит вызов метода Application.Exit()
            if (e.CloseReason == CloseReason.ApplicationExitCall)
            {
                Application.Exit();
            }
        }
    }
}