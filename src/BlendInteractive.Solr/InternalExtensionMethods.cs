using SolrNet.Attributes;
using System.Linq.Expressions;
using System.Reflection;

namespace BlendInteractive.Solr
{
    internal static class InternalExtensionMethods
    {

        private static MemberInfo GetMemberInfo<T>(Expression<T> expression)
        {
            return expression.Body switch
            {
                MemberExpression m => m.Member,
                UnaryExpression u when u.Operand is MemberExpression m2 => m2.Member,
                _ => throw new NotImplementedException(expression.GetType().ToString())
            };
        }

        public static string GetMemberSolrName<T>(this Expression<T> expression)
        {
            var member = GetMemberInfo(expression);
            var field = member.GetCustomAttribute<SolrFieldAttribute>();
            if (field is null)
                throw new InvalidOperationException($"Could not find a `SolrFieldAttribute` for member {member.Name}");
            return field.FieldName;
        }
    }
}
