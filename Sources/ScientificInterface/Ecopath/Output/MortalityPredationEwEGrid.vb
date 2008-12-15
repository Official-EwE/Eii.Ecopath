'==============================================================================
'
' $Log: MortalityPredationEwEGrid.vb,v $
' Revision 1.2  2008/12/15 15:55:37  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:33  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.19  2008/08/02 03:04:11  jeroens
' Renamed resources
'
' Revision 1.18  2008/07/29 13:06:43  jeroens
' Propery renamed 'IsStatic' method
'
' Revision 1.17  2008/06/02 00:01:27  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.16  2008/05/29 22:22:40  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.15  2007/10/10 02:59:12  jeroens
' * Updated to new EwEGrid MessageSource interface
'
' Revision 1.14  2007/07/03 07:08:45  jeroens
' * Fixed member naming inconsistencies
'
' Revision 1.13  2007/06/29 04:21:56  jeroens
' * Revamped clouring logic for values > P/B
'
' Revision 1.12  2007/06/22 02:57:58  jeroens
' * Selection state of cell now considered when drawing background
'
' Revision 1.11  2007/06/21 23:57:20  fgao
' Add grid selection, autosize..etc features..
'
' Revision 1.10  2007/04/29 03:45:10  jeroens
' * Connected to EwEGridRefresh
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core

#End Region

Namespace Ecopath.Output

    <CLSCompliant(False)> _
    Public Class MortalityPredationEwEGrid
        : Inherits EwEGrid

#Region " Helper classes "

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' A <see cref="cProperty">cProperty</see>-driven cell that reflects the 
        ''' property value by varying the colour intensity of the cell background.
        ''' </summary>
        ''' <remarks>This is a Hack'n'slash solution; no value range testing is 
        ''' performed when calculating the background colour.</remarks>
        ''' ---------------------------------------------------------------------------
        <CLSCompliant(False)> _
        Public Class MortalityGridCell
            : Inherits PropertyCell

            Private WithEvents m_propPB As cProperty = Nothing

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Constructor.
            ''' </summary>
            ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> data source.</param>
            ''' <param name="VarName">The <see cref="eVarNameFlags">VarName flag</see> that defines which aspect of the Source to acces.</param>
            ''' <param name="SourceSec">An optional secundary index in the VarName, or Nothing when irrelevant.</param>
            ''' -----------------------------------------------------------------------
            Public Sub New(ByVal Source As cCoreInputOutputBase, ByVal VarName As eVarNameFlags, _
                    Optional ByVal SourceSec As cCoreInputOutputBase = Nothing)
                MyBase.New(Source, VarName, SourceSec)
                ConnectToPB()
            End Sub

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Constructor.
            ''' </summary>
            ''' <param name="prop">The property to assign to the cell.</param>
            ''' -----------------------------------------------------------------------
            Public Sub New(ByVal prop As cProperty)
                ' Call baseclass constructor
                MyBase.New(prop)
                ConnectToPB()
            End Sub

            Private Sub ConnectToPB()
                Dim pm As cPropertyManager = cPropertyManager.GetInstance()
                Dim prop As cProperty = Me.GetProperty()
                Me.m_propPB = pm.GetProperty(prop.Source, eVarNameFlags.PBOutput, prop.SourceSec)
                Me.UpdateStyle()
            End Sub

            Private Sub m_propPB_PropertyChanged(ByVal prop As cProperty, ByVal changeFlags As cProperty.eChangeFlags) Handles m_propPB.PropertyChanged
                Me.UpdateStyle()
            End Sub

            Private Sub UpdateStyle()
                Dim style As StyleGuide.eStyleFlags = Me.GetProperty().GetStyle()
                Dim sPB As Single = CSng(m_propPB.GetValue())
                Dim sPmort As Single = CSng(Me.GetProperty().GetValue())

                If (sPmort > sPB) Then
                    style = style Or StyleGuide.eStyleFlags.Checked
                End If
                Me.Style = style
            End Sub

        End Class

#End Region ' Helper classes

        Public Sub New()
            MyBase.new()
            Me.FixedColumnWidths = False
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreGroupBase = Nothing
            Dim groupIndex As Integer

            Me.Redim(core.nLivingGroups + 1, 2)

            Dim rowCnt As Integer = Me.RowsCount

            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_PREYPREDATOR)

            Dim columnIndex As Integer = 2

            For groupIndex = 1 To core.nLivingGroups
                ' Column displays mixed consumer/producer groups ( PP < 1)
                source = core.EcoPathGroupOutputs(groupIndex)
                Me(groupIndex, 0) = New EwERowHeaderCell(groupIndex)
                Me(groupIndex, 1) = New EwERowHeaderCell(source.Name)

                If source.PP < 1 Then
                    Me.Columns.Insert(columnIndex)
                    Me(0, columnIndex) = New PropertyColumnHeaderCell(source, eVarNameFlags.Index)
                    columnIndex = columnIndex + 1
                End If
            Next

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreGroupBase = Nothing
            Dim sourceSec As cCoreGroupBase = Nothing
            Dim cell As PropertyCell = Nothing

            For rowIndex As Integer = 1 To core.nLivingGroups
                source = core.EcoPathGroupOutputs(rowIndex)
                Dim columnIndex As Integer = 2
                For groupIndex As Integer = 1 To core.nLivingGroups
                    sourceSec = core.EcoPathGroupOutputs(groupIndex)
                    If sourceSec.PP < 1 Then
                        ' Create cell
                        cell = New MortalityGridCell(source, eVarNameFlags.PredMort, sourceSec)
                        ' Value cells suppress zeroes to increase legibility of the grid
                        Cell.SuppressZero(-1) = True
                        ' Activate the cell
                        Me(rowIndex, columnIndex) = cell
                        ' Next
                        columnIndex = columnIndex + 1
                    End If
                Next
            Next
        End Sub

        Public Overrides ReadOnly Property MessageSource() As EwECore.eMessageSource
            Get
                Return eMessageSource.EcoPath
            End Get
        End Property

    End Class

End Namespace
