using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextEditor
{
    class Editor: TabControl
    {
        RecentList recentList;

        // Событие нужно для того, чтобы работать с одним и тем же объектом RecentList
        public delegate void EditorDelegate(RecentList recentList);
        public event EditorDelegate? EditorNotify;

        public Editor()
        {
            recentList = new();
            recentList.Capacity = 5;
        }

        public string DefaultPath
        {
            get
            {
                return @"C:\Users\pc\OneDrive\Документы\Технологии программирования\5 семестр\Файлы TextEditor\";
            }
        }

        public void TransferObject()
        {
            EditorNotify?.Invoke(recentList);
        }

        public int CountPages { get; set; } = 0;
        public void NewDocument()
        {
            Document document = new();

            SetTablePageText(document);

            // Добавление новой странички в коллекцию страничек
            this.TabPages.Add(document);
            // Переход на только что созданную страничку
            this.SelectedTab = document;
        }

        private void SetTablePageText(Document document)
        {
            // Название вкладки
            CountPages += 1;
            document.Text = string.Concat(document.DefaultFileName, CountPages);
        }

        public void SaveDocument()
        {
            if (SelectedDocument.HasPath)
            {
                SelectedDocument.Save();
            }
            else
            {
                SaveDocumentAs();
            }
        }

        // Выбранная вкладка
        private Document SelectedDocument
        {
            get
            {
                return SelectedTab as Document;
            }
        }

        public void SaveDocumentAs()
        {
            SaveFileDialog saveFileDialog = new();
            saveFileDialog.InitialDirectory = DefaultPath;
            saveFileDialog.FileName = SelectedDocument.Text;
            saveFileDialog.DefaultExt = ".txt";
            saveFileDialog.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string oldFileName = GetOldFileName(saveFileDialog);
                SelectedDocument.SaveAs(saveFileDialog.FileName);
                AddToRecentList(oldFileName, SelectedDocument.Path);
            }
        }

        // Добавление пути в recentList при вызове метода SaveDocumentAs
        private void AddToRecentList(string oldfileName, string newFileName)
        {
            if (recentList.Contains(oldfileName))
            {
                recentList.Remove(oldfileName);
            }

            recentList.Add(newFileName);
        }

        // Получение предыдущего имени файла
        private string GetOldFileName(SaveFileDialog saveFileDialog)
        {
            if (SelectedDocument.Path is null)
            {
                return saveFileDialog.FileName;
            }

            return SelectedDocument.Path;
        }

        public void OpenDocument()
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.InitialDirectory = DefaultPath;
            openFileDialog.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Создание новой вкладки для открывающегося документа 
                CreateTab(openFileDialog.FileName);
            }
        }

        private void CreateTab(string FileName)
        {
            if (IsDocumentOpen(FileName))
            {
                SetSelectedTab(FileName);

                return;
            }

            // Создание новой вкладки
            NewDocument();
            CountPages--;

            SelectedDocument.Open(FileName);

            // Добавление в список последних открытых файлов
            recentList.Add(SelectedDocument.Path);
        }

        private void SetSelectedTab(string FileName)
        {
            foreach (TabPage page in this.TabPages)
            {
                if ((page as Document).Path is null) continue;
                if ((page as Document).Path.Equals(FileName))
                {
                    this.SelectedTab = page;
                }
            }
        }

        public void CloseActiveDocument()
        {
            // Если нет вкладок, то ничего не происходит
            if (TabPages.Count == 0)
            {
                return;
            }

            if (SelectedDocument.Modified)
            {
                DialogResult dialogResult;
                dialogResult = MessageBox.Show("Do you want to save the file before closing?", "Close File", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (dialogResult)
                {
                    case DialogResult.Yes:
                        SaveDocumentAs();
                        Controls.Remove(SelectedDocument);
                        break;
                    case DialogResult.No:
                        Controls.Remove(SelectedDocument);
                        break;
                    case DialogResult.Cancel:
                        return;
                }
            }
            else
            {
                Controls.Remove(SelectedDocument);
            }
        }

        public bool Exit()
        {
            Document tempPage;
            for (int i = 0; i < this.TabCount;)
            {
                tempPage = this.TabPages[i] as Document;
                SelectedTab = tempPage;
                if (tempPage.Modified)
                {
                    DialogResult dialogResult;
                    dialogResult = MessageBox.Show("Do you want to save files before closing the application?", "Exit Program", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    switch (dialogResult)
                    {
                        case DialogResult.Yes:
                            SaveDocumentAs();
                            Controls.Remove(SelectedTab);
                            break;
                        case DialogResult.No:
                            Controls.Remove(SelectedTab);
                            break;
                        case DialogResult.Cancel:
                            return true;
                    }
                }
            }

            // Сохранение в текстовый файл списка последних открытых файлов
            recentList.SaveData();

            Application.Exit();

            return false;
        }

        public bool IsDocumentOpen(string FileName)
        {
            foreach (TabPage page in this.TabPages)
            {
                if ((page as Document).Path is null) continue;
                if ((page as Document).Path.Equals(FileName))
                {
                    return true;
                }
            }

            return false;
        }

        public void OpenDocumentByRecentIndex(int index)
        {
            CreateTab(recentList[index]);
        }
    }
}