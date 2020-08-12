﻿// NLog.Targets.Fluentd
// 
// Copyright (c) 2014 Moriyoshi Koizumi and contributors.
// 
// This file is licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using NLog;
using NLog.Targets.Fluentd;

namespace Demo
{
    static class Program
    {
        static void Main()
        {
            try
            {
                var config = new NLog.Config.LoggingConfiguration();
                using (var fluentdTarget = new Fluentd())
                {
                    fluentdTarget.Layout = new NLog.Layouts.SimpleLayout("${longdate}|${level}|${callsite}|${logger}|${message}");
                    config.AddTarget("fluentd", fluentdTarget);
                    config.LoggingRules.Add(new NLog.Config.LoggingRule("demo", LogLevel.Debug, fluentdTarget));
                    var loggerFactory = new LogFactory(config);
                    var logger = loggerFactory.GetLogger("demo");
                    logger.Info("Hello World!");
                }
            }
            catch (NLogConfigurationException exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
