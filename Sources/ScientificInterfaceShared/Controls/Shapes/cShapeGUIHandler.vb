'==============================================================================
'
' $Log: cShapeGUIHandler.vb,v $
' Revision 1.10  2009/03/21 00:30:32  jeroens
' Fixed unclear parameter names
'
' Revision 1.9  2009/03/20 17:53:59  jeroens
' Multiple selection
'
' Revision 1.8  2009/03/11 00:31:54  jeroens
' Shapes update on reset to make sure changes are committed to the core
'
' Revision 1.7  2009/03/04 06:31:55  jeroens
' ResetAll command properly enabled
'
' Revision 1.6  2009/03/04 06:29:34  jeroens
' ResetAll uses generic shape manager
'
' Revision 1.5  2009/03/02 20:08:23  jeroens
' Implemented FF reset all
'
' Revision 1.4  2009/03/02 01:45:20  jeroens
' Removed ecopath mort rate indicator from fishing rate shape manager
'
' Revision 1.3  2009/02/12 15:33:25  jeroens
' Fishing rates showing Y mark label
'
' Revision 1.2  2009/01/16 18:30:35  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/12/15 15:37:16  jeroens
' Moved from ScInt
'
'==============================================================================

#Region " Imports "

Imports EwECore
Imports EwEUtils.Commands
Imports ScientificInterfaceShared.Definitions
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Controls

#Region " Base class "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Common controller for all GUI components that implement a 
    ''' <see cref="cShapeData">shape</see> selection and/or modification interface.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(True)> _
    Public MustInherit Class cShapeGUIHandler

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Predefined interactions on shapes that can be supported by handlers.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Enum eShapeCommandTypes As Integer
            ''' <summary>Add a shape.</summary>
            Add
            ''' <summary>Change the contour of a shape to a common outline.</summary>
            ChangeShape
            ''' <summary>Set all shapes of a given type to default values.</summary>
            ResetAll
            ''' <summary>Duplicte a shape.</summary>
            Duplicate
            ''' <summary>Import shape data.</summary>
            Import
            ''' <summary>Load shape data.</summary>
            ''' <remarks>JS 22nov07: Is this used at all?</remarks>
            Load
            ''' <summary>Modify the values in a shape.</summary>
            Modify
            ''' <summary>Set shape display options.</summary>
            DisplayOptions
            ''' <summary>Remove a shape.</summary>
            Remove
            ''' <summary>Set one shape to specific values.</summary>
            Reset
            ''' <summary>Save shape to an image.</summary>
            SaveAsImage
            ''' <summary>Set the seasonal/long-term state of a shape.</summary>
            Seasonal
            ''' <summary>Set the weight of a single time series.</summary>
            SetWeight
            ''' <summary>Set a shape to a given value.</summary>
            SetValue
            ''' <summary>Set a shape to 0.</summary>
            SetToZero
            ''' <summary>Weight all time series.</summary>
            Weight
        End Enum

#Region " Private variables "

        ''' <summary>The single instance of the core.</summary>
        Protected m_core As cCore
        ''' <summary><see cref="ucShapeToolbox">Shape toolbox control </see> to handle.</summary>
        Private m_shapeToolBox As ucShapeToolbox = Nothing
        ''' <summary><see cref="ucShapeToolboxToolbar">Shape toolbox toolbar control </see> to handle.</summary>
        Private m_shapeToolBoxToolbar As ucShapeToolboxToolbar = Nothing
        ''' <summary><see cref="ucSketchPad">Shape sketch pad control </see> to handle.</summary>
        Private m_sketchPad As ucSketchPad = Nothing
        ''' <summary><see cref="ucSketchPadToolbar">Shape sketch pad toolbar control </see> to handle.</summary>
        Private m_sketchPadToolbar As ucSketchPadToolbar = Nothing
        ''' <summary>The color to use for rendering <see cref="cShapeData">shapes</see>.</summary>
        Private m_color As Color = Nothing
        ''' <summary>Selected <see cref="cShapeData">shapes</see>.</summary>
        Private m_ashapeSelected() As cShapeData = Nothing

#End Region ' Private variables

#Region " Constructor and destructor "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this handler.
        ''' </summary>
        ''' <param name="core">Core to connect to.</param>
        ''' <param name="stb"><see cref="ucShapeToolbox">Shape toolbox control </see> to handle, if any.</param>
        ''' <param name="stbtb"><see cref="ucShapeToolboxToolbar">Shape toolbox toolbar control </see> to handle, if any.</param>
        ''' <param name="sp"><see cref="ucSketchPad">Shape sketch pad control </see> to handle, if any.</param>
        ''' <param name="sptb"><see cref="ucSketchPadToolbar">Shape sketch pad toolbar control </see> to handle, if any.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal core As cCore, _
                ByVal stb As ucShapeToolbox, ByVal stbtb As ucShapeToolboxToolbar, _
                ByVal sp As ucSketchPad, ByVal sptb As ucSketchPadToolbar)

            Me.m_core = core
            Me.ShapeToolBox = stb
            Me.ShapeToolBoxToolbar = stbtb
            Me.SketchPad = sp
            Me.SketchPadToolbar = sptb

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Destructor; properly detaches from handled controls.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub Finalize()
            Me.ShapeToolBox = Nothing
            Me.ShapeToolBoxToolbar = Nothing
            Me.SketchPad = Nothing
            Me.SketchPadToolbar = Nothing
            MyBase.Finalize()
        End Sub

#End Region ' Constructor and destructor

#Region " Obligatory overrides "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Public interface to ask whether a given command is supported by this handler.
        ''' </summary>
        ''' <param name="cmd">The <see cref="eShapeCommandTypes">command</see> to test.</param>
        ''' <returns>True if supported.</returns>
        ''' -------------------------------------------------------------------
        Public MustOverride Function SupportCommand(ByVal cmd As eShapeCommandTypes) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Public interface to query the enables state of a given command by this handler.
        ''' </summary>
        ''' <param name="cmd">The <see cref="eShapeCommandTypes">command</see> to test.</param>
        ''' <returns>True if enabled.</returns>
        ''' -------------------------------------------------------------------
        Public MustOverride Function EnableCommand(ByVal cmd As eShapeCommandTypes) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Public interface to execute a given command by this handler.
        ''' </summary>
        ''' <param name="cmd">The <see cref="eShapeCommandTypes">command</see> to test.</param>
        ''' <param name="shape">The <see cref="EwECore.cShapeData">shape</see> to apply the command to.</param>
        ''' <param name="data">Optional data to accompany the command.</param>
        ''' -------------------------------------------------------------------
        Public MustOverride Sub ExecuteCommand(ByVal cmd As eShapeCommandTypes, _
                Optional ByVal shape As cShapeData() = Nothing, _
                Optional ByVal data As Object = Nothing)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Override this to make controls respond to any kind of change in the
        ''' shape data managed by this handler and its buddy GUI components.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public MustOverride Sub Refresh()

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Override this to respond to a <see cref="ucShapeToolbox.OnSelectionChanged">shape selection event</see> derived from the controlled <see cref="m_shapeToolBox">shape toolbox</see>.
        ''' </summary>
        ''' <param name="shape">The newly selected shape, or Nothing when no 
        ''' shape is selected.</param>
        ''' -------------------------------------------------------------------
        Public MustOverride Sub OnShapeSelected(ByVal shape() As cShapeData)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Override this to respond to a <see cref="ucSketchPad.ShapeChanged">shape changed event</see> derived from the controlled <see cref="m_sketchPad">shape sketchpad</see>.
        ''' </summary>
        ''' <param name="shape">The shape that changed.</param>
        ''' -------------------------------------------------------------------
        Public MustOverride Sub OnShapeChanged(ByVal shape As cShapeData)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Override this to respond to a <see cref="ucSketchPad.ShapeFinalized">shape finalized event</see> derived from the controlled <see cref="m_sketchPad">shape sketchpad</see>.
        ''' </summary>
        ''' <param name="shape"></param>
        ''' -------------------------------------------------------------------
        Public MustOverride Sub OnShapeFinalized(ByVal shape As cShapeData, ByVal sketchpad As ucSketchPad)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Override this to specify the color that should be used to render shapes.
        ''' </summary>
        ''' <returns>The color to use to render shapes.</returns>
        ''' -------------------------------------------------------------------
        Protected MustOverride Function Color() As Color

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Override this to specify the <see cref="eSketchDrawModeTypes">Sketch draw mode</see>
        ''' that should be used to render shapes.
        ''' </summary>
        ''' <returns>Well, what do YOU think?</returns>
        ''' -------------------------------------------------------------------
        Protected MustOverride Function SketchDrawMode() As eSketchDrawModeTypes

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Override this to specify the lowest Y-scale limit that should be used to render shapes with.
        ''' </summary>
        ''' <returns>The lowest Y-scale limit that should be used to render shapes with.</returns>
        ''' -------------------------------------------------------------------
        Protected MustOverride Function MinYScale() As Single

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the selected <see cref="cShapeData">shape</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Property SelectedShapes() As cShapeData()
            Get
                Return Me.m_ashapeSelected
            End Get
            Set(ByVal value As cShapeData())
                Me.m_ashapeSelected = value

                ' Single selection
                Dim shapeSelected As cShapeData = Nothing
                If (value IsNot Nothing) Then
                    If (value.Length = 1) Then shapeSelected = value(0)
                End If

                If (Me.SketchPad IsNot Nothing) Then Me.SketchPad.Shape = shapeSelected
                If (Me.ShapeToolBox IsNot Nothing) Then Me.ShapeToolBox.Selection = value

                If (Me.SketchPadToolbar IsNot Nothing) Then Me.SketchPadToolbar.Refresh()
                If (Me.ShapeToolBoxToolbar IsNot Nothing) Then Me.ShapeToolBoxToolbar.Refresh()

            End Set
        End Property

        Public Property SelectedShape() As cShapeData
            Get
                If (Me.SelectedShapes IsNot Nothing) Then
                    If (Me.SelectedShapes.Length = 1) Then Return Me.SelectedShapes(0)
                End If
                Return Nothing
            End Get
            Set(ByVal value As cShapeData)
                If value Is Nothing Then
                    Me.SelectedShapes = Nothing
                Else
                    Me.SelectedShapes = New cShapeData() {value}
                End If
            End Set
        End Property

#End Region ' Obligatory overrides

#Region " Tools "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Reset a shape to a particular value.
        ''' </summary>
        ''' <param name="ashapes">The <see cref="cShapeData">shape</see> to affect.</param>
        ''' <param name="sDefaultValue">The value to set.</param>
        ''' -------------------------------------------------------------------
        Protected Overridable Sub ResetShape(ByVal ashapes As cShapeData(), _
                Optional ByVal sDefaultValue As Single = 1.0!)

            Debug.Assert(ashapes IsNot Nothing)

            For Each shape As cShapeData In ashapes
                If shape IsNot Nothing Then
                    shape.LockUpdates()
                    shape.IsSeasonal = False
                    For i As Integer = 0 To shape.XMax ' - 1'jb why the minus one
                        shape.ShapeData(i) = sDefaultValue
                    Next i
                    shape.Update()
                    shape.UnlockUpdates()
                End If
            Next

            Me.SelectedShapes = Me.SelectedShapes
        End Sub

        Protected Overridable Sub SaveAsImage(ByVal shape As cShapeData, ByVal sp As ucSketchPad)

            Dim msg As cMessage = Nothing
            Dim strError As String = ""
            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)

            If sp Is Nothing Then Return

            cmdFS.Invoke(My.Resources.FILEFILTER_IMAGE)

            If cmdFS.Result = Windows.Forms.DialogResult.OK Then

                Dim imgFormat As System.Drawing.Imaging.ImageFormat = System.Drawing.Imaging.ImageFormat.Bmp
                Select Case cmdFS.FilterIndex
                    Case 1
                        imgFormat = System.Drawing.Imaging.ImageFormat.Bmp
                    Case 2
                        imgFormat = System.Drawing.Imaging.ImageFormat.Jpeg
                    Case 3
                        imgFormat = System.Drawing.Imaging.ImageFormat.Gif
                    Case 4
                        imgFormat = System.Drawing.Imaging.ImageFormat.Png
                    Case 5
                        imgFormat = System.Drawing.Imaging.ImageFormat.Tiff
                    Case Else
                        Debug.Assert(False)
                End Select

                ' Let sketchpad save the image
                If sp.SaveAsImage(shape, cmdFS.FileName, imgFormat, strError) Then
                    msg = New cMessage(String.Format(My.Resources.GENERIC_FILESAVE_SUCCES, shape.Name, cmdFS.FileName), _
                            eMessageType.Any, eCoreComponentType.External, eMessageImportance.Information)
                Else
                    msg = New cMessage(String.Format(My.Resources.GENERIC_FILESAVE_FAILURE, shape.Name, cmdFS.FileName, strError), _
                            eMessageType.Any, eCoreComponentType.External, eMessageImportance.Warning)
                End If
                ' Provide feedback on result
                Me.m_core.Messages.SendMessage(msg)

            End If
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the seasonal display flag or a shape.
        ''' </summary>
        ''' <param name="shape">The <see cref="cShapeData">shape</see> to affect.</param>
        ''' <param name="bSeasonal">Flag stating whether the shape should be rendered
        ''' as seasonal (true) or long-term (false)</param>
        ''' <remarks>
        ''' Note that toggling a shape from long-term to seasonal will distribute
        ''' the seasonal pattern across the entire length of the shape.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overridable Sub SetSeasonal(ByVal shape As cShapeData, ByVal bSeasonal As Boolean)

            Debug.Assert(shape IsNot Nothing)

            If (bSeasonal = False) Then
                Me.SelectedShape.IsSeasonal = False
            Else
                If Not Me.SelectedShape.IsSeasonal Then
                    If (MsgBox(My.Resources.SHAPE_TYPE_TO_SEASONAL_MSG, _
                             MsgBoxStyle.YesNo Or MsgBoxStyle.Exclamation, _
                             My.Resources.SHAPE_TYPE_TO_SEASONAL_CAPTION) = MsgBoxResult.Yes) Then
                        Me.SelectedShape.IsSeasonal = True
                    End If
                End If
            End If
            ' Cascade changes properly
            Me.SelectedShapes = Me.m_ashapeSelected
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Default implementation for the <see cref="eShapeCommandTypes.DisplayOptions">Options</see> command.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Sub ShapeOptions()
            Dim dlg As New dlgGraphDisplayOptions(Me.m_sketchPad)
            dlg.ShowDialog()
        End Sub

#End Region ' Tools

#Region " Internal implementation "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="ucSketchPad">Sketch pad control</see> to manage.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Property SketchPad() As ucSketchPad
            Get
                Return Me.m_sketchPad
            End Get
            Set(ByVal value As ucSketchPad)

                If (Me.m_sketchPad IsNot Nothing) Then
                    If (Object.ReferenceEquals(Me.m_sketchPad.Handler, Me)) Then Me.m_sketchPad.Handler = Nothing
                    RemoveHandler Me.m_sketchPad.ShapeChanged, AddressOf OnShapeChanged
                    RemoveHandler Me.m_sketchPad.ShapeFinalized, AddressOf OnShapeFinalized
                End If

                Me.m_sketchPad = value

                If (Me.m_sketchPad IsNot Nothing) Then
                    AddHandler Me.m_sketchPad.ShapeChanged, AddressOf OnShapeChanged
                    AddHandler Me.m_sketchPad.ShapeFinalized, AddressOf OnShapeFinalized
                    Me.m_sketchPad.ShapeColor = Me.Color
                    Me.m_sketchPad.YAxisMinValue = Me.MinYScale
                    Me.m_sketchPad.SketchDrawMode = Me.SketchDrawMode
                    Me.m_sketchPad.Handler = Me
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="ucShapeToolbox">Shape toolbox control</see> to manage.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Property ShapeToolBox() As ucShapeToolbox
            Get
                Return Me.m_shapeToolBox
            End Get
            Set(ByVal value As ucShapeToolbox)

                If (Me.m_shapeToolBox IsNot Nothing) Then
                    Me.m_shapeToolBox.Handler = Nothing
                    RemoveHandler Me.m_shapeToolBox.OnSelectionChanged, AddressOf OnShapeSelected
                End If

                Me.m_shapeToolBox = value

                If (Me.m_shapeToolBox IsNot Nothing) Then
                    AddHandler Me.m_shapeToolBox.OnSelectionChanged, AddressOf OnShapeSelected
                    Me.m_shapeToolBox.Handler = Me
                    Me.m_shapeToolBox.Color = Me.Color
                    Me.m_shapeToolBox.YAxisMinValue = Me.MinYScale
                End If

            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="ucShapeToolboxToolbar">Shape toolbox toolbar control</see> to manage.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Property ShapeToolBoxToolbar() As ucShapeToolboxToolbar
            Get
                Return Me.m_shapeToolBoxToolbar
            End Get
            Set(ByVal value As ucShapeToolboxToolbar)

                If (Me.m_shapeToolBoxToolbar IsNot Nothing) Then
                    Me.m_shapeToolBoxToolbar.Handler = Nothing
                End If

                Me.m_shapeToolBoxToolbar = value

                If (Me.m_shapeToolBoxToolbar IsNot Nothing) Then
                    Me.m_shapeToolBoxToolbar.Handler = Me
                End If

            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="ucSketchPadToolbar">Sketch pad toolbar control</see> to manage.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Property SketchPadToolbar() As ucSketchPadToolbar
            Get
                Return Me.m_sketchPadToolbar
            End Get
            Set(ByVal value As ucSketchPadToolbar)

                If (Me.m_sketchPadToolbar IsNot Nothing) Then
                    Me.m_sketchPadToolbar.Handler = Nothing
                End If

                Me.m_sketchPadToolbar = value

                If (Me.m_sketchPadToolbar IsNot Nothing) Then
                    Me.m_sketchPadToolbar.Handler = Me
                End If

            End Set
        End Property

#End Region

    End Class

#End Region ' Base class

#Region " Time series "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <see cref="cShapeGUIHandler">cShapeGUIHandler implementation</see> for 
    ''' handling <see cref="cTimeSeries">Time Series shapes</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(True)> _
    Public Class cTimeSeriesShapeGUIHandler
        Inherits cShapeGUIHandler

        ''' <summary>Flag to prevent update / response loops.</summary>
        Private m_bInUpdate As Boolean = False
        ''' <summary>The Time Series to distribute.</summary>
        Private m_lShapes As New List(Of cShapeData)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this handler.
        ''' </summary>
        ''' <param name="core">Core to connect to.</param>
        ''' <param name="stb"><see cref="ucShapeToolbox">Shape toolbox control </see> to handle, if any.</param>
        ''' <param name="stbtb"><see cref="ucShapeToolboxToolbar">Shape toolbox toolbar control </see> to handle, if any.</param>
        ''' <param name="sp"><see cref="ucSketchPad">Shape sketch pad control </see> to handle, if any.</param>
        ''' <param name="sptb"><see cref="ucSketchPadToolbar">Shape sketch pad toolbar control </see> to handle, if any.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal core As cCore, _
                ByVal stb As ucShapeToolbox, ByVal stbtb As ucShapeToolboxToolbar, _
                ByVal sp As ucSketchPad, ByVal sptb As ucSketchPadToolbar)

            MyBase.New(core, stb, stbtb, sp, sptb)

            ' Cannot draw onto tim series shapes
            Me.SketchPad.Enabled = False
            ' Add check boxes to the toolbox
            Me.ShapeToolBox.AllowCheckboxes = True

            Me.UpdateShapeList(New cShapeData() {sp.Shape})
        End Sub

#Region " Baseclass overrides "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Public interface to ask whether a given command is supported by this handler.
        ''' Overridden to weed out non-Time Series commands.
        ''' </summary>
        ''' <param name="cmd">The command to test.</param>
        ''' <returns>True if command is supported.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function SupportCommand(ByVal cmd As eShapeCommandTypes) As Boolean

            Select Case cmd
                Case eShapeCommandTypes.Add
                    Return True
                Case eShapeCommandTypes.Weight
                    Return True
                Case eShapeCommandTypes.Duplicate
                    Return False
                Case eShapeCommandTypes.Import
                    Return True
                Case eShapeCommandTypes.Load
                    Return True
                Case eShapeCommandTypes.Modify
                    Return True
                Case eShapeCommandTypes.Remove
                    Return True
                Case eShapeCommandTypes.Seasonal
                    Return False
                Case eShapeCommandTypes.SetWeight
                    Return True
                Case eShapeCommandTypes.SaveAsImage
                    Return True
                Case Else
                    ' Debug.Assert(False, String.Format("Command {0} not supported", cmd))
            End Select
            Return False

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Public interface to query the enables state of a given command by this handler.
        ''' Overridden to enable commands Time Series-style, kachingg!!
        ''' </summary>
        ''' <param name="cmd">The <see cref="eShapeCommandTypes">command</see> to test.</param>
        ''' <returns>True if enabled.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function EnableCommand(ByVal cmd As cShapeGUIHandler.eShapeCommandTypes) As Boolean

            Dim bHasSelection As Boolean = (Me.SelectedShapes IsNot Nothing)
            Dim bHasSingleSelection As Boolean = (Me.SelectedShape IsNot Nothing)

            Select Case cmd

                Case cShapeGUIHandler.eShapeCommandTypes.Import, _
                     eShapeCommandTypes.Load
                    Return True

                Case cShapeGUIHandler.eShapeCommandTypes.Add, _
                     eShapeCommandTypes.Weight
                    Return Me.m_core.HasTimeSeries

                Case cShapeGUIHandler.eShapeCommandTypes.Duplicate, _
                     cShapeGUIHandler.eShapeCommandTypes.Remove
                    Return bHasSelection

                Case eShapeCommandTypes.Modify, _
                     eShapeCommandTypes.SetWeight, _
                     eShapeCommandTypes.SaveAsImage
                    Return bHasSingleSelection

            End Select

            Return False

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Public interface to execute a given command by this handler. 
        ''' Overridden to implement Time Series commands.
        ''' </summary>
        ''' <param name="cmd">The <see cref="eShapeCommandTypes">command</see> to test.</param>
        ''' <param name="ashapes">The <see cref="EwECore.cShapeData">shapes</see> to apply the command to.</param>
        ''' <param name="data">Optional data to accompany the command.</param>
        ''' -------------------------------------------------------------------
        Public Overrides Sub ExecuteCommand(ByVal cmd As eShapeCommandTypes, _
             Optional ByVal ashapes As cShapeData() = Nothing, Optional ByVal data As Object = Nothing)

            If (ashapes Is Nothing) Then ashapes = Me.SelectedShapes

            Select Case cmd
                Case eShapeCommandTypes.Add
                    Me.AddTimeSeries()

                Case eShapeCommandTypes.Duplicate
                    Me.DuplicateTimeSeries(ashapes)

                Case eShapeCommandTypes.Import
                    Me.ImportTimeSeries()

                Case eShapeCommandTypes.Load
                    Me.LoadDatasets()

                Case eShapeCommandTypes.Remove
                    Me.RemoveTimeSeries(ashapes)

                Case eShapeCommandTypes.Modify
                    Me.ModifyTimeSeries(ashapes(0))

                Case eShapeCommandTypes.SetWeight
                    Me.SetWeight(ashapes(0), CSng(data))

                Case eShapeCommandTypes.SaveAsImage
                    Me.SaveAsImage(ashapes(0), Me.SketchPad)

                Case eShapeCommandTypes.Weight
                    Me.WeightTimeSeries()

            End Select
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overridden this to make controls respond to any kind of change in 
        ''' time series data.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Sub Refresh()
            If Me.m_bInUpdate Then Return
            Me.UpdateShapeList(Me.SelectedShapes)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Respond to local shape change.
        ''' </summary>
        ''' <param name="shape">The newly selected shape.</param>
        ''' -------------------------------------------------------------------
        Public Overrides Sub OnShapeChanged(ByVal shape As EwECore.cShapeData)
            If (Me.ShapeToolBox Is Nothing) Then Return
            If Me.m_bInUpdate Then Return

            Me.m_bInUpdate = True
            Me.ShapeToolBox.UpdateThumbnail(shape)
            Me.m_bInUpdate = False
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to kick the programmer; Time Series cannot be drawn by hand.
        ''' </summary>
        ''' <param name="shape"></param>
        ''' -------------------------------------------------------------------
        Public Overrides Sub OnShapeFinalized(ByVal shape As EwECore.cShapeData, ByVal sketchpad As ucSketchPad)
            Debug.Assert(False)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Cascade a newly selected shape to the managed controls.
        ''' </summary>
        ''' <param name="ashapes">The newly selected shapes.</param>
        ''' -------------------------------------------------------------------
        Public Overrides Sub OnShapeSelected(ByVal ashapes As EwECore.cShapeData())
            If Me.m_bInUpdate Then Return
            Me.m_bInUpdate = True
            If Me.SketchPad IsNot Nothing Then
                Me.SelectedShapes = ashapes
            End If
            Me.m_bInUpdate = False
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the colour for Time Series shapes.
        ''' </summary>
        ''' <returns>The color for Time Series shapes.</returns>
        ''' -----------------------------------------------------------------------
        Protected Overrides Function Color() As System.Drawing.Color
            Return Color.DarkGreen
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the default sketch mode for Time Series shapes.
        ''' </summary>
        ''' <returns>The default sketch mode for Time Series shapes.</returns>
        ''' -----------------------------------------------------------------------
        Protected Overrides Function SketchDrawMode() As eSketchDrawModeTypes
            Return eSketchDrawModeTypes.Dots
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the lower limit for the sketch pad Y-axis when displaying 
        ''' Time Series data.
        ''' </summary>
        ''' <returns>The lower limit for the sketch pad Y-axis when displaying 
        ''' Time Series data.</returns>
        ''' -----------------------------------------------------------------------
        Protected Overrides Function MinYScale() As Single
            Return 0.0!
        End Function

#End Region ' Baseclass overrides

#Region " Internal implementation "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Implementation of the <see cref="eShapeCommandTypes.Add">Add</see> commmand.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub AddTimeSeries()
            Dim frm As frmShapeValue = New frmShapeValue()
            If (frm.ShowDialog() = DialogResult.OK) Then
                ' Ecosim will reload, which means a reload of datasets and time series
                ' As a result, this control will be told to update
                Me.m_core.LoadTimeSeries(Me.m_core.ActiveTimeSeriesDatasetIndex)
            End If
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Implementation of the <see cref="eShapeCommandTypes.Weight">Weight</see> commmand.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub WeightTimeSeries()
            Dim cmd As Command = CommandHandler.GetInstance().GetCommand("WeightTimeSeries")

            If cmd IsNot Nothing Then
                cmd.Invoke()
            End If
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Implementation of the <see cref="eShapeCommandTypes.Load">Load</see> commmand.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub LoadDatasets()
            Dim cmd As Command = CommandHandler.GetInstance().GetCommand("LoadTimeSeries")

            If cmd IsNot Nothing Then
                cmd.Invoke()
            End If

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Implementation of the <see cref="eShapeCommandTypes.Duplicate">Duplicate</see> commmand.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub DuplicateTimeSeries(ByVal ashapes As cShapeData())

            ' Sanity check
            Debug.Assert(ashapes IsNot Nothing, "Need valid TS")

            Dim strNewTSName As String = ""
            Dim lstrTSNames As New List(Of String)
            Dim iNextTSNumber As Integer = 0
            Dim ts As cTimeSeries = Nothing
            Dim asValues() As Single
            Dim intDBID As Integer = -1
            Dim bSucces As Boolean = True

            ' Collect all current shape names
            For Each s As cShapeData In Me.m_lShapes
                lstrTSNames.Add(s.Name)
            Next

            ' Concoct a new name based on the numbered strings that are found
            iNextTSNumber = StringUtils.GetNextNumber(lstrTSNames.ToArray(), My.Resources.ECOSIM_DEFAULT_NEWTIMESERIES)
            strNewTSName = String.Format(My.Resources.ECOSIM_DEFAULT_NEWTIMESERIES, iNextTSNumber)

            ' Generate TS data
            For Each shape As cShapeData In ashapes
                ts = Me.m_core.EcosimTimeSeries(shape.Index)
                ReDim asValues(ts.ShapeData.Length - 2)
                For i As Integer = 1 To ts.ShapeData.Length - 1
                    asValues(i - 1) = ts.DatVal(i)
                Next

                bSucces = bSucces And (Me.m_core.AddTimeSeries(strNewTSName, _
                        ts.DataType, DirectCast(ts.TimeSeriesType, eTimeSeriesType), _
                        ts.WtType, asValues, intDBID))
            Next

            If bSucces Then
                ' Update shape to select
                Me.UpdateShapeList(Nothing, eAutoSelectMode.SelectLastShape)
            End If

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Implementation of the <see cref="eShapeCommandTypes.Import">Import</see> commmand.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub ImportTimeSeries()
            ' Launch via command!
            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim cmd As Command = cmdh.GetCommand("ImportTimeSeries")
            If cmd IsNot Nothing Then cmd.Invoke()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Implementation of the <see cref="eShapeCommandTypes.Remove">Remove</see> commmand.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub RemoveTimeSeries(ByVal ashapes As cShapeData())

            Dim bSucces As Boolean = True

            ' Sanity check
            Debug.Assert(ashapes IsNot Nothing, "Need valid TS")

            If ashapes.Length = 1 Then
                If MsgBox(String.Format(My.Resources.PROMPT_TIMESERIES_DELETE, ashapes(0).Name), _
                        MsgBoxStyle.YesNo Or MsgBoxStyle.Question) <> MsgBoxResult.Yes Then Return
            Else
                If MsgBox(String.Format(My.Resources.PROMPT_TIMESERIES_DELETE_MULTIPLE, ashapes.Length), _
                        MsgBoxStyle.YesNo Or MsgBoxStyle.Question) <> MsgBoxResult.Yes Then Return
            End If

            Me.m_core.SetBatchLock(cCore.eBatchLockType.Restructure)
            For Each shape As cShapeData In ashapes
                Debug.Assert(TypeOf shape Is cTimeSeries, "Need valid TS")
                bSucces = bSucces And Me.m_core.RemoveTimeSeries(shape.DBID)
            Next
            Me.m_core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecosim, bSucces)

            ' Refresh
            Me.UpdateShapeList()

        End Sub

        Private Sub ModifyTimeSeries(ByVal shape As cShapeData)

            ' Sanity check
            Debug.Assert(shape IsNot Nothing, "Need valid TS")
            Debug.Assert(TypeOf shape Is cTimeSeries, "Need valid TS")

            Dim dlg As New frmShapeValue(shape)
            dlg.ShowDialog()

        End Sub

        Private Sub SetWeight(ByVal shape As cShapeData, ByVal sWeight As Single)

            ' Sanity check
            Debug.Assert(shape IsNot Nothing, "Need valid TS")
            Debug.Assert(TypeOf shape Is cTimeSeries, "Need valid TS")

            DirectCast(shape, cTimeSeries).WtType = sWeight
            shape.Update()

        End Sub

#End Region ' Internal implementation 

#Region " Helper methods "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper enum; states how to reload data.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Enum eAutoSelectMode As Byte
            None = 0
            SelectFirstShape
            SelectLastShape
            SelectCurrentShape
        End Enum

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method; updates the list of time series to manage.
        ''' </summary>
        ''' <param name="ashapeSelect">Shapes to select.</param>
        ''' <param name="selectMode">If shape cannot be selected, or no shape 
        ''' has been provided, this mode indicates how the new selection should 
        ''' be made.</param>
        ''' -------------------------------------------------------------------
        Private Sub UpdateShapeList(Optional ByVal ashapeSelect As cShapeData() = Nothing, _
                Optional ByVal selectMode As eAutoSelectMode = eAutoSelectMode.SelectCurrentShape)

            Dim ts As cTimeSeries = Nothing
            Dim shapeSelectCurr As cShapeData() = Me.SelectedShapes

            Me.m_lShapes.Clear()

            For i As Integer = 1 To Me.m_core.nTimeSeries
                Me.m_lShapes.Add(Me.m_core.EcosimTimeSeries(i))
            Next

            ' Select a shape
            If Object.ReferenceEquals(ashapeSelect, Nothing) Then
                If Me.m_lShapes.Count > 0 Then
                    Select Case selectMode
                        Case eAutoSelectMode.None
                            ' Haha
                        Case eAutoSelectMode.SelectCurrentShape
                            ashapeSelect = shapeSelectCurr
                        Case eAutoSelectMode.SelectFirstShape
                            ashapeSelect = New cShapeData() {Me.m_lShapes(0)}
                        Case eAutoSelectMode.SelectLastShape
                            ashapeSelect = New cShapeData() {Me.m_lShapes(Me.m_lShapes.Count - 1)}
                    End Select
                End If
            End If

            If (Me.ShapeToolBox IsNot Nothing) Then
                Me.ShapeToolBox.SetShapes(Me.m_lShapes, ashapeSelect)
                ashapeSelect = Me.ShapeToolBox.Selection
            End If

            Me.SelectedShapes = ashapeSelect

        End Sub

#End Region ' Helper methods

    End Class

#End Region ' Time series

#Region " Forcing Functions "

#Region " Generic FF "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <see cref="cShapeGUIHandler">cShapeGUIHandler implementation</see> for 
    ''' handling generic <see cref="cForcingFunction">forcing functions</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(True)> _
    Public Class cForcingShapeGUIHandler
        : Inherits cShapeGUIHandler

        ''' <summary>Flag to prevent update / response loops.</summary>
        Private m_bInUpdate As Boolean = False
        ''' <summary>The FF to distribute.</summary>
        Private m_lShapes As New List(Of cShapeData)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this handler.
        ''' </summary>
        ''' <param name="core">Core to connect to.</param>
        ''' <param name="stb"><see cref="ucShapeToolbox">Shape toolbox control </see> to handle, if any.</param>
        ''' <param name="stbtb"><see cref="ucShapeToolboxToolbar">Shape toolbox toolbar control </see> to handle, if any.</param>
        ''' <param name="sp"><see cref="ucSketchPad">Shape sketch pad control </see> to handle, if any.</param>
        ''' <param name="sptb"><see cref="ucSketchPadToolbar">Shape sketch pad toolbar control </see> to handle, if any.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal core As cCore, _
               ByVal stb As ucShapeToolbox, ByVal stbtb As ucShapeToolboxToolbar, _
               ByVal sp As ucSketchPad, ByVal sptb As ucSketchPadToolbar)

            MyBase.New(core, stb, stbtb, sp, sptb)
            Me.UpdateShapeList()

        End Sub

#Region " Forcing overrides "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Specifies the shapes manager that delivers the data for this handler.
        ''' </summary>
        ''' <returns>The shapes manager that delivers the data for this handler.</returns>
        ''' -------------------------------------------------------------------
        Protected Overridable Function ShapeManager() As cBaseShapeManager
            Return Me.m_core.ForcingShapeManager()
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the name for a new forcing function.
        ''' </summary>
        ''' <returns>The name for a new forcing function.</returns>
        ''' -------------------------------------------------------------------
        Protected Overridable Function NewShapeNameMask() As String
            Return My.Resources.ECOSIM_DEFAULT_NEWFORCINGSHAPE
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Reset all shapes.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overridable Sub ResetAllShapes()

            Dim sm As cBaseShapeManager = Me.ShapeManager
            Dim shape As cForcingFunction = Nothing

            ' For every shape
            For iShape As Integer = 0 To sm.Count - 1
                ' Get the shape
                shape = sm.Item(iShape)
                ' Lock it to prevent noise during this process
                shape.LockUpdates()
                ' Reset the shape
                Me.ResetShape(New cShapeData() {shape})
                ' Cheat: force an update on the very last shape to trigger a GUI refresh
                shape.UnlockUpdates(iShape = sm.Count - 1)
            Next

        End Sub

#End Region ' Forcing overrides

#Region " Baseclass overrides "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Public interface to ask whether a given command is supported by this handler.
        ''' Overridden to weed out non-forcing function commands.
        ''' </summary>
        ''' <param name="cmd">The command to test.</param>
        ''' <returns>True if command is supported.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function SupportCommand(ByVal cmd As eShapeCommandTypes) As Boolean

            ' A 101 things you can do with a Forcing shape
            Select Case cmd
                Case eShapeCommandTypes.Add
                    Return True
                Case eShapeCommandTypes.ChangeShape
                    Return True
                Case eShapeCommandTypes.Duplicate
                    Return True
                Case eShapeCommandTypes.Modify
                    Return True
                Case eShapeCommandTypes.DisplayOptions
                    Return True
                Case eShapeCommandTypes.Remove
                    Return True
                Case eShapeCommandTypes.Reset, eShapeCommandTypes.ResetAll
                    Return True
                Case eShapeCommandTypes.SaveAsImage
                    Return True
                Case eShapeCommandTypes.Seasonal
                    Return True
                Case Else
                    Return False
            End Select
            Return False

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Public interface to query the enables state of a given command by this handler.
        ''' Overridden to enable forcing function commands.
        ''' </summary>
        ''' <param name="cmd">The <see cref="eShapeCommandTypes">command</see> to test.</param>
        ''' <returns>True if enabled.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function EnableCommand(ByVal cmd As cShapeGUIHandler.eShapeCommandTypes) As Boolean

            Dim bHasSelection As Boolean = (Me.SelectedShapes IsNot Nothing)
            Dim bHasSingleSelection As Boolean = (Me.SelectedShape IsNot Nothing)

            Select Case cmd

                Case eShapeCommandTypes.Add
                    Return True

                Case eShapeCommandTypes.Duplicate, _
                     eShapeCommandTypes.Remove, _
                     eShapeCommandTypes.Reset, _
                     eShapeCommandTypes.ResetAll
                    Return bHasSelection

                Case eShapeCommandTypes.ChangeShape, _
                     eShapeCommandTypes.Modify, _
                     eShapeCommandTypes.DisplayOptions, _
                     eShapeCommandTypes.SaveAsImage, _
                     eShapeCommandTypes.Seasonal
                    Return bHasSingleSelection

            End Select
            Return False

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Public interface to execute a given command by this handler. 
        ''' Overridden to implement forcing function commands.
        ''' </summary>
        ''' <param name="cmd">The <see cref="eShapeCommandTypes">command</see> to test.</param>
        ''' <param name="ashapes">The <see cref="EwECore.cShapeData">shape</see> to apply the command to.</param>
        ''' <param name="data">Optional data to accompany the command.</param>
        ''' -------------------------------------------------------------------
        Public Overrides Sub ExecuteCommand(ByVal cmd As eShapeCommandTypes, _
                 Optional ByVal ashapes As EwECore.cShapeData() = Nothing, Optional ByVal data As Object = Nothing)

            If (ashapes Is Nothing) Then ashapes = Me.SelectedShapes

            Select Case cmd
                Case eShapeCommandTypes.Add
                    Me.AddFF()

                Case eShapeCommandTypes.ChangeShape
                    Me.ChangeFFShape()

                Case eShapeCommandTypes.Duplicate
                    Me.DuplicateFF(ashapes)

                Case eShapeCommandTypes.Modify
                    Me.ModifyFF(ashapes(0))

                Case eShapeCommandTypes.DisplayOptions
                    Me.ShapeOptions()

                Case eShapeCommandTypes.Remove
                    Me.RemoveFF(ashapes)

                Case eShapeCommandTypes.Reset
                    Me.ResetShape(ashapes)

                Case eShapeCommandTypes.ResetAll
                    Me.ResetAllShapes()

                Case eShapeCommandTypes.SaveAsImage
                    Me.SaveAsImage(ashapes(0), Me.SketchPad)

                Case eShapeCommandTypes.Seasonal
                    Me.SetSeasonal(ashapes(0), CBool(data))

                Case Else
                    'Debug.Assert(False, String.Format("Command {0} not supported", cmd))
            End Select
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overridden this to make controls respond to any kind of change in 
        ''' forcing functions data.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Sub Refresh()
            If Me.m_bInUpdate Then Return
            Me.UpdateShapeList(Me.SelectedShapes)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to respond to a local change in the current selected forcing function.
        ''' The forcing function is still being modified; once modifications are complete
        ''' <see cref="OnShapeFinalized">OnShapeFinalized</see> is called.
        ''' </summary>
        ''' <param name="shape">The forcing function that has changed.</param>
        ''' -------------------------------------------------------------------
        Public Overrides Sub OnShapeChanged(ByVal shape As EwECore.cShapeData)
            If Me.m_bInUpdate Then Return
            If shape IsNot Nothing Then
                Me.m_bInUpdate = True
                Me.UpdateFF(shape)
                Me.m_bInUpdate = False
            End If
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to respond to a final change in the current selected forcing function.
        ''' </summary>
        ''' <param name="shape">The forcing function that has changed.</param>
        ''' -------------------------------------------------------------------
        Public Overrides Sub OnShapeFinalized(ByVal shape As EwECore.cShapeData, ByVal sketchpad As ucSketchPad)
            If Me.m_bInUpdate Then Return
            If shape IsNot Nothing Then
                Me.m_bInUpdate = True
                Me.CommitFF(shape)
                Me.m_bInUpdate = False
            End If
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Cascade a newly selected forcing function to the managed controls.
        ''' </summary>
        ''' <param name="shape">The newly selected shape.</param>
        ''' -------------------------------------------------------------------
        Public Overrides Sub OnShapeSelected(ByVal shape As EwECore.cShapeData())
            If Me.m_bInUpdate Then Return
            Me.m_bInUpdate = True
            Me.SelectedShapes = shape
            Me.m_bInUpdate = False
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the colour for rendering forcing functions.
        ''' </summary>
        ''' <returns>The color for rendering forcing functions.</returns>
        ''' -----------------------------------------------------------------------
        Protected Overrides Function Color() As System.Drawing.Color
            Return Color.FromArgb(255, 236, 55, 12)
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the default sketch mode for forcing functions.
        ''' </summary>
        ''' <returns>The default sketch mode for forcing functions.</returns>
        ''' -----------------------------------------------------------------------
        Protected Overrides Function SketchDrawMode() As eSketchDrawModeTypes
            Return eSketchDrawModeTypes.Fill
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the lower limit for the sketch pad Y-axis when displaying 
        ''' forcing functions.
        ''' </summary>
        ''' <returns>The lower limit for the sketch pad Y-axis when displaying 
        ''' forcing functions.</returns>
        ''' -----------------------------------------------------------------------
        Protected Overrides Function MinYScale() As Single
            Return 2.0!
        End Function

#End Region ' Baseclass overrides

#Region " Internal implementation "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Implementation of the <see cref="eShapeCommandTypes.Add">Add</see> commmand.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub AddFF()
            Me.CreateShape(Me.GetNewShapeName())
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Implementation of the <see cref="eShapeCommandTypes.ChangeShape">Change Shape</see> commmand.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub ChangeFFShape()
            Dim dlg As New dlgChangeShape(DirectCast(Me.SelectedShape, cForcingFunction))
            dlg.ShowDialog()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Implementation of the <see cref="eShapeCommandTypes.Duplicate">Duplicate</see> commmand.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub DuplicateFF(ByVal ashapes As cShapeData())

            ' Sanity check
            Debug.Assert(ashapes IsNot Nothing, "Need valid FF")

            Dim fsm As cBaseShapeManager = Me.ShapeManager
            Dim lffNew As New List(Of cForcingFunction)

            For Each shape As cShapeData In ashapes
                Dim ff As cForcingFunction = DirectCast(shape, cForcingFunction)
                If ff IsNot Nothing Then
                    ff = fsm.CreateNewShape(Me.GetNewShapeName(), ff.ShapeData, ff.YZero, ff.YBase, ff.YEnd, ff.Steep, ff.eShapeFunctionType)
                    If ff IsNot Nothing Then
                        lffNew.Add(ff)
                    End If
                End If
            Next

            Me.UpdateShapeList(lffNew.ToArray())

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Implementation of the <see cref="eShapeCommandTypes.Modify">Modify</see> commmand.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub ModifyFF(ByVal shape As cShapeData)

            ' Sanity check
            Debug.Assert(shape IsNot Nothing, "Need valid FF")
            Debug.Assert(TypeOf shape Is cForcingFunction, "Need valid FF")

            Dim dlg As New frmShapeValue(shape)
            dlg.ShowDialog()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Implementation of the <see cref="eShapeCommandTypes.Remove">Remove</see> commmand.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub RemoveFF(ByVal ashapes As cShapeData())

            Dim bSucces As Boolean = True

            ' Sanity check
            Debug.Assert(ashapes IsNot Nothing, "Need valid FF")

            If ashapes.Length = 1 Then
                If MsgBox(String.Format(My.Resources.PROMPT_FORCING_DELETE, ashapes(0).Name), _
                        MsgBoxStyle.YesNo Or MsgBoxStyle.Question) <> MsgBoxResult.Yes Then Return
            Else
                If MsgBox(String.Format(My.Resources.PROMPT_FORCING_DELETE_MULTIPLE, ashapes.Length), _
                        MsgBoxStyle.YesNo Or MsgBoxStyle.Question) <> MsgBoxResult.Yes Then Return
            End If

            Me.m_core.SetBatchLock(cCore.eBatchLockType.Restructure)
            For Each shape As cShapeData In ashapes
                Debug.Assert(TypeOf shape Is cForcingFunction, "Need valid FF")
                bSucces = bSucces And ShapeManager.Remove(DirectCast(shape, cForcingFunction))
            Next
            Me.m_core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecosim, bSucces)

            ' Refresh
            Me.UpdateShapeList()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Helper method; reflect on-going modifications in the selected forcing function.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub UpdateFF(ByVal shape As cShapeData)
            If (Me.ShapeToolBox IsNot Nothing) Then
                Me.ShapeToolBox.UpdateThumbnail(shape)
            End If
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Helper method; commit modifications of the selected forcing function to 
        ''' <see cref="ShapeManager">underlying manager</see>.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub CommitFF(ByVal shape As cShapeData)

            If shape IsNot Nothing Then

                Me.m_bInUpdate = True
                shape.Update()
                Me.m_bInUpdate = False

            End If

        End Sub

#End Region ' Internal implementation 

#Region " Helper methods "

        ' ToDo_JS: Obtain this constant from Core or Ecosim model?
        Private Const SIMU_YEAR_DEFAULT As Integer = 100

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Init the forcing shape params like newly added shape names, reset sketchpad, etc.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Function GetNewShapeName() As String

            Dim lstrFFNames As New List(Of String)
            Dim strNewFFName As String = ""
            Dim iNextShapeNumber As Integer = 0

            ' Collect all current shape names
            For Each s As cShapeData In Me.m_lShapes
                lstrFFNames.Add(s.Name)
            Next

            ' Concoct a new name based on the numbered strings that are found
            iNextShapeNumber = StringUtils.GetNextNumber(lstrFFNames.ToArray(), Me.NewShapeNameMask)
            Return String.Format(Me.NewShapeNameMask, iNextShapeNumber)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Herlper method; create a new forcing function.
        ''' </summary>
        ''' <param name="strName">Name of the new forcing function.</param>
        ''' -------------------------------------------------------------------
        Private Sub CreateShape(ByVal strName As String)
            ' Create new shape
            Dim shapeNew As cForcingFunction = ShapeManager.CreateNewShape(strName, Nothing)
            ' Validate
            If Object.ReferenceEquals(shapeNew, Nothing) Then Return
            ' Update 
            Me.UpdateShapeList(New cShapeData() {shapeNew})
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method; updates the list of forcing functions.
        ''' </summary>
        ''' <param name="ashapeSelect">Forcing functions to select.</param>
        ''' -------------------------------------------------------------------
        Private Sub UpdateShapeList(Optional ByVal ashapeSelect As cShapeData() = Nothing)
            Me.m_lShapes = Me.GetShapeList()
            If (Me.ShapeToolBox IsNot Nothing) Then
                Me.ShapeToolBox.SetShapes(Me.m_lShapes, ashapeSelect)
            Else
                Me.SelectedShapes = ashapeSelect
            End If
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overridable method to filter out specific forcing functions.
        ''' </summary>
        ''' <param name="shape">Forcing function to evaluate.</param>
        ''' <returns>True if forcing function should be included in the list.</returns>
        ''' -------------------------------------------------------------------
        Protected Overridable Function IncludeShape(ByVal shape As cShapeData) As Boolean
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Extract a list of shapes from the manager, calling 
        ''' <see cref="IncludeShape">IncludeShape</see> to determine if a shape
        ''' should be included in the list.
        ''' </summary>
        ''' <returns>A list of shapes to use.</returns>
        ''' -------------------------------------------------------------------
        Protected Overridable Function GetShapeList() As List(Of cShapeData)
            Dim lShapes As New List(Of cShapeData)
            Dim shape As cShapeData = Nothing

            For i As Integer = 0 To Me.ShapeManager.Count - 1
                shape = Me.ShapeManager.Item(i)
                If Me.IncludeShape(shape) Then
                    lShapes.Add(shape)
                End If
            Next
            Return lShapes
        End Function

#End Region ' Helper methods

    End Class

#End Region ' Generic FF

#Region " Egg Production "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <see cref="cShapeGUIHandler">cShapeGUIHandler implementation</see> for 
    ''' handling egg production <see cref="cForcingFunction">forcing shapes</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(True)> _
    Public Class cEggProductionShapeGUIHandler
        Inherits cForcingShapeGUIHandler

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this handler.
        ''' </summary>
        ''' <param name="core">Core to connect to.</param>
        ''' <param name="stb"><see cref="ucShapeToolbox">Shape toolbox control </see> to handle, if any.</param>
        ''' <param name="stbtb"><see cref="ucShapeToolboxToolbar">Shape toolbox toolbar control </see> to handle, if any.</param>
        ''' <param name="sp"><see cref="ucSketchPad">Shape sketch pad control </see> to handle, if any.</param>
        ''' <param name="sptb"><see cref="ucSketchPadToolbar">Shape sketch pad toolbar control </see> to handle, if any.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal core As cCore, _
                 ByVal stb As ucShapeToolbox, ByVal stbtb As ucShapeToolboxToolbar, _
                 ByVal sp As ucSketchPad, ByVal sptb As ucSketchPadToolbar)

            MyBase.New(core, stb, stbtb, sp, sptb)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Specifies the shapes manager that delivers the data for this handler.
        ''' </summary>
        ''' <returns>The shapes manager that delivers the data for this handler.</returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function ShapeManager() As cBaseShapeManager
            Return Me.m_core.EggProdShapeManager
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the colour for rendering egg production shapes.
        ''' </summary>
        ''' <returns>The color for rendering egg production shapes.</returns>
        ''' -----------------------------------------------------------------------
        Protected Overrides Function Color() As System.Drawing.Color
            Return Color.Orange
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the name for a new egg production shape.
        ''' </summary>
        ''' <returns>The name for a new egg production shape.</returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function NewShapeNameMask() As String
            Return My.Resources.ECOSIM_DEFAULT_NEWEGGPRODSHAPE
        End Function

    End Class

#End Region ' Egg Production

#Region " Effort "

#Region " Effort base class "

    <CLSCompliant(True)> _
    Public MustInherit Class cEffortShapeGUIHandler
        : Inherits cForcingShapeGUIHandler

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this handler.
        ''' </summary>
        ''' <param name="core">Core to connect to.</param>
        ''' <param name="stb"><see cref="ucShapeToolbox">Shape toolbox control </see> to handle, if any.</param>
        ''' <param name="sp"><see cref="ucSketchPad">Shape sketch pad control </see> to handle, if any.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal core As cCore, ByVal stb As ucShapeToolbox, ByVal stbtb As ucShapeToolboxToolbar, _
                ByVal sp As ucSketchPad, ByVal sptb As ucSketchPadToolbar)
            MyBase.New(core, stb, stbtb, sp, sptb)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to allow use of specific <see cref="eShapeCommandTypes">commands</see>.
        ''' </summary>
        ''' <param name="cmd">The command that is queried.</param>
        ''' <returns>True if the queried command is supported.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function SupportCommand(ByVal cmd As cShapeGUIHandler.eShapeCommandTypes) As Boolean
            Select Case cmd
                Case eShapeCommandTypes.SetToZero
                    Return True
                Case eShapeCommandTypes.SetValue
                    Return True
                Case eShapeCommandTypes.Reset, _
                     eShapeCommandTypes.ResetAll
                    Return True
                Case eShapeCommandTypes.Modify
                    Return True
            End Select
            Return False
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to enable fishing rate shape specific commands.
        ''' </summary>
        ''' <param name="cmd">The command that is queried.</param>
        ''' <returns>True if the queried command may be enabled.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function EnableCommand(ByVal cmd As cShapeGUIHandler.eShapeCommandTypes) As Boolean

            Dim bHasSelection As Boolean = (Me.SelectedShapes IsNot Nothing)
            Dim bHasSingleSelection As Boolean = (Me.SelectedShape IsNot Nothing)

            Select Case cmd

                Case eShapeCommandTypes.SetValue, _
                     eShapeCommandTypes.Modify
                    Return bHasSingleSelection

                Case eShapeCommandTypes.Reset, _
                     eShapeCommandTypes.ResetAll, _
                     eShapeCommandTypes.SetToZero
                    Return bHasSelection

            End Select
            Return False
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Public interface to execute a given command by this handler. 
        ''' Overridden to implement fishing rate forcing function commands.
        ''' </summary>
        ''' <param name="cmd">The <see cref="eShapeCommandTypes">command</see> to test.</param>
        ''' <param name="ashapes">The <see cref="EwECore.cShapeData">shapes</see> to apply the command to.</param>
        ''' <param name="data">Optional data to accompany the command.</param>
        ''' -------------------------------------------------------------------
        Public Overrides Sub ExecuteCommand(ByVal cmd As cShapeGUIHandler.eShapeCommandTypes, _
                    Optional ByVal ashapes As EwECore.cShapeData() = Nothing, _
                    Optional ByVal data As Object = Nothing)

            If (ashapes Is Nothing) Then ashapes = Me.SelectedShapes
            Select Case cmd
                Case eShapeCommandTypes.Reset
                    If (data IsNot Nothing) Then
                        MyBase.ResetShape(ashapes, CSng(data))
                    Else
                        Me.ResetShapePrompted(ashapes)
                    End If

                Case Else
                    MyBase.ExecuteCommand(cmd, ashapes, data)

            End Select
        End Sub

        Protected Overrides Sub ResetAllShapes()
            Me.m_core.FishingRateShapeManager.ResetToDefaults()
            Me.m_core.FishMortShapeManager.ResetToDefaults()
        End Sub

        Protected MustOverride Function ScaleMode() As eAxisTickmarkDisplayModeTypes

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="ucSketchPad">Sketch pad control</see> to manage
        ''' by this handler. Overridden to fix some behaviours of this control
        ''' particular to displaying fishing rate shapes.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Property SketchPad() As ucSketchPad
            Get
                Return MyBase.SketchPad
            End Get
            Set(ByVal value As ucSketchPad)
                MyBase.SketchPad = value
                If value IsNot Nothing Then
                    If (TypeOf value Is ucForcingSketchPad) Then
                        DirectCast(value, ucForcingSketchPad).AxisTickMarkDisplayMode = Me.ScaleMode()
                    End If
                End If
            End Set
        End Property

#Region " Internals "

        Private Sub ResetShapePrompted(ByVal ashapes As cShapeData())

            Dim strCaption As String = My.Resources.RUN_ECOSIM_F_VALUE_CAPTION
            Dim strMessage As String = My.Resources.RUN_ECOSIM_F_VALUE_MSG
            Dim strDefault As String = "1"
            Dim strValue As String = String.Empty

            ' Sanity check
            If ashapes Is Nothing Then Return

            strValue = Interaction.InputBox(strMessage, strCaption, strDefault)

            'User clicks OK
            If strValue.Length <> 0 Then

                Dim astrEntered As String() = strValue.Split(CChar(" "))

                ' One character entered?
                If astrEntered.Length = 1 Then
                    ' #Yes: duplicate this char over the entire shape
                    Try
                        Me.ResetShape(ashapes, CSng(Val(astrEntered(0))))
                    Catch ex As Exception
                        Me.m_core.Messages.SendMessage(New cMessage(String.Format("Failed to set value {0}", astrEntered(0)), _
                                eMessageType.NotSet, eCoreComponentType.ShapesManager, eMessageImportance.Warning))
                    End Try

                ElseIf astrEntered.Length > 1 Then

                    For Each shape As cShapeData In ashapes

                        ' Translate individual values
                        Dim asValues(shape.XMax) As Single
                        Dim sValue As Single = 0.0!

                        For i As Integer = 0 To shape.XMax
                            If (i < (astrEntered.Length - 1)) Then
                                Try
                                    sValue = CSng(Val(astrEntered(i)))
                                Catch ex As Exception
                                    sValue = -1
                                End Try
                            End If
                            asValues(i) = sValue
                        Next

                        shape.LockUpdates()
                        shape.ShapeData = asValues
                        shape.UnlockUpdates()

                    Next

                End If
            End If
        End Sub

#End Region ' Internals

    End Class

#End Region ' Effort base class

#Region " Fishing rate "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <see cref="cShapeGUIHandler">cShapeGUIHandler implementation</see> for 
    ''' handling fishing rate <see cref="cForcingFunction">forcing shapes</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(True)> _
    Public Class cFishingRateShapeGUIHandler
        : Inherits cEffortShapeGUIHandler

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this handler.
        ''' </summary>
        ''' <param name="core">Core to connect to.</param>
        ''' <param name="stb"><see cref="ucShapeToolbox">Shape toolbox control </see> to handle, if any.</param>
        ''' <param name="sp"><see cref="ucSketchPad">Shape sketch pad control </see> to handle, if any.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal core As cCore, ByVal stb As ucShapeToolbox, ByVal sp As ucSketchPad, _
                Optional ByVal stbtb As ucShapeToolboxToolbar = Nothing, Optional ByVal sptb As ucSketchPadToolbar = Nothing)
            MyBase.New(core, stb, stbtb, sp, sptb)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the colour for rendering fishing rate shapes.
        ''' </summary>
        ''' <returns>The color for rendering fishing rate shapes.</returns>
        ''' -----------------------------------------------------------------------
        Protected Overrides Function Color() As System.Drawing.Color
            Return Drawing.Color.Coral
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Specifies the shapes manager that delivers the data for this handler.
        ''' </summary>
        ''' <returns>The shapes manager that delivers the data for this handler.</returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function ShapeManager() As EwECore.cBaseShapeManager
            Return Me.m_core.FishingRateShapeManager
        End Function

        Protected Overrides Function ScaleMode() As eAxisTickmarkDisplayModeTypes
            Return eAxisTickmarkDisplayModeTypes.Relative
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to make shape display controls auto-scale the Y axis.
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function MinYScale() As Single
            Return cCore.NULL_VALUE
        End Function

    End Class

#End Region ' Fishing Rate

#Region " Fishing mortality "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <see cref="cShapeGUIHandler">cShapeGUIHandler implementation</see> for 
    ''' handling fishing mortality <see cref="cForcingFunction">forcing shapes</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(True)> _
    Public Class cFishingMortalityShapeGUIHandler
        : Inherits cEffortShapeGUIHandler

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this handler.
        ''' </summary>
        ''' <param name="core">Core to connect to.</param>
        ''' <param name="stb"><see cref="ucShapeToolbox">Shape toolbox control </see> to handle, if any.</param>
        ''' <param name="sp"><see cref="ucSketchPad">Shape sketch pad control </see> to handle, if any.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal core As cCore, ByVal stb As ucShapeToolbox, ByVal sp As ucSketchPad, _
                Optional ByVal stbtb As ucShapeToolboxToolbar = Nothing, Optional ByVal sptb As ucSketchPadToolbar = Nothing)
            MyBase.New(core, stb, stbtb, sp, sptb)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the colour for rendering fishing mortality shapes.
        ''' </summary>
        ''' <returns>The color for rendering fishing mortality shapes.</returns>
        ''' -----------------------------------------------------------------------
        Protected Overrides Function Color() As System.Drawing.Color
            Return Drawing.Color.DarkGray
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Specifies the shapes manager that delivers the data for this handler.
        ''' </summary>
        ''' <returns>The shapes manager that delivers the data for this handler.</returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function ShapeManager() As EwECore.cBaseShapeManager
            Return Me.m_core.FishMortShapeManager
        End Function

        Protected Overrides Function ScaleMode() As eAxisTickmarkDisplayModeTypes
            Return eAxisTickmarkDisplayModeTypes.Absolute
        End Function

        Protected Overrides Function MinYScale() As Single
            Return 0
        End Function

    End Class

#End Region ' Fishing mortality

#End Region ' Effort

#End Region ' Forcing Functions 

End Namespace
