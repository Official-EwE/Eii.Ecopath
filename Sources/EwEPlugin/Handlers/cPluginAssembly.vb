'==============================================================================
'
' $Log: cPluginAssembly.vb,v $
' Revision 1.1  2008/09/26 07:31:03  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.4  2008/07/16 13:27:47  jeroens
' Added AlwaysEnabled flag
'
' Revision 1.3  2007/10/10 16:52:10  jeroens
' + Added link to AssemblyName
'
' Revision 1.2  2007/03/17 01:57:34  jeroens
' * Plugins() property has gained the option to filter by class type
'
' Revision 1.1  2006/08/31 15:20:33  jeroens
' * Moved
'
' Revision 1.2  2006/08/24 02:47:36  jeroens
' + Added comments
'
' Revision 1.1  2006/08/08 14:11:50  jeroens
' + Initial version
'
'==============================================================================

Option Strict On

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
    ''' <remarks>An exception will be thrown when adding a plugin
    ''' with a duplicate name.</remarks>
    ''' -----------------------------------------------------------------------
    Public Property Plugin(ByVal strName As String) As IPlugin
        Get
            Dim ip As IPlugin = Nothing

            strName = strName.ToLower()
            If Me.Enabled Then
                If Me.m_dictPlugins.ContainsKey(strName) Then
                    ip = Me.m_dictPlugins(strName)
                End If
            End If
            Return ip
        End Get
        Set(ByVal ip As IPlugin)
            strName = strName.ToLower()
            If Me.m_dictPlugins.ContainsKey(strName) Then
                Throw New cPluginException("Duplicate plugin found in assembly", Me, ip)
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
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Plugins(Optional ByVal t As Type = Nothing) As ICollection(Of IPlugin)
        Get
            If t Is Nothing Then
                Return Me.m_dictPlugins.Values
            Else
                Dim collPlugins As New List(Of IPlugin)
                For Each ip As IPlugin In Me.m_dictPlugins.Values
                    If t.IsInstanceOfType(ip) Then
                        collPlugins.Add(ip)
                    End If
                Next
                Return collPlugins
            End If
        End Get
    End Property

#End Region ' Plugin interfaces

#Region " Enabling/disabling "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Assembly enabled state changed delegate.
    ''' </summary>
    ''' <param name="pa">The Plugin assemble that changed enabled state.</param>
    ''' <param name="bEnabled">The new enabled state.</param>
    ''' -----------------------------------------------------------------------
    Public Delegate Sub AssemblyEnabledHandler(ByVal pa As cPluginAssembly, ByVal bEnabled As Boolean)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Assembly enabled state changed event.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Event AssemblyEnabled As AssemblyEnabledHandler

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/Set assembly changed state.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Enabled() As Boolean
        Get
            Return Me.m_bEnabled
        End Get
        Set(ByVal bEnabled As Boolean)
            ' Abort when enabled state will not change
            If (Me.m_bEnabled = bEnabled) Then Return
            ' Abort when trying to disable an AlwaysEnabled plugin
            If (Me.AlwaysEnabled() And bEnabled = False) Then Return

            ' Update enabled state
            Me.m_bEnabled = bEnabled
            ' Notify the world (at least, the probably minute portion of the world interested in hearing this, phah!)
            RaiseEvent AssemblyEnabled(Me, bEnabled)
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
            Return StringUtils.EndsWith(Me.Filename, "ewecore.dll", True)
        End Get
    End Property

#End Region ' Enabling/disabling

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
