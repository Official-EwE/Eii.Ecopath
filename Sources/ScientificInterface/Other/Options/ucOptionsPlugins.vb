' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Option Explicit On

Imports System.IO
Imports System.Text
Imports System.Collections.Specialized
Imports EwECore
Imports EwEPlugin
Imports SharedResources = ScientificInterfaceShared.My.Resources

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
        Const cIMAGE_ENABLEDPLUGIN As Integer = 1
        Const cIMAGE_ANYPLUGINPOINT As Integer = 2
        Const cIMAGE_DISABLED As Integer = 3
        Const cIMAGE_CONFLICT As Integer = 4

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
        Private m_pm As cPluginManager = Nothing
        ''' <summary></summary>
        Private m_dictPluginAssemblyInfo As New Dictionary(Of cPluginAssembly, cPluginAssemblyInfo)

#End Region ' Private variables

#Region " Constructor "

        Public Sub New(ByVal uic As cUIContext)
            Me.UIContext = uic
            Me.m_pm = Me.UIContext.Core.PluginManager
            Me.InitializeComponent()
        End Sub

#End Region ' Constructor

#Region " Interface implementation "

 
#End Region ' Interface implementation

#Region " Public interfaces "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IUIElement.UIContext"/>
        ''' -------------------------------------------------------------------
        Public Property UIContext() As cUIContext _
                  Implements IUIElement.UIContext

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.CanApply"/>
        ''' -------------------------------------------------------------------
        Public Function CanApply() As Boolean _
            Implements IOptionsPage.CanApply
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.OnChanged"/>
        ''' -------------------------------------------------------------------
        Public Event OnOptionsPluginsChanged(sender As IOptionsPage, args As System.EventArgs) _
            Implements IOptionsPage.OnChanged

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.Apply"/>
        ''' -------------------------------------------------------------------
        Public Function Apply() As IOptionsPage.eApplyResultType _
            Implements IOptionsPage.Apply

            Dim alDisabledPlugins As New ArrayList()
            Dim bChanged As Boolean = False
            Dim result As IOptionsPage.eApplyResultType = IOptionsPage.eApplyResultType.Success

            ' Only when toggling this option on
            If (Me.m_cbDownloadUpdates.Checked And My.Settings.AutoUpdatePlugins = False) Then
                If (Not EwEUtils.SystemUtilities.cSystemUtils.IsAdministrator()) Then
                    result = IOptionsPage.eApplyResultType.Success_administrator
                Else
                    result = IOptionsPage.eApplyResultType.Success_restart
                End If
            End If

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
            My.Settings.AutoUpdatePlugins = Me.m_cbDownloadUpdates.Checked
            My.Settings.UpdatePluginsTimeout = CInt(Me.m_nudTimeOut.Value * 1000)

            ' Convey result
            If bChanged Then result = DirectCast(Math.Max(result, IOptionsPage.eApplyResultType.Success_restart), IOptionsPage.eApplyResultType)

            Return result

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.SetDefaults"/>
        ''' -------------------------------------------------------------------
        Public Sub SetDefaults() _
               Implements IOptionsPage.SetDefaults
            Me.m_cbDownloadUpdates.Checked = CBool(My.Settings.GetDefaultValue("AutoUpdatePlugins"))
            Me.m_nudTimeOut.Value = CDec(Math.Max(1, Math.Round(CDec(My.Settings.GetDefaultValue("UpdatePluginsTimeout")) / 1000)))
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.CanSetDefaults"/>
        ''' -------------------------------------------------------------------
        Public Function CanSetDefaults() As Boolean _
            Implements IOptionsPage.CanSetDefaults
            Return True
        End Function

#End Region ' Public interfaces

#Region " Events "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            Dim collPA As ICollection(Of cPluginAssembly) = Nothing
            Dim info As cPluginAssemblyInfo = Nothing
            Dim pa As cPluginAssembly = Nothing
            Dim tnPA As TreeNode = Nothing
            Dim p As IPlugin = Nothing
            Dim tnP As TreeNode = Nothing

            If (Me.m_pm Is Nothing) Then Return

            Me.m_cbDownloadUpdates.Checked = My.Settings.AutoUpdatePlugins
            Me.m_nudTimeOut.Value = CDec(Math.Max(1, Math.Round(My.Settings.UpdatePluginsTimeout / 1000)))

            ' Prepare image list
            Me.m_ilPlugins.Images.Add(SharedResources.nav8_ecopath)
            Me.m_ilPlugins.Images.Add(SharedResources.plugin)
            Me.m_ilPlugins.Images.Add(SharedResources.pluginpoint)
            Me.m_ilPlugins.Images.Add(SharedResources.Cancel)
            Me.m_ilPlugins.Images.Add(SharedResources.Warning)

            collPA = Me.m_pm.PluginAssemblies
            For Each pa In collPA

                info = New cPluginAssemblyInfo(pa)

                tnPA = New TreeNode(Path.GetFileNameWithoutExtension(pa.Filename))
                tnPA.Tag = pa
                tnPA.ImageIndex = Me.GetPluginAssemblyImageIndex(info)
                tnPA.SelectedImageIndex = tnPA.ImageIndex
                Me.m_dictPluginAssemblyInfo(pa) = info

                For Each p In pa.Plugins(Nothing, True)

                    ' Name plug-ins by rich text if possible
                    If (TypeOf p Is IGUIPlugin) Then
                        tnP = New TreeNode(DirectCast(p, IGUIPlugin).ControlText)
                    Else
                        tnP = New TreeNode(p.Name)
                    End If
                    tnP.Tag = p
                    tnP.ImageIndex = cIMAGE_ANYPLUGINPOINT
                    tnP.SelectedImageIndex = cIMAGE_ANYPLUGINPOINT

                    tnPA.Nodes.Add(tnP)
                Next
                Me.m_tvPlugins.Nodes.Add(tnPA)

            Next pa

            If pa IsNot Nothing Then
                Me.m_tvPlugins.SelectedNode = Me.m_tvPlugins.Nodes(0)
                Me.UpdateDetails()
            End If
            Me.UpdateControls()

            MyBase.OnLoad(e)

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

        Private Sub m_btnClear_Click(sender As System.Object, e As System.EventArgs) _
            Handles m_btnClear.Click
            My.Settings.SuppressedOverwritePrompts = ""
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
                If (tn Is Nothing) Then Return Nothing ' May have none
                If (TypeOf tn.Tag Is cPluginAssembly) Then
                    Return DirectCast(tn.Tag, cPluginAssembly)
                ElseIf (TypeOf tn.Tag Is IPlugin) Then
                    Return DirectCast(tn.Parent.Tag, cPluginAssembly)
                End If
                Return Nothing
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the image index to reflect a plug-in assembly.
        ''' </summary>
        ''' <param name="info">The plug-in assembly info to return the image for.</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function GetPluginAssemblyImageIndex(ByVal info As cPluginAssemblyInfo) As Integer
            If (info.Enabled = False) Then Return cIMAGE_DISABLED
            If (info.Compatible = False) Then Return cIMAGE_CONFLICT
            If (info.AlwaysEnabled = True) Then Return cIMAGE_CORE
            Return cIMAGE_ENABLEDPLUGIN
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
            Dim iIndex As Integer = Me.GetPluginAssemblyImageIndex(info)

            If tn IsNot Nothing Then
                tn.ImageIndex = iIndex
                tn.SelectedImageIndex = iIndex
            End If
        End Sub

        Private Sub UpdateControls()

            Dim pa As cPluginAssembly = Me.SelectedPluginAssembly
            Dim bHasSuppressedPrompts As Boolean = (Not String.IsNullOrEmpty(My.Settings.SuppressedOverwritePrompts))

            Dim bEnabled As Boolean = False
            Dim bCanDisable As Boolean = False

            If (pa IsNot Nothing) Then
                bEnabled = pa.Enabled
                bCanDisable = (pa.AlwaysEnabled = False)
            End If

            Me.m_cbEnablePlugin.Enabled = bCanDisable
            Me.m_cbEnablePlugin.Checked = bEnabled
            Me.m_btnClear.Enabled = bHasSuppressedPrompts

        End Sub

#End Region ' Private implementations

    End Class

End Namespace ' Other
