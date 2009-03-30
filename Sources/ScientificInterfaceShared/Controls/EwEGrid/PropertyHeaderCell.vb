'==============================================================================
'
' $Log: PropertyHeaderCell.vb,v $
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
    ''' PropertyHeaderCell implements a PropertyCell based class for creating 
    ''' header cells.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustInherit Class PropertyHeaderCell
        : Inherits PropertyCell

#Region " Construction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="prop"><see cref="cProperty">Property</see> to attach to the cell.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal prop As cProperty)
            ' Call baseclass constructor
            MyBase.New(prop)
            ' Always
            Me.DataModel.EnableEdit = False
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, instructing the cell to use a unit mask.
        ''' </summary>
        ''' <param name="prop"><see cref="cProperty">Property</see> to attach to the cell.</param>
        ''' <param name="strUnitMask">Mask that specifies how to substitute a
        ''' <see cref="StyleGuide.eUnitType">unit of measurement</see> into
        ''' the cell value.</param>
        ''' <param name="unitType">The <see cref="StyleGuide.eUnitType">unit of measurement</see>
        ''' to substitute into the header cell text.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal prop As cProperty, ByVal strUnitMask As String, ByVal unitType As StyleGuide.eUnitType)
            Me.New(prop)
            Me.SetUnitHeader(strUnitMask, New StyleGuide.eUnitType() {unitType})
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, instructing the cell to use a unit mask.
        ''' </summary>
        ''' <param name="prop"><see cref="cProperty">Property</see> to attach to the cell.</param>
        ''' <param name="strUnitMask">Mask that specifies how to substitute a series
        ''' of <see cref="StyleGuide.eUnitType">unit of measurements</see> into
        ''' the cell value.</param>
        ''' <param name="aUnitTypes">The <see cref="StyleGuide.eUnitType">unit of measurements</see>
        ''' to substitute into the header cell text.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal prop As cProperty, ByVal strUnitMask As String, ByVal aUnitTypes() As StyleGuide.eUnitType)
            Me.New(prop)
            Me.SetUnitHeader(strUnitMask, aUnitTypes)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> data source</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">VarName flag</see> that defines which aspect of the Source to acces</param>
        ''' <param name="SourceSec">An optional secundary index in the VarName, or <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> when irrelevant</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal Source As cCoreInputOutputBase, ByVal VarName As eVarNameFlags, _
                Optional ByVal SourceSec As cCoreInputOutputBase = Nothing)
            Me.New(cPropertyManager.GetInstance().GetProperty(Source, VarName, SourceSec))
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> data source</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">VarName flag</see> that defines which aspect of the Source to acces</param>
        ''' <param name="SourceSec">Secundary index in the VarName, or <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> when irrelevant</param>
        ''' <param name="strUnitMask">Mask that specifies how to substitute a
        ''' <see cref="StyleGuide.eUnitType">unit of measurement</see> into
        ''' the cell value.</param>
        ''' <param name="unitType">The <see cref="StyleGuide.eUnitType">unit of measurement</see>
        ''' to substitute into the header cell text.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal Source As cCoreInputOutputBase, ByVal VarName As eVarNameFlags, _
                ByVal SourceSec As cCoreInputOutputBase, _
                ByVal strUnitMask As String, ByVal unitType As StyleGuide.eUnitType)
            Me.New(cPropertyManager.GetInstance().GetProperty(Source, VarName, SourceSec), strUnitMask, New StyleGuide.eUnitType() {unitType})
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> data source</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">VarName flag</see> that defines which aspect of the Source to acces</param>
        ''' <param name="SourceSec">Secundary index in the VarName, or <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> when irrelevant</param>
        ''' <param name="strUnitMask">Mask that specifies how to substitute a series
        ''' of <see cref="StyleGuide.eUnitType">unit of measurements</see> into
        ''' the cell value.</param>
        ''' <param name="aUnitTypes">The <see cref="StyleGuide.eUnitType">unit of measurements</see>
        ''' to substitute into the header cell text.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal Source As cCoreInputOutputBase, ByVal VarName As eVarNameFlags, _
                ByVal SourceSec As cCoreInputOutputBase, _
                ByVal strUnitMask As String, ByVal aUnitTypes() As StyleGuide.eUnitType)
            Me.New(cPropertyManager.GetInstance().GetProperty(Source, VarName, SourceSec), strUnitMask, aUnitTypes)
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
                Return (MyBase.Style Or StyleGuide.eStyleFlags.NotEditable)
            End Get
            Set(ByVal styleNew As StyleGuide.eStyleFlags)
                MyBase.Style = (styleNew Or StyleGuide.eStyleFlags.NotEditable)
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

                If (m_aUnitTypes Is Nothing) Or (String.IsNullOrEmpty(Me.m_strUnitMask)) Then
                    strDisplayText = MyBase.DisplayText
                Else
                    Select Case m_aUnitTypes.Length
                        Case 0
                            strDisplayText = String.Format(MyBase.DisplayText, Me.Value)
                        Case 1
                            strDisplayText = String.Format(Me.m_strUnitMask, Me.Value, GetUnitString(m_aUnitTypes(0)))
                        Case 2
                            strDisplayText = String.Format(Me.m_strUnitMask, Me.Value, GetUnitString(m_aUnitTypes(0)), GetUnitString(m_aUnitTypes(1)))
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
