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
Partial Class Methods_Order_OrderDetailMethod
    Inherits System.Web.UI.Page
    Shared orderCfg As New OrderConfig()
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

    Public Class ParamBindOrderHeaderByID
        Public Property headerid As String
        Public Property ordertype As String

        Public Property loginid As String
        Public Property rolename As String
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

    Private Shared Function InArray(value As String, ParamArray list() As String) As Boolean
        Return list.Contains(value)
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindOrderHeaderByID(ByVal data As ParamBindOrderHeaderByID) As Object
        Try
            Dim HeaderData As Object
            Dim DetailData As New List(Of Object)()
            Dim Status As String
            Dim CreatedByName As String

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
                    h.OrderNote, h.StatusAdditional, h.Status, h.Delivery,
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

                            Dim Id As String = reader("Id").ToString()
                            Status = reader("Status").ToString()
                            CreatedByName = reader("CreatedByName").ToString()
                            Dim ResCheckOrder As Object = CekOrder(Id, Status, data.loginid, data.rolename)

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
                                .CreatedByName = CreatedByName,
                                .OrderNote = reader("OrderNote").ToString(),
                                .StatusAdditional = reader("StatusAdditional").ToString(),
                                .Status = reader("Status").ToString(),
                                .Delivery = reader("Delivery").ToString(),
                                .SubmittedDate = reader("SubmittedDate").ToString(),
                                .JobDate = reader("JobDate").ToString(),
                                .CompletedDate = reader("CompletedDate").ToString(),
                                .CanceledDate = reader("CanceledDate").ToString(),
                                .SumPrice = sumPrice,
                                .Gst = gst,
                                .FinalTotal = finalTotal,
                                .ResCheckOrder = ResCheckOrder
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
                            Dim HideNext As String = "hidden"
                            Dim TextNext As String = "Add blind that is doubled to this blind"

                            Dim Product As String = FindProduct(reader)

                            Dim Cost As String = FindCost(reader)
                            Dim Markup As String = reader("Markup").ToString()
                            If MarkUp = "0" Then MarkUp = ""

                            DetailData.Add(New With {
                                .Id = reader("Id").ToString(),
                                .HeaderId = reader("HeaderId").ToString(),
                                .DesignId = reader("DesignId").ToString(),
                                .BlindId = reader("BlindId").ToString(),
                                .Qty = reader("Qty").ToString(),
                                .Location = reader("Location").ToString(),
                                .Mounting = reader("Mounting").ToString(),
                                .DesignName = reader("DesignName").ToString(),
                                .BlindName = reader("BlindName").ToString(),
                                .KitName = reader("KitName").ToString(),
                                .BracketType = reader("BracketType").ToString(),
                                .TubeType = reader("TubeType").ToString(),
                                .ControlType = reader("ControlType").ToString(),
                                .FabricType = reader("FabricType").ToString(),
                                .BlindNo = reader("BlindNo").ToString(),
                                .UniqueId = reader("UniqueId").ToString(),
                                .Width = reader("Width").ToString(),
                                .Drop = reader("Drop").ToString(),
                                .FrameColour = reader("FrameColour").ToString(),
                                .PanelSize = reader("PanelSize").ToString(),
                                .PelmetType = reader("PelmetType").ToString(),
                                .BottomTrackType = reader("BottomTrackType").ToString(),
                                .MeshType = reader("MeshType").ToString(),
                                .FrameType = reader("FrameType").ToString(),
                                .Cost = Cost,
                                .Product = Product,
                                .Charge = reader("Charge").ToString(),
                                .Discount = reader("Discount").ToString(),
                                .Markup = reader("Markup").ToString(),
                                .FabricGroups = reader("FabricGroups").ToString(),
                                .OrderDelivery = reader("OrderDelivery").ToString(),
                                .PriceGroupName = reader("PriceGroupName").ToString()
                            })
                        End While
                    End Using
                End Using
            End Using

            Return New With {
                .header = HeaderData,
                .detail = DetailData
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
            ' Dim RoleName As String = HttpContext.Current.Session("RoleName").ToString()
            Dim CustomerContactId As String = HttpContext.Current.Session("CustomerContactId").ToString()

            Dim detailData As DataSet = publicCfg.GetListData("SELECT Id, UniqueId, DesignName, BracketType, FabricGroups FROM view_details WHERE HeaderId='" + headerid + "' AND Active=1 ORDER BY Id ASC")
            If detailData.Tables(0).Rows.Count < 1  Then Return Nothing

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

                If rolename = "PPIC & DE" And Not loginid = CustomerContactId Then
                    Action = "No"
                End If

                textSwall = "You have an incomplete roller blinds order, which is on the ITEM ID "+ msg +" <br /><br />If you want to complete it, please click the <b>Next Item</b> button on the order line ID."
                
            Next

            Return New With {.Action = Action, .Message = textSwall}
        Catch ex As Exception
            Return New With {.error = ex.Message}
        End Try
    End Function

    Private Shared Function FindProduct(reader As SqlDataReader) As String
        Dim Id As String = reader("Id").ToString()
        Dim HeaderId As String = reader("HeaderId").ToString()
        Dim DesignId As String = reader("DesignId").ToString()
        Dim BlindId As String = reader("BlindId").ToString()
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


        Return Product
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
End Class
