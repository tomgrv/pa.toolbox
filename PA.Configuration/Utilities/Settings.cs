using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace PA.Configuration
{
    /// <summary>
    /// Inspired by Qt Project
    /// </summary>
    public class Settings : IDisposable
    {
        #region Import Ini

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        private static extern int GetPrivateProfileString(string section,
            string key, string def, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        private static extern int GetPrivateProfileSectionNames(byte[] lpszReturnBuffer,
           int nSize, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        // Note that because the key/value pars are returned as null-terminated
        // strings with the last string followed by 2 null-characters, we cannot
        // use StringBuilder.
        private static extern int GetPrivateProfileSection(string section,
              IntPtr lpszReturnBuffer, int nSize, string filePath);


        #endregion

        public enum Status
        {
            NoError = 0,
            AccessError = 1,
            FormatError = 2,
        }

        public enum Format
        {
            NativeFormat = 0,
            IniFormat = 1,
            InvalidFormat = 16,
            CustomFormat1 = 17,
            CustomFormat2 = 18,
            CustomFormat3 = 19,
            CustomFormat4 = 20,
            CustomFormat5 = 21,
            CustomFormat6 = 22,
            CustomFormat7 = 23,
            CustomFormat8 = 24,
            CustomFormat9 = 25,
            CustomFormat10 = 26,
            CustomFormat11 = 27,
            CustomFormat12 = 28,
            CustomFormat13 = 29,
            CustomFormat14 = 30,
            CustomFormat15 = 31,
            CustomFormat16 = 32,
        }
        public enum Scope
        {
            UserScope = 0,
            SystemScope = 1,
        }

        public Settings(string organization, string application)
        {
            this.Group = "";
            this.CurrentFormat = DefaultFormat;
            this.CurrentScope = Scope.UserScope;
            this.OrganizationName = organization;
            this.ApplicationName = application;
            this.SetFileName();
        }

        public Settings(string organization)
        {
            this.Group = "";
            this.CurrentFormat = DefaultFormat;
            this.CurrentScope = Scope.UserScope;
            this.OrganizationName = organization;
            this.ApplicationName = GetAttribute<AssemblyProductAttribute>();
            this.SetFileName();
        }

        public Settings(Settings.Scope scope, string organization, string application)
        {
            this.Group = "";
            this.CurrentFormat = DefaultFormat;
            this.CurrentScope = scope;
            this.OrganizationName = organization;
            this.ApplicationName = application;
            this.SetFileName();
        }

        public Settings(Settings.Scope scope, string organization)
        {
            this.Group = "";
            this.CurrentFormat = DefaultFormat;
            this.CurrentScope = scope;
            this.OrganizationName = organization;
            this.ApplicationName = GetAttribute<AssemblyProductAttribute>();
            this.SetFileName();
        }

        public Settings(Settings.Format format, Settings.Scope scope, string organization, string application)
        {
            this.Group = "";
            this.CurrentFormat = format;
            this.CurrentScope = scope;
            this.OrganizationName = organization;
            this.ApplicationName = application;
            this.SetFileName();
        }

        public Settings(Settings.Format format, Settings.Scope scope, string organization)
        {
            this.Group = "";
            this.CurrentFormat = format;
            this.CurrentScope = scope;
            this.OrganizationName = organization;
            this.ApplicationName = GetAttribute<AssemblyProductAttribute>();
            this.SetFileName();
        }

        public Settings(string fileName, Settings.Format format)
        {
            this.Group = "";
            this.FileName = Path.GetFullPath(fileName);
            this.CurrentFormat = format;
            this.CurrentScope = Scope.UserScope;
            this.OrganizationName = GetAttribute<AssemblyCompanyAttribute>();
            this.ApplicationName = GetAttribute<AssemblyProductAttribute>();
        }

        public Settings()
        {
            this.Group = "";
            this.CurrentFormat = DefaultFormat;
            this.CurrentScope = Scope.UserScope;
            this.OrganizationName = GetAttribute<AssemblyCompanyAttribute>(); ;
            this.ApplicationName = GetAttribute<AssemblyProductAttribute>();
            this.SetFileName();
        }


        public Settings.Status status() { throw new NotImplementedException(); }

        #region Group

        public string Group { get; protected set; }

        public void BeginGroup(string g)
        {
            this.Group += (this.Group is string && this.Group.Length > 0 ? "/" : "") + g;
        }

        public void EndGroup()
        {
            if (this.Group.Contains('/'))
            {
                this.Group = this.Group.Substring(0, this.Group.LastIndexOf('/'));
            }
            else
            {
                this.Group = "";
            }
        }

        #endregion

        #region Array

        protected bool write;
        protected string array;
        protected int index = -1;

        public void SetArrayIndex(int index)
        {
            this.index = index;
        }

        public int BeginReadArray(string a)
        {
            string value = this.Value(a + "/size");
            if (value.Length > 0)
            {
                int size = int.Parse(value);
                this.write = false;
                this.index = -1;
                this.array = a;
                return size;
            }
            else
            {
                return 0;
            }
        }

        public void BeginWriteArray(string a, int size = -1)
        {
            this.write = size < 0;
            this.array = a;
            this.index = -1;
        }

        public void EndArray()
        {
            this.SetValue(this.array + "/size", this.index.ToString());
            this.array = "";
            this.index = -1;
        }

        #endregion

        #region Discovery

        public string[] AllKeys()
        {
            return this.subkeys()
                .ToArray();
        }

        public string[] ChildKeys()
        {
            return this.subkeys()
                .Where(key => !key.Contains('/'))
                .Distinct()
                .ToArray();
        }

        private IEnumerable<string> subkeys()
        {
            switch (this.CurrentFormat)
            {
                case Format.IniFormat:


                    List<string> temp = new List<string>();
                    IntPtr pBuffer = Marshal.AllocHGlobal(32767);

                    int size = GetPrivateProfileSection(this.Group.Contains('/') ? this.Group.Substring(0, this.Group.IndexOf('/')) : this.Group, pBuffer, 32767, this.FileName);

                    // iStartAddress will point to the first character of the buffer,
                    int iStartAddress = pBuffer.ToInt32();
                    // iEndAddress will point to the last null char in the buffer.
                    int iEndAddress = iStartAddress + size * sizeof(char);

                    // Navigate through pBuffer.
                    while (iStartAddress < iEndAddress)
                    {
                        // Get the current string which starts at "iStartAddress".
                        string strCurrent = Marshal.PtrToStringAuto(new IntPtr(iStartAddress));

                        // Strip comments
                        if (!strCurrent.StartsWith("#") && !strCurrent.StartsWith(";"))
                        {
                            string key = strCurrent.Substring(0, strCurrent.IndexOf('='));

                            if (key.Trim().Length > 0)
                            {
                                yield return key.Substring(this.Group.Contains('/') ? this.Group.IndexOf('/') : 0);
                            }
                        }

                        // Make "iStartAddress" point to the next string.
                        iStartAddress += (strCurrent.Length + 1) * sizeof(char);
                    }

                    Marshal.FreeHGlobal(pBuffer);
                    pBuffer = IntPtr.Zero;


                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public string[] ChildGroups()
        {
            switch (this.CurrentFormat)
            {
                case Format.IniFormat:

                    if (File.Exists(this.FileName))
                    {
                        if (this.Group.Length == 0)
                        {
                            byte[] temp = new byte[1024];
                            GetPrivateProfileSectionNames(temp, temp.Length, this.FileName);
                            return System.Text.Encoding.Unicode
                                .GetString(temp)
                                .Split('\0');
                        }
                        else
                        {
                            return this.subkeys()
                                .Where(key => key.Contains('/'))
                                .Select(key => key.Substring(0, key.IndexOf('/') - 1))
                                .Distinct()
                                .ToArray();
                        }
                    }
                    else
                    {
                        throw new FileNotFoundException("Ini file '" + this.FileName + "' not found");
                    }

                case Format.NativeFormat:

                    string[] rpath = this.FileName.Split(new char[] { '\\' }, 2);


                    switch (rpath[0])
                    {
                        case "HKEY_LOCAL_MACHINE":
                            return Registry.LocalMachine.OpenSubKey(rpath[1]).GetSubKeyNames();

                        case "HKEY_CURRENT_USER":
                            return Registry.CurrentUser.OpenSubKey(rpath[1]).GetSubKeyNames();

                        default:
                            throw new NotSupportedException("Path " + this.FileName + " not supported ");
                    }


                default:
                    throw new NotSupportedException("Format " + this.CurrentFormat + " not supported ");
            }
        }

        #endregion

        public bool IsWritable() { throw new NotImplementedException(); }

        #region Values

        private Dictionary<string, string> values = new Dictionary<string, string>();

        public void Clear()
        {
            this.values.Clear();
        }

        public void Sync()
        {
            foreach (KeyValuePair<string, string> item in this.values)
            {
                if (item.Value is string)
                {
                    string[] hash = this.GetHashArray(item.Key);

                    switch (this.CurrentFormat)
                    {
                        case Format.IniFormat:
                            WritePrivateProfileString(hash[0], hash[1], item.Value.ToString(), this.FileName);
                            break;

                        case Format.NativeFormat:
                            Microsoft.Win32.Registry.SetValue(this.FileName + (hash[0] is string && hash[0].Length > 0 ? "\\" + hash[0] : ""), hash[1], item.Value);
                            break;
                    }
                }
            }


        }

        public string Value(string key)
        {
            return this.Value(key, "");
        }

        public string Value(string key, object d)
        {
            string field;
            string hashk = GetHashKey(key, out field);

            switch (this.CurrentFormat)
            {
                case Format.IniFormat:

                    if (File.Exists(this.FileName))
                    {
                        StringBuilder temp = new StringBuilder(255);

                        try
                        {
                            GetPrivateProfileString(this.Group.Length > 0 ? this.Group : this.ApplicationName, field, d.ToString(), temp, temp.MaxCapacity, this.FileName);
                        }
                        catch
                        {
                            throw new InvalidOperationException("Cannot open Ini file '" + this.FileName + "'");
                        }

                        this.SetValue(key, temp.Length > 0 ? temp.ToString() : null);
                    }
                    else
                    {
                        throw new FileNotFoundException("Ini file '" + this.FileName + "' not found");
                    }
                    break;


                case Format.NativeFormat:

                    object data = Registry.GetValue(this.FileName + (this.Group is string && this.Group.Length > 0 ? "\\" + this.Group : ""), key, d);
                    if (data is object)
                    {
                        this.values[hashk] = data.ToString();
                    }
                    break;
            }

            return this.values[hashk];
        }

        public void SetValue(string key, string value)
        {
            string field;

            key = GetHashKey(key, out  field);

            if (this.values.ContainsKey(key))
            {
                this.values[key] = value;
            }
            else
            {
                this.values.Add(key, value);
            }
        }

        #endregion

        public void Remove(string key) { throw new NotImplementedException(); }

        public bool Contains(string key)
        {
            return this.Value(key) is object;
        }

        public void SetFallbacksEnabled(bool b) { throw new NotImplementedException(); }
        public bool FallbacksEnabled() { throw new NotImplementedException(); }


        public string FileName { get; private set; }

        public Settings.Format CurrentFormat { get; private set; }
        public Settings.Scope CurrentScope { get; private set; }
        public string OrganizationName { get; private set; }
        public string ApplicationName { get; private set; }


        //public void SetIniCodec(object codec);
        //public void SetIniCodec(string codecName);
        //public object IniCodec();

        public void Dispose()
        {
            this.Sync();
        }

        #region Path

        //public static void SetDefaultFormat(Settings.Format format);
        public static Settings.Format DefaultFormat { get; set; }
        private static Dictionary<KeyValuePair<Format, Scope>, string> DefaultPath;

        public static void SetPath(Settings.Format format, Settings.Scope scope, string path)
        {
            KeyValuePair<Format, Scope> kvp = new KeyValuePair<Format, Scope>(format, scope);

            if (!(DefaultPath is Dictionary<KeyValuePair<Format, Scope>, string>))
            {
                DefaultPath = new Dictionary<KeyValuePair<Format, Scope>, string>();
            }

            DefaultPath.Remove(kvp);
            DefaultPath.Add(kvp, path);
        }

        #endregion

        #region Private Helpers

        private string GetHashKey(string k, out string field)
        {
            string g = this.Group is string && this.Group.Length > 0 ? this.Group : "";
            string a = this.array is string && this.array.Length > 0 ? this.array : "";
            string i = this.index < 0 ? "" : this.index.ToString();

            if (a.Length > 0 && i.Length > 0)
            {
                field = a + "/" + i + "/" + k;
            }
            else if (a.Length > 0)
            {
                field = a + "/" + k;
            }
            else
            {
                field = k;
            }

            return g + "/" + field;
        }

        private string[] GetHashArray(string h)
        {
            return h.Split(new char[] { '/' }, 2);
        }

        private void SetFileName()
        {
            this.FileName = DefaultPath[new KeyValuePair<Format, Scope>(this.CurrentFormat, this.CurrentScope)];
            this.FileName += "\\" + this.OrganizationName + "\\" + this.ApplicationName;

            if (this.CurrentFormat == Format.IniFormat)
            {
                this.FileName += ".ini";
            }
        }

        private static void InitPath()
        {
            if (!(DefaultPath is Dictionary<KeyValuePair<Format, Scope>, string>))
            {
                DefaultPath = new Dictionary<KeyValuePair<Format, Scope>, string>();
            }

            DefaultPath.Clear();

            DefaultPath.Add(new KeyValuePair<Format, Scope>(Format.IniFormat, Scope.SystemScope),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));

            DefaultPath.Add(new KeyValuePair<Format, Scope>(Format.IniFormat, Scope.UserScope),
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

            DefaultPath.Add(new KeyValuePair<Format, Scope>(Format.NativeFormat, Scope.SystemScope),
                "HKEY_LOCAL_MACHINE\\SOFTWARE\\");

            DefaultPath.Add(new KeyValuePair<Format, Scope>(Format.NativeFormat, Scope.UserScope),
                "HKEY_CURRENT_USER\\SOFTWARE\\");
        }

        private static string GetAttribute<T>() where T : Attribute
        {
            Assembly a = Assembly.GetEntryAssembly();

            if (a == null)
            {
                a = Assembly.GetCallingAssembly();
            }

            T attribute = (a is Assembly) ? (T)Attribute.GetCustomAttribute(a, typeof(T)) : null;

            PropertyInfo[] pis = typeof(T).GetProperties();

            if (pis is PropertyInfo[] && attribute is T)
            {
                PropertyInfo pi = pis.First<PropertyInfo>();
                return pi.GetValue(attribute, null).ToString();
            }

            return string.Empty;
        }

        #endregion
    }
}
