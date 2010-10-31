#Region " Imports "

Option Strict On

Imports EwEUtils.Core
Imports ScientificInterfaceShared.Style

#End Region

Namespace Controls

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Combo box that allows the user to select a <see cref="eUnitMonetaryType">Monetary unit</see>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cMonetaryUnitComboBox
        Implements IUIElement

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

#Region " Private vars "

        Private m_uic As cUIContext = Nothing

#End Region ' Private vars

        Public Sub New()
            Me.InitializeComponent()
            Me.DropDownStyle = ComboBoxStyle.DropDownList
        End Sub

        Public Property UIContext() As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(ByVal value As cUIContext)
                Me.m_uic = value
                Me.Populate()
            End Set
        End Property

        Private Sub Populate()

            If Me.m_uic Is Nothing Then Return

            Dim sg As cStyleGuide = Me.m_uic.StyleGuide
            Dim strLabel As String = ""

            Me.SuspendLayout()
            For Each unit As eUnitMonetaryType In [Enum].GetValues(GetType(eUnitMonetaryType))
                If (unit <> eUnitMonetaryType.NotSet) Then
                    strLabel = String.Format(My.Resources.GENERIC_LABEL_DOUBLE, _
                        sg.MonetaryUnitText(unit), _
                        sg.MonetaryUnitDescription(unit))
                    Me.Items.Add(New MonetaryUnitItem(unit, strLabel))
                End If
            Next
            Me.Sorted = True
            Me.ResumeLayout()

        End Sub

        Public Property Unit() As eUnitMonetaryType
            Get
                If TypeOf Me.SelectedItem Is MonetaryUnitItem Then
                    Return DirectCast(Me.SelectedItem, MonetaryUnitItem).Unit
                Else
                    Return eUnitMonetaryType.NotSet
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
