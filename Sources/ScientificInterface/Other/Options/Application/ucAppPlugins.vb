'==============================================================================
'
' $Log: ucAppPlugins.vb,v $
' Revision 1.6  2009/03/31 16:13:47  jeroens
' Conflicts now clearly shown
' Conflicting assemblies cannot be loaded anymore
'
' Revision 1.5  2008/12/15 15:56:03  jeroens
' no message
'
' Revision 1.4  2008/12/07 20:44:54  jeroens
' Plugin tree images put in correct order
'
' Revision 1.3  2008/12/03 02:40:54  jeroens
' Added levels of plugin compatibility
'
' Revision 1.2  2008/11/28 02:43:26  jeroens
' Added plugin compatibility checks to prevent the system from dying
'
' Revision 1.1  2008/09/26 07:32:10  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports System.IO
Imports System.Text
Imports System.Collections.Specialized
Imports EwECore
Imports EwEPlugin

#End Region

Namespace Other

    ''' -----------------------------------------------------------------------
    ''' <summary>
    '''
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucAppPlugins

        Const cIMAGE_CORE As Integer = 0
        Const cIMAGE_ENABLED As Integer = 1
        Const cIMAGE_DISABLED As Integer = 2
        Const cIMAGE_CONFLICT As Integer = 3

#Region " Helper classes "

        Private Class cPluginAssemblyInfo

            Private m_pa As cPluginAssembly = Nothing
            Private m_bEnabled As Boolean = True

            Public Sub New(ByVal pa As cPluginAssembly)
                Me.m_pa = pa
                Me.m_bEnabled = pa.Enabled
            End Sub

            Public ReadOnly Property PluginAssembly() As cPluginAssembly
                Get
                    Return Me.m_pa
                End Get
            End Property

            Public Property Enabled() As Boolean
                Get
                    Return Me.m_bEnabled
                End Get
                Set(ByVal bEnabled As Boolean)
                    Me.m_bEnabled = bEnabled
                End Set
            End Property

        End Class

        'Private Class cPluginInfo

        '    Private m_pi As IPlugin = Nothing
        '    Private m_iImageIndex As Integer = cIMAGE_ENABLED

        '    Public Sub New(ByVal pi As IPlugin)
        '        Me.m_pi = pi
        '    End Sub

        '    Public Property ImageIndex() As Integer
        '        Get
        '            Return Me.m_iImageIndex
        '        End Get
        '        Set(ByVal iIndex As Integer)
        '            Me.m_iImageIndex = iIndex
        '        End Set
        '    End Property
        'End Class

#End Region ' Helper classes

#Region " Private variables "

        ''' <summary></summary>
        Private m_pm As cPluginManager = Nothing
        ''' <summary></summary>
        Private m_dictPluginAssemblyInfo As New Dictionary(Of cPluginAssembly, cPluginAssemblyInfo)
        'Private m_dictPluginInfo As New Dictionary(Of IPlugin, cPluginInfo)

#End Region ' Private variables

#Region " Public interfaces "

        Public Sub Apply()

            Dim alDisabledPlugins As ArrayList = My.Settings.DisabledPlugins

            If alDisabledPlugins Is Nothing Then
                alDisabledPlugins = New ArrayList()
                My.Settings.DisabledPlugins = alDisabledPlugins
            Else
                alDisabledPlugins.Clear()
            End If

            For Each info As cPluginAssemblyInfo In Me.m_dictPluginAssemblyInfo.Values
                info.Enabled = info.PluginAssembly.Enabled

                If (info.Enabled = False) Then
                    alDisabledPlugins.Add(info.PluginAssembly.Filename)
                End If
            Next
        End Sub

#End Region ' Public interfaces

#Region " Events "

        Private Sub ucAppPlugins_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim collPA As ICollection(Of cPluginAssembly) = Nothing
            Dim pa As cPluginAssembly = Nothing
            Dim tnPA As TreeNode = Nothing
            Dim p As IPlugin = Nothing
            Dim tnP As TreeNode = Nothing
            'Dim infoPI As cPluginInfo = Nothing

            Me.m_pm = cCore.GetInstance().PluginManager

            If (Me.m_pm Is Nothing) Then Return

            collPA = Me.m_pm.PluginAssemblies
            For Each pa In collPA
                tnPA = New TreeNode(Path.GetFileNameWithoutExtension(pa.Filename))
                tnPA.Tag = pa
                tnPA.ImageIndex = Me.GetPluginAssemblyImageIndex(pa)
                tnPA.SelectedImageIndex = Me.GetPluginAssemblyImageIndex(pa)

                AddHandler pa.AssemblyEnabled, AddressOf OnHandlePluginAssemblyEnabled
                Me.m_dictPluginAssemblyInfo(pa) = New cPluginAssemblyInfo(pa)

                For Each p In pa.Plugins
                    tnP = New TreeNode(p.Name)
                    tnP.Tag = p

                    'infoPI = New cPluginInfo(p)

                    '' Determine image
                    'If (TypeOf p Is IGUIPlugin) Then
                    '    Dim pui As IGUIPlugin = DirectCast(p, IGUIPlugin)
                    '    If pui.ControlImage IsNot Nothing Then
                    '        infoPI.ImageIndex = Me.m_ilPlugins.Images.Count
                    '        Me.m_ilPlugins.Images.Add(pui.ControlImage)
                    '    Else
                    '        infoPI.ImageIndex = cIMAGE_ENABLED
                    '    End If
                    'End If

                    tnP.ImageIndex = Me.GetPluginAssemblyImageIndex(pa)
                    tnP.SelectedImageIndex = Me.GetPluginAssemblyImageIndex(pa)
                    'Me.m_dictPluginInfo(p) = p

                    tnPA.Nodes.Add(tnP)
                Next
                Me.m_tvPlugins.Nodes.Add(tnPA)

            Next pa

            If pa IsNot Nothing Then
                Me.m_tvPlugins.SelectedNode = Me.m_tvPlugins.Nodes(0)
                Me.UpdateDetails()
            End If

        End Sub

        Private Sub ucAppPlugins_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles Me.Disposed

            For Each pa As cPluginAssembly In Me.m_dictPluginAssemblyInfo.Keys
                ' Stop listening to plugin assembly
                RemoveHandler pa.AssemblyEnabled, AddressOf OnHandlePluginAssemblyEnabled
                ' Restore enabled state
                pa.Enabled = Me.m_dictPluginAssemblyInfo(pa).Enabled
            Next

            Me.m_dictPluginAssemblyInfo.Clear()
            'Me.m_dictPluginInfo.Clear()

        End Sub

        Private Sub m_tvPlugins_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles m_tvPlugins.AfterSelect
            Me.UpdateDetails()
        End Sub

        Private Sub OnHandlePluginAssemblyEnabled(ByVal pa As cPluginAssembly, ByVal bEnabled As Boolean)
            Dim tn As TreeNode = Me.FindPluginAssemblyNode(pa)
            Dim iIndex As Integer = Me.GetPluginAssemblyImageIndex(pa)

            If tn IsNot Nothing Then
                tn.ImageIndex = iIndex
                tn.SelectedImageIndex = iIndex
            End If
        End Sub

        Private Function GetPluginAssemblyImageIndex(ByVal pa As cPluginAssembly) As Integer
            ' Order of state images:
            '    1. Disabled state
            '    2. Conflict state
            '    3. Always enabled state
            '    4. Normal enabled state
            If (pa.IsCompatible = False) Then Return cIMAGE_CONFLICT
            If (pa.Enabled = False) Then Return cIMAGE_DISABLED
            If (pa.AlwaysEnabled = True) Then Return cIMAGE_CORE
            Return cIMAGE_ENABLED
        End Function

#End Region ' Events

#Region " Private implementations "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdateDetails()

            Dim tn As TreeNode = Me.m_tvPlugins.SelectedNode
            Dim ctrl As UserControl = Nothing

            If (TypeOf tn.Tag Is cPluginAssembly) Then
                ctrl = New ucAppPluginAssemblyDetails(DirectCast(tn.Tag, cPluginAssembly))
            ElseIf (TypeOf tn.Tag Is IPlugin) Then
                ' Hackerdihack
                ctrl = New ucAppPluginDetails(DirectCast(tn.Tag, IPlugin), DirectCast(tn.Parent.Tag, cPluginAssembly))
            End If

            Me.m_split.SuspendLayout()

            Me.m_split.Panel2.Controls.Clear()
            If ctrl IsNot Nothing Then
                ctrl.Dock = DockStyle.Fill
                Me.m_split.Panel2.Controls.Add(ctrl)
            End If

            Me.m_split.ResumeLayout()

        End Sub

        Private Function FindPluginAssemblyNode(ByVal pa As cPluginAssembly) As TreeNode
            For Each tn As TreeNode In Me.m_tvPlugins.Nodes
                If (TypeOf tn.Tag Is cPluginAssembly) Then
                    If Object.ReferenceEquals(DirectCast(tn.Tag, cPluginAssembly), pa) Then
                        Return tn
                    End If
                End If
            Next
            Return Nothing
        End Function

#End Region ' Private implementations

    End Class

End Namespace ' Other
