using Config.Net;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekook.BackupR.Lib;
using Tekook.BackupR.Lib.Configs;
using Tekook.BackupR.Lib.Contracts;
using Tekook.BackupR.Lib.Ftp;
using Tekook.VerbR.Resolvers;
using Tekook.VerbR.Validators;

namespace Tekook.BackupR.Test
{
    internal class TestVerb : VerbR.Verb<TestOptions, IConfig<IFtpConfig>>
    {
        public TestVerb(TestOptions options) : base(options)
        {
            this.Resolver = new ConfigNetResolver<IConfig<IFtpConfig>, TestOptions>((builder) => builder.UseJsonFile(options.Config));
            this.Validator = new FluentValidator<IConfig<IFtpConfig>>(v =>
            {
                v.RuleFor(x => x.Provider.Config.Host).NotEmpty();
                v.RuleFor(x => x.Provider.Config.Path).NotEmpty();
            });
        }

        public async override Task<int> InvokeAsync()
        {
            FtpProvider provider = new FtpProvider(this.Config.Provider.Config);
            IContainer root = await provider.Read();
            long max = 1024 * 1024 * 1024;
            foreach (IContainer sub in root.Containers)
            {
                Console.WriteLine($"Checking {sub.Name} with Size: {sub.Size.ToReadableBytes()}");
                if (sub.Size > max)
                {
                    var files = sub.Items.OrderBy(x => x.Date).ToList();
                    var toDelete = new List<IItem>();
                    while (files.Sum(x => x.Size) > max && files.Count > 0)
                    {
                        toDelete.Add(files[0]);
                        files.RemoveAt(0);
                    }
                    foreach (var item in toDelete)
                    {
                        Console.WriteLine($"Deleting {item.Path} -> {item.Size.ToReadableBytes()}");
                        await item.Delete();
                    }
                }
            }
            return 0;
        }
    }
}