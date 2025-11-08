using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml;

namespace TestChInvestBank.Services
{
    public class XmlToCsvConverter
    {
      
        public void ConvertDynamic(string xmlPath, string xsdFolder, string csvOutputPath)
        {
            Console.WriteLine($"> Валидация и динамическая конвертация XML: {xmlPath}");

        
            //------------------------
            var schemaSet = new XmlSchemaSet();
            foreach (var xsdFile in Directory.GetFiles(xsdFolder, "*.xsd"))
                schemaSet.Add(null, xsdFile);

            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                Schemas = schemaSet
            };
            settings.ValidationEventHandler += (s, e) =>
            {
                if (e.Severity == XmlSeverityType.Error)
                    throw new XmlSchemaValidationException($"Ошибка валидации: {e.Message}");
            };



            using var reader = XmlReader.Create(xmlPath, settings);
            using var csvWriter = new StreamWriter(csvOutputPath, false, Encoding.UTF8);

            bool headerWritten = false;
            List<string> headerColumns = new();

            while (reader.Read())
            {
                // выбираем элементы верхнего уровня (обычно OBJECT, HOUSE и т.п.)
                if (reader.NodeType == XmlNodeType.Element && reader.HasAttributes)
                {
                    // собираем все атрибуты текущего узла
                    var attributes = new Dictionary<string, string>();

                    while (reader.MoveToNextAttribute())
                    {
                        attributes[reader.Name] = reader.Value;
                    }

                    reader.MoveToElement(); // вернуться к самому элементу

                    // если это первый элемент — формируем заголовок CSV
                    if (!headerWritten)
                    {
                        headerColumns = attributes.Keys.ToList();
                        csvWriter.WriteLine(string.Join(",", headerColumns));
                        headerWritten = true;
                    }

                    // записываем значения в том же порядке
                    var row = headerColumns.Select(name =>
                        attributes.TryGetValue(name, out var value)
                            ? EscapeCsv(value)
                            : "").ToArray();

                    csvWriter.WriteLine(string.Join(",", row));
                }
            }

            Console.WriteLine($"> Конвертация завершена: {csvOutputPath}");
        }


        private string EscapeCsv(string value)
        {
            // если значение содержит запятую или кавычки — экранируем
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
                return $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
        }




    }


}
