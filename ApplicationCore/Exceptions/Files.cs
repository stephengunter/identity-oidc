using Infrastructure.Entities;

namespace ApplicationCore.Exceptions;
public class FileNotExistException : Exception
{
	public FileNotExistException(string path) : base($"FileNotFound. Path: {path}")
	{
      Path = path;
   }

   public FileNotExistException(EntityBase entity, string path = "") : base($"FileNotFound. Type: {entity.GetType().Name}  Id: {entity.Id}  Path: {path}")
   {
      Path = path;
   }

   public string Path { get; set; }


}
public class UploadFileFailedException : Exception
{
   public UploadFileFailedException(string folderPath, string fileName) : base($"UploadFileFailedException. folderPath: {folderPath}  fileName: {fileName}")
   {
      FolderPath = folderPath;
      FileName = fileName;
   }
   public string FolderPath { get; set; }
   public string FileName { get; set; }
}

public class MoveFileFailedException : Exception
{
   public MoveFileFailedException(string source, string dest) : base($"MoveFileFailedException. SourcePath: {source}  DestPath: {dest}")
   {
      SourcePath = source;
      DestPath = dest;
   }

   public MoveFileFailedException(EntityBase entity, string source, string dest) : base($"MoveFileFailedException. Type: {entity.GetType().Name}  Id: {entity.Id}  SourcePath: {source}  DestPath: {dest}")
   {
      SourcePath = source;
      DestPath = dest;
   }
   public string SourcePath { get; set; }
   public string DestPath { get; set; }
}
