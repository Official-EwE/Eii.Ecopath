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
    ''' Cell class to implement a row header in an <see cref="EwEGrid">EWE grid</see>, 
    ''' that dynamically derives its <see cref="Cell.DisplayText">display text</see>
    ''' from the core.
    ''' </summary>
    ''' <remarks>
    ''' <para>This class inherits from <see cref="PropertyHeaderCell">PropertyHeaderCell</see> 
    ''' to implement basic, standardized formatting for row header cells. The
    ''' display text of the cell is tracked 'live' using <see cref="cProperty">properties</see>.</para>
    ''' <para>Additionally, the cell offers capabilities to incorporate units
    ''' that are updated whenever the system display units change.</para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class PropertyRowHeaderCell
        : Inherits PropertyHeaderCell

        ''' <summary>One visualizer for all cells</summary>
        Private Shared g_visualizer As EwECellVisualizerBase

#Region " Construction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor to create a row header cell that derives its 
        ''' <see cref="DisplayText">display text</see> from a 
        ''' <see cref="cProperty">cProperty</see>.
        ''' </summary>
        ''' <param name="prop">cProperty to deliver the cell value.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal prop As cProperty)
            MyBase.New(prop)
            Me.VisualModel = New cVisualizerEwERowHeader
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor to create a row header cell that derives its 
        ''' <see cref="DisplayText">display text</see> from a 
        ''' <see cref="cProperty">cProperty</see>. The property value is 
        ''' inserted in the cell display text via a 
        ''' <see cref="Strings.Format">format mask</see>.
        ''' </summary>
        ''' <param name="prop">cProperty to deliver the cell value.</param>
        ''' <param name="strUnitMask">The format mask to apply. This mask must
        ''' contain a '{0}' field where the property value is to be inserted.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal prop As cProperty, _
                       ByVal strUnitMask As String)
            Me.New(prop)
            Me.SetUnitHeader(strUnitMask, New cStyleGuide.eUnitType() {})
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor to create a row header cell that derives its 
        ''' <see cref="DisplayText">display text</see> from a 
        ''' <see cref="cProperty">cProperty</see> and a 
        ''' <see cref="cStyleGuide.eUnitType">system unit</see>. 
        ''' Both the property value and the unit mask text are inserted in the 
        ''' cell display text via a <see cref="Strings.Format">format mask</see>.
        ''' </summary>
        ''' <param name="prop">cProperty to deliver the cell value.</param>
        ''' <param name="strUnitMask">The format mask to apply. This mask must
        ''' contain a '{0}' field to place the property value, and a '{1}' field
        ''' to place the unit value.</param>
        ''' <param name="unitType">Definition of the unit to place in the cell
        ''' display text.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal prop As cProperty, _
                       ByVal strUnitMask As String, _
                       ByVal unitType As cStyleGuide.eUnitType)
            Me.New(prop)
            Me.SetUnitHeader(strUnitMask, New cStyleGuide.eUnitType() {unitType})
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor to create a row header cell that derives its 
        ''' <see cref="DisplayText">display text</see> from a 
        ''' <see cref="cProperty">cProperty</see> and a series of
        ''' <see cref="cStyleGuide.eUnitType">system units</see>. 
        ''' Both the property value and the unit mask texts are inserted in the 
        ''' cell display text via a <see cref="Strings.Format">format mask</see>.
        ''' </summary>
        ''' <param name="prop">cProperty to deliver the cell value.</param>
        ''' <param name="strUnitMask">The format mask to apply. This mask must
        ''' contain a '{0}' field to place the property value, and placeholder
        ''' fields for the units. The unit fields must be numbered '{1}', '{2}'
        ''' etc. Units will be placed in the placeholder fields in the order that
        ''' they are defined in <paramref name="aUnitTypes">aUnitTypes</paramref>.</param>
        ''' <param name="aUnitTypes">Definitions of units to place in the cell
        ''' display text.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal prop As cProperty, _
                       ByVal strUnitMask As String, _
                       ByVal aUnitTypes() As cStyleGuide.eUnitType)
            Me.New(prop)
            Me.SetUnitHeader(strUnitMask, aUnitTypes)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor to create a row header cell that synchronizes 
        ''' its <see cref="DisplayText">display text</see> live with core data.
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> 
        ''' object to deliver the core data.</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">variable</see> 
        ''' of the <paramref name="Source">Source</paramref> to display in the cell.</param>
        ''' <param name="SourceSec">An optional secundary index in the 
        ''' <paramref name="VarName">variable</paramref>, or 
        ''' <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> when this variable
        ''' does not require an index.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal pm As cPropertyManager, _
                       ByVal Source As cCoreInputOutputBase, _
                       ByVal VarName As eVarNameFlags, _
                       Optional ByVal SourceSec As cCoreInputOutputBase = Nothing)
            Me.New(pm.GetProperty(Source, VarName, SourceSec))
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor to create a row header cell that synchronizes 
        ''' its <see cref="DisplayText">display text</see> live with core data.
        ''' The core value is inserted in the cell display text via a 
        ''' <see cref="Strings.Format">format mask</see>.
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> 
        ''' object to deliver the core data.</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">variable</see> 
        ''' of the <paramref name="Source">Source</paramref> to display in the cell.</param>
        ''' <param name="SourceSec">An optional secundary index in the 
        ''' <paramref name="VarName">variable</paramref>, or 
        ''' <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> when this variable
        ''' does not require an index.</param>
        ''' <param name="strUnitMask">The format mask to apply. This mask must
        ''' contain a '{0}' field where the property value is to be inserted.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal pm As cPropertyManager, _
                       ByVal Source As cCoreInputOutputBase, _
                       ByVal VarName As eVarNameFlags, _
                       ByVal SourceSec As cCoreInputOutputBase, _
                       ByVal strUnitMask As String)
            Me.New(pm.GetProperty(Source, VarName, SourceSec), strUnitMask)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor to create a row header cell that synchronizes 
        ''' its <see cref="DisplayText">display text</see> live with core data
        ''' and a <see cref="cStyleGuide.eUnitType">system unit</see>.
        ''' Both the core value and the unit text are inserted in the cell 
        ''' display text via a <see cref="Strings.Format">format mask</see>.
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> 
        ''' object to deliver the core data.</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">variable</see> 
        ''' of the <paramref name="Source">Source</paramref> to display in the cell.</param>
        ''' <param name="SourceSec">An optional secundary index in the 
        ''' <paramref name="VarName">variable</paramref>, or 
        ''' <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> when this variable
        ''' does not require an index.</param>
        ''' <param name="strUnitMask">The format mask to apply. This mask must
        ''' contain a '{0}' field to place the property value, and placeholder
        ''' fields for the units. The unit fields must be numbered '{1}', '{2}'
        ''' etc. Units will be placed in the placeholder fields in the order that
        ''' they are defined in <paramref name="aUnitTypes">aUnitTypes</paramref>.</param>
        ''' <param name="unitType">Definition of the unit to place in the cell
        ''' display text.</param>
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
        ''' Constructor to create a row header cell that synchronizes 
        ''' its <see cref="DisplayText">display text</see> live with core data
        ''' and a series of<see cref="cStyleGuide.eUnitType">system units</see>.
        ''' Both the core value and the unit texts are inserted in the cell 
        ''' display text via a <see cref="Strings.Format">format mask</see>.
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> 
        ''' object to deliver the core data.</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">variable</see> 
        ''' of the <paramref name="Source">Source</paramref> to display in the cell.</param>
        ''' <param name="SourceSec">An optional secundary index in the 
        ''' <paramref name="VarName">variable</paramref>, or 
        ''' <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> when this variable
        ''' does not require an index.</param>
        ''' <param name="strUnitMask">The format mask to apply. This mask must
        ''' contain a '{0}' field to place the property value, and placeholder
        ''' fields for the units. The unit fields must be numbered '{1}', '{2}'
        ''' etc. Units will be placed in the placeholder fields in the order that
        ''' they are defined in <paramref name="aUnitTypes">aUnitTypes</paramref>.</param>
        ''' <param name="aUnitTypes">Definitions of units to place in the cell
        ''' display text.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal pm As cPropertyManager, _
                       ByVal Source As cCoreInputOutputBase, _
                       ByVal VarName As eVarNameFlags, _
                       ByVal SourceSec As cCoreInputOutputBase, _
                       ByVal strUnitMask As String, _
                       ByVal aUnitTypes() As cStyleGuide.eUnitType)
            Me.New(pm.GetProperty(Source, VarName, SourceSec), strUnitMask, aUnitTypes)
        End Sub

        Public Overrides Property Style() As Style.cStyleGuide.eStyleFlags
            Get
                Return MyBase.Style Or cStyleGuide.eStyleFlags.Names
            End Get
            Set(ByVal value As Style.cStyleGuide.eStyleFlags)
                MyBase.Style = value
            End Set
        End Property

#End Region ' Construction 

    End Class

End Namespace
