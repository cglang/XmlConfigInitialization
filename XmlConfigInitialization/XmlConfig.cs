using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace XmlConfigInitialization
{
    public class XmlConfig
    {
        private const string DefaultNode = "default";

        /// <summary>
        /// xml文档
        /// </summary>
        private readonly XmlDocument _doc = new XmlDocument();

        /// <summary>
        /// 配置文件全名
        /// </summary>
        private readonly string _fullName;

        /// <summary>
        /// 自动保存
        /// </summary>
        private readonly bool _autoSave;

        private XmlConfig(XmlOptions option = default)
        {
            if (option == default)
            {
                _fullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml");
                _autoSave = false;
            }
            else
            {
                _fullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, option.Directory, option.ConfigName);
                _autoSave = option.AutoSave;
            }

            if (!File.Exists(_fullName))
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));

                _doc.AppendChild(_doc.CreateXmlDeclaration("1.0", "UTF-8", null));
                _doc.AppendChild(_doc.CreateElement("root"));
                _doc.Save(_fullName);
            }

            _doc.Load(_fullName);
        }

        public XmlConfig(IOptions<XmlOptions> options)
        {
            _fullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, options.Value.Directory, options.Value.ConfigName);
            _autoSave = options.Value.AutoSave;


            if (!File.Exists(_fullName))
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, options.Value.Directory));

                _doc.AppendChild(_doc.CreateXmlDeclaration("1.0", "UTF-8", null));
                _doc.AppendChild(_doc.CreateElement("root"));
                _doc.Save(_fullName);
            }

            _doc.Load(_fullName);
        }

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <returns></returns>
        public static XmlConfig CreateObject(XmlOptions option = default)
        {
            return new XmlConfig(option);
        }

        /// <summary>
        /// 保存修改
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            try
            {
                _doc.Save(_fullName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region 同步版本

        /// <summary>
        /// 获取指定键的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public string GetValue(string key, string nodeName = DefaultNode)
        {
            try
            {
                CreateNode(nodeName);
                XmlElement selectEle = (XmlElement)_doc.DocumentElement?.SelectSingleNode($"/root/{nodeName}/item[@key='{key}']");
                return selectEle == null ? "" : selectEle.InnerText;
            }
            catch
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// 更改指定键的值 没有则添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public bool SetValue(string key, string value, string nodeName = DefaultNode)
        {
            try
            {
                CreateNode(nodeName);
                var keyValue = _doc.DocumentElement?.SelectSingleNode($"/root/{nodeName}");
                var node = (XmlElement)_doc.DocumentElement?.SelectSingleNode($"/root/{nodeName}/item[@key='{key}']");
                if (node == null)
                {
                    var newnode = _doc.CreateElement("item");
                    newnode.SetAttribute("key", key);
                    newnode.InnerText = value;
                    keyValue?.AppendChild(newnode);
                }
                else
                {
                    node.InnerText = value;
                }

                if (_autoSave) Save();
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取所有的键
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllKey(string nodeName = DefaultNode)
        {
            try
            {
                var keys = new List<string>();
                var selectEle = (XmlElement)_doc.DocumentElement?.SelectSingleNode($"/root/{nodeName}");
                if (selectEle != null)
                {
                    foreach (XmlElement node in selectEle)
                    {
                        keys.Add(node.GetAttribute("key"));
                    }
                }

                return keys;
            }
            catch
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// 获取所有的键
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllValue(string nodeName = DefaultNode)
        {
            try
            {
                var keys = new List<string>();
                var selectEle = (XmlElement)_doc.DocumentElement?.SelectSingleNode($"/root/{nodeName}");
                if (selectEle != null)
                {
                    foreach (XmlElement node in selectEle)
                    {
                        keys.Add(node.InnerText);
                    }
                }

                return keys;
            }
            catch
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// 获取所有的键/值
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetAllKeyValue(string nodeName = DefaultNode)
        {
            try
            {
                var keyValues = new Dictionary<string, string>();
                var selectEle = (XmlElement)_doc.DocumentElement?.SelectSingleNode($"/root/{nodeName}");
                if (selectEle != null)
                {
                    foreach (XmlElement node in selectEle)
                    {
                        keyValues.Add(node.GetAttribute("key"), node.InnerText);
                    }
                }

                return keyValues;
            }
            catch
            {
                return new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// 删除指定键的项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="nodeName"></param>
        public bool DeleteValue(string key, string nodeName = DefaultNode)
        {
            try
            {
                CreateNode(nodeName);
                var keyValue = _doc.DocumentElement?.SelectSingleNode($"/root/{nodeName}");
                var selectEle = _doc.DocumentElement?.SelectSingleNode($"/root/{nodeName}/item[@key='{key}']");
                if (selectEle != null) keyValue?.RemoveChild(selectEle);
                if (_autoSave) Save();
            }
            catch
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 获取所有的节点名称
        /// </summary>
        /// <returns></returns>
        public List<string> GetNodes()
        {
            try
            {
                var nodes = new List<string>();
                var selectEle = (XmlElement)_doc.DocumentElement?.SelectSingleNode($"/root");
                if (selectEle != null)
                {
                    foreach (XmlElement node in selectEle)
                    {
                        nodes.Add(node.Name);
                    }
                }

                return nodes;
            }
            catch
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public bool DeleteNode(string nodeName)
        {
            try
            {
                var keyValue = _doc.DocumentElement?.SelectSingleNode($"/root");
                var selectEle = _doc.DocumentElement?.SelectSingleNode($"/root/{nodeName}");

                if (selectEle != null)
                {
                    keyValue?.RemoveChild(selectEle);
                    if (_autoSave) Save();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检查创建节点
        /// </summary>
        /// <param name="nodeName"></param>
        private void CreateNode(string nodeName = DefaultNode)
        {
            if (_doc.DocumentElement?.SelectSingleNode($"/root/{nodeName}") == null)
            {
                _doc.DocumentElement?.AppendChild(_doc.CreateElement(nodeName));
            }
        }
        #endregion

        #region 异步版本

        /// <summary>
        /// 获取指定键的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public async Task<string> GetValueAsync(string key, string nodeName = DefaultNode)
        {
            string Func() => GetValue(key, nodeName);
            return await Task.Run(Func);
        }


        /// <summary>
        /// 更改指定键的值 没有则添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public async Task<bool> SetValueAsync(string key, string value, string nodeName = DefaultNode)
        {
            bool Func() => SetValue(key, value, nodeName);
            return await Task.Run(Func);
        }

        /// <summary>
        /// 获取所有的键
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetAllKeyAsync(string nodeName = DefaultNode)
        {
            List<string> Func() => GetAllKey(nodeName);
            return await Task.Run(Func);
        }

        /// <summary>
        /// 获取所有的键
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetAllValueAsync(string nodeName = DefaultNode)
        {
            List<string> Func() => GetAllValue(nodeName);
            return await Task.Run(Func);
        }

        /// <summary>
        /// 获取所有的键/值
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetAllKeyValueAsync(string nodeName = DefaultNode)
        {
            Dictionary<string, string> Func() => GetAllKeyValue(nodeName);
            return await Task.Run(Func);
        }

        /// <summary>
        /// 删除指定键的项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="nodeName"></param>
        public async Task<bool> DeleteValueAsync(string key, string nodeName = DefaultNode)
        {
            bool Func() => DeleteValue(key, nodeName);
            return await Task.Run(Func);
        }


        /// <summary>
        /// 获取所有的节点名称
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetNodesAsync()
        {
            List<string> Func() => GetNodes();
            return await Task.Run(Func);
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public async Task<bool> DeleteNodeAsync(string nodeName)
        {
            bool Func() => DeleteNode(nodeName);
            return await Task.Run(Func);
        }

        #endregion
    }
}
