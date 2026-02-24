' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.UI

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing a plugin point that automatically saves its
    ''' data. Note that this plug-in point just serves to identify the auto-save
    ''' setting in the user interface. The plug-in is responsible for triggering and
    ''' implementing the auto-save behaviour.
    ''' </summary>
    ''' <remarks>
    ''' <para>The EwE framework expects an AutoSave plug-in to store its files in a
    ''' location that is determined as follows:</para>
    ''' <code>Dim strPath as string = Path.Combine(core.DefaultOutputPath(Me.AutoSaveType), Me.AutoSaveSubPath)</code>
    ''' <para>The EwE auto-save options interface will display this storage location
    ''' for auto-save plug-ins. Developers are responsible to follow this folder
    ''' convention when implementing auto-save behaviour.</para>
    ''' </remarks>
    ''' ---------------------------------------------------------------------------
    Public Interface IAutoSavePlugin
        Inherits IPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether this plug-in is allowed to auto-save data.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Property AutoSave As Boolean

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the <see cref="eAutosaveTypes"/> core autosave type that defines the
        ''' output path that this plug-in writes to.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Function AutoSaveType() As eAutosaveTypes

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the output path to save to. A plug-in is responsible for ensuting
        ''' that the default output path is nested under the EwE location for the 
        ''' provided <see cref="AutoSaveType"/>.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Function AutoSaveOutputPath() As String

    End Interface

End Namespace