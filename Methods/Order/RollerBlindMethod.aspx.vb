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
        Public Property bracketext As String
        Public Property notes As String
        Public Property markup As String
        

        '#aditional param
        Public Property headerid As String
        Public Property itemaction As String
        Public Property itemid As String
        Public Property designid As String
        Public Property loginid As String
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
            Dim MyQuery As String = String.Format("SELECT ColourType FROM HardwareKits WHERE DesignId = '{0}' AND BlindId='{1}' AND BracketType='{2}' AND TubeType='{3}' AND Active=1 GROUP BY ColourType ORDER BY ColourType ASC", designid, UCase(blindid).ToString(), brackettype, tubetype, controltype)
            Dim datas As DataSet = publicCfg.GetListData(MyQuery)
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("ColourType").ToString()},
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
    Public Shared Function BindRailType(ByVal brackettype As String) As Object
        Try
            Dim FindBracket As String = brackettype

            If brackettype = "Headbox & Side Channels" Then
                FindBracket = "Headbox &amp; Side Channels"
            End If
            
            If brackettype = "With Tube & Bottom Included" Then
                FindBracket = "With Tube &amp; Bottom Included"
            End If

            Dim MyQuery As String = String.Format("SELECT UPPER(Type) AS TypeText, Type AS TypeValue FROM Bottoms CROSS APPLY STRING_SPLIT(BracketType, ',') WHERE VALUE = '{0}' AND Active ='1' GROUP BY Type ORDER BY Type ASC", FindBracket)
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
    Public Shared Function BindRailColour(ByVal brackettype As String, ByVal railtype As String) As Object
        Try
            Dim FindBracket As String = brackettype

            If brackettype = "Headbox & Side Channels" Then
                FindBracket = "Headbox &amp; Side Channels"
            End If
            
            If brackettype = "With Tube & Bottom Included" Then
                FindBracket = "With Tube &amp; Bottom Included"
            End If

            Dim MyQuery As String = String.Format("SELECT Id, UPPER(Colour) AS Colour, VALUE Product FROM Bottoms CROSS APPLY STRING_SPLIT(BracketType, ',') WHERE VALUE = '{0}' AND Type='{1}' AND Active ='1' ORDER BY Name ASC", FindBracket, railtype)
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
    Public Shared Function Submit(ByVal data As ParamSubmit) As Object
        Try
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

            If String.IsNullOrEmpty(data.mounting) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "mounting type is required !", .field = "mounting"}}
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

            Dim BlindName As String = publicCfg.GetItemData(String.Format("SELECT Name FROM Blinds WHERE Id = '{0}'", data.blindtype))
            If (BlindName = "Cassette" AND data.tubetype = "Motorised") OR BlindName = "Motorised" Then
                If String.IsNullOrEmpty(data.motorstyle) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "motor style is required !",.field = "motorstyle"}}
                End If
                If String.IsNullOrEmpty(data.motorremote) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "motor remote is required !",.field = "motorremote"}}
                End If
                If String.IsNullOrEmpty(data.charger) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "charger is required !",.field = "charger"}}
                End If
            End If

            If Not (data.tubetype = "Spring Operated" OR data.tubetype = "N/A") Then
                If Not BlindName = "Skin Only" Then
                    If String.IsNullOrEmpty(data.roll) Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "roll direction is required !",.field = "roll"}}
                    End If
                End If

                If InArray(data.brackettype, "Single", "Double", "Linked 2 Blinds (Ind)", "Headbox Only", "Headbox & Side Channels") Then
                    If String.IsNullOrEmpty(data.controlposition) Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position / side is required !",.field = "controlposition"}}
                    End If
                End If
            End IF

            If data.controltype = "Chain" Then
                If InArray(data.brackettype, "Single", "Double", "Linked 2 Blinds (Ind)") Then
                End If

                If data.brackettype = "Linked 2 Blinds (Dep)" Then
                End If
                
                If data.brackettype = "Linked 3 Blinds (Dep)" Then
                End If

                If data.brackettype = "Linked 3 Blinds (Ind)" Then
                End If

                If data.brackettype = "Double and Link System Dep" Then
                End If

                If data.brackettype = "Double and Link System Ind" Then
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
                If Not BlindName = "Skin Only" Then
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
            


            If (BlindName = "Skin Only" AND InStr(data.brackettype, "Tube") > 0 ) OR BlindName = "Roller Blind" Then
                If String.IsNullOrEmpty(data.tubesize) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "tube size is required !",.field = "tubesize"}}
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
            

            Dim msg As String = "200"
            If data.itemaction = "AddItem" OrElse data.itemaction = "CopyItem" Then
                msg = "Item added successfully !"
            End If

            If data.itemaction = "EditItem" OrElse data.itemaction = "ViewItem" Then
                msg = "Item added successfully !"
            End If

            Return New SuccessResponse With {.success = msg}
        Catch ex As Exception
            Return New ErrorResponse With { .error = New ErrorDetail With { .message = ex.Message, .field = ""}}
        End Try
    End Function

    Private Shared Function InArray(value As String, ParamArray list() As String) As Boolean
        Return list.Contains(value)
    End Function

End Class
