using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace WebResultsClient.Configuration
{
    public static class ConfigurationConverter
    {
        public static object GetSerializableConfigurationFrom(IConfiguration configuration)
        {
            var rootChildren = configuration.GetChildren().ToList();

            if (IsList(rootChildren))
            {
                return RecursiveListConfig(rootChildren);
            }
            else
            {
                return RecursiveDictConfig(rootChildren);
            }
        }

        private static List<object> RecursiveListConfig(List<IConfigurationSection> children)
        {
            List<object> list = new List<object>();

            foreach (var child in children)
            {
                if (child.Value != null)
                {
                    list.Add(child.Value);
                }
                else
                {
                    var subChildren = child.GetChildren().ToList();

                    if (IsList(subChildren))
                    {
                        list.Add(RecursiveListConfig(subChildren));
                    }
                    else
                    {
                        list.Add(RecursiveDictConfig(subChildren));
                    }
                }
            }

            return list;
        }

        private static Dictionary<string, object> RecursiveDictConfig(List<IConfigurationSection> children)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (var child in children)
            {
                if (child.Value != null)
                {
                    dict[child.Key] = child.Value;
                }
                else
                {
                    var subChildren = child.GetChildren().ToList();

                    if (IsList(subChildren))
                    {
                        dict[child.Key] = RecursiveListConfig(subChildren);
                    }
                    else
                    {
                        dict[child.Key] = RecursiveDictConfig(subChildren); ;
                    }
                }
            }

            return dict;
        }
        private static bool IsList(List<IConfigurationSection> subChildren)
        {
            var isList = true;
            for (int i = 0; i < subChildren.Count; i++)
            {
                if (subChildren[i].Key != i.ToString())
                {
                    isList = false;
                }
            }

            return isList;
        }
    }
}
