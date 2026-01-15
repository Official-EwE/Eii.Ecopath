' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'


Imports System.Reflection
Imports EwECore.Plugins
Imports EwECore.Plugins.Core
Imports EwEUtils.Utilities

''' ---------------------------------------------------------------------------
''' <summary>
''' Holds information on a particular plugin assembly (author, version, copyright, etc)
''' as well as a list of <see cref="IPlugin">plug-ins</see> found in the assembly.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cPluginAssembly

#Region " Private helper classes "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' IComparer that sorts plug-ins by name, ascending.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Class cPluginComparer
        Implements IComparer(Of IPlugin)

        Public Function Compare(x As IPlugin, y As IPlugin) As Integer _
            Implements IComparer(Of IPlugin).Compare
            Return String.Compare(x.DisplayName, y.DisplayName)
        End Function

    End Class

#End Region ' Private helper classes

#Region " Private parts "

    Private m_ass As Assembly = Nothing
    ''' <summary>All available plugins in this assembly.</summary>
    Private m_dictPlugins As New Dictionary(Of String, IPlugin)
    ''' <summary>Assembly enable state.</summary>
    Private m_bEnabled As Boolean = True
    ''' <summary>Assembly compatibility state.</summary>
    Private m_compatibility As ePluginCompatibilityTypes = ePluginCompatibilityTypes.VersionCompatible

    ''' <summary>License checked flag. For UI display purposes only.</summary>
    Private m_bLicenseChecked As Boolean = False
    ''' <summary>Assembly licensed state. For UI display purposes only</summary>
    Private m_bLicensed As Boolean = False
    ''' <summary>Assembly license expiry date. For UI display purposes only</summary>
    Private m_dtExpiry As DateTime = DateTime.MinValue


#End Region ' Private parts

#Region " Constructor "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a new plugin assembly wrapper.
    ''' </summary>
    ''' <param name="ass">The wrapped <see cref="Assembly"/>.</param>
    ''' <param name="bEnabled">Flag stating that the plug-in assembly is allowed to load.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ass As Assembly, bEnabled As Boolean)
        Me.m_ass = ass
        Me.SessionEnabled = bEnabled
        Me.m_bEnabled = bEnabled
    End Sub

#End Region ' Constructor

#Region " Plugin interfaces "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set a named <see cref="IPlugin">plugin</see>.
    ''' </summary>
    ''' <param name="strName">The <see cref="IPlugin.DisplayName">name</see>
    ''' of the plugin.</param>
    ''' <param name="bAllowDisabled">Flag stating if plug-ins from disabled 
    ''' assemblies can be aquired as well.</param>
    ''' <remarks>An exception will be thrown when adding a plugin
    ''' with a duplicate name.</remarks>
    ''' -----------------------------------------------------------------------
    Public Property Plugin(strName As String, Optional bAllowDisabled As Boolean = False) As IPlugin
        Get
            Dim ip As IPlugin = Nothing

            strName = strName.ToLower()
            If (Me.CanRun Or bAllowDisabled) Then
                If Me.m_dictPlugins.ContainsKey(strName) Then
                    ip = Me.m_dictPlugins(strName)
                End If
            End If
            Return ip
        End Get
        Set(ip As IPlugin)
            strName = strName.ToLower()
            If Me.m_dictPlugins.ContainsKey(strName) Then
                Throw New cPluginException(Me, String.Format(My.Resources.CoreDefaults.PLUGIN_EXCEPTION_DUPLICATE, Me.Filename, strName), Nothing)
            Else
                Me.m_dictPlugins.Add(strName, ip)
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Gets a collection of <see cref="IPlugin">plugins</see> in this assembly.
    ''' </summary>
    ''' <param name="t">The <see cref="Type">Type</see> of the plugins to retrieve,
    ''' or Nothing to return all plugins in this Assembly.</param>
    ''' <param name="bAllowDisabled">Flag stating if plug-ins from disabled 
    ''' assemblies can be aquired as well.</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Plugins(Optional t As Type = Nothing,
                                     Optional bAllowDisabled As Boolean = False) As ICollection(Of IPlugin)
        Get
            Dim collPlugins As New List(Of IPlugin)

            If (Me.CanRun Or bAllowDisabled) Then
                If t Is Nothing Then
                    collPlugins.AddRange(Me.m_dictPlugins.Values)
                Else
                    For Each ip As IPlugin In Me.m_dictPlugins.Values
                        If t.IsInstanceOfType(ip) Then
                            collPlugins.Add(ip)
                        End If
                    Next
                End If
            End If

            ' Sort plug-ins
            collPlugins.Sort(New cPluginComparer())
            ' Done
            Return collPlugins

        End Get
    End Property

#End Region ' Plugin interfaces

#Region " Enabling/disabling "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' States whether this assembly is allowed to be accessed for invoking 
    ''' plug-ins.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    ReadOnly Property CanRun() As Boolean
        Get
            Return (Me.Enabled And Me.SessionEnabled) Or Me.AlwaysEnabled
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/Set assembly enabled state.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Enabled() As Boolean
        Get
            Return Me.m_bEnabled Or Me.AlwaysEnabled()
        End Get
        Set(bEnabled As Boolean)
            ' Abort when enabled state will not change
            If (Me.m_bEnabled = bEnabled) Then Return
            ' Abort when trying to disable an AlwaysEnabled plugin
            If (Me.AlwaysEnabled() And bEnabled = False) Then Return
            ' Update enabled state
            Me.m_bEnabled = bEnabled
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get whether this assembly is enable for a session. This flag can only
    ''' be set at plugin assembly load time to ensure that a plug-in assembly
    ''' enabled state does not change thoughtout a session.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property SessionEnabled() As Boolean = True

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get whether this assembly cannot be disabled.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property AlwaysEnabled() As Boolean
        Get
            ' Core plugins are always enabled
            Return cStringUtils.EndsWith(Me.Filename, "ewecore.dll", True)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get whether this assembly is licensed.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property IsLicensed As Boolean
        Get
            Me.CheckLicense()
            Return Me.m_bLicensed
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the license date for this assembly, if any.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Expiry As DateTime
        Get
            Me.CheckLicense()
            Return Me.m_dtExpiry
        End Get
    End Property

#End Region ' Enabling/disabling

#Region " Compatibility "

    Public Enum ePluginCompatibilityTypes As Integer
        ''' <summary>Versions are fully compatible.</summary>
        VersionCompatible = 0
        ''' <summary>Versions may be compatible.</summary>
        VersionCompatibleCaution
        ''' <summary>Major revision version incompatibility detected.</summary>
        VersionIncompatible
        ''' <summary>Unable to determine level of incompatibility.</summary>
        IncompatibleUndetermined
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set plugin compatibility state.
    ''' </summary>
    ''' <remarks>
    ''' States whether a plug-in is compatible with the set of assemblies that
    ''' the main application relies on.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Property Compatibility() As ePluginCompatibilityTypes
        Get
            Return Me.m_compatibility
        End Get
        Friend Set(value As ePluginCompatibilityTypes)
            Me.m_compatibility = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' States whether a plugin assembly is compatible enough to run with EwE.
    ''' </summary>
    ''' <returns>True if compatible to run, false otherwise.</returns>
    ''' -----------------------------------------------------------------------
    Public Function IsCompatibleToRun() As Boolean
        ' Minor version revisions should not matter
        Return (Me.Compatibility = ePluginCompatibilityTypes.VersionCompatible) Or
               (Me.Compatibility = ePluginCompatibilityTypes.VersionCompatibleCaution)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' States whether a plugin assembly is compatible with all EwE assemblies.
    ''' </summary>
    ''' <returns>True if compatible to run, false otherwise.</returns>
    ''' -----------------------------------------------------------------------
    Public Function IsCompatible() As Boolean
        Return (Me.Compatibility = ePluginCompatibilityTypes.VersionCompatible)
    End Function

#End Region ' Compatibility

#Region " Assembly metadata "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set assembly company name.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Company() As String

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set assembly version.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Version() As String

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set assembly description.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Description() As String

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set assembly copyright.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Copyright() As String

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set assembly file name.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Filename() As String

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the <see cref="AssemblyName">AssemblyName</see> associated with this
    ''' plug-in assembly.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property AssemblyName() As AssemblyName
        Get
            Return Me.m_ass.GetName()
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the actual <see cref="Assembly">Assembly</see> of the plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Assembly() As Assembly
        Get
            Return Me.m_ass
        End Get
    End Property

#End Region ' Assembly metadata

#Region " Overrides "

    Public Overrides Function ToString() As String
        Return System.IO.Path.GetFileNameWithoutExtension(Me.Filename) & " " & If(Me.Enabled, "", "(disabled)")
    End Function

#End Region ' Overrides

#Region " Internals "

    Private Sub CheckLicense()

        If (Me.m_bLicenseChecked) Then Return

        For Each ip As IPlugin In Me.m_dictPlugins.Values
            If (TypeOf ip Is ILicensePlugin) Then
                Dim lp As ILicensePlugin = DirectCast(ip, ILicensePlugin)
                Me.m_bLicensed = True
                Try
                    lp.Expiry(Me.m_dtExpiry)
                Catch ex As Exception
                    ' NOP
                End Try
            End If
        Next

        Me.m_bLicenseChecked = True

    End Sub

#End Region ' Internals

End Class
