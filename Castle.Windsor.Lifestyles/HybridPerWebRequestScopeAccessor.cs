using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Lifestyle.Scoped;
using System.Web;
using Castle.Facilities.AspNet.SystemWeb;

namespace Castle.MicroKernel.Lifestyle {
    public class HybridPerWebRequestScopeAccessor: IScopeAccessor {
        private readonly IScopeAccessor webRequestScopeAccessor = new WebRequestScopeAccessor();
        private readonly IScopeAccessor secondaryScopeAccessor;

        public HybridPerWebRequestScopeAccessor(IScopeAccessor secondaryScopeAccessor) {
            this.secondaryScopeAccessor = secondaryScopeAccessor;
        }

        public ILifetimeScope GetScope(Context.CreationContext context) {
            if (HttpContext.Current != null && PerWebRequestLifestyleModuleUtils.IsInitialized)
            {
                try
                {
                    return webRequestScopeAccessor.GetScope(context);
                }
                catch (ObjectDisposedException)
                {
                    // If we get here, it means that we have a HttpContext, but the web request has ended.
                    // In this case we fall back to the secondary scope accessor.
                }
            }

            return secondaryScopeAccessor.GetScope(context);
        }

        public void Dispose() {
            webRequestScopeAccessor.Dispose();
            secondaryScopeAccessor.Dispose();
        }
    }
}
