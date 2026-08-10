Imports System.IO
Imports Microsoft.VisualBasic.FileIO
Imports System.Web.Services
Imports OfficeOpenXml
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Globalization
Imports System.Linq ' Pastikan ini ada di bagian atas file Anda untuk LINQ
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports iTextSharp.tool.xml
Imports Microsoft.VisualBasic
Imports Newtonsoft.Json
Imports System.Net
Imports System.Net.Mail
Imports System.Net.Http
Imports System.Text
Imports System.Xml
Partial Class Methods_Order_OrderDetailMethod
    Inherits System.Web.UI.Page
    Shared orderCfg As New OrderConfig()
    Shared exactCfg As New ExactConfig()
    Shared publicCfg As New PublicConfig()
    Shared printCfg As New PrintConfig()
    Shared jobsheet As New HalperJobSheetRenderer()
    Shared enUS As CultureInfo = New CultureInfo("en-US")
    Public Shared myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    '#--- Response Message ---#
    Public Class ErrorDetail
        Public Property message As String
        Public Property field As String
    End Class

    Public Class ErrorResponse
        Public Property [error] As ErrorDetail
    End Class


    Public Class SuccessDetail
        Public Property message As String
        Public Property url As String
    End Class

    Public Class SuccessResponse
        Public Property success As SuccessDetail
    End Class

    Public Class ParamBindOrderAggregate
        Public Property headerid As String
        Public Property ordertype As String

        Public Property loginid As String
        Public Property rolename As String
        Public Property customercontactid As String
        Public Property applicationid As String
    End Class

    Public Class ParamUpdateStatusOrder
        Public Property id  As String
        Public Property status As String
        Public Property statusOld As String
        Public Property submitteddate As String
        Public Property completeddate As String
        Public Property canceleddate As String
        Public Property description As String

        Public Property username As String
        Public Property loginid As String
    End Class

    Public Class ParamSubmitOverrideDisc
        Public Property discount  As String

        Public Property rolename As String
        Public Property headerid As String
        Public Property loginid As String
    End Class

    Public Class ParamSubmitChangeProductionDate
        Public Property productiondate  As String

        Public Property rolename As String
        Public Property headerid As String
        Public Property loginid As String
    End Class

    Public Class ParamSubmitSendMailQuote
        Public Property id  As String
        Public Property from As String
        Public Property mailto As String
        Public Property cc As String

        Public Property username As String
        Public Property headerid As String
        Public Property loginid As String
    End Class

    Public Class ParamFindProductForm
        Public Property id  As String
        Public Property rolename As String
        Public Property headerid As String
        Public Property ordertype As String
        Public Property action As String
        Public Property designid As String
        Public Property production As String
    End Class

    Public Class ParamOverwritePricing
        Public Property loginid  As String
        Public Property username  As String
        Public Property rolename  As String
        Public Property headerid As String
        Public Property itemid As String
        Public Property qty As String
        Public Property customerid As String
        Public Property details As List(Of PricingDetail)
    End Class
    Public Class PricingDetail
        Public Property id As String
        Public Property type As String
        Public Property poa As Decimal
    End Class

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function GetItemData(ByVal query As String) As Object
        Try
            Dim Item As String = publicCfg.GetItemData(query)
            Return Item
        Catch ex As Exception
            Return New With { .error = New With { .message = ex.Message}}
        End Try
    End Function

    Private Shared Function InArray(value As String, ParamArray list() As String) As Boolean
        Return list.Contains(value)
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindOrderAggregate(ByVal data As ParamBindOrderAggregate) As Object
        Try
            Dim HeaderData As Object
            Dim DetailData As New List(Of Object)()
            Dim OtherData As Object


            Dim QueryHeader As String = <sql>
                SELECT 
                    h.Id,
                    h.CustomerName,
                    h.CustomerId,
                    h.OrderId,
                    h.JoNumberId,
                    h.OrderType,
                    h.OrderNumber,
                    h.OrderName,
                    h.CreatedDate,
                    h.CreatedBy,
                    cl.FullName AS CreatedByName,
                    h.OrderNote,
                    h.StatusAdditional,
                    h.Status,
                    h.Delivery,
                    h.QuoteDisc,
                    h.SubmittedDate,
                    h.JobDate,
                    h.CompletedDate,
                    h.CanceledDate,

                    ISNULL(SUM(odp.Cost * odp.Qty), 0) 
                    - ISNULL(SUM((odp.Discount + odp.DiscountB + odp.DiscountC) * odp.Qty), 0) 
                    AS SumPrice

                FROM view_order_headers h
                LEFT JOIN CustomerLogins cl ON h.CreatedBy = cl.Id
                LEFT JOIN OrderDetails od ON od.HeaderId = h.Id AND od.Active = 1
                LEFT JOIN OrderDetailsPrice odp ON odp.ItemId = od.Id AND odp.HeaderId = h.Id

                WHERE h.Id = @Id 
                AND h.OrderType = @OrderType 
                AND h.Active = 1

                GROUP BY 
                    h.Id, h.CustomerName, h.CustomerId, h.OrderId,
                    h.JoNumberId, h.OrderType, h.OrderNumber, h.OrderName,
                    h.CreatedDate, h.CreatedBy, cl.FullName,
                    h.OrderNote, h.StatusAdditional, h.Status, h.Delivery, h.QuoteDisc,
                    h.SubmittedDate, h.JobDate, h.CompletedDate, h.CanceledDate
            </sql>.Value

            Using conn As New SqlConnection(myConn)
                Using cmd As New SqlCommand(QueryHeader, conn)
                    cmd.Parameters.AddWithValue("@Id", data.headerid)
                    cmd.Parameters.AddWithValue("@OrderType", data.ordertype)

                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim sumPrice As Decimal = Convert.ToDecimal(reader("SumPrice"))
                            Dim gst As Decimal = sumPrice * 0.1D
                            Dim finalTotal As Decimal = sumPrice + gst

                            HeaderData = New With {
                                .Id = reader("Id").ToString(),
                                .CustomerName = reader("CustomerName").ToString(),
                                .CustomerId = reader("CustomerId").ToString(),
                                .OrderId = reader("OrderId").ToString(),
                                .JoNumberId = reader("JoNumberId").ToString(),
                                .OrderType = reader("OrderType").ToString(),
                                .OrderNumber = reader("OrderNumber").ToString(),
                                .OrderName = reader("OrderName").ToString(),
                                .CreatedDate = reader("CreatedDate").ToString(),
                                .CreatedByName = reader("CreatedByName").ToString(),
                                .CreatedBy = reader("CreatedBy").ToString(),
                                .OrderNote = reader("OrderNote").ToString(),
                                .StatusAdditional = reader("StatusAdditional").ToString(),
                                .Status = reader("Status").ToString(),
                                .Delivery = reader("Delivery").ToString(),
                                .SubmittedDate = reader("SubmittedDate").ToString(),
                                .QuoteDisc = reader("QuoteDisc").ToString(),
                                .JobDate = reader("JobDate").ToString(),
                                .CompletedDate = reader("CompletedDate").ToString(),
                                .CanceledDate = reader("CanceledDate").ToString(),
                                .SumPrice = sumPrice,
                                .Gst = gst,
                                .FinalTotal = finalTotal
                            }
                        End If
                    End Using
                End Using
            End Using


            Dim QueryDetails As String = <sql>
                SELECT
                    vd.Id,
                    vd.HeaderId,
                    vd.DesignId,
                    vd.BlindId,
                    vd.Qty,
                    vd.Location,
                    vd.Mounting,
                    vd.DesignName,
                    vd.BlindName,
                    vd.KitName,
                    vd.BracketType,
                    vd.TubeType,
                    vd.ControlType,
                    vd.FabricType,
                    vd.BlindNo,
                    vd.UniqueId,
                    vd.Width,
                    vd.[Drop],
                    vd.FrameColour,
                    vd.PanelSize,
                    vd.PelmetType,
                    vd.BottomTrackType,
                    vd.MeshType,
                    vd.FrameType,

                    ISNULL(odp.Cost, 0) AS Cost,
                    ISNULL(odp.Charge, 0) AS Charge,
                    ISNULL(odp.Discount, 0) AS Discount,

                    vd.Markup,
                    vd.FabricGroups,
                    vd.OrderDelivery,
                    vd.PriceGroupName

                FROM view_details vd

                LEFT JOIN (
                    SELECT
                        ItemId,
                        SUM(CASE WHEN Type = @Matrix THEN Qty * Cost ELSE 0 END) AS Cost,
                        SUM(CASE WHEN Type = @Charge THEN Qty * Cost ELSE 0 END) AS Charge,
                        SUM((Discount + DiscountB + DiscountC) * Qty) AS Discount
                    FROM OrderDetailsPrice
                    GROUP BY ItemId
                ) odp ON vd.Id = odp.ItemId
                WHERE
                    Active=@Active 
                    AND HeaderId=@HeaderId
                ORDER BY vd.Id, vd.BlindNo, vd.DesignName ASC
            </sql>.Value

            Using conn As New SqlConnection(myConn)
                Using cmd As New SqlCommand(QueryDetails, conn)
                    cmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                    cmd.Parameters.AddWithValue("@Active", "1")
                    cmd.Parameters.AddWithValue("@Matrix", "Matrix")
                    cmd.Parameters.AddWithValue("@Charge", "Charge")

                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim Id As String = reader("Id").ToString()
                            Dim HeaderId As String = reader("HeaderId").ToString()
                            Dim DesignId As String = reader("DesignId").ToString()
                            Dim DesignName As String = reader("DesignName").ToString()
                            Dim FabricGroups As String = reader("FabricGroups").ToString()
                            Dim PriceGroupName As String = reader("PriceGroupName").ToString()

                            Dim Product As Object = FindProduct(reader)
                            If Product.error Then
                                Throw New Exception(Product.message)
                            End If

                            Dim HideNext As String = FindHideNext(reader, data.rolename, HeaderData.CreatedByName, data.customercontactid)
                            Dim TextNext As String = FindTextNext(reader)
                            Dim Production As String = "Sunlight"
                            If InStr(DesignName, "Global") > 0 Then
                                Production = "Global"
                            End If
                            Product.product += String.Format("<br><button type='button' class='btn btn-sm btn-outline-success mt-1' id='btnNextItem' data-id='{0}' data-headerid='{1}' data-designid='{2}' data-next='{3}' data-production='{4}' {5}><i class='bi bi-node-plus me-1'></i>Next Item</button>", Id, HeaderId, DesignId, TextNext, Production, HideNext)

                            Dim BaseCost As String = FindBaseCost(reader)
                            Dim Cost As String = FindCost(reader)
                            Dim FinalCost As String =  Cost
                            If FabricGroups = "POA" OR InStr(PriceGroupName, "POA") > 0 Then
                                Dim baseCostValue As Decimal
                                If Decimal.TryParse(BaseCost, baseCostValue) AndAlso baseCostValue = 0D Then
                                    FinalCost = "<span class='badge bg-orange-lt'>POA</span>"
                                End If
                            End If



                            
                            Dim Markup As String = reader("Markup").ToString()
                            If MarkUp = "0" Then MarkUp = ""

                            DetailData.Add(New With {
                                .Id = Id,
                                .HeaderId = HeaderId,
                                .CustomerContactId = data.customercontactid,
                                .StatusHeader = HeaderData.Status,
                                .DesignId = DesignId,
                                .BlindId = reader("BlindId").ToString(),
                                .DesignName = DesignName,
                                .Qty = reader("Qty").ToString(),
                                .Location = reader("Location").ToString(),
                                .Product = Product.product,
                                .HideNext = HideNext,
                                .TextNext = TextNext,
                                .Cost = FinalCost,
                                .Markup = Markup,
                                .Group = reader("FabricGroups").ToString(),
                                .OrderDelivery = reader("OrderDelivery").ToString(),
                                .PriceGroupName = reader("PriceGroupName").ToString()
                            })
                        End While
                    End Using
                End Using
            End Using

            Dim ResCheckOrder As Object = CekOrder(HeaderData.Id, HeaderData.Status, data.loginid, data.rolename)
            If ResCheckOrder.error Then
                Throw New Exception(ResCheckOrder.message)
            End If
            
            Dim SendMailQuote As Object = FindMailQuote(data.applicationid, HeaderData.CustomerId)
            If SendMailQuote.error Then 
                Throw New Exception(SendMailQuote.message)
            End If

            Dim Logs As Object = FindLogs(data.headerid, data.ordertype)
            If Logs.error Then 
                Throw New Exception(Logs.message)
            End If

            Dim Designs As Object = FindDesigns(HeaderData.CustomerId, data.ordertype, data.rolename)
            If Designs.error Then 
                Throw New Exception(Designs.message)
            End If
           

           

            OtherData = New With {
                .ResCheckOrder = ResCheckOrder,
                .SendMailQuote = SendMailQuote,
                .Logs = Logs,
                .Designs = Designs
            }


            Return New With {
                .header = HeaderData,
                .detail = DetailData,
                .other = OtherData
            }
        Catch ex As Exception
            Return New With {.error = True, .message = ex.Message}
        End Try
    End Function

    Private Shared Function CekOrder(ByVal headerid As String, ByVal status As String, ByVal loginid As String, ByVal rolename As String) As Object
        Try
            Dim msg As String = String.Empty
            Dim url As String = String.Empty
            Dim Action As String = String.Empty
            Dim textSwall As String = String.Empty
            Dim CustomerContactId As String = If(HttpContext.Current.Session("CustomerContactId") IsNot Nothing, HttpContext.Current.Session("CustomerContactId").ToString(), String.Empty)

            Dim detailData As DataSet = publicCfg.GetListData("SELECT Id, UniqueId, DesignName, BracketType, FabricGroups FROM view_details WHERE HeaderId='" + headerid + "' AND Active=1 ORDER BY Id ASC")
            If detailData.Tables(0).Rows.Count < 1  Then Return New With {.error = false, .Action = Action, .Message = "order details not found"}

            For i As Integer = 0 To detailData.Tables(0).Rows.Count - 1
                Dim Id As String = detailData.Tables(0).Rows(i).Item("Id").ToString()
                Dim UniqueId As String = detailData.Tables(0).Rows(i).Item("UniqueId").ToString()
                Dim DesignName As String = detailData.Tables(0).Rows(i).Item("DesignName").ToString()
                Dim BracketType As String = detailData.Tables(0).Rows(i).Item("BracketType").ToString()
                Dim FabricGroups As String = detailData.Tables(0).Rows(i).Item("FabricGroups").ToString()

                If FabricGroups = "POA" Then
                    Dim Poa As Integer = publicCfg.GetItemData(String.Format("SELECT Poa FROM OrderDetailsPrice where HeaderId={0} AND ItemId={1}", headerid, Id))
                    If Poa = 0 Then
                        Using thisConn As New SqlConnection(myConn)
                            Using myCmd As New SqlCommand("UPDATE OrderHeaders SET Status='Pending Price Approval' WHERE Id = @Id")
                                myCmd.Parameters.AddWithValue("@Id", headerid)
                                myCmd.Connection = thisConn
                                thisConn.Open()
                                myCmd.ExecuteNonQuery()
                                thisConn.Close()
                            End Using
                        End Using
                    End If
                    If Poa > 0  Then
                        Using thisConn As New SqlConnection(myConn)
                            Using myCmd As New SqlCommand("UPDATE OrderHeaders SET Status='Draft' WHERE Id = @Id")
                                myCmd.Parameters.AddWithValue("@Id", headerid)
                                myCmd.Connection = thisConn
                                thisConn.Open()
                                myCmd.ExecuteNonQuery()
                                thisConn.Close()
                            End Using
                        End Using
                    End If
                End If

                Dim TotalBlind As Integer = publicCfg.GetItemData("SELECT COUNT(*) FROM view_details WHERE UniqueId = '" + UniqueId + "' AND Active = 1")
                If DesignName = "Roller Blinds" or DesignName = "Global Roller Blinds" Then

                    If BracketType = "Double" Or BracketType = "Linked 2 Blinds (Dep)" Or BracketType = "Linked 2 Blinds (Ind)" Then
                        If TotalBlind < 2 Then
                            Action = "Yes"
                            msg += "<b> " & Id & "</b>,"
                        End If
                    End If

                    If BracketType = "Linked 3 Blinds (Dep)" Or BracketType = "Linked 3 Blinds (Ind)" Then
                        If TotalBlind < 3 Then
                            Action = "Yes"
                            msg += "<b> " & Id & "</b>,"
                        End If
                    End If

                    If BracketType = "Double and Link System Dep" Or BracketType = "Double and Link System Ind" Then
                        If TotalBlind < 4 Then
                            Action = "Yes"
                            msg += "<b> " & Id & "</b>,"
                        End If
                    End If

                End If

                If Not status = "Draft" Then
                    Action = "No"
                End If

                textSwall = "You have an incomplete roller blinds order, which is on the ITEM ID "+ msg +" <br /><br />If you want to complete it, please click the <b>Next Item</b> button on the order line ID."
                
            Next

            Return New With {.error = false, .Action = Action, .Message = textSwall}
        Catch ex As Exception
            Return New With {.error = true, .message = String.Format("CekOrder: {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function FindMailQuote(ByVal appid As String, ByVal customerid As String) As Object
        Try
            Dim Mailings As DataSet = publicCfg.GetListData(String.Format("SELECT Id, Server FROM Mailings WHERE ApplicationId ='{0}' AND Name = 'Quote Order Shutters' AND Active = 1", appid))
            Dim MailId As String = ""
            Dim MailFrom As String = ""
            Dim MailTo As String = publicCfg.GetItemData(String.Format("SELECT Email FROM CustomerContacts WHERE CustomerId = '{0}' AND [Primary]=1", customerid))
            If Mailings.Tables(0).Rows.Count > 0 Then
                MailId = Mailings.Tables(0).Rows(0).Item("Id").ToString()
                MailFrom = Mailings.Tables(0).Rows(0).Item("Server").ToString()
            End If

            Return New With {.error = false, .MailId = MailId, .MailFrom = MailFrom, .MailTo = MailTo}
        Catch ex As Exception
            Return New With {.error = true, .message = String.Format("FindMailQuote: {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function FindLogs(ByVal headerid As String, ByVal ordertype As String) As Object
        Try
            Dim Logs As DataSet = publicCfg.GetListData(String.Format("SELECT CustomerLogins.FullName, Log_Orders.ItemId, Log_Orders.ActionDate, Log_Orders.Description FROM Log_Orders INNER JOIN CustomerLogins ON Log_Orders.ActionBy=CustomerLogins.Id WHERE Log_Orders.HeaderId='{0}' AND Log_Orders.Type='{1}'  ORDER BY ActionDate DESC", headerid, ordertype))

            Dim LogsData As List(Of Dictionary(Of String, Object)) = New List(Of Dictionary(Of String, Object))()
            For Each row As DataRow In Logs.Tables(0).Rows
                Dim dict As New Dictionary(Of String, Object)()
                For Each col As DataColumn In Logs.Tables(0).Columns
                    dict.Add(col.ColumnName, row(col))
                Next
                LogsData.Add(dict)
            Next

            Return New With {.error = false, .LogsData = LogsData}
        Catch ex As Exception
            Return New With {.error = true, .message = String.Format("FindLogs: {0}", ex.Message)}
        End Try
    End Function
    
    Private Shared Function FindDesigns(ByVal customerid As String, ByVal ordertype As String, ByVal rolename As String) As Object
        Try
            Dim Env As String = ""
            If rolename = "Customer" Then
                Env = "AND Designs.Description = 'Environment : Production'"
            End If
            If InArray(rolename, "PPIC & DE", "Manager", "Customer Service") Then
                Env = "AND Designs.Description IN ('Environment : Production', 'Environment : Testing')"
            End If

            Dim datas As DataSet = publicCfg.GetListData(String.Format("SELECT Designs.Id, Designs.Name FROM CustomerProductAccess CROSS APPLY STRING_SPLIT ( CustomerProductAccess.DesignId, ',' ) AS designArray INNER JOIN Designs ON designArray.VALUE = Designs.Id WHERE CustomerProductAccess.Id = '{0}' AND Designs.Type <> 'Additional' AND Designs.Company = 'SP' AND Designs.Type = '{1}' {2} AND Designs.Active = 1 ORDER BY Designs.Name ASC", customerid, ordertype, Env))

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
            Return New With {.error = false, .list = list}
        Catch ex As Exception
            Return New With {.error = true, .message = String.Format("FindDesigns: {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function FindProduct(reader As SqlDataReader) As Object
        Try
            Dim Id As String = reader("Id").ToString()
            Dim HeaderId As String = reader("HeaderId").ToString()
            Dim DesignId As String = reader("DesignId").ToString()
            Dim BlindId As String = reader("BlindId").ToString()
            Dim Mounting As String = reader("Mounting").ToString()
            Dim DesignName As String = reader("DesignName").ToString()
            Dim BlindName As String = reader("BlindName").ToString()
            Dim KitName As String = reader("KitName").ToString()
            Dim BracketType As String = reader("BracketType").ToString()
            Dim TubeType As String = reader("TubeType").ToString()
            Dim ControlType As String = reader("ControlType").ToString()
            Dim FabricType As String = reader("FabricType").ToString()
            Dim BlindNo As String = reader("BlindNo").ToString()
            Dim UniqueId As String = reader("UniqueId").ToString()
            Dim Width As String = reader("Width").ToString()
            Dim Drop As String = reader("Drop").ToString()
            Dim PanelSize As String = reader("PanelSize").ToString()
            Dim FrameType As String = reader("FrameType").ToString()
            Dim BottomTrackType As String = reader("BottomTrackType").ToString()
            Dim PelmetType As String = reader("PelmetType").ToString()
    
            Dim Size As String = String.Format("({0} x {1})", Width, Drop)
            Dim Product As String = String.Format("{0} {1}", KitName, Size)
    
            If DesignName = "Additional" Then
                Product = String.Format("{0}", KitName)
                If BlindName = "Long Length Surcharge" Then
                    Dim CustomerId As String = publicCfg.GetItemData(String.Format("SELECT StoreId FROM OrderHeaders WHERE Id = '{0}'", HeaderId))
                    Dim States As String = publicCfg.GetItemData(String.Format("SELECT States FROM CustomerAddress WHERE CustomerId = '{0}'", CustomerId))
                    Product = String.Format("{0} {1}", KitName, States)
                End IF
    
                Product = String.Format("Surcharge - {0}", Product)
            End If
    
            If DesignName = "Surcharge" Then
                Product = String.Format("{0}", KitName)
                If InArray(BlindName, "Thrid Party Delivery", "Overlength Surcharge") Then
                    If TubeType = "Roller" Then
                        Dim CustomerId As String = publicCfg.GetItemData(String.Format("SELECT StoreId FROM OrderHeaders WHERE Id = '{0}'", HeaderId))
                        Dim States As String = publicCfg.GetItemData(String.Format("SELECT States FROM CustomerAddress WHERE CustomerId = '{0}'", CustomerId))
                        Product = String.Format("{0} {1} #{2}", BlindName, States, TubeType)
                        
                        If BlindName = "Overlength Surcharge" Then
                            Product = String.Format("{0} {3} {1} #{2}", BlindName, States, TubeType, ControlType)
                        End If
                    End If
                End IF
    
                Product = String.Format("Surcharge - {0}", Product)
            End If
    
            If DesignName = "Aluminium Blinds" Or DesignName = "Venetian Blinds" Then
                Product = String.Format("{0} {1}", KitName, Size)
            End If
    
            If DesignName = "Roller Blinds" Or DesignName = "Global Roller Blinds" Then
                Product = String.Format("{0} #{1} {2}", KitName, FabricType, Size)
    
                '#Linked 3 Blinds (Dep) & Linked 3 Blinds (Ind)
                If BracketType = "Linked 3 Blinds (Dep)" Or BracketType = "Linked 3 Blinds (Ind)" Then
                    '#blind 1
                    If BlindNo = "Blind 1" Then
                        Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                        Dim getConnectedId2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                        If Not getConnectedId2 = "" Then
                            getConnectedId2 = " & ITEM ID " & getConnectedId2
                        End If
                        If Not getConnectedId = "" Then
                            Product += "<br />"
                            Product += "<small style='color:red;'>* LINKED ITEM ID " & getConnectedId & getConnectedId2 & "</small>"
                        End If
                    End If
    
                    '#blind 2
                    If BlindNo = "Blind 2" Then
                        Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                        Dim getConnectedId2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                        If Not getConnectedId2 = "" Then
                            getConnectedId2 = " & ITEM ID " & getConnectedId2
                        End If
                        If Not getConnectedId = "" Then
                            Product += "<br />"
                            Product += "<small style='color:red;'>* LINKED ITEM ID " & getConnectedId & getConnectedId2 & "</small>"
                        End If
                    End If
    
                    '#blind 3
                    If BlindNo = "Blind 3" Then
                        Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                        Dim getConnectedId2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                        If Not getConnectedId2 = "" Then
                            getConnectedId2 = " & ITEM ID " & getConnectedId2
                        End If
                        If Not getConnectedId = "" Then
                            Product += "<br />"
                            Product += "<small style='color:red;'>* LINKED ITEM ID " & getConnectedId & getConnectedId2 & "</small>"
                        End If
                    End If
                End If
    
                '#Double and Link System Dep & Double and Link System Ind
                If BracketType = "Double and Link System Dep" Or BracketType = "Double and Link System Ind" Then
                    '#blinds 1
                    If BlindNo = "Blind 1" Then
                        Dim blind2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                        Dim blind3 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                        Dim blind4 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 4' AND UniqueId = '" + UniqueId + "' AND Active = 1")
    
                        Dim spare As String = ""
                        If Not blind3 = "" Then
                            blind3 = "ITEM ID " & blind3
                            spare = " & "
                        End If
                        If Not blind4 = "" Then
                            blind4 = " & ITEM ID " & blind4
                            spare = ", "
                        End If
                        If Not blind2 = "" Then
                            Product += "<br />"
                            Product += "<small style='color:red;'>* LINKED ITEM ID " & blind2 & spare & blind3 & blind4 & "</small>"
                        End If
                    End If
    
                    '#blinds 2
                    If BlindNo = "Blind 2" Then
                        Dim blind1 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                        Dim blind3 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                        Dim blind4 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 4' AND UniqueId = '" + UniqueId + "' AND Active = 1")
    
                        Dim spare As String = ""
                        If Not blind3 = "" Then
                            blind3 = "ITEM ID " & blind3
                            spare = " & "
                        End If
                        If Not blind4 = "" Then
                            blind4 = " & ITEM ID " & blind4
                            spare = ", "
                        End If
                        If Not blind1 = "" Then
                            Product += "<br />"
                            Product += "<small style='color:red;'>* LINKED ITEM ID " & blind1 & spare & blind3 & blind4 & "</small>"
                        End If
                    End If
    
                    '#blinds 3
                    If BlindNo = "Blind 3" Then
                        Dim blind1 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                        Dim blind2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                        Dim blind4 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 4' AND UniqueId = '" + UniqueId + "' AND Active = 1")
    
                        Dim spare As String = ""
                        If blind4 = "" Then
                            spare = " & "
                        End If
                        If Not blind4 = "" Then
                            blind4 = " & ITEM ID " & blind4
                            spare = ", "
                        End If
                        If Not blind2 = "" Then
                            blind2 = "ITEM ID " & blind2
                        End If
                        If Not blind1 = "" Then
                            Product += "<br />"
                            Product += "<small style='color:red;'>* LINKED ITEM ID " & blind1 & spare & blind2 & blind4 & "</small>"
                        End If
                    End If
    
                    '#blinds 4
                    If BlindNo = "Blind 4" Then
                        Dim blind1 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                        Dim blind2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                        Dim blind3 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                        Product += "<br />"
                        Product += "<small style='color:red;'>* LINKED ITEM ID " & blind1 & ", ITEM ID " & blind2 & " & " & "ITEM ID" & blind3 & "</small>"
                    End If
                End If
    
                '#Double, Linked 2 Blinds (Dep), Linked 2 Blinds (Ind)
                If BracketType = "Double" Or BracketType = "Linked 2 Blinds (Dep)" Or BracketType = "Linked 2 Blinds (Ind)" Then
                    If BlindNo = "Blind 1" Then
                        Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                        If Not getConnectedId = "" Then
                            Product += "<br />"
                            Product += "<small style='color:red;'>* Complete set with ITEM ID " & getConnectedId & "</small>"
                        End If
                    End If
    
                    If BlindNo = "Blind 2" Then
                        Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                        If Not getConnectedId = "" Then
                            Product += "<br />"
                            Product += "<small style='color:red;'>* Complete set with ITEM ID " & getConnectedId & "</small>"
                        End If
                    End If
                End If
    
                If BracketType = "With Tube & Bottom Included" then
                    Product = "Roller Skin Only (+Tube & Bottom Inc) #" & FabricType & " (" & Width & " x " & Drop & ")"
                End If
    
    
                If BracketType = "With Bottom Included" then
                    Product = "Roller Skin Only (+Bottom Inc) #" & FabricType & " (" & Width & " x " & Drop & ")"
                End If
    
                If BracketType = "With Tube Included" then
                    Product = "Roller Skin Only (+Tube Inc) #" & FabricType & " (" & Width & " x " & Drop & ")"
                End If
    
            End If
    
            If InArray(DesignName, "Veri Shades", "Vertical Blinds", "Global Vertical Blinds") Then
                Product = String.Format("{0} #{1} {2}", KitName, FabricType, Size)
                If BlindName = "Slat Only" Then
                    Product = String.Format("{0} #{1} (Drop : {2}mm)", KitName, FabricType, Drop)
                End If
                If BlindName = "Track Only" Then
                    Product = String.Format("{0} (Width : {1}mm)", KitName, Width)
                End If
            End If
    
            If InArray(DesignName, "Panel Glides", "Global Panel Glides", "Roman Blinds", "Global Roman Blinds", "Lumen") Then
                Product = String.Format("{0} #{1} {2}", KitName, FabricType, Size)
                If BlindName = "Track Only" Then
                    Product = String.Format("{0} (Width : {1}mm)", KitName, Width)
                End If
            End If
    
            If DesignName = "Cellular Blinds" Then
                Product = String.Format("{0} #{1} {2}", KitName, FabricType, Size)
                If BlindName = "Cellora" Then
                    Product = String.Format("{0} {1} #{2} {3}", BlindName, controltype, FabricType, Size)
                End If
                If BlindName = "Galaxy" Then
                    Product = String.Format("{0} ({1}) {2} #{3} {4}", BlindName, BracketType, ControlType, FabricType, Size)
                End If
                If BlindName = "Potrait" Then
                    Product = String.Format("{0} ({1}) {2} #{3} {4}", BlindName, BracketType, ControlType, FabricType, Size)
                End If
            End If
    
            If DesignName = "Window" Then
                Product = String.Format("{0} - {1} {2} ", BlindName, TubeType, Size)
    
                If TubeType = "Flyscreens" Then
                    Product = String.Format("{0} - {1} #{2} {3} ", BlindName, TubeType, FrameType, Size)
                End If
    
                If TubeType = "Retractable Flyscreen Pleated" Then
                    Product = String.Format("{0} - {1} #{2} {3}", BlindName, TubeType, BottomTrackType, Size)
                End If
            End If
    
            If DesignName = "Door" Then
                Product = String.Format("{0} - {1} #{2} {3}", BlindName, TubeType, ControlType, Size)
                IF ControlType = "N/A" Then
                    Product = String.Format("{0} - {1} {2}", BlindName, TubeType, Size)
                End IF
    
                If TubeType = "Retractable Pleated" Then
                    Product = String.Format("{0} - {1} #{2} {3}", BlindName, TubeType, BottomTrackType, Size)
                End If
            End If
    
            If DesignName = "Supply Only" Then
                If BlindName = "Mesh Only" AND NOT TubeType = "Ultra Barrier Mesh" Then
                    Product = String.Format("{0} - {1} (Width: {2}mm x Length: {3}lm)", DesignName, TubeType, Width, PanelSize)
                End If
            End If
    
            If InArray(DesignName, "Curtain", "Pelmet") Then
                Product = String.Format("{0} #{1} {2}", KitName, FabricType, Size)
                If BlindName = "Track Only" Then
                    Product = String.Format("{0} ({1}mm)", KitName, Width)
                End If
    
                If BlindName = "Uniline Pelmet" Then
                    IF String.IsNullOrEmpty(FabricType) Then FabricType = "No Fabric"
                    Product = String.Format("{0} {1} #{2} (Width:{3}mm)", KitName, PelmetType, FabricType, Width)
                End If
    
                If BlindName = "Fashade Pelmet" Then
                    Product = String.Format("{0} {1} (Width:{2}mm)", KitName, Mounting, Width)
                End If
            End If
    
            '#Final Product Name
            If InStr(DesignName, "Global") > 0 Then
                Product = String.Format("Global - {0}", Product)
            End IF
    
    
            Return New With {.error = false, .product = Product}
        Catch ex As Exception
            Return New With {.error = true, .message = String.Format("FindProduct: {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function FindHideNext(reader As SqlDataReader, rolename As String, createdby As String, customercontactid As String) As String
        Dim HideNext As String = "hidden"

        Dim DesignName As String = reader("DesignName").ToString()
        Dim BracketType As String = reader("BracketType").ToString()
        Dim UniqueId As String = reader("UniqueId").ToString()
        Dim BlindNo As String = reader("BlindNo").ToString()

        If DesignName = "Roller Blinds" Or DesignName = "Global Roller Blinds" Then

            Dim TotalBlind As Integer = Convert.ToInt32(publicCfg.GetItemData("SELECT COUNT(Id) FROM OrderDetails WHERE UniqueId = '" + UniqueId + "' AND Active = 1"))
            If BracketType = "Double" Or BracketType = "Linked 2 Blinds (Ind)" Or BracketType = "Linked 2 Blinds (Dep)" Then
                HideNext = ""
                If TotalBlind >= 2 Then : HideNext = "hidden" : End If 
            End If

            If BracketType = "Linked 3 Blinds (Ind)" Or BracketType = "Linked 3 Blinds (Dep)" Then
                HideNext = "hidden"
                If BlindNo = "Blind 1" And TotalBlind < 2 Then
                    HideNext = ""
                End If
                If BlindNo = "Blind 2" And TotalBlind < 3 Then
                    HideNext = ""
                End If
            End If

            If BracketType = "Double and Link System Dep" Or BracketType = "Double and Link System Ind" Then 'added 240925
                HideNext = "hidden"
                If BlindNo = "Blind 1" And TotalBlind < 2 Then
                    HideNext = ""
                End If
                If BlindNo = "Blind 2" And TotalBlind < 3 Then
                    HideNext = ""
                End If
                If BlindNo = "Blind 3" And TotalBlind < 4 Then
                    HideNext = ""
                End If
            End If

        End If


        Return HideNext
    End Function

    Private Shared Function FindTextNext(reader As SqlDataReader) As String
        Dim TextNext As String = "Add blind that is doubled to this blind"
        Dim BracketType As String = reader("BracketType").ToString()
        Dim BlindNo As String = reader("BlindNo").ToString()

        If BracketType = "Linked 2 Blinds (Ind)" Or BracketType = "Linked 2 Blinds (Dep)" Then
            TextNext = "Add 2nd blind that is linked to this blind"
        End If

        If BracketType = "Linked 3 Blinds (Ind)" Or BracketType = "Linked 3 Blinds (Dep)" Then
            TextNext = "Add 2nd blind that is linked to this blind"
            If BlindNo = "Blind 2" Then
                TextNext = "Add to complete blind"
            End If
        End If

        If BracketType = "Double and Link System Dep" Or BracketType = "Double and Link System Ind" Then 'added 240925
            TextNext = "Add a 2rd blind connected to this blind"
            If BlindNo = "Blind 2" Then
                TextNext = "Add a 3rd blind connected to this blind"
            End If
            If BlindNo = "Blind 3" Then
                TextNext = "Add to complete blind"
            End If
        End If


        Return TextNext
    End Function

    Private Shared Function FindBaseCost(reader As SqlDataReader) As String
        Dim Id As String = reader("Id").ToString()
        Dim HeaderId As String = reader("HeaderId").ToString()
        Dim result As String = publicCfg.GetItemData(String.Format("SELECT FORMAT(Cost, 'N2', 'en-US') AS FormatRealCost FROM OrderDetailsPrice WHERE Type ='Matrix' And HeaderId = '{0}' And ItemId = '{1}'", HeaderId, Id))

        Return result
    End Function

    Private Shared Function FindCost(reader As SqlDataReader) As String
        Dim result As String = String.Empty
        Dim DesignName As String = reader("DesignName").ToString()
        Dim BlindName As String = reader("BlindName").ToString()
        Dim Cost As String = reader("Cost").ToString()
        Dim Charge As String = reader("Charge").ToString()
        Dim Discount As String = reader("Discount").ToString()

        Dim totalCost As Decimal = 0.00
        Dim costVal As Decimal = 0
        Dim chargeVal As Decimal = 0
        Dim discountVal As Decimal = 0

        Decimal.TryParse(Cost, costVal)
        Decimal.TryParse(Charge, chargeVal)
        Decimal.TryParse(Discount, discountVal)
        If DesignName = "Vertical Blinds" AndAlso BlindName = "Slat Only" Then
            If costVal = 0 Then
                totalCost = chargeVal
            Else
                totalCost = (costVal + chargeVal) - discountVal
            End If
        Else
            If costVal > 0 Then
                totalCost = (costVal + chargeVal) - discountVal
            End If
        End If

        result = String.Format("${0}", totalCost.ToString("N2", enUS))
        Return result
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function SubmitOrder(ByVal headerid As String, ByVal loginid As String, ByVal rolename As String) As Object
        Try
            If String.IsNullOrEmpty(headerid) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "This order is missing !" }}
            End If
            Dim detailData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + headerid + "' AND Active='1'")

            If detailData.Tables(0).Rows.Count < 1 Then
                Return New With { .warning = true, .message = "Please add item first."}
            End If

            If rolename = "Administrator" Then
                ' Dim ResApi As String = SendOrderGlobal(headerid)
                ' If Not ResApi = "OK" Then
                '     Throw New Exception("API Error: " + ResApi)
                ' End If
                ' Throw New Exception("API Success")
            End If
            
            
            ' Throw New Exception("Debug")
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderHeaders SET Status='New Order',  SubmittedDate=GETDATE() WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", headerid)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Dim dataLog As Object() = {headerid, "", "Blinds", loginid, "Submit Order"}
            orderCfg.Log_Orders(dataLog)

            Return New With { .success = true, .message = "Order has been submitted successfully."}
        Catch ex As Exception
           Return New With { .error = true, .message = ex.Message}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function DeleteOrderHeader(ByVal id As String) As Object
        Try
         

        '#DELETE
        If String.IsNullOrEmpty(id) Then
            Throw New Exception("This order is missing !")
        End If

        If Not String.IsNullOrEmpty(id) Then
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderHeaders SET Active=0 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", id)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using
        End If 

        Return New With { .success = true, .message = "Order has been deleted successfully."}
        Catch ex As Exception
            Return New With { .error = true, .message = ex.Message}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function ReloadPricing(ByVal headerid As String) As Object
        Try
            Dim msg As String = "Reload pricing."
            Dim url As String = ""
            Dim rolename As String = HttpContext.Current.Session("rolename").ToString()

            Dim headerData As DataSet = publicCfg.GetListData("SELECT * FROM view_headers WHERE Id='" & headerid & "'")
            If headerData.Tables(0).Rows.Count < 1 Then
                Throw New Exception("Order Header not found.")
            End If


            Dim status As String = headerData.Tables(0).Rows(0)("Status").ToString()
            Dim customerid As String = headerData.Tables(0).Rows(0)("StoreId").ToString()
            ' If rolename <> "Administrator" AndAlso status <> "Draft" Then
            '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "Permission denied : not administrator."}}
            ' End If

            If status = "Canceled" Then
                Throw New Exception("Order canceled.")
            End If

            ' Ambil semua detail sekaligus
            Dim query As String = "SELECT Id, Mounting, KitName, BlindName, BracketType, TubeType, ControlType, FabricId, FabricIdB, DesignId, BlindId, DesignName, BottomHoldDown, PelmetType, OrderDelivery FROM view_details WHERE HeaderId='" & headerid & "' AND Active='1' ORDER BY Id, BlindNo, DesignName ASC"
            Dim detailData As DataSet = publicCfg.GetListData(query)

            If detailData.Tables(0).Rows.Count < 1 Then
               Return New With {.warning = true, .message = "Please add item first."}
            End If

            For Each row As DataRow In detailData.Tables(0).Rows
                Dim itemId = row("Id").ToString()
                Dim Mounting = row("Mounting").ToString()
                Dim kitName = row("KitName").ToString()
                Dim blindName = row("BlindName").ToString()
                Dim bracketType = row("BracketType").ToString()
                Dim controlType = row("ControlType").ToString()
                Dim tubeType = row("TubeType").ToString()
                Dim fabricId = row("FabricId").ToString()
                Dim fabricIdB = row("FabricIdB").ToString()
                Dim designId = row("DesignId").ToString()
                Dim blindId = row("BlindId").ToString()
                Dim designName = row("DesignName").ToString()
                Dim bottomHold = row("BottomHoldDown").ToString()
                Dim PelmetType = row("PelmetType").ToString()
                Dim OrderDelivery = row("OrderDelivery").ToString()


                Dim fabricGroup = publicCfg.GetFabricGroup(fabricId)
                Dim ListParam As New List(Of Object) From {
                    designName,
                    blindName,
                    bracketType,
                    controlType,
                    tubeType,
                    bottomHold,
                    fabricGroup,
                    PelmetType,
                    Mounting,
                    headerid,
                    KitName
                }
                Dim priceGroupName = GetPriceGroupName(ListParam) 'GetPriceGroupName(designName, blindName, bracketType, controlType, tubeType, bottomHold, fabricGroup)
                If Not String.IsNullOrEmpty(priceGroupName) Then
                    Dim priceGroupId = publicCfg.GetPriceGroupId(designId, priceGroupName)
                    If Not String.IsNullOrEmpty(priceGroupId) Then
                        publicCfg.UpdatePriceGroup(itemId, priceGroupId.ToUpper())
                    End If
                End If

                IF Not fabricIdB = "" Then
                    Dim fabricGroupB = publicCfg.GetFabricGroup(fabricIdB)
                    Dim ListParamB As New List(Of Object) From {
                        designName,
                        blindName,
                        bracketType,
                        controlType,
                        tubeType,
                        bottomHold,
                        fabricGroupB,
                        PelmetType,
                        Mounting,
                        headerid,
                        KitName
                    }
                    Dim priceGroupNameB = GetPriceGroupName(ListParamB) 'GetPriceGroupName(designName, blindName, bracketType, controlType, tubeType, bottomHold, fabricGroupB)
                    If Not String.IsNullOrEmpty(priceGroupNameB) Then
                        Dim priceGroupIdB = publicCfg.GetPriceGroupId(designId, priceGroupNameB)
                        If Not String.IsNullOrEmpty(priceGroupIdB) Then
                            publicCfg.UpdatePriceGroupB(itemId, priceGroupIdB.ToUpper())
                        End If
                    End If
                End If


                ' IF fabricGroup = "POA" OR InStr(priceGroupName, "POA") > 0 Then
                '     Dim Prices As DataSet = publicCfg.GetListData(String.Format("SELECT * FROM OrderDetailsPrice WHERE HeaderId={0} AND ItemId={1} AND Type='Matrix'", headerid, itemId))
                '     If Prices.Tables(0).Rows.Count > 0 Then
                '         Dim Qty As Integer = Convert.ToInt32(Prices.Tables(0).Rows(0)("Qty"))
                '         Dim Cost As Decimal = Convert.ToDecimal(Prices.Tables(0).Rows(0)("Cost"))
                '         Dim Poa As Decimal = Convert.ToDecimal(Prices.Tables(0).Rows(0)("Poa"))


                '         publicCfg.ResetPriceDetail(itemId)
                '         publicCfg.HitungHarga(headerid, itemId)
                '         publicCfg.HitungSurcharge(headerid, itemId)

                '         Dim ListParamDiscount As New List(Of Object) From {
                '             customerid,
                '             "",
                '             Poa,
                '             designId,
                '             blindId
                '         }
                '         Dim Discount As Decimal = publicCfg.HitungDiscount(ListParamDiscount)
                '         Dim Res As String = UpdateOverridePricing(itemId, Poa, Discount)
                '         IF Not Res = "200" Then
                '             Return New ErrorResponse With {.error = New ErrorDetail With {.message = Res}}
                '         End If

                '         Dim Matrix As Decimal = publicCfg.GetItemData(String.Format("SELECT SUM(Cost - Discount) As Matrix FROM OrderDetailsPrice WHERE HeaderId={0} AND ItemId={1} AND Type='Matrix' ", headerid, itemId))
                '         publicCfg.UpdateMatrix(UCase(itemId).ToString(), Qty, Matrix)
                '     End If
                ' Else
                '     publicCfg.ResetPriceDetail(itemId)
                '     publicCfg.HitungHarga(headerid, itemId)
                '     publicCfg.HitungSurcharge(headerid, itemId)
                ' End If

                Dim Prices As DataSet = publicCfg.GetListData(String.Format("SELECT * FROM OrderDetailsPrice WHERE HeaderId='{0}' AND ItemId='{1}' ORDER BY CASE WHEN Type = 'Matrix' THEN 1 WHEN Type = 'Charge' THEN 2 WHEN Type = 'Discount' THEN 3 ELSE 4 END", headerid, itemId))
                If Prices.Tables(0).Rows.Count > 0 Then
                    '#Reset dan Hitung Harga Default
                    publicCfg.ResetPriceDetail(itemId)
                    publicCfg.HitungHarga(headerid, itemId)
                    publicCfg.HitungSurcharge(headerid, itemId)

                    For Each priceRow As DataRow In Prices.Tables(0).Rows
                        '# Simpan Nilai
                        Dim Id As String = priceRow("Id").ToString()
                        Dim Type As String = priceRow("Type").ToString()
                        Dim Description As String = priceRow("Description").ToString()
                        Dim Qty As Integer = Convert.ToInt32(priceRow("Qty"))
                        Dim Cost As Decimal = Convert.ToDecimal(priceRow("Cost"))
                        Dim Poa As Decimal = Convert.ToDecimal(priceRow("Poa"))

                        IF CInt(Poa) > 0 Then
                            '#masukan ulang nilai sebelum di reset
                            Dim ListParamDiscount As New List(Of Object) From {
                                headerid,
                                customerid,
                                "",
                                Poa,
                                designId,
                                blindId
                            }
                            Dim Discount As Decimal = publicCfg.HitungDiscount(ListParamDiscount)
                            Dim DiscountB As Decimal = publicCfg.HitungCustomDiscount(headerid, itemId, (Poa - Discount), Type)

                            If Type = "Charge" Then
                                Discount = DiscountB
                            End If
                            Using thisConn As New SqlConnection(myConn)
                                Using myCmd As New SqlCommand("UPDATE OrderDetailsPrice SET Cost=@Cost, Discount=@Disc, DiscountB=@DiscB, Poa=@Poa WHERE ItemId=@ItemId AND Description=@Description", thisConn)
                                    myCmd.Parameters.AddWithValue("@ItemId", itemId)
                                    myCmd.Parameters.AddWithValue("@Description", Description)
                                    myCmd.Parameters.AddWithValue("@Cost", Poa)
                                    myCmd.Parameters.AddWithValue("@Disc", Discount)
                                    myCmd.Parameters.AddWithValue("@DiscB", DiscountB)
                                    myCmd.Parameters.AddWithValue("@Poa", Poa)
                                    myCmd.Connection = thisConn
                                    thisConn.Open()
                                    myCmd.ExecuteNonQuery()
                                    thisConn.Close()
                                End Using
                            End Using

                            Dim Matrix As String = publicCfg.GetItemData(String.Format("SELECT SUM(( odp.Cost * odp.Qty ) - ( odp.Qty * ISNULL( odp.Discount, 0 ) ) - ( odp.Qty * ISNULL( odp.DiscountB, 0 ) ) - ( odp.Qty * ISNULL( odp.DiscountC, 0 ) )) As Matrix FROM OrderDetailsPrice odp INNER JOIN OrderDetails od ON odp.ItemId=od.id WHERE odp.HeaderId='{0}' AND odp.ItemId='{1}' AND odp.Type='Matrix' AND od.Active='1'", headerid, itemId))
                            
                            Dim Charge As String = publicCfg.GetItemData(String.Format("SELECT SUM(( odp.Cost * odp.Qty ) - ( odp.Qty * ISNULL( odp.Discount, 0 ) ) - ( odp.Qty * ISNULL( odp.DiscountB, 0 ) ) - ( odp.Qty * ISNULL( odp.DiscountC, 0 ) )) As Charge FROM OrderDetailsPrice odp INNER JOIN OrderDetails od ON odp.ItemId=od.Id WHERE odp.HeaderId='{0}' AND odp.ItemId='{1}' AND odp.Type='Charge' AND odp.Description NOT LIKE '%Powder Coating%' AND odp.Description NOT LIKE '%Tracking & Interlock%' AND od.Active='1'", headerid, itemId))

                            publicCfg.UpdateMatrix(itemId, Qty, If(Matrix = "", 0D, CDec(Matrix)))
                            publicCfg.UpdateCharge(itemId, Qty, If(Charge = "", 0D, CDec(Charge)))
                        End If
                    Next
                    
                Else
                    publicCfg.ResetPriceDetail(itemId)
                    publicCfg.HitungHarga(headerid, itemId)
                    publicCfg.HitungSurcharge(headerid, itemId)
                End If
            Next

            Return New With {.success = true, .message = "Reload pricing has been updated successfully."}
        Catch ex As Exception
            Return New With {.error = true, .message = String.Format("ReloadPricing: {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function GetPriceGroupName(ListParam As List(Of Object)) As String
        Dim dname As String = CStr(ListParam(0))
        Dim bname As String = CStr(ListParam(1))
        Dim brackettype As String = CStr(ListParam(2))
        Dim controltype As String = CStr(ListParam(3))
        Dim tube As String = CStr(ListParam(4))
        Dim bottomHold As String = CStr(ListParam(5))
        Dim fabricGroup As String = CStr(ListParam(6))
        Dim pelmetOver As String = CStr(ListParam(7))
        Dim mounting As String = CStr(ListParam(8))
        Dim headerid As String = CStr(ListParam(9))
        Dim kitname As String = CStr(ListParam(10))
        Select Case dname
            Case "Additional"
                If bname = "Long Length Surcharge" Then
                     If InStr(kitName, "Delivery") > 0 Then
                        Return "Long Length Delivery"
                    Else
                        Dim CustomerId As String = publicCfg.GetItemData(String.Format("SELECT StoreId FROM OrderHeaders WHERE Id='{0}'", headerid))
                        Dim CustomerStates As String = publicCfg.GetItemData(String.Format("SELECT States FROM CustomerAddress WHERE CustomerId='{0}' AND [Primary]=1", CustomerId))
                        If CustomerStates = "NSW" Then
                            Return String.Format("{0} NSW", kitname)
                        End If
                        If CustomerStates = "VIC" Or CustomerStates = "QLD" Then
                            Return String.Format("{0} NSW", kitname)
                        End If
                    End If
                End IF

                IF InArray(bname, "Interim Levy Surcharge", "Fashade Pelmet Delivery") Then
                    Return bname
                End If

                If bname = "Uniline Pelmet Delivery" Then
                    Return "Uniline Pelmet Delivery - POA"
                End If
            Case "Cellular Blinds"
                If bname = "Galaxy" Then
                    Return String.Format("{0} {1} - {2}", bname, brackettype, fabricGroup)
                End If
                Return String.Format("{0} {1} - {2}", bname, controltype, fabricGroup) 

            Case "Panel Glides"
                Return String.Format("Panel Glide - {0}", fabricGroup)

            Case "Roller Blinds"
                If bname = "Skin Only" Then Return String.Format("Roller Skin Only - {0}", fabricGroup)
                Return String.Format("Roller Blind - {0}", fabricGroup)

            Case "Roman Blinds"
                Return String.Format("Roman Blind - {0}", fabricGroup)

            Case "Venetian Blinds", "Aluminium Blinds"
                Return bname

            Case "Veri Shades"
                If bname = "Single" Then Return String.Format("Veri Shades - {0}", fabricGroup)
                If bname = "Slat Only" Then Return String.Format("{0} - {1}", bname, fabricGroup)
                Return bname

            Case "Vertical Blinds"
                If bname = "Track Only" Then Return String.Format("{0} - {1}", bname,tube)
                If bname = "Slat Only" AndAlso bottomHold = "Top Hanger Only" Then Return String.Format("{0} With Hanger - {1}", bname, fabricGroup)
                Return String.Format("{0} - {1}", bname, fabricGroup)

            Case "Pelmet"
                ' Dim Delivery As String = publicCfg.GetItemData(String.Format("SELECT Delivery FROM OrderHeaders WHERE Id = '{0}'", headerid))
                ' If Delivery = "Delivery" AND bname = "Uniline Pelmet" then
                '     Return String.Format("{0} {1} - {2} POA", bname, pelmetOver, fabricGroup)
                ' End If

                If String.IsNullOrEmpty(fabricGroup) Then fabricGroup = "No Fabric"
                If bname = "Uniline Pelmet" Then
                    Return String.Format("{0} {1} - {2}", bname, pelmetOver, fabricGroup)
                End If

                If bname = "Fashade Pelmet" Then
                    Return String.Format("{0} - {1}", bname, mounting)
                End If

            Case Else
                Return ""
        End Select
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function UpdateStatusOrder(data As ParamUpdateStatusOrder) As Object
        Try
            Dim msg As String

            If String.IsNullOrEmpty(data.id) Then
                Throw New Exception("this order is missing !")
            End If

            If String.IsNullOrEmpty(data.status) Then
                Return New With { .warning = true, .message = "status is required !", .field = "#modalChangeStatus #status"}
            End If
            If data.status = data.statusOld Then
                Return New With { .warning = true, .message = "you don't choose different changes on status, don't do it with the same status!", .field = "#modalChangeStatus #status"}
            End If


            If data.status = "New Order" Then
                If data.submittedDate = "" Then
                    Return New With { .warning = true, .message = "submitted date is required !", .field = "#modalChangeStatus #submitteddate"}
                End If
            End If

            If data.status = "Completed" Then
                If data.completeddate = "" Then
                    Return New With { .warning = true, .message = "shipped date is required !", .field = "#modalChangeStatus #completeddate"}
                End If
            End If

            If data.status = "Canceled" Then
                If data.canceleddate = "" Then
                    Return New With { .warning = true, .message = "canceled date is required !", .field = "#modalChangeStatus #canceleddate"}
                End If
            End If
            
            If data.description = "" Then
                Return New With { .warning = true, .message = "description is required !", .field = "#modalChangeStatus #description"}
            End If          

            Dim findDesc As String = data.description
            Select Case data.status
                Case "Draft"
                    findDesc = "Status changed to draft by <b>" & data.username & "</b>"
                    findDesc += "<br />"
                    findDesc += "Notes from the office:"
                    findDesc += "<br />"
                    findDesc += data.description
                Case "New Order"
                    findDesc = "Status changed to new order by <b>" & data.username & "</b>"
                    findDesc += "<br />"
                    findDesc += "Notes from the office:"
                    findDesc += "<br />"
                    findDesc += data.description
                Case "In Production"
                    findDesc = "Your order is currently in the production process"
                    findDesc += "<br />"
                    findDesc += "Notes from the office:"
                    findDesc += "<br />"
                    findDesc += data.description
                Case "On Hold"
                    findDesc = "Your order on hold by <b>" & data.username & "</b>"
                    findDesc += "<br />"
                    findDesc += "Notes from the office:"
                    findDesc += "<br />"
                    findDesc += data.description
                Case "Completed"
                    findDesc += "Notes from the office:"
                    findDesc += "<br />"
                    findDesc += data.description
                Case "Canceled"
                    findDesc = "Your order has been canceled by <b>" & data.username & "</b>"
                    findDesc += "<br />"
                    findDesc += "Notes from the office:"
                    findDesc += "<br />"
                    findDesc += data.description
            End Select

            If Not String.IsNullOrEmpty(data.id) Then
                Dim query As String = "UPDATE OrderHeaders SET Status='Draft', StatusDescription=@StatusDescription, SubmittedDate=NULL, JobDate=NULL, CanceledDate=NULL, CompletedDate=NULL WHERE Id=@Id"
                Select Case data.status
                    Case "New Order"
                        query = "UPDATE OrderHeaders SET Status='New Order', StatusDescription=@StatusDescription, SubmittedDate=@SubmittedDate WHERE Id=@Id"
                    Case "In Production"
                        query = "UPDATE OrderHeaders SET Status='In Production', StatusDescription=@StatusDescription, JobDate=GETDATE() WHERE Id=@Id"
                    Case "On Hold"
                        query = "UPDATE OrderHeaders SET Status='On Hold', StatusDescription=@StatusDescription WHERE Id=@Id"
                    Case "Completed"
                        query = "UPDATE OrderHeaders SET Status='Completed', StatusDescription=@StatusDescription, CompletedDate=@CompletedDate WHERE Id=@Id"
                    Case "Canceled"
                        query = "UPDATE OrderHeaders SET Status='Canceled', StatusDescription=@StatusDescription, CanceledDate=@CanceledDate WHERE Id=@Id"
                End Select

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand(query, thisConn)
                        myCmd.Parameters.AddWithValue("@Id", data.id)
                        myCmd.Parameters.AddWithValue("@Status", data.status)
                        myCmd.Parameters.AddWithValue("@StatusDescription", findDesc)
                        myCmd.Parameters.AddWithValue("@SubmittedDate", data.submitteddate)
                        myCmd.Parameters.AddWithValue("@CompletedDate", data.completeddate)
                        myCmd.Parameters.AddWithValue("@CanceledDate", data.canceleddate)
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                Dim dataLog As Object() = {data.id, "", "Blinds", data.loginid, "Update Status Order"}
                orderCfg.Log_Orders(dataLog)

                msg = "Status has been updated successfully."
             End If

            Return New With {.success = true, .message = msg}
        Catch ex As Exception
            Return New With {.error = true, .message = ex.Message}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function DownloadBarcode(ByVal headerid As String, ByVal itemid As String) As Object
        Try
            Dim msg As String = "Barcode has been downloaded."
            Dim url As String = ""
            Dim rolename As String = HttpContext.Current.Session("rolename").ToString()

            

            Dim HeaderData As DataSet = publicCfg.GetListData(String.Format("SELECT * FROM view_headers WHERE Id='{0}'", headerid))
            If HeaderData.Tables(0).Rows.Count < 1 Then
                Throw New Exception("Order Header not found.")
            End If

            ' Dim status As String = HeaderData.Tables(0).Rows(0)("Status").ToString()
            ' If rolename <> "Administrator" AndAlso status <> "Draft" Then
            '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "Permission denied : not administrator."}}
            ' End If

            Dim OrderCust As String = HeaderData.Tables(0).Rows(0).Item("OrderCust").ToString()
            Dim OrderNo As String = HeaderData.Tables(0).Rows(0).Item("OrderNo").ToString()
            Dim StoreId As String = HeaderData.Tables(0).Rows(0).Item("StoreId").ToString()
            Dim StoreName As String = HeaderData.Tables(0).Rows(0).Item("StoreName").ToString()
            Dim Delivery As String = HeaderData.Tables(0).Rows(0).Item("Delivery").ToString()
            Dim FileName As String = ("-BARCODE-ORDER-" & OrderNo & "-" & StoreId & ".txt").Replace(" ", "")

            Dim dirPath As String = HttpContext.Current.Server.MapPath("~/File/Order/Barcode/")
            If Not Directory.Exists(dirPath) Then
                Directory.CreateDirectory(dirPath)
            End If

            Dim WhereId As String =""
            If Not String.IsNullOrEmpty(itemid) Then
                WhereId = String.Format(" AND Id='{0}'", itemid)
                FileName = (String.Format("-BARCODE-ORDER-{0}-{1}-{2}.txt", OrderNo, StoreId, itemid)).Replace(" ", "")
            End If

            Dim fullPath As String = Path.Combine(dirPath, FileName)
            Dim DetailData As DataSet = publicCfg.GetListData(String.Format("SELECT * FROM view_details WHERE HeaderId='{0}' And Active='1' AND DesignName NOT IN ('Surcharge', 'Additional') {1} ORDER BY Id ASC", headerid, WhereId))
            If DetailData.Tables(0).Rows.Count < 1 Then
                Return New With {.warning = true, .message = "Please add item first."}
            End If

            Dim sb As New StringBuilder()

            ' 1. Hitung total seluruh label yang akan dicetak berdasarkan akumulasi Qty
            Dim TotalCount As Integer = 0
            For Each row As DataRow In DetailData.Tables(0).Rows
                Dim qVal As Integer = 0
                Integer.TryParse(row("Qty").ToString(), qVal)
                TotalCount += Math.Max(1, qVal) ' Menjaga agar minimal dihitung 1 jika Qty tidak valid/0
            Next

            Dim PageOf As Integer = 0
            Dim Count As Integer = DetailData.Tables(0).Rows.Count

            For i As Integer = 0 To Count - 1
                ' Ambil dan konversi Qty ke Integer
                Dim itemQty As Integer = 0
                Integer.TryParse(DetailData.Tables(0).Rows(i).Item("Qty").ToString(), itemQty)
                If itemQty < 1 Then itemQty = 1 ' Minimal cetak 1x

                Dim Location As String = DetailData.Tables(0).Rows(i).Item("Location").ToString()
                Dim Width As String = DetailData.Tables(0).Rows(i).Item("Width").ToString()
                Dim Drop As String = DetailData.Tables(0).Rows(i).Item("Drop").ToString()
                Dim DesignName As String = DetailData.Tables(0).Rows(i).Item("DesignName").ToString()
                Dim FabricName As String = DetailData.Tables(0).Rows(i).Item("FabricName").ToString()
                Dim Product As String = String.Format("{0} X {1} {2}", Width, Drop, DesignName)

                ' 2. Loop sebanyak jumlah Qty item tersebut
                For q As Integer = 1 To itemQty
                    PageOf += 1 ' Increment nomor halaman aktif

                    sb.AppendLine("^XA")
                    sb.AppendLine("^FO17,50")
                    sb.AppendLine(String.Format("^FO35,10^A0N,45,45^CI13^FH^FD{0}^FS", StoreName))
                    sb.AppendLine(String.Format("^FO35,50^A0N,40,40^CI13^FH^FD{0}^FS", OrderCust))
                    sb.AppendLine(String.Format("^FO600,90^A0N,40,40^CI13^FH^FD{0}^FS", OrderNo))
                    sb.AppendLine(String.Format("^FO35,90^A0N,45,45^CI13^FH^FD{0}^FS", headerid))
                    sb.AppendLine(String.Format("^FO600,130^A0N,25,25^CI13^FH^FD{0}^FS", Location))
                    sb.AppendLine(String.Format("^FO35,140^A0N,25,25^CI13^FH^FD{0}^FS", FabricName))
                    sb.AppendLine(String.Format("^FO35,173^A0N,25,25^CI13^FH^FD{0}^FS", Product))
                    ' Menampilkan nomor urut halaman aktif (PageOf) dari total keseluruhan (TotalCount)
                    sb.AppendLine(String.Format("^FO610,155^A0N,30,30^CI13^FH^FD({0} OF {1})^FS", PageOf, TotalCount))
                    sb.AppendLine(String.Format("^FO630,49^A0N,45,45^CI13^FH^FD{0}^FS", Delivery))
                    sb.AppendLine("^PQ1,0,0,Y")
                    sb.AppendLine("^XZ")
                    sb.AppendLine()
                    
                    sb.AppendLine("^XA")
                    sb.AppendLine("^FO17,50")
                    sb.AppendLine(String.Format("^FO35,10^A0N,45,45^CI13^FH^FD{0}^FS", StoreName))
                    sb.AppendLine(String.Format("^FO35,50^A0N,40,40^CI13^FH^FD{0}^FS", OrderCust))
                    sb.AppendLine(String.Format("^FO600,90^A0N,40,40^CI13^FH^FD{0}^FS", OrderNo))
                    sb.AppendLine(String.Format("^FO35,90^A0N,45,45^CI13^FH^FD{0}^FS", headerid))
                    sb.AppendLine(String.Format("^FO600,130^A0N,25,25^CI13^FH^FD{0}^FS", Location))
                    sb.AppendLine(String.Format("^FO35,140^A0N,25,25^CI13^FH^FD{0}^FS", FabricName))
                    sb.AppendLine(String.Format("^FO35,173^A0N,25,25^CI13^FH^FD{0}^FS", Product))
                    ' Menampilkan nomor urut halaman aktif (PageOf) dari total keseluruhan (TotalCount)
                    sb.AppendLine(String.Format("^FO610,155^A0N,30,30^CI13^FH^FD({0} OF {1})^FS", PageOf, TotalCount))
                    sb.AppendLine(String.Format("^FO630,49^A0N,45,45^CI13^FH^FD{0}^FS", Delivery))
                    sb.AppendLine("^PQ1,0,0,Y")
                    sb.AppendLine("^XZ")
                    sb.AppendLine()
                Next
            Next

            File.WriteAllText(fullPath, sb.ToString(), Encoding.ASCII)
            
            url = String.Format("/Methods/Order/Handler/DowloadPDFOrder.ashx?file={0}&keyDownload=barcode", FileName)

            Return New With {.success = true, .message = msg, .url = url}
        Catch ex As Exception
            Return New With {.error = true, .message = ex.Message}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function SubmitOverrideDisc(data As ParamSubmitOverrideDisc) As Object
        Try
            Dim msg As String = "200"

            Dim HeaderData As DataSet = publicCfg.GetListData("SELECT * FROM view_order_headers WHERE OrderType='Blinds' AND Id='" & data.headerid & "'")
            If HeaderData.Tables(0).Rows.Count < 1 Then
               Throw New Exception("This order is missing !")
            End If

            If String.IsNullOrEmpty(data.discount) Then
                Return New With {.warning = true, .message = "discount is required !", .field = "#modalQuoteDisc #discount"}
            End If

            
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderHeaders SET QuoteDisc=@QuoteDisc WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", data.headerid)
                    myCmd.Parameters.AddWithValue("@QuoteDisc", data.discount)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using


            Dim OrderType As String = publicCfg.GetItemData(String.Format("SELECT OrderType FROM OrderHeaders WHERE Id='{0}'", data.headerid))
            Dim dataLog As Object() = {data.headerid, "", OrderType, data.loginid, "Override Customer Discount"}
            orderCfg.Log_Orders(dataLog)


            Return New With {.success = true, .message = "Discount has been applied successfully."}
        Catch ex As Exception
           Return New With {.error = true, .message = ex.Message}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function SubmitChangeProductionDate(data As ParamSubmitChangeProductionDate) As Object
        Try

            Dim HeaderData As DataSet = publicCfg.GetListData("SELECT * FROM view_order_headers WHERE OrderType='Blinds' AND Id='" & data.headerid & "'")
            If HeaderData.Tables(0).Rows.Count < 1 Then
                Throw New Exception("order is missing !")
            End If

            If String.IsNullOrEmpty(data.productiondate) Then
                Return New With {.warning = true, .message = "date is required !", .field = "#modalQuoteDisc #productiondate"}
            End If

            
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderHeaders SET JobDate=@JobDate WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", data.headerid)
                    myCmd.Parameters.AddWithValue("@JobDate", data.productiondate)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using


            Dim OrderType As String = publicCfg.GetItemData(String.Format("SELECT OrderType FROM OrderHeaders WHERE Id='{0}'", data.headerid))
            Dim dataLog As Object() = {data.headerid, "", OrderType, data.loginid, "Change Production Date"}
            orderCfg.Log_Orders(dataLog)


            Return New With {.success = true, .message = "Date has been applied successfully."}
        Catch ex As Exception
            Dim msg As String = ex.Message
            If Not data.rolename = "Administrator" Then msg = "Please contact our IT team at support@onlineorder.au"
            Return New With { .error = true, .message = msg}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function SubmitSendMailQuote(data As ParamSubmitSendMailQuote) As Object
        Try
            Dim msg As String = "Mail has been sent successfully."

            Dim HeaderData As DataSet = publicCfg.GetListData("SELECT * FROM view_order_headers WHERE OrderType='Blinds' AND Id='" & data.headerid & "'")
            If HeaderData.Tables(0).Rows.Count < 1 Then
                Throw New Exception("This order is missing !")
            End If

            If String.IsNullOrEmpty(data.id) Then
                Throw New Exception("id is missing !")
            End If

            If String.IsNullOrEmpty(data.from) Then
                Return New With {.warning = true, .message = "pleasze check mail from !", .field = "#modalSendMailQuote #from"}
            End If

            If String.IsNullOrEmpty(data.mailto) Then
                Return New With {.warning = true, .message = "please check mail to !", .field = "#modalSendMailQuote #mailto"}
            End If
            If Not String.IsNullOrEmpty(data.mailto) Then
                If Not Regex.IsMatch(data.mailto, "^[\w\.-]+@([\w-]+\.)+[\w-]{2,}$") Then
                    Return New With {.warning = true, .message = "please check mail to format !",.field = "#modalSendMailQuote #mailto"}
                End If
            End If

           
            If Not String.IsNullOrEmpty(data.cc) Then
                If Not Regex.IsMatch(data.cc, "^[\w\.-]+@([\w-]+\.)+[\w-]{2,}$") Then
                    Return New With {.warning = true, .message = "please check mail to format !", .field = "#modalSendMailQuote #cc"}
                End If
            End If

            Dim OrderName As String = HeaderData.Tables(0).Rows(0).Item("OrderName").ToString()
            Dim OrderNumber As String = HeaderData.Tables(0).Rows(0).Item("OrderNumber").ToString()
            Dim CustomerId As String = HeaderData.Tables(0).Rows(0).Item("CustomerId").ToString()
            Dim CustomerName As String = HeaderData.Tables(0).Rows(0).Item("CustomerName").ToString()
            Dim FileName As String = ("-QUOTE-ORDER-" & OrderNumber & "-" & CustomerId & ".pdf").Replace(" ", "")

            Dim dirPath As String = HttpContext.Current.Server.MapPath("~/File/Order/Quote/Origin/")
            If Not Directory.Exists(dirPath) Then
                Directory.CreateDirectory(dirPath)
            End If
            Dim fullPath As String = Path.Combine(dirPath, FileName)
            printCfg.CreatePDFQuote(data.headerid, data.username, dirPath, FileName, "Origin")

            Dim Res As String = MailOriginQuote(data.headerid, dirPath, data.id, data.mailto)
            If Not Res = "200" Then
                throw New Exception(Res)
            End If

            


            Return New With {.success = true, .message = msg}
        Catch ex As Exception
            Return New With {.error = True, .message = ex.Message}
        End Try
    End Function

    Private Shared Function MailOriginQuote(headerid As String, directory As String, mailingid As String, customermail As String) As String
        Try
            Dim OrderData As DataSet = publicCfg.GetListData(String.Format("SELECT * FROM view_order_headers WHERE Id = '{0}' AND OrderType = 'Blinds' ", headerid))
            If OrderData.Tables(0).Rows.Count = 0 Then Return "invalid orders"

            Dim OrderId As String = OrderData.Tables(0).Rows(0).Item("OrderId").ToString()
            Dim CustomerId As String = OrderData.Tables(0).Rows(0).Item("CustomerId").ToString()
            Dim OrderNumber As String = OrderData.Tables(0).Rows(0).Item("OrderNumber").ToString()
            Dim OrderName As String = OrderData.Tables(0).Rows(0).Item("OrderName").ToString()
            Dim Delivery As String = OrderData.Tables(0).Rows(0).Item("Delivery").ToString()

            Dim mailData As DataSet = publicCfg.GetListData(String.Format("SELECT * FROM Mailings WHERE Id = '{0}' AND Active = '1' ", mailingid))
            Dim mailDevelopment As DataSet = publicCfg.GetListData("SELECT * From MailConfiguration WHERE Id='FADBA62C-2072-4501-8901-5E071BBF5E67'")

            If mailData.Tables(0).Rows.Count = 0 Then Return "invalid mailings"
            Dim CustomerName As String = publicCfg.GetItemData(String.Format("SELECT Name FROM Customers WHERE Id = '{0}'", CustomerId))

            Dim mailServer As String = mailData.Tables(0).Rows(0)("Server").ToString()
            Dim mailHost As String = mailData.Tables(0).Rows(0)("Host").ToString()
            Dim mailPort As Integer = CInt(mailData.Tables(0).Rows(0)("Port"))
            Dim mailAccount As String = mailData.Tables(0).Rows(0)("Account").ToString()
            Dim mailPassword As String = mailData.Tables(0).Rows(0)("Password").ToString()
            Dim mailAlias As String = mailData.Tables(0).Rows(0)("Alias").ToString()
            Dim mailTo As String = mailData.Tables(0).Rows(0)("To").ToString()
            Dim mailCc As String = mailData.Tables(0).Rows(0)("Cc").ToString()
            Dim mailBcc As String = mailData.Tables(0).Rows(0)("Bcc").ToString()
            Dim mailNetworkCredentials As Boolean = CBool(mailData.Tables(0).Rows(0)("NetworkCredentials"))
            Dim mailDefaultCredentials As Boolean = CBool(mailData.Tables(0).Rows(0)("DefaultCredentials"))
            Dim mailEnableSSL As Boolean = CBool(mailData.Tables(0).Rows(0)("EnableSSL"))

            Dim mailBody As String = "<span style='font-family: Lucida Sans Unicode, sans-serif; font-size: 14px;'>Hi <b>" & CustomerName & ",</b></span>"
            mailBody &= "<br /><br />Please see the files we have attached.<br /><br />"
            mailBody &= "<span style='font-weight: bold;'>Kind Regards,<br /><br />Customer Service<br />Sunlight Products</span>"

             Using myMail As New MailMessage()

                Dim fileName As String = Trim("-QUOTE-ORDER-" & OrderNumber.Replace(" ", "") & "-" & CustomerId & ".pdf")
                myMail.Subject = "Quote Order - " & OrderId
                myMail.From = New MailAddress(mailServer, mailAlias)
                myMail.Body = mailBody
                myMail.IsBodyHtml = True

                If mailDevelopment.Tables.Count > 0 Then
                    Dim mDev As String = mailDevelopment.Tables(0).Rows(0).Item("To").ToString()
                    Dim activeDev As String = mailDevelopment.Tables(0).Rows(0).Item("Active").ToString()

                    If activeDev = "True" Or activeDev = "1" Then
                        myMail.To.Add(mdev)
                    Else
                        myMail.To.Add(customermail)
                        If Not String.IsNullOrEmpty(mailTo) Then myMail.To.Add(mailTo)
                        If Not String.IsNullOrEmpty(mailCc) Then myMail.CC.Add(mailCc)
                        If Not String.IsNullOrEmpty(mailBcc) Then
                            Dim BccList() As String = mailBcc.Split(";")
                            Dim ThisMail As String = ""
                            For Each ThisMail In BccList
                                myMail.Bcc.Add(ThisMail)
                            Next
                        End If
                    End If
                Else
                    myMail.To.Add(customermail)
                    If Not String.IsNullOrEmpty(mailTo) Then myMail.To.Add(mailTo)
                    If Not String.IsNullOrEmpty(mailCc) Then myMail.CC.Add(mailCc)
                    If Not String.IsNullOrEmpty(mailBcc) Then
                        Dim BccList() As String = mailBcc.Split(";")
                        Dim ThisMail As String = ""
                        For Each ThisMail In BccList
                            myMail.Bcc.Add(ThisMail)
                        Next
                    End If
                End If

                Dim fullPath = Path.Combine(directory, fileName)
                Using fs As New FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read)
                Dim attach As New Attachment(fs, fileName)
                myMail.Attachments.Add(attach)
                    Using smtpClient As New SmtpClient(mailHost, mailPort)
                        smtpClient.EnableSsl = mailEnableSSL
                        smtpClient.UseDefaultCredentials = mailDefaultCredentials
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network

                        If mailNetworkCredentials Then
                            smtpClient.Credentials = New NetworkCredential(mailAccount, mailPassword)
                        ElseIf mailDefaultCredentials Then
                            smtpClient.UseDefaultCredentials = True
                        Else
                            smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials
                        End If

                        smtpClient.Send(myMail)
                    End Using
                End Using
            End Using


            Return "200"
        Catch ex As Exception
            Dim errorMessage As String = "Failure sending mail. " & ex.Message
            If ex.InnerException IsNot Nothing Then
                errorMessage &= " Inner Exception: " & ex.InnerException.Message
            End If
            Return errorMessage
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindProduction(ByVal designname As String, ByVal rolename As String) As Object
        Try
            If String.IsNullOrEmpty(designname) Then
                Throw New Exception("designname is null or empty.")
            End If

            Dim HasGlobal As List(Of String) = New List(Of String) From {
                "Roller Blinds", 
                "Panel Glides", 
                "Roman Blinds", 
                "Vertical Blinds"
            }

            Dim OptProd As List(Of String) = New List(Of String) From {
                "Sunlight"
            }

            Dim VisProd As Boolean = False

            If HasGlobal.Contains(designname) Then
                Dim Env As String = ""

                If rolename = "Customer" Then
                    Env = "AND Description = 'Environment : Production'"
                End If

                If InArray(rolename, "PPIC & DE", "Manager", "Customer Service") Then
                    Env = "AND Description IN ('Environment : Production', 'Environment : Testing')"
                End If

                Dim GlobalProduct As String = publicCfg.GetItemData(String.Format("SELECT Id FROM Designs WHERE Name = 'Global {0}' {1} AND Active = 1", designname, Env))

                If Not String.IsNullOrEmpty(GlobalProduct) Then
                    OptProd.Add("Global")
                    VisProd = True
                End If
            End If

            Return New With {
                .error = false,
                .OptProd = OptProd,
                .VisProd = VisProd
            }
        Catch ex As Exception
            Return New With {.error = True, .message = String.Format("BindProduction: {0}", ex.Message)}
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function FindProductForm(data as ParamFindProductForm) As Object
        Try
            If String.isNullOrEmpty(data.headerid) Then
                Throw New Exception("headerid is null or empty !")
            End If
            If String.isNullOrEmpty(data.ordertype) Then
                Throw New Exception("ordertype is null or empty !")
            End If
            If String.isNullOrEmpty(data.action) Then
                Throw New Exception("action is null or empty !")
            End If
            If String.isNullOrEmpty(data.designid) Then
                Return New With { .warning = true, .message = "product is required !", .field = "#modalAddItem #designid"}
            End If

            Dim DesignName As String = publicCfg.GetDesignName(data.designid)
            Dim page As String = publicCfg.GetDesignPage(data.designid)
            If InArray(DesignName, "Roller Blinds", "Panel Glides", "Roman Blinds", "Vertical Blinds") Then

                Dim Env As String =""
                If data.rolename = "Customer" Then
                    Env = "AND Description = 'Environment : Production'"
                End If
                if InArray(data.rolename, "PPIC & DE", "Manager", "Customer Service") Then
                    Env = "AND Description IN ('Environment : Production', 'Environment : Testing')"
                End If

                Dim GlobalDesigns As String = publicCfg.GetItemData(String.Format("SELECT Id FROM Designs WHERE Name = 'Global {0}' {1} AND Active = 1", DesignName, Env))
                IF Not String.IsNullOrEmpty(GlobalDesigns) Then
                    If String.isNullOrEmpty(data.production) Then
                        Return New With { .warning = true, .message = "production is required !", .field = "#modalAddItem #production"}
                    End If
                    If Not String.isNullOrEmpty(data.production) AND data.production = "Global" Then
                        Dim Name As String = String.Format("{0} {1}", data.production, DesignName)
                        page = publicCfg.GetItemData(String.Format("SELECT Page FROM Designs WHERE Name = '{0}'", Name))
                        data.designid = publicCfg.GetItemData(String.Format("SELECT Id FROM Designs WHERE Name = '{0}'", Name))
                    End If
                End IF

            End If
            
            ' Throw New Exception("page: " & page)

            HttpContext.Current.Session("headerId") = data.headerid 
            HttpContext.Current.Session("itemAction") = data.action
            HttpContext.Current.Session("orderType") = data.ordertype
            HttpContext.Current.Session("designId") = UCase(data.designid).ToString()

            If Not String.IsNullOrEmpty(data.id) And (data.action ="EditItem" Or data.action = "ViewItem" Or data.action = "NextItem") Then
                HttpContext.Current.Session("itemId") = data.id
            End If


            Return New With {.success = true, .page = page}
        Catch ex As Exception
            Return New With { .error = true, .message = String.Format("FindProductForm: {0}", ex.Message)}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function CopyItem(ByVal id As String, ByVal headerid As String, ByVal loginid As String) As Object
        Try
            If String.IsNullOrEmpty(id) Then
                Throw new Exception("id is null or empty !")
            End If

            Dim detailData As DataSet = publicCfg.GetListData(String.Format("SELECT * FROM view_details WHERE Id='{0}' AND Active='1'",id))
            If detailData.Tables(0).Rows.Count < 1 Then
                Throw New Exception("Order not found.")
            End If

            Dim DesignId As String = detailData.Tables(0).Rows(0).Item("DesignId").ToString()
            Dim DesignName As String = detailData.Tables(0).Rows(0).Item("DesignName").ToString()
            Dim BracketType As String = detailData.Tables(0).Rows(0).Item("BracketType").ToString()

            Dim NewItemId As string = publicCfg.CreateOrderItemId()
            Dim NewBlindNo As String = "Blind 1"
            Dim NewUniqueId As String = String.Empty
            If InArray(BracketType, "Double", "Linked 2 Blinds (Ind)", "Linked 2 Blinds (Dep)", "Linked 3 Blinds (Ind)", "Linked 3 Blinds (Dep)", "Double and Link System Ind", "Double and Link System Dep") Then
                NewUniqueId = GenerateUniqueId()
            End IF


            Dim OngoingFieldWindow As String = "MeshType, FrameColour, Brace, AngleType, AngleLength, AngleQty, PortHole, PlungerPin, SwipelColour, SwipelQty, SwipelQtyB, SpringQty, TopPLasticQty,"

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand(String.Format("INSERT INTO OrderDetails SELECT @IdNew, HeaderId, KitId, SoeKitId, ExactId, FabricId, FabricIdB, ChainId, BottomRailId, PriceGroupId, PriceGroupIdB, CassetteExtraId, @UniqueId, BlindNo, Qty, Location, Mounting, Width, WidthB, WidthMiddle, WidthBottom, [Drop], DropB, DropMiddle, DropRight, SemiInsideMount, LouvreSize, LouvrePosition, HingeColour, MidrailHeight1, MidrailHeight2, MidrailCritical, Layout, LayoutSpecial, CustomHeaderLength, FrameType, FrameLeft, FrameRight, FrameTop, FrameBottom, BottomTrackType, BottomTrackRecess, Buildout, BuildoutPosition, PanelQty, TrackQty, PanelSize, NumOfPanel, HingeQtyPerPanel, PanelQtyWithHinge, LocationTPost1, LocationTPost2, LocationTPost3, LocationTPost4, LocationTPost5, HorizontalTPost, HorizontalTPostHeight, JoinedPanels, ReverseHinged, PelmetFlat, ExtraFascia, HingesLoose, TiltrodType, TiltrodSplit, SplitHeight1, SplitHeight2, DoorCutOut, SpecialShape, TemplateProvided, {0} SquareMetre, LinearMetre, StackPosition, TilterPosition, RollDirection, ControlPosition, ControlColour, @ControlLength, ChainLength, MaterialChain, MotorStyle, MotorRemote, MotorRequired, MotorBattery, MotorCharger, Connector, AdditionalMotor, CableExitPoint, TrackType, TrackColour, TrackLength, NumOfWand, WandPosition,  WandColour, WandLength, CordColour, CordLength, MaterialCord, AcornPlasticColour, Accessory, SideBySide, SlatSize, SlatQty, TubeSize, Trim, Batten, BattenColour,  BracketOption, BracketColour, BracketCover, BracketExtension, Fitting, FlatType, ChildSafe, Cleat, BottomHoldDown, HangerType, PelmetType, @PelmetWidth, PelmetSize, PelmetReturn, PelmetReturnPosition, PelmetReturnSize, PelmetReturnSize2, CutOut_LeftTop, CutOut_RightTop, CutOut_LeftBottom, CutOut_RightBottom, LHSWidth_Top, LHSHeight_Top, RHSWidth_Top, RHSHeight_Top, LHSWidth_Bottom, LHSHeight_Bottom, RHSWidth_Bottom, RHSHeight_Bottom, BlindSize, Sloper, InsertInTrack, Notes, Matrix, Charge, Discount, TotalMatrix, TotalCharge, TotalDiscount, MarkUp, Active FROM OrderDetails WHERE Id=@Id", OngoingFieldWindow), thisConn)
                    myCmd.Parameters.AddWithValue("@Id", id)
                    myCmd.Parameters.AddWithValue("@IdNew", NewItemId)
                    myCmd.Parameters.AddWithValue("@UniqueId", NewUniqueId)
                    myCmd.Parameters.AddWithValue("@PelmetWidth", DBNull.Value)
                    myCmd.Parameters.AddWithValue("@ControlLength", DBNull.Value)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            If Not DesignName = "Roller Blinds" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO OrderDetailsPrice SELECT	NEWID(), HeaderId, @ItemIdNew, Type, Qty, Description, Cost, Discount, DiscountB, DiscountC, Poa FROM OrderDetailsPrice WHERE ItemId=@ItemId", thisConn)
                        myCmd.Parameters.AddWithValue("@ItemId", id)
                        myCmd.Parameters.AddWithValue("@ItemIdNew", NewItemId)
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using
            End If


            publicCfg.ResetPriceDetail(NewItemId)
            publicCfg.HitungHarga(headerid, NewItemId)
            publicCfg.HitungSurcharge(headerid, NewItemId)
            Dim OrderType As String = publicCfg.GetItemData(String.Format("SELECT OrderType FROM OrderHeaders WHERE Id='{0}'", headerid))
            Dim dataLog As Object() = {headerid, NewItemId, OrderType, loginid, "Copy Item Order"}
            orderCfg.Log_Orders(dataLog)

            Return New With {.success = true, .message = "Data has been copied successfully, Click <b>OK</b> to reload item list."}
        Catch ex As Exception
            Return New With {.error = true, .message = String.Format("CopyItem: {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function GenerateUniqueId() As String
        Return Guid.NewGuid().ToString("N")
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function DeleteItem(ByVal id As String) As Object
        Try
            If String.IsNullOrEmpty(id) Then
                Throw New Exception("id is null or empty !")
            End If

            Dim detailData As DataSet = publicCfg.GetListData(String.Format("SELECT * FROM view_details WHERE Id='{0}' AND Active='1'", id))
            If detailData.Tables(0).Rows.Count = 0 Then
                Throw New Exception("Order not found.")
            End If

            Dim BracketName As String = detailData.Tables(0).Rows(0).Item("BracketType").ToString()
            Dim BlindNo As String = detailData.Tables(0).Rows(0).Item("BlindNo").ToString()
            Dim UniqueId As String = detailData.Tables(0).Rows(0).Item("UniqueId").ToString()

            UpdateDetail(id)
            DeleteDetail(id)

            Dim NewBlindNo As String = String.Empty
            If BracketName = "Double" Or BracketName = "Linked 2 Blinds (Dep)" Or BracketName = "Linked 2 Blinds (Ind)" Then
                If BlindNo = "Blind 1" Then
                    BlindNo = "Blind 2"
                    NewBlindNo = "Blind 1"
                    UpdateDetailBlindNo(NewBlindNo, BlindNo, UniqueId)
                End If
            End If

            If BracketName = "Linked 3 Blinds (Dep)" Or BracketName = "Linked 3 Blinds (Ind)" Then
                If BlindNo = "Blind 1" Then
                    BlindNo = "Blind 2"
                    NewBlindNo = "Blind 1"

                    UpdateDetailBlindNo(NewBlindNo, BlindNo, UniqueId)

                    BlindNo = "Blind 3"
                    NewBlindNo = "Blind 2"
                    UpdateDetailBlindNo(NewBlindNo, BlindNo, UniqueId)
                End If

                If BlindNo = "Blind 2" Then
                    BlindNo = "Blind 3"
                    NewBlindNo = "Blind 2"
                    UpdateDetailBlindNo(NewBlindNo, BlindNo, UniqueId)
                End If
            End If

            Return New With { .success = true, .message = "Data has been deleted successfully, Click <b>OK</b> to reload item list."}
        Catch ex As Exception
            Return New With { .error = true, .message = ex.Message}
        End Try
    End Function

    Private Shared Sub UpdateDetail(ByVal id As String)
        Using thisConn As New SqlConnection(myConn)
            Using myCmd As New SqlCommand("UPDATE OrderDetails SET BlindNo=NULL WHERE Id=@Id", thisConn)
                myCmd.Parameters.AddWithValue("@Id", id)
                myCmd.Connection = thisConn
                thisConn.Open()
                myCmd.ExecuteNonQuery()
                thisConn.Close()
            End Using
        End Using
    End Sub

     Private Shared Sub DeleteDetail(ByVal id As String)
        Using thisConn As New SqlConnection(myConn)
            Using myCmd As New SqlCommand("UPDATE OrderDetails SET Active=0 WHERE Id=@Id", thisConn)
                myCmd.Parameters.AddWithValue("@Id", id)
                myCmd.Connection = thisConn
                thisConn.Open()
                myCmd.ExecuteNonQuery()
                thisConn.Close()
            End Using
        End Using
    End Sub

    Private Shared Sub UpdateDetailBlindNo(ByVal newblindno As String, ByVal blindno As String, ByVal uniqueid As String)
        Using thisConn As New SqlConnection(myConn)
            Using myCmd As New SqlCommand("UPDATE OrderDetails SET BlindNo=@BlindNoNew WHERE UniqueId=@UniqueId AND BlindNo=@BlindNo AND Active=1", thisConn)
                myCmd.Parameters.AddWithValue("@BlindNoNew", newblindno)
                myCmd.Parameters.AddWithValue("@BlindNo", blindno)
                myCmd.Parameters.AddWithValue("@UniqueId", uniqueid)
                myCmd.Connection = thisConn
                thisConn.Open()
                myCmd.ExecuteNonQuery()
                thisConn.Close()
            End Using
        End Using
    End Sub

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindOrderDetailPrice(ByVal itemid As String) As Object
        Try
            Dim dt As DataTable = publicCfg.GetListData(String.Format("SELECT Id, Qty, Type, Description, Cost, Poa FROM OrderDetailsPrice WHERE ItemId = '{0}' AND ((Type = 'Charge' AND Description NOT LIKE '%Powder Coating%' AND Description NOT LIKE '%Tracking & Interloock%') OR (Type = 'Matrix')) ORDER BY CASE WHEN Type = 'Matrix' THEN 1 WHEN Type = 'Charge' THEN 2 WHEN Type = 'Discount' THEN 3 ELSE 4 END", itemid)).Tables(0)

            Dim list As New List(Of Object)

            For Each row As DataRow In dt.Rows
                list.Add(New With {
                    .Id = row("Id").ToString(),
                    .Qty = row("Qty").ToString(),
                    .Type = row("Type").ToString(),
                    .Description = row("Description").ToString(),
                    .Cost = CDec(row("Cost")).ToString("C", New CultureInfo("en-US")),
                    .Poa = row("Poa").ToString().Replace(",", ".")
                })
            Next

            Return New With {.success = True, .odp = list}

        Catch ex As Exception
            Return New With {.success = False, .message = ex.Message}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function OverwritePricing(data As ParamOverwritePricing) As Object
        Try
            If data Is Nothing OrElse data.details Is Nothing Then
               Throw New Exception("Data is null or empty !")
            End If

            Dim updatedCount As Integer = 0
            Dim OrderType As String = publicCfg.GetItemData(String.Format("SELECT OrderType FROM OrderHeaders where Id={0}", data.headerid))
            Dim DetailData As Dataset = publicCfg.GetListData(String.Format("SELECT DesignId, BlindId FROM view_details WHERE Id='{0}'", data.itemid))
            Dim DesignId As String = DetailData.Tables(0).Rows(0)("DesignId").ToString()
            Dim BlindId As String = DetailData.Tables(0).Rows(0)("BlindId").ToString()

            For Each item In data.details

                If item.poa = 0 Then
                    Continue For
                End If

                Dim guidId As Guid
                If Not Guid.TryParse(item.id, guidId) Then
                    Continue For
                End If


                Dim ListParamDiscount As New List(Of Object) From {
                    data.headerid,
                    data.customerid,
                    "",
                    item.poa,
                    DesignId,
                    BlindId
                }
                Dim Discount As Decimal = publicCfg.HitungDiscount(ListParamDiscount)
                Dim DiscountB As Decimal = publicCfg.HitungCustomDiscount(data.headerid, data.itemid, (item.poa - Discount), item.type)

                If item.type = "Charge" Then
                    Discount = DiscountB
                End If

                ' Throw New Exception(DiscountB.ToString())

                Dim Res As String = UpdateOverwritePricing(item.id, item.poa, Discount, DiscountB)
                IF Not Res = "200" Then
                    Throw New Exception(Res)
                End If
            Next

            
            Dim Matrix As String = publicCfg.GetItemData(String.Format("SELECT SUM(( odp.Cost * odp.Qty ) - ( odp.Qty * ISNULL( odp.Discount, 0 ) ) - ( odp.Qty * ISNULL( odp.DiscountB, 0 ) ) - ( odp.Qty * ISNULL( odp.DiscountC, 0 ) )) As Matrix FROM OrderDetailsPrice odp INNER JOIN OrderDetails od ON odp.ItemId=od.id WHERE odp.HeaderId='{0}' AND odp.ItemId='{1}' AND odp.Type='Matrix' AND od.Active='1'", data.headerid, data.itemid))
            
            Dim Charge As String = publicCfg.GetItemData(String.Format("SELECT SUM(( odp.Cost * odp.Qty ) - ( odp.Qty * ISNULL( odp.Discount, 0 ) ) - ( odp.Qty * ISNULL( odp.DiscountB, 0 ) ) - ( odp.Qty * ISNULL( odp.DiscountC, 0 ) )) As Charge FROM OrderDetailsPrice odp INNER JOIN OrderDetails od ON odp.ItemId=od.Id WHERE odp.HeaderId='{0}' AND odp.ItemId='{1}' AND odp.Type='Charge' AND odp.Description NOT LIKE '%Powder Coating%' AND odp.Description <> 'Tracking & Interloock' AND od.Active='1'", data.headerid, data.itemid))

            publicCfg.UpdateMatrix(data.itemid, data.qty, If(Matrix = "", 0D, CDec(Matrix)))
            publicCfg.UpdateCharge(data.itemid, data.qty, If(Charge = "", 0D, CDec(Charge)))

            Dim dataLog As Object() = {data.headerid, data.itemid, OrderType, data.loginid, "Override Pricing"}
            orderCfg.Log_Orders(dataLog)
            
            Return New With { .success = true, .message = "Pricing has been updated successfully."}
        Catch ex As Exception
            Return New With {.error = true, .message = String.Format("OverwritePricing : {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function UpdateOverwritePricing(id As String, newcost As Decimal, disc As Decimal, discB As Decimal) As String
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderDetailsPrice SET Cost=@Cost, Discount=@Disc, DiscountB=@DiscB, Poa=@Poa WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", id)
                    myCmd.Parameters.AddWithValue("@Cost", newcost)
                    myCmd.Parameters.AddWithValue("@Disc", disc)
                    myCmd.Parameters.AddWithValue("@DiscB", discB)
                    myCmd.Parameters.AddWithValue("@Poa", newcost)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Return "200"
        Catch ex As Exception
            Return   "UpdateOverwritePricing : " & ex.Message
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindPricingItem(ByVal id As String, ByVal rolename As String) As Object
        Try
            Dim PricingData As New List(Of Object)()

            Dim Query As String = "SELECT *, FORMAT(Cost, 'C', 'en-US') AS FormatCost, FORMAT(Discount, 'C', 'en-US') AS FormatDiscount, FORMAT(DiscountB, 'C', 'en-US') AS FormatDiscountB, FORMAT ( Poa, 'C', 'en-US' ) AS FormatPoa FROM OrderDetailsPrice WHERE ItemId = @ItemId ORDER BY CASE WHEN Type = 'Matrix' THEN 1 WHEN Type = 'Charge' THEN 2 WHEN Type = 'Discount' THEN 3 ELSE 4 END"
            Using conn As New SqlConnection(myConn)
                Using cmd As New SqlCommand(Query, conn)
                    cmd.Parameters.AddWithValue("@ItemId", id)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()

                            Dim Type As String = reader("Type").ToString()
                            Dim Qty As Integer = reader("Qty").ToString()
                            Dim Description As String = reader("Description").ToString()
                            Dim Cost As Decimal = CDec(reader("Cost"))
                            Dim Discount As Decimal = CDec(reader("Discount"))
                            Dim DiscountB As Decimal = CDec(reader("DiscountB"))
                            Dim DiscounCt As Decimal = CDec(reader("DiscountC"))
                            Dim Poa As Decimal = CDec(reader("Poa"))

                            '#Initialize
                            Dim isPOA As Integer = InStr(Description, "POA")
                            Dim markPOA As String = "<span class='badge bg-orange-lt'>POA</span>"
                            Dim isTrackInterlock As Integer = InStr(Description, "Tracking & Interlock")
                            Dim isCoating As Integer = InStr(Description, "Powder Coating")
                            Dim isCharge As Boolean = (Type = "Charge")
                            Dim isRoles As Boolean = InArray(rolename, "Administrator", "PPIC & DE", "Customer Service")
                            Dim isContinue As Boolean = false
                            Dim isOpacity As String = ""

                            '#Find Cost
                            Dim ThisCost As String = Cost.ToString("C", New CultureInfo("en-US"))
                            If isPOA > 0 Then ThisCost = markPOA
                            

                            '#Find POA
                            Dim ThisPOA As String = ""
                            If Poa > 0 Then ThisPOA = Poa.ToString("C", New CultureInfo("en-US"))

                            '#Find Final Cost
                            Dim FinalCost As Decimal = 0.00
                            Dim DiscountInPercent As Decimal = 0.00
                            IF Cost > 0 Then 
                                FinalCost = (Cost - Discount) * Qty
                                DiscountInPercent = (Discount / Cost) * 100
                            End If

                            Dim FinalCostB As Decimal = 0
                            Dim DiscountInPercentB As Decimal = 0
                            If DiscountB > 0 Then
                                FinalCostB = (FinalCost - DiscountB) * Qty
                                DiscountInPercentB = (DiscountB / FinalCost) * 100
                            End If

                            Dim ThisFinalCost As String = FinalCost.ToString("C", New CultureInfo("en-US"))
                            If FinalCostB > 0 Then
                                ThisFinalCost += String.Format("<br/> {0}", FinalCostB.ToString("C", New CultureInfo("en-US")))
                            End If

                            '#Find Discount
                            Dim ThisDisc As String = If(Discount > 0, Discount.ToString("C", New CultureInfo("en-US")), "")
                            Dim ThisDiscB As String = ""
                            Dim ElDisc As String = String.Format("<button type='button' class='border-0 bg-transparent' data-bs-container='body' data-bs-toggle='popover' data-bs-trigger='hover focus' data-bs-placement='bottom' data-bs-content='Discount in {0}%'>{1}</button>", DiscountInPercent.ToString("0.##"), ThisDisc)
                            IF DiscountB > 0 Then
                                ThisCost += String.Format("<br/> {0}", FinalCost.ToString("C", New CultureInfo("en-US")))
                                If Type = "Matrix" Then
                                    ThisDiscB = DiscountB.ToString("C", New CultureInfo("en-US"))
                                    ElDisc += String.Format("<button type='button' class='border-0 bg-transparent' data-bs-container='body' data-bs-toggle='popover' data-bs-trigger='hover focus' data-bs-placement='bottom' data-bs-content='Discount in {0}%'>{1}</button>", DiscountInPercentB.ToString("0.##"), ThisDiscB)
                                End If
                            End If


                            If isCharge AND (isTrackInterlock > 0 Or isCoating > 0) Then 
                                ThisCost = String.Format("<span class='text-decoration-line-through'>{0}</span>", ThisCost)
                                ThisFinalCost = String.Format("<span class='text-decoration-line-through'>{0}</span>", ThisFinalCost)
                                If Not isRoles Then isContinue = true
                                isOpacity = "opacity-50"
                            End If
                            

                            PricingData.Add(New With {
                                .isContinue = isContinue,
                                .isOpacity = isOpacity,
                                .Id = reader("Id").ToString(),
                                .HeaderId = reader("HeaderId").ToString(),
                                .ItemId = reader("ItemId").ToString(),
                                .Qty = reader("Qty").ToString(),
                                .Description = Description,
                                .Type = Type,
                                .Cost = ThisCost,
                                .Poa = ThisPOA,
                                .Discount = ElDisc,
                                .FinalCost = ThisFinalCost
                            })
                        End While
                    End Using
                End Using
            End Using


            Return New With {
                .price = PricingData
            }

        Catch ex As Exception
            Return New With {.error = true, .message = String.Format("BindPricingItem : {0}", ex.Message)}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function ExactSlip(ByVal headerid As String, ByVal ordertype As String) As Object
        Try
            Dim HeaderData As DataSet = publicCfg.GetListData(String.Format("SELECT * FROM view_order_headers WHERE Id='{0}' AND OrderType='{1}'", headerid, ordertype))
            If HeaderData.Tables(0).Rows.Count < 1 Then
                Throw New Exception("Order Header not found.")
            End If
            
            Dim OrderId As String = HeaderData.Tables(0).Rows(0).Item("OrderId").ToString()
            Dim Status As String = HeaderData.Tables(0).Rows(0).Item("Status").ToString()
            Dim FileName As String = String.Format("Order-Blinds-{0}.xml", OrderId)
            Dim FilePath As String = HttpContext.Current.Server.MapPath("~/file/inv/")
            Dim PathCombine As String = Path.Combine(FilePath, FileName)

            If Not Status = "In Production" Then
                Return New With {.warning = true, .message = "This order is not in production."}
            End If

            ' Dim Res As String = CreateXMLB(headerid, FileName, FilePath)
            ' If Not Res = "200" Then Throw New Exception(Res)
            exactCfg.CreateXMLB(headerid, FileName, FilePath)
            exactCfg.Connect(PathCombine)
            
            Return New With {.success = true, .message = "The Exact Slip was successfully sent."}
        Catch ex As Exception
            Return New With {.error = true, .message = String.Format("ExactSlip : {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function CreateXMLB(Id As String, fileName As String, folderPath As String) As String
        Try
            Dim sb As New StringBuilder()
            sb.AppendLine("<?xml version=""1.0""?>")

            Dim settings As New XmlWriterSettings()
            settings.Indent = True
            settings.OmitXmlDeclaration = True
            settings.Encoding = New UTF8Encoding(False)

            Dim headerData As DataSet = publicCfg.GetListData("SELECT * FROM view_order_headers WHERE Id = '" + Id + "' AND OrderType = 'Blinds'")
            If headerData.Tables(0).Rows.Count < 0 Then Throw New Exception("Order Header not found.")

            For h As Integer = 0 To headerData.Tables(0).Rows.Count - 1
                Dim orderId As String = headerData.Tables(0).Rows(h).Item("OrderId").ToString()
                Dim orderNumber As String = headerData.Tables(0).Rows(h).Item("OrderNumber").ToString()
                Dim orderName As String = headerData.Tables(0).Rows(h).Item("OrderName").ToString()
                Dim customerId As String = headerData.Tables(0).Rows(h).Item("CustomerId").ToString()
                Dim jobDate As String = Convert.ToDateTime(headerData.Tables(0).Rows(h).Item("JobDate")).ToString("yyyy-MM-dd")
                Dim shipmentId As String = headerData.Tables(0).Rows(h).Item("ShipmentId").ToString()

                Dim customerAccount As String = publicCfg.GetItemData("SELECT Account FROM Customers WHERE Id = '" + customerId + "'")

                Dim exactCustomer As String = String.Empty
                If customerAccount = "Master" Or customerAccount = "Regular" Or customerAccount = "REGULAR" Then
                    exactCustomer = publicCfg.GetItemData("SELECT ExactId FROM Customers WHERE Id='" + customerId + "'")
                End If

                If customerAccount = "Sub" Then
                    Dim masterId As String = publicCfg.GetItemData("SELECT MasterId FROM Customers WHERE Id='" + customerId + "'")
                    exactCustomer = publicCfg.GetItemData("SELECT ExactId FROM Customers WHERE Id='" + masterId + "'")
                End If

                Dim etaCustomer As String = String.Empty
                If Not String.IsNullOrEmpty(shipmentId) Then
                    Dim shipmentData As DataSet = publicCfg.GetListData("SELECT * FROM OrderShipments WHERE Id='" + shipmentId + "'")
                    etaCustomer = Convert.ToDateTime(shipmentData.Tables(0).Rows(0).Item("ETACustomer")).ToString("yyyy-MM-dd")
                End If

                Using stringWriter As New StringWriter(sb)
                    Using writer As XmlWriter = XmlWriter.Create(stringWriter, settings)
                        writer.WriteStartDocument()
                        writer.WriteStartElement("eExact")
                        writer.WriteAttributeString("xmlns", "xsi", Nothing, "http://www.w3.org/2001/XMLSchema-instance")
                        writer.WriteAttributeString("xsi", "noNamespaceSchemaLocation", "http://www.w3.org/2001/XMLSchema-instance", "eExact-Schema.xsd")

                        writer.WriteStartElement("Orders")
                        writer.WriteStartElement("Order")
                        writer.WriteAttributeString("type", "V")

                        writer.WriteElementString("YourRef", Id)
                        writer.WriteElementString("Description", orderId & " - " & orderNumber & " - " & orderName)

                        writer.WriteStartElement("Resource")
                        writer.WriteAttributeString("number", "99")
                        writer.WriteString("")
                        writer.WriteEndElement()

                        writer.WriteStartElement("OrderedBy")
                        writer.WriteStartElement("Debtor")
                        writer.WriteAttributeString("code", exactCustomer)
                        writer.WriteString("")
                        writer.WriteEndElement()
                        writer.WriteElementString("Date", jobDate)
                        writer.WriteEndElement()

                        Dim detailData As DataSet = publicCfg.GetListData("SELECT * FROM OrderDetails WHERE HeaderId = '" & Id & "' AND Active = 1 ORDER BY Id ASC")

                        For i As Integer = 0 To detailData.Tables(0).Rows.Count - 1
                            Dim itemId As String = detailData.Tables(0).Rows(i).Item("Id").ToString()
                            Dim kitId As String = detailData.Tables(0).Rows(i).Item("KitId").ToString()
                            Dim totalMatrix As Decimal = detailData.Tables(0).Rows(i).Item("TotalMatrix").ToString()
                            Dim totalCharge As Decimal = detailData.Tables(0).Rows(i).Item("TotalCharge").ToString()
                            Dim finalCost As Decimal = totalMatrix + totalCharge
                            Dim finalCostString As String = finalCost.ToString(CultureInfo.InvariantCulture)

                            Dim exactProduct As String = detailData.Tables(0).Rows(i).Item("ExactId").ToString()
                            If String.IsNullOrEmpty(exactProduct) Then
                                Dim designId As String = publicCfg.GetItemData("SELECT DesignId FROM HardwareKits WHERE Id='" + UCase(kitId) + "'")
                                Dim blindId As String = publicCfg.GetItemData("SELECT BlindId FROM HardwareKits WHERE Id='" + UCase(kitId) + "'")
                                Dim designName As String = publicCfg.GetItemData("SELECT Name FROM Designs WHERE Id = '" + designId + "'")
                                Dim blindName As String = publicCfg.GetItemData("SELECT Name FROM Blinds WHERE Id = '" + blindId + "'")

                                Dim exactName As String = designName & " - " & blindName
                                exactProduct = publicCfg.GetItemData("SELECT ExactId FROM Exacts WHERE Name = '" + exactName + "'")
                            End If

                            Dim productName As String = publicCfg.GetItemData("SELECT Name FROM HardwareKits WHERE Id = '" & UCase(kitId) & "'")
                            Dim width As String = detailData.Tables(0).Rows(i).Item("Width").ToString()
                            Dim drop As String = detailData.Tables(0).Rows(i).Item("Drop").ToString()
                            Dim itemDescription As String = productName & " " & width & "x" & drop

                            writer.WriteStartElement("OrderLine")

                            writer.WriteStartElement("Item")
                            writer.WriteAttributeString("code", exactProduct)
                            writer.WriteString("")
                            writer.WriteEndElement()

                            writer.WriteElementString("Quantity", "1")

                            writer.WriteStartElement("Price")
                            writer.WriteAttributeString("type", "S")

                            writer.WriteStartElement("Currency")
                            writer.WriteAttributeString("code", "AUD")
                            writer.WriteString("")
                            writer.WriteEndElement()

                            writer.WriteElementString("Value", finalCostString)
                            writer.WriteEndElement() ' Price

                            writer.WriteStartElement("Delivery")
                            writer.WriteElementString("Date", etaCustomer)
                            writer.WriteEndElement()

                            writer.WriteElementString("Text", itemDescription)
                            writer.WriteEndElement() ' OrderLine
                        Next

                        writer.WriteEndElement() ' Order
                        writer.WriteEndElement() ' Orders
                        writer.WriteEndElement() ' eExact
                        writer.WriteEndDocument()
                    End Using
                End Using
            Next

            Dim filePath As String = Path.Combine(folderPath, fileName)
            Dim xmlFinal As String = sb.ToString().Replace(" />", "/>")
            File.WriteAllText(filePath, xmlFinal, New UTF8Encoding(False))
            Return "200"
        Catch ex As Exception
            Return String.Format("CreateXMLB: {0}", ex.Message)
        End Try
    End Function

End Class
