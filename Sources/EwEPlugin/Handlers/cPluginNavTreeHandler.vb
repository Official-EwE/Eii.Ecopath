'==============================================================================
'
' $Log: cPluginNavTreeHandler.vb,v $
' Revision 1.4  2009/02/27 08:10:24  sherman
' Changed Plugin Icon
'
' Revision 1.3  2008/12/03 02:33:09  jeroens
' Added crash test
'
' Revision 1.2  2008/11/02 00:53:42  jeroens
' Fixed missing selected item index
'
' Revision 1.1  2008/09/26 07:31:04  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.8  2008/09/05 16:13:40  jeroens
' PluginManager set/get via Property
'
' Revision 1.7  2008/07/16 13:28:09  jeroens
' Fixed plugin removal bug
'
' Revision 1.6  2007/06/29 20:33:54  jeroens
' * Tree nodes use regular font
'
' Revision 1.5  2007/04/26 13:39:54  jeroens
' * Fixed plugin placement logic bugs
'
' Revision 1.4  2007/04/25 16:21:47  joeb
' Added Trim  to Treenode name and plugin location string matching
'
' Revision 1.3  2007/03/19 02:32:23  jeroens
' * Plugin invoked locally
'
' Revision 1.2  2007/03/14 00:53:37  jeroens
' * Uses plug-in tooltip text
'
' Revision 1.1  2006/09/06 16:55:30  jeroens
' + Initial version
'
'==============================================================================

Option Strict On

Imports System.Windows.Forms

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
    Public Sub New(ByVal tv As TreeView, ByRef pm As cPluginManager)
        MyBase.new()
        ' Remember tree view
        Me.m_tv = tv
        ' Hook up to plug-ins
        Me.PluginManager = pm
    End Sub

#End Region ' Construction 

#Region " Tree item handling "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Place or remove a plug-in tree item.
    ''' </summary>
    ''' <param name="p_ip">The <see cref="INavigationTreeItemPlugin">INavigationTreeItemPlugin</see> to place.</param>
    ''' <param name="bPlace">States whether the tree item should be placed (True)
    ''' or removed (False).</param>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub PlacePlugin(ByVal p_ip As IGUIPlugin, ByVal bPlace As Boolean)

        Dim tnc As TreeNodeCollection = Me.m_tv.Nodes
        Dim tn As TreeNode = Nothing
        Dim ip As INavigationTreeItemPlugin = Nothing
        Dim strLocation As String = Nothing
        Dim aLocations() As String = Nothing
        Dim iLocation As Integer = 0
        Dim iItem As Integer = 0
        Dim bError As Boolean = False
        Dim bFound As Boolean = False

        ' Sanity check
        If Not TypeOf p_ip Is INavigationTreeItemPlugin Then Return

        ' Get the real node
        ip = DirectCast(p_ip, INavigationTreeItemPlugin)
        ' Get node location
        strLocation = ip.NavigationTreeItemLocation
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
                    tn = New TreeNode(ip.ControlText)
                    ' Set name
                    tn.Name = ip.Name
                    ' Set tooltip text
                    tn.ToolTipText = ip.ControlTooltipText
                    ' Attach plugin info to node tag
                    tn.Tag = ip
                    ' Attach an image, if any
                    If ip.ControlImage IsNot Nothing Then
                        tn.ImageIndex = Me.m_tv.ImageList.Images.Count
                        tn.SelectedImageIndex = Me.m_tv.ImageList.Images.Count
                        Me.m_tv.ImageList.Images.Add(ip.ControlImage)
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
                    tn = tnc.Item(ip.Name)
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

    Private Sub tvNavigation_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles m_tv.AfterSelect

        ' Sanity checks
        If Not (TypeOf e.Node.Tag Is INavigationTreeItemPlugin) Then Return
        ' Fire plugin
        Me.RunPlugin(DirectCast(e.Node.Tag, INavigationTreeItemPlugin), sender, e)

    End Sub

End Class