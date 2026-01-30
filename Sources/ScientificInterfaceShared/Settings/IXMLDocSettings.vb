' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Xml

Public Interface IXMLDocSettings

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the XML document with the content of a group of settings.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Property Settings() As XmlDocument

End Interface
