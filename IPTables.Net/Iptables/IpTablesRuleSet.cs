﻿using System;
using System.Collections.Generic;
using System.Linq;
using IPTables.Net.Netfilter;
using IPTables.Net.Netfilter.Sync;

namespace IPTables.Net.Iptables
{
    public class IpTablesRuleSet
    {
        private readonly IpTablesChainSet _chains = new IpTablesChainSet();

        private readonly IpTablesSystem _system;

        public IpTablesRuleSet(IpTablesSystem system)
        {
            _system = system;
        }

        public IpTablesRuleSet(List<string> rules, IpTablesSystem system)
        {
            _system = system;
            foreach (string s in rules)
            {
                AddRule(s);
            }
        }

        public IEnumerable<IpTablesChain> Chains
        {
            get { return _chains.Chains; }
        }

        public IpTablesChainSet ChainSet
        {
            get { return _chains; }
        }


        public void AddRule(IpTablesRule rule)
        {
            IpTablesChain ipchain = _chains.GetChainOrAdd(rule.Chain);

            ipchain.Rules.Add(rule);
        }

        public IpTablesRule AddRule(String rawRule)
        {
            IpTablesRule rule = IpTablesRule.Parse(rawRule, _system, _chains);

            AddRule(rule);

            return rule;
        }


        public void AddChain(String name, String table)
        {
            if (_chains.HasChain(name, table))
            {
                throw new Exception("A chain with that name already exists");
            }

            _chains.AddChain(new IpTablesChain(table, name, _system));
        }

        public void SyncChains(INetfilterSync<IpTablesRule> sync,
            Func<IpTablesChain, bool> canDeleteChain = null)
        {
            //Start transaction
            _system.Adapter.StartTransaction();
            
            var tableChains = new Dictionary<string, List<IpTablesChain>>();
            foreach (IpTablesChain chain in Chains)
            {
                if (!tableChains.ContainsKey(chain.Table))
                {
                    var chains = _system.GetChains(chain.Table).ToList();
                    tableChains.Add(chain.Table, chains);
                }
                if (tableChains[chain.Table].FirstOrDefault(a => a.Name == chain.Name && a.Table == chain.Table) == null)
                {
                    //Chain doesnt exist create
                    tableChains[chain.Table].Add(_system.AddChain(chain));
                }
            }

            foreach (IpTablesChain chain in Chains)
            {
                IpTablesChain realChain =
                    tableChains[chain.Table].First(a => a.Name == chain.Name && a.Table == chain.Table);
                if (realChain != null)
                {
                    //Update chain
                    realChain.SyncInternal(chain.Rules, sync);
                }
            }

            if (canDeleteChain != null)
            {
                foreach (string table in Chains.Select(a => a.Table).Distinct())
                {
                    foreach (IpTablesChain chain in _system.GetChains(table))
                    {
                        if (!_chains.HasChain(chain.Name, chain.Table) && canDeleteChain(chain))
                        {
                            chain.Delete();
                        }
                    }
                }
            }

            //End Transaction: COMMIT
            _system.Adapter.EndTransactionCommit();
        }
    }
}