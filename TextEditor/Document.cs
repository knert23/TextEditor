using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextEditor
{
    class Document : TabPage
    {
        public Document() : base()
        {
            RichTextBox rtb = new RichTextBox();
            rtb.Parent = this;
            rtb.Name = "rtb";
            rtb.Dock = DockStyle.Fill;
        }
        public string Path { get; set; }

        public string DefaultFileName
        {
            get
            {
                return "NewFile";
            }
        }

        private string RTBText { get; set; }

        public string ShortName
        {
            get
            {
                return Path.Substring(Path.LastIndexOf('\\') + 1);
            }
        }

        public bool HasPath
        {
            get
            {
                return Path is not null;
            }
        }

        public void Save()
        {
            // Текст с текс бокса передается в свойство
            RTBText = SelectedRTB.Text;
            WriteToFile();

            // Файл после сохранения не имеет изменений
            SelectedRTB.Modified = false;
        }

        public void SaveAs(string FileName)
        {
            // Текст с текс бокса передается в свойство
            RTBText = SelectedRTB.Text;
            // Если файл перемещается, то он удаляется с предыдущего места хранения
            if (HasPath & !FileName.Equals(Path))
            {
                File.Delete(Path);
            }

            Path = FileName;
            Text = ShortName;
            WriteToFile();

            // Файл после сохранения не имеет изменений
            SelectedRTB.Modified = false;
        }

        private void WriteToFile()
        {
            using (Stream streamWriter = new FileStream(Path, FileMode.Create))
            {
                // Преобразование текста в массив байтов
                byte[] buffer = Encoding.Default.GetBytes(RTBText);

                // Запись в файл
                streamWriter.Write(buffer);
            }
        }

        public void Open(String FileName)
        {
            Path = FileName;
            Text = ShortName;
            ReadFile();
        }

        private void ReadFile()
        {
            using (Stream streamReader = new FileStream(Path, FileMode.Open))
            {
                // Массив байтов для считывания
                byte[] buffer = new byte[streamReader.Length];

                // Считывание данных
                streamReader.Read(buffer);

                // Декодирование
                string fileText = Encoding.Default.GetString(buffer);

                // Запись декодированного текста в RichTextBox
                SelectedRTB.Text = fileText;

                // Запись декодированного текста в свойство для дальнейшей работы
                RTBText = fileText;
            }
        }

        private RichTextBox SelectedRTB
        {
            get
            {
                RichTextBox selectedRtb = (RichTextBox)this.Controls["rtb"];
                return selectedRtb;
            }
        }

        public bool Modified
        {
            get
            {
                return SelectedRTB.Modified;
            }
        }
    }
}