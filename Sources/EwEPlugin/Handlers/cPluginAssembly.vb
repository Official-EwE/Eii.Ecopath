Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Reflection
Imports EwEUtils.Utilities

''' ---------------------------------------------------------------------------
''' <summary>
''' Holds information on a particular plugin assembly (author, version, copyright, etc)
''' as well as a list of <see cref="IPlugin">plug-ins</see> found in the assembly.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cPluginAssembly

#Region " Private parts "

    Private m_an As AssemblyName = Nothing
    ''' <summary>All available plugins in this assembly.</summary>
    Private m_dictPlugins As New Dictionary(Of String, IPlugin)
    ''' <summary>Assembly company name.</summary>
    Private m_strCompany As String = ""
    ''' <summary>Assembly version number.</summary>
    Private m_strVersion As String = ""
    ''' <summary>Assembly description.</summary>
    Private m_strDescription As String = ""
    ''' <summary>Assembly copyright notice.</summary>
    Private m_strCopyright As String = ""
    ''' <summary>Assembly file name.</summary>
    Private m_strFileName As String = ""
    ''' <summary>Assembly enable state.</summary>
    Private m_bEnabled As Boolean = True
    ''' <summary>Assembly compatibility state.</summary>
    Private m_compatibility As ePluginCompatibilityTypes = ePluginCompatibilityTypes.VersionCompatible

#End Region ' Private parts

#Region " Constructor "

    Public Sub New(ByVal an As AssemblyName)
        Me.m_an = an
    End Sub

#End Region ' Constructor

#Region " Plugin interfaces "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set a named <see cref="IPlugin">plugin</see>.
    ''' </summary>
    ''' <param name="strName">The <see cref="IPlugin.Name">name</see>
    ''' of the plugin.</param>
    ''' <param name="bAllowDisabled">Flag stating if plug-ins from disabled 
    ''' assemblies can be aquired as well.</param>
    ''' <remarks>An exception will be thrown when adding a plugin
    ''' with a duplicate name.</remarks>
    ''' -----------------------------------------------------------------------
    Public Property Plugin(ByVal strName As String, Optional ByVal bAllowDisabled As Boolean = False) As IPlugin
        Get
            Dim ip As IPlugin = Nothing

            strName = strName.ToLower()
            If (Me.Enabled Or bAllowDisabled) Then
                If Me.m_dictPlugins.ContainsKey(strName) Then
                    ip = Me.m_dictPlugins(strName)
                End If
            End If
            Return ip
        End Get
        Set(ByVal ip As IPlugin)
            strName = strName.ToLower()
            If Me.m_dictPlugins.ContainsKey(strName) Then
                Throw New cPluginException(Me, String.Format(My.Resources.PLUGIN_EXCEPTION_DUPLICATE, Me.Filename, strName), Nothing)
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
    Public ReadOnly Property Plugins(Optional ByVal t As Type = Nothing, _
                                     Optional ByVal bAllowDisabled As Boolean = False) As ICollection(Of IPlugin)
        Get
            Dim collPlugins As New List(Of IPlugin)

            If (Me.Enabled Or bAllowDisabled) Then
                If t Is Nothing Then
                    Return Me.m_dictPlugins.Values
                Else
                    For Each ip As IPlugin In Me.m_dictPlugins.Values
                        If t.IsInstanceOfType(ip) Then
                            collPlugins.Add(ip)
                        End If
                    Next
                End If
            End If
            Return collPlugins

        End Get
    End Property

#End Region ' Plugin interfaces

#Region " Enabling/disabling "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/Set assembly enabled state.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Enabled() As Boolean
        Get
            Return Me.m_bEnabled Or Me.AlwaysEnabled()
        End Get
        Set(ByVal bEnabled As Boolean)
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
    ''' Get whether this assembly should always be enabled (for core plug-ins)
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property AlwaysEnabled() As Boolean
        Get
            ' Core plugins always enabled
            Return cStringUtils.EndsWith(Me.Filename, "ewecore.dll", True)
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
        Friend Set(ByVal value As ePluginCompatibilityTypes)
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
        Return (Me.Compatibility = ePluginCompatibilityTypes.VersionCompatible) Or _
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
        Get
            Return Me.m_strCompany
        End Get
        Friend Set(ByVal strValue As String)
            Me.m_strCompany = strValue
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set assembly version.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Version() As String
        Get
            Return Me.m_strVersion
        End Get
        Friend Set(ByVal strValue As String)
            Me.m_strVersion = strValue
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set assembly description.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Description() As String
        Get
            Return Me.m_strDescription
        End Get
        Friend Set(ByVal strValue As String)
            Me.m_strDescription = strValue
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set assembly copyright.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Copyright() As String
        Get
            Return Me.m_strCopyright
        End Get
        Friend Set(ByVal strValue As String)
            Me.m_strCopyright = strValue
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set assembly file name.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Filename() As String
        Get
            Return Me.m_strFileName
        End Get
        Friend Set(ByVal strValue As String)
            Me.m_strFileName = strValue
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set <see cref="AssemblyName">AssemblyName</see> associated with this
    ''' plug-in assembly.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property AssemblyName() As AssemblyName
        Get
            Return Me.m_an
        End Get
    End Property

#End Region ' Assembly metadata

End Class
