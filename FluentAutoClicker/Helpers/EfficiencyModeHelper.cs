// Copyright (C) 2025 Ryan Luu
//
// This file is part of Fluent Auto Clicker.
//
// Fluent Auto Clicker is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Fluent Auto Clicker is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with Fluent Auto Clicker. If not, see <https://www.gnu.org/licenses/>.

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace FluentAutoClicker.Helpers;
public static class EfficiencyModeHelper
{
    [DllImport("KERNEL32.dll", ExactSpelling = true)]
    private static extern IntPtr GetCurrentProcess();

    [DllImport("KERNEL32.dll", ExactSpelling = true, SetLastError = true)]
    private static extern unsafe bool SetProcessInformation(IntPtr hProcess, PROCESS_INFORMATION_CLASS processInformationClass, void* processInformation, uint processInformationSize);

    [DllImport("KERNEL32.dll", ExactSpelling = true, SetLastError = true)]
    private static extern bool SetPriorityClass(IntPtr hProcess, PROCESS_CREATION_FLAGS dwPriorityClass);

    public enum ProcessPriorityClass
    {
        Default,
        Idle,
        BelowNormal,
        Normal,
        AboveNormal,
        High,
        Realtime,
    }

    [SupportedOSPlatform("windows8.0")]
    public enum QualityOfServiceLevel
    {
        Default,
        [SupportedOSPlatform("windows10.0.16299.0")]
        High,
        [SupportedOSPlatform("windows10.0.16299.0")]
        Medium,
        [SupportedOSPlatform("windows10.0.16299.0")]
        Low,
        [SupportedOSPlatform("windows11.0.22621.0")]
        Utility,
        [SupportedOSPlatform("windows11.0")]
        Eco,
        [SupportedOSPlatform("windows10.0.19041.0")]
        Media,
        [SupportedOSPlatform("windows10.0.19041.0")]
        Deadline,
    }

    private enum PROCESS_INFORMATION_CLASS
    {
        ProcessMemoryPriority = 0,
        ProcessMemoryExhaustionInfo = 1,
        ProcessAppMemoryInfo = 2,
        ProcessInPrivateInfo = 3,
        ProcessPowerThrottling = 4,
        ProcessReservedValue1 = 5,
        ProcessTelemetryCoverageInfo = 6,
        ProcessProtectionLevelInfo = 7,
        ProcessLeapSecondInfo = 8,
        ProcessMachineTypeInfo = 9,
        ProcessOverrideSubsequentPrefetchParameter = 10,
        ProcessMaxOverridePrefetchParameter = 11,
        ProcessInformationClassMax = 12,
    }

    private enum PROCESS_CREATION_FLAGS : uint
    {
        DEBUG_PROCESS = 0x00000001,
        DEBUG_ONLY_THIS_PROCESS = 0x00000002,
        CREATE_SUSPENDED = 0x00000004,
        DETACHED_PROCESS = 0x00000008,
        CREATE_NEW_CONSOLE = 0x00000010,
        NORMAL_PRIORITY_CLASS = 0x00000020,
        IDLE_PRIORITY_CLASS = 0x00000040,
        HIGH_PRIORITY_CLASS = 0x00000080,
        REALTIME_PRIORITY_CLASS = 0x00000100,
        CREATE_NEW_PROCESS_GROUP = 0x00000200,
        CREATE_UNICODE_ENVIRONMENT = 0x00000400,
        CREATE_SEPARATE_WOW_VDM = 0x00000800,
        CREATE_SHARED_WOW_VDM = 0x00001000,
        CREATE_FORCEDOS = 0x00002000,
        BELOW_NORMAL_PRIORITY_CLASS = 0x00004000,
        ABOVE_NORMAL_PRIORITY_CLASS = 0x00008000,
        INHERIT_PARENT_AFFINITY = 0x00010000,
        INHERIT_CALLER_PRIORITY = 0x00020000,
        CREATE_PROTECTED_PROCESS = 0x00040000,
        EXTENDED_STARTUPINFO_PRESENT = 0x00080000,
        PROCESS_MODE_BACKGROUND_BEGIN = 0x00100000,
        PROCESS_MODE_BACKGROUND_END = 0x00200000,
        CREATE_SECURE_PROCESS = 0x00400000,
        CREATE_BREAKAWAY_FROM_JOB = 0x01000000,
        CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x02000000,
        CREATE_DEFAULT_ERROR_MODE = 0x04000000,
        CREATE_NO_WINDOW = 0x08000000,
        PROFILE_USER = 0x10000000,
        PROFILE_KERNEL = 0x20000000,
        PROFILE_SERVER = 0x40000000,
        CREATE_IGNORE_SYSTEM_DEFAULT = 0x80000000,
    }

    private partial struct PROCESS_POWER_THROTTLING_STATE
    {
        internal uint Version;
        internal uint ControlMask;
        internal uint StateMask;
    }

    private const uint PROCESS_POWER_THROTTLING_CURRENT_VERSION = 1U;
    private const uint PROCESS_POWER_THROTTLING_EXECUTION_SPEED = 1U;

    [SupportedOSPlatform("windows10.0.16299.0")]
    private static unsafe void SetProcessQualityOfServiceLevel(QualityOfServiceLevel level)
    {
        var powerThrottling = new PROCESS_POWER_THROTTLING_STATE
        {
            Version = PROCESS_POWER_THROTTLING_CURRENT_VERSION
        };

        switch (level)
        {
            // Let system manage all power throttling. ControlMask is set to 0 as we don’t want
            // to control any mechanisms.
            case QualityOfServiceLevel.Default:
                powerThrottling.ControlMask = 0;
                powerThrottling.StateMask = 0;
                break;

            // Turn EXECUTION_SPEED throttling on.
            // ControlMask selects the mechanism and StateMask declares which mechanism should be on or off.
#pragma warning disable CA1416 // Validate platform compatibility
            case QualityOfServiceLevel.Eco when Environment.OSVersion.Version >= new Version(11, 0):
#pragma warning restore CA1416 // Validate platform compatibility
            case QualityOfServiceLevel.Low:
                powerThrottling.ControlMask = PROCESS_POWER_THROTTLING_EXECUTION_SPEED;
                powerThrottling.StateMask = PROCESS_POWER_THROTTLING_EXECUTION_SPEED;
                break;

            // Turn EXECUTION_SPEED throttling off.
            // ControlMask selects the mechanism and StateMask is set to zero as mechanisms should be turned off.
            case QualityOfServiceLevel.High:
                powerThrottling.ControlMask = PROCESS_POWER_THROTTLING_EXECUTION_SPEED;
                powerThrottling.StateMask = 0;
                break;

            default:
                throw new NotImplementedException();
        }

        _ = SetProcessInformation(
            hProcess: GetCurrentProcess(),
            processInformationClass: PROCESS_INFORMATION_CLASS.ProcessPowerThrottling,
            processInformation: &powerThrottling,
            processInformationSize: (uint)sizeof(PROCESS_POWER_THROTTLING_STATE));
    }

    /// <summary>
    /// Based on <see href="https://docs.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-setpriorityclass"/>
    /// </summary>
    [SupportedOSPlatform("windows5.1.2600")]
    private static unsafe void SetProcessPriorityClass(ProcessPriorityClass priorityClass)
    {
        var flags = priorityClass switch
        {
            ProcessPriorityClass.Default => PROCESS_CREATION_FLAGS.NORMAL_PRIORITY_CLASS,
            ProcessPriorityClass.Idle => PROCESS_CREATION_FLAGS.IDLE_PRIORITY_CLASS,
            ProcessPriorityClass.BelowNormal => PROCESS_CREATION_FLAGS.BELOW_NORMAL_PRIORITY_CLASS,
            ProcessPriorityClass.Normal => PROCESS_CREATION_FLAGS.NORMAL_PRIORITY_CLASS,
            ProcessPriorityClass.AboveNormal => PROCESS_CREATION_FLAGS.ABOVE_NORMAL_PRIORITY_CLASS,
            ProcessPriorityClass.High => PROCESS_CREATION_FLAGS.HIGH_PRIORITY_CLASS,
            ProcessPriorityClass.Realtime => PROCESS_CREATION_FLAGS.REALTIME_PRIORITY_CLASS,
            _ => throw new NotImplementedException(),
        };

        _ = SetPriorityClass(
            hProcess: GetCurrentProcess(),
            dwPriorityClass: flags);
    }

    /// <summary>
    /// Enables/disables efficient mode for process <br/>
    /// Based on: <see href="https://devblogs.microsoft.com/performance-diagnostics/reduce-process-interference-with-task-manager-efficiency-mode/"/>
    /// </summary>
    /// <param name="value"></param>
    [SupportedOSPlatform("windows10.0.16299.0")]
    public static void SetEfficiencyMode(bool value)
    {
        var ecoLevel = Environment.OSVersion.Version >= new Version(11, 0)
#pragma warning disable CA1416 // Validate platform compatibility
            ? QualityOfServiceLevel.Eco
#pragma warning restore CA1416 // Validate platform compatibility
            : QualityOfServiceLevel.Low;

        SetProcessQualityOfServiceLevel(value
            ? ecoLevel
            : QualityOfServiceLevel.Default);
        SetProcessPriorityClass(value
            ? ProcessPriorityClass.Idle
            : ProcessPriorityClass.Default);
    }
}
