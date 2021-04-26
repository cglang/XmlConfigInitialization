using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace XmlConfigInitialization
{
    public class XmlConfig
    {
        public const string DEFAULT_NODE = "default";

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
        private readonly bool _autoSave = true;

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

        /// <summary>
        /// 获取指定键的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key, string nodeName = DEFAULT_NODE)
        {
            try
            {
                CreateNode(nodeName);
                XmlElement selectEle = (XmlElement)_doc.DocumentElement.SelectSingleNode($"/root/{nodeName}/item[@key='{key}']");
                return selectEle == null ? "" : selectEle.GetAttribute("value");
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 更改指定键的值 没有则添加
        /// </summary>
        /// <param name="xmlNode"></param>
        public bool SetValue(string key, string value, string nodeName = DEFAULT_NODE)
        {
            try
            {
                CreateNode(nodeName);
                XmlNode keyValue = _doc.DocumentElement.SelectSingleNode($"/root/{nodeName}");
                XmlElement node = (XmlElement)_doc.DocumentElement.SelectSingleNode($"/root/{nodeName}/item[@key='{key}']");
                if (node == null)
                {
                    XmlElement newnode = _doc.CreateElement("item");
                    newnode.SetAttribute("key", key);
                    newnode.SetAttribute("value", value);
                    keyValue.AppendChild(newnode);
                }
                else
                {
                    node.SetAttribute("value", value);
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
        public List<string> GetAllKey(string nodeName = DEFAULT_NODE)
        {
            try
            {
                List<string> keys = new List<string>();
                XmlElement selectEle = (XmlElement)_doc.DocumentElement.SelectSingleNode($"/root/{nodeName}");
                foreach (XmlElement node in selectEle)
                {
                    keys.Add(node.GetAttribute("key"));
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
        public List<string> GetAllValue(string nodeName = DEFAULT_NODE)
        {
            try
            {
                List<string> keys = new List<string>();
                XmlElement selectEle = (XmlElement)_doc.DocumentElement.SelectSingleNode($"/root/{nodeName}");
                foreach (XmlElement node in selectEle)
                {
                    keys.Add(node.GetAttribute("value"));
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
        public Dictionary<string, string> GetAllKeyValue(string nodeName = DEFAULT_NODE)
        {
            try
            {
                Dictionary<string, string> keyValues = new Dictionary<string, string>();
                XmlElement selectEle = (XmlElement)_doc.DocumentElement.SelectSingleNode($"/root/{nodeName}");
                foreach (XmlElement node in selectEle)
                {
                    keyValues.Add(node.GetAttribute("key"), node.GetAttribute("value"));
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
        public bool DeleteValue(string key, string nodeName = DEFAULT_NODE)
        {
            try
            {
                CreateNode(nodeName);
                XmlNode keyValue = _doc.DocumentElement.SelectSingleNode($"/root/{nodeName}");
                XmlNode selectEle = _doc.DocumentElement.SelectSingleNode($"/root/{nodeName}/item[@key='{key}']");
                keyValue.RemoveChild(selectEle);

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
                List<string> nodes = new List<string>();
                XmlElement selectEle = (XmlElement)_doc.DocumentElement.SelectSingleNode($"/root");
                foreach (XmlElement node in selectEle)
                {
                    nodes.Add(node.Name);
                }
                return nodes;
            }
            catch
            {
                return new List<string>();
            }
        }


        /// <summary>
        /// 检查创建节点
        /// </summary>
        /// <param name="nodeName"></param>
        private void CreateNode(string nodeName = DEFAULT_NODE)
        {
            if (_doc.DocumentElement.SelectSingleNode($"/root/{nodeName}") == null)
            {
                _doc.DocumentElement.AppendChild(_doc.CreateElement(nodeName));
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
                XmlNode keyValue = _doc.DocumentElement.SelectSingleNode($"/root");
                XmlNode selectEle = _doc.DocumentElement.SelectSingleNode($"/root/{nodeName}");

                if (selectEle != null)
                {
                    keyValue.RemoveChild(selectEle);
                    if (_autoSave) Save();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
