using System.Collections;
using System.Collections.Specialized;
using System.Text;

namespace Glass.Core.Database.Builders {
    class MySQLErrorBuilder {

        private StringDictionary properties;

        public MySQLErrorBuilder() { 
            properties = new StringDictionary();
        }

        public int GetPropertyAmount() {
            return properties.Count;
        }

        public void AddProperty(string propertyName, string propertyValue) {
            properties.Add(propertyName, propertyValue);
        }

        public string GetEmptyPropertiesError() {
            StringBuilder error = new StringBuilder("Campo(s) ");
            foreach (DictionaryEntry entry in properties)
                error.Append("{" + entry.Key + "} ");

            error.Append("não pode(m) ser vazio(s)");
            return error.ToString();
        }

    }
}
