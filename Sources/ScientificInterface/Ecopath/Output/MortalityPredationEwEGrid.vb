#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Properties
Imports SharedResources = ScientificInterfaceShared.My.Resources

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

            ''' <summary>PB value to monitor.</summary>
            Private m_propPB As cSingleProperty = Nothing

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Constructor.
            ''' </summary>
            ''' <param name="pm">Property manager to obtain data from.</param>
            ''' <param name="source">The source providing the property value.</param>
            ''' <param name="varname">Source variable name.</param>
            ''' <param name="sourceSec">Secundary index.</param>
            ''' -----------------------------------------------------------------------
            Public Sub New(ByVal pm As cPropertyManager, _
                            ByVal source As cCoreInputOutputBase, _
                            ByVal varname As eVarNameFlags, _
                            Optional ByVal sourceSec As cCoreInputOutputBase = Nothing)
                MyBase.New(pm, source, varname, sourceSec)

                Me.PB = DirectCast(pm.GetProperty(source, eVarNameFlags.PBOutput, sourceSec), cSingleProperty)
            End Sub

            Protected Overrides Sub Dispose(ByVal disposing As Boolean)
                Me.PB = Nothing
                MyBase.Dispose(disposing)
            End Sub

            Private Property PB() As cSingleProperty
                Get
                    Return Me.m_propPB
                End Get
                Set(ByVal value As cSingleProperty)

                    If (Me.m_propPB IsNot Nothing) Then
                        RemoveHandler Me.m_propPB.PropertyChanged, AddressOf OnPBChanged
                    End If

                    Me.m_propPB = value

                    If (Me.m_propPB IsNot Nothing) Then
                        AddHandler Me.m_propPB.PropertyChanged, AddressOf OnPBChanged
                        Me.UpdateStyle()
                    End If

                End Set
            End Property

            Private Sub UpdateStyle()

                Dim prop As cProperty = Me.GetProperty()
                Dim style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.Null
                Dim sPB As Single = 0
                Dim sPmort As Single = 0

                If (prop IsNot Nothing) And (Me.m_propPB IsNot Nothing) Then
                    style = prop.GetStyle()
                    sPB = CSng(Me.m_propPB.GetValue())
                    sPmort = CSng(prop.GetValue())
                End If

                If (sPmort > sPB) Then
                    style = style Or cStyleGuide.eStyleFlags.Checked
                End If
                Me.Style = style

            End Sub

            Protected Overridable Sub OnPBChanged(ByVal prop As cProperty, ByVal changeFlags As cProperty.eChangeFlags)
                Me.UpdateStyle()
                Me.Invalidate()
            End Sub

            Protected Overrides Sub OnPropertyChanged(ByVal prop As cProperty, ByVal changeFlags As cProperty.eChangeFlags)
                Me.UpdateStyle()
                MyBase.OnPropertyChanged(prop, changeFlags)
            End Sub

        End Class

#End Region ' Helper classes

        Public Sub New()
            MyBase.new()
            Me.FixedColumnWidths = False
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Dim source As cCoreGroupBase = Nothing
            Dim groupIndex As Integer

            Me.Redim(core.nLivingGroups + 1, 2)

            Dim rowCnt As Integer = Me.RowsCount

            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(SharedResources.HEADER_PREYPREDATOR)

            Dim columnIndex As Integer = 2

            For groupIndex = 1 To core.nLivingGroups
                ' Column displays mixed consumer/producer groups ( PP < 1)
                source = core.EcoPathGroupOutputs(groupIndex)
                Me(groupIndex, 0) = New EwERowHeaderCell(groupIndex)
                Me(groupIndex, 1) = New EwERowHeaderCell(source.Name)

                If source.PP < 1 Then
                    Me.Columns.Insert(columnIndex)
                    Me(0, columnIndex) = New PropertyColumnHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
                    columnIndex = columnIndex + 1
                End If
            Next

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

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
                        cell = New MortalityGridCell(Me.PropertyManager, source, eVarNameFlags.PredMort, sourceSec)
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

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
