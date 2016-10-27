﻿// OpenCppCoverage is an open source code coverage for C++.
// Copyright (C) 2014 OpenCppCoverage
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenCppCoverage.VSPackage;

namespace VSPackage_IntegrationTests
{
    [TestClass()]
    public class RunCoverageTests: TestHelpers
    {
        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void DoesNotCompile()
        {
            OpenSolution(CppConsoleApplication2);
            var outputMessage = GetOpenCppCoverageMessage(RunCoverage);
            CheckMessage(CoverageRunner.BuilderFailedMsg, outputMessage);
            WaitEndOfBuild();
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void InvalidProgramToRun()
        {
            OpenSolution(CppConsoleApplication);
            using (var debugSettings = 
                SolutionConfigurationHelpers.GetCurrentDebugSettings(
                            CppConsoleApplication))
            {
                var settings = debugSettings.Value;
                settings.Command = "Invalid";
                var outputMessage = GetOpenCppCoverageMessage(RunCoverage);

                CheckMessage(
                    string.Format(CoverageRunner.InvalidProgramToRunMsg, settings.Command), 
                    outputMessage);
            }
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void DisableCompileBeforeRunning()
        {
            OpenSolution(CppConsoleApplication);
            SolutionConfigurationHelpers.CleanSolution();
            var controller = ExecuteOpenCppCoverageCommand();
            controller.BasicSettingController.CompileBeforeRunning = false;
                
            RunInUIhread(() => 
            {
                var outputMessage = GetOpenCppCoverageMessage(() => RunCoverageCommand(controller));

                Assert.IsTrue(outputMessage.EndsWith(CoverageRunner.InvalidValueForProgramToRun));
            });
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void CheckCoverageX86()
        {
            OpenSolution(
                CppConsoleApplication, 
                ConfigurationName.Debug, 
                PlatFormName.Win32);
            var coverageTreeController = RunCoverageAndWait();
            var root = coverageTreeController.Root;

            Assert.IsTrue(root.CoveredLineCount > 0);
            Assert.IsTrue(root.UncoveredLineCount > 0);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void CheckCoverageX64()
        {
            OpenSolution(
                CppConsoleApplication,
                ConfigurationName.Debug,
                PlatFormName.x64);
            var coverageTreeController = RunCoverageAndWait();
            var root = coverageTreeController.Root;

            Assert.IsTrue(root.CoveredLineCount > 0);
            Assert.IsTrue(root.UncoveredLineCount > 0);
        }

        //---------------------------------------------------------------------
        static void CheckMessage(string expectedMessage, string outputMessage)
        {
            Assert.AreEqual("OpenCppCoverage\n\n" + expectedMessage, outputMessage);
        }
    }
}
