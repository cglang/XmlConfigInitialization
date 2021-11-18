﻿namespace XmlConfigInitialization
{
    public static class XmlConfigFactory
    {
        private static XmlConfig _xmlConfig;

        private static XmlOptions _option;

        private static readonly object _lock = new object();

        public static XmlConfig GetSingletonInstance()
        {
            if (_xmlConfig == null)
            {
                lock (_lock)
                {
                    if (_xmlConfig == null)
                    {
                        _xmlConfig = XmlConfig.CreateObject(_option);
                    }
                }
            }
            return _xmlConfig;
        }

        public static void InitXmlOptions(XmlOptions option)
        {
            _option = option;
        }
    }
}
