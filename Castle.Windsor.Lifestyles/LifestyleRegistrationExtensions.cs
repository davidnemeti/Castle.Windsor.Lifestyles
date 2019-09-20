using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration.Lifestyle;

namespace Castle.MicroKernel.Registration
{
    public static class LifestyleRegistrationExtensions
    {
        /// <see cref="PerWebSession{S}"/>
        public static ComponentRegistration<TService> LifestylePerWebSession<TService>(this ComponentRegistration<TService> registration)
            where TService : class =>
            registration.LifeStyle.PerWebSession();

        /// <see cref="PerHttpApplication{S}"/>
        public static ComponentRegistration<TService> LifestylePerHttpApplication<TService>(this ComponentRegistration<TService> registration)
            where TService : class =>
            registration.LifeStyle.PerHttpApplication();

        /// <see cref="HybridPerWebRequestTransient{S}"/>
        public static ComponentRegistration<TService> LifestyleHybridPerWebRequestTransient<TService>(this ComponentRegistration<TService> registration)
            where TService : class =>
            registration.LifeStyle.HybridPerWebRequestTransient();

        /// <see cref="HybridPerWebRequestPerThread{S}"/>
        public static ComponentRegistration<TService> LifestyleHybridPerWebRequestPerThread<TService>(this ComponentRegistration<TService> registration)
            where TService : class =>
            registration.LifeStyle.HybridPerWebRequestPerThread();

        /// <summary>
        /// One component instance per web session.
        /// Warning: because the session end event request only works InProc, components can't be reliably disposed. Burden is also affected.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="group"></param>
        /// <returns></returns>
        public static ComponentRegistration<S> PerWebSession<S>(this LifestyleGroup<S> @group)
            where S : class =>
            @group.Scoped<WebSessionScopeAccessor>();

        /// <summary>
        /// One component instance per HttpApplication instance.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="group"></param>
        /// <returns></returns>
        public static ComponentRegistration<S> PerHttpApplication<S>(this LifestyleGroup<S> @group)
            where S : class
            => @group.Scoped<HttpApplicationScopeAccessor>();

        /// <summary>
        /// One component instance per web request, or if HttpContext is not available, transient.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="group"></param>
        /// <returns></returns>
        public static ComponentRegistration<S> HybridPerWebRequestTransient<S>(this LifestyleGroup<S> @group)
            where S : class =>
            @group.Scoped<HybridPerWebRequestTransientScopeAccessor>();

        /// <summary>
        /// One component instance per web request, or if HttpContext is not available, one per thread.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="group"></param>
        /// <returns></returns>
        public static ComponentRegistration<S> HybridPerWebRequestPerThread<S>(this LifestyleGroup<S> @group)
            where S : class =>
            @group.Scoped<HybridPerWebRequestPerThreadScopeAccessor>();
    }
}