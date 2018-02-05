using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DapperRepository
{
    public class ExpressionHelper
    {
        public virtual IDictionary<string, object> GetWhereParemeters<T>(Expression<Func<T, bool>> expression) where T : BaseEntity
        {
            IDictionary<string, object> dictionaryParams = new Dictionary<string, object>();
            FillQueryProperties(expression.Body, ExpressionType.Default, ref dictionaryParams);

            return dictionaryParams;
        }

        private void FillQueryProperties(Expression expr, ExpressionType linkingType, ref IDictionary<string, object> dictionaryParams)
        {
            if (expr is MethodCallExpression body)
            {
                var innerBody = body;
                var methodName = innerBody.Method.Name;
                switch (methodName)
                {
                    case "Contains":
                        {
                            var propertyName = GetPropertyNamePath(innerBody, out var isNested);
                            var propertyValue = GetValuesFromCollection(innerBody);
                            dictionaryParams.Add(propertyName, propertyValue);

                            break;
                        }
                    default:
                        throw new NotSupportedException($"'{methodName}' method is not supported");
                }
            }
            else if (expr is BinaryExpression innerbody)
            {
                if (innerbody.NodeType != ExpressionType.AndAlso && innerbody.NodeType != ExpressionType.OrElse)
                {
                    var propertyName = GetPropertyNamePath(innerbody, out var isNested);
                    var propertyValue = GetValue(innerbody.Right);
                    dictionaryParams.Add(propertyName, propertyValue);
                }
                else
                {
                    FillQueryProperties(innerbody.Left, innerbody.NodeType, ref dictionaryParams);
                    FillQueryProperties(innerbody.Right, innerbody.NodeType, ref dictionaryParams);
                }
            }
            else
            {
                FillQueryProperties(GetBinaryExpression(expr), linkingType, ref dictionaryParams);
            }
        }

        public static string GetPropertyName<TSource, TField>(Expression<Func<TSource, TField>> field)
        {
            if (Equals(field, null))
                throw new ArgumentNullException(nameof(field), "field can't be null");

            MemberExpression expr;

            switch (field.Body)
            {
                case MemberExpression body:
                    expr = body;
                    break;
                case UnaryExpression expression:
                    expr = (MemberExpression)expression.Operand;
                    break;
                default:
                    throw new ArgumentException("Expression field isn't supported", nameof(field));
            }

            return expr.Member.Name;
        }

        public static object GetValue(Expression member)
        {
            var objectMember = Expression.Convert(member, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();
            return getter();
        }

        public static BinaryExpression GetBinaryExpression(Expression expression)
        {
            var binaryExpression = expression as BinaryExpression;
            var body = binaryExpression ?? Expression.MakeBinary(ExpressionType.Equal, expression, expression.NodeType == ExpressionType.Not ? Expression.Constant(false) : Expression.Constant(true));
            return body;
        }

        public static object GetValuesFromCollection(MethodCallExpression callExpr)
        {
            var expr = callExpr.Object as MemberExpression;

            if (!(expr?.Expression is ConstantExpression))
                throw new NotSupportedException(callExpr.Method.Name + " isn't supported");

            var constExpr = (ConstantExpression)expr.Expression;

            var constExprType = constExpr.Value.GetType();
            return constExprType.GetField(expr.Member.Name).GetValue(constExpr.Value);
        }


        public static MemberExpression GetMemberExpression(Expression expression)
        {
            switch (expression)
            {
                case MethodCallExpression expr:
                    return (MemberExpression)expr.Arguments[0];

                case MemberExpression memberExpression:
                    return memberExpression;

                case UnaryExpression unaryExpression:
                    return (MemberExpression)unaryExpression.Operand;

                case BinaryExpression binaryExpression:
                    var binaryExpr = binaryExpression;

                    if (binaryExpr.Left is UnaryExpression left)
                        return (MemberExpression)left.Operand;

                    //should we take care if right operation is memberaccess, not left?
                    return (MemberExpression)binaryExpr.Left;

                case LambdaExpression expression1:
                    var lambdaExpression = expression1;

                    switch (lambdaExpression.Body)
                    {
                        case MemberExpression body:
                            return body;
                        case UnaryExpression expressionBody:
                            return (MemberExpression)expressionBody.Operand;
                    }
                    break;
            }

            return null;
        }

        /// <summary>
        ///     Gets the name of the property.
        /// </summary>
        /// <param name="expr">The Expression.</param>
        /// <param name="nested">Out. Is nested property.</param>
        /// <returns>The property name for the property expression.</returns>
        public static string GetPropertyNamePath(Expression expr, out bool nested)
        {
            var path = new StringBuilder();
            var memberExpression = GetMemberExpression(expr);
            var count = 0;
            do
            {
                count++;
                if (path.Length > 0)
                    path.Insert(0, "");
                path.Insert(0, memberExpression.Member.Name);
                memberExpression = GetMemberExpression(memberExpression.Expression);
            } while (memberExpression != null);

            if (count > 2)
                throw new ArgumentException("Only one degree of nesting is supported");

            nested = count == 2;

            return path.ToString();
        }
    }
}