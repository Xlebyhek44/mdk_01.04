using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        string directoryPath = args.Length > 0 ? args[0] : GetDirectoryFromUser();

        if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine($"Ошибка: Каталог '{directoryPath}' не существует.");
            return;
        }

        Console.WriteLine($"Поиск файлов .txt и .cs в: {directoryPath}");
        Console.WriteLine(new string('-', 90));

        List<FileInfo> files = new List<FileInfo>();
        SearchFiles(directoryPath, files);

        // Вывод таблицы
        Console.WriteLine($"{"Имя файла",-55} {"Размер (байт)",-12} {"Дата изменения"}");
        Console.WriteLine(new string('-', 90));

        foreach (var file in files)
        {
            Console.WriteLine($"{file.Name,-55} {file.Length,-12} {file.LastWriteTime}");
        }

        Console.WriteLine($"\nНайдено файлов: {files.Count}");

        // Сохранение в CSV
        string csvPath = Path.Combine(directoryPath, "file_search_result.csv");
        SaveToCsv(files, csvPath);
        Console.WriteLine($"Результаты сохранены в: {csvPath}");
    }

    static string GetDirectoryFromUser()
    {
        Console.Write("Введите путь к каталогу (Enter — текущая папка): ");
        string path = Console.ReadLine()?.Trim();
        return string.IsNullOrEmpty(path) ? Directory.GetCurrentDirectory() : path;
    }

    static void SearchFiles(string path, List<FileInfo> files)
    {
        try
        {
            foreach (string filePath in Directory.GetFiles(path))
            {
                string ext = Path.GetExtension(filePath).ToLower();
                if (ext == ".txt" || ext == ".cs")
                    files.Add(new FileInfo(filePath));
            }

            foreach (string dir in Directory.GetDirectories(path))
                SearchFiles(dir, files);
        }
        catch { /* Пропускаем недоступные папки */ }
    }

    static void SaveToCsv(List<FileInfo> files, string csvPath)
    {
        using (StreamWriter writer = new StreamWriter(csvPath, false, Encoding.UTF8))
        {
            writer.WriteLine("Имя файла;Полный путь;Размер (байт);Дата изменения");
            foreach (var f in files)
            {
                writer.WriteLine($"\"{f.Name}\";\"{f.FullName}\";{f.Length};{f.LastWriteTime:yyyy-MM-dd HH:mm:ss}");
            }
        }
    }
}
