'==============================================================================
'
' $Log: EwEHeaderCell.vb,v $
' Revision 1.1  2009/03/30 16:59:25  jeroens
' Split
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwECore
Imports ScientificInterfaceShared.Properties
Imports ScientificInterfaceShared.Style
Imports SourceGrid2
Imports SourceGrid2.Cells.Real
Imports SourceGrid2.VisualModels
Imports EwEUtils.Commands
Imports EwEUtils.Core

#End Region ' Imports

Namespace Controls.EwEGrid

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' EwERowHeaderCell implements a Common cell rendered as an EwE name field.
    ''' EwERowHeaderCells are used in EwE to replace Row headers which values are statically
    ''' set.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustInherit Class EwEHeaderCell
        : Inherits EwECell

#Region " Construction "

        Public Sub New(Optional ByVal objValue As Object = Nothing)
            MyBase.New(objValue, GetType(String))
            ' Disable edit
            Me.DataModel.EnableEdit = False
        End Sub

        Public Sub New(ByVal strUnitMask As String, ByVal unitType As StyleGuide.eUnitType)
            Me.New("")
            Me.SetUnitHeader(strUnitMask, New StyleGuide.eUnitType() {unitType})
        End Sub

        Public Sub New(ByVal strUnitMask As String, ByVal aUnitTypes() As StyleGuide.eUnitType)
            Me.New("")
            Me.SetUnitHeader(strUnitMask, aUnitTypes)
        End Sub

#End Region ' Construction 

#Region " Data (style) "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to enusre that header cells use names colour feedback
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overrides Property Style() As StyleGuide.eStyleFlags
            Get
                Return (StyleGuide.eStyleFlags.Names Or StyleGuide.eStyleFlags.NotEditable Or MyBase.Style)
            End Get
            Set(ByVal styleNew As StyleGuide.eStyleFlags)
                MyBase.Style = (styleNew Or StyleGuide.eStyleFlags.Names Or StyleGuide.eStyleFlags.NotEditable)
            End Set
        End Property

#End Region ' Data (style) 

#Region " Unit header text "

        Protected m_aUnitTypes() As StyleGuide.eUnitType
        Protected m_strUnitMask As String = ""

        Public Sub SetUnitHeader(ByVal strUnitMask As String, ByVal aUnitTypes() As StyleGuide.eUnitType)
            ' Sanity checks
            Debug.Assert(aUnitTypes.Length = 1 Or aUnitTypes.Length = 2)

            Me.m_strUnitMask = strUnitMask
            Me.m_aUnitTypes = aUnitTypes
        End Sub

        Public Overrides ReadOnly Property DisplayText() As String
            Get
                Dim strDisplayText As String = ""

                If m_aUnitTypes Is Nothing Then
                    strDisplayText = MyBase.DisplayText
                Else
                    Select Case m_aUnitTypes.Length
                        Case 0
                            strDisplayText = MyBase.DisplayText
                        Case 1
                            strDisplayText = String.Format(Me.m_strUnitMask, GetUnitString(m_aUnitTypes(0)))
                        Case 2
                            strDisplayText = String.Format(Me.m_strUnitMask, GetUnitString(m_aUnitTypes(0)), GetUnitString(m_aUnitTypes(1)))
                        Case Else
                            Debug.Assert(False)
                    End Select
                End If
                Return strDisplayText
            End Get
        End Property

        Private Function GetUnitString(ByVal unitType As StyleGuide.eUnitType) As String
            Dim sg As StyleGuide = StyleGuide.GetInstance()
            Dim strUnitString As String = ""
            Select Case unitType
                Case StyleGuide.eUnitType.Currency
                    strUnitString = sg.CurrencyUnitText(sg.CurrencyUnit)
                Case StyleGuide.eUnitType.Time
                    strUnitString = sg.TimeUnitText(sg.TimeUnit)
                Case StyleGuide.eUnitType.Monetary
                    strUnitString = sg.MonetaryUnitText(sg.MonetaryUnit)
                Case StyleGuide.eUnitType.Nominal
                    strUnitString = sg.NominalUnitText()
                Case Else
                    Debug.Assert(False)
            End Select
            Return strUnitString
        End Function

#End Region ' Unit header text

    End Class

End Namespace
