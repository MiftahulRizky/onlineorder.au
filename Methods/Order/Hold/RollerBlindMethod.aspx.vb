Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic
Imports System.Linq
Partial Class Methods_Order_RollerBlindMethod
    Inherits System.Web.UI.Page
    
    Shared publicCfg As New PublicConfig()
    Shared orderCfg As New OrderConfig()
    Public Shared myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Class ParamSubmit
        '#Submit OrderHeader
        Public Property blindtype As String
        Public Property brackettype As String
        Public Property tubetype As String
        Public Property controltype As String
        Public Property colourtype As String
        Public Property qty As String
        Public Property room As String
        Public Property sizetype As String
        Public Property dropfloor As String
        Public Property mounting As String
        Public Property width As String
        Public Property drop As String
        Public Property fabrictype As String
        Public Property fabriccolour As String
        Public Property motorstyle As String
        Public Property motorremote As String
        Public Property externalbattery As String
        Public Property charger As String
        Public Property cableexitpoint As String
        Public Property connector As String
        Public Property roll As String
        Public Property controlposition As String
        Public Property chaincolour As String
        Public Property chainlength As String
        Public Property trim As String
        Public Property railtype As String
        Public Property railcolour As String
        Public Property tubesize As String
        Public Property childsafe As String
        Public Property accessory As String
        Public Property extras As String
        Public Property bracketcovers As String
        Public Property bracketcovercolours As String
        Public Property bracketext As String
        Public Property notes As String
        Public Property markup As String
        

        '#aditional param
        Public Property headerid As String
        Public Property itemaction As String
        Public Property itemid As String
        Public Property designid As String
        Public Property loginid As String
        Public Property blindno As String
        Public Property uniqueid As String
        Public Property isConfirmed As Boolean
    End Class

    Public Class ParamListData
        Public Property field As String
        Public Property designid As String
        Public Property blindtype As String
        Public Property brackettype As String
        Public Property tubetype As String
        Public Property controltype As String
        Public Property fabrictype As String
        Public Property trim As String
    End Class

    '#--- Kelas Output WebMethod ---#
    Public Class ErrorDetail
        Public Property message As String
        Public Property field As String
    End Class

    Public Class ErrorResponse
        Public Property [error] As ErrorDetail
    End Class

    Public Class SuccessResponse
        Public Property success As String
    End Class

    Public Class ConfirmDetail
        Public Property message As String
    End Class

    Public Class ConfirmResponse
        Public Property confirm As ConfirmDetail
        ' Public Property message As String
    End Class

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function GetItemData(ByVal query As String) As Object
        Try
            Dim Item As String = publicCfg.GetItemData(query)
            Return Item
        Catch ex As Exception
            Return "ERROR: " & ex.Message ' biar kelihatan errornya
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindListData(ByVal data As ParamListData) As Object
        Try
            Dim query As String = ""
            Dim resultList As New List(Of Dictionary(Of String, String))()

            Select Case data.field.ToLower()
                Case "blindtype"
                    query = String.Format("SELECT Id, Name FROM Blinds WHERE DesignId='{0}' AND Active=1 ORDER BY Name ASC", data.designid)
                    Return GetFormattedData(query, "Id", "Name")

                Case "brackettype"
                    query = String.Format("SELECT BracketType FROM HardwareKits WHERE DesignId = '{0}' AND BlindId='{1}' AND Active=1 GROUP BY BracketType ORDER BY BracketType ASC", data.designid, UCase(data.blindtype).ToString())
                    Return GetFormattedData(query, "BracketType", "BracketType")

                Case "tubetype"
                    query = String.Format("SELECT TubeType FROM HardwareKits WHERE DesignId = '{0}' AND BlindId='{1}' AND BracketType='{2}' AND Active=1 GROUP BY TubeType ORDER BY TubeType ASC", data.designid, UCase(data.blindtype).ToString(), data.brackettype)
                    Return GetFormattedData(query, "TubeType", "TubeType")

                Case "controltype"
                    query = String.Format("SELECT ControlType FROM HardwareKits WHERE DesignId = '{0}' AND BlindId='{1}' AND BracketType='{2}' AND TubeType='{3}' AND Active=1 GROUP BY ControlType ORDER BY ControlType ASC", data.designid, UCase(data.blindtype).ToString(), data.brackettype, data.tubetype)
                    Return GetFormattedData(query, "ControlType", "ControlType")

                Case "colourtype"
                    query = String.Format("SELECT Id, ColourType FROM HardwareKits WHERE BlindId = '{1}' AND BracketType = '{2}' AND TubeType = '{3}' AND ControlType='{4}' AND Active=1 ORDER BY Name ASC", data.designid, UCase(data.blindtype).ToString(), data.brackettype, data.tubetype, data.controltype)
                    Return GetFormattedData(query, "Id", "ColourType")

                Case "fabrictype"
                    query = String.Format("SELECT Type FROM Fabrics WHERE DesignId='{0}' AND Active='1' GROUP BY Type ORDER BY Type ASC", data.designid)
                    Return GetFormattedData(query, "Type", "Type")

                Case "fabriccolour"
                    query = String.Format("SELECT Id, Colour FROM Fabrics WHERE DesignId='{0}' AND Active='1' AND Type='{1}' ORDER BY Name ASC", data.designid, data.fabrictype)
                    Return GetFormattedData(query, "Id", "Colour")

                Case "railtype"
                    Dim FindBracket As String = data.brackettype
                    If data.brackettype = "Headbox & Side Channels" Then
                        FindBracket = "Headbox &amp; Side Channels"
                    End If
                    
                    If data.brackettype = "With Tube & Bottom Included" Then
                        FindBracket = "With Tube &amp; Bottom Included"
                    End If

                    query = String.Format("SELECT Type FROM Bottoms CROSS APPLY STRING_SPLIT(BracketType, ',') WHERE VALUE = '{0}' AND Company = 'SP' AND Trim ='{1}' AND Active ='1' GROUP BY Type ORDER BY Type ASC", FindBracket, data.trim)
                    Return GetFormattedData(query, "Type", "Type")

                Case Else
                    Return New With {.error = "Invalid field"}
            End Select

        Catch ex As Exception
            Return New With {.error = ex.Message}
        End Try
    End Function

    Private Shared Function GetFormattedData(query As String, valueField As String, textField As String) As Object
        Dim list As New List(Of Dictionary(Of String, String))()

        Dim datas As DataSet = publicCfg.GetListData(query)

        If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
            For Each row As DataRow In datas.Tables(0).Rows
                list.Add(New Dictionary(Of String, String) From {
                    {"value", row(valueField).ToString()},
                    {"text", row(textField).ToString()}
                })
            Next
        End If

        Return list
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindBlindType(ByVal designid As String) As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT * FROM Blinds WHERE DesignId='" + designid + "' AND Active=1 ORDER BY Name ASC")
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("Id").ToString()},
                        {"text", row("Name").ToString()}
                    }
                    list.Add(result)
                Next
            End If
            Return list
        Catch ex As Exception
            ' Return sebagai objek error agar bisa ditangani di sisi client
            Return New With {.error = ex.Message}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindBracketType(ByVal designid As String, ByVal blindid As String) As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT BracketType FROM HardwareKits WHERE DesignId = '" + designid + "' AND BlindId='" + UCase(blindid).ToString() + "' AND Active=1 GROUP BY BracketType ORDER BY BracketType ASC")
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("BracketType").ToString()},
                        {"text", row("BracketType").ToString()}
                    }
                    list.Add(result)
                Next
            End If
            Return list
        Catch ex As Exception
            ' Return sebagai objek error agar bisa ditangani di sisi client
            Return New With {.error = ex.Message}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindTubeType(ByVal designid As String, ByVal blindid As String, ByVal brackettype As String) As Object
        Try
            Dim MyQuery As String = String.Format("SELECT TubeType FROM HardwareKits WHERE DesignId = '{0}' AND BlindId='{1}' AND BracketType='{2}' AND Active=1 GROUP BY TubeType ORDER BY TubeType ASC", designid, UCase(blindid).ToString(), brackettype)
            Dim datas As DataSet = publicCfg.GetListData(MyQuery)
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("TubeType").ToString()},
                        {"text", row("TubeType").ToString()}
                    }
                    list.Add(result)
                Next
            End If
            Return list
        Catch ex As Exception
            ' Return sebagai objek error agar bisa ditangani di sisi client
            Return New With {.error = ex.Message}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindControlType(ByVal designid As String, ByVal blindid As String, ByVal brackettype As String, ByVal tubetype As String) As Object
        Try
            Dim MyQuery As String = String.Format("SELECT ControlType FROM HardwareKits WHERE DesignId = '{0}' AND BlindId='{1}' AND BracketType='{2}' AND TubeType='{3}' AND Active=1 GROUP BY ControlType ORDER BY ControlType ASC", designid, UCase(blindid).ToString(), brackettype, tubetype)
            Dim datas As DataSet = publicCfg.GetListData(MyQuery)
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("ControlType").ToString()},
                        {"text", row("ControlType").ToString()}
                    }
                    list.Add(result)
                Next
            End If
            Return list
        Catch ex As Exception
            ' Return sebagai objek error agar bisa ditangani di sisi client
            Return New With {.error = ex.Message}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindColourType(ByVal designid As String, ByVal blindid As String, ByVal brackettype As String, ByVal tubetype As String, ByVal controltype As String) As Object
        Try
            ' Dim MyQuery As String = String.Format("SELECT Id, ColourType FROM HardwareKits WHERE DesignId = '{0}' AND BlindId='{1}' AND BracketType='{2}' AND TubeType='{3}' AND Active=1 GROUP BY ColourType ORDER BY ColourType ASC", designid, UCase(blindid).ToString(), brackettype, tubetype, controltype)
            Dim MyQuery As String = String.Format("SELECT *, UPPER(ColourType) AS ColourText FROM HardwareKits WHERE BlindId = '{1}' AND BracketType = '{2}' AND TubeType = '{3}' AND ControlType='{4}' AND Active=1 ORDER BY Name ASC", designid, UCase(blindid).ToString(), brackettype, tubetype, controltype)
            Dim datas As DataSet = publicCfg.GetListData(MyQuery)
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("Id").ToString()},
                        {"text", row("ColourType").ToString()}
                    }
                    list.Add(result)
                Next
            End If
            Return list
        Catch ex As Exception
            ' Return sebagai objek error agar bisa ditangani di sisi client
            Return New With {.error = ex.Message}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindFabricType(ByVal designid As String) As Object
        Try
            Dim MyQuery As String = String.Format("SELECT Type FROM Fabrics WHERE DesignId='{0}' AND Active='1' GROUP BY Type ORDER BY Type ASC", designid)
            Dim datas As DataSet = publicCfg.GetListData(MyQuery)
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("Type").ToString()},
                        {"text", row("Type").ToString()}
                    }
                    list.Add(result)
                Next
            End If
            Return list
        Catch ex As Exception
            ' Return sebagai objek error agar bisa ditangani di sisi client
            Return New With {.error = ex.Message}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindFabricColour(ByVal designid As String, ByVal fabrictype As String) As Object
        Try
            Dim MyQuery As String = String.Format("SELECT Id, Colour FROM Fabrics WHERE DesignId='{0}' AND Active='1' AND Type='{1}' ORDER BY Name ASC", designid, fabrictype)
            Dim datas As DataSet = publicCfg.GetListData(MyQuery)
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("Id").ToString()},
                        {"text", row("Colour").ToString()}
                    }
                    list.Add(result)
                Next
            End If
            Return list
        Catch ex As Exception
            ' Return sebagai objek error agar bisa ditangani di sisi client
            Return New With {.error = ex.Message}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindRailType(ByVal brackettype As String, ByVal trim As String) As Object
        Try
            Dim FindBracket As String = brackettype

            If brackettype = "Headbox & Side Channels" Then
                FindBracket = "Headbox &amp; Side Channels"
            End If
            
            If brackettype = "With Tube & Bottom Included" Then
                FindBracket = "With Tube &amp; Bottom Included"
            End If

            Dim AdditionalQuery As String =  ""

            Dim MyQuery As String = String.Format("SELECT UPPER(Type) AS TypeText, Type AS TypeValue FROM Bottoms CROSS APPLY STRING_SPLIT(BracketType, ',') WHERE VALUE = '{0}' AND Company = 'SP' AND Trim ='{2}' AND Active ='1' GROUP BY Type ORDER BY Type ASC", FindBracket, AdditionalQuery, trim)
            Dim datas As DataSet = publicCfg.GetListData(MyQuery)
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("TypeValue").ToString()},
                        {"text", row("TypeText").ToString()}
                    }
                    list.Add(result)
                Next
            End If
            Return list
        Catch ex As Exception
            ' Return sebagai objek error agar bisa ditangani di sisi client
            Return New With {.error = ex.Message}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindRailColour(ByVal brackettype As String, ByVal railtype As String, ByVal trim As String) As Object
        Try
            Dim FindBracket As String = brackettype

            If brackettype = "Headbox & Side Channels" Then
                FindBracket = "Headbox &amp; Side Channels"
            End If
            
            If brackettype = "With Tube & Bottom Included" Then
                FindBracket = "With Tube &amp; Bottom Included"
            End If

             Dim AdditionalQuery As String =  ""

            Dim MyQuery As String = String.Format("SELECT Id, UPPER(Colour) AS Colour, VALUE Product FROM Bottoms CROSS APPLY STRING_SPLIT(BracketType, ',') WHERE VALUE = '{0}' AND Type='{1}' AND Company = 'SP' AND Trim ='{3}' AND Active ='1' ORDER BY Name ASC", FindBracket, railtype, AdditionalQuery, trim)
            Dim datas As DataSet = publicCfg.GetListData(MyQuery)
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("Id").ToString()},
                        {"text", row("Colour").ToString()}
                    }
                    list.Add(result)
                Next
            End If
            Return list
        Catch ex As Exception
            ' Return sebagai objek error agar bisa ditangani di sisi client
            Return New With {.error = ex.Message}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindItemOrder(ByVal itemid As String) As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData(String.Format("SELECT * FROM view_details WHERE Id = '{0}'", itemid))

            Dim data As DataSet = DirectCast(datas, DataSet)

            Dim resultList As New List(Of Dictionary(Of String, String))()

            If data IsNot Nothing AndAlso data.Tables.Count > 0 Then
                For Each row As DataRow In data.Tables(0).Rows
                    Dim dict As New Dictionary(Of String, String)()
                    For Each col As DataColumn In data.Tables(0).Columns
                        dict(col.ColumnName) = row(col).ToString()
                    Next
                    resultList.Add(dict)
                Next
            End If

            Return resultList
        Catch ex As Exception
            ' Tangani error agar bisa dikenali di JavaScript
            Return New With {.error = True, .message = ex.Message}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function Submit(ByVal data As ParamSubmit) As Object
        Try
            Dim BlindName As String = publicCfg.GetItemData(String.Format("SELECT Name FROM Blinds WHERE Id = '{0}'", data.blindtype))
            Dim qty As Integer
            If String.IsNullOrEmpty(data.qty) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "qty type is required !", .field = "qty"}}
            End If
            If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "please check your qty !", .field = "qty"}}
            End If

            If Not String.IsNullOrEmpty(data.room) Then
                If InStr(data.room, "&") > 0 Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "character [&] is not allowed !", .field = "room"}}
                End If
            End If

            If InArray(BlindName, "Standard", "Motorised", "Cassette") Then
                ' If String.IsNullOrEmpty(data.sizetype) Then
                '     Return New ErrorResponse With { .error = New ErrorDetail With { .message = "size type is required !", .field = "sizetype"}}
                ' End If

                ' If data.sizetype = "Opening Size" AND data.mounting = "Face Fit"
                '     If String.IsNullOrEmpty(data.dropfloor) Then
                '         Return New ErrorResponse With { .error = New ErrorDetail With { .message = "drop to the floor is required !", .field = "dropfloor"}}
                '     End If
                ' End If
            End IF

            If data.sizetype = "Opening Size" AND String.IsNullOrEmpty(data.mounting) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "mounting is required !", .field = "mounting"}}
            End If
            
            Dim width As Integer
            If String.IsNullOrEmpty(data.width) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width is required !",.field = "width"}}
            End If
            If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width must be a positive integer !",.field = "width"}}
            End If
            If width < 150 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width must be less than or equal to 150 !",.field = "width"}}
            End If
            If width > 6000 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width must be less than or equal to 6000 !",.field = "width"}}
            End If

            Dim drop As Integer
            If String.IsNullOrEmpty(data.drop) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "drop is required !",.field = "drop"}}
            End If
            If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "drop must be a positive integer !",.field = "drop"}}
            End If
            If drop < 150 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "drop must be greater than or equal to 150 !",.field = "drop"}}
            End If
            If drop > 3200 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "drop must be less than or equal to 3200 !",.field = "drop"}}
            End If

            If String.IsNullOrEmpty(data.fabrictype) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fabric type is required !",.field = "fabrictype"}}
            End If
            If String.IsNullOrEmpty(data.fabriccolour) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fabric colour is required !",.field = "fabriccolour"}}
            End If

            If (BlindName = "Cassette" AND data.tubetype = "Motorised") OR BlindName = "Motorised" Then

                If Not InArray(data.brackettype, "Linked 2 Blinds (Dep)", "Linked 3 Blinds (Dep)") Then
                    If String.IsNullOrEmpty(data.motorstyle) Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "motor style is required !",.field = "motorstyle"}}
                    End If
                End If
                ' If String.IsNullOrEmpty(data.motorremote) Then
                '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "motor remote is required !",.field = "motorremote"}}
                ' End If
            End If

            If Not (data.tubetype = "Spring Operated" OR data.tubetype = "N/A") Then
                If Not BlindName = "Skin Only" Then
                    If String.IsNullOrEmpty(data.roll) Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "roll direction is required !",.field = "roll"}}
                    End If
                End If

                ' If InArray(data.brackettype, "Single", "Double", "Linked 2 Blinds (Ind)", "Headbox Only", "Headbox & Side Channels") Then
                '     If String.IsNullOrEmpty(data.controlposition) Then
                '         Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position / side is required !",.field = "controlposition"}}
                '     End If
                ' End If
            End IF

            If data.controltype = "Chain" Then
                If InArray(data.brackettype, "Single", "Double") Then
                    If String.IsNullOrEmpty(data.controlposition) Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position is required !",.field = "controlposition"}}
                    End If

                    If String.IsNullOrEmpty(data.chaincolour) Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour is required !",.field = "chaincolour"}}
                    End If
                    Dim chainlength As Integer
                    If Not String.IsNullOrEmpty(data.chainlength) Then
                        If Not Integer.TryParse(data.chainlength, chainlength) OrElse chainlength <= 0 Then
                            Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain length must be a positive integer !",.field = "chainlength"}}
                        End If
                    End If
                End If

                If data.brackettype = "Linked 2 Blinds (Ind)" Then
                    If String.IsNullOrEmpty(data.controlposition) Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position is required !",.field = "controlposition"}}
                    End If

                    If String.IsNullOrEmpty(data.chaincolour) Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour is required !",.field = "chaincolour"}}
                    End If
                    Dim chainlength As Integer
                    If Not String.IsNullOrEmpty(data.chainlength) Then
                        If Not Integer.TryParse(data.chainlength, chainlength) OrElse chainlength <= 0 Then
                            Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain length must be a positive integer !",.field = "chainlength"}}
                        End If
                    End If

                    If data.blindno = "Blind 1" Then
                        If InArray(data.itemaction, "EditItem", "ViewItem") Then
                            Dim ControlB2 As String = FindControlPosition(data.uniqueid, "Blind 2")
                            If data.controlposition = ControlB2 AndAlso Not data.isConfirmed Then
                                Return New ConfirmResponse With { .confirm = New ConfirmDetail With { .message = "For linked 2 blinds independent: <b>The control position cannot be the same as the second blind! </b> If this process continues, the controls will end up in opposing positions. Do you want to continue?"}}
                            End If
                        End If
                    End If

                    If data.blindno = "Blind 2" Then
                        If InArray(data.itemaction, "NextItem", "EditItem", "ViewItem") Then
                            Dim ControlB1 As String = FindControlPosition(data.uniqueid, "Blind 1")
                            If data.controlposition = ControlB1 AndAlso Not data.isConfirmed Then
                                Return New ConfirmResponse With { .confirm = New ConfirmDetail With { .message = "For linked 2 blinds independent: <b>The control position cannot be the same as the first blind! </b> If this process continues, the controls will end up in opposing positions. Do you want to continue?"}}
                            End If
                        End If
                    End If
                End If

                If data.brackettype = "Linked 2 Blinds (Dep)" Then
                    If data.blindno = "Blind 1" Then
                        If data.itemaction = "AddItem" Then
                            If String.IsNullOrEmpty(data.controlposition) AND Not String.IsNullOrEmpty(data.chaincolour) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position is required !",.field = "controlposition"}}
                            End If
                            If Not String.IsNullOrEmpty(data.controlposition) AND String.IsNullOrEmpty(data.chaincolour) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour is required !",.field = "chaincolour"}}
                            End If
                        End If

                        If InArray(data.itemaction, "EditItem", "ViewItem") Then
                            Dim ControlB2 As String = FindControlPosition(data.uniqueid, "Blind 2")
                            IF Not String.IsNullOrEmpty(ControlB2) Then
                                If Not String.IsNullOrEmpty(data.controlposition) AndAlso Not data.isConfirmed Then
                                    Return New ConfirmResponse With { .confirm = New ConfirmDetail With { .message = "For linked 2 blinds dependent: <br/><br/> Blind 1 may have a control side → then Blind 2 must remain empty.<br/> Alternatively, Blind 2 may have a control side → then Blind 1 must remain empty. Switching control is only allowed under these rules. Do you want to continue switching?"}}
                                End If
                            End If

                            IF String.IsNullOrEmpty(ControlB2) Then
                                If String.IsNullOrEmpty(data.controlposition) Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position is required !",.field = "controlposition"}}
                                End If
                                If String.IsNullOrEmpty(data.chaincolour) Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour is required !",.field = "chaincolour"}}
                                End If
                                Dim chainlength As Integer
                                If Not String.IsNullOrEmpty(data.chainlength) Then
                                    If Not Integer.TryParse(data.chainlength, chainlength) OrElse chainlength <= 0 Then
                                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain length must be a positive integer !",.field = "chainlength"}}
                                    End If
                                End If
                            End If
                        End If
                    End If

                    If data.blindno = "Blind 2" Then
                        Dim ControlB1 As String = FindControlPosition(data.uniqueid, "Blind 1")
                        If Not String.IsNullOrEmpty(ControlB1) Then
                            If Not String.IsNullOrEmpty(data.controlposition) AndAlso Not data.isConfirmed Then
                                Return New ConfirmResponse With { .confirm = New ConfirmDetail With { .message = "For linked 2 blinds dependent: <br/><br/> Blind 1 may have a control side → then Blind 2 must remain empty.<br/> Alternatively, Blind 2 may have a control side → then Blind 1 must remain empty. Switching control is only allowed under these rules. Do you want to continue switching?"}}
                            End If
                        End If

                        IF String.IsNullOrEmpty(ControlB1) Then
                            If String.IsNullOrEmpty(data.controlposition) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position is required !",.field = "controlposition"}}
                            End If
                            If String.IsNullOrEmpty(data.chaincolour) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour is required !",.field = "chaincolour"}}
                            End If
                            Dim chainlength As Integer
                            If Not String.IsNullOrEmpty(data.chainlength) Then
                                If Not Integer.TryParse(data.chainlength, chainlength) OrElse chainlength <= 0 Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain length must be a positive integer !",.field = "chainlength"}}
                                End If
                            End If
                        End if

                    End If

                End If
                
                If data.brackettype = "Linked 3 Blinds (Dep)" Then
                    If data.blindno = "Blind 1" Then
                        If data.itemaction = "AddItem" Then
                            If String.IsNullOrEmpty(data.controlposition) AND Not String.IsNullOrEmpty(data.chaincolour) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position is required !",.field = "controlposition"}}
                            End If
                            If Not String.IsNullOrEmpty(data.controlposition) AND String.IsNullOrEmpty(data.chaincolour) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour is required !",.field = "chaincolour"}}
                            End If
                        End If

                        If InArray(data.itemaction, "EditItem", "ViewItem") Then
                            Dim ControlB3 As String = FindControlPosition(data.uniqueid, "Blind 3")
                            IF Not String.IsNullOrEmpty(ControlB3) Then
                                If Not String.IsNullOrEmpty(data.controlposition) AndAlso Not data.isConfirmed Then
                                    Return New ConfirmResponse With { .confirm = New ConfirmDetail With { .message = "For linked 3 blinds dependent: <br/><br/> Blind 1 may have a control side → then Blind 2 and Blind 3 must remain empty.<br/> Alternatively, Blind 3 may have a control side → then Blind 1 and Blind 2 must remain empty. Switching control is only allowed under these rules. Do you want to continue switching?"}}
                                End If
                            End If

                            IF String.IsNullOrEmpty(ControlB3) Then
                                If String.IsNullOrEmpty(data.controlposition) Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position is required !",.field = "controlposition"}}
                                End If
                                If String.IsNullOrEmpty(data.chaincolour) Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour is required !",.field = "chaincolour"}}
                                End If
                                Dim chainlength As Integer
                                If Not String.IsNullOrEmpty(data.chainlength) Then
                                    If Not Integer.TryParse(data.chainlength, chainlength) OrElse chainlength <= 0 Then
                                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain length must be a positive integer !",.field = "chainlength"}}
                                    End If
                                End If
                            End If
                        End If
                    End If

                    If data.blindno = "Blind 2" Then
                        If Not String.IsNullOrEmpty(data.controlposition) Then
                            Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position not required !",.field = "controlposition"}}
                        End If
                        If Not String.IsNullOrEmpty(data.chaincolour) Then
                            Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour not required !",.field = "chaincolour"}}
                        End If
                        If Not String.IsNullOrEmpty(data.chainlength) Then
                            Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain length not required !",.field = "chainlength"}}
                        End If
                    End If

                    If data.blindno = "Blind 3" Then
                        Dim ControlB1 As String = FindControlPosition(data.uniqueid, "Blind 1")
                        If Not String.IsNullOrEmpty(ControlB1) Then
                            If Not String.IsNullOrEmpty(data.controlposition) AndAlso Not data.isConfirmed Then
                                Return New ConfirmResponse With { .confirm = New ConfirmDetail With { .message = "For linked 3 blinds dependent: <br/><br/> Blind 1 may have a control side → then Blind 2 and Blind 3 must remain empty.<br/> Alternatively, Blind 3 may have a control side → then Blind 1 and Blind 2 must remain empty. Switching control is only allowed under these rules. Do you want to continue switching?"}}
                            End If
                        End If

                        IF String.IsNullOrEmpty(ControlB1) Then
                            If String.IsNullOrEmpty(data.controlposition) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position is required !",.field = "controlposition"}}
                            End If
                            If String.IsNullOrEmpty(data.chaincolour) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour is required !",.field = "chaincolour"}}
                            End If
                            Dim chainlength As Integer
                            If Not String.IsNullOrEmpty(data.chainlength) Then
                                If Not Integer.TryParse(data.chainlength, chainlength) OrElse chainlength <= 0 Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain length must be a positive integer !",.field = "chainlength"}}
                                End If
                            End If
                        End if
                    End If
                End If

                If data.brackettype = "Linked 3 Blinds (Ind)" Then
                    If String.IsNullOrEmpty(data.controlposition) Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position is required !",.field = "controlposition"}}
                    End If

                    If InArray(data.blindno, "Blind 1", "Blind 3") Then
                        If String.IsNullOrEmpty(data.chaincolour) Then
                            Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour is required !",.field = "chaincolour"}}
                        End If
                        Dim chainlength As Integer
                        If Not String.IsNullOrEmpty(data.chainlength) Then
                            If Not Integer.TryParse(data.chainlength, chainlength) OrElse chainlength <= 0 Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain length must be a positive integer !",.field = "chainlength"}}
                            End If
                        End If
                    End If
                    
                    If data.blindno = "Blind 1" Then
                        If InArray(data.itemaction, "EditItem", "ViewItem") Then
                            Dim ControlB3 As String = FindControlPosition(data.uniqueid, "Blind 3")
                            If data.controlposition = ControlB3 AndAlso Not data.isConfirmed Then
                                Return New ConfirmResponse With { .confirm = New ConfirmDetail With { .message = "For linked 3 blinds independent: <b>The control position cannot be the same as the third blind! </b> If this process continues, the controls will end up in opposing positions. Do you want to continue?"}}
                            End If
                        End If
                    End If

                    If data.blindno = "Blind 3" Then
                        If InArray(data.itemaction, "NextItem", "EditItem", "ViewItem") Then
                            Dim ControlB1 As String = FindControlPosition(data.uniqueid, "Blind 1")
                            If data.controlposition = ControlB1 AndAlso Not data.isConfirmed Then
                                 Return New ConfirmResponse With { .confirm = New ConfirmDetail With { .message = "For linked 3 blinds independent: <b>The control position cannot be the same as the first blind! </b> If this process continues, the controls will end up in opposing positions. Do you want to continue?"}}
                            End If                    
                        End If
                    End If
                End If

                If data.brackettype = "Double and Link System Dep" Then
                    If data.blindno = "Blind 1" Then
                        If data.itemaction = "AddItem" Then
                            If String.IsNullOrEmpty(data.controlposition) AND Not String.IsNullOrEmpty(data.chaincolour) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position is required !",.field = "controlposition"}}
                            End If
                            If Not String.IsNullOrEmpty(data.controlposition) AND String.IsNullOrEmpty(data.chaincolour) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour is required !",.field = "chaincolour"}}
                            End If
                            Dim chainlength As Integer
                            If Not String.IsNullOrEmpty(data.chainlength) Then
                                If Not Integer.TryParse(data.chainlength, chainlength) OrElse chainlength <= 0 Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain length must be a positive integer !",.field = "chainlength"}}
                                End If
                            End If
                        End If

                        If data.itemaction = "EditItem" Then
                            Dim controlposition As String = publicCfg.GetItemData(String.Format("SELECT ControlPosition FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId='{0}' AND Active = 1", data.uniqueid))
                            IF Not controlposition = "" Then
                                If Not String.IsNullOrEmpty(data.controlposition) Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position not required !",.field = "controlposition"}}
                                End If
                                If Not String.IsNullOrEmpty(data.chaincolour) Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour not required !",.field = "chaincolour"}}
                                End If
                                If Not String.IsNullOrEmpty(data.chainlength) Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain length not required !",.field = "chainlength"}}
                                End If
                            End If

                            If controlposition = "" Then
                                If String.IsNullOrEmpty(data.controlposition) Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position is required !",.field = "controlposition"}}
                                End If
                                If String.IsNullOrEmpty(data.chaincolour) Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour is required !",.field = "chaincolour"}}
                                End If
                                Dim chainlength As Integer
                                If Not String.IsNullOrEmpty(data.chainlength) Then
                                    If Not Integer.TryParse(data.chainlength, chainlength) OrElse chainlength <= 0 Then
                                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain length must be a positive integer !",.field = "chainlength"}}
                                    End If
                                End If
                            End If

                        End If
                    End If

                    If data.blindno = "Blind 2" Then
                        Dim controlposition As String = publicCfg.GetItemData(String.Format("SELECT ControlPosition FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId='{0}' AND Active = 1", data.uniqueid))
                        If Not controlposition = "" Then
                            If Not String.IsNullOrEmpty(data.controlposition) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position not required !",.field = "controlposition"}}
                            End If
                            If Not String.IsNullOrEmpty(data.chaincolour) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour not required !",.field = "chaincolour"}}
                            End If
                            If Not String.IsNullOrEmpty(data.chainlength) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain length not required !",.field = "chainlength"}}
                            End If
                        End If

                        If controlposition = "" Then
                            If String.IsNullOrEmpty(data.controlposition) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position is required !",.field = "controlposition"}}
                            End If
                            If String.IsNullOrEmpty(data.chaincolour) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour is required !",.field = "chaincolour"}}
                            End If
                            Dim chainlength As Integer
                            If Not String.IsNullOrEmpty(data.chainlength) Then
                                If Not Integer.TryParse(data.chainlength, chainlength) OrElse chainlength <= 0 Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain length must be a positive integer !",.field = "chainlength"}}
                                End If
                            End If
                        End If

                    End If

                    If data.blindno = "Blind 3" Then
                        If data.itemaction = "AddItem" Then
                            If String.IsNullOrEmpty(data.controlposition) AND Not String.IsNullOrEmpty(data.chaincolour) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position is required !",.field = "controlposition"}}
                            End If
                            If Not String.IsNullOrEmpty(data.controlposition) AND String.IsNullOrEmpty(data.chaincolour) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour is required !",.field = "chaincolour"}}
                            End If
                        End If

                        IF data.itemaction = "EditItem" Then
                            Dim controlposition As String = publicCfg.GetItemData(String.Format("SELECT ControlPosition FROM OrderDetails WHERE BlindNo = 'Blind 4' AND UniqueId='{0}' AND Active = 1", data.uniqueid))
                            If Not controlposition = "" Then
                                If Not String.IsNullOrEmpty(data.controlposition) Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position not required !",.field = "controlposition"}}
                                End If
                                If Not String.IsNullOrEmpty(data.chaincolour) Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour not required !",.field = "chaincolour"}}
                                End If
                                If Not String.IsNullOrEmpty(data.chainlength) Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain length not required !",.field = "chainlength"}}
                                End If
                            End If

                            If controlposition = "" Then
                                If String.IsNullOrEmpty(data.controlposition) Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position is required !",.field = "controlposition"}}
                                End If
                                If String.IsNullOrEmpty(data.chaincolour) Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour is required !",.field = "chaincolour"}}
                                End If
                                Dim chainlength As Integer
                                If Not String.IsNullOrEmpty(data.chainlength) Then
                                    If Not Integer.TryParse(data.chainlength, chainlength) OrElse chainlength <= 0 Then
                                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain length must be a positive integer !",.field = "chainlength"}}
                                    End If
                                End If
                            End If

                        End If
                    End If 

                    If data.blindno = "Blind 4" Then
                        Dim controlposition As String = publicCfg.GetItemData(String.Format("SELECT ControlPosition FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId='{0}' AND Active = 1", data.uniqueid))
                        If Not controlposition = "" Then
                            If Not String.IsNullOrEmpty(data.controlposition) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position not required !",.field = "controlposition"}}
                            End If
                            If Not String.IsNullOrEmpty(data.chaincolour) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour not required !",.field = "chaincolour"}}
                            End If
                            If Not String.IsNullOrEmpty(data.chainlength) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain length not required !",.field = "chainlength"}}
                            End If
                        End If

                        If controlposition = "" Then
                            If String.IsNullOrEmpty(data.controlposition) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position is required !",.field = "controlposition"}}
                            End If
                            If String.IsNullOrEmpty(data.chaincolour) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour is required !",.field = "chaincolour"}}
                            End If
                            Dim chainlength As Integer
                            If Not String.IsNullOrEmpty(data.chainlength) Then
                                If Not Integer.TryParse(data.chainlength, chainlength) OrElse chainlength <= 0 Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain length must be a positive integer !",.field = "chainlength"}}
                                End If
                            End If
                        End If
                    End If
                End If

                If data.brackettype = "Double and Link System Ind" Then
                    If data.blindno = "Blind 1" Then
                        If data.itemaction = "AddItem" Then
                            If String.IsNullOrEmpty(data.controlposition) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position is required !",.field = "controlposition"}}
                            End If
                            If String.IsNullOrEmpty(data.chaincolour) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour is required !",.field = "chaincolour"}}
                            End If
                            Dim chainlength As Integer
                            If Not String.IsNullOrEmpty(data.chainlength) Then
                                If Not Integer.TryParse(data.chainlength, chainlength) OrElse chainlength <= 0 Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain length must be a positive integer !",.field = "chainlength"}}
                                End If
                            End If
                        End If

                        If data.itemaction = "EditItem" Then
                            Dim controlposition As String = publicCfg.GetItemData(String.Format("SELECT ControlPosition FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId='{0}' AND Active = 1", data.uniqueid))
                            Dim controlpositionVar As String = data.controlposition
                            If String.IsNullOrEmpty(data.controlposition) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position is required !",.field = "controlposition"}}
                            End If
                            If String.IsNullOrEmpty(data.chaincolour) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour is required !",.field = "chaincolour"}}
                            End If
                            Dim chainlength As Integer
                            If Not String.IsNullOrEmpty(data.chainlength) Then
                                If Not Integer.TryParse(data.chainlength, chainlength) OrElse chainlength <= 0 Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain length must be a positive integer !",.field = "chainlength"}}
                                End If
                            End If
                            If Not controlposition = "" Then
                                If controlposition = controlpositionVar Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position not allowed to change !",.field = "controlposition"}}
                                End If
                            End If
                        End If
                    End If

                    If data.blindno = "Blind 2" Then
                        If InArray(data.itemaction, "EditItem", "NextItem") Then
                            Dim controlposition As String = publicCfg.GetItemData(String.Format("SELECT ControlPosition FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId='{0}' AND Active = 1", data.uniqueid))
                            Dim controlpositionVar As String = data.controlposition
                            If String.IsNullOrEmpty(data.controlposition) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position is required !",.field = "controlposition"}}
                            End If
                            If String.IsNullOrEmpty(data.chaincolour) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour is required !",.field = "chaincolour"}}
                            End If
                            Dim chainlength As Integer
                            If Not String.IsNullOrEmpty(data.chainlength) Then
                                If Not Integer.TryParse(data.chainlength, chainlength) OrElse chainlength <= 0 Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain length must be a positive integer !",.field = "chainlength"}}
                                End If
                            End If
                            If Not controlposition = "" Then
                                If controlposition = controlpositionVar Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position not allowed to change !",.field = "controlposition"}}
                                End If
                            End If
                        End If
                    End If

                    If data.blindno = "Blind 3" Then
                        If InArray(data.itemaction, "EditItem", "NextItem") Then
                            Dim controlposition As String = publicCfg.GetItemData(String.Format("SELECT ControlPosition FROM OrderDetails WHERE BlindNo = 'Blind 4' AND UniqueId='{0}' AND Active = 1", data.uniqueid))
                            Dim controlpositionVar As String = data.controlposition
                            If String.IsNullOrEmpty(data.controlposition) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position is required !",.field = "controlposition"}}
                            End If
                            If String.IsNullOrEmpty(data.chaincolour) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour is required !",.field = "chaincolour"}}
                            End If
                            Dim chainlength As Integer
                            If Not String.IsNullOrEmpty(data.chainlength) Then
                                If Not Integer.TryParse(data.chainlength, chainlength) OrElse chainlength <= 0 Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain length must be a positive integer !",.field = "chainlength"}}
                                End If
                            End If
                            If Not controlposition = "" Then
                                If controlposition = controlpositionVar Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position not allowed to change !",.field = "controlposition"}}
                                End If
                            End If
                        End If
                    End If

                    If data.blindno = "Blind 4" Then
                        If InArray(data.itemaction, "EditItem", "NextItem") Then
                            Dim controlposition As String = publicCfg.GetItemData(String.Format("SELECT ControlPosition FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId='{0}' AND Active = 1", data.uniqueid))
                            Dim controlpositionVar As String = data.controlposition
                            If String.IsNullOrEmpty(data.controlposition) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position is required !",.field = "controlposition"}}
                            End If
                            If String.IsNullOrEmpty(data.chaincolour) Then
                                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour is required !",.field = "chaincolour"}}
                            End If
                            Dim chainlength As Integer
                            If Not String.IsNullOrEmpty(data.chainlength) Then
                                If Not Integer.TryParse(data.chainlength, chainlength) OrElse chainlength <= 0 Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain length must be a positive integer !",.field = "chainlength"}}
                                End If
                            End If
                            If Not controlposition = "" Then
                                If controlposition = controlpositionVar Then
                                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position not allowed to change !",.field = "controlposition"}}
                                End If
                            End If
                        End If
                    End If
                End If
            End If

            If InArray(data.brackettype, "With Tube & Bottom Included", "With Bottom Included") Then
                If String.IsNullOrEmpty(data.trim) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "trim is required !",.field = "trim"}}
                End If

                If Not String.IsNullOrEmpty(data.trim) AND data.trim = "1F" Then
                    If String.IsNullOrEmpty(data.railtype) Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "bottom rail type is required !",.field = "railtype"}}
                    End If
                    If String.IsNullOrEmpty(data.railcolour) Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "bottom rail colour is required !",.field = "railcolour"}}
                    End If
                End If
            End If

            If Not data.tubetype = "N/A" Then
                If Not BlindName = "Skin Only" AND String.IsNullOrEmpty(data.trim) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "trim is required !",.field = "trim"}}
                End If
                
                If data.trim = "1F" AND data.tubetype = "Spring Operated" Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "trim 1f is not allowed for spring operated !",.field = "trim"}}
                End If

                If Not String.IsNullOrEmpty(data.trim) AND data.trim = "1F" Then
                    If String.IsNullOrEmpty(data.railtype) Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "bottom rail type is required !",.field = "railtype"}}
                    End If
                    If String.IsNullOrEmpty(data.railcolour) Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "bottom rail colour is required !",.field = "railcolour"}}
                    End If
                End If
            End If
            


            If (BlindName = "Skin Only" AND InStr(data.brackettype, "Tube") > 0 ) OR BlindName = "Standard" OR (BlindName = "Cassette" AND Not data.tubetype = "Motorised") Then
                If String.IsNullOrEmpty(data.tubesize) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "tube size is required !",.field = "tubesize"}}
                End If
            End If

            If Not String.IsNullOrEmpty(data.bracketcovers) Then
                If String.IsNullOrEmpty(data.bracketcovercolours) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "bracket cover colour is required !",.field = "bracketcovercolours"}}
                End If
            End If

            If Not String.IsNullOrEmpty(data.notes) Then
                If InStr(data.notes, "&") > 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "notes must not contain [&] character !",.field = "notes"}}
                End If

                If data.notes.Trim().Length > 1000 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "notes must be less than 1000 characters !",.field = "notes"}}
                End If
            End If

            Dim markup As Integer
            If Not String.IsNullOrEmpty(data.markup) Then
                If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "please check your markup !",.field = "markup"}}
                End If
            End If



            If String.IsNullOrEmpty(data.markup) Then
                data.markup = "0"
            End If

            
            Dim SoeId As String = publicCfg.GetSoeKitId(data.colourtype)
            Dim DesignName As String = publicCfg.GetDesignName(data.designid)
            Dim ExactName As String = String.Format("{0} - {1}", DesignName, BlindName)
            Dim ExactId As String = orderCfg.GetItemData(String.Format("SELECT ExactId FROM Exacts WHERE Name = '{0}'", ExactName))

            Dim FabricGroup As String = publicCfg.GetFabricGroup(data.fabriccolour)
            Dim PriceGroupName As String = String.Format("Roller Blind - {0}", FabricGroup)
            If BlindName = "Skin Only" Then
                PriceGroupName = String.Format("Roller Skin Only - {0}", FabricGroup)
            End If

            Dim PriceGroupId As String = publicCfg.GetPriceGroupId(data.designid, PriceGroupName)
            Dim CassetteExtraId As String = ""
            If BlindName = "Cassette" Then
                Dim CassetteExtraName As String = String.Format("{0} - {1}", data.brackettype, data.mounting)
                If data.brackettype = "Headbox Only" Then
                    CassetteExtraName = data.brackettype
                End If
                ' PriceGroupId = publicCfg.GetPriceGroupId(data.designid, CassetteExtraName)
                CassetteExtraId = publicCfg.GetPriceGroupId(data.designid, CassetteExtraName)
            End If

            If PriceGroupId = "" Then
                Throw New Exception("Something went wrong !")
            End If

            
            


            Dim ChainId As String = ""
            Dim CLength As String = data.chainlength
            If data.controltype = "Chain" AND Not String.IsNullOrEmpty(data.chaincolour) Then
                Dim ChainColour As String = String.format("({0})", data.chaincolour)

                If String.IsNullOrEmpty(data.chainlength) Or data.chainlength = "0" Then
                    If drop >= 3000 Then
                        CLength = "2200"
                    ElseIf drop >= 2700 Then
                        CLength = "2000"
                    ElseIf drop >= 2400 Then
                        CLength = "1800"
                    ElseIf drop >= 2000 Then
                        CLength = "1500"
                    ElseIf drop >= 1600 Then
                        CLength = "1250"
                    ElseIf drop >= 1300 Then
                        CLength = "1000"
                    ElseIf drop >= 1100 Then
                        CLength = "800"
                    ElseIf drop >= 800 Then
                        CLength = "600"
                    Else
                        CLength = "500"
                    End If
                End If

                If Not String.IsNullOrEmpty(data.chainlength) Then
                    CLength  = "500"
                    If data.chainlength > 500 Then : CLength = "600" : End If
                    If data.chainlength > 600 Then : CLength = "800" : End If
                    If data.chainlength > 800 Then : CLength = "1000" : End If
                    If data.chainlength > 1000 Then : CLength = "1250" : End If
                    If data.chainlength > 1250 Then : CLength = "1500" : End If
                    If data.chainlength > 1500 Then : CLength = "1800" : End If
                    If data.chainlength > 1800 Then : CLength = "2000" : End If
                    If data.chainlength > 2000 Then : CLength = "2200" : End If
                    If data.chainlength > 2200 Then : CLength = "2500" : End If
                End If

                Dim ChainName As String = String.Format("{0} Chain + Joiner {1}", CLength, ChainColour)
                ChainId = publicCfg.GetItemData(String.Format("SELECT Id FROM Chains WHERE Name = '{0}'", ChainName))

                If String.IsNullOrEmpty(ChainId) Then
                    ChainName = String.Format("Custom Chain + Joiner {0}", ChainColour)
                    ChainId = publicCfg.GetItemData(String.Format("SELECT Id FROM Chains WHERE Name = '{0}'", ChainName))
                End If
                
                If String.IsNullOrEmpty(data.chainlength) OR data.chainlength = "0" Then : data.chainlength = CLength : End If
                If Not String.IsNullOrEmpty(data.chainlength) Then : data.chainlength = data.chainlength : End If
                ' Throw New Exception(data.chainlength)


                data.motorstyle = ""
                data.externalbattery = ""
                data.charger = ""
                data.cableexitpoint = ""
                data.connector = ""
            End If

            Dim BottomRailId As String = ""
            If Not String.IsNullOrEmpty(data.railcolour) Then
                BottomRailId = data.railcolour
            End If

            If InStr(data.controltype, "Somfy") > 0 Or InStr(data.controltype, "Alpha") > 0 Then
                data.chaincolour = ""
                data.chainlength = ""
                ChainId = ""
                CLength = ""

                If data.brackettype = "Headbox & Side Channels" Then
                    data.connector = ""
                End If

                If BlindName = "Motorised" Or (data.controltype = "Alpha WF" Or data.controltype = "Somfy WF") Then
                    data.cableexitpoint = ""
                End If
            End If

            If Not InArray(BlindName, "Standard", "Motorised", "Cassette") Then
                data.sizetype = ""
                data.dropfloor = ""
            End If
            data.sizetype = ""
            data.dropfloor = ""

            ' If data.trim = "1F" Then
            '     data.accessory = ""
            ' End If

            
            
            ' Return New ErrorResponse With {.error = New ErrorDetail With {.message = data.uniqueid, .field = ""}}
            
            
            Dim msg As String = "200"
            If data.itemaction = "AddItem" OrElse data.itemaction = "CopyItem" Then
                Dim ItemId As String = publicCfg.CreateOrderItemId()
                data.uniqueid = ""

                If data.brackettype = "Double" Or InStr(data.brackettype, "Linked") > 0 Or InStr(data.brackettype, "Link") > 0 Then
                    data.uniqueid = GenerateUniqueId()
                End If

                ' Return New ErrorResponse With {.error = New ErrorDetail With {.message = data.uniqueid, .field = ""}}


                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, UniqueId, BlindNo, KitId, SoeKitId, ExactId, FabricId, ChainId, BottomRailId, PriceGroupId, CassetteExtraId, Qty, Location, LouvreSize, LouvrePosition, Mounting, Width, [Drop], RollDirection, ControlPosition, ChainLength, Accessory, TubeSize, Trim, BracketCover, BracketColour, BracketExtension, ChildSafe, MotorStyle, MotorRemote, MotorBattery, MotorCharger, Connector, AdditionalMotor, CableExitPoint, Notes, Matrix, Charge, TotalMatrix, TotalCharge, MarkUp, Active) VALUES (@Id, @HeaderId, @UniqueId, @BlindNo, @KitId, @SoeKitId, @ExactId, @FabricId, @ChainId, @BottomRailId, @PriceGroupId, @CassetteExtraId, @Qty, @Location, @LouvreSize, @LouvrePosition, @Mounting, @Width, @Drop, @RollDirection, @ControlPosition, @ChainLength, @Accessory, @TubeSize, @Trim, @BracketCover, @BracketColour, @BracketExtension, @ChildSafe, @MotorStyle, @MotorRemote, @MotorBattery, @MotorCharger, @Connector, @AdditionalMotor, @CableExitPoint, @Notes, 0.00, 0.00, 0.00, 0.00, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", ItemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@UniqueId", If(String.IsNullOrEmpty(data.uniqueid), DBNull.Value, data.uniqueid))
                        myCmd.Parameters.AddWithValue("@BlindNo", data.blindno)
                        myCmd.Parameters.AddWithValue("@KitId", If(String.IsNullOrEmpty(data.colourtype), DBNull.Value, UCase(data.colourtype).ToString()))
                        myCmd.Parameters.AddWithValue("@SoeKitId", If(String.IsNullOrEmpty(SoeId), DBNull.Value, SoeId))
                        myCmd.Parameters.AddWithValue("@ExactId", If(String.IsNullOrEmpty(ExactId), DBNull.Value, ExactId))
                        myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabriccolour), DBNull.Value, UCase(data.fabriccolour).ToString()))
                        myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(ChainId), DBNull.Value, ChainId))
                        myCmd.Parameters.AddWithValue("@BottomRailId", If(String.IsNullOrEmpty(BottomRailId), DBNull.Value, BottomRailId))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", If(String.IsNullOrEmpty(PriceGroupId), DBNull.Value, PriceGroupId))
                        myCmd.Parameters.AddWithValue("@CassetteExtraId", If(String.IsNullOrEmpty(CassetteExtraId), DBNull.Value, CassetteExtraId))
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@LouvreSize", data.sizetype)
                        myCmd.Parameters.AddWithValue("@LouvrePosition", data.dropfloor)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@RollDirection", data.roll)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@ChainLength", If(String.IsNullOrEmpty(data.chainlength), DBNull.Value, data.chainlength))
                        myCmd.Parameters.AddWithValue("@Accessory", data.accessory)
                        myCmd.Parameters.AddWithValue("@TubeSize", data.tubesize)
                        myCmd.Parameters.AddWithValue("@Trim", data.trim)
                        myCmd.Parameters.AddWithValue("@BracketCover", data.bracketcovers)
                        myCmd.Parameters.AddWithValue("@BracketColour", data.bracketcovercolours)
                        myCmd.Parameters.AddWithValue("@BracketExtension", data.bracketext)
                        myCmd.Parameters.AddWithValue("@ChildSafe", data.childsafe)
                        myCmd.Parameters.AddWithValue("@MotorStyle", data.motorstyle)
                        myCmd.Parameters.AddWithValue("@MotorRemote", data.motorremote)
                        myCmd.Parameters.AddWithValue("@MotorBattery", data.externalbattery)
                        myCmd.Parameters.AddWithValue("@MotorCharger", data.charger)
                        myCmd.Parameters.AddWithValue("@Connector", data.connector)
                        myCmd.Parameters.AddWithValue("@AdditionalMotor", data.extras)
                        myCmd.Parameters.AddWithValue("@CableExitPoint", data.cableexitpoint)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                publicCfg.ResetPriceDetail(ItemId)
                publicCfg.HitungHarga(data.headerid, ItemId)
                publicCfg.HitungSurcharge(data.headerid, ItemId)

                Dim dataLog As Object() = {data.headerid, ItemId, "Blinds", data.loginid, "Add Item Order"}
                orderCfg.Log_Orders(dataLog)

                msg = "Item added successfully !"

                If data.brackettype = "Double" Or InStr(data.brackettype, "Linked") > 0 Or InStr(data.brackettype, "Link") > 0 Then
                    Dim BlindNoSelected As String = "first blind"
                    If data.blindno = "Blind 2" Then
                        BlindNoSelected = "second blind"
                    End If

                    msg += String.Format("<br/><br/> This is the <b>{0}</b>.", BlindNoSelected)
                    msg += String.Format("<br/> from <b>{0}</b> - <b>{1}</b>", BlindName, data.brackettype)
                    msg += String.Format("<br /><br />Please click the <b>Next Item</b> button that is written in green color of the <b>ITEM ID {0}</b>.", ItemId)
                End If

                If InStr(data.brackettype, "Linked") > 0 AND data.controltype = "Somfy WF" Then
                    msg += "<br/><br/><b>Warning :</b>Check SP the availability for linking blind for WF motorised !"
                End If
                If InStr(data.brackettype, "Linked") > 0 AND data.controltype = "Alpha WF" AndAlso data.motorstyle = "Alpha 2NM Std" Then
                    msg += "<br/><br/><b>Warning :</b> Check SP the availability for linking blind for WF motorised !"
                End If

                ' Return New ErrorResponse With {.error = New ErrorDetail With {.message = msg, .field = ""}}

                
            End If

            If data.itemaction = "NextItem" Then
                Dim ItemId As String = publicCfg.CreateOrderItemId()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, UniqueId, BlindNo, KitId, SoeKitId, ExactId, FabricId, ChainId, BottomRailId, PriceGroupId, CassetteExtraId, Qty, Location, LouvreSize, LouvrePosition, Mounting, Width, [Drop], RollDirection, ControlPosition, ChainLength, Accessory, TubeSize, Trim, BracketCover, BracketColour, BracketExtension, ChildSafe, MotorStyle, MotorRemote, MotorBattery, MotorCharger, Connector, AdditionalMotor, CableExitPoint, Notes, Matrix, Charge, TotalMatrix, TotalCharge, MarkUp, Active) VALUES (@Id, @HeaderId, @UniqueId, @BlindNo, @KitId, @SoeKitId, @ExactId, @FabricId, @ChainId, @BottomRailId, @PriceGroupId, @CassetteExtraId, @Qty, @Location, @LouvreSize, @LouvrePosition, @Mounting, @Width, @Drop, @RollDirection, @ControlPosition, @ChainLength, @Accessory, @TubeSize, @Trim, @BracketCover, @BracketColour, @BracketExtension, @ChildSafe, @MotorStyle, @MotorRemote, @MotorBattery, @MotorCharger, @Connector, @AdditionalMotor, @CableExitPoint, @Notes, 0.00, 0.00, 0.00, 0.00, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@UniqueId", data.uniqueid)
                        myCmd.Parameters.AddWithValue("@BlindNo", data.blindno)
                        myCmd.Parameters.AddWithValue("@KitId", If(String.IsNullOrEmpty(data.colourtype), DBNull.Value, UCase(data.colourtype).ToString()))
                        myCmd.Parameters.AddWithValue("@SoeKitId", If(String.IsNullOrEmpty(SoeId), DBNull.Value, SoeId))
                        myCmd.Parameters.AddWithValue("@ExactId", If(String.IsNullOrEmpty(ExactId), DBNull.Value, ExactId))
                        myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabriccolour), DBNull.Value, UCase(data.fabriccolour).ToString()))
                        myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(ChainId), DBNull.Value, ChainId))
                        myCmd.Parameters.AddWithValue("@BottomRailId", If(String.IsNullOrEmpty(BottomRailId), DBNull.Value, BottomRailId))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", If(String.IsNullOrEmpty(PriceGroupId), DBNull.Value, PriceGroupId))
                        myCmd.Parameters.AddWithValue("@CassetteExtraId", If(String.IsNullOrEmpty(CassetteExtraId), DBNull.Value, CassetteExtraId))
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@LouvreSize", data.sizetype)
                        myCmd.Parameters.AddWithValue("@LouvrePosition", data.dropfloor)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@RollDirection", data.roll)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@ChainLength", If(String.IsNullOrEmpty(data.chainlength), DBNull.Value, data.chainlength))
                        myCmd.Parameters.AddWithValue("@Accessory", data.accessory)
                        myCmd.Parameters.AddWithValue("@TubeSize", data.tubesize)
                        myCmd.Parameters.AddWithValue("@Trim", data.trim)
                        myCmd.Parameters.AddWithValue("@BracketCover", data.bracketcovers)
                        myCmd.Parameters.AddWithValue("@BracketColour", data.bracketcovercolours)
                        myCmd.Parameters.AddWithValue("@BracketExtension", data.bracketext)
                        myCmd.Parameters.AddWithValue("@ChildSafe", data.childsafe)
                        myCmd.Parameters.AddWithValue("@MotorStyle", data.motorstyle)
                        myCmd.Parameters.AddWithValue("@MotorRemote", data.motorremote)
                        myCmd.Parameters.AddWithValue("@MotorBattery", data.externalbattery)
                        myCmd.Parameters.AddWithValue("@MotorCharger", data.charger)
                        myCmd.Parameters.AddWithValue("@Connector", data.connector)
                        myCmd.Parameters.AddWithValue("@AdditionalMotor", data.extras)
                        myCmd.Parameters.AddWithValue("@CableExitPoint", data.cableexitpoint)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                If data.brackettype = "Double" Then
                    '#SdsNext
                    Dim ListNext As New List(Of Object) From {
                        data.uniqueid,
                        data.tubesize,
                        data.mounting,
                        data.room,
                        data.childsafe,
                        data.accessory,
                        data.bracketcovers,
                        data.bracketext,
                        data.motorstyle,
                        markup
                    }
                    Dim ResNext As String = SdsNext(ListNext)
                    IF Not ResNext = "200" Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResNext, .field = ""}}
                    End If

                    '#SdsSize
                    Dim ListSize As New List(Of Object) From {
                        data.uniqueid,
                        width,
                        drop
                    }
                    Dim ResSize As String = SdsSize(ListSize)
                    IF Not ResSize = "200" Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResSize, .field = ""}}
                    End If
                End If

                If InArray(data.brackettype, "Linked 2 Blinds (Dep)", "Linked 3 Blinds (Dep)") Then
                    '#SdsDrop
                    Dim ListDrop As New List(Of Object) From {
                        data.uniqueid,
                        drop
                    }
                    Dim ResDrop As String = SdsDrop(ListDrop)
                    IF Not ResDrop = "200" Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResDrop, .field = ""}}
                    End If

                    '#SdsRollDep
                    Dim ListRollDep As New List(Of Object) From {
                        data.uniqueid,
                        data.roll
                    }
                    Dim ResRollDep As String = SdsRollDep(ListRollDep)
                    IF Not ResRollDep = "200" Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResRollDep, .field = ""}}
                    End If

                    '#SdsFabric
                    Dim ListFabric As New List(Of Object) From {
                        data.uniqueid,
                        data.fabriccolour,
                        PriceGroupId
                    }
                    Dim ResFabric As String = SdsFabric(ListFabric)
                    IF Not ResFabric = "200" Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResFabric, .field = ""}}
                    End If

                    '#SdsNext
                    Dim ListNext As New List(Of Object) From {
                        data.uniqueid,
                        data.tubesize,
                        data.mounting,
                        data.room,
                        data.childsafe,
                        data.accessory,
                        data.bracketcovers,
                        data.bracketext,
                        data.motorstyle,
                        markup
                    }
                    Dim ResNext As String = SdsNext(ListNext)
                    IF Not ResNext = "200" Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResNext, .field = ""}}
                    End If

                    If data.brackettype = "Linked 2 Blinds (Dep)" AndAlso data.isConfirmed Then
                        '#SdsControlLink3Ind
                        Dim ListControl As New List(Of Object) From {
                            ItemId,
                            data.uniqueid,
                            data.blindno
                        }
                        
                        Dim ResControl As String = SdsControlLink2Dep(ListControl)
                        IF Not ResControl = "200" Then
                            Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResControl, .field = ""}}
                        End If
                    End If

                    If data.brackettype = "Linked 3 Blinds (Dep)" AndAlso data.isConfirmed Then
                        '#SdsControlLink3Ind
                        Dim ListControl As New List(Of Object) From {
                            ItemId,
                            data.uniqueid,
                            data.blindno
                        }
                        
                        Dim ResControl As String = SdsControlLink3Dep(ListControl)
                        IF Not ResControl = "200" Then
                            Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResControl, .field = ""}}
                        End If
                    End If


                End If

                If InArray(data.brackettype, "Linked 2 Blinds (Ind)", "Linked 3 Blinds (Ind)") Then
                    '#SdsNext
                    Dim ListNext As New List(Of Object) From {
                        data.uniqueid,
                        data.tubesize,
                        data.mounting,
                        data.room,
                        data.childsafe,
                        data.accessory,
                        data.bracketcovers,
                        data.bracketext,
                        data.motorstyle,
                        markup
                    }
                    Dim ResNext As String = SdsNext(ListNext)
                    IF Not ResNext = "200" Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResNext, .field = ""}}
                    End If

                    '#SdsFabric
                    Dim ListFabric As New List(Of Object) From {
                        data.uniqueid,
                        data.fabriccolour,
                        PriceGroupId
                    }
                    Dim ResFabric As String = SdsFabric(ListFabric)
                    IF Not ResFabric = "200" Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResFabric, .field = ""}}
                    End If

                    If data.brackettype = "Linked 2 Blinds (Ind)" AndAlso data.isConfirmed Then
                        '#SdsControlLink3Ind
                        Dim ListControl As New List(Of Object) From {
                            ItemId,
                            data.uniqueid,
                            data.blindno
                        }
                        
                        Dim ResControl As String = SdsControlLink2Ind(ListControl)
                        IF Not ResControl = "200" Then
                            Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResControl, .field = ""}}
                        End If
                    End If

                    If data.brackettype = "Linked 3 Blinds (Ind)" AndAlso data.isConfirmed Then
                        '#SdsControlLink3Ind
                        Dim ListControl As New List(Of Object) From {
                            ItemId,
                            data.uniqueid,
                            data.blindno
                        }
                        
                        Dim ResControl As String = SdsControlLink3Ind(ListControl)
                        IF Not ResControl = "200" Then
                            Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResControl, .field = ""}}
                        End If
                    End If

                End If

                If data.brackettype = "Double and Link System Dep" Then
                    '#SdsDrop
                    Dim ListDrop As New List(Of Object) From {
                        data.uniqueid,
                        drop
                    }
                    Dim ResDrop As String = SdsDrop(ListDrop)
                    IF Not ResDrop = "200" Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResDrop, .field = ""}}
                    End If

                    If data.blindno = "Blind 2" Then
                        '#SdsDB2First
                        Dim ListDB2DepFirst As New List(Of Object) From {
                            data.uniqueid,
                            data.fabriccolour,
                            PriceGroupId,
                            data.roll
                        }
                        Dim ResDB2DepFirst As String = SdsDB2First(ListDB2DepFirst)
                        IF Not ResDB2DepFirst = "200" Then
                            Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResDB2DepFirst, .field = ""}}
                        End If
                    End If

                    If data.blindno = "Blind 4" Then
                        '#SdsDB2Second
                        Dim ListDB2DepSecond As New List(Of Object) From {
                            data.uniqueid,
                            data.fabriccolour,
                            PriceGroupId,
                            data.roll
                        }
                        Dim ResDB2DepSecond As String = SdsDB2Second(ListDB2DepSecond)
                        IF Not ResDB2DepSecond = "200" Then
                            Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResDB2DepSecond, .field = ""}}
                        End If
                    End If
                    
                    '#SdsNext
                    Dim ListNext As New List(Of Object) From {
                        data.uniqueid,
                        data.tubesize,
                        data.mounting,
                        data.room,
                        data.childsafe,
                        data.accessory,
                        data.bracketcovers,
                        data.bracketext,
                        data.motorstyle,
                        markup
                    }
                    Dim ResNext As String = SdsNext(ListNext)
                    IF Not ResNext = "200" Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResNext, .field = ""}}
                    End If

                End If

                If data.brackettype = "Double and Link System Ind" Then

                    If data.blindno = "Blind 2" Then
                        '#SdsDB2IndFirst
                        Dim ListDB2IndFirst As New List(Of Object) From {
                            data.uniqueid,
                            data.fabriccolour,
                            PriceGroupId,
                            data.roll
                        }
                        Dim ResDB2IndFirst As String = SdsDB2First(ListDB2IndFirst)
                        IF Not ResDB2IndFirst = "200" Then
                            Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResDB2IndFirst, .field = ""}}
                        End If
                    End If

                    If data.blindno = "Blind 4" Then
                        '#SdsDB2DepSecond
                        Dim ListDB2DepSecond As New List(Of Object) From {
                            data.uniqueid,
                            data.fabriccolour,
                            PriceGroupId,
                            data.roll
                        }
                        Dim ResDB2DepSecond As String = SdsDB2Second(ListDB2DepSecond)
                        IF Not ResDB2DepSecond = "200" Then
                            Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResDB2DepSecond, .field = ""}}
                        End If
                    End If
                    
                    '#SdsNext
                    Dim ListNext As New List(Of Object) From {
                        data.uniqueid,
                        data.tubesize,
                        data.mounting,
                        data.room,
                        data.childsafe,
                        data.accessory,
                        data.bracketcovers,
                        data.bracketext,
                        data.motorstyle,
                        markup
                    }
                    Dim ResNext As String = SdsNext(ListNext)
                    IF Not ResNext = "200" Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResNext, .field = ""}}
                    End If

                End If

                publicCfg.ResetPriceDetail(ItemId)
                publicCfg.HitungHarga(data.headerid, ItemId)
                publicCfg.HitungSurcharge(data.headerid, ItemId)

                Dim dataLog As Object() = {data.headerid, ItemId, "Blinds", data.loginid, "Add Item Order"}
                orderCfg.Log_Orders(dataLog)

                If data.blindno = "Blind 2" AND InArray(data.brackettype, "Linked 3 Blinds (Dep)", "Linked 3 Blinds (Ind)", "Double and Link System Dep", "Double and Link System Ind") Then
                    Dim BlindNoSelected As String = "first blind"
                    If data.blindno = "Blind 2" Then
                        BlindNoSelected = "second blind"
                    End If

                    msg += String.Format("<br/><br/> This is the <b>{0}</b>.", BlindNoSelected)
                    msg += String.Format("<br/> from <b>{0}</b> - <b>{1}</b>", BlindName, data.brackettype)
                    msg += String.Format("<br /><br />Please click the <b>Next Item</b> button that is written in green color of the <b>ITEM ID {0}</b>.", ItemId)
                End If

                msg = "Item added successfully !"
            End If

            If data.itemaction = "EditItem" OrElse data.itemaction = "ViewItem" Then

                
                Dim ItemId As String = data.itemid
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("UPDATE OrderDetails SET BlindNo=@BlindNo, UniqueId=@UniqueId, KitId=@KitId, SoeKitId=@SoeKitId, ExactId=@ExactId, FabricId=@FabricId, ChainId=@ChainId, BottomRailId=@BottomRailId, PriceGroupId=@PriceGroupId, CassetteExtraId=@CassetteExtraId, Qty=@Qty, Location=@Location, LouvreSize=@LouvreSize, LouvrePosition=@LouvrePosition, Mounting=@Mounting, Width=@Width, [Drop]=@Drop, RollDirection=@RollDirection, ControlPosition=@ControlPosition, ChainLength=@ChainLength, Accessory=@Accessory, TubeSize=@TubeSize, Trim=@Trim, BracketCover=@BracketCover, BracketColour=@BracketColour, BracketExtension=@BracketExtension, ChildSafe=@ChildSafe, MotorStyle=@MotorStyle, MotorRemote=@MotorRemote, MotorBattery=@MotorBattery, MotorCharger=@MotorCharger, Connector=@Connector, AdditionalMotor=@AdditionalMotor, CableExitPoint=@CableExitPoint, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", ItemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@UniqueId", If(String.IsNullOrEmpty(data.uniqueid), DBNull.Value, data.uniqueid))
                        myCmd.Parameters.AddWithValue("@BlindNo", data.blindno)
                        myCmd.Parameters.AddWithValue("@KitId", UCase(data.colourtype).ToString())
                        myCmd.Parameters.AddWithValue("@SoeKitId", SoeId)
                        myCmd.Parameters.AddWithValue("@ExactId", If(String.IsNullOrEmpty(ExactId), DBNull.Value, ExactId))
                        myCmd.Parameters.AddWithValue("@FabricId", UCase(data.fabriccolour).ToString())
                        myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(ChainId), DBNull.Value, ChainId))
                        myCmd.Parameters.AddWithValue("@BottomRailId", If(String.IsNullOrEmpty(BottomRailId), DBNull.Value, BottomRailId))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", UCase(PriceGroupId).ToString())
                        myCmd.Parameters.AddWithValue("@CassetteExtraId", If(String.IsNullOrEmpty(CassetteExtraId), DBNull.Value, CassetteExtraId))
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@LouvreSize", data.sizetype)
                        myCmd.Parameters.AddWithValue("@LouvrePosition", data.dropfloor)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@RollDirection", data.roll)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@ChainLength", If(String.IsNullOrEmpty(data.chainlength), DBNull.Value, data.chainlength))
                        myCmd.Parameters.AddWithValue("@Accessory", data.accessory)
                        myCmd.Parameters.AddWithValue("@TubeSize", data.tubesize)
                        myCmd.Parameters.AddWithValue("@Trim", data.trim)
                        myCmd.Parameters.AddWithValue("@BracketCover", data.bracketcovers)
                        myCmd.Parameters.AddWithValue("@BracketColour", data.bracketcovercolours)
                        myCmd.Parameters.AddWithValue("@BracketExtension", data.bracketext)
                        myCmd.Parameters.AddWithValue("@ChildSafe", data.childsafe)
                        myCmd.Parameters.AddWithValue("@MotorStyle", data.motorstyle)
                        myCmd.Parameters.AddWithValue("@MotorRemote", data.motorremote)
                        myCmd.Parameters.AddWithValue("@MotorBattery", data.externalbattery)
                        myCmd.Parameters.AddWithValue("@MotorCharger", data.charger)
                        myCmd.Parameters.AddWithValue("@Connector", data.connector)
                        myCmd.Parameters.AddWithValue("@AdditionalMotor", data.extras)
                        myCmd.Parameters.AddWithValue("@CableExitPoint", data.cableexitpoint)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                If data.brackettype = "Double" Then
                    '#SdsNext
                    Dim ListNext As New List(Of Object) From {
                        data.uniqueid,
                        data.tubesize,
                        data.mounting,
                        data.room,
                        data.childsafe,
                        data.accessory,
                        data.bracketcovers,
                        data.bracketext,
                        data.motorstyle,
                        markup
                    }
                    Dim ResNext As String = SdsNext(ListNext)
                    IF Not ResNext = "200" Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResNext, .field = ""}}
                    End If

                    '#SdsSize
                    Dim ListSize As New List(Of Object) From {
                        data.uniqueid,
                        width,
                        drop
                    }
                    Dim ResSize As String = SdsSize(ListSize)
                    IF Not ResSize = "200" Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResSize, .field = ""}}
                    End If
                End If

                If InArray(data.brackettype, "Linked 2 Blinds (Dep)", "Linked 3 Blinds (Dep)") Then
                    '#SdsDrop
                    Dim ListDrop As New List(Of Object) From {
                        data.uniqueid,
                        drop
                    }
                    Dim ResDrop As String = SdsDrop(ListDrop)
                    IF Not ResDrop = "200" Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResDrop, .field = ""}}
                    End If

                    '#SdsRollDep
                    Dim ListRollDep As New List(Of Object) From {
                        data.uniqueid,
                        data.roll
                    }
                    Dim ResRollDep As String = SdsRollDep(ListRollDep)
                    IF Not ResRollDep = "200" Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResRollDep, .field = ""}}
                    End If

                    '#SdsFabric
                    Dim ListFabric As New List(Of Object) From {
                        data.uniqueid,
                        data.fabriccolour,
                        PriceGroupId
                    }
                    Dim ResFabric As String = SdsFabric(ListFabric)
                    IF Not ResFabric = "200" Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResFabric, .field = ""}}
                    End If

                    '#SdsNext
                    Dim ListNext As New List(Of Object) From {
                        data.uniqueid,
                        data.tubesize,
                        data.mounting,
                        data.room,
                        data.childsafe,
                        data.accessory,
                        data.bracketcovers,
                        data.bracketext,
                        data.motorstyle,
                        markup
                    }
                    Dim ResNext As String = SdsNext(ListNext)
                    IF Not ResNext = "200" Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResNext, .field = ""}}
                    End If

                    If data.brackettype = "Linked 2 Blinds (Dep)" AndAlso data.isConfirmed Then
                        '#SdsControlLink3Ind
                        Dim ListControl As New List(Of Object) From {
                            ItemId,
                            data.uniqueid,
                            data.blindno
                        }
                        
                        Dim ResControl As String = SdsControlLink2Dep(ListControl)
                        IF Not ResControl = "200" Then
                            Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResControl, .field = ""}}
                        End If
                    End If

                    If data.brackettype = "Linked 3 Blinds (Dep)" AndAlso data.isConfirmed Then
                        '#SdsControlLink3Ind
                        Dim ListControl As New List(Of Object) From {
                            ItemId,
                            data.uniqueid,
                            data.blindno
                        }
                        
                        Dim ResControl As String = SdsControlLink3Dep(ListControl)
                        IF Not ResControl = "200" Then
                            Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResControl, .field = ""}}
                        End If
                    End If

                End If

                If InArray(data.brackettype, "Linked 2 Blinds (Ind)", "Linked 3 Blinds (Ind)") Then
                    '#SdsNext
                    Dim ListNext As New List(Of Object) From {
                        data.uniqueid,
                        data.tubesize,
                        data.mounting,
                        data.room,
                        data.childsafe,
                        data.accessory,
                        data.bracketcovers,
                        data.bracketext,
                        data.motorstyle,
                        markup
                    }
                    Dim ResNext As String = SdsNext(ListNext)
                    IF Not ResNext = "200" Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResNext, .field = ""}}
                    End If

                    '#SdsTubeSize
                    Dim ListTube As New List(Of Object) From {
                        data.uniqueid,
                        data.tubesize
                    }
                    Dim ResTube As String = SdsTubeSize(ListTube)
                    IF Not ResTube = "200" Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResTube, .field = ""}}
                    End If

                    '#SdsFabric
                    Dim ListFabric As New List(Of Object) From {
                        data.uniqueid,
                        data.fabriccolour,
                        PriceGroupId
                    }
                    Dim ResFabric As String = SdsFabric(ListFabric)
                    IF Not ResFabric = "200" Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResFabric, .field = ""}}
                    End If

                    If data.brackettype = "Linked 2 Blinds (Ind)" AndAlso data.isConfirmed Then
                        '#SdsControlLink3Ind
                        Dim ListControl As New List(Of Object) From {
                            ItemId,
                            data.uniqueid,
                            data.blindno
                        }
                        
                        Dim ResControl As String = SdsControlLink2Ind(ListControl)
                        IF Not ResControl = "200" Then
                            Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResControl, .field = ""}}
                        End If
                    End If

                    If data.brackettype = "Linked 3 Blinds (Ind)" AndAlso data.isConfirmed Then
                        '#SdsControlLink3Ind
                        Dim ListControl As New List(Of Object) From {
                            ItemId,
                            data.uniqueid,
                            data.blindno
                        }
                        
                        Dim ResControl As String = SdsControlLink3Ind(ListControl)
                        IF Not ResControl = "200" Then
                            Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResControl, .field = ""}}
                        End If
                    End If

                End If

                If data.brackettype = "Double and Link System Dep" Then
                    '#SdsDrop
                    Dim ListDrop As New List(Of Object) From {
                        data.uniqueid,
                        drop
                    }
                    Dim ResDrop As String = SdsDrop(ListDrop)
                    IF Not ResDrop = "200" Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResDrop, .field = ""}}
                    End If

                    If InArray(data.blindno, "Blind 1", "Blind 2") Then
                        '#SdsDB2IndFirst
                        Dim ListDB2IndFirst As New List(Of Object) From {
                            data.uniqueid,
                            data.fabriccolour,
                            PriceGroupId,
                            data.roll
                        }
                        Dim ResDB2IndFirst As String = SdsDB2First(ListDB2IndFirst)
                        IF Not ResDB2IndFirst = "200" Then
                            Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResDB2IndFirst, .field = ""}}
                        End If
                    End If

                    If InArray(data.blindno, "Blind 3", "Blind 4") Then
                        '#SdsDB2DepSecond
                        Dim ListDB2DepSecond As New List(Of Object) From {
                            data.uniqueid,
                            data.fabriccolour,
                            PriceGroupId,
                            data.roll
                        }
                        Dim ResDB2DepSecond As String = SdsDB2Second(ListDB2DepSecond)
                        IF Not ResDB2DepSecond = "200" Then
                            Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResDB2DepSecond, .field = ""}}
                        End If
                    End If

                End If

                publicCfg.ResetPriceDetail(ItemId)
                publicCfg.HitungHarga(data.headerid, ItemId)
                publicCfg.HitungSurcharge(data.headerid, ItemId)

                Dim dataLog As Object() = {data.headerid, ItemId, "Blinds", data.loginid, "Update Item Order"}
                orderCfg.Log_Orders(dataLog)

                msg = "Item updated successfully !"

                If InStr(data.brackettype, "Linked") > 0 AND data.controltype = "Somfy WF" Then
                    msg += "<br/><br/><b>Warning :</b>Check SP the availability for linking blind for WF motorised !"
                End If
                If InStr(data.brackettype, "Linked") > 0 AND data.controltype = "Alpha WF" AndAlso data.motorstyle = "Alpha 2NM Std" Then
                    msg += "<br/><br/><b>Warning :</b> Check SP the availability for linking blind for WF motorised !"
                End If

            End If

            Return New SuccessResponse With {.success = msg}
        Catch ex As Exception
            Return New ErrorResponse With { .error = New ErrorDetail With { .message = ex.Message, .field = ""}}
        End Try
    End Function

    Private Shared Function InArray(value As String, ParamArray list() As String) As Boolean
        Return list.Contains(value)
    End Function

    ' Private Shared Function GenerateUniqueId() As String
    '     Try
    '         Dim result As String = String.Empty

    '         Dim alphabets As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
    '         Dim small_alphabets As String = "abcdefghijklmnopqrstuvwxyz"
    '         Dim numbers As String = "1234567890"

    '         Dim characters As String = Convert.ToString(alphabets & small_alphabets) & numbers
    '         Dim length As Integer = Integer.Parse(20)
    '         Dim uniqueId As String = String.Empty
    '         For i As Integer = 0 To length - 1
    '             Dim character As String = String.Empty
    '             Do
    '                 Dim index As Integer = New Random().Next(0, characters.Length)
    '                 character = characters.ToCharArray()(index).ToString()
    '             Loop While uniqueId.IndexOf(character) <> -1
    '             uniqueId += character
    '         Next
    '         result = uniqueId

    '         Return result
    '     Catch ex As Exception
    '         Return "500"
    '     End Try
    ' End Function

    Private Shared Function GenerateUniqueId() As String
        Return Guid.NewGuid().ToString("N") ' tanpa dash
    End Function

    Private Shared Function SdsNext(ListParam As List(Of Object)) As String
        Try
            Dim UniqueId As String = CStr(ListParam(0))
            Dim TubeSize As String = CStr(ListParam(1))
            Dim Mounting As String = CStr(ListParam(2))
            Dim Room As String = CStr(ListParam(3))
            Dim ChildSafe As String = CStr(ListParam(4))
            Dim Accessory As String = CStr(ListParam(5))
            Dim BracketCover As String = CStr(ListParam(6))
            Dim BracketExtension As String = CStr(ListParam(7))
            Dim MotorStyle As String = CStr(ListParam(8))
            Dim Markup As String = CStr(ListParam(9))

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderDetails SET TubeSize=@TubeSize, Mounting=@Mounting, Location=@Location, ChildSafe=@ChildSafe, Accessory=@Accessory, BracketCover=@BracketCover, BracketExtension=@BracketExtension, MotorStyle=@MotorStyle, MarkUp=@MarkUp WHERE UniqueId=@UniqueId AND Active=1", thisConn)
                    myCmd.Parameters.AddWithValue("@UniqueId", UniqueId)
                    myCmd.Parameters.AddWithValue("@TubeSize", TubeSize)
                    myCmd.Parameters.AddWithValue("@Mounting", Mounting)
                    myCmd.Parameters.AddWithValue("@Location", Room)
                    myCmd.Parameters.AddWithValue("@ChildSafe", ChildSafe)
                    myCmd.Parameters.AddWithValue("@Accessory", Accessory)
                    myCmd.Parameters.AddWithValue("@BracketCover", BracketCover)
                    myCmd.Parameters.AddWithValue("@BracketExtension", BracketExtension)
                    myCmd.Parameters.AddWithValue("@MotorStyle", MotorStyle)
                    myCmd.Parameters.AddWithValue("@MarkUp", Markup)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Return "200"
        Catch ex As Exception
            Return "SdsNext: " & ex.Message
        End Try
    End function

    Private Shared Function SdsSize(ListParam As List(Of Object)) As String
        Try
            Dim UniqueId As String = CStr(ListParam(0))
            Dim Width As String = CStr(ListParam(1))
            Dim Drop As String = CStr(ListParam(2))

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderDetails SET Width=@Width, [Drop]=@Drop WHERE UniqueId=@UniqueId AND Active=1", thisConn)
                    myCmd.Parameters.AddWithValue("@UniqueId", UniqueId)
                    myCmd.Parameters.AddWithValue("@Width", Width)
                    myCmd.Parameters.AddWithValue("@Drop", Drop)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Return "200"
        Catch ex As Exception
            Return "SdsSize: " & ex.Message
        End Try
    End function


    Private Shared Function SdsDrop(ListParam As List(Of Object)) As String
        Try
            Dim UniqueId As String = CStr(ListParam(0))
            Dim Drop As String = CStr(ListParam(1))

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderDetails SET [Drop]=@Drop WHERE UniqueId=@UniqueId AND Active=1", thisConn)
                    myCmd.Parameters.AddWithValue("@UniqueId", UniqueId)
                    myCmd.Parameters.AddWithValue("@Drop", Drop)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Return "200"
        Catch ex As Exception
            Return "SdsDrop: " & ex.Message
        End Try
    End function

    Private Shared Function SdsTubeSize(ListParam As List(Of Object)) As String
        Try
            Dim UniqueId As String = CStr(ListParam(0))
            Dim TubeSize As String = CStr(ListParam(1))

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderDetails SET TubeSize=@TubeSize WHERE UniqueId=@UniqueId AND Active=1", thisConn)
                    myCmd.Parameters.AddWithValue("@UniqueId", UniqueId)
                    myCmd.Parameters.AddWithValue("@TubeSize", TubeSize)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Return "200"
        Catch ex As Exception
            Return "SdsTubeSize: " & ex.Message
        End Try
    End function

    Private Shared Function SdsRollDep(ListParam As List(Of Object)) As String
        Try
            Dim UniqueId As String = CStr(ListParam(0))
            Dim RollDirection As String = CStr(ListParam(1))

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderDetails SET RollDirection=@RollDirection WHERE UniqueId=@UniqueId AND Active=1", thisConn)
                    myCmd.Parameters.AddWithValue("@UniqueId", UniqueId)
                    myCmd.Parameters.AddWithValue("@RollDirection", RollDirection)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Return "200"
        Catch ex As Exception
            Return "SdsRollDep: " & ex.Message
        End Try
    End function

    Private Shared Function SdsControlLink2Dep(ListParam As List(Of Object)) As String
        Try
            Dim ThisControl As String = ""
            Dim ThisBlindNo As String = ""
            Dim ThisChainId As String = ""
            Dim ThisChainLength As String = ""
            Dim ItemId As String = CStr(ListParam(0))
            Dim UniqueId As String = CStr(ListParam(1))
            Dim BlindNo As String = CStr(ListParam(2))
            Dim ControlB1 As String = FindControlPosition(UniqueId, "Blind 1")
            Dim ControlB2 As String = FindControlPosition(UniqueId, "Blind 2")
            Dim ChainIdB1 As String = FindChainId(UniqueId, "Blind 1")
            Dim ChainIdB2 As String = FindChainId(UniqueId, "Blind 2")
            Dim ChainLengthB1 As String = FindChainLength(UniqueId, "Blind 1")
            Dim ChainLengthB2 As String = FindChainLength(UniqueId, "Blind 2")

           
            If BlindNo = "Blind 1" Then
                ThisBlindNo = "Blind 2"
                ThisControl = ""
                ThisChainId = ""
                ThisChainLength = ""
            End If

            If BlindNo = "Blind 2" Then
                ThisBlindNo = "Blind 1"
                ThisControl = ""
                ThisChainId = ""
                ThisChainLength = ""
            End If

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderDetails SET ControlPosition=@ControlPosition, ChainId=@ChainId, ChainLength=@ChainLength WHERE BlindNo=@BlindNo AND UniqueId=@UniqueId AND Active=1", thisConn)
                    myCmd.Parameters.AddWithValue("@UniqueId", UniqueId)
                    myCmd.Parameters.AddWithValue("@ControlPosition", ThisControl)
                    myCmd.Parameters.AddWithValue("@ChainId", ThisChainId)
                    myCmd.Parameters.AddWithValue("@ChainLength", if(ThisChainLength = "", DBNull.Value, ThisChainLength))
                    myCmd.Parameters.AddWithValue("@BlindNo", ThisBlindNo)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            If BlindNo = "Blind 1" Then
                ThisBlindNo = "Blind 1"
                ThisChainId = ChainIdB2
                ThisChainLength = ChainLengthB2
            End If

            If BlindNo = "Blind 2" Then
                ThisBlindNo = "Blind 2"
                ThisChainId = ChainIdB1
                ThisChainLength = ChainLengthB1
            End If

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderDetails SET ChainId=@ChainId, ChainLength=@ChainLength WHERE BlindNo=@BlindNo AND UniqueId=@UniqueId AND Active=1", thisConn)
                    myCmd.Parameters.AddWithValue("@UniqueId", UniqueId)
                    myCmd.Parameters.AddWithValue("@ChainId", ThisChainId)
                    myCmd.Parameters.AddWithValue("@ChainLength", ThisChainLength)
                    myCmd.Parameters.AddWithValue("@BlindNo", ThisBlindNo)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Return "200"
        Catch ex As Exception
            Return "SdsFabric: " & ex.Message
        End Try
    End function

    Private Shared Function SdsControlLink2Ind(ListParam As List(Of Object)) As String
        Try
            Dim ThisControl As String = ""
            Dim ThisBlindNo As String = ""
            Dim ItemId As String = CStr(ListParam(0))
            Dim UniqueId As String = CStr(ListParam(1))
            Dim BlindNo As String = CStr(ListParam(2))
            Dim ControlB1 As String = FindControlPosition(UniqueId, "Blind 1")
            Dim ControlB2 As String = FindControlPosition(UniqueId, "Blind 2")

           
            If BlindNo = "Blind 1" Then
                ThisBlindNo = "Blind 2"
                If ControlB1 = "Left" Then
                    ThisControl = "Right"
                Else If ControlB1 = "Right" Then
                    ThisControl = "Left"
                End If
            End If

            If BlindNo = "Blind 2" Then
                ThisBlindNo = "Blind 1"
                If ControlB2 = "Left" Then
                    ThisControl = "Right"
                Else If ControlB2 = "Right" Then
                    ThisControl = "Left"
                End If
            End If

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderDetails SET ControlPosition=@ControlPosition WHERE BlindNo=@BlindNo AND UniqueId=@UniqueId AND Active=1", thisConn)
                    myCmd.Parameters.AddWithValue("@UniqueId", UniqueId)
                    myCmd.Parameters.AddWithValue("@ControlPosition", ThisControl)
                    myCmd.Parameters.AddWithValue("@BlindNo", ThisBlindNo)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Return "200"
        Catch ex As Exception
            Return "SdsFabric: " & ex.Message
        End Try
    End function

    Private Shared Function SdsControlLink3Dep(ListParam As List(Of Object)) As String
        Try
            Dim ThisControl As String = ""
            Dim ThisBlindNo As String = ""
            Dim ThisChainId As String = ""
            Dim ThisChainLength As String = ""
            Dim ItemId As String = CStr(ListParam(0))
            Dim UniqueId As String = CStr(ListParam(1))
            Dim BlindNo As String = CStr(ListParam(2))
            Dim ControlB1 As String = FindControlPosition(UniqueId, "Blind 1")
            Dim ControlB3 As String = FindControlPosition(UniqueId, "Blind 3")
            Dim ChainIdB1 As String = FindChainId(UniqueId, "Blind 1")
            Dim ChainIdB3 As String = FindChainId(UniqueId, "Blind 3")
            Dim ChainLengthB1 As String = FindChainLength(UniqueId, "Blind 1")
            Dim ChainLengthB3 As String = FindChainLength(UniqueId, "Blind 3")

           
            If BlindNo = "Blind 1" Then
                ThisBlindNo = "Blind 3"
                ThisControl = ""
                ThisChainId = ""
                ThisChainLength = ""
            End If

            If BlindNo = "Blind 3" Then
                ThisBlindNo = "Blind 1"
                ThisControl = ""
                ThisChainId = ""
                ThisChainLength = ""
            End If

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderDetails SET ControlPosition=@ControlPosition, ChainId=@ChainId, ChainLength=@ChainLength WHERE BlindNo=@BlindNo AND UniqueId=@UniqueId AND Active=1", thisConn)
                    myCmd.Parameters.AddWithValue("@UniqueId", UniqueId)
                    myCmd.Parameters.AddWithValue("@ControlPosition", ThisControl)
                    myCmd.Parameters.AddWithValue("@ChainId", ThisChainId)
                    myCmd.Parameters.AddWithValue("@ChainLength", if(ThisChainLength = "", DBNull.Value, ThisChainLength))
                    myCmd.Parameters.AddWithValue("@BlindNo", ThisBlindNo)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            If BlindNo = "Blind 1" Then
                ThisBlindNo = "Blind 1"
                ThisChainId = ChainIdB3
                ThisChainLength = ChainLengthB3
            End If

            If BlindNo = "Blind 3" Then
                ThisBlindNo = "Blind 3"
                ThisChainId = ChainIdB1
                ThisChainLength = ChainLengthB1
            End If

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderDetails SET ChainId=@ChainId, ChainLength=@ChainLength WHERE BlindNo=@BlindNo AND UniqueId=@UniqueId AND Active=1", thisConn)
                    myCmd.Parameters.AddWithValue("@UniqueId", UniqueId)
                    myCmd.Parameters.AddWithValue("@ChainId", ThisChainId)
                    myCmd.Parameters.AddWithValue("@ChainLength", ThisChainLength)
                    myCmd.Parameters.AddWithValue("@BlindNo", ThisBlindNo)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Return "200"
        Catch ex As Exception
            Return "SdsFabric: " & ex.Message
        End Try
    End function

    Private Shared Function SdsControlLink3Ind(ListParam As List(Of Object)) As String
        Try
            Dim ThisControl As String = ""
            Dim ThisBlindNo As String = ""
            Dim ItemId As String = CStr(ListParam(0))
            Dim UniqueId As String = CStr(ListParam(1))
            Dim BlindNo As String = CStr(ListParam(2))
            Dim ControlB1 As String = publicCfg.GetItemData(String.Format("SELECT ControlPosition FROM OrderDetails WHERE UniqueId='{0}' AND BlindNo='Blind 1' AND Active=1", UniqueId))
            Dim ControlB3 As String = publicCfg.GetItemData(String.Format("SELECT ControlPosition FROM OrderDetails WHERE UniqueId='{0}' AND BlindNo='Blind 3' AND Active=1", UniqueId))

            If BlindNo = "Blind 2" Then Return "200"
           
            If BlindNo = "Blind 1" Then
                ThisBlindNo = "Blind 3"
                If ControlB1 = "Left" Then
                    ThisControl = "Right"
                Else If ControlB1 = "Right" Then
                    ThisControl = "Left"
                End If
            End If

            If BlindNo = "Blind 3" Then
                ThisBlindNo = "Blind 1"
                If ControlB3 = "Left" Then
                    ThisControl = "Right"
                Else If ControlB3 = "Right" Then
                    ThisControl = "Left"
                End If
            End If

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderDetails SET ControlPosition=@ControlPosition WHERE BlindNo=@BlindNo AND UniqueId=@UniqueId AND Active=1", thisConn)
                    myCmd.Parameters.AddWithValue("@UniqueId", UniqueId)
                    myCmd.Parameters.AddWithValue("@ControlPosition", ThisControl)
                    myCmd.Parameters.AddWithValue("@BlindNo", ThisBlindNo)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Return "200"
        Catch ex As Exception
            Return "SdsFabric: " & ex.Message
        End Try
    End function

    Private Shared Function SdsFabric(ListParam As List(Of Object)) As String
        Try
            Dim UniqueId As String = CStr(ListParam(0))
            Dim FabricId As String = CStr(ListParam(1))
            Dim PriceGroupId As String = CStr(ListParam(2))

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderDetails SET FabricId=@FabricId, PriceGroupId=@PriceGroupId WHERE UniqueId=@UniqueId AND Active=1", thisConn)
                    myCmd.Parameters.AddWithValue("@UniqueId", UniqueId)
                    myCmd.Parameters.AddWithValue("@FabricId", FabricId)
                    myCmd.Parameters.AddWithValue("@PriceGroupId", PriceGroupId)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Return "200"
        Catch ex As Exception
            Return "SdsFabric: " & ex.Message
        End Try
    End function

    Private Shared Function SdsDB2First(ListParam As List(Of Object)) As String
        Try
            Dim UniqueId As String = CStr(ListParam(0))
            Dim FabricId As String = CStr(ListParam(1))
            Dim PriceGroupId As String = CStr(ListParam(2))
            Dim RollDirection As String = CStr(ListParam(3))

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderDetails SET FabricId=@FabricId, PriceGroupId=@PriceGroupId, RollDirection=@RollDirection WHERE BlindNo IN ('Blind 1', 'Blind 2') AND UniqueId=@UniqueId AND Active=1", thisConn)
                    myCmd.Parameters.AddWithValue("@UniqueId", UniqueId)
                    myCmd.Parameters.AddWithValue("@FabricId", FabricId)
                    myCmd.Parameters.AddWithValue("@PriceGroupId", PriceGroupId)
                    myCmd.Parameters.AddWithValue("@RollDirection", RollDirection)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Return "200"
        Catch ex As Exception
            Return "SdsFabric: " & ex.Message
        End Try
    End function

    Private Shared Function SdsDB2Second(ListParam As List(Of Object)) As String
        Try
            Dim UniqueId As String = CStr(ListParam(0))
            Dim FabricId As String = CStr(ListParam(1))
            Dim PriceGroupId As String = CStr(ListParam(2))
            Dim RollDirection As String = CStr(ListParam(3))

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderDetails SET FabricId=@FabricId, PriceGroupId=@PriceGroupId, RollDirection=@RollDirection WHERE BlindNo IN ('Blind 3', 'Blind 4') AND UniqueId=@UniqueId AND Active=1", thisConn)
                    myCmd.Parameters.AddWithValue("@UniqueId", UniqueId)
                    myCmd.Parameters.AddWithValue("@FabricId", FabricId)
                    myCmd.Parameters.AddWithValue("@PriceGroupId", PriceGroupId)
                    myCmd.Parameters.AddWithValue("@RollDirection", RollDirection)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Return "200"
        Catch ex As Exception
            Return "SdsFabric: " & ex.Message
        End Try
    End function

    ' Private Shared Function SdsDB2IndFirst(ListParam As List(Of Object)) As String
    '     Try
    '         Dim UniqueId As String = CStr(ListParam(0))
    '         Dim FabricId As String = CStr(ListParam(1))
    '         Dim PriceGroupId As String = CStr(ListParam(2))
    '         Dim RollDirection As String = CStr(ListParam(3))

    '         Using thisConn As New SqlConnection(myConn)
    '             Using myCmd As New SqlCommand("UPDATE OrderDetails SET FabricId=@FabricId, PriceGroupId=@PriceGroupId, RollDirection=@RollDirection WHERE BlindNo IN ('Blind 1', 'Blind 2') AND UniqueId=@UniqueId AND Active=1", thisConn)
    '                 myCmd.Parameters.AddWithValue("@UniqueId", UniqueId)
    '                 myCmd.Parameters.AddWithValue("@FabricId", FabricId)
    '                 myCmd.Parameters.AddWithValue("@PriceGroupId", PriceGroupId)
    '                 myCmd.Parameters.AddWithValue("@RollDirection", RollDirection)
    '                 myCmd.Connection = thisConn
    '                 thisConn.Open()
    '                 myCmd.ExecuteNonQuery()
    '                 thisConn.Close()
    '             End Using
    '         End Using

    '         Return "200"
    '     Catch ex As Exception
    '         Return "SdsFabric: " & ex.Message
    '     End Try
    ' End function

    ' Private Shared Function SdsDB2DepSecond(ListParam As List(Of Object)) As String
    '     Try
    '         Dim UniqueId As String = CStr(ListParam(0))
    '         Dim FabricId As String = CStr(ListParam(1))
    '         Dim PriceGroupId As String = CStr(ListParam(2))
    '         Dim RollDirection As String = CStr(ListParam(3))

    '         Using thisConn As New SqlConnection(myConn)
    '             Using myCmd As New SqlCommand("UPDATE OrderDetails SET FabricId=@FabricId, PriceGroupId=@PriceGroupId, RollDirection=@RollDirection WHERE BlindNo IN ('Blind 3', 'Blind 4') AND UniqueId=@UniqueId AND Active=1", thisConn)
    '                 myCmd.Parameters.AddWithValue("@UniqueId", UniqueId)
    '                 myCmd.Parameters.AddWithValue("@FabricId", FabricId)
    '                 myCmd.Parameters.AddWithValue("@PriceGroupId", PriceGroupId)
    '                 myCmd.Parameters.AddWithValue("@RollDirection", RollDirection)
    '                 myCmd.Connection = thisConn
    '                 thisConn.Open()
    '                 myCmd.ExecuteNonQuery()
    '                 thisConn.Close()
    '             End Using
    '         End Using

    '         Return "200"
    '     Catch ex As Exception
    '         Return "SdsFabric: " & ex.Message
    '     End Try
    ' End function

    Private Shared Function FindControlPosition(uniqueid As String, blindno As String) As String
        Try
            Dim result As String = publicCfg.GetItemData(String.Format("SELECT ControlPosition FROM OrderDetails WHERE BlindNo = '{0}' AND UniqueId='{1}' AND Active = 1",blindno, uniqueid))
            Return result
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Private Shared Function FindChainId(uniqueid As String, blindno As String) As String
        Try
            Dim result As String = publicCfg.GetItemData(String.Format("SELECT ChainId FROM OrderDetails WHERE BlindNo = '{0}' AND UniqueId='{1}' AND Active = 1",blindno, uniqueid))
            Return result
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Private Shared Function FindChainLength(uniqueid As String, blindno As String) As String
        Try
            Dim result As String = publicCfg.GetItemData(String.Format("SELECT ChainLength FROM OrderDetails WHERE BlindNo = '{0}' AND UniqueId='{1}' AND Active = 1",blindno, uniqueid))
            Return result
        Catch ex As Exception
            Return ""
        End Try
    End Function

End Class
