namespace Application.Interfaces;

public interface IExporter<T>
{
  void Export(T toExport, string filePath);
}
