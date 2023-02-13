namespace Application.Interfaces;

public interface IFileWriter
{
  void WriteText(string path, string content);
}
