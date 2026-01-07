using System.Linq.Expressions;

namespace TechWayFit.ContentOS.Abstractions.Filtering;

/// <summary>
/// Filter specification for querying entities
/// </summary>
public sealed class FilterSpecification<T> where T : class
{
    public Expression<Func<T, bool>>? Criteria { get; set; }
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public List<(Expression<Func<T, object>> KeySelector, bool Descending)> OrderBy { get; } = new();
    public int? Skip { get; set; }
    public int? Take { get; set; }

    public FilterSpecification<T> Where(Expression<Func<T, bool>> criteria)
    {
        Criteria = Criteria == null ? criteria : CombineExpressions(Criteria, criteria);
        return this;
    }

    public FilterSpecification<T> Include(Expression<Func<T, object>> include)
    {
        Includes.Add(include);
        return this;
    }

    public FilterSpecification<T> AddOrderBy(Expression<Func<T, object>> keySelector, bool descending = false)
    {
        OrderBy.Add((keySelector, descending));
        return this;
    }

    public FilterSpecification<T> WithPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        return this;
    }

    private static Expression<Func<T, bool>> CombineExpressions(
        Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(first.Parameters[0], parameter);
        var left = leftVisitor.Visit(first.Body);

        var rightVisitor = new ReplaceExpressionVisitor(second.Parameters[0], parameter);
        var right = rightVisitor.Visit(second.Body);

        return Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(left!, right!), parameter);
    }

    private class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression? Visit(Expression? node)
        {
            return node == _oldValue ? _newValue : base.Visit(node);
        }
    }
}
