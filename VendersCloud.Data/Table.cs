using System.Linq.Expressions;
using VendersCloud.Common.Extensions;

namespace VendersCloud.Data.Data
{
    public class Table<T> where T : new() {

        private string _alias;
        private string _tableName;
        private string _tableAs;

        public Table(string alias = null) {
            _alias = alias;
            _tableName = GetTableName();
            _tableAs = $"{_tableName} AS {_alias}";
        }

        public string TableName {
            get {
                return string.IsNullOrWhiteSpace(_alias) ? this._tableName : _tableAs;
            }
        }

        public string UnAliasedTableName {
            get {
                return this._tableName;
            }
        }

        string GetTableName() {
            var objType = typeof(T);
            object[] attributes = objType.GetCustomAttributes(true);
            foreach (var attr in attributes) {
                AliasAttribute tableName = attr as AliasAttribute;
                if (tableName != null)
                    return tableName.Name;
            }
            return objType.Name;
        }

        public string Col(Expression<Func<T, object>> expression, string alias = null) {
            var prop = string.IsNullOrWhiteSpace(_alias) 
               ? PropertyName(expression)
               : $"{_alias}.{PropertyName(expression)}";
            return !string.IsNullOrWhiteSpace(alias) ? $"{prop} As {alias}" : prop;
        }

        public string ColWithTableName(Expression<Func<T, object>> expression, string alias = null) {
            var prop = string.IsNullOrWhiteSpace(_alias)
               ? $"{_tableName}.{PropertyName(expression)}"
               : $"{_alias}.{PropertyName(expression)}";
            return !string.IsNullOrWhiteSpace(alias) ? $"{prop} As {alias}" : prop;
        }

        public ColumnSelector<T> ColAs(Expression<Func<T, object>> expression) {
            var columnName = PropertyName(expression);
            return new ColumnSelector<T>(columnName);
        }

        public string[] Cols(params Expression<Func<T, object>>[] expressions) {
            var instance = new T();
            return string.IsNullOrWhiteSpace(_alias)
                ? instance.Props(expressions).ToArray()
                : instance.Props(expressions).Select(p => $"{_alias}.{p}").ToArray();
        }

        public string[] AllColsExcept(params Expression<Func<T, object>>[] expressions) {
            var instance = new T();
            return string.IsNullOrWhiteSpace(_alias)
                ? instance.PropsExcept(expressions).ToArray()
                : instance.PropsExcept(expressions).Select(p => $"{_alias}.{p}").ToArray();
        }

        public string[] AllColsExceptWithTableName(params Expression<Func<T, object>>[] expressions) {
            var instance = new T();
            return string.IsNullOrWhiteSpace(_alias)
                ? instance.PropsExcept(expressions).Select(p => $"{_tableName}.{p}").ToArray()
                : instance.PropsExcept(expressions).Select(p => $"{_alias}.{p}").ToArray();
        }

        public string AllCols {
            get {
                return string.IsNullOrWhiteSpace(_alias)
                    ? "*"
                    : $"{_alias}.*";
            }
        }

        public string Count(Expression<Func<T, object>> expression) {
            return string.IsNullOrWhiteSpace(_alias)
                ? $"COUNT({PropertyName(expression)})"
                : $"COUNT({_alias}.{PropertyName(expression)})";
        }

        public string Sum(Expression<Func<T, object>> expression) {
            return string.IsNullOrWhiteSpace(_alias)
                ? $"SUM({PropertyName(expression)})"
                : $"SUM({_alias}.{PropertyName(expression)})";
        }

        public string Avg(Expression<Func<T, object>> expression) {
            return string.IsNullOrWhiteSpace(_alias)
                ? $"AVG({PropertyName(expression)})"
                : $"AVG({_alias}.{PropertyName(expression)})";
        }

        public string Min(Expression<Func<T, object>> expression) {
            return string.IsNullOrWhiteSpace(_alias)
                ? $"MIN({PropertyName(expression)})"
                : $"MIN({_alias}.{PropertyName(expression)})";
        }

        public string Max(Expression<Func<T, object>> expression) {
            return string.IsNullOrWhiteSpace(_alias)
                ? $"MAX({PropertyName(expression)})"
                : $"MAX({_alias}.{PropertyName(expression)})";
        }

        string PropertyName(Expression<Func<T, object>> expression) {
            var body = expression.Body as MemberExpression;

            if (body == null) {
                body = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }

            return body.Member.Name;
        }

        public class ColumnSelector<T> {
            private readonly string _columnName;
            private string _alias;

            public ColumnSelector(string columnName) {
                _columnName = columnName;
            }

            public string As(string alias) {
                _alias = alias;
                return string.IsNullOrWhiteSpace(_alias) ? _columnName : $"{_columnName} AS {_alias}";
            }
        }
    }
}
