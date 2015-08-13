﻿using System;
using System.Collections.Generic;
using System.Text;
using IPTables.Net.Iptables.Helpers;

namespace IPTables.Net.Iptables.Modules.Log
{
    public class LogModule : ModuleBase, IEquatable<LogModule>, IIpTablesModule
    {
        private const String OptionPrefixLong = "--log-prefix";
        private const String OptionLevelLong = "--log-level";


        public int LogLevel = 7;
        public String LogPrefix;

        public LogModule(int version) : base(version)
        {
        }

        public bool Equals(LogModule other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(LogPrefix, other.LogPrefix) && LogLevel == other.LogLevel;
        }


        public int Feed(RuleParser parser, bool not)
        {
            switch (parser.GetCurrentArg())
            {
                case OptionPrefixLong:
                    LogPrefix = parser.GetNextArg();
                    return 1;
                case OptionLevelLong:
                    LogLevel = int.Parse(parser.GetNextArg());
                    return 1;
            }

            return 0;
        }

        public bool NeedsLoading
        {
            get { return false; }
        }

        public String GetRuleString()
        {
            var sb = new StringBuilder();

            if (LogPrefix != null)
            {
                sb.Append(OptionPrefixLong + " ");
                sb.Append(ShellHelper.EscapeArguments(LogPrefix));
            }

            if (sb.Length != 0) sb.Append(" ");
            sb.Append(OptionLevelLong + " ");
            sb.Append(LogLevel);

            return sb.ToString();
        }

        public static HashSet<String> GetOptions()
        {
            var options = new HashSet<string>
            {
                OptionLevelLong,
                OptionPrefixLong
            };
            return options;
        }

        public static ModuleEntry GetModuleEntry()
        {
            return GetTargetModuleEntryInternal("LOG", typeof (LogModule), GetOptions);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((LogModule) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((LogPrefix != null ? LogPrefix.GetHashCode() : 0)*397) ^ LogLevel;
            }
        }
    }
}