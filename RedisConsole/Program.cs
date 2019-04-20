using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RedisConsole
{
    class Program
    {
        private static readonly RedisEndpoint _redisEndpoint;

        static Program()
        {
            var host = "localhost";
            var port = 6379;
            _redisEndpoint = new RedisEndpoint(host, port);
        }
        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("1. Set String");
            Console.WriteLine("2. Get String");
            Console.WriteLine("3. Key Exists");
            Console.WriteLine("4. Set Entity");
            Console.WriteLine("5. Get Entity");
            Console.WriteLine("6. Append To Value");
            Console.WriteLine("7. Remove Key");
            Console.WriteLine("8. Remove Keys");
            Console.WriteLine();
            Console.WriteLine("0. Exit");
            Console.WriteLine("---------------------------");
            Console.WriteLine("Enter Your Selection:");
            var k = Convert.ToInt32(Console.ReadLine());
            Program p = new Program();
            while (k != 0)
            {
                switch (k)
                {
                    case 1:
                        Console.WriteLine("SET OPERATION");
                        Console.WriteLine("Enter Key");
                        string keySet = Console.ReadLine();
                        Console.WriteLine("Enter Value");
                        string valueSet = Console.ReadLine();
                        p.Set(keySet,valueSet);
                        Console.WriteLine("SET SUCCESSFULLY.");
                        break;
                    case 2:
                        Console.WriteLine("GET OPERATION");
                        Console.WriteLine("Enter Key");
                        string keyGet = Console.ReadLine();
                        string resultGet= p.Get(keyGet);
                        Console.WriteLine("Result:" +resultGet);
                        break;
                    case 3:
                        Console.WriteLine("IS EXISTS OPERATION");
                        Console.WriteLine("Enter Key");
                        string keyExists = Console.ReadLine();
                        bool resultExists = p.KeyExists(keyExists);
                        Console.WriteLine("Result:" + resultExists);
                        break;
                    case 4:
                        Console.WriteLine("SET ENTITY OPERATION");
                        Console.WriteLine("Enter Key");
                        string keySetE = Console.ReadLine();
                        Console.WriteLine("Enter UserName For Entity");
                        string valueSetE1 = Console.ReadLine();
                        Console.WriteLine("Enter Password For Entity");
                        string valueSetE2 = Console.ReadLine();
                        User user = new User();
                        user.UserName = valueSetE1;
                        user.Password = valueSetE2;
                        p.SetEntity(keySetE,user,TimeSpan.MaxValue);
                        Console.WriteLine("SET SUCCESSFULLY.");
                        break;
                    case 5:
                        Console.WriteLine("GET ENTITY OPERATION");
                        Console.WriteLine("Enter Key");
                        string keyGetE = Console.ReadLine();
                        var userE = p.GetEntity<User>(keyGetE);
                        Console.WriteLine("Result: " + userE.UserName + " " + userE.Password);
                        break;
                    case 6:
                        Console.WriteLine("APPEND TO VALUE OPERATION");
                        Console.WriteLine("Enter SetId");
                        string setId = Console.ReadLine();
                        Console.WriteLine("Enter Item");
                        string item = Console.ReadLine();
                        p.AppendToValue(setId,item);
                        Console.WriteLine("SET SUCCESSFULLY.");
                        break;
                    case 7:
                        Console.WriteLine("REMOVE KEY OPERATION");
                        Console.WriteLine("Enter Key");
                        string keyRemove = Console.ReadLine();
                        p.GetEntity<User>(keyRemove);
                        Console.WriteLine("KEY REMOVED SUCCESSFULLY ");
                        break;
                    case 8:
                        Console.WriteLine("REMOVE KEYS OPERATION");
                        Console.WriteLine("Enter Keys (Comma Seperated)");
                        string keysRemove = Console.ReadLine();
                        p.Remove(keysRemove.Split(',').Select(x => x));
                        Console.WriteLine("KEYS REMOVED SUCCESSFULLY ");
                        break;
                    default:
                        break;
                }
                Console.WriteLine("---------------------");
                Console.WriteLine("Enter Your Selection:");
                k = Convert.ToInt32(Console.ReadLine());
            }
        }

        public bool KeyExists(string key)
        {
            using (var redisClient = new RedisClient(_redisEndpoint))
            {
                if (redisClient.ContainsKey(key))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public void Set(string key, string value)
        {
            using (var redisClient = new RedisClient(_redisEndpoint))
            {
                redisClient.SetValue(key, value);
            }
        }

        public string Get(string key)
        {
            using (var redisClient = new RedisClient(_redisEndpoint))
            {
                return redisClient.GetValue(key);
            }
        }

        public bool SetEntity<T>(string key, T value, TimeSpan timeout)
        {
            try
            {
                using (var redisClient = new RedisClient(_redisEndpoint))
                {
                    redisClient.As<T>().SetValue(key, value, timeout);
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public T GetEntity<T>(string key)
        {
            T result;

            using (var client = new RedisClient(_redisEndpoint))
            {
                var wrapper = client.As<T>();

                result = wrapper.GetValue(key);
            }

            return result;
        }

        public bool AppendToValue(string key, string value)
        {
            try
            {
                using (var redisClient = new RedisClient(_redisEndpoint))
                {
                    redisClient.AppendToValue(key, value);
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Remove(string key)
        {
            try
            {
                using (var redisClient = new RedisClient(_redisEndpoint))
                {
                    redisClient.Remove(key);
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Remove(IEnumerable<string> keys)
        {
            try
            {
                using (var redisClient = new RedisClient(_redisEndpoint))
                {
                    redisClient.RemoveAll(keys);
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long Increment(string key)
        {
            using (var client = new RedisClient(_redisEndpoint))
            {
                return client.Increment(key, 1);
            }
        }

        public long Decrement(string key)
        {
            using (var client = new RedisClient(_redisEndpoint))
            {
                return client.Decrement(key, 1);
            }
        }
    }

    public class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
