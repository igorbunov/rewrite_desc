using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Collections.Specialized;

namespace Rewrite
{
    public class ResourcesLoadder
    {
        private string resourceNameSyn = "Rewrite.RusSyn.txt";
        private string resourceNameDic = "Rewrite.Dictionary.txt";
        public delegate void DoneLoadSyn(Dictionary<string, string> result);
        public delegate void DoneLoadDict(HashSet<string> result);
        public event DoneLoadSyn doneLoadSynonimsEvent;
        public event DoneLoadDict doneLoadDictionaryEvent;

        public void LoadSynonims()
        {
            var assembly = Assembly.GetExecutingAssembly();
            Dictionary<string, string> synonims = new Dictionary<string, string>();

            using (Stream stream = assembly.GetManifestResourceStream(resourceNameSyn))
            using (StreamReader reader = new StreamReader(stream))
            {
                string[] sysn = reader.ReadToEnd().Split('\n');

                foreach (string lane in sysn)
                {
                    string[] arr = lane.Split('|');

                    if (arr.Length == 2)
                    {                        
                        synonims.Add(arr[0], arr[1]);
                    }
                }

                sysn = null;
            }
            
            doneLoadSynonimsEvent(synonims);
        }

        public void LoadDictionary()
        {
            var assembly = Assembly.GetExecutingAssembly();
            HashSet<string> allowedWords = null;

            using (Stream stream = assembly.GetManifestResourceStream(resourceNameDic))
            using (StreamReader reader = new StreamReader(stream))
            {
                allowedWords = new HashSet<string>(reader.ReadToEnd().Split('\n'));
            }

            doneLoadDictionaryEvent(allowedWords);
        }
    }
}
