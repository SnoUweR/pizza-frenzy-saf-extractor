using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PizzaFrenzySAFExtractor
{
    public class SafExtractor
    {
        private string _rootDirectoryForSaving;
        private string _safFilePath;
        private bool _makePngFromJpegAndMask;

        public SafExtractor(string rootDirectoryForSaving, string safFilePath, bool makePngFromJpegAndMask)
        {
            if (string.IsNullOrEmpty(rootDirectoryForSaving))
                throw new ArgumentNullException($"{nameof(rootDirectoryForSaving)} can't be null or empty");
            
            if (string.IsNullOrEmpty(safFilePath))
                throw new ArgumentNullException($"{nameof(safFilePath)} can't be null or empty");
            
            if (!File.Exists(safFilePath))
                throw new FileNotFoundException($"File not found at path {safFilePath}");
            
            _rootDirectoryForSaving = rootDirectoryForSaving;
            _safFilePath = safFilePath;
            _makePngFromJpegAndMask = makePngFromJpegAndMask;
        }
        
        public void Extract()
        {
            using (var fs = new FileStream(_safFilePath, FileMode.Open))
            {
                using (var reader = new BinaryReader(fs))
                {
                    var header = ReadHeader(reader);
                    var filesList = ReadAllFilesLists(header, fs, reader);

                    foreach (var file in filesList)
                    {
                        fs.Seek(file.DataAddress, SeekOrigin.Begin);
                        using (var savingFile = CreateFileStreamForSaving(file))
                        {
                            for (int dataIndex = 0; dataIndex < file.DataSize; dataIndex++)
                                savingFile.WriteByte(reader.ReadByte());
                        }
                    }

                    if (_makePngFromJpegAndMask)
                    {
                        Parallel.ForEach(filesList, file =>
                        {
                            var fileName = Path.GetFileName(file.Path);
                            if (!fileName.EndsWith(".jpg") || fileName.StartsWith("_"))
                                return;
                            
                            var jpegFilePath = GetFullPath(file);
                            var jpegFileDirectory = Path.GetDirectoryName(jpegFilePath);
                            var fileNameWithoutExtensions = Path.GetFileNameWithoutExtension(fileName);
                            var maskFilePath = Path.Combine(jpegFileDirectory, $"_{fileNameWithoutExtensions}.png");

                            if (!File.Exists(maskFilePath))
                                return;

                            var resultFilePath = Path.Combine(jpegFileDirectory, $"{fileNameWithoutExtensions}.png");
                            MaskProcessor.CreatePng(jpegFilePath, maskFilePath, resultFilePath);

                            File.Delete(jpegFilePath);
                            File.Delete(maskFilePath);
                        });
                    }
                }
            }
        }

        private string GetFullPath(SAFFileInfo fileInfo)
        {
            if (string.IsNullOrEmpty(fileInfo.Path))
                throw new ArgumentException($"{nameof(fileInfo.Path)} is null or empty");

            return Path.Combine(_rootDirectoryForSaving, fileInfo.Path);
        }

        private FileStream CreateFileStreamForSaving(SAFFileInfo fileInfo)
        {
            var fullPath = GetFullPath(fileInfo);
            var directory = Path.GetDirectoryName(fullPath);
            if (string.IsNullOrEmpty(directory))
                throw new Exception($"Can't get DirectoryName from path {fullPath}");
            
            Directory.CreateDirectory(directory);
            return File.Create(fullPath, 1024 * 8);
        }
        
        private static SAFFileInfo[] ReadAllFilesLists(SAFHeader header, FileStream fs, BinaryReader reader)
        {
            var list = new List<SAFFileInfo>();
            
            foreach (var address in header.FileListsAddresses)
            {
                fs.Seek(address, SeekOrigin.Begin);

                var filesListHeader = ReadFilesListHeader(reader);

                for (int fileIndex = 0; fileIndex < filesListHeader.FilesCount; fileIndex++)
                {
                    var fileInfo = new SAFFileInfo
                    {
                        DataAddress = reader.ReadInt32(),
                        DataSize = reader.ReadInt32(),
                        UnknownInformation = reader.ReadBytes(16),
                        PathLength = reader.ReadInt16()
                    };
                    
                    // Trim is needed because of Zero-symbol in the end.
                    fileInfo.Path = Encoding.ASCII.GetString(reader.ReadBytes(fileInfo.PathLength)).TrimEnd('\0');
                    list.Add(fileInfo);
                }
            }

            return list.ToArray();
        }

        private static SAFHeader ReadHeader(BinaryReader reader)
        {
            var header = new SAFHeader
            {
                ChunkId = reader.ReadChars(4),
                TotalFileLists = reader.ReadInt32()
            };
            
            header.FileListsAddresses = new int[header.TotalFileLists];
                    
            for (int i = 0; i < header.TotalFileLists; i++)
            {
                var address = reader.ReadInt32();
                header.FileListsAddresses[i] = address;
            }

            return header;
        }
        
        private static SAFFilesListHeader ReadFilesListHeader(BinaryReader reader)
        {
            return new SAFFilesListHeader
            {
                Index = reader.ReadInt32(),
                UnknownInformation = reader.ReadBytes(16),
                FilesCount = reader.ReadInt32()
            };
        }
    }
}