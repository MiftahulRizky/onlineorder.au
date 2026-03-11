Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic
Imports System.Linq
Partial Class Methods_Order_VerticalBlindMethod
    Inherits System.Web.UI.Page

    Shared publicCfg As New PublicConfig()
    Shared orderCfg As New OrderConfig()
    Public Shared myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Class ParamSubmit
        '#Submit OrderHeader
        Public Property blindtype As String
        Public Property tubetype As String
        Public Property controltype As String
        Public Property qty As String
        Public Property room As String
        Public Property mounting As String
        Public Property width As String
        Public Property drop As String
        Public Property slatsize As String
        Public Property slatqty As String
        Public Property fabrictype As String
        Public Property fabriclength As String
        Public Property fabriccolour As String
        Public Property trackcolour As String
        Public Property stackposition As String
        Public Property controlposition As String
        Public Property chaincolour As String
        Public Property chainlength As String
        Public Property wandlength As String
        Public Property wandcolour As String
        Public Property wandcustomlength As String
        Public Property bracket As String
        Public Property bracketcolour As String
        Public Property hangertype As String
        Public Property bottom As String
        Public Property inserttrack As String
        Public Property sloper As String
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
    Public Shared Function BindTubeType(ByVal designid As String, ByVal blindid As String) As Object
        Try
            Dim MyQuery As String = String.Format("SELECT TubeType FROM HardwareKits WHERE DesignId = '{0}' AND BlindId='{1}' AND Active=1 GROUP BY TubeType ORDER BY TubeType ASC", designid, UCase(blindid).ToString())
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
    Public Shared Function BindControlType(ByVal designid As String, ByVal blindid As String, ByVal tubetype As String) As Object
        Try
            Dim MyQuery As String = String.Format("SELECT *, UPPER(ControlType) AS ControlText FROM HardwareKits WHERE DesignId='{0}' AND BlindId = '{1}' AND TubeType='{2}' ORDER BY Name ASC", designid, UCase(blindid).ToString(), tubetype)
            Dim datas As DataSet = publicCfg.GetListData(MyQuery)
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("Id").ToString()},
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
    Public Shared Function BindFabricLength(ByVal designid As String, ByVal tubetype As String, ByVal fabrictype As String) As Object
        Try
            Dim Width As String = ""
            If tubetype = "Louvolite" Then
                Width = "AND Width IN ('89', '127')"
            End If

            Dim MyQuery As String = String.Format("SELECT Width FROM Fabrics WHERE DesignId='{0}' AND Type='{1}' {2} AND Active='1' GROUP BY Width ORDER BY Width ASC", designid, fabrictype, Width)
            Dim datas As DataSet = publicCfg.GetListData(MyQuery)
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("Width").ToString()},
                        {"text", row("Width").ToString()}
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
    Public Shared Function BindFabricColour(ByVal designid As String, ByVal fabrictype As String, ByVal fabriclength As String) As Object
        Try
            Dim MyQuery As String = String.Format("SELECT Id, Colour FROM Fabrics WHERE DesignId='{0}' AND Type='{1}' AND Width='{2}' AND Active='1'  ORDER BY Name ASC", designid, fabrictype, fabriclength)
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
            Dim msg As String = "200"

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

            If Not BlindName = "Slat Only" And String.IsNullOrEmpty(data.mounting) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "mounting is required !", .field = "mounting"}}
            End If

            If BlindName = "Complete" Or BlindName = "Track Only" Then
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
            End If

            If BlindName = "Slat Only" Then
                If Not String.IsNullOrEmpty(data.slatqty) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "slat qty is required !",.field = "slatqty"}}
                End If
            End If

            If Not BlindName = "Track Only" Then
                If String.IsNullOrEmpty(data.fabrictype) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fabric type is required !",.field = "fabrictype"}}
                End If
                If String.IsNullOrEmpty(data.fabriclength) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fabric / slat is required !",.field = "fabriclength"}}
                End If
                If String.IsNullOrEmpty(data.fabriccolour) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fabric colour is required !",.field = "fabriccolour"}}
                End If
            End If

            If BlindName = "Complete" Or BlindName = "Track Only" Then
                If String.IsNullOrEmpty(data.trackcolour) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "track colour is required !",.field = "trackcolour"}}
                End If
                If String.IsNullOrEmpty(data.stackposition) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "stack position is required !",.field = "stackposition"}}
                End If
                If String.IsNullOrEmpty(data.controlposition) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position is required !",.field = "controlposition"}}
                End If
            End If

            Dim ControlType As String = publicCfg.GetItemData(String.Format("SELECT ControlType FROM HardwareKits WHERE Id = '{0}'", data.controltype))
            If ControlType = "Chain" Then
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

            If ControlType = "Wand" Then
                If String.IsNullOrEmpty(data.wandlength) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "wand length is required !",.field = "wandlength"}}
                End If
                If String.IsNullOrEmpty(data.wandcolour) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "wand colour is required !",.field = "wandcolour"}}
                End If
                If data.wandlength = "custom" Then
                    If String.IsNullOrEmpty(data.wandcustomlength) Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "custom wand length is required !",.field = "wandcustomlength"}}
                    End If
                End If
            End If

            If BlindName = "Complete" Or BlindName = "Track Only" Then
                If String.IsNullOrEmpty(data.bracket) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "bracket is required !",.field = "bracket"}}
                End If
                If String.IsNullOrEmpty(data.bracketcolour) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "bracket colour is required !",.field = "bracketcolour"}}
                End If
            End If
            
            If String.IsNullOrEmpty(data.hangertype) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "hanger type is required !",.field = "hangertype"}}
            End If

            If Not BlindName = "Track Only" Then
                If String.IsNullOrEmpty(data.bottom) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "bottom is required !",.field = "bottom"}}
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

            Return New SuccessResponse With {.success = msg}
        Catch ex As Exception
            Return New ErrorResponse With { .error = New ErrorDetail With { .message = ex.Message, .field = ""}}
        End Try
    End Function  
End Class
