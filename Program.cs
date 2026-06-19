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
        Console.WriteLine(new string('-', 100));

        List<FileInfo> files = new List<FileInfo>();
        SearchFiles(directoryPath, files);

        
        Console.WriteLine($"{"Имя файла",-60} {"Размер (байт)",-15} {"Дата изменения"}");
        Console.WriteLine(new string('-', 100));

        foreach (var file in files)
        {
            Console.WriteLine($"{file.Name,-60} {file.Length,-15} {file.LastWriteTime:dd.MM.yyyy HH:mm:ss}");
        }

        Console.WriteLine($"\nНайдено файлов: {files.Count}");

        
        string csvPath = Path.Combine(directoryPath, "file_search_result.csv");
        SaveToCsv(files, csvPath);
        Console.WriteLine($"Результаты сохранены в: {csvPath}");
    }

    static string GetDirectoryFromUser()
    {
        Console.Write("Введите путь к каталогу (Enter — текущая папка): ");
        string? path = Console.ReadLine()?.Trim();
        return string.IsNullOrEmpty(path) ? Directory.GetCurrentDirectory() : path;
    }

    
    static void SearchFiles(string path, List<FileInfo> files)
    {
        try
        {
            string dirName = new DirectoryInfo(path).Name.ToLower();
            if (dirName == "bin" || dirName == "obj")
                return; // Пропускаем папки сборки

            foreach (string filePath in Directory.GetFiles(path))
            {
                string ext = Path.GetExtension(filePath).ToLowerInvariant();
                if (ext == ".txt" || ext == ".cs")
                {
                    files.Add(new FileInfo(filePath));
                }
            }

            foreach (string dir in Directory.GetDirectories(path))
            {
                SearchFiles(dir, files);
            }
        }
        catch (Exception)
        {
            
        }
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
