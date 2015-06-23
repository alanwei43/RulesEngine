using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;

namespace RulesEngine
{
    public class ValidationReport : IValidationReport
    {
        private bool _hasErrors = false;
        private readonly Dictionary<object, Dictionary<EquatableExpression, List<ValidationError>>> _errors
            = new Dictionary<object, Dictionary<EquatableExpression, List<ValidationError>>>(new ReferenceEqualityComparer());
        private IEngine _engine;

        public ValidationReport()
        {
        }

        public ValidationReport(IEngine engine)
        {
            if (engine == null) throw new System.ArgumentNullException("engine");
            _engine = engine;
        }

        public bool HasError<T, R>(T value, Expression<Func<T, R>> expression, out ValidationError[] validationErrors)
        {
            return HasError(value, new EquatableExpression(expression), out validationErrors);
        }

        public bool HasError(object value, EquatableExpression expression)
        {
            ValidationError[] errors;
            return HasError(value, expression, out errors);
        }

        private bool HasError(object value, EquatableExpression expression, out ValidationError[] validationErrors)
        {
            validationErrors = null;
            if (value == null) throw new System.ArgumentNullException("value");
            Dictionary<EquatableExpression, List<ValidationError>> errorsOnValue;
            if (!_errors.TryGetValue(value, out errorsOnValue)) return false;

            //If we don't care on which expression the error occured, return true.
            if (expression == null)
            {
                //Get all validation errors for that value.
                validationErrors = errorsOnValue.SelectMany(k => k.Value).ToArray();
                return true;
            }

            List<ValidationError> errorsOnExpression;
            if (!errorsOnValue.TryGetValue(expression, out errorsOnExpression)) return false;
            validationErrors = errorsOnExpression.ToArray();
            return true;
        }

        public void AddError(ValidationError validationError)
        {
            _hasErrors = true;
            Dictionary<EquatableExpression, List<ValidationError>> errorsOnValue;
            if (!_errors.TryGetValue(validationError.Value, out errorsOnValue))
            {
                //NOTE: Explicit use of the AppliesToComparer here.
                errorsOnValue = new Dictionary<EquatableExpression, List<ValidationError>>(EquatableExpression.AppliesToComparer);
                _errors[validationError.Value] = errorsOnValue;
            }

            List<ValidationError> errorsOnExpression;
            if (!errorsOnValue.TryGetValue(validationError.Expression, out errorsOnExpression))
            {
                errorsOnExpression = new List<ValidationError>();
                errorsOnValue[validationError.Expression] = errorsOnExpression;
            }

            errorsOnExpression.Add(validationError);
        }

        public bool HasErrors
        {
            get { return _hasErrors; }
        }

        public bool Validate(object value)
        {
            var engine = GetEngine();
            _errors.Clear();
            _hasErrors = false;
            engine.Validate(value, this, ValidationReportDepth.FieldShortCircuit);
            return !_hasErrors;
        }

        private IEngine GetEngine()
        {
            if (_engine == null) throw new InvalidOperationException("Report does not have an Engine. Use the overloaded constructor.");
            return _engine;
        }

        public string GetErrorMessage<T, R>(T value, Expression<Func<T, R>> expression)
        {
            return GetErrorMessage(value, new EquatableExpression(expression), GetEngine().ErrorResolver);
        }
        public string GetErrorMessage<T, R>(T value, Expression<Func<T, R>> expression, IErrorResolver resolver)
        {
            return GetErrorMessage(value, new EquatableExpression(expression), resolver);
        }
        public string GetErrorMessage(object value, EquatableExpression expression, IErrorResolver resolver)
        {
            //NOTE: Expression can be null.
            if (value == null) throw new System.ArgumentNullException("value");
            if (resolver == null) throw new System.ArgumentNullException("resolver");

            ValidationError[] errors;
            ValidationError error;
            object[] validationArguments = new object[0];

            if (this.HasError(value, expression, out errors))
            {
                error = errors[0];
                validationArguments = error.ValidationArguments;
                var message = resolver.GetErrorMessage(error.OriginatingValue.GetType(), error.OriginatingExpression, error.Rule, error.ValidationArguments);
                return message;
            }
            else if (expression != null && expression.Expression.ReturnType.IsClass && expression.Expression.ReturnType != typeof(string))
            {
                //There may be the case that for compositions, a message is set for CallValidate(). When
                //this happens, there won't be any error for a => a.C (where 'C' is another object)
                //still, you would like to have a 'C is not valid' message...

                //Step 1: find out if there are any Messages defined for that property
                var message = resolver.GetErrorMessage(value.GetType(), expression, null, new object[0]);
                //if null fallback on message for the type of that property.
                if (message == null) message = resolver.GetErrorMessage(expression.Expression.ReturnType, null, null, new object[0]);
                if (message == null) return null;

                //Step 2: There is a Message defined. Find out if there is an error for the property.
                object propertyValue = null;
                try
                {
                    //TODO: This should be resolved using a Interface maybe.
                    propertyValue = expression.Expression.Compile().DynamicInvoke(value);
                }
                catch (NullReferenceException)
                {
                    //NOTE: Swallow all NullReferenceException.
                }
                catch (Exception ex)
                {
                    //Wrap and re-throw.
                    throw new InvalidOperationException(string.Format("Could not get property value for expression {0}. See InnerException for details.", expression.Expression), ex);
                }
                if (propertyValue == null) return null;
                
                //Step 3: The property may be an IEnumerable. Need to return the message if any elements has an error...
                //TODO: This is bit dodgy, you should only enumerate if a CallValidateForEach was used...
                //Return null if none of the propertyValue(s) have errors.
                if (ToEnumerable(propertyValue).All(v => !HasError(v, null))) return null;
                return message;
            }

            return null;
        }

        private IEnumerable<object> ToEnumerable(object value)
        {
            yield return value;
            if (value is IEnumerable)
            {
                foreach (var v in (IEnumerable)value)
                {
                    yield return v;
                }
            }
        }

        public string[] GetErrorMessages()
        {
            return GetErrorMessages(GetEngine().ErrorResolver);
        }

        public string[] GetErrorMessages(IErrorResolver errorResolver)
        {
            if (errorResolver == null) throw new System.ArgumentNullException("errorResolver");
            object[] errorObjects = _errors.Keys.ToArray();
            var result = errorObjects.SelectMany(o => GetErrorMessages(o, errorResolver)).ToArray();
            return result;
        }

        public string[] GetErrorMessages(object value)
        {
            if (value == null) throw new System.ArgumentNullException("value");
            return GetErrorMessages(value, GetEngine().ErrorResolver);
        }

        public string[] GetErrorMessages(object value, IErrorResolver errorResolver)
        {
            if (errorResolver == null) throw new System.ArgumentNullException("errorResolver");
            if (value == null) throw new System.ArgumentNullException("value");
            var result = new List<string>();
            Dictionary<EquatableExpression, List<ValidationError>> errorsOnValue;
            if (!_errors.TryGetValue(value, out errorsOnValue)) return new string[0];
            foreach (var expression in errorsOnValue.Keys)
            {
                foreach (var errorOnExpression in errorsOnValue[expression])
                {
                    var message = errorResolver.GetErrorMessage(errorOnExpression.Value.GetType(), errorOnExpression.Expression, errorOnExpression.Rule, errorOnExpression.ValidationArguments);
                    if (message == null) continue;
                    result.Add(message);
                }
            }
            return result.ToArray();
        }
    }
}
