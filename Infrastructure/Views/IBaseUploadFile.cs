namespace Infrastructure.Views;

public interface IBaseUploadFileView
{
   string FileName { get; set; }
   string Ext { get; set; }
   long FileSize { get; set; }
   string Host { get; set; }
   string DirectoryPath { get; set; }
}
