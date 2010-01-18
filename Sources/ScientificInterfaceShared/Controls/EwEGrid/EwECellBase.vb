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

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Base class for EwE SourceGrid2 Grid cells, implementing EwE GUI 
    ''' feedback.
    ''' </summary>
    ''' -------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public MustInherit Class EwECellBase
        Inherits Cell
        Implements IDisposable

        ' JS 26Jan08: experimental new behaviour for EwECells: trap enter key
        Class CatchEnterPressBehaviour
            Inherits BehaviorModels.Common

            Public Overrides Sub OnKeyUp(ByVal e As SourceGrid2.PositionKeyEventArgs)
                If (e.KeyEventArgs.KeyCode = Keys.Enter) Then
                    Dim p As New Position((e.Position.Row + 1) Mod e.Grid.RowsCount, e.Position.Column)
                    e.Grid.SetFocusCell(p)
                End If
            End Sub

        End Class

#Region " Construction and destruction"

        ''' <summary>Default visualizer for EwECells</summary>
        Private Shared g_visualizer As New EwECellVisualizer()

        ''' <summary>StyleGuide instance for subscribing to events</summary>
        Protected m_sg As cStyleGuide = cStyleGuide.GetInstance()
        ''' <summary>Behaviour model to catch [ENTER] key presses.</summary>
        Private m_bmCatchEnter As BehaviorModels.IBehaviorModel = Nothing
        ''' <summary>Behaviour model to catch cell resize events.</summary>
        Private m_bmResize As BehaviorModels.IBehaviorModel = Nothing

        Public Sub New(ByVal objVal As Object, ByRef t As Type)
            MyBase.New(Nothing, t)
            ' Set shared visualizer
            Me.VisualModel = g_visualizer
            ' Configure data model
            Me.DataModel.AllowNull = True

            ' Catch ENTER presses
            Me.m_bmCatchEnter = New CatchEnterPressBehaviour()
            Me.Behaviors.Add(Me.m_bmCatchEnter)

            ' Only resize width, not height of cells
            Me.m_bmResize = New SourceGrid2.BehaviorModels.Resize(CellResizeMode.Width)
            Me.Behaviors.Add(Me.m_bmResize)

            AddHandler Me.m_sg.StyleGuideChanged, AddressOf Me.OnStyleGuideChanged
        End Sub

        Private m_bIsThrashed As Boolean = False

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal bThrashing As Boolean)
            If Not Me.m_bIsThrashed Then
                If bThrashing Then

                    ' Release style guide event handler
                    RemoveHandler Me.m_sg.StyleGuideChanged, AddressOf Me.OnStyleGuideChanged
                    Me.m_sg = Nothing

                    ' Remove all bahaviour models
                    Me.Behaviors.Remove(m_bmCatchEnter)
                    Me.m_bmCatchEnter = Nothing

                    Me.Behaviors.Remove(m_bmResize)
                    Me.m_bmResize = Nothing

                End If
            End If
            Me.m_bIsThrashed = True
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

#End Region ' Construction

#Region " Data (value, style, remarks) "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Custom cell style
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private m_style As cStyleGuide.eStyleFlags = 0

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Allows to set a custom <see cref="cStyleGuide.eStyleFlags">style</see>,
        ''' triggering EwE colour feedback and EwE cell edit behaviour.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Property Style() As cStyleGuide.eStyleFlags

            Get
                Return Me.m_style
            End Get

            Set(ByVal s As cStyleGuide.eStyleFlags)
                Me.m_style = s
                If ((s And cStyleGuide.eStyleFlags.NotEditable) = 0) Then
                    Me.DataModel.EnableEdit = True
                    Me.DataModel.EditableMode = SourceGrid2.EditableMode.Default
                Else
                    Me.DataModel.EnableEdit = False
                    Me.DataModel.EditableMode = SourceGrid2.EditableMode.None
                End If
            End Set

        End Property

        Public Overrides Sub OnEditStarting(ByVal e As SourceGrid2.PositionCancelEventArgs)
            ' JS, 26aug08: Bug fix 502
            ' Safety catch, this method should be obsolete but *apparently* a double-click on
            ' disabled cells (EndableEdit and EditableMode locked down) still
            ' results into EditStarting!
            If ((Me.Style And cStyleGuide.eStyleFlags.NotEditable) = cStyleGuide.eStyleFlags.NotEditable) Then
                e.Cancel = True
            End If
            MyBase.OnEditStarting(e)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>If true, the cell will not show numerical '0' values.</summary>
        ''' -------------------------------------------------------------------
        Private m_bSuppressZero As Boolean = False

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Zero value to suppress
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private m_sZeroValue As Single = 0.0

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' When set to True, cells will not show numerical '0' values
        ''' </summary>
        ''' <param name="sZeroValue">A custom zero value, if applicable.</param>
        ''' -------------------------------------------------------------------
        Public Property SuppressZero(Optional ByVal sZeroValue As Single = 0.0) As Boolean
            Get
                Return Me.m_bSuppressZero
            End Get
            Set(ByVal bSuppress As Boolean)
                If (bSuppress <> Me.m_bSuppressZero) Then
                    Me.m_bSuppressZero = bSuppress
                    Me.m_sZeroValue = sZeroValue
                    Me.Invalidate()
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Number of decimal digits to display
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private m_iNumDigits As Integer = -1

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the number of decimal digits to display when formatting
        ''' numeric values. Set this value to a negative number to use the 
        ''' system-wide <see cref="cStyleGuide.NumDigits">NumDigits</see> setting.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property NumDigits() As Integer
            Get
                Return Me.m_iNumDigits
            End Get
            Set(ByVal iNumDigits As Integer)
                If (iNumDigits <> Me.m_iNumDigits) Then
                    Me.m_iNumDigits = iNumDigits
                    Me.Invalidate()
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the text to display in the cell.
        ''' </summary>
        ''' <returns>The formatted value of the cell.</returns>
        ''' <remarks>
        ''' Real values will be formatted according
        ''' to the <see cref="cStyleGuide.NumDigits">number of digits</see>
        ''' setting specified in the EwE <see cref="cStyleGuide">StyleGuide</see>,
        ''' or via the local <see cref="NumDigits">NumDigits</see> override
        ''' if provided.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property DisplayText() As String
            Get

                Dim objValue As Object = Me.Value
                Dim tValue As Type = Me.DataModel.ValueType

                If ((Me.Style And cStyleGuide.eStyleFlags.Null) > 0) Then
                    Return ""
                End If

                ' Is this a single?
                If (tValue Is GetType(Single)) Then
                    Dim sValue As Single = 0
                    Try
                        ' #Yes: apply format
                        sValue = CSng(Val(objValue))
                    Catch ex As Exception

                    End Try
                    ' Must suppress true zero?
                    If (Me.SuppressZero And (sValue = Me.m_sZeroValue)) Then
                        ' #Yes: return empty cell
                        Return ""
                    End If
                    Return Me.m_sg.FormatNumber(sValue, Me.Style, Me.m_iNumDigits)
                End If

                ' Is this a double?
                If (tValue Is GetType(Double)) Then
                    ' #Yes: apply format
                    Dim dValue As Double = 0
                    Try
                        dValue = CDbl(Val(objValue))
                    Catch ex As Exception

                    End Try
                    ' Must suppress true zero?
                    If (Me.SuppressZero And (dValue = CDbl(Me.m_sZeroValue))) Then
                        ' #Yes: return empty cell
                        Return ""
                    End If
                    Return Me.m_sg.FormatNumber(dValue, Me.Style, Me.m_iNumDigits)
                End If

                ' Is this an integer?
                If (tValue Is GetType(Integer)) Then
                    ' #Yes: apply format
                    Dim iValue As Integer = 0
                    Try
                        iValue = CInt(Val(objValue))
                    Catch ex As Exception

                    End Try
                    ' Must suppress true zero?
                    If (Me.SuppressZero And (iValue = CInt(Me.m_sZeroValue))) Then
                        ' #Yes: return empty cell
                        Return ""
                    End If
                    Return CStr(iValue)
                End If

                ' Return value as-is
                Return CStr(objValue)

            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Configure a cell by examining <see cref="cVariableMetaData">variable meta data</see>.
        ''' </summary>
        ''' <param name="md">The <see cref="cVariableMetaData">variable meta data</see> to examine.</param>
        ''' -------------------------------------------------------------------
        Public Sub ConfigureCell(ByVal md As cVariableMetaData)

            ' Sanity check
            If md Is Nothing Then Return
            ' Set default val
            Me.DataModel.DefaultValue = md.NullValue

            ' Do not set min and max; the default may be out of range and this will massively confuse the grid engine
            'Me.DataModel.MinimumValue = md.Min
            'Me.DataModel.MaximumValue = md.Max

        End Sub

#End Region ' Data (value, style, remarks)

#Region " Updates (StyleGuide)"

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' StyleGuide change event handler; makes sure cells are redrawn
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overridable Sub OnStyleGuideChanged(ByVal changeType As cStyleGuide.eChangeType)
            Me.Invalidate()
        End Sub

#End Region ' Updated (StyleGuide)

    End Class

End Namespace