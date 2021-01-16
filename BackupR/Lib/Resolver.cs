using Config.Net;
using System;
using Tekook.BackupR.Lib.Configs;
using Tekook.BackupR.Lib.Contracts;
using Tekook.VerbR.Resolvers;

namespace Tekook.BackupR.Lib
{
    internal class Resolver
    {
        public static T ResolveConfig<T>(IOptions options)
        {
            var resolver = new ConfigNetResolver<IConfig<T>, IOptions>((b) => b.UseJsonFile(options.Config));
            return resolver.Resolve(options).Provider;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="config"></param>
        /// <param name="options"></param>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="IConfig.Type"/> does not implement <see cref="IProvider"/>.</exception>
        /// <returns></returns>
        public static IProvider ResolveProvider(IConfig config, IOptions options)
        {
            Type t = Type.GetType(config.Type);
            if (!typeof(IProvider).IsAssignableFrom(t))
            {
                throw new InvalidOperationException("Invalid type supplied. Must implement " + nameof(IProvider));
            }
            return (IProvider)Activator.CreateInstance(t, new object[] { options });
        }
    }
}