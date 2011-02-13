﻿using System;
using System.Collections.Generic;
using System.Linq;
using NChurn.Core.Adapters;

namespace NChurn.Core.Analyzers
{
    public class Analyzer
    {
        private readonly IAdapterResolver _adapterResolver;

        public static Analyzer Create()
        {
            //todo: ioc resolve
            return new Analyzer(new AdapterResolver());
        }
        

        internal Analyzer(IAdapterResolver adapterResolver)
        {
            _adapterResolver = adapterResolver;
        }

        public AnalysisResult Analyze(DateTime backTo)
        {
            IEnumerable<string> changedResources = GetChangedResources(backTo);

            return AnalyzeChangedResources(changedResources);
        }
        public AnalysisResult Analyze()
        {
            IEnumerable<string> changedResources = GetChangedResources();

            return AnalyzeChangedResources(changedResources);
        }

        private static AnalysisResult AnalyzeChangedResources(IEnumerable<string> changedResources)
        {

            var d = new Dictionary<string, int>();

            foreach (var x in changedResources)
            {
                if (!d.ContainsKey(x)) d[x] = 0; d[x]++;
            }
            return new AnalysisResult {FileChurn = d.OrderByDescending(x=>x.Value).ThenBy( x=>x.Key)};
        }

        private IEnumerable<string> GetChangedResources()
        {
            IVersioningAdapter versioningAdapter = _adapterResolver.CreateAdapter();
            return versioningAdapter.ChangedResources();
        }

        private IEnumerable<string> GetChangedResources(DateTime backTo)
        {
            IVersioningAdapter versioningAdapter = _adapterResolver.CreateAdapter();
            return versioningAdapter.ChangedResources(backTo);
        }


    }
}


//
// .net 40 analyzer map/reduce. taken out because of tools not being ready / hacky (ncover, specifying framework dir in parameters)
//

/*
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NChurn.Core.Adapters;

namespace NChurn.Core.Analyzers
{
    public class Analyzer
    {
        private readonly IAdapterResolver _adapterResolver;

        public static Analyzer Create()
        {
            //todo: ioc resolve
            return new Analyzer(new AdapterResolver());
        }
        

        internal Analyzer(IAdapterResolver adapterResolver)
        {
            _adapterResolver = adapterResolver;
        }

        public AnalysisResult Analyze(DateTime backTo)
        {
            IEnumerable<string> changedResources = GetChangedResources(backTo);

            return AnalyzeChangedResources(changedResources);
        }
        public AnalysisResult Analyze()
        {
            IEnumerable<string> changedResources = GetChangedResources();

            return AnalyzeChangedResources(changedResources);
        }

        private static AnalysisResult AnalyzeChangedResources(IEnumerable<string> changedResources)
        {
            var concurrentDictionary = new ConcurrentDictionary<Guid, Dictionary<string, int>>();

            Parallel.ForEach(Partitioner.Create(changedResources), () => new Dictionary<string, int>(),
                             (x, s, i, d) => { if (!d.ContainsKey(x)) d[x] = 0; d[x]++; return d; }, x => concurrentDictionary[Guid.NewGuid()] = x);


            var r = new Dictionary<string, int>();
            Reduce(concurrentDictionary, r);
            return new AnalysisResult {FileChurn = r.OrderByDescending(x=>x.Value).ThenBy( x=>x.Key)};
        }

        private IEnumerable<string> GetChangedResources()
        {
            IVersioningAdapter versioningAdapter = _adapterResolver.CreateAdapter();
            return versioningAdapter.ChangedResources();
        }

        private IEnumerable<string> GetChangedResources(DateTime backTo)
        {
            IVersioningAdapter versioningAdapter = _adapterResolver.CreateAdapter();
            return versioningAdapter.ChangedResources(backTo);
        }

        private static void Reduce(ConcurrentDictionary<Guid, Dictionary<string, int>> concurrentDictionary, Dictionary<string, int> r)
        {
            foreach (var d in concurrentDictionary)
            {
                MergeResult(d.Value, r);
            }
        }

        private static void MergeResult(Dictionary<string, int> d, Dictionary<string, int> r)
        {
            foreach (var kp in d)
            {
                if (!r.ContainsKey(kp.Key))
                    r[kp.Key] = 0;
                r[kp.Key] = r[kp.Key] + kp.Value;
            }
        }
    }
}
*/