using System;
using System.Collections.Specialized;
using System.Web;

namespace Business.PayOne
{
    public class NvCodec : NameValueCollection
    {
        private const string Lf = "\n";
        private const string Ampersand = "&";
        private new const string Equals = "=";
        private static readonly char[] AmpersandCharArray = Ampersand.ToCharArray();
        private static readonly char[] LfCharArray = Lf.ToCharArray();
        private static readonly char[] EqualsCharArray = Equals.ToCharArray();

        /// <summary>
        /// Decoding the string
        /// </summary>
        /// <param name="nvpString"></param>
        public void Decode(string nvpString)
        {
            Clear();
            
            var str = nvpString.Split(nvpString.Contains(Lf) ? LfCharArray : AmpersandCharArray);
            foreach (var nvp in str)
            {
                var tokens = nvp.Split(EqualsCharArray,2);
                if (tokens.Length < 2) 
                    continue;
                
                var name = UrlDecode(tokens[0]);
                var value = UrlDecode(tokens[1]);
                Add(name, value);
            }
        }

        private static string UrlDecode(string s) { return HttpUtility.UrlDecode(s); }
        
        #region Array methods
        public void Add(string name, string value, int index)
        {
            this.Add(GetArrayName(index, name), value);
        }

        public void Remove(string arrayName, int index)
        {
            this.Remove(GetArrayName(index, arrayName));
        }

        /// <summary>
        /// 
        /// </summary>
        public string this[string name, int index]
        {
            get => this[GetArrayName(index, name)];
            set => this[GetArrayName(index, name)] = value;
        }

        private static string GetArrayName(int index, string name)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "index can not be negative : " + index);
            }
            return name + index;
        }
        #endregion    }

    }
}