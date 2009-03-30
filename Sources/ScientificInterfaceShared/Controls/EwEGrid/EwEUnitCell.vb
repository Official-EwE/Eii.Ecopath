'==============================================================================
'
' $Log: EwEUnitCell.vb,v $
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
    ''' UnitCell implements a cell that shows a dynamic unit string.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class EwEUnitCell
        : Inherits EwECell

        ''' <summary>One visualizer for all cells</summary>
        Private Shared g_visualizer As EwECellVisualizerBase
        Protected m_aUnitTypes() As StyleGuide.eUnitType
        Protected m_strUnitMask As String = ""

#Region " Construction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal unitType As StyleGuide.eUnitType)
            Me.New("{0}", New StyleGuide.eUnitType() {unitType})
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal strUnitMask As String, ByVal aUnitTypes() As StyleGuide.eUnitType)
            MyBase.New(Nothing, GetType(String))

            Me.m_strUnitMask = strUnitMask
            Me.m_aUnitTypes = aUnitTypes
        End Sub

#End Region ' Construction 

#Region " Overrides "

        Public Overrides ReadOnly Property DisplayText() As String
            Get
                Dim strDisplayText As String = ""

                If Me.m_aUnitTypes IsNot Nothing Then

                    Select Case m_aUnitTypes.Length
                        Case 0
                            ' NOP
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

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to enusre that this cell cannot be edited.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overrides Property Style() As StyleGuide.eStyleFlags
            Get
                Return (MyBase.Style Or StyleGuide.eStyleFlags.NotEditable)
            End Get
            Set(ByVal styleNew As StyleGuide.eStyleFlags)
                MyBase.Style = (styleNew Or StyleGuide.eStyleFlags.NotEditable)
            End Set
        End Property

#End Region ' Overrides

    End Class

End Namespace
