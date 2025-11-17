
Imports System.Data
Imports System.Globalization

Partial Class Setting_Customer_Discount
    Inherits Page

    Dim settingCfg As New SettingConfig
    Dim mailCfg As New MailConfig

    Dim enUS As CultureInfo = New CultureInfo("en-US")

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" And Not Session("RoleName") = "Customer Service" And Not Session("RoleName") = "Data Entry" And Not Session("RoleName") = "Representative" Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not Session("selectedTabCustomer") = "" Then
            selected_tab.Value = Session("selectedTabCustomer").ToString()
        End If

        If Not IsPostBack Then
            BindDiscountProduct()
            BindDiscountFabric()
        End If
    End Sub

    Private Sub BindDiscountProduct()
        MessageError_Products(False, String.Empty)
        Try
            gvListProducts.DataSource = settingCfg.GetListData("SELECT CustomerDiscounts.DesignId, CustomerDiscounts.Discount, CustomerDiscounts.CustomerBy FROM CustomerDiscounts INNER JOIN Designs ON CustomerDiscounts.DesignId = Designs.Id WHERE CustomerDiscounts.DiscountType = 'Product' AND CustomerDiscounts.Active = 1 GROUP BY CustomerDiscounts.DesignId, CustomerDiscounts.Discount, CustomerDiscounts.CustomerBy, Designs.Name ORDER BY Designs.Name ASC")
            gvListProducts.DataBind()
        Catch ex As Exception
            MessageError_Products(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_Products(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "BindProduct", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindDiscountFabric()
        MessageError_Fabrics(False, String.Empty)
        Try
            Dim thisQuery As String = "SELECT CustomerDiscounts.*, Fabrics.Name AS FabricName, CASE WHEN CustomerDiscounts.FinalDiscount = 1 THEN 'Yes' WHEN CustomerDiscounts.FinalDiscount = 0 THEN 'No' ELSE 'Error' END AS DiscFinal, CASE WHEN CustomerDiscounts.Active = 1 THEN 'Yes' WHEN CustomerDiscounts.Active = 0 THEN 'No' ELSE 'Error' END AS DiscActive FROM CustomerDiscounts INNER JOIN Fabrics ON CustomerDiscounts.FabricId = Fabrics.Id WHERE CustomerDiscounts.DiscountType = 'Fabric' ORDER BY CustomerDiscounts.CustomerBy, CustomerDiscounts.CustomerData, Fabrics.Name ASC"
            gvLlistFabrics.DataSource = settingCfg.GetListData(thisQuery)
            gvLlistFabrics.DataBind()
        Catch ex As Exception
            MessageError_Fabrics(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_Fabrics(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "BindDiscountFabric", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Function DiscountDataProduct(Id As String, Disc As Decimal) As String
        Dim result As String = String.Empty
        Try
            Dim designName As String = settingCfg.GetItemData("SELECT Name FROM Designs WHERE Id = '" + UCase(Id).ToString() + "'")
            Dim formattedDisc As String = Disc.ToString("G29", enUS)
            result = designName & " " & formattedDisc & "%"
        Catch ex As Exception
            result = "Error"
        End Try
        Return result
    End Function

    Protected Function DiscountCustomerProduct(Id As String, Disc As Decimal, Cust As String) As String
        Dim result As String = String.Empty
        Try
            If Cust = "CustomerGroup" Then
                Dim custName As String = settingCfg.GetItemData("SELECT CustomerGroups.Name FROM CustomerDiscounts INNER JOIN CustomerGroups ON CustomerDiscounts.CustomerData = CustomerGroups.Id WHERE CustomerDiscounts.DesignId = '" + Id + "' AND CustomerDiscounts.Discount = " + Disc.ToString.Replace(",", ".") + " AND CustomerBy = 'CustomerGroup' AND CustomerDiscounts.DiscountType = 'Product'")

                result = "Customer Group : " & custName
            End If

            If Cust = "CustomerAccount" Then
                Dim custName As String = ""
                Dim dataCust As DataSet = settingCfg.GetListData("SELECT Customers.Name AS CustName FROM CustomerDiscounts INNER JOIN Customers ON CustomerDiscounts.CustomerData = Customers.Id WHERE CustomerDiscounts.DesignId = '" + Id + "' AND CustomerDiscounts.Discount = " + Disc.ToString.Replace(",", ".") + " AND CustomerBy = 'CustomerAccount' AND CustomerDiscounts.DiscountType = 'Product'")
                If dataCust.Tables(0).Rows.Count > 0 Then
                    For i As Integer = 0 To dataCust.Tables(0).Rows.Count - 1
                        Dim name As String = dataCust.Tables(0).Rows(i).Item(0).ToString()
                        custName += name.ToString() & ", "
                    Next
                End If
                result = custName.Remove(custName.Length - 2).ToString()
            End If
        Catch ex As Exception
            result = ex.ToString()
        End Try
        Return result
    End Function

    Protected Function DiscountCustomerFabric(CustomerBy As String, CustomerData As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisQuery As String = "SELECT Name FROM Customers WHERE Id = '" + CustomerData + "'"
            If CustomerBy = "CustomerGroup" Then
                thisQuery = "SELECT Name FROM CustomerGroups WHERE Id = '" + CustomerData + "'"
            End If
            Dim name As String = settingCfg.GetItemData(thisQuery)
            result = name
            If CustomerBy = "CustomerGroup" Then
                result = "Customer Group : " & name
            End If
        Catch ex As Exception
            result = "Error"
        End Try
        Return result
    End Function

    Protected Function TeksDiscount(disc As Decimal) As String
        Dim result As String = String.Empty

        If disc > 0 Then
            result = disc.ToString("0.##") & "%"
            If disc = Math.Truncate(disc) Then
                result = disc.ToString("0") & "%"
            End If
        End If
        Return result
    End Function

    Private Sub MessageError_Products(Show As Boolean, Msg As String)
        divErrorProducts.Visible = Show : msgErrorProducts.InnerText = Msg
    End Sub

    Private Sub MessageError_Fabrics(Show As Boolean, Msg As String)
        divErrorFabrics.Visible = Show : msgErrorFabrics.InnerText = Msg
    End Sub
End Class
