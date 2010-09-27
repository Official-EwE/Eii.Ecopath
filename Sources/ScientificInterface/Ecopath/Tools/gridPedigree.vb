#Region " Imports "

Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports SourceGrid2

#End Region 'Imports

Namespace Ecopath.Tools

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Grid for displaying Pedigree assignments.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class gridPedigree
        Inherits EwEGrid

#Region " Helper classes "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Visualizer for rendering pedigree cells in the lovely
        ''' <see cref="gridPedigree">pedigree grid</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Class cPedigreeCellVisualizer
            Inherits EwECellVisualizerBase

            Private m_psg As cPedigreeStyleGuide = Nothing

            Public Sub New(ByVal psg As cPedigreeStyleGuide)
                Me.m_psg = psg
            End Sub

            ''' <summary>
            ''' Helper method, returns a pedigree level for a given cell.
            ''' </summary>
            ''' <param name="cell">The cell to obtain pedigree info for.</param>
            ''' <param name="pos">The position to obtain pedigree info for.</param>
            ''' <returns>A <see cref="cPedigreeLevel">pedigree level</see>, or
            ''' Nothing if something went wrong.</returns>
            Private Function GetLevel(ByVal cell As SourceGrid2.Cells.ICellVirtual, _
                    ByVal pos As SourceGrid2.Position) As cPedigreeLevel

                Try

                    ' Sanity checks
                    If (cell Is Nothing) Then Return Nothing

                    Dim value As Object = cell.GetValue(pos)
                    ' Need an integer value representing a level index
                    If Not (TypeOf value Is Integer) Then Return Nothing

                    Dim iLevel As Integer = CInt(value)
                    ' Need a one-based index
                    If (iLevel = 0) Then Return Nothing

                    Dim clr As Color = Nothing
                    Dim core As cCore = Me.Core(cell)
                    If (core Is Nothing) Then Return Nothing

                    ' Ok, this is downright nasty code. The visualizer assumes that the
                    ' columns represent pedigree variable indices starting at column 2.
                    Dim var As eVarNameFlags = core.PedigreeVariable(pos.Column - 1)

                    ' Get manager
                    Dim man As cPedigreeManager = core.GetPedigreeManager(var)
                    If (man Is Nothing) Then Return Nothing

                    Return man.Level(iLevel)

                Catch ex As Exception
                    ' Whoah
                End Try

                Return Nothing

            End Function

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Overidden to draw pedigree cell content background bits.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Protected Overrides Sub DrawCell_Background( _
                    ByVal cell As SourceGrid2.Cells.ICellVirtual, _
                    ByVal pos As SourceGrid2.Position, _
                    ByVal e As System.Windows.Forms.PaintEventArgs, _
                    ByVal rc As System.Drawing.Rectangle, _
                    ByVal status As SourceGrid2.DrawCellStatus)

                MyBase.DrawCell_Background(cell, pos, e, rc, status)

                ' Do not render details on a readonly cell
                If (TypeOf cell Is EwECellBase) Then
                    ' #Yes: obtain cell style
                    If (DirectCast(cell, EwECellBase).Style And cStyleGuide.eStyleFlags.NotEditable) > 0 Then
                        Return
                    End If
                End If

                Dim level As cPedigreeLevel = Me.GetLevel(cell, pos)
                If (level Is Nothing) Then Return

                Using br As New SolidBrush(Me.m_psg.BackgroundColor(Me.BackColor, level))
                    e.Graphics.FillRectangle(br, New Rectangle(rc.Left + 3, rc.Top + 3, rc.Width - 6, rc.Height - 6))
                End Using

            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Overidden to draw pedigree cell content text.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Protected Overrides Sub DrawCell_ImageAndText( _
                    ByVal cell As SourceGrid2.Cells.ICellVirtual, _
                    ByVal pos As SourceGrid2.Position, _
                    ByVal e As System.Windows.Forms.PaintEventArgs, _
                    ByVal rc As System.Drawing.Rectangle, _
                    ByVal status As SourceGrid2.DrawCellStatus)

                Dim level As cPedigreeLevel = Me.GetLevel(cell, pos)
                If (level Is Nothing) Then Return

                Dim clrFore As Color = Me.m_psg.ForegroundColor(Me.ForeColor, level)
                Dim strText As String = Me.m_psg.DisplayText(level)
                Dim fmt As StringFormat = Me.StringFormat

                fmt.Alignment = StringAlignment.Center
                fmt.LineAlignment = StringAlignment.Center

                ' Render Image and Text in determined fore colour and text
                Utility.PaintImageAndText(e.Graphics, rc, _
                    Me.Image, Me.ImageAlignment, Me.ImageStretch, _
                    strText, fmt, _
                    Me.AlignTextToImage, Me.Border, _
                    clrFore, Me.GetCellFont())

            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Borrow core reference from parent cell, if possible.
            ''' </summary>
            ''' <param name="cell">Cell to borrow core from.</param>
            ''' -------------------------------------------------------------------
            Protected ReadOnly Property Core(ByVal cell As SourceGrid2.Cells.ICellVirtual) As cCore
                Get
                    If (TypeOf cell Is IUIElement) Then
                        Dim uic As cUIContext = DirectCast(cell, IUIElement).UIContext
                        If (uic IsNot Nothing) Then
                            Return uic.Core
                        End If
                    End If
                    Return Nothing
                End Get
            End Property

        End Class

#End Region ' Helper classes

#Region " Private vars "

        ' 
        Private m_psg As cPedigreeStyleGuide = Nothing
        Private m_viz As cPedigreeCellVisualizer = Nothing

#End Region ' Private vars

#Region " Construction "

        Public Sub New()
            MyBase.New()
            Me.FixedColumnWidths = False
        End Sub

#End Region ' Construction

#Region " Grid configuration "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="cPedigreeStyleGuide">pedigree style guide</see>
        ''' to use for rendering cells.
        ''' </summary>
        ''' <remarks>
        ''' This monster has to be provided by the master UI. Only once occurrance
        ''' is used to trigger display changes throughout the pedigree interface.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Property Vizualizer() As cPedigreeStyleGuide
            Get
                Return Me.m_psg
            End Get
            Set(ByVal value As cPedigreeStyleGuide)
                If (Me.m_psg IsNot Nothing) Then
                    RemoveHandler Me.m_psg.OnRenderStyleChanged, AddressOf OnRenderStyleChanged
                End If

                Me.m_psg = value

                If (Me.m_psg IsNot Nothing) Then
                    AddHandler Me.m_psg.OnRenderStyleChanged, AddressOf OnRenderStyleChanged
                    Me.m_viz = New cPedigreeCellVisualizer(Me.m_psg)
                    Me.RefreshContent()
                End If
            End Set
        End Property

#End Region ' Grid configuration

#Region " Grid overrides "

        Protected Overrides Sub InitLayout()
            If (Me.m_psg Is Nothing) Then Return
            MyBase.InitLayout()
        End Sub

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Dim group As cCoreGroupBase = Nothing

            Me.Redim(Me.Core.nGroups + 1, Me.Core.nPedigreeVariables + 2)

            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)

            For iVariable As Integer = 1 To Me.Core.nPedigreeVariables
                Me(0, iVariable + 1) = New EwEColumnHeaderCell(Me.Core.PedigreeVariable(iVariable).ToString)
            Next iVariable

            For iGroup As Integer = 1 To Core.nGroups
                group = Me.Core.EcoPathGroupInputs(iGroup)
                Me(iGroup, 0) = New PropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Index)
                Me(iGroup, 1) = New PropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Name)
            Next iGroup

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim group As cCoreGroupBase = Nothing
            Dim man As cPedigreeManager = Nothing
            Dim cell As EwECellBase = Nothing
            Dim style As cStyleGuide.eStyleFlags = (cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.ValueComputed)

            For iGroup As Integer = 1 To Core.nGroups
                ' Get group
                group = Me.Core.EcoPathGroupInputs(iGroup)
                For iVariable As Integer = 1 To Me.Core.nPedigreeVariables
                    ' Get pedigree
                    man = Me.Core.GetPedigreeManager(Me.Core.PedigreeVariable(iVariable))
                    cell = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.Pedigree, man)
                    cell.SuppressZero = True
                    cell.Behaviors.Add(Me.EwEEditHandler)
                    cell.VisualModel = Me.m_viz
                    cell.EditableMode = EditableMode.None
                    Me(iGroup, 1 + iVariable) = cell
                Next
            Next
        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

        Protected Sub OnRenderStyleChanged(ByVal viz As cPedigreeStyleGuide)
            Me.FillData()
        End Sub

#End Region ' Grid overrides

    End Class

End Namespace

