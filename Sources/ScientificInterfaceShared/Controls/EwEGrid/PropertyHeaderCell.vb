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
    ''' header cells in <see cref="EwEGrid">EwE grids</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
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
        ''' <see cref="cStyleGuide.eUnitType">unit of measurement</see> into
        ''' the cell value.</param>
        ''' <param name="unitType">The <see cref="cStyleGuide.eUnitType">unit of measurement</see>
        ''' to substitute into the header cell text.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal prop As cProperty, _
                       ByVal strUnitMask As String, _
                       ByVal unitType As cStyleGuide.eUnitType)
            Me.New(prop)
            Me.SetUnitHeader(strUnitMask, New cStyleGuide.eUnitType() {unitType})
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, instructing the cell to use a unit mask.
        ''' </summary>
        ''' <param name="prop"><see cref="cProperty">Property</see> to attach to the cell.</param>
        ''' <param name="strUnitMask">Mask that specifies how to substitute a series
        ''' of <see cref="cStyleGuide.eUnitType">unit of measurements</see> into
        ''' the cell value.</param>
        ''' <param name="aUnitTypes">The <see cref="cStyleGuide.eUnitType">unit of measurements</see>
        ''' to substitute into the header cell text.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal prop As cProperty, ByVal strUnitMask As String, ByVal aUnitTypes() As cStyleGuide.eUnitType)
            Me.New(prop)
            Me.SetUnitHeader(strUnitMask, aUnitTypes)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="pm"><see cref="cPropertyManager">Property manager</see> to obtain values from.</param>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> data source</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">VarName flag</see> that defines which aspect of the Source to acces</param>
        ''' <param name="SourceSec">An optional secundary index in the VarName, or <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> when irrelevant</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal pm As cPropertyManager, _
                       ByVal Source As cCoreInputOutputBase, _
                       ByVal VarName As eVarNameFlags, _
                       Optional ByVal SourceSec As cCoreInputOutputBase = Nothing)
            Me.New(pm.GetProperty(Source, VarName, SourceSec))
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="pm"><see cref="cPropertyManager">Property manager</see> to obtain values from.</param>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> data source</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">VarName flag</see> that defines which aspect of the Source to acces</param>
        ''' <param name="SourceSec">Secundary index in the VarName, or <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> when irrelevant</param>
        ''' <param name="strUnitMask">Mask that specifies how to substitute a
        ''' <see cref="cStyleGuide.eUnitType">unit of measurement</see> into
        ''' the cell value.</param>
        ''' <param name="unitType">The <see cref="cStyleGuide.eUnitType">unit of measurement</see>
        ''' to substitute into the header cell text.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal pm As cPropertyManager, _
                       ByVal Source As cCoreInputOutputBase, _
                       ByVal VarName As eVarNameFlags, _
                       ByVal SourceSec As cCoreInputOutputBase, _
                       ByVal strUnitMask As String, _
                       ByVal unitType As cStyleGuide.eUnitType)
            Me.New(pm.GetProperty(Source, VarName, SourceSec), strUnitMask, New cStyleGuide.eUnitType() {unitType})
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="pm"><see cref="cPropertyManager">Property manager</see> to obtain values from.</param>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> data source</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">VarName flag</see> that defines which aspect of the Source to acces</param>
        ''' <param name="SourceSec">Secundary index in the VarName, or <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> when irrelevant</param>
        ''' <param name="strUnitMask">Mask that specifies how to substitute a series
        ''' of <see cref="cStyleGuide.eUnitType">unit of measurements</see> into
        ''' the cell value.</param>
        ''' <param name="aUnitTypes">The <see cref="cStyleGuide.eUnitType">unit of measurements</see>
        ''' to substitute into the header cell text.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal pm As cPropertyManager, _
                       ByVal Source As cCoreInputOutputBase, _
                       ByVal VarName As eVarNameFlags, _
                       ByVal SourceSec As cCoreInputOutputBase, _
                       ByVal strUnitMask As String, _
                       ByVal aUnitTypes() As cStyleGuide.eUnitType)
            Me.New(pm.GetProperty(Source, VarName, SourceSec), strUnitMask, aUnitTypes)
        End Sub

#End Region ' Construction 

#Region " Data (style) "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to enusre that header cells use names colour feedback
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overrides Property Style() As cStyleGuide.eStyleFlags
            Get
                Return (MyBase.Style Or cStyleGuide.eStyleFlags.NotEditable)
            End Get
            Set(ByVal styleNew As cStyleGuide.eStyleFlags)
                MyBase.Style = (styleNew Or cStyleGuide.eStyleFlags.NotEditable)
            End Set
        End Property

#End Region ' Data (style) 

#Region " Unit header text "

        Protected m_aUnitTypes() As cStyleGuide.eUnitType
        Protected m_strUnitMask As String = ""

        Protected Sub SetUnitHeader(ByVal strUnitMask As String, ByVal aUnitTypes() As cStyleGuide.eUnitType)
            Me.m_strUnitMask = strUnitMask
            Me.m_aUnitTypes = aUnitTypes
        End Sub

        Public Overrides ReadOnly Property DisplayText() As String
            Get
                Dim strDisplayText As String = ""

                If (m_aUnitTypes Is Nothing) Or (String.IsNullOrEmpty(Me.m_strUnitMask)) Then
                    strDisplayText = MyBase.DisplayText
                Else
                    Try
                        Dim sg As cStyleGuide = Me.Property.PropertyManager.StyleGuide

                        Select Case m_aUnitTypes.Length
                            Case 0
                                strDisplayText = String.Format(Me.m_strUnitMask, Me.Value)
                            Case 1
                                strDisplayText = String.Format(Me.m_strUnitMask, Me.Value, _
                                                               sg.GetUnitString(m_aUnitTypes(0)))
                            Case 2
                                strDisplayText = String.Format(Me.m_strUnitMask, Me.Value, _
                                                               sg.GetUnitString(m_aUnitTypes(0)), _
                                                               sg.GetUnitString(m_aUnitTypes(1)))
                            Case Else
                                Debug.Assert(False)
                        End Select
                    Catch ex As Exception
                        Debug.Assert(False, "Failed to apply format mask, please check")
                        strDisplayText = MyBase.DisplayText
                    End Try
                End If
                Return strDisplayText
            End Get
        End Property

#End Region ' Unit header text

    End Class

End Namespace
