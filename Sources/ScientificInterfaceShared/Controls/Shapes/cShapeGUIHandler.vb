#Region " Imports "

Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Commands
Imports ScientificInterfaceShared.Definitions
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Commands

#End Region ' Imports

Namespace Controls

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
            Export
            ''' <summary>Export shape data.</summary>
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
            ''' <summary>Define mediation items.</summary>
            DefineMediation
            ''' <summary>Filter display of shapes.</summary>
            Filter
            <Obsolete("Use DefineMediation instead")> _
            DefineXAxis = DefineMediation
        End Enum

#Region " Private variables "

        Private m_uic As cUIContext = Nothing

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

        Private m_mhShapes As cMessageHandler = Nothing

#End Region ' Private variables

#Region " Constructor and destructor "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this handler.
        ''' </summary>
        ''' <param name="uic"><see cref="cUIContext">UI contextual</see> information.</param>
        ''' <param name="stb"><see cref="ucShapeToolbox">Shape toolbox control </see> to handle, if any.</param>
        ''' <param name="stbtb"><see cref="ucShapeToolboxToolbar">Shape toolbox toolbar control </see> to handle, if any.</param>
        ''' <param name="sp"><see cref="ucSketchPad">Shape sketch pad control </see> to handle, if any.</param>
        ''' <param name="sptb"><see cref="ucSketchPadToolbar">Shape sketch pad toolbar control </see> to handle, if any.</param>
        ''' -------------------------------------------------------------------
        Public Overridable Sub Attach(ByVal uic As cUIContext, _
                                      ByVal stb As ucShapeToolbox, _
                                      ByVal stbtb As ucShapeToolboxToolbar, _
                                      ByVal sp As ucSketchPad, _
                                      ByVal sptb As ucSketchPadToolbar)

            Me.m_uic = uic
            Me.ShapeToolBox = stb
            Me.ShapeToolBoxToolbar = stbtb
            Me.SketchPad = sp
            Me.SketchPadToolbar = sptb

            Me.m_mhShapes = New cMessageHandler(AddressOf OnCoreMessage, eCoreComponentType.EcoSim, eMessageType.DataModified, Me.m_uic.SyncObject)
            Me.UIContext.Core.Messages.AddMessageHandler(Me.m_mhShapes)
        End Sub

        Public Overridable Sub Detach()
            Me.UIContext.Core.Messages.RemoveMessageHandler(Me.m_mhShapes)
            Me.m_mhShapes = Nothing

            Me.ShapeToolBox = Nothing
            Me.ShapeToolBoxToolbar = Nothing
            Me.SketchPad = Nothing
            Me.SketchPadToolbar = Nothing
            'Me.UIContext = Nothing
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

        Protected Overridable Sub OnCoreMessage(ByRef mgs As cMessage)
            If Me.Datatypes Is Nothing Then Return
            If Array.IndexOf(Me.Datatypes, mgs.DataType) >= 0 Then
                Me.Refresh()
            End If
        End Sub

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
        Public MustOverride Function Color() As Color

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

        Protected MustOverride Function Datatypes() As eDataTypes()

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

        Protected Overridable Sub SaveAsImage(ByVal shape As cShapeData, ByVal sp As ucSketchPad)

            Dim msg As cMessage = Nothing
            Dim strError As String = ""
            Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
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
                Me.Core.Messages.SendMessage(msg)

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

            Dim fms As cFeedbackMessage = Nothing

            Debug.Assert(shape IsNot Nothing)

            If (bSeasonal = False) Then
                Me.SelectedShape.IsSeasonal = False
            Else
                If Not Me.SelectedShape.IsSeasonal Then
                    fms = New cFeedbackMessage(My.Resources.SHAPE_TYPE_TO_SEASONAL_MSG, _
                                               eCoreComponentType.ShapesManager, _
                                               eMessageType.Any, _
                                               eMessageImportance.Information, _
                                               cFeedbackMessage.eReplyStyle.YES_NO, _
                                               eDataTypes.Forcing, cFeedbackMessage.eReply.YES)
                    Me.Core.Messages.SendMessage(fms, True)
                    If fms.Reply = cFeedbackMessage.eReply.YES Then
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
            Dim dlg As New dlgGraphDisplayOptions(Me.UIContext, Me.m_sketchPad)
            dlg.ShowDialog()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get a list of filters to apply to the shape handler.
        ''' </summary>
        ''' <returns>A list of filter descriptions, or Nothing if filters do not
        ''' apply.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable ReadOnly Property Filters() As String()
            Get
                Return Nothing
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the selected filter index.
        ''' </summary>
        ''' <remarks>The filter index corresponds to an index in the list of 
        ''' provided <see cref="Filters"/></remarks>
        ''' -------------------------------------------------------------------
        Public Overridable Property FilterIndex() As Integer
            Get
                Return -1
            End Get
            Set(ByVal value As Integer)
                ' NOP
            End Set
        End Property

#End Region ' Tools

#Region " Public access "

        Public ReadOnly Property UIContext() As cUIContext
            Get
                Return Me.m_uic
            End Get
        End Property

        Public ReadOnly Property Core() As cCore
            Get
                Return Me.m_uic.Core
            End Get
        End Property

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
                    Me.m_sketchPad.UIContext = Me.UIContext
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
                    Me.m_shapeToolBox.UIContext = Me.UIContext
                    Me.m_shapeToolBox.Color = Me.Color
                    Me.m_shapeToolBox.YAxisMinValue = Me.MinYScale
                    Me.m_shapeToolBox.SketchDrawMode = Me.SketchDrawMode
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

#End Region ' Public access

#Region " Factory "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Factory method; build a shape gui handler for a given <see cref="eDataTypes">shape data type</see>.
        ''' </summary>
        ''' <param name="dt">The <see cref="eDataTypes"/> to build the handler for.</param>
        ''' <returns>A shape gui handler, or nothing if the data type did not 
        ''' denote a shape type.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function GetShapeUIHandler(ByVal dt As eDataTypes) As cShapeGUIHandler
            Select Case dt
                Case eDataTypes.Forcing : Return New cForcingShapeGUIHandler()
                Case eDataTypes.FishingEffort : Return New cFishingEffortShapeGUIHandler()
                Case eDataTypes.FishMort : Return New cFishingMortalityShapeGUIHandler()
                Case eDataTypes.Mediation : Return New cMediationShapeGUIHandler()
                Case eDataTypes.PriceMediation : Return New cLandingsShapeGUIHandler()
                Case eDataTypes.GroupTimeSeries : Return New cTimeSeriesShapeGUIHandler()
                Case eDataTypes.FleetTimeSeries : Return New cTimeSeriesShapeGUIHandler()
            End Select
            Return Nothing
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Factory method; build a shape gui handler for a given <see cref="cShapeData">shape</see>.
        ''' </summary>
        ''' <param name="shape">The <see cref="cShapeData"/> to build the handler for.</param>
        ''' <returns>A shape gui handler, or nothing if the shape is of a new type
        ''' that is not yet supported.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function GetShapeUIHandler(ByVal shape As cShapeData) As cShapeGUIHandler
            If (shape Is Nothing) Then Return Nothing
            Dim handler As cShapeGUIHandler = cShapeGUIHandler.GetShapeUIHandler(shape.DataType)
            Debug.Assert(handler IsNot Nothing, "Unknown shape type")
            Return handler
        End Function

#End Region ' Factory

    End Class

End Namespace
