using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khsw.Instrument.Demo.Commons.Helper
{
    /// <summary>
    /// 文件读取帮助类
    /// </summary>
    public static class FileReadHelper
    {
        /// <summary>
        /// 读取整个文本文件内容为一个字符串
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件内容字符串</returns>
        public static string ReadAllText(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    throw new ArgumentNullException("文件路径不能为空!");

                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"没有找到{filePath}路径下的文件!");

                return File.ReadAllText(filePath);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 逐行读取文本文件内容为字符串数组
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件内容的字符串数组</returns>
        public static string[] ReadAllLines(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    throw new ArgumentNullException("文件路径不能为空!");

                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"没有找到{filePath}路径下的文件");

                return File.ReadAllLines(filePath);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 使用 StreamReader 逐行读取文件内容并通过回调处理每一行
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="processLine">处理每一行的回调函数</param>
        public static void ReadFileByLines(string filePath, Func<string,bool> processLine)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    throw new ArgumentNullException("文件路径不能为空!");

                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"没有找到{filePath}路径下的文件");

                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!processLine(line))
                        {
                            break;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
