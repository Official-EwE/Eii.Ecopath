' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Plugins.UI
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Commands



Namespace Integration

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' GUI utility class, handles the placement of <see cref="IGUIPlugin">IGUIPlugin</see>-
    ''' derived plugins in the menu structure of a <see cref="Form">Form</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cPluginMenuHandler
        Inherits cPluginGUIHandler

#Region " Private parts "

#Region " Private helper class "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Helper class, sorts IMenuItemPlugin instances by menu item name in 
        ''' ascending order.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Class MenuItemPluginComparer
            Implements IComparer(Of IMenuItemPlugin)

            Public Function Compare(x As IMenuItemPlugin,
                                    y As IMenuItemPlugin) As Integer _
                                    Implements IComparer(Of IMenuItemPlugin).Compare

                Return String.Compare(x.MenuItemLocation, y.MenuItemLocation, True)

            End Function

        End Class

#End Region ' Private helper class

        ''' <summary>The menu to modify.</summary>
        Private m_menu As MenuStrip = Nothing
        ''' <summary>The items that have been created.</summary>
        Private m_lItems As New List(Of ToolStripMenuItem)

#End Region ' Private parts

#Region " Construction "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of a cPluginMenuHandler.
        ''' </summary>
        ''' <param name="menu"><see cref="MenuStrip">Menu strip</see> that contains the menu
        ''' that must be modified.</param>
        ''' <param name="pm"><see cref="cPluginManager">Plugin manager</see>
        ''' that holds the plugins to place in the main menu.</param>
        ''' -----------------------------------------------------------------------
        Public Sub New(menu As MenuStrip,
                       pm As cPluginManager,
                       cmdh As cCommandHandler)

            MyBase.New(pm, cmdh)
            Me.m_menu = menu

        End Sub

#End Region ' Construction 

#Region " Command idle time updating "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Application idle time handler, makes sure that every plug-in menu item
        ''' is updated.
        ''' </summary>
        ''' <remarks>
        ''' This method should be invoked in response to the .NET Idle event.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Sub OnIdle(sender As Object, e As EventArgs)
            For Each tsi As ToolStripMenuItem In Me.m_lItems
                If (TypeOf tsi.Tag Is IMenuItemTogglePlugin) Then
                    Try
                        Dim ip As IMenuItemTogglePlugin = DirectCast(tsi.Tag, IMenuItemTogglePlugin)
                        tsi.Checked = ip.IsChecked
                    Catch ex As Exception

                    End Try
                End If
            Next
        End Sub

#End Region ' Command idle time updating 

#Region " Menu item handling "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Sort a list of menu item plug-ins by menu item name.
        ''' </summary>
        ''' <param name="aip">The plug-ins to sort.</param>
        ''' <returns>An array of sorted plug-ins.</returns>
        ''' -----------------------------------------------------------------------
        Protected Overrides Function SortPlugins(aip() As IGUIPlugin) As IGUIPlugin()

            Dim lPlugins As New List(Of IMenuItemPlugin)

            For Each ip As IGUIPlugin In aip
                If TypeOf ip Is IMenuItemPlugin Then
                    lPlugins.Add(DirectCast(ip, IMenuItemPlugin))
                End If
            Next

            lPlugins.Sort(New MenuItemPluginComparer())

            Return lPlugins.ToArray()

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Place or remove a GUI plugin menu item. Menu items are inserted sorted by
        ''' <see cref="MenuItem.Name">name</see>, ascending.
        ''' </summary>
        ''' <param name="p_ip">The <see cref="IGUIPlugin">IGUIPlugin</see> to place.</param>
        ''' <param name="bPlace">States whether the menu item should be placed (True)
        ''' or removed (False).</param>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub PlacePlugin(p_ip As IGUIPlugin, bPlace As Boolean)

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
            ' Split locations path
            If strLocation.IndexOf("\") >= 0 Then
                ' New style: split locations by path separator char '\'
                aLocations = strLocation.Split("\"c)
            Else
                ' Old style: split locations by pipe char '|'
                aLocations = strLocation.Split("|"c)
            End If
            bFound = String.IsNullOrWhiteSpace(strLocation)

            ' Find named menu item for every level
            While iLocation < aLocations.Length And Not bError
                iItem = 0
                bFound = False

                ' New EwE 6.6: can create menu items now too
                Dim strNodeName As String = aLocations(iLocation).Trim()

                bFound = String.IsNullOrWhiteSpace(strNodeName)
                While iItem < tsic.Count And Not bFound
                    If (TypeOf tsic.Item(iItem) Is ToolStripMenuItem) Then
                        tsi = DirectCast(tsic.Item(iItem), ToolStripMenuItem)
                        bFound = (String.Compare(tsi.Name.Trim(), strNodeName, False) = 0)
                    End If
                    iItem += 1
                End While
                If bFound Then
                    If (tsi IsNot Nothing) Then tsic = tsi.DropDownItems
                End If
                bError = Not bFound
                iLocation += 1
            End While

            Try

                ' Found item position?
                If Not bError Then
                    If (bPlace) Then
                        Dim img As Image = TryCast(ip.ControlImage, Image)
                        ' Create menu item and add it
                        tsi = New ToolStripMenuItem(ip.DisplayName, img, AddressOf Me.OnPluginMenuItemClick)
                        ' Set name
                        tsi.Name = ip.DisplayName
                        ' Set tooltip text
                        tsi.ToolTipText = cStringUtils.ToTooltip(ip.ControlTooltipText)
                        ' Add tag
                        tsi.Tag = ip

                        Me.m_lItems.Add(tsi)

                        ' try to insert menu item into strip
                        Dim bFoundGroup As Boolean = False
                        For iItem = 0 To tsic.Count - 1
                            If (tsi.Name.Contains(tsic(iItem).Name)) Then
                                bFoundGroup = True
                            ElseIf (bFoundGroup) Then
                                tsic.Insert(iItem, tsi)
                                Return ' Done
                            End If
                        Next
                        ' Add new item to menu item strip
                        tsic.Add(tsi)
                    Else
                        ' Remove menu item
                        tsic.RemoveByKey(ip.DisplayName)
                        tsi = DirectCast(tsic.Find(ip.DisplayName, True)(0), ToolStripMenuItem)
                        Me.m_lItems.Remove(tsi)
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
        Private Sub OnPluginMenuItemClick(sender As Object, e As EventArgs)

            Debug.Assert(TypeOf sender Is ToolStripMenuItem)

            Dim tsi As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)

            If Not (TypeOf tsi.Tag Is IMenuItemPlugin) Then Return

            ' Fire plugin
            Me.RunPlugin(DirectCast(tsi.Tag, IGUIPlugin), sender, e)

        End Sub

        Protected Overrides Sub EnablePlugin(ip As IGUIPlugin, bEnable As Boolean)

            Dim tsic As ToolStripItemCollection = Me.m_menu.Items
            Dim atsi As ToolStripItem() = Nothing

            If Not (TypeOf ip Is IMenuItemPlugin) Then Return

            atsi = tsic.Find(ip.DisplayName, True)
            For Each tsi As ToolStripItem In atsi
                If (tsi.Tag Is ip) Then
                    tsi.Enabled = bEnable
                End If
            Next

        End Sub

#End Region ' Menu item handling

    End Class

End Namespace
