
Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Price_Surcharge_Default
    Inherits Page

    Dim settingCfg As New SettingConfig
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting/price", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)

            BindDesign()
            ddlDesignId.SelectedValue = Session("surchargeDesign")
            BindBlind(ddlDesignId.SelectedValue)
            ddlBlindId.SelectedValue = Session("surchargeBlind")
            ddlActive.SelectedValue = Session("surchargeActive")
            txtSearch.Text = Session("surchargeSearch")
            BindData(ddlDesignId.SelectedValue, ddlBlindId.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("surchargeSearch") = txtSearch.Text
        Session("surchargeDesign") = ddlDesignId.SelectedValue
        Session("surchargeBlind") = ddlBlindId.SelectedValue
        Session("surchargeActive") = ddlActive.SelectedValue

        Response.Redirect("~/setting/price/surcharge/add", False)
    End Sub

    Protected Sub ddlDesignId_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindBlind(ddlDesignId.SelectedValue)
        BindData(ddlDesignId.SelectedValue, ddlBlindId.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub ddlBlindId_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(ddlDesignId.SelectedValue, ddlBlindId.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub ddlActive_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(ddlDesignId.SelectedValue, ddlBlindId.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(ddlDesignId.SelectedValue, ddlBlindId.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(ddlDesignId.SelectedValue, ddlBlindId.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError(False, String.Empty)
                Try
                    Session("surchargeSearch") = txtSearch.Text
                    Session("surchargeDesign") = ddlDesignId.SelectedValue
                    Session("surchargeBlind") = ddlBlindId.SelectedValue
                    Session("surchargeActive") = ddlActive.SelectedValue
                    Session("surchargeDetail") = dataId

                    Response.Redirect("~/setting/price/surcharge/detail", False)
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = settingCfg.GetListData("SELECT * FROM Log_Prices WHERE DataId = '" + dataId + "' AND Type='PriceSurcharges' ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnSubmitCopy_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            lblId.Text = txtIdCopy.Text

            Dim thisId As String = String.Empty
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO PriceSurcharges OUTPUT INSERTED.Id SELECT NEWID(), DesignId, BlindId, BlindNumber, Name + ' - Copy', FieldName, Formula, Charge, Description, Active FROM PriceSurcharges WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    thisId = myCmd.ExecuteScalar().ToString()
                    thisConn.Close()
                End Using
            End Using

            Dim dataLog As Object() = {"PriceSurcharges", lblId.Text, Session("LoginId").ToString(), "Surcharges Createad | Duplicate from ID : " & lblId.Text}
            settingCfg.Log_Price(dataLog)

            Session("surchargeSearch") = txtSearch.Text
            Session("surchargeDesign") = ddlDesignId.SelectedValue
            Session("surchargeBlind") = ddlBlindId.SelectedValue
            Session("surchargeActive") = ddlActive.SelectedValue

            Response.Redirect("~/setting/price/surcharge", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnSubmitDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            lblId.Text = txtIdDelete.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM PriceSurcharges WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Session("surchargeSearch") = txtSearch.Text
            Session("surchargeDesign") = ddlDesignId.SelectedValue
            Session("surchargeBlind") = ddlBlindId.SelectedValue
            Session("surchargeActive") = ddlActive.SelectedValue

            Response.Redirect("~/setting/price/surcharge", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Session("surchargeSearch") = txtSearch.Text
        Session("surchargeDesign") = ddlDesignId.SelectedValue
        Session("surchargeBlind") = ddlBlindId.SelectedValue
        Session("surchargeActive") = ddlActive.SelectedValue

        Response.Redirect("~/setting/price/surcharge", False)
    End Sub

    Private Sub BindData(Design As String, Blind As String, Active As String, Search As String)
        Try
            Dim designString As String = ""
            Dim blindString As String = ""
            Dim activeString As String = " WHERE PriceSurcharges.Active = 1"
            Dim searchString As String = ""

            If Active = 0 Then
                activeString = " WHERE PriceSurcharges.Active = 0"
            End If

            If Not Design = "" Then
                designString = " AND PriceSurcharges.DesignId = '" + UCase(Design).ToString() + "'"
            End If

            If Not Blind = "" Then
                blindString = " AND PriceSurcharges.BlindId = '" + UCase(Blind).ToString() + "'"
            End If

            If Not Search = "" Then
                searchString = " AND (Designs.Name LIKE '%" + Search.Trim() + "%' OR Blinds.Name LIKE '%" + Search + "%' OR PriceSurcharges.Name LIKE '%" + Search.Trim() + "%' OR PriceSurcharges.FieldName LIKE '%" + Search.Trim() + "%' OR PriceSurcharges.Formula LIKE '%" + Search.Trim() + "%' OR PriceSurcharges.Charge LIKE '%" + Search + "%' OR PriceSurcharges.Description LIKE '%" + Search.Trim() + "%')"
            End If

            Dim thisQuery As String = String.Format("SELECT PriceSurcharges.*, Designs.Name + ' | ' + Blinds.Name AS Product FROM PriceSurcharges INNER JOIN Designs ON PriceSurcharges.DesignId = Designs.Id INNER JOIN Blinds ON PriceSurcharges.BlindId = Blinds.Id {0} {1} {2} {3} ORDER BY PriceSurcharges.FieldName, PriceSurcharges.Name, PriceSurcharges.BlindNumber ASC", activeString, designString, blindString, searchString)

            gvList.DataSource = settingCfg.GetListData(thisQuery)
            gvList.DataBind()

            btnAdd.Visible = False
            If Session("RoleName") = "Administrator" And Session("LevelName") = "Leader" Then btnAdd.Visible = True
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindDesign()
        ddlDesignId.Items.Clear()
        Try
            ddlDesignId.Items.Clear()
            ddlDesignId.DataSource = settingCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Designs WHERE Active = 1 ORDER BY Name ASC")
            ddlDesignId.DataTextField = "NameText"
            ddlDesignId.DataValueField = "Id"
            ddlDesignId.DataBind()

            ddlDesignId.Items.Insert(0, New ListItem("", ""))
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindBlind(DesignId As String)
        ddlBlindId.Items.Clear()
        Try
            If Not DesignId = "" Then
                ddlBlindId.Items.Clear()
                ddlBlindId.DataSource = settingCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Blinds WHERE DesignId='" + UCase(DesignId).ToString() + "' AND Active = 1 ORDER BY Name ASC")
                ddlBlindId.DataTextField = "NameText"
                ddlBlindId.DataValueField = "Id"
                ddlBlindId.DataBind()

                If ddlBlindId.Items.Count > 1 Then
                    ddlBlindId.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub

    Private Sub MessageError_Log(Show As Boolean, Msg As String)
        divErrorLog.Visible = Show : msgErrorLog.InnerText = Msg
    End Sub

    Protected Function BindTextLog(Id As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = settingCfg.GetListData("SELECT * FROM Log_Prices WHERE Id = '" + Id + "'")
            Dim actionBy As String = thisData.Tables(0).Rows(0).Item("ActionBy").ToString()
            Dim actionDate As String = Convert.ToDateTime(thisData.Tables(0).Rows(0).Item("ActionDate")).ToString("dd MMM yyyy hh:mm")
            Dim description As String = thisData.Tables(0).Rows(0).Item("Description").ToString()

            Dim fullName As String = settingCfg.GetItemData("SELECT FullName FROM CustomerLogins WHERE Id = '" + UCase(actionBy).ToString() + "'")

            result = "<b>" & fullName & "</b> on " & actionDate & ". Action: " & description
        Catch ex As Exception
            result = ""
        End Try
        Return result
    End Function

    Protected Function VisibleAction() As Boolean
        If Session("LevelName") = "Leader" OR Session("LevelName") = "Super Admin" Then
            Return True
        End If
        Return False
    End Function
End Class
