using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web;
using Castle.MicroKernel.Lifestyle.Scoped;
using Castle.Facilities.AspNet.SystemWeb;

namespace Castle.MicroKernel.Lifestyle {
    public class HybridPerWebRequestScopeAccessor: IScopeAccessor {
        private readonly IScopeAccessor webRequestScopeAccessor = new WebRequestScopeAccessor();
        private readonly IScopeAccessor secondaryScopeAccessor;

        public HybridPerWebRequestScopeAccessor(IScopeAccessor secondaryScopeAccessor) {
            this.secondaryScopeAccessor = secondaryScopeAccessor;
        }

        public ILifetimeScope GetScope(Context.CreationContext context)
        {
            // If webRequestScope is disposed, then it means that we have a HttpContext, but the web request has ended.
            // We fall back to the secondary scope accessor in this case too.

            return HttpContext.Current != null && PerWebRequestLifestyleModuleUtils.IsInitialized &&
                webRequestScopeAccessor.GetScope(context) is ILifetimeScope webRequestScope && !IsScopeDisposed(webRequestScope)
                ? webRequestScope
                : secondaryScopeAccessor.GetScope(context);
        }

        public void Dispose() {
            webRequestScopeAccessor.Dispose();
            secondaryScopeAccessor.Dispose();
        }

        /// <summary>
        /// Tests the given <paramref name="scope"/> whether it has been disposed.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Due to a change in Castle.Windsor's code, the code which uses reflection here cannot determine whether the given <paramref name="scope"/> has been disposed.
        /// </exception>
        private bool IsScopeDisposed(ILifetimeScope scope)
        {
            const string SCOPE_CACHE_FIELD_NAME = "scopeCache";

            var scopeCacheField = scope.GetType().GetField(SCOPE_CACHE_FIELD_NAME, BindingFlags.NonPublic | BindingFlags.Instance) ??
                throw new InvalidOperationException($"{GetWindsorVersionErrorMessage()} ('{SCOPE_CACHE_FIELD_NAME}' field not found in '{scope.GetType()}')");

            var scopeCacheFieldValue = scopeCacheField.GetValue(scope) ??
                throw new InvalidOperationException($"{GetWindsorVersionErrorMessage()} ('{SCOPE_CACHE_FIELD_NAME}' field's value is null)");

            var scopeCache = scopeCacheFieldValue as IScopeCache ??
                throw new InvalidOperationException($"{GetWindsorVersionErrorMessage()} ('{SCOPE_CACHE_FIELD_NAME}' field's type is '{scopeCacheField.FieldType}' instead of '{typeof(IScopeCache)}')");

            try
            {
                // actually use the scopeCache to test whether it has been disposed, and provoke the ObjectDisposedException in that case
                _ = scopeCache[string.Empty];
                return false;
            }
            catch (ObjectDisposedException)
            {
                return true;
            }

            string GetWindsorVersionErrorMessage() => $"Castle.Windsor version has been updated. Please fix the code of {GetType().Assembly.FullName}";
        }
    }
}
