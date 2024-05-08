using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor
{
    class RecentList: List<String>
    {
        public delegate void RecentListAddDelegate();
        public RecentListAddDelegate RecentListAddNotify;

        private string PathRecentList
        {
            get
            {
                return @"C:\Users\pc\OneDrive\Документы\Технологии программирования\5 семестр\Файлы TextEditor\RecentList.txt";
            }
        }
        public new void Add(string FileName)
        {
            if (this.Contains(FileName))
            {
                PutInFirstPlace(FileName);

                // Перестановка имен файлов в меню Recent
                RecentListAddNotify?.Invoke();

                return;
            }

            if (this.Count == 5)
            {
                // Удаление последнего элемента
                this.RemoveAt(this.Count - 1);
            }

            base.Add(FileName);
            PutInFirstPlace(FileName);

            // Перестановка имен файлов в меню Recent
            RecentListAddNotify?.Invoke();
        }

        // Поставнока имени файла на первое место в меню Recent
        private void PutInFirstPlace(string FileName)
        {
            int indexOfFileName = this.IndexOf(FileName);

            for (int i = indexOfFileName; i > 0; i--)
            {
                string temp = this[i];
                this[i] = this[i - 1];
                this[i - 1] = temp;
            }
        }

        // Запись в файл со списком последних открытых файлов
        public void SaveData()
        {
            using (StreamWriter streamWriter = new StreamWriter(PathRecentList))
            {
                for (int i = 0; i < this.Count; i++)
                {
                    streamWriter.WriteLine(this[i]);
                }
            }
        }

        // Чтение из файла списка последних открытых файлов
        public void LoadData()
        {
            using (StreamReader streamReader = new StreamReader(PathRecentList))
            {
                string? line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    base.Add(line);
                }
            }
        }
    }
}