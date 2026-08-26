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

    Public Class ParamBindFormAggregate
        Public Property headerid As String
        Public Property ordertype As String
        Public Property designid As String
        Public Property itemid As String
        Public Property itemaction As String
    End Class

     Public Class ParamListData
        Public Property field As String
        Public Property designid As String
        Public Property blindtype As String
        Public Property brackettype As String
        Public Property tubetype As String
        Public Property controltype As String
        Public Property fabrictype As String
        Public Property fabriclength As String
        Public Property trim As String
        Public Property railtype As String
    End Class

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindFormAggregate(ByVal data As ParamBindFormAggregate) As Object
        Try
            Dim DesignName As String = publicCfg.GetItemData(String.Format("SELECT Name FROM Designs WHERE Id = '{0}'", data.designid))

            Dim HeaderData As Object
            Using conn As New SqlConnection(myConn)
                Using cmd As New SqlCommand("SELECT OrderId, OrderNumber, OrderName FROM view_order_headers WHERE OrderType IN ('Blinds', 'Door and Window') AND Id = @Id", conn)
                    cmd.Parameters.AddWithValue("@Id", data.headerid)
                    ' cmd.Parameters.AddWithValue("@OrderType", data.ordertype)

                    conn.Open()
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then

                            HeaderData = New With {
                                .OrderId = reader("OrderId").ToString(),
                                .OrderNumber = reader("OrderNumber").ToString(),
                                .OrderName = reader("OrderName").ToString()
                            }
                        End If
                    End Using
                End Using
            End Using

            Dim Blinds As Object  = BindListData(New ParamListData With {
                .field = "blindtype",
                .designid = data.designid
            })
            If Blinds.error Then
                Throw New Exception(Blinds.message)
            End If

            Return New With {
                .DesignName = DesignName,
                .HeaderData = HeaderData,
                .Blinds = Blinds.list
            }
        Catch ex As Exception
            Return New With {.error = True, .message = String.Format("BindFormAggregate: {0}", ex.Message)}
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
                    resultList = GetFormattedData(query, "Id", "Name")

                Case "brackettype"
                    query = String.Format("SELECT BracketType FROM HardwareKits WHERE DesignId = '{0}' AND BlindId='{1}' AND Active=1 GROUP BY BracketType ORDER BY BracketType ASC", data.designid, UCase(data.blindtype).ToString())
                    resultList = GetFormattedData(query, "BracketType", "BracketType")

                Case "tubetype"
                    query = String.Format("SELECT TubeType FROM HardwareKits WHERE DesignId = '{0}' AND BlindId='{1}' AND BracketType='{2}' AND Active=1 GROUP BY TubeType ORDER BY TubeType ASC", data.designid, UCase(data.blindtype).ToString(), data.brackettype)
                    resultList = GetFormattedData(query, "TubeType", "TubeType")

                Case "controltype"
                    query = String.Format("SELECT ControlType FROM HardwareKits WHERE DesignId = '{0}' AND BlindId='{1}' AND BracketType='{2}' AND TubeType='{3}' AND Active=1 GROUP BY ControlType ORDER BY ControlType ASC", data.designid, UCase(data.blindtype).ToString(), data.brackettype, data.tubetype)
                    resultList = GetFormattedData(query, "ControlType", "ControlType")

                Case "colourtype"
                    query = String.Format("SELECT Id, ColourType FROM HardwareKits WHERE BlindId = '{1}' AND BracketType = '{2}' AND TubeType = '{3}' AND ControlType='{4}' AND Active=1 ORDER BY Name ASC", data.designid, UCase(data.blindtype).ToString(), data.brackettype, data.tubetype, data.controltype)
                    resultList = GetFormattedData(query, "Id", "ColourType")

                Case "fabrictype"
                    query = String.Format("SELECT Type FROM Fabrics WHERE DesignId='{0}' AND Active='1' GROUP BY Type ORDER BY Type ASC", data.designid)
                    resultList = GetFormattedData(query, "Type", "Type")

                Case "fabriccolour"
                    query = String.Format("SELECT Id, Colour FROM Fabrics WHERE DesignId='{0}' AND Active='1' AND Type='{1}' ORDER BY Name ASC", data.designid, data.fabrictype)
                    resultList = GetFormattedData(query, "Id", "Colour")

                Case "railtype"
                    Dim FindBracket As String = data.brackettype
                    If data.brackettype = "Headbox & Side Channels" Then
                        FindBracket = "Headbox &amp; Side Channels"
                    End If
                    
                    If data.brackettype = "With Tube & Bottom Included" Then
                        FindBracket = "With Tube &amp; Bottom Included"
                    End If

                    query = String.Format("SELECT Type FROM Bottoms CROSS APPLY STRING_SPLIT(BracketType, ',') WHERE VALUE = '{0}' AND Company = 'SP' AND Trim ='{1}' AND Active ='1' GROUP BY Type ORDER BY Type ASC", FindBracket, data.trim)
                    resultList = GetFormattedData(query, "Type", "Type")

                Case "railcolour"
                    Dim FindBracket As String = data.brackettype
                    If data.brackettype = "Headbox & Side Channels" Then
                        FindBracket = "Headbox &amp; Side Channels"
                    End If
                    
                    If data.brackettype = "With Tube & Bottom Included" Then
                        FindBracket = "With Tube &amp; Bottom Included"
                    End If

                    query = String.Format("SELECT Id, Colour FROM Bottoms CROSS APPLY STRING_SPLIT(BracketType, ',') WHERE VALUE = '{0}' AND Type='{1}' AND Company = 'SP' AND Trim ='{2}' AND Active ='1' ORDER BY Name ASC", FindBracket, data.railtype, data.trim)
                    resultList = GetFormattedData(query, "Id", "Colour")

                Case Else
                    Return New With {.error = true, .message = "Invalid field"}
            End Select

            Return New With {
                .error = false,
                .list = resultList
            }
        Catch ex As Exception
            Return New With {.error = true, .message = String.Format("BindListData: {0}", ex.Message)}
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




End Class
