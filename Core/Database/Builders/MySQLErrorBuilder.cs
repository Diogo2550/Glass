using System.Collections;
using System.Collections.Specialized;

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
            string error = "Campo(s) ";
            foreach (DictionaryEntry entry in properties)
                error += "{" + entry.Key + "} ";

            error += "não pode(m) ser vazio(s)";
            return error;
        }

    }
}
