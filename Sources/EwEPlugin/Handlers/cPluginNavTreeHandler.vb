#Region " Imports "

Option Strict On
Imports System
Imports System.Windows.Forms
Imports Microsoft.VisualBasic
Imports EwEUtils.Commands

#End Region ' Imports

''' -----------------------------------------------------------------------
''' <summary>
''' GUI utility class, handles the placement of
''' <see cref="INavigationTreeItemPlugin">INavigationTreeItemPlugin</see>-
''' derived plugins in a <see cref="TreeView">TreeView</see>.
''' </summary>
''' -----------------------------------------------------------------------
Public Class cPluginNavTreeHandler
    Inherits cPluginGUIHandler

#Region " Private parts "

    ''' <summary>The tree view to modify.</summary>
    Private WithEvents m_tv As TreeView = Nothing

#End Region ' Private parts

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of a cPluginManuHandler.
    ''' </summary>
    ''' <param name="tv"><see cref="TreeView">TreeView</see> that contains the 
    ''' navigation structure that must be modified.</param>
    ''' <param name="pm"><see cref="cPluginManager">Plugin manager</see>
    ''' that holds the plugins to place in the control.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal tv As TreeView, _
                   ByVal pm As cPluginManager, _
                   ByVal cmdh As cCommandHandler)
        MyBase.new(pm, cmdh)
        ' Remember tree view
        Me.m_tv = tv
    End Sub

#End Region ' Construction 

#Region " Tree item handling "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Place or remove a plug-in tree item.
    ''' </summary>
    ''' <param name="ip">The <see cref="INavigationTreeItemPlugin">INavigationTreeItemPlugin</see> to place.</param>
    ''' <param name="bPlace">States whether the tree item should be placed (True)
    ''' or removed (False).</param>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub PlacePlugin(ByVal ip As IGUIPlugin, ByVal bPlace As Boolean)

        Dim tnc As TreeNodeCollection = Me.m_tv.Nodes
        Dim tn As TreeNode = Nothing
        Dim ipNavTree As INavigationTreeItemPlugin = Nothing
        Dim strLocation As String = Nothing
        Dim aLocations() As String = Nothing
        Dim iLocation As Integer = 0
        Dim iItem As Integer = 0
        Dim bError As Boolean = False
        Dim bFound As Boolean = False

        ' Sanity check
        If Not TypeOf ip Is INavigationTreeItemPlugin Then Return

        ' Get the real node
        ipNavTree = DirectCast(ip, INavigationTreeItemPlugin)
        ' Get node location
        strLocation = ipNavTree.NavigationTreeItemLocation
        ' Split locations by pipe char '|'
        aLocations = strLocation.Split("|"c)
        ' Already there?
        bFound = (String.IsNullOrEmpty(strLocation))

        ' Trace locations across existing node levels to find where to position this plug-in
        While iLocation < aLocations.Length And Not bError
            ' Reset level search
            iItem = 0
            bFound = False
            ' Find node that matches this locations' name
            While iItem < tnc.Count And Not bFound
                tn = DirectCast(tnc.Item(iItem), TreeNode)
                bFound = (String.Compare(Trim(tn.Name), Trim(aLocations(iLocation)), True) = 0)
                iItem += 1
            End While
            ' Found a node?
            If bFound Then
                ' #Yes: move to next level
                tnc = tn.Nodes
                iLocation += 1
            Else
                ' #No: error encountered
                bError = True
            End If
        End While

        Try
            ' Found place to add node item?
            If Not bError Then
                ' #Yes, handle the item.
                ' Adding or removing an item?
                If (bPlace) Then
                    ' #Adding: create new node
                    tn = New TreeNode(ipNavTree.ControlText)
                    ' Set name
                    tn.Name = ipNavTree.Name
                    ' Set tooltip text
                    tn.ToolTipText = ipNavTree.ControlTooltipText
                    ' Attach plugin info to node tag
                    tn.Tag = ipNavTree
                    ' Attach an image, if any
                    If ipNavTree.ControlImage IsNot Nothing Then
                        tn.ImageIndex = Me.m_tv.ImageList.Images.Count
                        tn.SelectedImageIndex = Me.m_tv.ImageList.Images.Count
                        Me.m_tv.ImageList.Images.Add(ipNavTree.ControlImage)
                    Else
                        tn.ImageIndex = Me.m_tv.ImageList.Images.Count
                        tn.SelectedImageIndex = Me.m_tv.ImageList.Images.Count
                        Me.m_tv.ImageList.Images.Add(My.Resources.pluginicon)
                    End If
                    ' Regular font
                    tn.NodeFont = New System.Drawing.Font(m_tv.Font, Drawing.FontStyle.Regular)

                    ' Add the node
                    tnc.Add(tn)
                Else
                    ' #Removing: try to remove the node
                    tn = tnc.Item(ipNavTree.Name)
                    If (tn IsNot Nothing) Then tnc.Remove(tn)
                End If
            End If
        Catch ex As Exception
            ' For now pretend all is well. Even if it is not ;)
        End Try

    End Sub

    Protected Overrides Sub EnablePlugin(ByVal ip As IGUIPlugin, ByVal bEnable As Boolean)
        ' Always enabled
    End Sub


#End Region ' Tree item handling

#Region " Tree node events "

    Private Sub tvNavigation_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) _
        Handles m_tv.AfterSelect

        ' Sanity checks
        If Not (TypeOf e.Node.Tag Is INavigationTreeItemPlugin) Then Return
        ' Fire plugin
        Me.RunPlugin(DirectCast(e.Node.Tag, INavigationTreeItemPlugin), sender, e)

    End Sub

#End Region ' Tree node events

End Class