using System;
using System.IO;

public class ImageMetadata
{
    public string FileName { get; private set; }
    public long FileSize { get; private set; }
    public DateTime CreationDate { get; private set; }
    public DateTime UploadDate { get; private set; }

    public ImageMetadata(string filePath)
    {
        LoadMetadata(filePath);
        UploadDate = DateTime.Now; // Data wgrania na serwer ustawiona na bieżącą datę
    }

    private void LoadMetadata(string filePath)
    {
        if (File.Exists(filePath))
        {
            FileInfo fileInfo = new FileInfo(filePath);

            FileName = fileInfo.Name; // Nazwa pliku
            FileSize = fileInfo.Length; // Rozmiar pliku w bajtach
            CreationDate = fileInfo.CreationTime; // Data utworzenia pliku
        }
        else
        {
            throw new FileNotFoundException("Plik nie istnieje.");
        }
    }

    public static ImageMetadata LoadFromFile(string filePath)
    {
        return new ImageMetadata(filePath);
    }
}
