' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Reflection
Imports System.Text
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities
Imports CoreResoures = EwECore.My.Resources.CoreDefaults

Namespace Controls

    Public Class cSysConfig

        Public Shared Function OSVersion() As String
            Dim strOS As String = String.Format(My.Resources.GENERIC_LABEL_DOUBLE, My.Computer.Info.OSFullName, My.Computer.Info.OSVersion)
            Dim strBit As String = If(cSystemUtils.Is64BitOS, CoreResoures.BITNESS_64, CoreResoures.BITNESS_32)
            Return String.Format(My.Resources.GENERIC_LABEL_DETAILED, strOS, strBit)
        End Function

        Public Shared Function NETVersion() As String
            Return String.Format(CoreResoures.NET_VERSION,
                                 System.Environment.Version.ToString(),
                                 If(cSystemUtils.Is64BitProcess, CoreResoures.BITNESS_64, CoreResoures.BITNESS_32))
        End Function

        Public Shared Function Modules(pm As cPluginManager) As String

            Dim aanLoaded As AssemblyName() = pm.PluginAssemblyNames()
            Dim aanPlugins As AssemblyName() = cAssemblyUtils.GetSummary()
            Dim an As AssemblyName = Nothing
            Dim sb As New StringBuilder()

            For Each an In cAssemblyUtils.GetSummary(cAssemblyUtils.eSummaryFlags.EwECore)
                sb.AppendLine(String.Format("* {0}={2},{1}",
                                                an.Name, cStringUtils.ToHexString(an.GetPublicKeyToken), an.Version))
            Next
            If (pm IsNot Nothing) Then
                For Each pa As cPluginAssembly In pm.PluginAssemblies
                    an = pa.AssemblyName
                    sb.AppendLine(String.Format("- {0}={2},{1}",
                                                    an.Name, cStringUtils.ToHexString(an.GetPublicKeyToken), an.Version))
                Next
            End If

            Return sb.ToString

        End Function

    End Class

End Namespace
