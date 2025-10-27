using ByCoders.CNAB.Application.Files.CNAB.Upload;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.Text;

namespace ByCoders.CNAB.UnitTests.Builders.Application;

public class UploadCNABFileRequestBuilder
{
    private IFormFile _file;
    private string _fileName = "CNAB_20240101.txt";
    private string _contentType = "text/plain";
    private long _length = 100;
    private string _fileContent = "3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";

    public static UploadCNABFileRequestBuilder New => new();

    public UploadCNABFileRequestBuilder()
    {
        // Initialize with default valid file
        _file = CreateMockFile(_fileName, _fileContent, _contentType);
    }

    public UploadCNABFileRequestBuilder WithFileName(string fileName)
    {
        _fileName = fileName;
        RebuildFile();
        return this;
    }

    public UploadCNABFileRequestBuilder WithContent(string content)
    {
        _fileContent = content;
        _length = Encoding.UTF8.GetByteCount(content);
        RebuildFile();
        return this;
    }

    public UploadCNABFileRequestBuilder WithContentType(string contentType)
    {
        _contentType = contentType;
        RebuildFile();
        return this;
    }

    public UploadCNABFileRequestBuilder WithEmptyFile()
    {
        _fileContent = "";
        _length = 0;
        RebuildFile();
        return this;
    }

    public UploadCNABFileRequestBuilder WithLargeFile(int sizeInMB = 10)
    {
        // Create content that simulates a large file
        var lineCount = (sizeInMB * 1024 * 1024) / 86; // Each CNAB line is ~86 chars
        var lines = new List<string>();
        
        for (int i = 0; i < lineCount; i++)
        {
            lines.Add("3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ");
        }
        
        _fileContent = string.Join(Environment.NewLine, lines);
        _length = Encoding.UTF8.GetByteCount(_fileContent);
        RebuildFile();
        return this;
    }

    public UploadCNABFileRequestBuilder WithNullFile()
    {
        _file = null!;
        return this;
    }

    public UploadCNABFileRequestBuilder WithInvalidExtension()
    {
        _fileName = "invalid_file.pdf";
        RebuildFile();
        return this;
    }

    public UploadCNABFileRequestBuilder WithValidCNABContent()
    {
        var cnabLines = new[]
        {
            "3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ",
            "5201903010000013200556418150633123****7687145607MARIA JOSEFINA LOJA DO Ó - MATRIZ",
            "3201903010000012200845152540736777****1313172712MARCOS PEREIRAMERCADO DA AVENIDA",
            "2201903010000011200096206760173648****0099234234JOÃO MACEDO   BAR DO JOÃO       ",
            "1201903010000015200096206760171234****7890233000JOÃO MACEDO   BAR DO JOÃO       "
        };
        
        _fileContent = string.Join(Environment.NewLine, cnabLines);
        _length = Encoding.UTF8.GetByteCount(_fileContent);
        RebuildFile();
        return this;
    }

    public UploadCNABFileRequestBuilder WithMixedValidAndInvalidContent()
    {
        var lines = new[]
        {
            "3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ",
            "INVALID LINE FORMAT",
            "5201903010000013200556418150633123****7687145607MARIA JOSEFINA LOJA DO Ó - MATRIZ",
            "", // Empty line
            "2201903010000011200096206760173648****0099234234JOÃO MACEDO   BAR DO JOÃO       "
        };
        
        _fileContent = string.Join(Environment.NewLine, lines);
        _length = Encoding.UTF8.GetByteCount(_fileContent);
        RebuildFile();
        return this;
    }

    public UploadCNABFileRequestBuilder WithCustomFile(IFormFile file)
    {
        _file = file;
        return this;
    }

    public UploadCNABFileRequestBuilder WithRandomData()
    {
        var random = new Random();
        _fileName = $"CNAB_{DateTime.Now:yyyyMMdd}_{random.Next(1000, 9999)}.txt";
        
        var lineCount = random.Next(10, 100);
        var lines = new List<string>();
        
        for (int i = 0; i < lineCount; i++)
        {
            var transactionType = random.Next(1, 10);
            var date = DateTime.Now.AddDays(-random.Next(0, 30));
            var amount = random.Next(100, 100000);
            var cpf = random.Next(10000000, 99999999).ToString() + "000";
            
            var line = $"{transactionType}{date:yyyyMMdd}{amount:D010}{amount:D010}{cpf}4753****3153153453STORE {i:D5}    OWNER {i:D5}     ";
            lines.Add(line.PadRight(86));
        }
        
        _fileContent = string.Join(Environment.NewLine, lines);
        _length = Encoding.UTF8.GetByteCount(_fileContent);
        RebuildFile();
        return this;
    }

    private void RebuildFile()
    {
        _file = CreateMockFile(_fileName, _fileContent, _contentType);
    }

    private static IFormFile CreateMockFile(string fileName, string content, string contentType)
    {
        var file = Substitute.For<IFormFile>();
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);
        
        file.FileName.Returns(fileName);
        file.Name.Returns(Path.GetFileNameWithoutExtension(fileName));
        file.ContentType.Returns(contentType);
        file.Length.Returns(bytes.Length);
        file.OpenReadStream().Returns(stream);
        file.CopyToAsync(Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var targetStream = callInfo.ArgAt<Stream>(0);
                stream.Position = 0;
                return stream.CopyToAsync(targetStream);
            });
        
        return file;
    }

    public UploadCNABFileRequest Build()
    {
        return new UploadCNABFileRequest(_file);
    }

    public static implicit operator UploadCNABFileRequest(UploadCNABFileRequestBuilder builder)
    {
        return builder.Build();
    }
}
