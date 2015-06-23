using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RulesEngine.Fluent
{
    public interface IFluentNode
    {
        FluentScope FluentScope { get; }
        object GetSelf(FluentScope fluentScope);
    }

    internal static class BuilderHelper
    {
        public static T CreateFluentNode<T>(FluentScope scope)
        {
            return (T)CreateFluentNode(typeof(T), scope);
        }

        private static object CreateFluentNode(Type type, FluentScope scope)
        {
            var binding = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.CreateInstance;
            return type.GetConstructors(binding).First(c => c.GetParameters().Any(p => p.ParameterType == typeof(FluentScope))).Invoke(new[] { scope });
        }

        public static T CreateSetupNode<T>(IFluentNode node, System.Linq.Expressions.LambdaExpression expression)
        {
            var scope = node.FluentScope.CreateChild();
            scope.Set(RegistryKeys.SetupExpression, expression);
            //TODO: Get rid of FluentNodeParent (If I can - I think it is only used by EndSetup now. EndSetup should use the same principle as IfScope).
            scope.Set(RegistryKeys.FluentNodeParent, node);

            //Clear all unwanted inherited values.
            scope.Delete(RegistryKeys.Rule);
            scope.Delete(RegistryKeys.IsCrossField);
            scope.Delete(RegistryKeys.CulpritResolver);

            return (T)CreateFluentNode(typeof(T), scope);
        }


        public static void RegisterRule(FluentScope fluentScope, FluentBuilderToken token)
        {
            var rule = fluentScope.Get(RegistryKeys.Rule);
            var sourceType = fluentScope.Get(RegistryKeys.SourceType);
            var setupExpression = fluentScope.Get(RegistryKeys.SetupExpression, Utilities.ReturnSelf(sourceType));
            var valueResolverFactory = token.ValueResolverFactory;
            var isCrossField = fluentScope.Get(RegistryKeys.IsCrossField, false);
            var expressionToInvokeForValidation = isCrossField ? Utilities.ReturnSelf(sourceType) : fluentScope.Get(RegistryKeys.SetupExpression);
            var resultType = expressionToInvokeForValidation.ReturnType;
            var valueToValidateResolver = valueResolverFactory.CreateResolver(expressionToInvokeForValidation);

            var culpritResolver = fluentScope.Get(RegistryKeys.CulpritResolver, token.CulpritResolverFactory.Create(null, setupExpression));
            var result = (IRuleInvoker)Utilities.CreateType(typeof(RuleInvoker<,>), sourceType, resultType)
                                                .CreateInstance(rule
                                                                , valueToValidateResolver
                                                                , new EquatableExpression(setupExpression)
                                                                , culpritResolver
                                                                );

            token.CurrentEngine.InvokerRegistry.RegisterInvoker(result);
        }

        public static void RegisterComposition(FluentScope fluentScope, FluentBuilderToken token)
        {
            var usingEngine = fluentScope.Get(RegistryKeys.UsingEngine, token.RootEngine);
            var compositionExpression = fluentScope.Get(RegistryKeys.SetupExpression);
            var sourceType = compositionExpression.Parameters[0].Type;
            var resultType = compositionExpression.ReturnType;

            var invoker = (IRuleInvoker)Utilities.CreateType(typeof(CompositionInvoker<,>), sourceType, resultType)
                                                    .CreateInstance(usingEngine, compositionExpression);

            token.CurrentEngine.InvokerRegistry.RegisterInvoker(invoker);
        }
        public static void RegisterEnumerableComposition(FluentScope fluentScope, FluentBuilderToken token)
        {
            var usingEngine = fluentScope.Get(RegistryKeys.UsingEngine, token.RootEngine);
            var compositionExpression = fluentScope.Get(RegistryKeys.SetupExpression);
            var sourceType = compositionExpression.Parameters[0].Type;
            var resultType = compositionExpression.ReturnType;

            var result = (IRuleInvoker)Utilities.CreateType(typeof(EnumerableCompositionInvoker<,>), sourceType, resultType)
                                                    .CreateInstance(usingEngine, compositionExpression);

            token.CurrentEngine.InvokerRegistry.RegisterInvoker(result);
        }


        public static void RegisterConditional(FluentScope fluentScope, FluentBuilderToken token)
        {
            var conditionalExpression = fluentScope.Get(RegistryKeys.ConditionalExpression);
            var sourceType = conditionalExpression.Parameters[0].Type;
            var engine = token.CurrentEngine;
            var conditionalToken = token.Condition();

            var invoker = (IRuleInvoker)Utilities.CreateType(typeof(ConditionalInvoker<>), sourceType)
                                                    .CreateInstance(conditionalExpression, conditionalToken.IfTrueEngine, conditionalToken.IfFalseEngine);

            engine.InvokerRegistry.RegisterInvoker(invoker);
        }

        public static void RegisterMessage(FluentScope fluentScope, FluentBuilderToken token)
        {
            var sourceType = fluentScope.Get(RegistryKeys.SourceType);
            var messageEntry = new MessageEntry(
                sourceType,
                fluentScope.Get(RegistryKeys.SetupExpression, Utilities.ReturnSelf(sourceType)),
                fluentScope.Get(RegistryKeys.Rule, null),
                fluentScope.Get(RegistryKeys.Message));

            token.ErrorResolver.AddEntry(messageEntry);
        }

        public static void SwitchToElse(FluentScope fluentScope, FluentBuilderToken token)
        {
            token.Else();
        }

        public static ForClassElseEndIf<T, ENDIF> CreateIf<T, ENDIF>(FluentScope fluentScope, LambdaExpression conditionalExpression)
        {
            var tmpScope = fluentScope.CreateChild();
            tmpScope.Set(RegistryKeys.FluentScopeAction, new FluentScopeAction(BuilderHelper.RegisterConditional));
            tmpScope.Set(RegistryKeys.ConditionalExpression, conditionalExpression);
            tmpScope.Set(RegistryKeys.IfScope, fluentScope);

            var result = new ForClassElseEndIf<T, ENDIF>(tmpScope);
            return result;
        }

        public static TEndIf CreateEndIf<TEndIf>(FluentScope fluentScope)
        {
            var tmpScope = fluentScope.CreateChild(fluentScope.Get(RegistryKeys.IfScope));
            //TODO: Make all IFluentNodes have a new() constructor and an Initialize(FluentScope2) method
            return CreateFluentNode<TEndIf>(tmpScope);
        }

        public static TResult CreateMustPassRule<TRule, TResult>(IRule<TRule> rule, FluentScope fluentScope, bool isCrossField)
        {
            var tmpScope = fluentScope.CreateChild();
            tmpScope.Set(RegistryKeys.FluentScopeAction, new FluentScopeAction(BuilderHelper.RegisterRule));
            tmpScope.Set(RegistryKeys.Rule, rule);
            tmpScope.Set(RegistryKeys.IsCrossField, isCrossField);

            return CreateFluentNode<TResult>(tmpScope);
        }

        public static TResult CreateElseNode<TResult>(FluentScope fluentScope)
        {
            var tmpScope = fluentScope.CreateChild();
            tmpScope.Set(RegistryKeys.FluentScopeAction, new FluentScopeAction(BuilderHelper.SwitchToElse));
            return CreateFluentNode<TResult>(tmpScope);
        }

        public static TResult SetBlame<TResult>(TResult setupNode, LambdaExpression culpritExpression)
            where TResult : IFluentNode
        {
            return SetBlame(setupNode, null, culpritExpression);
        }


        public static TResult SetBlame<TResult>(TResult setupNode, LambdaExpression culpritObjectExpression, LambdaExpression culpritExpression)
            where TResult : IFluentNode
        {
            setupNode.FluentScope.Set(RegistryKeys.CulpritResolver,  new BlameCulpritResolver(culpritObjectExpression, culpritExpression));
            return setupNode;
        }
    }
}
