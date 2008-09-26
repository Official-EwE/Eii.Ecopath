'==============================================================================
'
' $Log: ucHatchSelect.vb,v $
' Revision 1.1  2008/09/26 07:31:26  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2008/01/01 19:51:18  jeroens
' New and/or moved
'
' Revision 1.3  2007/12/01 19:41:39  jeroens
' + Added class description, need to document further
'
' Revision 1.2  2007/09/30 19:09:03  jeroens
' * Hatch combo closed on double-click
'
' Revision 1.1  2007/09/21 16:33:28  jeroens
' Initial version
'
'==============================================================================

Option Strict On
Imports System.Drawing.Drawing2D

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Combo box dropdown control that allows the user to pick a hatch pattern
    ''' from a range of provided system hatch brushes.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucHatchSelect

#Region " Privates "

        Private m_dtBrushes As New Dictionary(Of HatchStyle, ucHatch)
        Private m_hbsSelected As HatchStyle = Drawing2D.HatchStyle.Cross
        Private m_bHasFocus As Boolean = False
        Private m_parent As ucEditHatch = Nothing

#End Region ' Privates

        Public Sub New(ByVal parent As ucEditHatch)

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            Me.m_parent = parent

            Me.Dock = DockStyle.Fill
        End Sub

#Region " Public interfaces "

        Public Property SelectedHatchStyle() As HatchStyle
            Get
                Return Me.m_hbsSelected
            End Get
            Set(ByVal value As HatchStyle)

                If value <> Me.m_hbsSelected Then
                    Me.m_dtBrushes(Me.m_hbsSelected).Selected = False
                    Me.m_hbsSelected = value
                    Me.m_dtBrushes(Me.m_hbsSelected).Selected = True
                    ' Update parent
                    Me.m_parent.SelectedHatchStyle = value
                End If

            End Set
        End Property

        Public Sub Colours(ByVal clrFore As Color, ByVal clrBack As Color)
            For Each uc As ucHatch In Me.m_dtBrushes.Values
                uc.Colours(clrFore, clrBack)
            Next
        End Sub

#End Region ' Public interfaces

#Region " Events "

        Private Sub ucHatchSelect_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.SuspendLayout()

            ' Plunder HatchStyle enum and generate an image for each
            For Each hbs As HatchStyle In [Enum].GetValues(GetType(HatchStyle))
                If Not Me.m_dtBrushes.ContainsKey(hbs) Then
                    Dim uc As New ucHatch(Me, hbs)
                    Me.flpItems.Controls.Add(uc)
                    Me.m_dtBrushes.Add(hbs, uc)

                    AddHandler uc.Click, AddressOf OnHatchClick
                    AddHandler uc.DoubleClick, AddressOf OnHatchDoubleClick
                End If
            Next

            Me.ResumeLayout()
        End Sub

        Private Sub ucHatchSelect_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed

            ' Clean-up
            For Each ctrl As Control In Me.flpItems.Controls
                If TypeOf ctrl Is ucHatch Then
                    Dim uc As ucHatch = DirectCast(ctrl, ucHatch)

                    RemoveHandler uc.Click, AddressOf OnHatchClick
                    RemoveHandler uc.DoubleClick, AddressOf OnHatchDoubleClick
                End If
            Next
            Me.flpItems.Controls.Clear()

        End Sub

#End Region ' Events

#Region " Internal implementation "

        Private Sub OnHatchClick(ByVal sender As Object, ByVal e As EventArgs)
            Debug.Assert(TypeOf sender Is ucHatch)
            Me.SelectedHatchStyle = DirectCast(sender, ucHatch).HatchStyle
        End Sub

        Private Sub OnHatchDoubleClick(ByVal sender As Object, ByVal e As EventArgs)
            Me.m_parent.HideDropdown()
        End Sub

#End Region ' Internal implementation

    End Class

End Namespace
