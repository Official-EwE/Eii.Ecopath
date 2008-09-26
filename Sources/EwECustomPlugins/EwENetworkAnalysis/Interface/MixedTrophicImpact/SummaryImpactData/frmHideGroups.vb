'==============================================================================
'
' $Log: frmHideGroups.vb,v $
' Revision 1.1  2008/09/26 07:30:54  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.12  2007/09/27 17:31:14  joeh
' Fix bug 251
'
' Revision 1.11  2007/09/27 00:51:50  joeh
' Fix bug 251
'
' Revision 1.10  2007/09/25 19:01:33  joeb
' Fixed bug that cause IsGroupShown(strKey) to explode.
'
' Revision 1.9  2007/06/28 19:26:04  joeh
' Allow ony the first two fleet names to be hidden
'
' Revision 1.8  2007/06/22 00:35:30  joeh
' Add Option Strict On and Option Explicit On
'
' Revision 1.7  2007/06/21 00:14:40  joeh
' Rename SetUpPanel() to DisplayData()
'
' Revision 1.6  2007/06/20 18:13:58  joeh
' add header to the top of the file so that CVS will log the file with every update
'
'
'==============================================================================
Option Strict On
Option Explicit On

Imports EwECore

Public Class frmHideGroups

    Public Const FLEET_PREFIX As String = "Fleet-"
    Private Shared m_HideGroupsFormInstance As frmHideGroups

    Private m_NetworkManager As cNetworkManager
    'Private m_Core As cCore
    'Private m_GroupDisplayFlags() As Boolean
    Private m_ItemNames() As String
    Private m_ItemsDisplayed As New Dictionary(Of String, Boolean) 'Selection displayed but unconfirmed because user can click Cancel
    Private m_ItemsConfirmed As New Dictionary(Of String, Boolean)  'Selection confirmed when 'OK' button was pressed 

    Private m_NewDBModelMainNetwork As Boolean
    Private m_FormLoadCounter As Integer

    Public Shared Function GetInstance(ByVal NetworkManager As cNetworkManager) As frmHideGroups
        If m_HideGroupsFormInstance Is Nothing Then m_HideGroupsFormInstance = New frmHideGroups(NetworkManager)
        Return m_HideGroupsFormInstance
    End Function

    Private Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        'm_Core = cCore.GetInstance()

    End Sub

    Private Sub New(ByVal NetworkManager As cNetworkManager)
        Me.New()
        m_NetworkManager = NetworkManager
    End Sub


    ''' <summary>
    ''' Load the Groupname and Fleetnames of the latest manager. 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Init()

        'jb GroupNames and FleetNames can be the same!
        'Make sure the Fleet Name is unique by adding prefix to the name
        m_ItemsConfirmed.Clear()
        m_ItemsConfirmed.Add(String.Empty, False)
        For i As Integer = 1 To m_NetworkManager.nGroups
            Dim strName As String = m_NetworkManager.GroupName(i)
            m_ItemsConfirmed.Add(strName, True)
        Next
        For i As Integer = 1 To m_NetworkManager.nFleets
            Dim strName As String = m_NetworkManager.FleetName(i)
            m_ItemsConfirmed.Add(FLEET_PREFIX & strName, True)
        Next

        'Bug 251 fix
        'Add
        m_FormLoadCounter = 0
        'End Add
    End Sub

    Public ReadOnly Property IsGroupShown(ByVal strKey As String) As Boolean
        Get

            If m_ItemsConfirmed.Count = 0 Then
                'The case of no frmHideGroups has been used for selection before IsGroupShown is called.
                'Then every item is set to be selected
                'Bug 251 fix
                'Change
                'Init()
                m_ItemsConfirmed.Clear()
                m_ItemsConfirmed.Add(String.Empty, False)
                For i As Integer = 1 To m_NetworkManager.nGroups
                    Dim strName As String = m_NetworkManager.GroupName(i)
                    m_ItemsConfirmed.Add(strName, True)
                Next
                For i As Integer = 1 To m_NetworkManager.nFleets
                    Dim strName As String = m_NetworkManager.FleetName(i)
                    m_ItemsConfirmed.Add(FLEET_PREFIX & strName, True)
                Next
                'End Change
            End If
            Return m_ItemsConfirmed(strKey)

        End Get
    End Property

    Private Sub HideGroups_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        m_FormLoadCounter = m_FormLoadCounter + 1

        'Bug 251 fix
        'Change
        'If a new db model has just been opened and Main Network Analysis has not been run
        'Or this form is loaded the first time Then refresh the form
        'If m_FormLoadCounter = 1 Then
        If m_NetworkManager.IsMainNetworkRun = False Or m_FormLoadCounter = 1 Then
            'End change
            'm_GroupDisplayFlags = AppLauncher.GetInstance.GroupDisplayFlags

            'Add an empty item into m_ItemsDisplayed for Position zero, 
            'So all data structures index from One for consistency
            m_ItemsDisplayed.Clear()
            m_ItemsDisplayed.Add(String.Empty, False)

            'ReDim m_ItemNames(m_GroupDisplayFlags.Length - 1)
            'For i As Integer = 1 To m_GroupDisplayFlags.Length - 3
            '    Dim name As String = m_Core.EcoPathGroupInputs(i).Name
            '    m_ItemsDisplayed.Add(name, m_GroupDisplayFlags(i))
            '    m_ItemNames(i) = name
            'Next
            ReDim m_ItemNames(m_NetworkManager.nGroups + m_NetworkManager.nFleets)
            For i As Integer = 1 To m_NetworkManager.nGroups
                Dim strName As String = m_NetworkManager.GroupName(i)
                m_ItemsDisplayed.Add(strName, True)
                m_ItemNames(i) = strName
            Next
            For i As Integer = 1 To m_NetworkManager.nFleets
                Dim strName As String = m_NetworkManager.FleetName(i)
                'Bug 251 fix
                'Change
                'm_ItemsDisplayed.Add(strName, True)
                m_ItemsDisplayed.Add(FLEET_PREFIX & strName, True)
                'm_ItemNames(i + m_NetworkManager.nGroups) = strName
                m_ItemNames(i + m_NetworkManager.nGroups) = FLEET_PREFIX & strName
                'End change
            Next

            'm_ItemsDisplayed.Add("Total catch", m_GroupDisplayFlags(m_GroupDisplayFlags.Length - 2))
            'm_ItemNames(m_GroupDisplayFlags.Length - 2) = "Total catch"
            'm_ItemsDisplayed.Add("Total Length", m_GroupDisplayFlags(m_GroupDisplayFlags.Length - 1))
            'm_ItemNames(m_GroupDisplayFlags.Length - 1) = "Total Length"

            LoadLists(0, 0)

            'Treat it as if 'OK' button has been pressed thus set m_ItemsConfirmed dictionary
            m_ItemsConfirmed.Clear()
            m_ItemsConfirmed.Add(String.Empty, False)
            For i As Integer = 1 To m_NetworkManager.nGroups
                Dim strName As String = m_NetworkManager.GroupName(i)
                Dim blnShown As Boolean = m_ItemsDisplayed(strName)
                m_ItemsConfirmed.Add(strName, blnShown)
            Next
            For i As Integer = 1 To m_NetworkManager.nFleets
                Dim strName As String = m_NetworkManager.FleetName(i)
                'Bug 251 fix
                'Change
                'Dim blnShown As Boolean = m_ItemsDisplayed(strName)
                Dim blnShown As Boolean = m_ItemsDisplayed(FLEET_PREFIX & strName)
                'm_ItemsConfirmed.Add(strName, blnShown)
                m_ItemsConfirmed.Add(FLEET_PREFIX & strName, blnShown)
                'End change
            Next
        End If

    End Sub

    Private Sub LoadLists(ByVal i1Select As Integer, ByVal i2Select As Integer)

        'Init the form controls
        lbDisplayedGrps.Items.Clear()
        lbHiddenGrps.Items.Clear()

        SetButtonsStatus(New Boolean() {True, True, True, True})

        'Insert list into items
        For i As Integer = 1 To m_ItemNames.Length - 1
            InsertItemToList(m_ItemNames(i))
        Next

        If lbDisplayedGrps.Items.Count > 0 Then
            If i1Select = -1 Then i1Select = 0
            lbDisplayedGrps.SelectedIndex = i1Select
        Else
            SetButtonsStatus(New Boolean() {False, True, False, True})
        End If

        If lbHiddenGrps.Items.Count > 0 Then
            If i2Select = -1 Then i2Select = 0
            lbHiddenGrps.SelectedIndex = i2Select
        Else
            SetButtonsStatus(New Boolean() {True, False, True, False})
        End If

    End Sub

    Private Sub InsertItemToList(ByRef item As String)

        If m_ItemsDisplayed(item) Then
            lbDisplayedGrps.Items.Add(item)
        Else
            lbHiddenGrps.Items.Add(item)
        End If

    End Sub

    Private Sub SetButtonsStatus(ByVal v() As Boolean)

        btnHideOne.Enabled = v(0)
        btnShowOne.Enabled = v(1)
        btnHideAll.Enabled = v(2)
        btnShowAll.Enabled = v(3)

    End Sub

    Private Sub btnHideOne_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHideOne.Click

        ShowHideOneGroup(lbDisplayedGrps, lbHiddenGrps, False)

    End Sub

    Private Sub btnShowOne_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnShowOne.Click

        ShowHideOneGroup(lbHiddenGrps, lbDisplayedGrps, True)

    End Sub

    Private Sub btnHideAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHideAll.Click
        ShowHideAllGroups(lbDisplayedGrps, lbHiddenGrps, False)
    End Sub

    Private Sub btnShowAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnShowAll.Click
        ShowHideAllGroups(lbHiddenGrps, lbDisplayedGrps, True)
    End Sub

    Private Sub ShowHideOneGroup(ByRef l1 As Windows.Forms.ListBox, ByRef l2 As Windows.Forms.ListBox, ByVal isShown As Boolean)

        Dim i1Select As Integer = l1.SelectedIndex

        'Ony the first two fleet names are allowed to be hidden
        For i As Integer = 3 To m_NetworkManager.nFleets
            'Bug 251 fix
            'Change
            'If l1.SelectedItem.ToString = m_NetworkManager.FleetName(i) Then Return
            If l1.SelectedItem.ToString = FLEET_PREFIX & m_NetworkManager.FleetName(i) Then Return
            'End change
        Next

        If i1Select <> -1 Then

            m_ItemsDisplayed(CStr(l1.SelectedItem)) = isShown
            If i1Select = l1.Items.Count - 1 Then
                i1Select -= 1
            End If
            Dim i2Select As Integer = l2.SelectedIndex

            If isShown Then
                LoadLists(i2Select, i1Select)
            Else
                LoadLists(i1Select, i2Select)
            End If

        End If

    End Sub

    Private Sub ShowHideAllGroups(ByRef l1 As Windows.Forms.ListBox, ByRef l2 As Windows.Forms.ListBox, ByVal isShown As Boolean)

        If isShown Then
            For i As Integer = 1 To m_ItemsDisplayed.Count - 1
                m_ItemsDisplayed(m_ItemNames(i)) = isShown
            Next
        Else
            For i As Integer = 1 To m_ItemsDisplayed.Count - 1
                'Ony the first two fleet names are allowed to be hidden
                For j As Integer = 3 To m_NetworkManager.nFleets
                    'Bug 251 fix
                    'Change
                    'If m_ItemNames(i) = m_NetworkManager.FleetName(j) Then
                    If m_ItemNames(i) = FLEET_PREFIX & m_NetworkManager.FleetName(j) Then
                        m_ItemsDisplayed(m_ItemNames(i)) = True
                        Exit For
                    Else
                        m_ItemsDisplayed(m_ItemNames(i)) = isShown
                    End If
                Next
            Next

        End If

        LoadLists(0, 0)

    End Sub

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click

        Me.DialogResult = System.Windows.Forms.DialogResult.OK

        'Save the flag back to GroupDisplayFlags
        'ReDim m_GroupDisplayFlags(m_ItemNames.Length - 1)
        'For i As Integer = 1 To m_ItemNames.Length - 1
        '    m_GroupDisplayFlags(i) = m_ItemsDisplayed(m_ItemNames(i))
        'Next

        'AppLauncher.GetInstance.GroupDisplayFlags = m_GroupDisplayFlags

        'Selection confirmed thus set m_ItemsConfirmed dictionary
        m_ItemsConfirmed.Clear()
        m_ItemsConfirmed.Add(String.Empty, False)
        For i As Integer = 1 To m_NetworkManager.nGroups
            Dim strName As String = m_NetworkManager.GroupName(i)
            Dim blnShown As Boolean = m_ItemsDisplayed(strName)
            m_ItemsConfirmed.Add(strName, blnShown)
        Next
        For i As Integer = 1 To m_NetworkManager.nFleets
            Dim strName As String = m_NetworkManager.FleetName(i)
            'Bug 251 fix
            'Change
            'Dim blnShown As Boolean = m_ItemsDisplayed(strName)
            Dim blnShown As Boolean = m_ItemsDisplayed(FLEET_PREFIX & strName)
            'm_ItemsConfirmed.Add(strName, blnShown)
            m_ItemsConfirmed.Add(FLEET_PREFIX & strName, blnShown)
            'End change
        Next
        Me.Close()

    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel

        'Revert back to selection confirmed when 'OK' button was last pressed 
        m_ItemsDisplayed.Clear()
        m_ItemsDisplayed.Add(String.Empty, False)
        For i As Integer = 1 To m_NetworkManager.nGroups
            Dim strName As String = m_NetworkManager.GroupName(i)
            Dim blnShown As Boolean = m_ItemsConfirmed(strName)
            m_ItemsDisplayed.Add(strName, blnShown)
        Next
        For i As Integer = 1 To m_NetworkManager.nFleets
            Dim strName As String = m_NetworkManager.FleetName(i)
            'Bug 251 fix
            'Change
            'Dim blnShown As Boolean = m_ItemsConfirmed(strName)
            Dim blnShown As Boolean = m_ItemsConfirmed(FLEET_PREFIX & strName)
            'm_ItemsDisplayed.Add(strName, blnShown)
            m_ItemsDisplayed.Add(FLEET_PREFIX & strName, blnShown)
            'End change
        Next
        LoadLists(0, 0)

        Me.Close()
    End Sub

End Class

