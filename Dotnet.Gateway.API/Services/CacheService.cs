using StackExchange.Redis;

namespace Dotnet.Gateway.API.Services
{
    public class CacheService
    {
        IDatabase? db;
        bool isConnected = false;
        ConnectionMultiplexer? redis;

        public CacheService(ConfigurationManager _configuration)
        {
            try
            {
                redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
                {
                    EndPoints = { _configuration.GetConnectionString("Redis") }
                });
                db = redis.GetDatabase();
                isConnected = true;
            } catch (Exception)
            {
                isConnected = false;
            }
        }

        public Boolean CheckKeyExist(string _key)
        {
            if (isConnected)
            {
                if (db!.KeyExists(_key))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            } else
            {
                return false;
            }
        }

        public void SetString(string _key, string _value)
        {
            if (isConnected)
            {
                db!.StringSet(_key, _value);
            }
        }
    }
}
