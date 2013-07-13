﻿
#region Comment

/*
 * Project：    FineUI
 * 
 * FileName:    StringUtil.cs
 * CreatedOn:   2008-06-25
 * CreatedBy:   30372245@qq.com
 * 
 * 
 * Description：
 *      ->
 *   
 * History：
 *      ->
 * 
 * 
 * 
 * 
 */

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web;
using System.Text.RegularExpressions;
using System.IO;
using System.IO.Compression;

namespace FineUI
{
    public class StringUtil
    {
        #region GZIPPED_VIEWSTATE

        /// <summary>
        /// GZIP压缩的ViewState隐藏字段的ID
        /// </summary>
        public static readonly string VIEWSTATE_ID = "__VIEWSTATE";

        /// <summary>
        /// GZIP压缩的ViewState隐藏字段的ID
        /// </summary>
        public static readonly string GZIPPED_VIEWSTATE_ID = "__VIEWSTATE_GZ"; 

        #endregion

        #region EnumFromName EnumToName

        public static object EnumFromName(Type enumType, string enumName)
        {
            return Enum.Parse(enumType, enumName);
        }

        public static string EnumToName(Enum param)
        {
            return Enum.GetName(param.GetType(), param);
        }

        #endregion

        #region StripHtml

        /// <summary>
        /// 去除字符串中的Html
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string StripHtml(string source)
        {
            return Regex.Replace(source, @"<[\s\S]*?>", "", RegexOptions.IgnoreCase);
        }

        #endregion

        #region GetIntListFromString GetStringListFromString

        /// <summary>
        /// 由字符串"1,2,3"转化为整形列表[1,2,3]
        /// </summary>
        /// <param name="postValue"></param>
        /// <returns></returns>
        public static List<int> GetIntListFromString(string postValue)
        {
            return GetIntListFromString(postValue, true);
        }

        /// <summary>
        /// 由字符串"1,2,3"转化为整形列表[1,2,3]
        /// </summary>
        /// <param name="postValue"></param>
        /// <param name="sortBeforeReturn">返回之前是否对数组进行排序（由小到大）</param>
        /// <returns></returns>
        public static List<int> GetIntListFromString(string postValue, bool sortBeforeReturn)
        {
            if (String.IsNullOrEmpty(postValue))
            {
                return new List<int>();
            }

            List<int> intList = new List<int>();
            string[] intStrArray = postValue.Trim().TrimEnd(',').Split(',');
            foreach (string rowIndex in intStrArray)
            {
                if (!String.IsNullOrEmpty(rowIndex))
                {
                    intList.Add(Convert.ToInt32(rowIndex));
                }
            }

            if (sortBeforeReturn)
            {
                // 按照从小到大排序 
                intList.Sort();
            }

            return intList;
        }


        /// <summary>
        /// 由字符串"ssdd,2,ok"转化为字符串列表["ssdd","2","ok"]
        /// </summary>
        /// <param name="postValue"></param>
        /// <returns></returns>
        public static List<string> GetStringListFromString(string postValue)
        {
            return GetStringListFromString(postValue);
        }

        /// <summary>
        /// 由字符串"ssdd,2,ok"转化为字符串列表["ssdd","2","ok"]
        /// </summary>
        /// <param name="postValue"></param>
        /// <param name="sortBeforeReturn">返回之前是否对数组进行排序（由小到大）</param>
        /// <returns></returns>
        public static List<string> GetStringListFromString(string postValue, bool sortBeforeReturn)
        {
            if (String.IsNullOrEmpty(postValue))
            {
                return new List<string>();
            }

            List<string> strList = new List<string>();
            string[] strArray = postValue.Trim().TrimEnd(',').Split(',');
            foreach (string str in strArray)
            {
                if (!String.IsNullOrEmpty(str))
                {
                    strList.Add(str);
                }
            }

            if (sortBeforeReturn)
            {
                // 按照从小到大排序 
                strList.Sort();
            }

            return strList;
        }

        /// <summary>
        /// 由字符串数组["ssdd","2","ok"]转化为字符串"ssdd,2,ok"
        /// </summary>
        /// <param name="postValue"></param>
        /// <returns></returns>
        public static string GetStringFromStringArray(string[] strArray)
        {
            if (strArray == null || strArray.Length == 0)
            {
                return String.Empty;
            }

            StringBuilder sb = new StringBuilder();
            foreach (string str in strArray)
            {
                sb.AppendFormat("{0},", str);
            }

            return sb.ToString().TrimEnd(',');
        }

        /// <summary>
        /// 由整型数组[2,3,4]转化为字符串"2,3,4"
        /// </summary>
        /// <param name="postValue"></param>
        /// <returns></returns>
        public static string GetStringFromIntArray(int[] intArray)
        {
            if (intArray == null || intArray.Length == 0)
            {
                return String.Empty;
            }

            StringBuilder sb = new StringBuilder();
            foreach (int str in intArray)
            {
                sb.AppendFormat("{0},", str);
            }

            return sb.ToString().TrimEnd(',');
        }

        #endregion

        #region CompareIntArray/CompareStringArray

        /// <summary>
        /// 比较两个整形数组是否相等
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        public static bool CompareIntArray(int[] array1, int[] array2)
        {
            if (array1 == null && array2 == null)
            {
                return true;
            }

            if ((array1 == null && array2 != null) || (array1 != null && array2 == null))
            {
                return false;
            }

            if (array1.Length != array2.Length)
            {
                return false;
            }

            List<int> list1 = new List<int>(array1);
            List<int> list2 = new List<int>(array2);

            for (int i = 0; i < list1.Count; i++)
            {
                if (list1[i] != list2[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 比较两个字符串数组是否相等
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        public static bool CompareStringArray(string[] array1, string[] array2)
        {
            if (array1 == null && array2 == null)
            {
                return true;
            }

            if ((array1 == null && array2 != null) || (array1 != null && array2 == null))
            {
                return false;
            }

            if (array1.Length != array2.Length)
            {
                return false;
            }

            List<string> list1 = new List<string>(array1);
            List<string> list2 = new List<string>(array2);

            for (int i = 0; i < list1.Count; i++)
            {
                if (list1[i] != list2[i])
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region ConvertPercentageToDecimalString

        /// <summary>
        /// 将 10% 转换为 0.1 的字符串的形式
        /// </summary>
        /// <param name="percentageStr"></param>
        /// <returns></returns>
        public static string ConvertPercentageToDecimalString(string percentageStr)
        {
            string decimalStr = String.Empty;

            percentageStr = percentageStr.Trim().Replace("％", "%").TrimEnd('%');

            try
            {
                decimalStr = (Convert.ToDouble(percentageStr) * 0.01).ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                ;
            }

            return decimalStr;
        }

        #endregion

        #region DecodeFrom64/EncodeTo64

        public static string DecodeFrom64(byte[] encodedDataAsBytes)
        {
            return System.Text.UTF8Encoding.UTF8.GetString(encodedDataAsBytes);
        }

        public static string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
            return System.Text.UTF8Encoding.UTF8.GetString(encodedDataAsBytes);
        }

        public static string EncodeTo64(byte[] toEncodeAsBytes)
        {
            return System.Convert.ToBase64String(toEncodeAsBytes);
        }

        public static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.UTF8Encoding.UTF8.GetBytes(toEncode);
            return System.Convert.ToBase64String(toEncodeAsBytes);
        } 
        #endregion

        #region Gzip/Ungzip

        public static string Gzip(string source)
        {
            using (var outStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(outStream, CompressionMode.Compress))
                {
                    using (var mStream = new MemoryStream(Encoding.UTF8.GetBytes(source)))
                    {
                        mStream.WriteTo(gzipStream);
                    }
                }

                return StringUtil.EncodeTo64(outStream.ToArray());
            }
        }

        public static string Ungzip(string source)
        {
            byte[] bytes = Convert.FromBase64String(source);

            using (GZipStream stream = new GZipStream(new MemoryStream(bytes), CompressionMode.Decompress))
            {
                const int size = 512;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    } while (count > 0);

                    return System.Text.Encoding.UTF8.GetString(memory.ToArray());
                }
            }
        } 
        #endregion

        #region LoadGzippedViewState
        /// <summary>
        /// 加载Gzipped的ViewState
        /// </summary>
        /// <param name="gzippedState"></param>
        /// <returns></returns>
        public static object LoadGzippedViewState(string gzippedState)
        {
            string ungzippedState = StringUtil.Ungzip(gzippedState);
            LosFormatter formatter = new LosFormatter();
            return formatter.Deserialize(ungzippedState);
        }

        /// <summary>
        /// 生成Gzipped的ViewState
        /// </summary>
        /// <param name="viewState"></param>
        /// <returns></returns>
        public static string GenerateGzippedViewState(object viewState)
        {
            LosFormatter formatter = new LosFormatter();
            using (StringWriter writer = new StringWriter())
            {
                formatter.Serialize(writer, viewState);
                return StringUtil.Gzip(writer.ToString());
            }
        } 
        #endregion

    }
}
