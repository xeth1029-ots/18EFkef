using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WKEFSERVICE.Commons
{
    public class FileTypeDetector
    {
        /// <summary>文件簽名列表</summary>
        private static readonly Dictionary<byte[], string> _fileSignatures = new Dictionary<byte[], string>
        {
            // https://en.wikipedia.org/wiki/List_of_file_signatures
            { new byte[] { 0xFF, 0xD8, 0xFF }, "jpg" },
            { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, "png" },
            { new byte[] { 0x47, 0x49, 0x46, 0x38 }, "gif" },
            { new byte[] { 0x42, 0x4D }, "bmp" },
            { new byte[] { 0x49, 0x49, 0x2A, 0x00 }, "tif" },
            { new byte[] { 0x4D, 0x4D, 0x00, 0x2A }, "tif" },
            { new byte[] { 0x25, 0x50, 0x44, 0x46 }, "pdf" },
            { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }, "doc" },//used by doc,xls,ppt,msi,msg
            { new byte[] { 0x50, 0x4B, 0x03, 0x04 }, "zip" } // Also used by DOCX, XLSX, PPTX,odt,odp
        };

        /// <summary>取得實際檔案fileHeader,比對後的真實副檔名</summary>
        /// <param name="MyPostedFile"></param>
        /// <returns></returns>
        public static string GetFileType(HttpPostedFileBase MyPostedFile)
        {
            byte[] fileHeader = new byte[8];
            byte[] buffer = new byte[8];
            MyPostedFile.InputStream.Seek(0, SeekOrigin.Begin);
            MyPostedFile.InputStream.Read(buffer, 0, 8); //'Convert.ToInt32(MyPostedFile.InputStream.Length))
            MemoryStream stream2 = new MemoryStream(buffer);
            using (BinaryReader binReader = new BinaryReader(stream2))
            {
                fileHeader = binReader.ReadBytes(fileHeader.Length);
            }

            foreach (KeyValuePair<byte[], string> signature in _fileSignatures)
            {
                if (fileHeader.Take(signature.Key.Length).SequenceEqual(signature.Key))
                {
                    return signature.Value;
                }
            }

            return "unknown";
        }

        /// <summary>比對檔案副檔名</summary>
        /// <param name="MyPostedFile"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsFileTypeValid(HttpPostedFileBase myPostedFile, string filePath)
        {
            string s_ReadlFileType = GetFileType(myPostedFile);
            string fileExtension = Path.GetExtension(filePath).TrimStart('.').ToLowerInvariant();

            if (s_ReadlFileType == "zip")
            {
                if (fileExtension == "zip" || fileExtension == "docx" || fileExtension == "xlsx" || fileExtension == "pptx")
                {
                    return true;
                }
            }
            else if (s_ReadlFileType == "jpg" || s_ReadlFileType == "png")
            {
                if (fileExtension == "png" || fileExtension == "jpg" || fileExtension == "jpeg")
                {
                    return true;
                }
            }
            else if (s_ReadlFileType == "doc")
            {
                if (fileExtension == "doc" || fileExtension == "xls")
                {
                    return true;
                }
            }
            return s_ReadlFileType == fileExtension;
        }

        public static bool IsFileTypeValidpdf(HttpPostedFileBase myPostedFile)
        {
            string s_ReadlFileType = GetFileType(myPostedFile);
            string filePath = myPostedFile.FileName;
            string fileExtension = Path.GetExtension(filePath).TrimStart('.').ToLowerInvariant();
            if (fileExtension != "pdf") { return false; }
            return s_ReadlFileType == fileExtension;
        }
    }
}