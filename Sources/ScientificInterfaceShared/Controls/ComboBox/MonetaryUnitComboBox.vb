'==============================================================================
'
' $Log: MonetaryUnitComboBox.vb,v $
' Revision 1.1  2008/09/26 07:31:15  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.2  2008/07/18 19:26:11  jeroens
' Uses styleguide to obtain currency descriptions
'
' Revision 1.1  2008/07/17 19:18:26  jeroens
' Initial version
'
'==============================================================================

#Region "Imports Directive"

Option Strict On

Imports EwEUtils.Core
Imports ScientificInterfaceShared.Style

#End Region

Namespace Controls

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Combo box that allows the user to select a 
    ''' <see cref="eUnitMonetaryType">onetary unit</see>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class MonetaryUnitComboBox

#Region " Helper classes "

        Private Class MonetaryUnitItem

            Private m_unit As eUnitMonetaryType = 0
            Private m_strDescription As String

            Public Sub New(ByVal unit As eUnitMonetaryType, ByVal strDescription As String)
                Me.m_unit = unit
                Me.m_strDescription = strDescription
            End Sub

            Public Overrides Function ToString() As String
                Return Me.m_strDescription
            End Function

            Public ReadOnly Property Unit() As eUnitMonetaryType
                Get
                    Return Me.m_unit
                End Get
            End Property

        End Class

#End Region ' Helper classes

        Public Sub New()
            Me.InitializeComponent()
            Me.Populate()
        End Sub

        Private Sub Populate()

            Dim sg As StyleGuide = StyleGuide.GetInstance()
            Dim strLabel As String = ""

            Me.SuspendLayout()
            For Each unit As eUnitMonetaryType In [Enum].GetValues(GetType(eUnitMonetaryType))
                If (unit <> eUnitMonetaryType.Custom) Then
                    strLabel = String.Format(My.Resources.GENERIC_LABEL_CURRENCY, _
                        sg.MonetaryUnitText(unit), _
                        sg.MonetaryUnitDescription(unit))
                    Me.Items.Add(New MonetaryUnitItem(unit, strLabel))
                End If
            Next
            Me.Sorted = True
            Me.DropDownStyle = ComboBoxStyle.DropDownList
            Me.ResumeLayout()

        End Sub

        Public Property Unit() As eUnitMonetaryType
            Get
                If TypeOf Me.SelectedItem Is MonetaryUnitItem Then
                    Return DirectCast(Me.SelectedItem, MonetaryUnitItem).Unit
                Else
                    Return eUnitMonetaryType.Custom
                End If
            End Get
            Set(ByVal value As eUnitMonetaryType)
                For iItem As Integer = 0 To Me.Items.Count - 1
                    If TypeOf Me.Items(iItem) Is MonetaryUnitItem Then
                        If DirectCast(Me.Items(iItem), MonetaryUnitItem).Unit = value Then
                            Me.SelectedIndex = iItem
                        End If
                    End If
                Next
            End Set
        End Property
    End Class

End Namespace
