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
    ''' User control; implements the Options > Plug-in options interface.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucOptionsPlugins
        Implements IUIElement
        Implements IOptionsPage

#Region " Helper classes "

        Const cIMAGE_CORE As Integer = 0
        Const cIMAGE_ENABLED As Integer = 1
        Const cIMAGE_DISABLED As Integer = 2
        Const cIMAGE_CONFLICT As Integer = 3

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

            Public ReadOnly Property AlwaysEnabled() As Boolean
                Get
                    Return Me.m_pa.AlwaysEnabled
                End Get
            End Property

            Public ReadOnly Property Compatible() As Boolean
                Get
                    Return Me.m_pa.IsCompatible
                End Get
            End Property

        End Class

#End Region ' Helper classes

#Region " Private variables "

        ''' <summary></summary>
        Private m_uic As cUIContext = Nothing
        ''' <summary></summary>
        Private m_pm As cPluginManager = Nothing
        ''' <summary></summary>
        Private m_dictPluginAssemblyInfo As New Dictionary(Of cPluginAssembly, cPluginAssemblyInfo)

#End Region ' Private variables

#Region " Constructor "

        Public Sub New(ByVal uic As cUIContext)
            Me.InitializeComponent()
            Me.m_pm = uic.Core.PluginManager
        End Sub

#End Region ' Constructor

#Region " Interface implementation "

        Public Property UIContext() As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Protected Set(ByVal uic As cUIContext)
                Me.m_uic = uic
            End Set
        End Property

#End Region ' Interface implementation

#Region " Public interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update which plug-ins to disable after a restart.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function Apply() As IOptionsPage.eApplyResultType _
            Implements IOptionsPage.Apply

            Dim alDisabledPlugins As New ArrayList()
            Dim bChanged As Boolean = False

            ' Build list of plugins to disable
            For Each info As cPluginAssemblyInfo In Me.m_dictPluginAssemblyInfo.Values
                If (info.Enabled = False) Then
                    alDisabledPlugins.Add(info.PluginAssembly.Filename)
                End If
            Next

            ' Detect changes that may require a restart
            If (My.Settings.DisabledPlugins IsNot Nothing) Then
                For Each strFN As String In alDisabledPlugins
                    bChanged = bChanged Or Not My.Settings.DisabledPlugins.Contains(strFN)
                Next
                For Each strFN As String In My.Settings.DisabledPlugins
                    bChanged = bChanged Or Not alDisabledPlugins.Contains(strFN)
                Next
            Else
                bChanged = (alDisabledPlugins.Count > 0)
            End If

            ' Update settings
            My.Settings.DisabledPlugins = alDisabledPlugins

            ' Convey result
            If bChanged Then Return IOptionsPage.eApplyResultType.Success_restart
            Return IOptionsPage.eApplyResultType.Success

        End Function

#End Region ' Public interfaces

#Region " Events "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            Dim collPA As ICollection(Of cPluginAssembly) = Nothing
            Dim info As cPluginAssemblyInfo = Nothing
            Dim pa As cPluginAssembly = Nothing
            Dim tnPA As TreeNode = Nothing
            Dim p As IPlugin = Nothing
            Dim tnP As TreeNode = Nothing

            If (Me.m_pm Is Nothing) Then Return

            collPA = Me.m_pm.PluginAssemblies
            For Each pa In collPA

                info = New cPluginAssemblyInfo(pa)

                tnPA = New TreeNode(Path.GetFileNameWithoutExtension(pa.Filename))
                tnPA.Tag = pa
                tnPA.ImageIndex = Me.GetImageIndex(info)
                tnPA.SelectedImageIndex = tnPA.ImageIndex
                Me.m_dictPluginAssemblyInfo(pa) = info

                For Each p In pa.Plugins(Nothing, True)

                    ' Name plug-ins by rich text if possible
                    If TypeOf p Is IGUIPlugin Then
                        tnP = New TreeNode(DirectCast(p, IGUIPlugin).ControlText)
                    Else
                        tnP = New TreeNode(p.Name)
                    End If
                    tnP.Tag = p

                    ' Determine (static) image
                    If (TypeOf p Is IGUIPlugin) Then
                        Dim pui As IGUIPlugin = DirectCast(p, IGUIPlugin)
                        If pui.ControlImage IsNot Nothing Then
                            tnP.ImageIndex = Me.m_ilPlugins.Images.Count
                            tnP.SelectedImageIndex = Me.m_ilPlugins.Images.Count
                            Me.m_ilPlugins.Images.Add(pui.ControlImage)
                        Else
                            tnP.ImageIndex = cIMAGE_ENABLED
                            tnP.SelectedImageIndex = cIMAGE_ENABLED
                        End If
                    End If

                    tnPA.Nodes.Add(tnP)
                Next
                Me.m_tvPlugins.Nodes.Add(tnPA)

            Next pa

            If pa IsNot Nothing Then
                Me.m_tvPlugins.SelectedNode = Me.m_tvPlugins.Nodes(0)
                Me.UpdateDetails()
            End If

            Me.UpdateControls()

        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing Then
                    For Each pa As cPluginAssembly In Me.m_dictPluginAssemblyInfo.Keys
                        '' Stop listening to plugin assembly
                        'RemoveHandler pa.AssemblyEnabled, AddressOf OnHandlePluginAssemblyEnabled
                        ' Restore enabled state
                        pa.Enabled = Me.m_dictPluginAssemblyInfo(pa).Enabled
                    Next

                    Me.m_dictPluginAssemblyInfo.Clear()

                    If components IsNot Nothing Then
                        components.Dispose()
                    End If
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        Private Sub OnAfterSelectNode(ByVal sender As System.Object, ByVal e As TreeViewEventArgs) _
            Handles m_tvPlugins.AfterSelect
            Me.UpdateDetails()
        End Sub

        Private Sub OnEnableCheckChanged(ByVal sender As Object, ByVal e As EventArgs) _
            Handles m_cbEnablePlugin.CheckedChanged

            Dim pa As cPluginAssembly = Me.SelectedPluginAssembly
            Dim info As cPluginAssemblyInfo = Me.m_dictPluginAssemblyInfo(pa)
            If (pa IsNot Nothing) Then
                info.Enabled = Me.m_cbEnablePlugin.Checked
                Me.UpdatePluginImage(info)
            End If
        End Sub

#End Region ' Events

#Region " Private implementations "

        Private Function FindPluginAssemblyNode(ByVal pa As cPluginAssembly) As TreeNode
            If pa Is Nothing Then Return Nothing
            For Each tn As TreeNode In Me.m_tvPlugins.Nodes
                If (TypeOf tn.Tag Is cPluginAssembly) Then
                    If Object.ReferenceEquals(DirectCast(tn.Tag, cPluginAssembly), pa) Then
                        Return tn
                    End If
                End If
            Next
            Return Nothing
        End Function

        Private ReadOnly Property SelectedPluginAssembly() As cPluginAssembly
            Get
                Dim tn As TreeNode = Me.m_tvPlugins.SelectedNode
                If (TypeOf tn.Tag Is cPluginAssembly) Then
                    Return DirectCast(tn.Tag, cPluginAssembly)
                ElseIf (TypeOf tn.Tag Is IPlugin) Then
                    Return DirectCast(tn.Parent.Tag, cPluginAssembly)
                End If
                Return Nothing
            End Get
        End Property

        Private Function GetImageIndex(ByVal info As cPluginAssemblyInfo) As Integer
            If (info.Enabled = False) Then Return cIMAGE_DISABLED
            If (info.Compatible = False) Then Return cIMAGE_CONFLICT
            If (info.AlwaysEnabled = True) Then Return cIMAGE_CORE
            Return cIMAGE_ENABLED
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdateDetails()

            Dim tn As TreeNode = Me.m_tvPlugins.SelectedNode
            Dim ctrl As UserControl = Nothing

            If (TypeOf tn.Tag Is cPluginAssembly) Then
                ctrl = New ucOptionsPluginAssemblyDetails(DirectCast(tn.Tag, cPluginAssembly))
            ElseIf (TypeOf tn.Tag Is IPlugin) Then
                ' Hackerdihack
                ctrl = New ucOptionsPluginDetails(Me.UIContext, _
                                              DirectCast(tn.Tag, IPlugin), _
                                              DirectCast(tn.Parent.Tag, cPluginAssembly))
            End If

            Me.m_split.SuspendLayout()

            Me.m_split.Panel2.Controls.Clear()
            If ctrl IsNot Nothing Then
                ctrl.Dock = DockStyle.Fill
                Me.m_split.Panel2.Controls.Add(ctrl)
            End If

            Me.m_split.ResumeLayout()
            Me.UpdateControls()

        End Sub

        Private Sub UpdatePluginImage(ByVal info As cPluginAssemblyInfo)
            Dim tn As TreeNode = Me.FindPluginAssemblyNode(info.PluginAssembly)
            Dim iIndex As Integer = Me.GetImageIndex(info)

            If tn IsNot Nothing Then
                tn.ImageIndex = iIndex
                tn.SelectedImageIndex = iIndex
            End If
        End Sub

        Private Sub UpdateControls()

            Dim pa As cPluginAssembly = Me.SelectedPluginAssembly

            Dim bEnabled As Boolean = False
            Dim bCanDisable As Boolean = False

            If (pa IsNot Nothing) Then
                bEnabled = pa.Enabled
                bCanDisable = (pa.AlwaysEnabled = False)
            End If

            Me.m_cbEnablePlugin.Enabled = bCanDisable
            Me.m_cbEnablePlugin.Checked = bEnabled

        End Sub

#End Region ' Private implementations

    End Class

End Namespace ' Other
