using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace WKEFSERVICE.Commons
{
    public class FileHeader
    {
        // read files in hex:       https://stackoverflow.com/questions/9071581/how-to-read-tiff-header-file-c
        // public class MimeType:   https://stackoverflow.com/questions/58510/using-net-how-can-you-find-the-mime-type-of-a-file-based-on-the-file-signature
        // List of file signatures: https://en.wikipedia.org/wiki/List_of_file_signatures

        /// <summary>檔案標頭類別 None, Image, Media, Document, Other</summary>
        public enum TypeFileHeader { None, Image, Media, Document, Other };

        // Image
        private static readonly byte[] ICO = { 0, 0, 1, 0 };
        private static readonly byte[] BMP = { 66, 77 };
        private static readonly byte[] JPG = { 255, 216, 255 };
        private static readonly byte[] PNG = { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82 };
        private static readonly byte[] GIF = { 71, 73, 70, 56 };
        private static readonly byte[] TIFF = { 73, 73, 42, 0 };
        // Media
        private static readonly byte[] MP3 = { 255, 251, 48 };
        private static readonly byte[] WAV_AVI = { 82, 73, 70, 70 };
        private static readonly byte[] WMV_WMA = { 48, 38, 178, 117, 142, 102, 207, 17, 166, 217, 0, 170, 0, 98, 206, 108 };
        private static readonly byte[] OGG = { 79, 103, 103, 83, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0 };
        private static readonly byte[] SWF = { 70, 87, 83 };
        // Document
        private static readonly byte[] DOC_XLS_PPT = { 208, 207, 17, 224, 161, 177, 26, 225 };
        private static readonly byte[] PDF = { 37, 80, 68, 70, 45, 49, 46 };
        private static readonly byte[] TXT = { 239, 187, 191 };
        // Other
        private static readonly byte[] TTF = { 0, 1, 0, 0, 0 };
        private static readonly byte[] EXE_DLL = { 77, 90 };
        private static readonly byte[] TORRENT = { 100, 56, 58, 97, 110, 110, 111, 117, 110, 99, 101 };
        private static readonly byte[] RAR = { 82, 97, 114, 33, 26, 7, 0 };
        private static readonly byte[] ZIP_DOCX = { 80, 75, 3, 4 };

        private static readonly List<byte[]> Types_Image = new List<byte[]>() { ICO, BMP, JPG, PNG, GIF, TIFF };
        private static readonly List<byte[]> Types_Media = new List<byte[]>() { MP3, WAV_AVI, WMV_WMA, OGG, SWF };
        private static readonly List<byte[]> Types_Document = new List<byte[]>() { DOC_XLS_PPT, PDF, TXT };
        private static readonly List<byte[]> Types_Other = new List<byte[]>() { TTF, EXE_DLL, TORRENT, RAR, ZIP_DOCX };

        const int HEADER_SIZE = 32;

        /// <summary>
        /// 辨識檔案類型<br />
        /// 傳入: <br />
        /// - fileStream: 欲辨識檔案的 Stream
        /// </summary>
        /// <param name="fileStream">欲辨識檔案的 Stream</param>
        /// <returns>檔案標頭類別 None, Image, Media, Document, Other</returns>
        public static TypeFileHeader GetFileType(Stream fileStream)
        {
            if (fileStream.Length <= 0) return TypeFileHeader.None;
            fileStream.Position = 0;
            // Get file header
            byte[] fileHeader = new byte[HEADER_SIZE];
            using (var fs = fileStream)
            {
                fs.Read(fileHeader, 0, HEADER_SIZE);
                fs.Close();
            }
            // Check file header type
            if (CheckIs(fileHeader, Types_Image)) return TypeFileHeader.Image;
            if (CheckIs(fileHeader, Types_Media)) return TypeFileHeader.Media;
            if (CheckIs(fileHeader, Types_Document)) return TypeFileHeader.Document;
            if (CheckIs(fileHeader, Types_Other)) return TypeFileHeader.Other;
            return TypeFileHeader.None;
        }

        /// <summary>
        /// 辨識檔案類型<br />
        /// 傳入: <br />
        /// - fileStream: 欲辨識檔案的 Stream<br />
        /// - CheckType: 欲檢查的標頭類別 TypeFileHeader
        /// </summary>
        /// <param name="fileStream">欲辨識檔案的 Stream</param>
        /// <param name="CheckType">欲檢查的標頭類別 TypeFileHeader</param>
        /// <returns>相同則 true, 否則 false</returns>
        public static bool CheckFileType(Stream fileStream, TypeFileHeader CheckType)
        {
            if (fileStream.Length <= 0) return false;
            fileStream.Position = 0;
            // Get file header
            byte[] fileHeader = new byte[HEADER_SIZE];
            using (var fs = fileStream)
            {
                fs.Read(fileHeader, 0, HEADER_SIZE);
                fs.Close();
            }
            // Check file header type
            switch (CheckType)
            {
                case TypeFileHeader.Image: return CheckIs(fileHeader, Types_Image);
                case TypeFileHeader.Media: return CheckIs(fileHeader, Types_Media);
                case TypeFileHeader.Document: return CheckIs(fileHeader, Types_Document);
                default: return false;
            }
        }

        private static bool CheckIs(byte[] fileHeader, List<byte[]> fileHeaderCheck)
        {
            foreach (var item in fileHeaderCheck)
            {
                // Check byte one by one
                bool isSame = true;
                for (int i = 0; i <= item.Length - 1; i++)
                {
                    if (item[i] != fileHeader[i])
                    {
                        isSame = false;
                        break;
                    }
                }
                // The byte is all same
                if (isSame) return true;
            }
            return false;
        }
    }
}