using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

public class Streamer<T> where T : class, new()
{
    public Streamer(string fileName) => this.FileName = fileName;

    public string FileName { get; set; }

    public void SaveOptions(T options)
    {
        var path = this.GetPath();
        if (this.IsFileExist())
        {
            File.Delete(path);
        }

        var formatter = new BinaryFormatter();
        var stream = new FileStream(path, FileMode.CreateNew);
        formatter.Serialize(stream, options);
        stream.Close();
    }
    
    public T LoadOptions()
    {
        var path = this.GetPath();
        var formatter = new BinaryFormatter();
        var stream = new FileStream(path, FileMode.Open);
        var t = formatter.Deserialize(stream) as T;
        stream.Close();
        return t;
    }

    public bool IsFileExist() => File.Exists(this.GetPath());

    private string GetPath()
    {
        var pathParts = this.GetType().Assembly.Location.Split(Path.PathSeparator);
        pathParts = pathParts.Take(pathParts.Length - 1).ToArray();
        return Path.Combine(string.Join($"{Path.PathSeparator}", pathParts), this.FileName);
    }
}