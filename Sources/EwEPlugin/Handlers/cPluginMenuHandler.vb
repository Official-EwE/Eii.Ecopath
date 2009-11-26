Option Strict On
Imports System
Imports System.Diagnostics
Imports System.Windows.Forms
Imports Microsoft.VisualBasic
Imports EwEUtils.Commands

''' -----------------------------------------------------------------------
''' <summary>
''' GUI utility class, handles the placement of <see cref="IGUIPlugin">IGUIPlugin</see>-
''' derived plugins in the menu structure of a <see cref="Form">Form</see>.
''' </summary>
''' -----------------------------------------------------------------------
Public Class cPluginMenuHandler
    Inherits cPluginGUIHandler

#Region " Private parts "

    ''' <summary>The form holding the menu to modify.</summary>
    Private m_menu As MenuStrip = Nothing

#End Region ' Private parts

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of a cPluginManuHandler.
    ''' </summary>
    ''' <param name="menu"><see cref="MenuStrip">Menu strip</see> that contains the menu
    ''' that must be modified.</param>
    ''' <param name="pm"><see cref="cPluginManager">Plugin manager</see>
    ''' that holds the plugins to place in the main menu of <paramref name="f">Form frm</paramref></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByRef menu As MenuStrip, ByRef pm As cPluginManager)
        MyBase.new()
        ' Set form
        Me.m_menu = menu
        Me.PluginManager = pm
    End Sub

#End Region ' Construction 

#Region " Menu item handling "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Place or remove a GUI plugin menu item.
    ''' </summary>
    ''' <param name="p_ip">The <see cref="IGUIPlugin">IGUIPlugin</see> to place.</param>
    ''' <param name="bPlace">States whether the menu item should be placed (True)
    ''' or removed (False).</param>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub PlacePlugin(ByVal p_ip As IGUIPlugin, ByVal bPlace As Boolean)

        Dim tsic As ToolStripItemCollection = Me.m_menu.Items
        Dim tsi As ToolStripMenuItem = Nothing
        Dim ip As IMenuItemPlugin = Nothing
        Dim strLocation As String = Nothing
        Dim aLocations() As String = Nothing
        Dim iLocation As Integer = 0
        Dim iItem As Integer = 0
        Dim bError As Boolean = False
        Dim bFound As Boolean = False

        If Not TypeOf p_ip Is IMenuItemPlugin Then
            Return
        End If

        ip = DirectCast(p_ip, IMenuItemPlugin)
        strLocation = ip.MenuItemLocation
        aLocations = strLocation.Split("|"c)
        bFound = String.IsNullOrEmpty(strLocation)

        ' Find named menu item for every level
        While iLocation < aLocations.Length And Not bError
            iItem = 0
            bFound = False
            While iItem < tsic.Count And Not bFound
                tsi = CType(tsic.Item(iItem), ToolStripMenuItem)
                bFound = (String.Compare(Trim(tsi.Name), Trim(aLocations(iLocation)), False) = 0)
                iItem += 1
            End While
            If bFound Then tsic = tsi.DropDownItems
            bError = Not bFound
            iLocation += 1
        End While

        Try

            ' Found item position?
            If Not bError Then
                If (bPlace) Then
                    ' Create menu item and add it
                    tsi = New ToolStripMenuItem(ip.ControlText, ip.ControlImage, AddressOf OnPluginMenuItemClick)
                    ' Set name
                    tsi.Name = ip.Name
                    ' Set tooltip text
                    tsi.ToolTipText = ip.ControlTooltipText
                    ' Add tag
                    tsi.Tag = ip
                    ' Add new item to menu item strip
                    tsic.Add(tsi)
                Else
                    ' Remove menu item
                    tsic.RemoveByKey(ip.Name)
                End If
            End If

        Catch ex As Exception
            ' For now pretend all is well. Even if it is not ;)
        End Try

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' One of 'our' menu items has been clicked.
    ''' </summary>
    ''' <param name="sender">The sender of the event, which in this case must be
    ''' a <see cref="ToolStripMenuItem">ToolStripMenuItem</see>.</param>
    ''' <param name="e">Additional <see cref="EventArgs">event arguments</see>.</param>
    ''' -----------------------------------------------------------------------
    Private Sub OnPluginMenuItemClick(ByVal sender As Object, ByVal e As EventArgs)

        Debug.Assert(TypeOf sender Is ToolStripMenuItem)

        Dim tsi As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)

        If Not (TypeOf tsi.Tag Is IMenuItemPlugin) Then Return

        ' Fire plugin
        Me.RunPlugin(DirectCast(tsi.Tag, IGUIPlugin), sender, e)

    End Sub

    Protected Overrides Sub EnablePlugin(ByVal ip As IGUIPlugin, ByVal bEnable As Boolean)

        Dim tsic As ToolStripItemCollection = Me.m_menu.Items
        Dim atsi As ToolStripItem() = Nothing

        If Not (TypeOf ip Is IMenuItemPlugin) Then Return

        atsi = tsic.Find(ip.Name, True)
        For Each tsi As ToolStripItem In atsi
            If tsi.Tag Is ip Then
                tsi.Enabled = bEnable
            End If
        Next

    End Sub

#End Region ' Menu item handling

End Class
