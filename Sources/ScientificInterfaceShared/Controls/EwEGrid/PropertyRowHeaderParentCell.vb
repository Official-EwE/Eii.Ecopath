#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Properties
Imports ScientificInterfaceShared.Style
Imports SourceGrid2

#End Region ' Imports

Namespace Controls.EwEGrid

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' PropertyRowHeaderParentCell implements a PropertyRowHeaderCell rendered 
    ''' as a parent cell in a hierarchy.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class PropertyRowHeaderParentCell
        : Inherits PropertyRowHeaderCell

        ''' <summary>One visualizer for all cells</summary>
        Private Shared g_visualizer As New cVisualizerEwEParentRowHeader()

#Region " Construction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
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
        ''' <param name="prop">cProperty to attach to the cell</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal prop As cProperty)
            ' Call baseclass constructor
            MyBase.New(prop)
            ' Set shared visualizer
            Me.VisualModel = g_visualizer
            Me.DataModel.EditableMode = SourceGrid2.EditableMode.None
        End Sub

#End Region ' Construction 

    End Class

End Namespace
