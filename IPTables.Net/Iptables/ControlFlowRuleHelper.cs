﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInteract;
using IPTables.Net.Iptables.Modules.Core;

namespace IPTables.Net.Iptables
{
    public static class ControlFlowRuleHelper
    {
        public static IpTablesRule CreateJump(IpTablesChain chainIn, String chainJump, ISystemFactory system)
        {
            IpTablesRule rule = new IpTablesRule(system, chainIn);
            rule.GetModuleOrLoad<CoreModule>("core").Jump = chainJump;
            return rule;
        }

        public static IpTablesRule CreateGoto(IpTablesChain chainIn, String chainJump, ISystemFactory system)
        {
            IpTablesRule rule = new IpTablesRule(system, chainIn);
            rule.GetModuleOrLoad<CoreModule>("core").Goto = chainJump;
            return rule;
        }

        public static IpTablesRule CreateJump(IpTablesChain chain, String target)
        {
            return CreateJump(chain, target, chain.System.System);
        }

        public static IpTablesRule CreateGoto(IpTablesChain chain, String target)
        {
            return CreateGoto(chain, target, chain.System.System);
        }
    }
}
