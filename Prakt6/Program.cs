using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Text.Json;
using System.Xml.Linq;

namespace BelieveOrNotBelieve
{
    // Класс для вопроса
    [Serializable]//Атрибут
    public class Question
    {
        string text = null; // Текст вопроса
        private bool trueFalse = false;// Правда или нет
                                       // Вам же предлагается сделать поля закрытыми и реализовать открытые свойства Text и TrueFalse
                                       // Для сериализации должен быть пустой конструктор.
        public Question()
        {
        }

        public Question(string text, bool trueFalse)
        {
            this.text = text;
            this.trueFalse = trueFalse;
        }

        //Свойство класса
        public string Text
        {
            //акцессор доступа (access)
            get { return text; } //получить данные из объекта
            set { text = value; }//записать данные в объект

        }

        public bool TrueFalse
        {
            get { return trueFalse; }
            set { trueFalse = value; }
        }
    }
    // Класс для хранения списка вопросов. А так же для сериализации в XML и десериализации из XML
    class TrueFalseData
    {
        private string fileName;
        private List<Question> list;
        
        public string FileName//свойства
        {
            set { fileName = value; }
        }

        //Create - создаем или считываем
        public TrueFalseData(string fileName,bool Create=true)
        {
            this.fileName = fileName;
            list = new List<Question>();
            if (!Create)
            {
                string ext = System.IO.Path.GetExtension(fileName);//получаем расширение файла
                switch (ext)
                {
                    case ".xml":
                        LoadXML();
                        break;
                    case ".json":
                        LoadJSON();
                        break;
                    case ".txt":
                        LoadTXT();
                        break;
                }
            }

        }
        public void Add(string text, bool trueFalse)
        {
            list.Add(new Question(text, trueFalse));
        }
        public void Remove(int index)
        {

            if (list != null) throw new NullReferenceException("Список не создан");
            if (index < list.Count && index >= 0) list.RemoveAt(index); 
            else throw new IndexOutOfRangeException("Индекс вне диапазона");
        }
        public int Count
        {
            get { return list.Count; }
        }

        public List<Question> List
        {
            get //механизм доступа к приватным полям
            { 
                    
                return list; 
            }
        }

        //Сериализатор
        public void SaveXML()
        {
            XmlSerializer xmlFormat = new XmlSerializer(typeof(List<Question>));
            Stream fStream = new FileStream(System.IO.Path.GetFileNameWithoutExtension(fileName) + ".xml", FileMode.Create, FileAccess.Write);
            xmlFormat.Serialize(fStream, list);
            fStream.Close();
        }
        //Десериализатор
        public void LoadXML()
        {
            XmlSerializer xmlFormat = new XmlSerializer(typeof(List<Question>));
            Stream fStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            list = (List<Question>)xmlFormat.Deserialize(fStream);
            fStream.Close();
        }

        public void SaveJSON()
        {
            string jsonString=JsonSerializer.Serialize(list);            
            File.WriteAllText(System.IO.Path.GetFileNameWithoutExtension(fileName) + ".json", jsonString);
        }

        public void LoadJSON()
        {
            string temp=File.ReadAllText(fileName);
            list=JsonSerializer.Deserialize<List<Question>>(temp);
        }

        public void SaveTXT()
        {
            string text = "";
            foreach (Question question in list)
            {
                text += question.Text+"\n";
                text += question.TrueFalse.ToString() + "\n";
            }
            File.WriteAllText(System.IO.Path.GetFileNameWithoutExtension(fileName)+".txt", text);
        }

        public void LoadTXT()
        {
            string[] lines = File.ReadAllLines(fileName);
            for(int i=0;i<lines.Length; i++)
            {
                list.Add(new Question(lines[i], Convert.ToBoolean(lines[i+1])));
                i++;
            }
            
        }
    }
}

namespace BelieveOrNotBelieve
{
    class Program
    { 


        static void SaveToFile(string fileName)
        {
            TrueFalseData data = new TrueFalseData(fileName, true);
            data.Add("В японии на уроках на доске пишут кисточкой",true);
            data.Add("Первым программистом была женщина", true);
            data.Add("2x2=5", false);
            string ext = System.IO.Path.GetExtension(fileName);//получаем расширение файла
            switch (ext)
            {
                case ".xml":
                    data.SaveXML();
                    break;
                case ".json":
                    data.SaveJSON();
                    break;
                case ".txt":
                    data.SaveTXT();
                    break;
            }
        }

        static void Edit(TrueFalseData data)
        {
            ConsoleKeyInfo key;
            do
            {
                Console.WriteLine("Сохранить файл в одном из трех форматов (F1-txt,F2-json,F3-xml). ESC - выход");
                key = Console.ReadKey();
                if (key.Key == ConsoleKey.F1) data.SaveTXT();
                if (key.Key == ConsoleKey.F2) data.SaveJSON();
                if (key.Key == ConsoleKey.F3) data.SaveXML();
            }
            while (key.Key != ConsoleKey.Escape);
        }

        static void PrintList(List<Question> list)
        {
            foreach (var item in list)
            {
                Console.WriteLine(item.Text + " " + item.TrueFalse);
            }
        }

     static void Main()
        {
            string filename;            
            do
            {
                Console.Clear();
                Console.WriteLine("Введите путь до файла (вместе с названием), который нужно открыть (Exit - выход)");
                Console.WriteLine("-----------------------------------------------------------------");
                filename=Console.ReadLine();
                if (filename == "Exit") break;
                TrueFalseData data = new TrueFalseData(filename,false);
                PrintList(data.List);
                Edit(data);
            }
            while (true);
            Console.WriteLine("Press any key");
            Console.ReadKey();
        }
    
    }

}