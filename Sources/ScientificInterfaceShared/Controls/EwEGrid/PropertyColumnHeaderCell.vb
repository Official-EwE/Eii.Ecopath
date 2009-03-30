'==============================================================================
'
' $Log: PropertyColumnHeaderCell.vb,v $
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
    ''' PropertyColumnHeaderCell implements a PropertyCellBase rendered as clickable,
    ''' sortable column header.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class PropertyColumnHeaderCell
        : Inherits PropertyHeaderCell

        ''' <summary>One visualizer for all cells</summary>
        Private Shared g_visualizer As New EwEColumnHeaderVisualizer()

#Region " Construction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="prop">cProperty to attach to the cell</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal prop As cProperty)
            MyBase.New(prop)
            ' Set shared visualizer
            Me.VisualModel = g_visualizer
            If prop.VarName <> eVarNameFlags.Name Then Me.ToolTipText = prop.Source().Name
        End Sub

        Public Sub New(ByVal prop As cProperty, ByVal strUnitMask As String, ByVal unitType As StyleGuide.eUnitType)
            Me.New(prop)
            Me.SetUnitHeader(strUnitMask, New StyleGuide.eUnitType() {unitType})
        End Sub

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
        ''' <param name="SourceSec">An optional secundary index in the VarName, or Nothing when irrelevant</param>
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

    End Class

End Namespace
