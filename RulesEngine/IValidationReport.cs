using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine
{
    public interface IValidationReport
    {
        bool HasError<T, R>(T value, Expression<Func<T, R>> expression, out ValidationError[] validationErrors);
        bool HasError(object value, EquatableExpression expression);
        string GetErrorMessage<T, R>(T value, Expression<Func<T, R>> expression, IErrorResolver resolver);
        void AddError(ValidationError validationError);
        bool HasErrors { get; }
    }
}
