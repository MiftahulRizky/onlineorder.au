
Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Customer_Default
    Inherits Page

    Dim settingCfg As New SettingConfig
    Dim mailCfg As New MailConfig

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" And Not Session("RoleName") = "Customer Service" And Not Session("RoleName") = "Data Entry" And Not Session("RoleName") = "Representative" Then
            Response.Redirect("~/setting/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("customerSearch")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/customer", False)
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/customer/add", False)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "gvList_PageIndexChanging", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            If e.CommandName = "Detail" Then
                MessageError(False, String.Empty)
                Try
                    Dim dataId As String = e.CommandArgument.ToString()

                    Session("customerDetail") = dataId
                    Session("customerSearch") = txtSearch.Text

                    Response.Redirect("~/setting/customer/detail", False)
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                    If Not Session("RoleName") = "Administrator" Then
                        MessageError(True, "Please contact IT Support at reza@bigblinds.co.id !")
                        mailCfg.MailError(Page.Title, "linkDetail_Click", Session("LoginId"), ex.ToString())
                    End If
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Try
                    Dim headerId As String = e.CommandArgument.ToString()

                    gvListLogs.DataSource = settingCfg.GetListData("SELECT * FROM Log_Customers WHERE Type = 'Customers' AND DataId = '" + headerId + "'  ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()

                    Dim myScript As String = "window.onload = function() { showLog(); };"
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", myScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    If Not Session("RoleName") = "Administrator" Then
                        MessageError_Log(True, "Please contact IT Support at reza@bigblinds.co.id")
                        mailCfg.MailError(Page.Title, "linkLog_Click", Session("LoginId"), ex.ToString())
                    End If
                End Try
            End If
        End If
    End Sub

    Protected Sub btnSubmitAdd_Click(sender As Object, e As EventArgs)
        MessageError_Add(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showAdd(); };"
        Try
            If Not String.IsNullOrEmpty(txtId.Text) Then
                If txtId.Text.Length > 100 Then
                    MessageError_Add(True, "MAXIMUM CHARACTERS FOR DEBTOR CODE IS 100 !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showAdd", thisScript, True)
                    Exit Sub
                End If
                If txtId.Text = settingCfg.GetItemData("SELECT Id FROM Customers WHERE Id='" + txtId.Text + "'") Then
                    MessageError_Add(True, "ID / DEBTOR CODE ALREADY EXISTS !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showAdd", thisScript, True)
                    Exit Sub
                End If
            End If

            If Not String.IsNullOrEmpty(txtMicronetId.Text) Then
                Dim micronetId As String = settingCfg.GetItemData("SELECT MicronetId FROM Customers WHERE MicronetId = '" + txtMicronetId.Text.Trim() + "'")

                If Not String.IsNullOrEmpty(micronetId) Then
                    MessageError_Add(True, "MICRONET ID ALREADY EXISTS !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showAdd", thisScript, True)
                    Exit Sub
                End If
            End If

            If Not String.IsNullOrEmpty(txtExactId.Text) Then
                Dim exactId As String = settingCfg.GetItemData("SELECT ExactId FROM Customers WHERE ExactId = '" + txtExactId.Text.Trim() + "'")

                If Not String.IsNullOrEmpty(exactId) Then
                    MessageError_Add(True, "EXACT ID ALREADY EXISTS !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showAdd", thisScript, True)
                    Exit Sub
                End If
            End If

            If txtName.Text = "" Then
                MessageError_Add(True, "CUSTOMER NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showAdd", thisScript, True)
                Exit Sub
            End If

            If ddlType.SelectedValue = "" Then
                MessageError_Add(True, "CUSTOMER TYPE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showAdd", thisScript, True)
                Exit Sub
            End If

            If ddlSalesPerson.SelectedValue = "" Then
                MessageError_Add(True, "SALES PERSON IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showAdd", thisScript, True)
                Exit Sub
            End If

            If msgErrorAdd.InnerText = "" Then
                If txtId.Text = "" Then
                    Dim lastId As String = settingCfg.GetItemData("SELECT TOP 1 Id FROM (SELECT Id, TRY_CAST(SUBSTRING(Id, CHARINDEX('A', Id) + 1, LEN(Id)) AS INT) AS NumericValue FROM Customers) AS Subquery WHERE NumericValue IS NOT NULL ORDER BY NumericValue DESC;")
                    Dim pos As Integer = lastId.IndexOf("A")
                    Dim result As String = String.Empty

                    If pos <> -1 Then
                        Dim extracted As String = lastId.Substring(pos + 1)
                        Dim num As Integer
                        If Integer.TryParse(extracted, num) Then
                            num += 1
                            result = num.ToString()
                        Else
                            result = Now.ToString("ddMMyyhhmmss")
                        End If
                    Else
                        result = Now.ToString("ddMMyyhhmmss")
                    End If
                    txtId.Text = "LS-A" & result
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Customers VALUES (@Id, @MicronetId, @ExactId, @CompanyId, @Name, @Type, @Group, @PriceGroup, @SalesPerson, @OnStop, @CashSale, @Newsletter, @MininimuOrderSurcharge, 1)")
                        myCmd.Parameters.AddWithValue("@Id", txtId.Text.Trim())
                        myCmd.Parameters.AddWithValue("@MicronetId", txtMicronetId.Text.Trim())
                        myCmd.Parameters.AddWithValue("@ExactId", txtExactId.Text.Trim())
                        myCmd.Parameters.AddWithValue("@CompanyId", UCase(ddlCompany.SelectedValue).ToString())
                        myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Group", ddlGroup.SelectedValue)
                        myCmd.Parameters.AddWithValue("@PriceGroup", ddlPriceGroup.SelectedValue)
                        myCmd.Parameters.AddWithValue("@SalesPerson", ddlSalesPerson.SelectedValue)
                        myCmd.Parameters.AddWithValue("@OnStop", ddlOnStop.SelectedValue)
                        myCmd.Parameters.AddWithValue("@CashSale", ddlCashSale.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Newsletter", ddlNewsletter.SelectedValue)
                        myCmd.Parameters.AddWithValue("@MininimuOrderSurcharge", ddlMinimumOrderSurcharge.SelectedValue)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                ' CUSTOMER QUOTES
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerQuotes VALUES (@Id, 'LS.png', '')")
                        myCmd.Parameters.AddWithValue("@Id", txtId.Text.Trim())

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                ' CUSTOMER PRODUCT ACCESS
                Dim desingId As String = settingCfg.GetProductAccess()
                lblDesignId.Text = UCase(desingId).ToString()
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerProductAccess VALUES (@Id, @DesignId)")
                        myCmd.Parameters.AddWithValue("@Id", txtId.Text.Trim())
                        myCmd.Parameters.AddWithValue("@DesignId", lblDesignId.Text.Trim())

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                Dim dataLog As Object() = {"Customers", txtId.Text, Session("LoginId"), "Customer Created"}
                settingCfg.Log_Customer(dataLog)

                Session("customerDetail") = txtId.Text
                Response.Redirect("~/setting/customer/detail", False)
            End If
        Catch ex As Exception
            MessageError_Add(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_Add(True, "Please contact IT Support at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnSubmitAdd_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showAdd", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSubmitOnStop_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim newOnStop As Integer = 0
            Dim onStop As String = txtActive.Text
            If onStop = 0 Then : newOnStop = 1 : End If

            lblId.Text = txtIdActive.Text
            lblOnStop.Text = newOnStop

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE Customers SET OnStop=@OnStop WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                    myCmd.Parameters.AddWithValue("@OnStop", lblOnStop.Text)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Dim description As String = "Online Customer"
            If newOnStop = 1 Then description = "On Stop Customer"

            Dim dataLog As Object() = {"Customers", lblId.Text, Session("LoginId"), description}
            settingCfg.Log_Customer(dataLog)

            Session("customerSearch") = txtSearch.Text
            Response.Redirect("~/setting/customer", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnSubmitOnStop_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnSubmitDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            lblId.Text = txtIdDelete.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE Customers SET Active=0 WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Dim dataLog As Object() = {"Customers", lblId.Text, Session("LoginId"), "Delete Customer"}
            settingCfg.Log_Customer(dataLog)

            Session("customerSearch") = txtSearch.Text
            Response.Redirect("~/setting/customer", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnDelete_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindData(SearchText As String)
        Try
            Dim search As String = String.Empty
            If Not SearchText = "" Then
                search = " WHERE Customers.Id LIKE '%" + SearchText.Trim() + "%' OR Customers.MicronetId LIKE '%" + SearchText.Trim() + "%' OR Customers.ExactId LIKE '%" + SearchText.Trim() + "%'  OR Customers.Name LIKE '%" + SearchText.Trim() + "%' OR Customers.Type LIKE '%" + SearchText.Trim() + "%' OR CustomerGroups.Name LIKE '%" + SearchText.Trim() + "%'"
            End If

            Dim thisQuery As String = String.Format("SELECT Customers.*, CASE WHEN Customers.CashSale = 1 THEN 'Yes' ELSE 'No' END AS CustomerCashSale, CASE WHEN Customers.OnStop = 1 THEN 'Yes' ELSE 'No' END AS CustomerOnStop, CASE WHEN Customers.MinimumOrderSurcharge = 1 THEN 'Yes' ELSE 'No' END AS CustomerMinSurcharge, CustomerGroups.Name AS CustomerGroup, CASE WHEN Customers.Active = 1 THEN 'Yes' WHEN Customers.Active = 0 THEN 'No' ELSE 'Error' END AS DataActive FROM Customers LEFT JOIN CustomerGroups ON CustomerGroups.Id = Customers.[Group] {0} ORDER BY Customers.Name ASC", search)

            If Session("RoleName") = "Administrator" Then
                thisQuery = String.Format("SELECT Customers.*, CASE WHEN Customers.CashSale = 1 THEN 'Yes' ELSE 'No' END AS CustomerCashSale, CASE WHEN Customers.OnStop = 1 THEN 'Yes' ELSE 'No' END AS CustomerOnStop, CASE WHEN Customers.MinimumOrderSurcharge = 1 THEN 'Yes' ELSE 'No' END AS CustomerMinSurcharge, CustomerGroups.Name AS CustomerGroup, CASE WHEN Customers.Active = 1 THEN 'Yes' WHEN Customers.Active = 0 THEN 'No' ELSE 'Error' END AS DataActive FROM Customers LEFT JOIN CustomerGroups ON CustomerGroups.Id = Customers.[Group] {0} ORDER BY CASE WHEN Customers.Id = 'DEFAULT' THEN 0 ELSE 1 END, Customers.Name ASC", search)
            End If

            gvList.DataSource = settingCfg.GetListData(thisQuery)
            gvList.DataBind()

            gvList.Columns(1).Visible = False ' ID
            If Session("RoleName") = "Administrator" And Session("LevelName") = "Leader" Then
                gvList.Columns(1).Visible = True
            End If

            divDebtorCode.Visible = False
            divExact.Visible = False
            If Session("RoleName") = "Administrator" And (Session("LevelName") = "Leader" Or Session("LevelName") = "Member") Then
                divDebtorCode.Visible = True
                divExact.Visible = True
            End If

            If Session("RoleName") = "Customer Service" Then
                divExact.Visible = True
            End If

            btnAdd.Visible = False
            If Session("RoleName") = "Administrator" Or Session("RoleName") = "Customer Service" Or Session("RoleName") = "Data Entry" Then
                btnAdd.Visible = True
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "BindData", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindCompany()
        ddlCompany.Items.Clear()
        Try
            ddlCompany.DataSource = settingCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Companys ORDER BY Name ASC")
            ddlCompany.DataTextField = "NameText"
            ddlCompany.DataValueField = "Id"
            ddlCompany.DataBind()

            If ddlCompany.Items.Count > 0 Then
                ddlCompany.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "BindCompany", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindCustomerGroup()
        ddlGroup.Items.Clear()
        Try
            ddlGroup.DataSource = settingCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM CustomerGroups ORDER BY Id ASC")
            ddlGroup.DataTextField = "NameText"
            ddlGroup.DataValueField = "Id"
            ddlGroup.DataBind()

            If ddlGroup.Items.Count > 0 Then
                ddlGroup.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "BindCustomerGroup", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindCustomerPriceGroup()
        ddlPriceGroup.Items.Clear()
        Try
            ddlPriceGroup.DataSource = settingCfg.GetListData("SELECT *,  UPPER(Name) AS NameText FROM CustomerPriceGroups WHERE Active=1 ORDER BY Id ASC")
            ddlPriceGroup.DataTextField = "NameText"
            ddlPriceGroup.DataValueField = "Id"
            ddlPriceGroup.DataBind()

            If ddlPriceGroup.Items.Count > 0 Then
                ddlPriceGroup.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "BindCustomerPriceGroup", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Function VisibleDetail(Id As String) As Boolean
        If Session("RoleName") = "Administrator" Or Session("RoleName") = "Customer Service" Or Session("RoleName") = "Data Entry" Or Session("RoleName") = "Representative" Then
            If Id = "DEFAULT" Then
                If Session("RoleName") = "Administrator" Then
                    Return True
                End If
                Return False
            End If
            Return True
        End If
        Return False
    End Function

    Protected Function VisibleDelete(Id As String) As Boolean
        If Session("RoleName") = "Administrator" Or Session("RoleName") = "Customer Service" Or Session("RoleName") = "Data Entry" Then
            If Id = "DEFAULT" Then Return False
            Return True
        End If
        Return False
    End Function

    Protected Function VisibleOnStop(Id As String) As Boolean
        If Session("RoleName") = "Administrator" Or Session("RoleName") = "Customer Service" Or Session("RoleName") = "Data Entry" Then
            If Id = "DEFAULT" Then Return False
            Return True
        End If
        Return False
    End Function

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub

    Private Sub MessageError_Add(Show As Boolean, Msg As String)
        divErrorAdd.Visible = Show : msgErrorAdd.InnerText = Msg
    End Sub

    Private Sub MessageError_Log(Show As Boolean, Msg As String)
        divErrorLog.Visible = Show : msgErrorLog.InnerText = Msg
    End Sub

    Protected Function TextOnStop(Active As Boolean) As String
        Dim result As String = "Offline / On Stop"
        If Active = True Then : Return "Online" : End If
        Return result
    End Function

    Protected Function BindTextLog(Id As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = settingCfg.GetListData("SELECT * FROM Log_Customers WHERE Id = '" + Id + "'")

            Dim actionBy As String = thisData.Tables(0).Rows(0).Item("ActionBy").ToString()
            Dim actionDate As String = Convert.ToDateTime(thisData.Tables(0).Rows(0).Item("ActionDate")).ToString("dd MMM yyyy hh:mm")
            Dim description As String = thisData.Tables(0).Rows(0).Item("Description").ToString()

            Dim fullName As String = settingCfg.GetItemData("SELECT FullName FROM CustomerLogins WHERE Id = '" + UCase(actionBy).ToString() + "'")

            result = "<b>" & fullName & "</b> on " & actionDate & ". Action: " & description
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function
End Class
