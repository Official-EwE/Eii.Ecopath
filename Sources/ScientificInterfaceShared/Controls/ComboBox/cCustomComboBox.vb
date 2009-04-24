'==============================================================================
'
' $Log: cCustomComboBox.vb,v $
' Revision 1.2  2009/04/24 13:58:42  jeroens
' Me
'
' Revision 1.1  2009/02/24 03:47:36  jeroens
' Renamed
'
' Revision 1.2  2008/12/15 15:37:27  jeroens
' no message
'
' Revision 1.1  2008/06/01 23:45:06  jeroens
' Separated from Scientific Interface
'
' Revision 1.2  2007/12/14 15:49:59  jeroens
' * Renamed conflicting property
'
' Revision 1.1  2007/09/07 13:33:08  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On

Imports EwECore

#End Region

Namespace Controls

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' <para>ComboBox-derived class that drops down any custom control.</para>
    ''' <para>This class was based on the Custom ComboBox by Jaredpar, 
    ''' http://blogs.msdn.com/jaredpar/archive/2006/10/13/custom-combobox.aspx</para>
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class CustomComboBox

        ''' <summary>Form to display the control.</summary>
        Private m_form As Form = Nothing
        ''' <summary>Original drop down height, preserved.</summary>
        Private m_dropDownHeight As Integer = 0
        ''' <summary>The actual drop down control.</summary>
        Private m_control As Control = Nothing

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Setup the form to display the control
            Me.m_form = New Form()
            Me.m_form.StartPosition = FormStartPosition.Manual
            Me.m_form.FormBorderStyle = FormBorderStyle.None
            Me.m_form.Hide()
            Me.m_form.ShowInTaskbar = False

            Me.DropdownControl = New Control()
            Me.m_dropDownHeight = Me.DropDownHeight
            ' Prevent the DropDown from showing
            Me.DropDownHeight = 1
        End Sub

        Protected Overrides Sub OnDropDown(ByVal e As System.EventArgs)
            MyBase.OnDropDown(e)

            If Not Me.m_form.Visible Then
                Me.DisplayControl()
            End If

            Me.DroppedDown = False
        End Sub

        Private Sub DisplayControl()
            Dim loc As Point = Me.PointToScreen(Point.Empty)
            loc.Y += Me.Height

            Me.m_form.Location = loc
            Me.m_form.Width = Me.Width
            Me.m_form.Height = Me.m_dropDownHeight
            Me.m_form.Show()
        End Sub

        Public Property DropdownControl() As Control
            Get
                Return m_control
            End Get
            Set(ByVal value As Control)
                If Not m_control Is Nothing Then
                    Me.m_form.Controls.Remove(m_control)
                    RemoveHandler m_control.LostFocus, AddressOf Me.OnControlLostFocus
                    RemoveHandler m_control.DoubleClick, AddressOf Me.OnControlDoubleClick
                End If

                Me.m_control = value
                Me.m_control.Dock = DockStyle.Fill
                AddHandler m_control.LostFocus, AddressOf Me.OnControlLostFocus
                AddHandler m_control.DoubleClick, AddressOf Me.OnControlDoubleClick
                Me.m_form.Controls.Add(m_control)
            End Set
        End Property

        Private Sub OnControlLostFocus(ByVal sender As Object, ByVal e As EventArgs)
            Me.m_form.Hide()
        End Sub

        Private Sub OnControlDoubleClick(ByVal sender As Object, ByVal e As EventArgs)
            Me.m_form.Hide()
        End Sub

    End Class

End Namespace ' Controls
