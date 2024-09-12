﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace Khsw.Instrument.Demo.Commons.Helper
{
    /// <summary>
    /// 字节数组帮助扩展
    /// </summary>
    public static class ByteArrayHelperExtensions
    {
        public static string ToAppendString(this byte[] bytes)
        {
            if (bytes == null || !bytes.Any())
                return null;

            StringBuilder str = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                str.Append(bytes[i].ToString("X2"));
                str.Append(" ");
            }
            return str.ToString();
        }
    }
}
