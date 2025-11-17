
Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Customer_Edit
    Inherits Page

    Dim settingCfg As New SettingConfig
    Dim mailCfg As New MailConfig

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If String.IsNullOrEmpty(Session("customerEdit")) Then
            Response.Redirect("~/setting/customer", False)
            Exit Sub
        End If

        txtId.Text = Session("customerEdit")
        If Not IsPostBack Then
            BindData(txtId.Text)
        End If
    End Sub

    Protected Sub ddlAccount_SelectedIndexChanged(sender As Object, e As EventArgs)
        BackColor()
        BindComponentForm(ddlAccount.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        BackColor()
        Try
            If ddlAccount.SelectedValue = "" Then
                MessageError(True, "CUSTOMER NAME IS REQUIRED !")
                ddlAccount.BackColor = Drawing.Color.Red
                ddlAccount.Focus()
                Exit Sub
            End If

            If ddlAccount.SelectedValue = "Sub" Then
                If ddlMaster.SelectedValue = "" Then
                    MessageError(True, "MASTER ID IS REQUIRED !")
                    ddlAccount.BackColor = Drawing.Color.Red
                    ddlAccount.Focus()
                    Exit Sub
                End If
            End If

            If txtName.Text = "" Then
                MessageError(True, "CUSTOMER NAME IS REQUIRED !")
                txtName.BackColor = Drawing.Color.Red
                txtName.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                If ddlAccount.SelectedValue = "Master" Or ddlAccount.SelectedValue = "Regular" Then
                    ddlMaster.SelectedValue = ""
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE Customers SET Account=@Account, MasterId=@MasterId, ExactId=@ExactId, Name=@Name, Type='Shutters', [Group]=NULL, Pricing=@PriceGroup, SalesPerson='Office', OnStop=@OnStop, CashSale=@CashSale, Newsletter=@Newsletter, MinimumOrderSurcharge=@MinimumOrderSurcharge, Active=@Active WHERE Id=@Id")
                        myCmd.Parameters.AddWithValue("@Id", txtId.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Account", ddlAccount.SelectedValue)
                        myCmd.Parameters.AddWithValue("@MasterId", ddlMaster.SelectedValue)
                        myCmd.Parameters.AddWithValue("@ExactId", txtExactId.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@PriceGroup", ddlPriceGroup.SelectedValue)
                        myCmd.Parameters.AddWithValue("@OnStop", ddlOnStop.SelectedValue)
                        myCmd.Parameters.AddWithValue("@CashSale", ddlCashSale.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Newsletter", ddlNewsletter.SelectedValue)
                        myCmd.Parameters.AddWithValue("@MinimumOrderSurcharge", ddlMinimumOrderSurcharge.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                Dim dataLog As Object() = {"Customers", txtId.Text, Session("LoginId"), "Customers Updated"}
                settingCfg.Log_Customer(dataLog)

                Session("customerDetail") = txtId.Text
                Response.Redirect("~/setting/customer/detail", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnSubmit_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/customer/detail", False)
    End Sub

    Private Sub BindData(Id As String)
        Try
            Dim myData As DataSet = settingCfg.GetListData("SELECT * FROM Customers WHERE Id = '" + Id + "'")

            If myData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/setting/customer", False)
                Exit Sub
            End If

            BackColor()
            BindCustomerMaster()
            BindCustomerPriceGroup()

            ddlAccount.SelectedValue = myData.Tables(0).Rows(0).Item("Account").ToString()
            ddlMaster.SelectedValue = myData.Tables(0).Rows(0).Item("MasterId").ToString()
            txtExactId.Text = myData.Tables(0).Rows(0).Item("ExactId").ToString()
            txtName.Text = myData.Tables(0).Rows(0).Item("Name").ToString()
            ddlPriceGroup.SelectedValue = myData.Tables(0).Rows(0).Item("Pricing").ToString()
            ddlOnStop.SelectedValue = Convert.ToInt32(myData.Tables(0).Rows(0).Item("OnStop"))
            ddlCashSale.SelectedValue = Convert.ToInt32(myData.Tables(0).Rows(0).Item("CashSale"))
            ddlNewsletter.SelectedValue = Convert.ToInt32(myData.Tables(0).Rows(0).Item("Newsletter"))
            ddlMinimumOrderSurcharge.SelectedValue = Convert.ToInt32(myData.Tables(0).Rows(0).Item("MinimumOrderSurcharge"))

            BindComponentForm(ddlAccount.SelectedValue)
        Catch ex As Exception
            mailCfg.MailError(Page.Title, "BindData", Session("LoginId"), ex.ToString())
            Response.Redirect("~/setting/customer", False)
        End Try
    End Sub

    Private Sub BindCustomerMaster()
        ddlMaster.Items.Clear()
        Try
            ddlMaster.DataSource = settingCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Customers WHERE Account='Master' ORDER BY Id ASC")
            ddlMaster.DataTextField = "NameText"
            ddlMaster.DataValueField = "Id"
            ddlMaster.DataBind()

            If ddlMaster.Items.Count > 0 Then
                ddlMaster.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            mailCfg.MailError(Page.Title, "BindCustomerMaster", Session("LoginId"), ex.ToString())
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
            mailCfg.MailError(Page.Title, "BindCustomerPriceGroup", Session("LoginId"), ex.ToString())
        End Try
    End Sub

    Private Sub BindComponentForm(Account As String)
        BackColor()
        Try
            divMaster.Visible = False
            divInternal.Visible = False
            divExact.Visible = False

            If String.IsNullOrEmpty(Account) Then
                Exit Sub
            End If

            If Account = "Master" Or Account = "Regular" Then
                divInternal.Visible = True
                divExact.Visible = True
            End If

            If Account = "Sub" Then
                divMaster.Visible = True
            End If
        Catch ex As Exception
            mailCfg.MailError(Page.Title, "BindComponentForm", Session("LoginId"), ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        MessageError(False, "")

        ddlAccount.BackColor = Drawing.Color.Empty
        txtExactId.BackColor = Drawing.Color.Empty
        ddlMaster.BackColor = Drawing.Color.Empty
        txtName.BackColor = Drawing.Color.Empty
        ddlPriceGroup.BackColor = Drawing.Color.Empty
        ddlOnStop.BackColor = Drawing.Color.Empty
        ddlNewsletter.BackColor = Drawing.Color.Empty
        ddlCashSale.BackColor = Drawing.Color.Empty
        ddlMinimumOrderSurcharge.BackColor = Drawing.Color.Empty
        ddlActive.BackColor = Drawing.Color.Empty
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub
End Class
