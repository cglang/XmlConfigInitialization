namespace XmlConfigInitialization
{
    public static class XmlConfigFactory
    {
        private static XmlConfig _xmlConfig;

        private static readonly object _lock = new object();

        public static XmlConfig GetSingletonInstance(XmlOptions option = default)
        {
            if (_xmlConfig == null)
            {
                lock (_lock)
                {
                    if (_xmlConfig == null)
                    {
                        _xmlConfig = XmlConfig.CreateObject(option);
                    }
                }
            }
            return _xmlConfig;
        }
    }
}
